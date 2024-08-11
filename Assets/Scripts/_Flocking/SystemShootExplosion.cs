using Flocking;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct SystemShootExplosion : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ComponentFlockingLife>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var flockingLifeQuery = state.GetEntityQuery(ComponentType.ReadWrite<ComponentFlockingLife>(), ComponentType.ReadWrite<LocalToWorld>());

        var entities = flockingLifeQuery.ToEntityArray(Allocator.Temp);
        var flockingLifes = flockingLifeQuery.ToComponentDataArray<ComponentFlockingLife>(state.WorldUpdateAllocator);
        var localToWorlds = flockingLifeQuery.ToComponentDataArray<LocalToWorld>(state.WorldUpdateAllocator);

        var markedForDeathEntities = new NativeArray<Entity>(entities.Length, Allocator.Temp);

        var isMarkedforDeathJob = new IsMarkedForDeathJob
        {
            Entities = entities,
            FlockingLifes = flockingLifes,
            MarkedForDeathEntities = markedForDeathEntities
        };
        isMarkedforDeathJob.Schedule(entities.Length, 64).Complete();

        var explosionJob = new ExplosionAtFlockJob
        {
            Entities = entities,
            LocalToWorlds = localToWorlds,
            MarkedForDeathEntities = markedForDeathEntities
        };
        explosionJob.Schedule(markedForDeathEntities.Length, 64).Complete();

        markedForDeathEntities.Dispose();
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