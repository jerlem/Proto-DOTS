using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

partial struct NewISystemScript : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TagFlyingCube>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float fD_T = SystemAPI.Time.DeltaTime;
        float3 f3Velocity = new float3(10.2f, 0, 0);
        foreach (var (transform, _) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<TagFlyingCube>>())
        {
            // move
            transform.ValueRW.Position += f3Velocity * fD_T;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}
