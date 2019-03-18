using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class SwarmMechanics : ComponentSystem
{
    private static float timer = 0f;
    private static int position;
    private static int red;
    private static int blue;
    private static int modifier = -1;
    private const int loop_limit = 100;

    public struct AntData
    {
        public readonly int Length;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<Carrying> Carrying;
        public ComponentDataArray<StartPosition> StartPosition;
        public ComponentDataArray<NextPosition> NextPosition;
    }

    [Inject] private AntData a_Data;


    // Update is called once per frame
    protected override void OnUpdate()
    {
        // Every Bootstrap.Delay seconds update the ants
        timer += Time.deltaTime;
        if (timer > Bootstrap.Delay)
        {
            // Update new direction/actions
            for (int i = 0; i < a_Data.Length; ++i)
            {
                int originalPosition = Common.GetGridIndex(a_Data.StartPosition[i].Value);
                position = Common.GetGridIndex(a_Data.NextPosition[i].Value);
                CountLocality();
                UpdatePosition(i);
                if (a_Data.Carrying[i].Value == Common.False && Bootstrap.balls.ContainsKey(originalPosition))
                {
                    PickupItem(i, originalPosition);
                    
                }
                else if (a_Data.Carrying[i].Value == Common.True)
                {
                    if (!Bootstrap.ants.ContainsKey(position))
                    {
                        DropoffItem(i);
                    }
                    else
                    {
                        // Update position of ball
                        UpdateBallPosition(i, originalPosition);
                    }
                }
                
            }
            timer = 0f;
            Debug.Log("Balls size: " + Bootstrap.balls.Count);
        }
        else
        {
            // LERP postional movement
            float timeLeft = 1 - (Bootstrap.Delay - timer) / Bootstrap.Delay;            
            for (int i = 0; i < a_Data.Length; ++i)
            {
                a_Data.Position[i] = new Position { Value = Vector3.Lerp(a_Data.StartPosition[i].Value, a_Data.NextPosition[i].Value, timeLeft) };
            }
        }
    }

    private void CountLocality()
    {
        red = 0;
        blue = 0;

        // I am allowing this space to act as a Torus, ignoring the edges.
        for (int i = 0; i < 8; ++i)
        {
            CheckLocality(GetPosition(i));
        }
    }

    private void UpdatePosition(int index)
    {
        // Update positional data
        int loop_count = 0;
        int newPosition = GetPosition(Random.Range(0, 8));
        int originalPosition = Common.GetGridIndex(a_Data.StartPosition[index].Value);

        a_Data.StartPosition[index] = new StartPosition { Value = a_Data.NextPosition[index].Value };
        if (a_Data.Carrying[index].Value == Common.True)
        {
            while ((OnEdge(newPosition) || Bootstrap.ants.ContainsKey(newPosition) || Bootstrap.balls.ContainsKey(newPosition)) && loop_count < loop_limit)
            {
                newPosition = GetPosition(Random.Range(0, 8));
                ++loop_count;
            }
        }
        else
        {
            while ((OnEdge(newPosition) || Bootstrap.ants.ContainsKey(newPosition)) && loop_count < loop_limit)
            {
                newPosition = GetPosition(Random.Range(0, 8));
                ++loop_count;
            }
        }
        if (loop_count != loop_limit)
        {
            a_Data.NextPosition[index] = new NextPosition { Value = Common.GetGridLocation(newPosition) };
            a_Data.Position[index] = new Position { Value = a_Data.StartPosition[index].Value };
            // After updating position clear the old one and store the new one
            Bootstrap.ants.Remove(originalPosition);
            Bootstrap.ants.Add(newPosition, 0);
        }
        // Else we were stuck in an infinite loop, stay still
    }

    private int GetPosition(int pos)
    {
        int ret_val = 0;
        switch (pos)
        {
            case 0:
                // Top Left
                ret_val = position - Bootstrap.width - 1;
                break;
            case 1:
                // Top Center
                ret_val = position - Bootstrap.width;
                break;
            case 2:
                // Top Right
                ret_val = position - Bootstrap.width + 1;
                break;
            case 3:
                // Left
                ret_val = position - 1;
                break;
            case 4:
                // Right
                ret_val = position + 1;
                break;
            case 5:
                // Bottom Left
                ret_val = position + Bootstrap.width - 1;
                break;
            case 6:
                // Bottom Center
                ret_val = position + Bootstrap.width;
                break;
            case 7:
                // Bottom Right
                ret_val = position + Bootstrap.width + 1;
                break;

        }
        return ret_val;
    }

    private bool OnEdge(int newPosition)
    {
        if (newPosition < Bootstrap.width)
        {
            return true;
        }
        else if (newPosition % Bootstrap.width == 0)
        {
            return true;
        }
        else if (newPosition % Bootstrap.width == Bootstrap.width - 1)
        {
            return true;
        }
        else if (newPosition >= Bootstrap.max_value - Bootstrap.width)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void UpdateRedBlue()
    {
        if (modifier == Common.Red)
        {
            ++red;
        }
        else if (modifier == Common.Blue)
        {
            ++blue;
        }
        else
        {
            Debug.Log("shouldn't happen to modifiers...");
        }
        modifier = -1;
    }

    private void CheckLocality(int checkPosition)
    {
        if (Bootstrap.ants.TryGetValue(checkPosition, out modifier))
        {
            UpdateRedBlue();
        }
    }

    private void PickupItem(int index, int originalPosition)
    {
        // Not sure where this cutoff will be yet
        if (ProbabilityPickup() > 0.0f)
        {
            a_Data.Carrying[index] = new Carrying { Value = Common.True };
            UpdateBallPosition(index, originalPosition);
        }
    }

    private void UpdateBallPosition(int index, int originalPosition)
    {
        if (Bootstrap.balls.TryGetValue(originalPosition, out Entity ball))
        {
            // Remove ball from balls
            Bootstrap.balls.Remove(originalPosition);
            //Update ball to ant position
            Bootstrap.em.SetComponentData(ball, new Position { Value = a_Data.NextPosition[index].Value });
            Bootstrap.balls.Add(Common.GetGridIndex(a_Data.NextPosition[index].Value), ball);
        }
    }

    private float ProbabilityPickup()
    {
        return Random.value;
    }

    private void DropoffItem(int index)
    {
        if (ProbabilityDropoff() > 1.0f)
        {

        }
    }

    private float ProbabilityDropoff()
    {
        return 0f;
    }
}
