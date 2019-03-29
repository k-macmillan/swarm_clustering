using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class SwarmMechanics : ComponentSystem
{
    private static float timer = 0f;

    public struct AntData
    {
        public readonly int Length;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<BestPosition> BestPosition;
        public ComponentDataArray<Fitness> Fitness;
        public ComponentDataArray<BestFitness> BestFitness;
        public ComponentDataArray<Velocity> Velocity;
    }

    [Inject] private AntData a_Data;


    // Update is called once per frame
    protected override void OnUpdate()
    {
        // Every Bootstrap.Delay seconds update the ants
        timer += Time.deltaTime;
        if (timer >= Common.Delay)
        {
            timer = 0f;
            // Update new direction/actions
            for (int i = 0; i < Bootstrap.POP; ++i)
            {
                UpdateAnt(i);
            }
        }
    }

    private void UpdateAnt(int index)
    {
        float phi1 = Random.value;
        float phi2 = Random.value;

        a_Data.Velocity[index] = new Velocity
        {
            Value = a_Data.Velocity[index].Value +
            phi1 * (a_Data.BestPosition[index].Value - a_Data.Position[index].Value) +
            phi2 * (Common.Global - a_Data.Position[index].Value)
        };

        if(a_Data.Position[index].Value.x + a_Data.Velocity[index].Value.x < Common.max_value.x &&
            a_Data.Position[index].Value.x + a_Data.Velocity[index].Value.x > Common.min_value.x)
        {
            a_Data.Position[index] = new Position { Value = a_Data.Position[index].Value + a_Data.Velocity[index].Value };
            a_Data.Fitness[index] = new Fitness { Value = Common.Evaluation(a_Data.Position[index].Value.x) };
        }
        
        if(a_Data.Fitness[index].Value > a_Data.BestFitness[index].Value)
        {
            a_Data.BestFitness[index] = new BestFitness { Value = a_Data.Fitness[index].Value };
            a_Data.BestPosition[index] = new BestPosition { Value = a_Data.Position[index].Value };
        }

        if(a_Data.Fitness[index].Value > Common.Evaluation(Common.Global.x))
        {
            Common.Global = a_Data.BestPosition[index].Value;
        }
    }
}
