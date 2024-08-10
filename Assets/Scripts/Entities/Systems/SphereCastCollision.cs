using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public partial struct ISystemSphereCast : ISystem
{
    public CollisionWorld world;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TagFlyingCube>();
    }

    public unsafe Entity SphereCast(float3 RayFrom, float3 RayTo, float radius)
    {
        // create builder, make query for collision world
        // then dispose query
        EntityQueryBuilder builder = new EntityQueryBuilder(Allocator.Temp).WithAll<PhysicsWorldSingleton>();
        EntityQuery query = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(builder);

        CollisionWorld collisionWorld = query.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

        query.Dispose();

        CollisionFilter filter = new CollisionFilter()
        {
            BelongsTo = ~0u,
            CollidesWith = ~0u,
            GroupIndex = 0
        };

        // we create our primitive for sphere casting
        SphereGeometry sphere = new SphereGeometry() { Center = float3.zero, Radius = radius };
        BlobAssetReference<Unity.Physics.Collider> sphereCollider = Unity.Physics.SphereCollider.Create(sphere, filter);

        // setting cast input
        ColliderCastInput input = new ColliderCastInput()
        {
            Collider = (Unity.Physics.Collider*)sphereCollider.GetUnsafePtr(),
            Orientation = quaternion.identity,
            Start = RayFrom,
            End = RayTo,
        };

        // setting hit
        ColliderCastHit hit = new ColliderCastHit();
        bool haveHit = collisionWorld.CastCollider(input, out hit);
        if (haveHit)
            return hit.Entity;

        sphereCollider.Dispose();

        return Entity.Null;
    }
}
