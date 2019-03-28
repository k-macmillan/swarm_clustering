using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;

public static class Ball
{
    public static EntityArchetype ballArchetype;

    public static MeshInstanceRenderer ballRedMesh;
    public static MeshInstanceRenderer ballBlueMesh;
    public static MeshInstanceRenderer ballGreenMesh;
    public static MeshInstanceRenderer ballYellowMesh;
    public static MeshInstanceRenderer ballPurpleMesh;

    public static void CreateBall(ref Entity ball, ref EntityManager em, int position, int color)
    {
        em.SetComponentData(ball, new Position { Value = Common.GetGridLocation(position) });
        em.SetComponentData(ball, new Faction { Value = color });

        switch (color)
        {
            case Common.Red:
                em.AddSharedComponentData(ball, ballRedMesh);
                break;
            case Common.Blue:
                em.AddSharedComponentData(ball, ballBlueMesh);
                break;
            case Common.Green:
                em.AddSharedComponentData(ball, ballGreenMesh);
                break;
            case Common.Yellow:
                em.AddSharedComponentData(ball, ballYellowMesh);
                break;
            case Common.Purple:
                em.AddSharedComponentData(ball, ballPurpleMesh);
                break;
        }
    }
}