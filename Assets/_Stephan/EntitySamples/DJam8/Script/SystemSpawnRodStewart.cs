using Flocking;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Flocking
{
    [RequireMatchingQueriesForUpdate]
    [BurstCompile]
    public partial struct SystemRodSpawner : ISystem
    {
        private EntityQuery _rodQuery;
        public void OnCreate(ref SystemState state)
        {
            _rodQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<ComponentRodStewartPieceOfEight, LocalTransform>().Build(ref state);
        }

        public void OnUpdate(ref SystemState state)
        {
            var localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var world = state.World.Unmanaged;

            foreach (var (rodComponent, entity) in
                     SystemAPI.Query<RefRO<ComponentRodStewartPieceOfEight>>()
                         .WithEntityAccess())
            {
                var rodEntities =
                    CollectionHelper.CreateNativeArray<Entity, RewindableAllocator>(rodComponent.ValueRO.NumSegments,
                        ref world.UpdateAllocator);

                // Buffer the instantiation of entities
                ecb.Instantiate(rodComponent.ValueRO.Prefab, rodEntities);

                var setRodLocalToWorldJob = new SetRodLocalToWorld
                {
                    LocalToWorldFromEntity = localToWorldLookup,
                    Entities = rodEntities,
                    RodLength = rodComponent.ValueRO.SegmentLength * rodComponent.ValueRO.NumSegments,
                    RodRadius = rodComponent.ValueRO.RodRadius,
                    SegmentLength = rodComponent.ValueRO.SegmentLength
                };
                state.Dependency = setRodLocalToWorldJob.Schedule(rodComponent.ValueRO.NumSegments, 64, state.Dependency);
                state.Dependency.Complete();
            }

            // Play back the entity command buffer
            ecb.Playback(state.EntityManager);
            state.EntityManager.RemoveComponent<LocalTransform>(_rodQuery);
        }

        //public void OnUpdate(ref SystemState state)
        //{
        //    var localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>();
        //    var ecb = new EntityCommandBuffer(Allocator.Temp);
        //    var world = state.World.Unmanaged;

        //    foreach (var (rodComponent, entity) in
        //             SystemAPI.Query<RefRO<ComponentRodStewartPieceOfEight>>()
        //             .WithEntityAccess())
        //    {
        //        var rodEntities =
        //            CollectionHelper.CreateNativeArray<Entity, RewindableAllocator>(rodComponent.ValueRO.NumSegments,
        //                ref world.UpdateAllocator);

        //        ecb.Instantiate(rodComponent.ValueRO.Prefab, rodEntities);

        //        var setRodLocalToWorldJob = new SetRodLocalToWorld
        //        {
        //            LocalToWorldFromEntity = localToWorldLookup,
        //            Entities = rodEntities,
        //            RodLength = rodComponent.ValueRO.SegmentLength * rodComponent.ValueRO.NumSegments,
        //            RodRadius = rodComponent.ValueRO.RodRadius,
        //            SegmentLength = rodComponent.ValueRO.SegmentLength
        //        };
        //        state.Dependency = setRodLocalToWorldJob.Schedule(rodComponent.ValueRO.NumSegments, 64, state.Dependency);
        //        state.Dependency.Complete();
        //    }

        //    ecb.Playback(state.EntityManager);
        //}
    }

    [BurstCompile]
    struct SetRodLocalToWorld : IJobParallelFor
    {
        [NativeDisableContainerSafetyRestriction]
        [NativeDisableParallelForRestriction]
        public ComponentLookup<LocalToWorld> LocalToWorldFromEntity;

        public NativeArray<Entity> Entities;
        public float RodLength;
        public float RodRadius;
        public float SegmentLength;

        public void Execute(int i)
        {
            var entity = Entities[i];
            var segmentPosition = new float3(0, i * SegmentLength, 0);
            var segmentRotation = quaternion.identity;

            // Calculate the rotation of the segment based on its position in the rod
            var rotationAngle = (i / (float)Entities.Length) * math.PI * 2;
            segmentRotation = quaternion.RotateY(rotationAngle);

            // Calculate the position of the segment based on its rotation and the rod's radius
            segmentPosition = math.mul(segmentRotation, new float3(RodRadius, 0, 0)) + new float3(0, i * SegmentLength, 0);

            var localToWorld = new LocalToWorld
            {
                Value = float4x4.TRS(segmentPosition, segmentRotation, new float3(1.0f, 1.0f, 1.0f))
            };
            LocalToWorldFromEntity[entity] = localToWorld;
        }
    }
}