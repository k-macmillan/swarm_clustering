using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public Transform pointPrefab;
    [Range(1, 100)] public int resolution = 10;

    public void Awake()
    {
        Vector3 position;
        float step = 1 / 40f;
        Vector3 scale = Vector3.one * step;
        position.y = 2f;
        for (int i = 0; i < 1000; i++)
        {
            Transform point = Instantiate(pointPrefab);
            position.x = (i / 1000f + 0.005f);
            position.z = (Mathf.Pow(2, -2 * Mathf.Pow(((float)position.x - 0.1f) / 0.9f, 2))
                * Mathf.Pow(Mathf.Sin(5 * Mathf.PI * position.x), 2));
            point.localPosition = position;
            point.localScale = scale;
            point.SetParent(transform);
        }
    }

}