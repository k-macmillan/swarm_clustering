using Unity.Entities;
using Unity.Transforms;
using UnityEngine.SceneManagement;


public sealed class AntBootstrap
{
    public static void Initialize()
    {
        // Create ant archetype
        Ant.antArchetype = Bootstrap.em.CreateArchetype(
            ComponentType.Create<Position>(),
            ComponentType.Create<Carrying>(),
            ComponentType.Create<StartPosition>(),
            ComponentType.Create<NextPosition>()
            );
    }

    public static void InitializeWithScene()
    {
        Ant.antMesh = Common.GetLookFromPrototype("AntBodyPrototype");
    }



    private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        InitializeWithScene();
    }
}
