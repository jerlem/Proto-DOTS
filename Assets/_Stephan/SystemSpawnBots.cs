//SystemSpawnBots.cs
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

// [UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct SystemSpawnBots : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ComponentConfig>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        UnityEngine.Debug.Log("Bot Spawning System Update");

        state.Enabled = false;

        ComponentConfig config = SystemAPI.GetSingleton<ComponentConfig>();
        Random rand = new Unity.Mathematics.Random(123);

        // Spawn Bot Teams
        {
            int iBotsToSpawn = rand.NextInt(5, 123);
            NativeHashSet<float3> nativehashsetf3OccupiedPositions = new NativeHashSet<float3>(iBotsToSpawn, Allocator.Temp);

            for (int iForIdx = 0; iForIdx < iBotsToSpawn; iForIdx++)
            {
                Entity entityBotFor = state.EntityManager.Instantiate(config.EntityBotPrefab);

                int iColumnsFor = (int)math.floor(math.sqrt(iBotsToSpawn));
                int iRowsFor = (int)math.ceil((float)iBotsToSpawn / iColumnsFor);

                float fXFor, fZFor;
                float3 f3PositionFor;
                do
                {
                    fXFor = rand.NextFloat(0.5f, iColumnsFor - 0.5f);
                    fZFor = rand.NextFloat(0.5f, iRowsFor - 0.5f);
                    f3PositionFor = new float3(fXFor, 1, fZFor);
                } while (nativehashsetf3OccupiedPositions.Contains(f3PositionFor));

                nativehashsetf3OccupiedPositions.Add(f3PositionFor);

                //fXFor = rand.NextFloat(0.5f, iColumnsFor - 0.5f);
                //fZFor = rand.NextFloat(0.5f, iRowsFor - 0.5f);
                //f3PositionFor = new float3(fXFor, 1, fZFor);

                state.EntityManager.SetComponentData(entityBotFor, LocalTransform.FromPosition(f3PositionFor));
            }

            nativehashsetf3OccupiedPositions.Dispose();
        }
    }
}