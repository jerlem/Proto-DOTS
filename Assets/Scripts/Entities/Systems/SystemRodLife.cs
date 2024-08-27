using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Flocking;
using FPSTemplate;
using Unity.Physics;

public partial class SystemRodLife : SystemBase
{
    protected override void OnUpdate()
    {
        int indexForeach = 0;
        // Query entities with ComponentRodLife and LocalTransform components
        foreach (var (rodLife, transform) in SystemAPI.Query<RefRW<ComponentRodLife>, RefRW<LocalTransform>>())
        {
            float fLife = rodLife.ValueRO.RodLife / 10000.0f;
            // Calculate the interpolated height
            float normalizedRodLife = math.clamp(fLife, 0.0f, 1.0f);
            float interpolatedHeight = math.lerp(-120.0f, 200.0f, normalizedRodLife);

            // Set the height position
            transform.ValueRW.Position.y = interpolatedHeight;

            // UI interface
            // GameManager.Instance.SetRodHP(indexForeach, fLife);
            indexForeach++;
        }
    }
}
