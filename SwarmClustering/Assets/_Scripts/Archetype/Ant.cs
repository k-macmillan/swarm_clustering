using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public static class Ant
{
    public static EntityArchetype antArchetype;

    public static MeshInstanceRenderer antMesh;

    public static void CreateAnt(ref Entity ant, ref EntityManager em, float3 position)
    {
        ant = em.CreateEntity(antArchetype);
        em.SetComponentData(ant, new Position { Value = position });
        em.SetComponentData(ant, new Rotation { Value = new quaternion(0f, 0f, 0f, 1f) });
        em.SetComponentData(ant, new Carrying { Value = Common.False });

        em.AddSharedComponentData(ant, antMesh);
    }
}