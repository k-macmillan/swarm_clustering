using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;

public class SwarmMechanics : ComponentSystem
{
    private static float timer = 0f;
    private static int position;
    private static int red;
    private static int blue;
    private static int modifier = -1;

    public struct Data
    {
        public readonly int Length;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<Carrying> Carrying;
        public ComponentDataArray<StartPosition> StartPosition;
        public ComponentDataArray<NextPosition> NextPosition;
    }

    [Inject] private Data m_Data;

    // Update is called once per frame
    protected override void OnUpdate()
    {
        // Every Bootstrap.Delay seconds update the ants
        timer += Time.deltaTime;
        if (timer > Bootstrap.Delay)
        {
            // Update new direction/actions
            for (int i = 0; i < m_Data.Length; ++i)
            {
                position = Common.GetGridIndex(m_Data.NextPosition[i].Value);
                CountLocality();
                
                // Update positional data
                m_Data.StartPosition[i] = new StartPosition { Value = m_Data.NextPosition[i].Value };
                int newPosition = GetPosition(UnityEngine.Random.Range(0, 8));
                while (OnEdge(newPosition))
                {
                    newPosition = GetPosition(UnityEngine.Random.Range(0, 8));
                }
                m_Data.NextPosition[i] = new NextPosition { Value = Common.GetGridLocation(newPosition) };
                m_Data.Position[i] = new Position { Value = m_Data.StartPosition[i].Value };
            }
            timer = 0f;
        }
        else
        {
            // LERP postional movement
            float timeLeft = 1 - (Bootstrap.Delay - timer) / Bootstrap.Delay;            
            for (int i = 0; i < m_Data.Length; ++i)
            {
                m_Data.Position[i] = new Position { Value = Vector3.Lerp(m_Data.StartPosition[i].Value, m_Data.NextPosition[i].Value, timeLeft) };
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
            CheckPosition(GetPosition(i));
        }
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

    private void CheckPosition(int checkPosition)
    {
        if (Bootstrap.grid.TryGetValue(checkPosition, out modifier))
        {
            UpdateRedBlue();
        }
    }
}
