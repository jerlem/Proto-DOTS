using Flocking;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct SystemShootAtFlock : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // Get the PhysicsWorld from the system state
        state.RequireForUpdate<PhysicsWorld>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Get the PhysicsWorld from the system state
        var physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        var physicsWorld = physicsWorldSingleton.PhysicsWorld;

        // Create a query to find all entities with ComponentShotFire
        var shotFireQuery = state.GetEntityQuery(ComponentType.ReadWrite<ComponentShotFire>());
        var shotFireEntities = shotFireQuery.ToEntityArray(state.WorldUpdateAllocator);
        var shotFires = shotFireQuery.ToComponentDataArray<ComponentShotFire>(state.WorldUpdateAllocator);
        // Create a query to find all entities with ComponentFlockingLife
        var flockingLifeQuery = state.GetEntityQuery(ComponentType.ReadWrite<ComponentFlockingLife>());
        var flockingLifeEntities = flockingLifeQuery.ToEntityArray(state.WorldUpdateAllocator);
        var flockingLifes = flockingLifeQuery.ToComponentDataArray<ComponentFlockingLife>(state.WorldUpdateAllocator);

        // Create a job to process the entities
        var shootAtFlockJob = new ShootAtFlockJob
        {
            PhysicsWorld = physicsWorld,
            ShotFireEntities = shotFireEntities,
            ShotFires = shotFires,
            FlockingLifeEntities = flockingLifeEntities,
            FlockingLifes = flockingLifes,
        }.Schedule(shotFireEntities.Length, 1);
        state.Dependency = shootAtFlockJob;
    }

    [BurstCompile]
    struct ShootAtFlockJob : IJobParallelFor
    {
        public PhysicsWorld PhysicsWorld;
        [ReadOnly] public NativeArray<Entity> ShotFireEntities;
        [ReadOnly] public NativeArray<ComponentShotFire> ShotFires;
        [ReadOnly] public NativeArray<Entity> FlockingLifeEntities;
        public NativeArray<ComponentFlockingLife> FlockingLifes;

        public void Execute(int index)
        {
            var shotFireEntity = ShotFireEntities[index];
            var shotFire = ShotFires[index];

            // Create a raycast input
            var raycastInput = new RaycastInput
            {
                Start = shotFire.Origin,
                End = shotFire.Destination,
                Filter = CollisionFilter.Default
            };

            // Perform the raycast
            if (PhysicsWorld.CastRay(raycastInput, out var hit))
            {
                // Get the entity that was hit
                var hitEntity = hit.Entity;

                // Find the index of the hit entity in the FlockingLifeEntities array
                int flockingLifeIndex = -1;
                for (int i = 0; i < FlockingLifeEntities.Length; i++)
                {
                    if (FlockingLifeEntities[i] == hitEntity)
                    {
                        flockingLifeIndex = i;
                        break;
                    }
                }

                if (flockingLifeIndex != -1)
                {
                    // Set BIsMarkedForDeath to true
                    var flockingLife = FlockingLifes[flockingLifeIndex];
                    flockingLife.BIsMarkedForDeath = true;

                    // Update the component
                    FlockingLifes[flockingLifeIndex] = flockingLife;
                }
            }
        }
    }
}
