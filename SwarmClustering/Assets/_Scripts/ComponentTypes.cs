using Unity.Entities;


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
