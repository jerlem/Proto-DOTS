using Flocking;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct SystemShootExplosion : ISystem
{

    private const float explosionRange = 5f;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ComponentFlockingLife>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var flockingLifeQuery = state.GetEntityQuery(ComponentType.ReadWrite<ComponentFlockingLife>(), ComponentType.ReadWrite<LocalToWorld>());

        NativeArray<Entity> entities = flockingLifeQuery.ToEntityArray(Allocator.Temp);
        var flockingLifes = flockingLifeQuery.ToComponentDataArray<ComponentFlockingLife>(state.WorldUpdateAllocator);
        var localToWorlds = flockingLifeQuery.ToComponentDataArray<LocalToWorld>(state.WorldUpdateAllocator);

        var flockingLifeEntities = new NativeArray<Entity>(entities.Length, Allocator.Temp);

        int i = 0;
        foreach (var e in flockingLifes)
        {
            if (e.BIsMarkedForDeath)
                flockingLifeEntities[i] = entities[i];
            else
                flockingLifeEntities[i] = Entity.Null;

            i++;
        }

        // trigger marked for death on an entity
        // then destroy other entities in an Area
        int triggeredIndex = 0;
        foreach (Entity triggeredEntity in flockingLifeEntities)
        {
            float3 triggeredEntityPosition = localToWorlds[triggeredIndex].Position;

            int max = 0;
            foreach (var (localToWorld, flockingLife, entity) in
                    SystemAPI.Query<RefRO<LocalToWorld>, RefRO<ComponentFlockingLife>>().WithEntityAccess())
            {
                float dist = math.distance(triggeredEntityPosition, localToWorld.ValueRO.Position);
                if (dist < explosionRange)
                    max++;
            }

            var finalDestroy = new NativeArray<Entity>(max, Allocator.Temp);

            foreach (var (localToWorld, flockingLife, entity) in
                    SystemAPI.Query<RefRO<LocalToWorld>, RefRO<ComponentFlockingLife>>().WithEntityAccess())
            {
                float dist = math.distance(triggeredEntityPosition, localToWorld.ValueRO.Position);

                if (dist < explosionRange)
                {
                    UnityEngine.Debug.Log($"DestroyEntity at {localToWorld.ValueRO.Position}");
                    finalDestroy[i] = entity;

                    state.EntityManager.DestroyEntity(finalDestroy);
                }
            }


        }


        var isMarkedforDeathJob = new IsMarkedForDeathJob
        {
            Entities = entities,
            FlockingLifes = flockingLifes,
            MarkedForDeathEntities = flockingLifeEntities
        };

        isMarkedforDeathJob.Schedule(entities.Length, 64).Complete();

        var explosionJob = new ExplosionAtFlockJob
        {
            Entities = entities,
            LocalToWorlds = localToWorlds,
            MarkedForDeathEntities = flockingLifeEntities
        };
        explosionJob.Schedule(flockingLifeEntities.Length, 64).Complete();

        flockingLifeEntities.Dispose();
    }

    [BurstCompile]
    struct IsMarkedForDeathJob : IJobParallelFor
    {
        public NativeArray<Entity> Entities;
        public NativeArray<ComponentFlockingLife> FlockingLifes;
        public NativeArray<Entity> MarkedForDeathEntities;

        public void Execute(int index)
        {
            if (FlockingLifes[index].BIsMarkedForDeath)
            {
                MarkedForDeathEntities[index] = Entities[index];
            }
            else
            {
                MarkedForDeathEntities[index] = Entity.Null;
            }
        }
    }

    [BurstCompile]
    struct ExplosionAtFlockJob : IJobParallelFor
    {
        public NativeArray<Entity> Entities;
        public NativeArray<LocalToWorld> LocalToWorlds;
        public NativeArray<Entity> MarkedForDeathEntities;

        public void Execute(int index)
        {
            var markedForDeathEntity = MarkedForDeathEntities[index];
            if (markedForDeathEntity != Entity.Null)
            {
                var markedForDeathPosition = LocalToWorlds[index].Position;

                for (int i = 0; i < Entities.Length; i++)
                {
                    var otherEntity = Entities[i];
                    if (otherEntity != markedForDeathEntity)
                    {
                        var otherPosition = LocalToWorlds[i].Position;
                        var distance = math.distance(markedForDeathPosition, otherPosition);

                        if (distance <= 5f)
                        {
                            // TODO Destroy entity
                            UnityEngine.Debug.Log($"DestroyEntity({otherEntity})");
                            //EntityCommandBuffer.DestroyEntity(otherEntity);
                        }
                    }
                }
                // TODO Destroy entity
                UnityEngine.Debug.Log($"DestroyEntity({markedForDeathEntity})");
                //EntityCommandBuffer.DestroyEntity(index, markedForDeathEntity);
            }
        }
    }
}