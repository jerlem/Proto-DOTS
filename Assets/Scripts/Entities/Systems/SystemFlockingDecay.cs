using Flocking;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace FlockingDecay
{
    public partial struct SystemFlockingDecay : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            EntityQuery rodQuery = state.GetEntityQuery(ComponentType.ReadWrite<ComponentRodLife>(), ComponentType.ReadWrite <LocalToWorld> ());
            //Unity.Collections.NativeArray<Entity> rodEntities = rodQuery.ToEntityArray(state.WorldUpdateAllocator);
            Unity.Collections.NativeArray<LocalToWorld> localToWorldRod = rodQuery.ToComponentDataArray<LocalToWorld>(state.WorldUpdateAllocator);
            Unity.Collections.NativeArray<ComponentRodLife> componentRodLife = rodQuery.ToComponentDataArray<ComponentRodLife>(state.WorldUpdateAllocator);

            new FlockingDecayJob
            {
                RodQuery = rodQuery,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                //RodEntities = rodEntities,
                LocalToWorldRod = localToWorldRod,
                //ComponentRodLife = componentRodLife
            }.ScheduleParallel();
        }
    }

    [WithAll(typeof(ComponentFlockingLife))]
    [BurstCompile]
    public partial struct FlockingDecayJob : IJobEntity
    {
        public EntityQuery RodQuery;
        public EntityCommandBuffer.ParallelWriter ECB;
        //public Unity.Collections.NativeArray<Entity> RodEntities;
        public Unity.Collections.NativeArray<LocalToWorld> LocalToWorldRod;
        //public Unity.Collections.NativeArray<ComponentRodLife> ComponentRodLife;

        void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref ComponentFlockingLife flockingLife, in LocalToWorld localToWorld)
        {
            int iForeach = 0;
            // foreach (var rodEntity in RodEntities)
            foreach (var (rodLife, transform) in RodQuery/*SystemAPI.Query<RefRW<ComponentRodLife>, RefRW<LocalToWorld>>()*/)
            {
                var rodLocalToWorld = LocalToWorldRod[iForeach];
                float3 rodPosition = rodLocalToWorld.Position;
                float3 entityPosition = localToWorld.Position;
                float3 entityDirection = localToWorld.Up;

                float height = math.abs(entityPosition.z - rodPosition.z);
                float radius = math.length(entityPosition.xy - rodPosition.xy);

                if (height < 200 && radius < 10)
                {
                    //ComponentRodLife[iForeach].RodLife -= 0.1f;
                }
                iForeach++;
            }
        }
    }
    //public partial struct SystemFlockingDecay : ISystem
    //{
    //    [BurstCompile]
    //    public void OnCreate(ref SystemState state)
    //    {
    //    }

    //    [BurstCompile]
    //    public void OnUpdate(ref SystemState state)
    //    {
    //        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
    //        var rodQuery = state.GetEntityQuery(ComponentType.ReadWrite<ComponentRodLife>());
    //        Unity.Collections.NativeArray<Entity> rodEntities = rodQuery.ToEntityArray(state.WorldUpdateAllocator);

    //        new FlockingDecayJob
    //        {
    //            ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
    //            RodEntities = rodEntities,
    //        }.ScheduleParallel();
    //    }
    //}

    //[WithAll(typeof(ComponentFlockingLife))]
    //[BurstCompile]
    //public partial struct FlockingDecayJob : IJobEntity
    //{
    //    public EntityCommandBuffer.ParallelWriter ECB;
    //    public Unity.Collections.NativeArray<Entity> RodEntities;

    //    void Execute([ChunkIndexInQuery] int chunkIndex, Entity entity, ref ComponentFlockingLife flockingLife, in PhysicsCollider collider)
    //    {
    //        foreach (var rodEntity in RodEntities)
    //        {
    //            if (Physics.CheckSphere(entity, rodEntity, 0.1f, out _))
    //            {
    //                ECB.SetComponent(rodEntity, new ComponentRodLife { RodLife = ECB.GetComponent<ComponentRodLife>(rodEntity).RodLife - 0.1f });
    //            }
    //        }
    //    }
    //}
}