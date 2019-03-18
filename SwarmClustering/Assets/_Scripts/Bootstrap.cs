using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


public class Bootstrap
{
    public static EntityManager em;
    public static Dictionary<int, int> ants = new Dictionary<int, int>();
    public static Dictionary<int, Entity> balls = new Dictionary<int, Entity>();
    public static int width = 200;
    public static int height= 200;
    public static int max_value = 200 * 200 - 1;
    public static GameObject camera;
    public static float Delay = 0.25f;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void CreateArchetypes()
    {
        em = World.Active.GetOrCreateManager<EntityManager>();

        AntBootstrap.Initialize();
        BallBootstrap.Initialize();
    }


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void LoadMeshes()
    {
        AntBootstrap.InitializeWithScene();
        BallBootstrap.InitializeWithScene();
        camera = GameObject.Find("PlayerCamera").gameObject;
        PlayerController.LockCursor();
        NewGame();
    }


    public static void NewGame()
    {
        InitializeGame();
        Run();
    }

    private static void InitializeGame()
    {

        // Place Balls
        for (int i = 0; i < 100; ++i)
        {
            GenerateBall(Common.Blue);
            GenerateBall(Common.Red);
        }

        // Place Ants
        for (int i = 0; i < 500; ++i)
        {
            GenerateAnt();
        }
    }

    private static void GenerateBall(int color)
    {
        int position = Random.Range(0, max_value);
        while (balls.ContainsKey(position) || ants.ContainsKey(position))
        {
            position = Random.Range(0, max_value);
        }
        Entity ball = em.CreateEntity(Ball.ballArchetype);
        Ball.CreateBall(ref ball, ref em, position, color);
        balls.Add(position, ball);
    }

    private static void GenerateAnt()
    {
        int position = Random.Range(0, max_value);
        while (balls.ContainsKey(position) || ants.ContainsKey(position))
        {
            position = Random.Range(0, max_value);
        }
        Entity ant = em.CreateEntity(Ant.antArchetype);
        Ant.CreateAnt(ref ant, ref em, position);
        ants.Add(position, 0);
    }

    private static void Run()
    {

    }

}
