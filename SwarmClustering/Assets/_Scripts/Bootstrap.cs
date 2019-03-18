using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public class Bootstrap
{
    public static EntityManager em;
    static Entity ball;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void CreateArchetypes()
    {
        em = World.Active.GetOrCreateManager<EntityManager>();

        AntBootstrap.Initialize(ref em);
        BlueBallBootstrap.Initialize(ref em);
        RedBallBootstrap.Initialize(ref em);
    }


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void LoadMeshes()
    {
        AntBootstrap.InitializeWithScene();
        BlueBallBootstrap.InitializeWithScene();
        RedBallBootstrap.InitializeWithScene();

        NewGame();
    }


    public static void NewGame()
    {
        // TODO: Add a camera controller
        InitializeGame();


    }

    private static void InitializeGame()
    {
        // Place Blue Balls
        for (int i = 0; i < 100; ++i)
        {
            int x = UnityEngine.Random.Range(0, 199);
            int z = UnityEngine.Random.Range(0, 199);
            BlueBall.GenerateBall(ref ball, ref em, new float3(x, 1, z));
        }

        // Place Red Balls
        for (int i = 0; i < 100; ++i)
        {
            int x = UnityEngine.Random.Range(0, 199);
            int z = UnityEngine.Random.Range(0, 199);
            RedBall.GenerateBall(ref ball, ref em, new float3(x, 1, z));
        }

        // Place Ants
        for (int i = 0; i < 500; ++i)
        {
            int x = UnityEngine.Random.Range(0, 199);
            int z = UnityEngine.Random.Range(0, 199);
            Ant.GenerateAnt(ref ball, ref em, new float3(x, 1, z));
        }
    }

}
