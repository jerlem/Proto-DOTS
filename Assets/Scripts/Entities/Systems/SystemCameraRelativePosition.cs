using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

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
        float3 cameraPosition = SystemCacheCameraData.CameraPosition;
        float3 offset = SystemCacheCameraData.CameraForward * 300.0f; // Adjust 2.0f to move the crosshair closer or farther

        // Update all entities with the ComponentCameraRelativePosition component
        foreach (var positionRef in SystemAPI.Query<RefRW<ComponentCameraRelativePosition>>())
        {
            positionRef.ValueRW.Position = cameraPosition + offset;
        }
    }
}


