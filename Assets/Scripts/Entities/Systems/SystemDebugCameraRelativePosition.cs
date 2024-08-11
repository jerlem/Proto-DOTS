using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct SystemDebugCameraRelativePosition : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Query all entities with ComponentCameraRelativePosition and ComponentDebugSphere
        foreach (var (cameraRelativePosition, transform, debugSphere) in
                 SystemAPI.Query<RefRO<ComponentCameraRelativePosition>, RefRW<LocalTransform>, RefRO<ComponentDebugSphere>>())
        {
            // Update the position of the debug sphere based on the camera relative position
            transform.ValueRW.Position = cameraRelativePosition.ValueRO.Position;
            LogCameraRelativePosition(cameraRelativePosition.ValueRO.Position);
        }

        void LogCameraRelativePosition(float3 position)
        {
            Debug.Log($"Camera Relative Position PROUTTT: {position}");
        }

    }
}

