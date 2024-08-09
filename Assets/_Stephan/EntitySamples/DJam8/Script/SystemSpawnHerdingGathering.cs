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
    public partial struct SystemSpawnHerdingGathering : ISystem
    {
        private EntityQuery _boidQuery;
        public void OnCreate(ref SystemState state)
        {
            _boidQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<SharedComponentHerdingSettings, LocalTransform>().Build(ref state);
        }

        public void OnUpdate(ref SystemState state)
        {
            var localToWorldLookup = SystemAPI.GetComponentLookup<LocalToWorld>();
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var world = state.World.Unmanaged;

            foreach (var (boidSchool, boidSchoolLocalToWorld, entity) in
                     SystemAPI.Query<RefRO<ComponentHerdingGathering>, RefRO<LocalToWorld>>()
                         .WithEntityAccess())
            {
                var boidEntities =
                    CollectionHelper.CreateNativeArray<Entity, RewindableAllocator>(boidSchool.ValueRO.Count,
                        ref world.UpdateAllocator);

                state.EntityManager.Instantiate(boidSchool.ValueRO.Prefab, boidEntities);

                var setBoidLocalToWorldJob = new SetHerdingLocalToWorld
                {
                    LocalToWorldFromEntity = localToWorldLookup,
                    Entities = boidEntities,
                    Center = boidSchoolLocalToWorld.ValueRO.Position,
                    Radius = boidSchool.ValueRO.InitialRadius
                };
                state.Dependency = setBoidLocalToWorldJob.Schedule(boidSchool.ValueRO.Count, 64, state.Dependency);
                state.Dependency.Complete();

                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
            // TODO: all Prefabs are currently forced to TransformUsageFlags.Dynamic by default, which means boids get a LocalTransform
            // they don't need. As a workaround, remove the component at spawn-time.
            state.EntityManager.RemoveComponent<LocalTransform>(_boidQuery);
        }
    }

    [BurstCompile]
    struct SetHerdingLocalToWorld : IJobParallelFor
    {
        [NativeDisableContainerSafetyRestriction]
        [NativeDisableParallelForRestriction]
        public ComponentLookup<LocalToWorld> LocalToWorldFromEntity;

        public NativeArray<Entity> Entities;
        public float3 Center;
        public float Radius;

        public void Execute(int i)
        {
            var entity = Entities[i];
            var random = new Random(((uint)(entity.Index + i + 1) * 0x9F6ABC1));
            var angle = random.NextFloat() * 2 * math.PI; // Generate a random angle between 0 and 2π
            var radius = random.NextFloat() * Radius; // Generate a random radius within the disk's radius
            var dir = new float3(math.cos(angle), 0, math.sin(angle)); // Calculate the direction vector on the disk
            var pos = Center + (dir * radius); // Calculate the position on the disk
            var localToWorld = new LocalToWorld
            {
                Value = float4x4.TRS(pos, quaternion.LookRotationSafe(dir, math.up()), new float3(1.0f, 1.0f, 1.0f))
            };
            LocalToWorldFromEntity[entity] = localToWorld;
        }
    }
}
