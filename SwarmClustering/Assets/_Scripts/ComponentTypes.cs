using Unity.Entities;
using Unity.Mathematics;

public struct Position : IComponentData
{
    public float3 Value;
}

public struct BestPosition : IComponentData
{
    public float3 Value;
}

public struct Fitness : IComponentData
{
    public float Value;
}

public struct BestFitness : IComponentData
{
    public float Value;
}

public struct Velocity : IComponentData
{
    public float3 Value;
}