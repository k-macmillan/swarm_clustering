using Unity.Entities;
using Unity.Transforms;
using UnityEngine.SceneManagement;


public sealed class BallBootstrap
{
    public static EntityManager em;

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize(ref EntityManager entityManager)
    {
        em = entityManager;

        // Create ball archetype
        Ball.ballArchetype = em.CreateArchetype(
            ComponentType.Create<Position>(),
            ComponentType.Create<Rotation>(),
            ComponentType.Create<Faction>(),
            ComponentType.Create<Carried>()
            );
    }

    public static void InitializeWithScene()
    {
        Ball.ballBlueMesh = Common.GetLookFromPrototype("BlueBallPrototype");
        Ball.ballRedMesh = Common.GetLookFromPrototype("RedBallPrototype");
    }



    public static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        InitializeWithScene();
    }
}
