﻿using Unity.Entities;
using Unity.Transforms;
using UnityEngine.SceneManagement;


public sealed class AntBootstrap
{
    public static EntityManager em;

    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize(ref EntityManager entityManager)
    {
        em = entityManager;

        // Create ant archetype
        Ant.antArchetype = em.CreateArchetype(
            ComponentType.Create<Position>(),
            ComponentType.Create<Rotation>(),
            ComponentType.Create<Carrying>()
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