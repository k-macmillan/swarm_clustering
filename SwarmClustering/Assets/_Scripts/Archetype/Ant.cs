using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public static class Ant
{
    public static EntityArchetype antArchetype;

    public static MeshInstanceRenderer antMesh;

    public static void CreateAnt(ref Entity ant, ref EntityManager em)
    {
       
        em.SetComponentData(ant, new Position { Value = Common.SetPosition() });
        em.SetComponentData(ant, new Fitness { Value = em.GetComponentData<Position>(ant).Value.z });
        em.SetComponentData(ant, new BestFitness { Value = em.GetComponentData<Fitness>(ant).Value });
        em.SetComponentData(ant, new BestPosition { Value = em.GetComponentData<Position>(ant).Value });
        em.SetComponentData(ant, new Velocity { Value = Random.value});

        em.AddSharedComponentData(ant, antMesh);
    }
}