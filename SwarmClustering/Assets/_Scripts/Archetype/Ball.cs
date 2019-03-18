using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

public static class Ball
{
    public static EntityArchetype ballArchetype;

    public static MeshInstanceRenderer ballBlueMesh;
    public static MeshInstanceRenderer ballRedMesh;

    public static void CreateBall(ref Entity ball, ref EntityManager em, float3 position, int color)
    {
        em.SetComponentData(ball, new Position { Value = position });
        em.SetComponentData(ball, new Rotation { Value = new quaternion(0f, 0f, 0f, 1f) });
        em.SetComponentData(ball, new Faction { Value = color });
        em.SetComponentData(ball, new Carried { Value = Common.False });

        if (color == Common.Blue)
        {
            em.AddSharedComponentData(ball, ballBlueMesh);
        }
        else
        {
            em.AddSharedComponentData(ball, ballRedMesh);
        }
        
    }
}