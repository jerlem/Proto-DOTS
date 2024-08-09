using Unity.Collections;
using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

namespace Flocking
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    [UpdateAfter(typeof(SystemFlocking))]
    public partial struct SystemCrawler : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var crawlerQuery = SystemAPI.QueryBuilder()
                                        .WithAllRW<LocalToWorld>()
                                        .WithAll<SharedComponentFlockingCrawler>()
                                        .Build();

            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

            var crawlOnSurfaceJob = new CrawlOnSurfaceJob
            {
                PhysicsWorld = physicsWorld,
            };
            var crawlOnSurfaceJobHandle = crawlOnSurfaceJob.ScheduleParallel(crawlerQuery, state.Dependency);

            state.Dependency = crawlOnSurfaceJobHandle;
        }

        [BurstCompile]
        partial struct CrawlOnSurfaceJob : IJobEntity
        {
            [ReadOnly] public PhysicsWorld PhysicsWorld;

            void Execute([EntityIndexInQuery] int entityIndexInQuery, ref LocalToWorld localToWorld, in SharedComponentFlockingCrawler crawlerComponent)
            {
                float3 crawlerPosition = localToWorld.Position;

                //// Initial downward raycast to check for surface underneath
                //var downwardRayInput = new RaycastInput
                //{
                //    Start = crawlerPosition,
                //    End = crawlerPosition - new float3(0, crawlerComponent.InitialRayLength, 0), // Raycast directly downward
                //    Filter = CollisionFilter.Default
                //};

                //if (PhysicsWorld.CastRay(downwardRayInput, out var downwardHit))
                //{
                //    // If a surface is found directly underneath, update position and orientation
                //    localToWorld = new LocalToWorld
                //    {
                //        Value = float4x4.TRS(
                //            downwardHit.Position,
                //            quaternion.LookRotationSafe(math.up(), downwardHit.SurfaceNormal),
                //            new float3(1.0f, 1.0f, 1.0f))
                //    };
                //    return; // No need to perform the sphere cast, exit early
                //}

                float rayLength = crawlerComponent.InitialRayLength; // Initial ray length
                while (rayLength < crawlerComponent.MaxRayLength) // Maximum ray length
                {
                    for (int i = 0; i < 64; i++)
                    {
                        float3 spherePoint = math.mul(quaternion.RotateY(i * (math.PI * 2f / 64f)), new float3(0, 0, 1)) * rayLength;
                        var rayInput = new RaycastInput
                        {
                            Start = crawlerPosition + spherePoint,
                            End = crawlerPosition - new float3(0, rayLength, 0), // Cast ray downwards
                            Filter = CollisionFilter.Default
                        };

                        if (PhysicsWorld.CastRay(rayInput, out var hit))
                        {
                            localToWorld = new LocalToWorld
                            {
                                Value = float4x4.TRS(
                                    hit.Position,
                                    quaternion.LookRotationSafe(math.up(), hit.SurfaceNormal),
                                    new float3(1.0f, 1.0f, 1.0f))
                            };
                            return; // Exit the function once a collision is found
                        }
                    }
                    rayLength *= 2f; // Increase ray length by a factor of 2
                }
            }
        }
    }
}
