using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class SwarmMechanics : ComponentSystem
{
    private static float timer = 0f;
    private static int position;
    private static int pos_x;
    private static int pos_z;
    private static float red;
    private static float blue;
    private static float green;
    private static float yellow;
    private static float purple;

    private static int modifier = -1;
    private static bool meshEnabled = true;

    public static bool meshToggle = false;

    // Pickup/Dropoff probability constants
    private const float k1 = 0.0125f;
    private const float k2 = 0.004f;

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

        if (meshToggle)
        {
            ToggleAntMesh();
        }
    }

    private void ToggleAntMesh()
    {
        if (meshEnabled)
        {
            for (int i = 0; i < a_Data.Length; ++i)
            {
                if (Bootstrap.ants.TryGetValue(Common.GetGridIndex(a_Data.NextPosition[i].Value), out Entity e))
                {
                    PostUpdateCommands.SetSharedComponent(e, Ant.antMeshOff);
                }
            }
        }
        else
        {
            for (int i = 0; i < a_Data.Length; ++i)
            {
                if (Bootstrap.ants.TryGetValue(Common.GetGridIndex(a_Data.NextPosition[i].Value), out Entity e))
                {
                    PostUpdateCommands.SetSharedComponent(e, Ant.antMeshOn);
                }
            }
        }
        meshEnabled = !meshEnabled;
        meshToggle = false;
    }

    private void UpdateAnt(int index)
    {
        position = Common.GetGridIndex(a_Data.NextPosition[index].Value);
        pos_x = (int)a_Data.NextPosition[index].Value.x;
        pos_z = (int)a_Data.NextPosition[index].Value.z;

        // Compute F(x) - Locality
        UpdateLocality();

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

    private void UpdateLocality()
    {
        red = 0;
        blue = 0;
        green = 0;
        yellow = 0;
        purple = 0;

        for (int i = -Common.radius; i <= Common.radius; ++i)
        {
            for (int j = -Common.radius; j <= Common.radius; ++j)
            {
                if (OnGrid(pos_x + i, pos_z + j))
                {
                    CheckLocality(Common.GetGridIndex(pos_x + i, pos_z + j), Mathf.Max(Mathf.Max(Mathf.Abs(i), Mathf.Abs(j)), 1f));
                }
            }
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

        if (!redo)
        {
            // Remove from ants and set new start position
            Bootstrap.ants.TryGetValue(prevPosition, out Entity e);
            Bootstrap.ants.Remove(prevPosition);
            a_Data.StartPosition[index] = new StartPosition { Value = a_Data.NextPosition[index].Value };
            a_Data.Position[index] = new Position { Value = a_Data.StartPosition[index].Value };

            // Add new start position and set next position
            Bootstrap.ants.Add(newPosition, e);
            a_Data.NextPosition[index] = new NextPosition { Value = Common.GetGridLocation(newPosition) };
            
        }
        else
        {
            // It stays still
            a_Data.StartPosition[index] = new StartPosition { Value = a_Data.NextPosition[index].Value };
            a_Data.Position[index] = new Position { Value = a_Data.StartPosition[index].Value };
#if UNITY_EDITOR
            Debug.Log("Ant is stuck at: " + position);
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
        if (Bootstrap.ants.ContainsKey(pos))
        {
            return true;
        }

        var p = Common.GetGridLocation(pos);

        if (!OnGrid(p.x, p.z) || !InRange(p.x, p.z))
        {
            return true;
        }

        return false;
    }

    private bool InRange(float x, float z)
    {
        if (Mathf.Abs(pos_x - x) > 1f)
        {
            return false;
        }

        if (Mathf.Abs(pos_z - z) > 1f)
        {
            return false;
        }

        return true;
    }

    private bool OnGrid(float x, float z)
    {
        if (x >= Common.width || x < 0f)
        {
            return false;
        }

        if (z >= Common.height || z < 0f)
        {
            return false;
        }

        return true;
    }

    private bool OnGrid(int x, int z)
    {
        if (x >= Common.width || x < 0f)
        {
            return false;
        }

        if (z >= Common.height || z < 0f)
        {
            return false;
        }

        return true;
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

    private void UpdateBalls(float dist)
    {
        switch (modifier)
        {
            case Common.Red:
                red += dist;
                break;
            case Common.Blue:
                blue += dist;
                break;
            case Common.Green:
                green += dist;
                break;
            case Common.Yellow:
                yellow += dist;
                break;
            case Common.Purple:
                purple += dist;
                break;
            default:
                modifier = -1;
                break;
        }
        modifier = -1;
    }

    private void CheckLocality(int checkPosition, float dist)
    {
        if (Bootstrap.balls.TryGetValue(checkPosition, out Entity ball))
        {
            modifier = Bootstrap.em.GetComponentData<Faction>(ball).Value;
            UpdateBalls(1f / Mathf.Pow(dist, 3f));
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
        // If we end up going with 5x5 locality check: http://www.xuru.org/rt/PR.asp
        if (Bootstrap.balls.TryGetValue(position, out Entity ball))
        {
            float count = -1f;
            switch (Bootstrap.em.GetComponentData<Faction>(ball).Value)
            {
                case Common.Red:
                    count += red;
                    break;
                case Common.Blue:
                    count += blue;
                    break;
                case Common.Green:
                    count += green;
                    break;
                case Common.Yellow:
                    count += yellow;
                    break;
                case Common.Purple:
                    count += purple;
                    break;
            }
            count = (count * count) / Mathf.Pow(Common.areaValue[Common.radius], 2f);
            return Mathf.Pow(k1 / (k1 + count), 2f);
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
            float count = -1f;
            switch (Bootstrap.em.GetComponentData<Faction>(ball).Value)
            {
                case Common.Red:
                    count += red;
                    break;
                case Common.Blue:
                    count += blue;
                    break;
                case Common.Green:
                    count += green;
                    break;
                case Common.Yellow:
                    count += yellow;
                    break;
                case Common.Purple:
                    count += purple;
                    break;
            }
            count = (count * count) / Mathf.Pow(Common.areaValue[Common.radius], 2f);
            return Mathf.Pow(count / (k2 + count), 2f);

        }
        return 0f;
    }
}
