using Flocking;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

// Update SystemHerding to follow similar structure as SystemFlocking
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial struct SystemHerding : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var boidQuery = SystemAPI.QueryBuilder().WithAll<SharedComponentHerdingSettings>().WithAllRW<LocalToWorld>().Build();
        var targetQuery = SystemAPI.QueryBuilder().WithAll<ComponentFlockingTarget, LocalToWorld>().Build();
        var dangerQuery = SystemAPI.QueryBuilder().WithAll<ComponentFlockingDanger, LocalToWorld>().Build();
        var obstacleQuery = SystemAPI.QueryBuilder().WithAll<ComponentFlockingObstacle, LocalToWorld, PhysicsCollider>().Build();

        var dangerCount = dangerQuery.CalculateEntityCount();
        var targetCount = targetQuery.CalculateEntityCount();

        var world = state.WorldUnmanaged;
        state.EntityManager.GetAllUniqueSharedComponents(out NativeList<SharedComponentHerdingSettings> uniqueBoidTypes, world.UpdateAllocator.ToAllocator);
        float dt = math.min(0.05f, SystemAPI.Time.DeltaTime);

        if (uniqueBoidTypes.IsEmpty)
        {
            UnityEngine.Debug.Log("No unique herding types found. Exiting early.");
            return;
        }

        if (targetCount == 0)
        {
            UnityEngine.Debug.Log("No targets found. Skipping target-related processing.");
        }

        if (dangerCount == 0)
        {
            UnityEngine.Debug.Log("No dangers found. Skipping danger-related processing.");
        }

        // Each variant of the Boid represents a different value of the SharedComponentData and is self-contained,
        // meaning Boids of the same variant only interact with one another. Thus, this loop processes each
        // variant type individually.
        foreach (var boidSettings in uniqueBoidTypes)
        {
            boidQuery.AddSharedComponentFilter(boidSettings);

            var boidCount = boidQuery.CalculateEntityCount();
            if (boidCount == 0)
            {
                // Early out. If the given variant includes no Boids, move on to the next loop.
                // For example, variant 0 will always exit early bc it's it represents a default, uninitialized
                // Boid struct, which does not appear in this sample.
                boidQuery.ResetFilter();
                continue;
            }


            var hashMap = new NativeParallelMultiHashMap<int, int>(boidCount, world.UpdateAllocator.ToAllocator);
            var cellIndices = CollectionHelper.CreateNativeArray<int, RewindableAllocator>(boidCount, ref world.UpdateAllocator);
            var cellDangerPositionIndex = CollectionHelper.CreateNativeArray<int, RewindableAllocator>(boidCount, ref world.UpdateAllocator);
            var cellTargetPositionIndex = CollectionHelper.CreateNativeArray<int, RewindableAllocator>(boidCount, ref world.UpdateAllocator);
            var cellCount = CollectionHelper.CreateNativeArray<int, RewindableAllocator>(boidCount, ref world.UpdateAllocator);
            var cellDangerDistance = CollectionHelper.CreateNativeArray<float, RewindableAllocator>(boidCount, ref world.UpdateAllocator);
            var cellAlignment = CollectionHelper.CreateNativeArray<float3, RewindableAllocator>(boidCount, ref world.UpdateAllocator);
            var cellSeparation = CollectionHelper.CreateNativeArray<float3, RewindableAllocator>(boidCount, ref world.UpdateAllocator);

            var copyTargetPositions = CollectionHelper.CreateNativeArray<float3, RewindableAllocator>(targetCount, ref world.UpdateAllocator);
            var copyDangerPositions = CollectionHelper.CreateNativeArray<float3, RewindableAllocator>(dangerCount, ref world.UpdateAllocator);

            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

            var initialHerdingJob = new InitialPerHerdingJob
            {
                CellAlignment = cellAlignment,
                CellSeparation = cellSeparation,
                ParallelHashMap = hashMap.AsParallelWriter(),
                InverseHerdingCellRadius = 1.0f / boidSettings.CellRadius,
            };
            var initialHerdingJobHandle = initialHerdingJob.ScheduleParallel(boidQuery, state.Dependency);

            var initialTargetJob = new InitialPerTargetJob
            {
                TargetPositions = copyTargetPositions,
            };
            var initialTargetJobHandle = initialTargetJob.ScheduleParallel(targetQuery, state.Dependency);

            var initialDangerJob = new InitialPerDangerJob
            {
                DangerPositions = copyDangerPositions,
            };
            var initialDangerJobHandle = initialDangerJob.ScheduleParallel(dangerQuery, state.Dependency);

            var initialCellCountJob = new MemsetNativeArray<int>
            {
                Source = cellCount,
                Value = 1
            };
            var initialCellCountJobHandle = initialCellCountJob.Schedule(boidCount, 64, state.Dependency);

            var initialCellBarrierJobHandle = JobHandle.CombineDependencies(initialHerdingJobHandle, initialCellCountJobHandle);
            var copyTargetDangerBarrierJobHandle = JobHandle.CombineDependencies(initialTargetJobHandle, initialDangerJobHandle);
            var mergeCellsBarrierJobHandle = JobHandle.CombineDependencies(initialCellBarrierJobHandle, copyTargetDangerBarrierJobHandle);

            var mergeCellsJob = new MergeCells
            {
                cellIndices = cellIndices,
                cellAlignment = cellAlignment,
                cellSeparation = cellSeparation,
                cellDangerDistance = cellDangerDistance,
                cellDangerPositionIndex = cellDangerPositionIndex,
                cellTargetPositionIndex = cellTargetPositionIndex,
                cellCount = cellCount,
                targetPositions = copyTargetPositions,
                obstaclePositions = copyDangerPositions
            };
            var mergeCellsJobHandle = mergeCellsJob.Schedule(hashMap, 64, mergeCellsBarrierJobHandle);

            var steerHerdingJob = new SteerHerdingJob
            {
                CellIndices = cellIndices,
                CellCount = cellCount,
                CellAlignment = cellAlignment,
                CellSeparation = cellSeparation,
                CellDangerDistance = cellDangerDistance,
                CellDangerPositionIndex = cellDangerPositionIndex,
                CellTargetPositionIndex = cellTargetPositionIndex,
                DangerPositions = copyDangerPositions,
                TargetPositions = copyTargetPositions,
                CurrentBoidVariant = boidSettings,
                DeltaTime = dt,
                MoveDistance = boidSettings.MoveSpeed * dt,
            };
            var steerHerdingJobHandle = steerHerdingJob.ScheduleParallel(boidQuery, mergeCellsJobHandle);

            var avoidObstacleJob = new AvoidObstacleJob
            {
                DeltaTime = dt,
                PhysicsWorld = physicsWorld,
                CurrentBoidVariant = boidSettings
            };
            var avoidObstacleJobHandle = avoidObstacleJob.ScheduleParallel(boidQuery, steerHerdingJobHandle);

            state.Dependency = JobHandle.CombineDependencies(steerHerdingJobHandle, avoidObstacleJobHandle);

            boidQuery.AddDependency(state.Dependency);
            boidQuery.ResetFilter();
        }
        uniqueBoidTypes.Dispose();
    }

    [BurstCompile]
    struct MergeCells : IJobNativeParallelMultiHashMapMergedSharedKeyIndices
    {
        public NativeArray<int> cellIndices;
        public NativeArray<float3> cellAlignment;
        public NativeArray<float3> cellSeparation;
        public NativeArray<int> cellDangerPositionIndex;
        public NativeArray<float> cellDangerDistance;
        public NativeArray<int> cellTargetPositionIndex;
        public NativeArray<int> cellCount;
        [ReadOnly] public NativeArray<float3> targetPositions;
        [ReadOnly] public NativeArray<float3> obstaclePositions;

        void NearestPosition(NativeArray<float3> targets, float3 position, out int nearestPositionIndex, out float nearestDistance)
        {
            nearestPositionIndex = 0;
            nearestDistance = math.lengthsq(position - targets[0]);
            for (int i = 1; i < targets.Length; i++)
            {
                var targetPosition = targets[i];
                var distance = math.lengthsq(position - targetPosition);
                var nearest = distance < nearestDistance;

                nearestDistance = math.select(nearestDistance, distance, nearest);
                nearestPositionIndex = math.select(nearestPositionIndex, i, nearest);
            }
            nearestDistance = math.sqrt(nearestDistance);
        }

        public void ExecuteFirst(int index)
        {
            var position = cellSeparation[index] / cellCount[index];

            int obstaclePositionIndex;
            float obstacleDistance;
            NearestPosition(obstaclePositions, position, out obstaclePositionIndex, out obstacleDistance);
            cellDangerPositionIndex[index] = obstaclePositionIndex;
            cellDangerDistance[index] = obstacleDistance;

            int targetPositionIndex;
            float targetDistance;
            NearestPosition(targetPositions, position, out targetPositionIndex, out targetDistance);
            cellTargetPositionIndex[index] = targetPositionIndex;

            cellIndices[index] = index;
        }

        public void ExecuteNext(int cellIndex, int index)
        {
            cellCount[cellIndex] += 1;
            cellAlignment[cellIndex] += cellAlignment[index];
            cellSeparation[cellIndex] += cellSeparation[index];
            cellIndices[index] = cellIndex;
        }
    }

    [BurstCompile]
    partial struct InitialPerHerdingJob : IJobEntity
    {
        public NativeArray<float3> CellAlignment;
        public NativeArray<float3> CellSeparation;
        public NativeParallelMultiHashMap<int, int>.ParallelWriter ParallelHashMap;
        public float InverseHerdingCellRadius;
        void Execute([EntityIndexInQuery] int entityIndexInQuery, in LocalToWorld localToWorld)
        {
            CellAlignment[entityIndexInQuery] = localToWorld.Forward;
            CellSeparation[entityIndexInQuery] = localToWorld.Position;
            ParallelHashMap.Add(entityIndexInQuery, entityIndexInQuery);
        }
    }

    [BurstCompile]
    partial struct InitialPerTargetJob : IJobEntity
    {
        public NativeArray<float3> TargetPositions;
        void Execute([EntityIndexInQuery] int entityIndexInQuery, in ComponentFlockingTarget target, in LocalToWorld localToWorld)
        {
            TargetPositions[entityIndexInQuery] = localToWorld.Position;
        }
    }

    [BurstCompile]
    partial struct InitialPerDangerJob : IJobEntity
    {
        public NativeArray<float3> DangerPositions;
        void Execute([EntityIndexInQuery] int entityIndexInQuery, in ComponentFlockingDanger danger, in LocalToWorld localToWorld)
        {
            DangerPositions[entityIndexInQuery] = localToWorld.Position;
        }
    }

    [BurstCompile]
    partial struct SteerHerdingJob : IJobEntity
    {
        [ReadOnly] public NativeArray<int> CellIndices;
        [ReadOnly] public NativeArray<int> CellCount;
        [ReadOnly] public NativeArray<float3> CellAlignment;
        [ReadOnly] public NativeArray<float3> CellSeparation;
        [ReadOnly] public NativeArray<float> CellDangerDistance;
        [ReadOnly] public NativeArray<int> CellDangerPositionIndex;
        [ReadOnly] public NativeArray<int> CellTargetPositionIndex;
        [ReadOnly] public NativeArray<float3> DangerPositions;
        [ReadOnly] public NativeArray<float3> TargetPositions;
        public SharedComponentHerdingSettings CurrentBoidVariant;
        public float DeltaTime;
        public float MoveDistance;

        void Execute([EntityIndexInQuery] int entityIndexInQuery, ref LocalToWorld localToWorld)
        {
            // temporarily storing the values for code readability
            var forward = localToWorld.Forward;
            var currentPosition = localToWorld.Position;
            var cellIndex = CellIndices[entityIndexInQuery];
            var neighborCount = CellCount[cellIndex];
            var alignment = CellAlignment[cellIndex];
            var separation = CellSeparation[cellIndex];
            var nearestDangerDistance = CellDangerDistance[cellIndex];
            var nearestDangerPositionIndex = CellDangerPositionIndex[cellIndex];
            var nearestTargetPositionIndex = CellTargetPositionIndex[cellIndex];
            var nearestDangerPosition = DangerPositions[nearestDangerPositionIndex];
            var nearestTargetPosition = TargetPositions[nearestTargetPositionIndex];

            // Setting up the directions for the three main biocrowds influencing directions adjusted based
            // on the predefined weights:
            // 1) alignment - how much should it move in a direction similar to those around it?
            // note: we use `alignment/neighborCount`, because we need the average alignment in this case; however
            // alignment is currently the summation of all those of the boids within the cellIndex being considered.
            var alignmentResult = CurrentBoidVariant.AlignmentWeight
                                      * math.normalizesafe((alignment / neighborCount) - forward);
            // 2) separation - how close is it to other boids and are there too many or too few for comfort?
            // note: here separation represents the summed possible center of the cell. We perform the multiplication
            // so that both `currentPosition` and `separation` are weighted to represent the cell as a whole and not
            // the current individual boid.
            var separationResult = CurrentBoidVariant.SeparationWeight
                                      * math.normalizesafe((currentPosition * neighborCount) - separation);
            // 3) target - is it still towards its destination?
            var targetHeading = CurrentBoidVariant.TargetWeight
                                      * math.normalizesafe(nearestTargetPosition - currentPosition);

            // creating the obstacle avoidant vector s.t. it's pointing towards the nearest obstacle
            // but at the specified 'DangerAversionDistance'. If this distance is greater than the
            // current distance to the obstacle, the direction becomes inverted. This simulates the
            // idea that if `currentPosition` is too close to an obstacle, the weight of this pushes
            // the current boid to escape in the fastest direction; however, if the obstacle isn't
            // too close, the weighting denotes that the boid doesnt need to escape but will move
            // slower if still moving in that direction (note: we end up not using this move-slower
            // case, because of `targetForward`'s decision to not use obstacle avoidance if an obstacle
            // isn't close enough).
            var obstacleSteering = currentPosition - nearestDangerPosition;
            var avoidDangerHeading = (nearestDangerPosition + math.normalizesafe(obstacleSteering)
                * CurrentBoidVariant.DangerAversionDistance) - currentPosition;

            // the updated heading direction. If not needing to be avoidant (ie obstacle is not within
            // predefined radius) then go with the usual defined heading that uses the amalgamation of
            // the weighted alignment, separation, and target direction vectors.
            var nearestDangerDistanceFromRadius = nearestDangerDistance - CurrentBoidVariant.DangerAversionDistance;
            var normalHeading = math.normalizesafe(alignmentResult + separationResult + targetHeading);
            var targetForward = math.select(normalHeading, avoidDangerHeading, nearestDangerDistanceFromRadius < 0);

            // updates using the newly calculated heading direction
            var nextHeading = math.normalizesafe(forward + DeltaTime * (targetForward - forward));
            nextHeading = new float3(nextHeading.x, 0, nextHeading.z);
            localToWorld = new LocalToWorld
            {
                Value = float4x4.TRS(
                    // TODO: precalc speed*dt
                    new float3(localToWorld.Position + (nextHeading * MoveDistance)),
                    quaternion.LookRotationSafe(nextHeading, math.up()),
                    new float3(1.0f, 1.0f, 1.0f))
            };
        }
    }

    [BurstCompile]
    partial struct AvoidObstacleJob : IJobEntity
    {
        public float DeltaTime;
        public float3 BoidPosition;
        public quaternion BoidRotation;
        public Unity.Physics.PhysicsWorld PhysicsWorld;
        public SharedComponentHerdingSettings CurrentBoidVariant;

        void Execute([EntityIndexInQuery] int entityIndexInQuery, ref LocalToWorld localToWorld)
        {
            var forward = localToWorld.Forward;
            //var ray = new Unity.Physics.Ray(localToWorld.Position, forward);

            Unity.Physics.RaycastHit hit;
            if (PhysicsWorld.CastRay(new Unity.Physics.RaycastInput
            {
                Start = localToWorld.Position,
                End = localToWorld.Position + forward * CurrentBoidVariant.DangerAversionDistance,
                Filter = Unity.Physics.CollisionFilter.Default
            }, out hit))
            {
                // Obstacle detected within the DangerAversionDistance, steer away
                var hitPosition = hit.Position;
                var avoidDirection = math.normalizesafe(localToWorld.Position - hitPosition);
                var newDirection = math.normalizesafe(forward + avoidDirection * DeltaTime);
                localToWorld = new LocalToWorld
                {
                    Value = float4x4.TRS(
                        localToWorld.Position + newDirection * CurrentBoidVariant.MoveSpeed * DeltaTime,
                        quaternion.LookRotationSafe(newDirection, math.up()),
                        new float3(1.0f, 1.0f, 1.0f))
                };
            }
        }
    }
}
