using FPSTemplate;
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
        if (!GameManager.PlayerController.isFiring)
            return;        

        float3 cameraPosition = SystemCacheCameraData.CameraPosition;
        cameraPosition.y -= 1.5f;

        float3 offset = SystemCacheCameraData.CameraForward * 2.0f; // Adjust 2.0f to move the crosshair closer or farther

        // Update all entities with the ComponentCameraRelativePosition component
        foreach (var positionRef in SystemAPI.Query<RefRW<ComponentCameraRelativePosition>>())
        {
            positionRef.ValueRW.Position = cameraPosition + offset;
        }
    }
}


