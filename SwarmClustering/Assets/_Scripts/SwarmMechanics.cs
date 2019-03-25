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
    private static int[] Edge = { 0, 0, 0, 0 };

    // Pickup/Dropoff probability constants
    private const float k1 = 1f;
    private const float k2 = 0.25f;

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
        if (timer >= Common.Delay)
        {
            timer = 0f;
            // Update new direction/actions
            for (int i = 0; i < a_Data.Length; ++i)
            {
                UpdateAnt(i);
            }
        }
        else
        {
            // LERP postional movement
            float timeLeft = 1 - (Common.Delay - timer) / Common.Delay;            
            for (int i = 0; i < a_Data.Length; ++i)
            {
                a_Data.Position[i] = new Position { Value = Vector3.Lerp(a_Data.StartPosition[i].Value, a_Data.NextPosition[i].Value, timeLeft) };
            }
        }
    }

    private void UpdateAnt(int index)
    {
        position = Common.GetGridIndex(a_Data.NextPosition[index].Value);
        // Compute F(x) - Locality
        CountLocality();

        if (a_Data.Carrying[index].Value == Common.False && Bootstrap.balls.ContainsKey(position))
        {
            // IF unloaded and circle
            PickupItem(index);
        }
        else if (a_Data.Carrying[index].Value == Common.True)
        {
            // ELSE IF loaded and empty
            DropoffItem(index);
        }

        // Handle movement
        if (a_Data.Carrying[index].Value == Common.False)
        {
            // Move rando no ants
            UpdateAntPosition(index);
        }
        else
        {
            // Move rando no ants no balls
            UpdateAntPosition(index, true);
            UpdateBallPosition(index);
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

    private void UpdateAntPosition(int index, bool noBalls = false)
    {
        int loop_count = 0;
        int newPosition = GetPosition(Random.Range(0, 8));
        int prevPosition = Common.GetGridIndex(a_Data.NextPosition[index].Value);
        bool redo = InvalidMove(newPosition);
        if (noBalls)
        {
            redo = redo || Bootstrap.balls.ContainsKey(newPosition);
        }
        while (redo && ++loop_count < Common.loop_limit)
        {
            newPosition = GetPosition(Random.Range(0, 8));
            redo = InvalidMove(newPosition);
            if (noBalls)
            {
                redo = redo || Bootstrap.balls.ContainsKey(newPosition);
            }
        }

        if (loop_count < Common.loop_limit)
        {
            if (!InvalidMove(newPosition)) {
                // Remove from ants and set new start position
                Bootstrap.ants.Remove(prevPosition);
                a_Data.StartPosition[index] = new StartPosition { Value = a_Data.NextPosition[index].Value };
                a_Data.Position[index] = new Position { Value = a_Data.StartPosition[index].Value };

                // Add new start position and set next position
                Bootstrap.ants.Add(newPosition, 0);
                a_Data.NextPosition[index] = new NextPosition { Value = Common.GetGridLocation(newPosition) };
            }
            else
            {
                Debug.Log("Why was this invalid?");
            }
        }
        else
        {
            // It stays still
            a_Data.StartPosition[index] = new StartPosition { Value = a_Data.NextPosition[index].Value };
            a_Data.Position[index] = new Position { Value = a_Data.StartPosition[index].Value };
#if UNITY_EDITOR
            Debug.Log("Ant is stuck...");
            Debug.Log(position);
            Debug.Log("Max value: " + Common.max_value);
#endif
        }
    }

    private void UpdateBallPosition(int index)
    {
        if (Bootstrap.balls.TryGetValue(position, out Entity ball))
        {
            // Remove ball from balls
            Bootstrap.balls.Remove(position);
            //Update ball to ant position
            Bootstrap.em.SetComponentData(ball, new Position { Value = a_Data.NextPosition[index].Value });
            Bootstrap.balls.Add(Common.GetGridIndex(a_Data.NextPosition[index].Value), ball);
        }
    }

    private bool InvalidMove(int pos)
    {
        bool ret_val = Bootstrap.ants.ContainsKey(pos);
        if (ret_val)
            return true;


        if (EdgeValue())
        {
            if (Edge[Common.Top] == 1)
            {
                Edge[0] = 0;
                if (pos < 0)
                {
                    ret_val = true;
                }
            }

            if (Edge[Common.Left] == 1)
            {
                Edge[1] = 0;
                if (pos - 1 % Common.width == Common.width - 1)
                {
                    ret_val = true;
                }
            }

            if (Edge[Common.Right] == 1)
            {
                Edge[2] = 0;
                if (pos % Common.width == 0)
                {
                    ret_val = true;
                }
            }

            if (Edge[Common.Bottom] == 1)
            {
                Edge[3] = 0;
                if (pos > Common.max_value)
                {
                    ret_val = true;
                }
            }
        }

        return ret_val;
    }


    private int GetPosition(int pos)
    {
        int ret_val = 0;
        switch (pos)
        {
            case 0:
                // Top Left
                ret_val = position - (Common.width + 1);
                break;
            case 1:
                // Top Center
                ret_val = position - Common.width;
                break;
            case 2:
                // Top Right
                ret_val = position - (Common.width - 1);
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
                ret_val = position + (Common.width - 1);
                break;
            case 6:
                // Bottom Center
                ret_val = position + Common.width;
                break;
            case 7:
                // Bottom Right
                ret_val = position + (Common.width + 1);
                break;
            default:
#if UNITY_EDITOR

                Debug.Log("How is this possible?");
#endif
                break;

        }
        return ret_val;
    }

    private bool EdgeValue()
    {
        if (position < Common.width)
        {
            Edge[Common.Top] = 1;
            return true;
        }
        if (position % Common.width == 0)
        {
            Edge[Common.Left] = 1;
            return true;
        }
        if (position % Common.width == Common.width - 1)
        {
            Edge[Common.Right] = 1;
            return true;
        }
        if (position > (Common.max_value - Common.width))
        {
            Edge[Common.Bottom] = 1;
            return true;
        }
        return false;
    }

    private void UpdateRedBlue()
    {
        switch (modifier)
        {
            case Common.Red:
                ++red;
                break;
            case Common.Blue:
                ++blue;
                break;
            default:
                modifier = -1;
                break;
        }
        modifier = -1;
    }

    private void CheckLocality(int checkPosition)
    {
        if (Bootstrap.balls.TryGetValue(checkPosition, out Entity ball))
        {
            modifier = Bootstrap.em.GetComponentData<Faction>(ball).Value;
            UpdateRedBlue();
        }
    }

    private void PickupItem(int index)
    {
        if (ProbabilityPickup() > Random.value)
        {
            a_Data.Carrying[index] = new Carrying { Value = Common.True };
        }
    }

    private float ProbabilityPickup()
    {
        if(Bootstrap.balls.TryGetValue(position, out Entity ball))
        {
            switch (Bootstrap.em.GetComponentData<Faction>(ball).Value)
            {
                case Common.Red:
                    //return Mathf.Pow(k1 / (k1 + red), 1f);
                    return 1f - 0.9f * Mathf.Pow(0.5f * (red - 2f), 3f) - 0.9f;
                case Common.Blue:
                    //return Mathf.Pow(k1 / (k1 + blue), 1f);
                    return 1f - 0.9f * Mathf.Pow(0.5f * (blue - 2f), 3f) - 0.9f;
            }
        }
        return 0f;
    }

    private void DropoffItem(int index)
    {
        if (ProbabilityDropoff() > Random.value)
        {
            a_Data.Carrying[index] = new Carrying { Value = Common.False };
        }
    }

    private float ProbabilityDropoff()
    {
        if (Bootstrap.balls.TryGetValue(position, out Entity ball))
        {
            switch (Bootstrap.em.GetComponentData<Faction>(ball).Value)
            {
                case Common.Red:
                    //return Mathf.Pow(red / (k2 + red), 2f);
                    return 0.4f * Mathf.Pow(red - 2f, 3f) + Mathf.Pow(red / (0.25f + red), 2f);
                case Common.Blue:
                    //return Mathf.Pow(blue / (k2 + blue), 2f);
                    return 0.4f * Mathf.Pow(blue - 2f, 3f) + Mathf.Pow(blue / (0.25f + blue), 2f);
            }
        }
        return 0f;
    }
}
