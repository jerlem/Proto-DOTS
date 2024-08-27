using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Physics;

[BurstCompile]
public struct SphereCastJob : IJobParallelFor
{
    [ReadOnly]
    public CollisionWorld World;

    [ReadOnly]
    public NativeArray<RaycastInput> Inputs;

    public NativeArray<RaycastHit> Results;

    public unsafe void Execute(int index)
    {
        RaycastHit hit;
        World.CastRay(Inputs[index], out hit);
        Results[index] = hit;
    }

    public static JobHandle ScheduleSphereCast(CollisionWorld world, NativeArray<RaycastInput> inputs, 
        NativeArray<RaycastHit> results)
    {
        JobHandle handle = new SphereCastJob
        {
            Inputs = inputs,
            Results = results,
            World = world
        }.Schedule(inputs.Length, 4);

        return handle;
    }

}

