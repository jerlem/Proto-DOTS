using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Collections;
using System;
using Flocking;
using UnityEngine;
using Unity.Entities.UniversalDelegates;
using Unity.Transforms;

partial struct SystemDebugShootAtFlock : ISystem
{
    private double _nextShotTime;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _nextShotTime = SystemAPI.Time.ElapsedTime + 10f;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //float dt = math.min(0.05f, SystemAPI.Time.DeltaTime);
        var currentTime = SystemAPI.Time.ElapsedTime;
        if (currentTime >= _nextShotTime)
        {
            _nextShotTime = currentTime + 10f;

            // Get the PhysicsWorld from the system state
            var physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var physicsWorld = physicsWorldSingleton.PhysicsWorld;

            // Create a query to find all entities with ComponentShotFire
            var shotFireQuery = state.GetEntityQuery(ComponentType.ReadWrite<ComponentShotFire>());
            var shotFireEntities = shotFireQuery.ToEntityArray(state.WorldUpdateAllocator);
            var shotFires = shotFireQuery.ToComponentDataArray<ComponentShotFire>(state.WorldUpdateAllocator);

            // Create a query to find all entities with ComponentFlockingLife
            var flockingLifeQuery = state.GetEntityQuery(ComponentType.ReadWrite<ComponentFlockingLife>(), ComponentType.ReadWrite<LocalToWorld>());
            var flockingLifeEntities = flockingLifeQuery.ToEntityArray(state.WorldUpdateAllocator);
            var flockingLifes = flockingLifeQuery.ToComponentDataArray<ComponentFlockingLife>(state.WorldUpdateAllocator);
            var localToWorlds = flockingLifeQuery.ToComponentDataArray<LocalToWorld>(state.WorldUpdateAllocator);

            if (shotFireEntities.Length > 0 && flockingLifeEntities.Length > 0)
            {
                for (int i = 0; i < shotFireEntities.Length; i++)
                {
                    var random = new Unity.Mathematics.Random(((uint)(shotFireEntities[i].Index + i + 1) * 0x9F6ABC1));
                    int shotFireIndex = random.NextInt(0, shotFireEntities.Length);
                    var shotFireEntity = shotFireEntities[shotFireIndex];
                    var shotFire = shotFires[shotFireIndex];

                    random = new Unity.Mathematics.Random(((uint)(flockingLifeEntities[i].Index + i + 1) * 0x9F6ABC1));
                    int flockingLifeIndex = random.NextInt(0, flockingLifeEntities.Length);
                    var flockingLifeEntity = flockingLifeEntities[flockingLifeIndex];
                    var flockingLife = flockingLifes[flockingLifeIndex];
                    var localToWorld = localToWorlds[flockingLifeIndex];

                    // Create a raycast input
                    var raycastInput = new RaycastInput
                    {
                        Start = shotFire.Origin,
                        End = localToWorld.Position,
                        Filter = CollisionFilter.Default
                    };

                    // Perform the raycast
                    if (physicsWorld.CastRay(raycastInput, out var hit))
                    {
                        // Set BIsMarkedForDeath to true
                        flockingLife.BIsMarkedForDeath = true;

                        // Update the component
                        flockingLifes[flockingLifeIndex] = flockingLife;
                    }
                }
            }
        }
    }
}
