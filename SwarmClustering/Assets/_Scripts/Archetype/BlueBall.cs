using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public static class BlueBall
{
    public static EntityArchetype ballArchetype;

    public static MeshInstanceRenderer ballMesh;

    public static void GenerateBall(ref Entity ball, ref EntityManager em, float3 position)
    {
        ball = em.CreateEntity(ballArchetype);
        em.SetComponentData(ball, new Position { Value = position });
        em.SetComponentData(ball, new Rotation { Value = new quaternion(0f, 0f, 0f, 1f) });

        em.AddSharedComponentData(ball, ballMesh);
    }
}