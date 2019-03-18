using Unity.Entities;
using Unity.Transforms;
using UnityEngine.SceneManagement;


public sealed class BlueBallBootstrap
{
    public static EntityManager em;

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize(ref EntityManager entityManager)
    {
        em = entityManager;

        // Create ship archetype
        BlueBall.ballArchetype = em.CreateArchetype(
            ComponentType.Create<Position>(),
            ComponentType.Create<Rotation>()
            );
    }

    public static void InitializeWithScene()
    {
        BlueBall.ballMesh = Common.GetLookFromPrototype("BlueBallPrototype");
    }



    public static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        InitializeWithScene();
    }
}
