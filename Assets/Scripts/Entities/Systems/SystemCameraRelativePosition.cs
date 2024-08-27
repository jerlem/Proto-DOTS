using Flocking;
using FPSTemplate;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

[BurstCompile]
public partial class SystemUpdateCameraRelativePosition : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<ComponentCameraRelativePosition>();
    }

    [BurstCompile]
    protected override void OnUpdate()
    {
        if (!GameManager.PlayerController.isFiring)
            return;

        float3 cameraPosition = (float3)Camera.main.transform.TransformPoint(Camera.main.transform.localPosition);
        float3 cameraForward = (float3)Camera.main.transform.TransformDirection(Camera.main.transform.localRotation * Vector3.forward);

        // Update all entities with the ComponentCameraRelativePosition component
        foreach (var positionRef in SystemAPI.Query<RefRW<ComponentCameraRelativePosition>>())
        {
            positionRef.ValueRW.Position = cameraPosition;
        }

        var t = RayCast(cameraPosition, cameraForward);
    }

    public Entity RayCast(float3 rayFrom, float3 rayTo)
    {
        EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<PhysicsWorldSingleton>();
        EntityQuery singletonQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(builder);

        var collisionWorld = singletonQuery.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        singletonQuery.Dispose();

        RaycastInput input = new RaycastInput()
        {
            Start = rayFrom,
            End = rayTo,
            Filter = new CollisionFilter()
            {
                BelongsTo = ~0u,
                CollidesWith = ~0u,
                GroupIndex = 0
            }
        };

        Unity.Physics.RaycastHit hit = new Unity.Physics.RaycastHit();

        bool haveHit = collisionWorld.CastRay(input, out hit);
        if (haveHit)
        {
            // check if flockinglife for mark
            Debug.Log("something hitted");

            return hit.Entity;
        }
        return Entity.Null;

        // Set BIsMarkedForDeath to true
        //var flockingLife = FlockingLifes[flockingLifeIndex];
        //flockingLife.BIsMarkedForDeath = true;

    }
}


