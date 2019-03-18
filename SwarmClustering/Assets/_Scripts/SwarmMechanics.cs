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

    public struct Data
    {
        public readonly int Length;
        public ComponentDataArray<Position> Position;
        public ComponentDataArray<Carrying> Carrying;
    }

    [Inject] private Data m_Data;

    // Update is called once per frame
    protected override void OnUpdate()
    {
        // Every Bootstrap.Delay seconds update the ants
        timer += Time.deltaTime;
        if (timer > Bootstrap.Delay)
        {
            for (int i = 0; i < m_Data.Length; ++i)
            {
                position = Common.GetGridIndex(m_Data.Position[i].Value);
                CountLocality();
            }
            timer = 0f;
        }
    }

    private void CountLocality()
    {
        red = 0;
        blue = 0;

        // I am allowing this space to act as a Torus, ignoring the edges.
        CheckPosition(position - Bootstrap.width - 1);
        CheckPosition(position - Bootstrap.width);
        CheckPosition(position - Bootstrap.width + 1);
        CheckPosition(position - 1);
        CheckPosition(position + 1);
        CheckPosition(position + Bootstrap.width - 1);
        CheckPosition(position + Bootstrap.width);
        CheckPosition(position + Bootstrap.width + 1);
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
