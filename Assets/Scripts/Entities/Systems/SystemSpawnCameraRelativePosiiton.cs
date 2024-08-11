using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;


//public struct ComponentCameraRelativePosition : IComponentData
//{
//    public float3 Position;
//}

public partial struct SystemEntitySpawnCameraRelativePosition : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // This system should only run once to spawn the entity.
        state.RequireForUpdate<ComponentExecuteCameraPositionEntitySpawn>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Create the entity
        Entity entity = state.EntityManager.CreateEntity();

        // Add the ComponentCameraRelativePosition component
        state.EntityManager.AddComponentData(entity, new ComponentCameraRelativePosition
        {
            Position = float3.zero // Initialize the position to zero
        });

        // Remove the requirement to run again after spawning
        state.EntityManager.DestroyEntity(SystemAPI.GetSingletonEntity<ComponentExecuteCameraPositionEntitySpawn>());
    }
}