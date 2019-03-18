using Unity.Entities;
using Unity.Mathematics;

public struct Carried : IComponentData
{
    public int Value;
}

public struct Carrying : IComponentData
{
    public int Value;
}

public struct Faction : IComponentData
{
    public int Value;
}

public struct StartPosition : IComponentData
{
    public float3 Value;
}

public struct NextPosition : IComponentData
{
    public float3 Value;
}