using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


public class Bootstrap
{
    public static EntityManager em;
    public static int POP = 20;
    public static Graph graph;
    public static GameObject camera;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void CreateArchetypes()
    {
        em = World.Active.GetOrCreateManager<EntityManager>();

        AntBootstrap.Initialize();
    }


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void LoadMeshes()
    {
        AntBootstrap.InitializeWithScene();
        camera = GameObject.Find("PlayerCamera").gameObject;
        PlayerController.LockCursor();
        graph = GameObject.Find("Graph").gameObject.GetComponent<Graph>();
        NewGame();
    }


    public static void NewGame()
    {
        //TestCase();
        UpdateCamera();
        InitializeGame();
        Run();
    }

    private static void InitializeGame()
    {
        // Place Ants
        for (int i = 0; i < POP; ++i)
        {
            GenerateAnt(i);
        }
       // Common.DestroySphere("Sphere");
    }

    private static void UpdateTerrain()
    {
        GameObject terrain = GameObject.Find("Terrain").gameObject;
        var terrainComponent = terrain.GetComponent<Terrain>();
        terrainComponent.terrainData.size = new Vector3(Common.width + 10, 1, Common.height + 10);
    }

    private static void UpdateCamera()
    {
        camera = GameObject.Find("PlayerCamera").gameObject;
        camera.transform.position = new Vector3(0, 20, -30);
    }

    private static void GenerateAnt(int index)
    {
        Entity ant = em.CreateEntity(Ant.antArchetype);
        Ant.CreateAnt(ref ant, ref em);
    }

    private static void Run()
    {

    }

    private static void TestCase()
    {
        Common.width = 5;
        Common.height = 5;
        Common.max_value = Common.width * Common.height - 1;

        // Ant
        Entity ant = em.CreateEntity(Ant.antArchetype);
        Ant.CreateAnt(ref ant, ref em);
    }

}
