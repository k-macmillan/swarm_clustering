using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

public static class Ball
{
    public static EntityArchetype ballArchetype;

    public static MeshInstanceRenderer ballBlueMesh;
    public static MeshInstanceRenderer ballRedMesh;

    public static void CreateBall(ref Entity ball, ref EntityManager em, int position, int color)
    {
        em.SetComponentData(ball, new Position { Value = Common.GetGridLocation(position) });
        em.SetComponentData(ball, new Faction { Value = color });

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