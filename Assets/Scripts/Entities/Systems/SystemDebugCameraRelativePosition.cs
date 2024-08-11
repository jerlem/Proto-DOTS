using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

[BurstCompile]
partial struct SystemDebugCameraRelativePosition : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ComponentCameraRelativePosition>();
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var query = state.GetEntityQuery(ComponentType.ReadOnly<ComponentCameraRelativePosition>());
        var cameraRelativePosition = query.ToComponentDataArray<ComponentCameraRelativePosition>(state.WorldUpdateAllocator);

        var sphereQuery = SystemAPI.QueryBuilder()
            .WithAll<ComponentDebugSphere>()
            .WithAll<LocalToWorld>()
            .Build();


        float3 camPosition = float3.zero;
        foreach (var p in cameraRelativePosition)
        {
            camPosition = p.Position;
            break;
        }

        foreach (var (transform, debugSphere) in
           SystemAPI.Query<RefRW<LocalToWorld>, RefRW<ComponentDebugSphere>>())
        {
            var localToWorld = new LocalToWorld
            {
                Value = float4x4.TRS(
                         new float3(camPosition),
                         quaternion.identity,
                         new float3(1.0f, 1.0f, 1.0f))
            };

            transform.ValueRW = localToWorld;

            Debug.Log("cam pos ->" + camPosition);
        }
    }
}

