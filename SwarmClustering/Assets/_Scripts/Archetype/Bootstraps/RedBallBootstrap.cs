using Unity.Entities;
using Unity.Transforms;
using UnityEngine.SceneManagement;


public sealed class RedBallBootstrap
{
    public static EntityManager em;

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize(ref EntityManager entityManager)
    {
        em = entityManager;

        // Create ship archetype
        RedBall.ballArchetype = em.CreateArchetype(
            ComponentType.Create<Position>(),
            ComponentType.Create<Rotation>()
            );
    }

    public static void InitializeWithScene()
    {
        RedBall.ballMesh = Common.GetLookFromPrototype("RedBallPrototype");
    }



    private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        InitializeWithScene();
    }
}
