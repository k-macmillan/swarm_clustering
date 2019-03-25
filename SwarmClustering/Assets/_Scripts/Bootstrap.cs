using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


public class Bootstrap
{
    public static EntityManager em;
    public static Dictionary<int, int> ants = new Dictionary<int, int>();
    public static Dictionary<int, Entity> balls = new Dictionary<int, Entity>();
    
    public static GameObject camera;


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
        //TestCase();
        InitializeGame();
        Run();
    }

    private static void InitializeGame()
    {

        // Place Balls
        for (int i = 0; i < 500; ++i)
        {
            GenerateBall(Common.Red);
            GenerateBall(Common.Blue);
        }

        // Place Ants
        for (int i = 0; i < 500; ++i)
        {
            GenerateAnt();
        }
    }

    private static void GenerateBall(int color)
    {
        int loop_count = 0;
        int position = Random.Range(0, Common.max_value);
        while ((balls.ContainsKey(position) || ants.ContainsKey(position)) && loop_count < Common.loop_limit)
        {
            position = Random.Range(0, Common.max_value);
            ++loop_count;
        }
        if (loop_count != Common.loop_limit)
        {
            Entity ball = em.CreateEntity(Ball.ballArchetype);
            Ball.CreateBall(ref ball, ref em, position, color);
            balls.Add(position, ball);
        }
    }

    private static void GenerateAnt()
    {
        int loop_count = 0;
        int position = Random.Range(0, Common.max_value);
        while ((balls.ContainsKey(position) || ants.ContainsKey(position)) && loop_count < Common.loop_limit)
        {
            position = Random.Range(0, Common.max_value);
            ++loop_count;
        }
        if (loop_count != Common.loop_limit)
        {
            Entity ant = em.CreateEntity(Ant.antArchetype);
            Ant.CreateAnt(ref ant, ref em, position);
            ants.Add(position, 0);
        }
    }

    private static void Run()
    {

    }

    private static void TestCase()
    {
        Common.width = 5;
        Common.height = 5;
        Common.max_value = Common.width * Common.height - 1;

        // Center
        Entity ball = em.CreateEntity(Ball.ballArchetype);
        Ball.CreateBall(ref ball, ref em, 12, Common.Blue);
        balls.Add(12, ball);

        // Edge
        Entity ball2 = em.CreateEntity(Ball.ballArchetype);
        Ball.CreateBall(ref ball2, ref em, 0, Common.Blue);
        balls.Add(0, ball2);

        // Ant
        Entity ant = em.CreateEntity(Ant.antArchetype);
        Ant.CreateAnt(ref ant, ref em, 0);
        ants.Add(0, 0);

        // Border
        // Edge
        Entity ball3 = em.CreateEntity(Ball.ballArchetype);
        Ball.CreateBall(ref ball3, ref em, 4, Common.Red);
        balls.Add(4, ball3);

        Entity ball4 = em.CreateEntity(Ball.ballArchetype);
        Ball.CreateBall(ref ball4, ref em, 24, Common.Red);
        balls.Add(24, ball4);

        Entity ball5 = em.CreateEntity(Ball.ballArchetype);
        Ball.CreateBall(ref ball5, ref em, 20, Common.Red);
        balls.Add(20, ball5);
    }

}
