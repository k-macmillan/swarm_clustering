using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
public class Common
{
    public const int loop_limit = 100;
    public static int width = 1;
    public static int height = 1;
    public static float3 min_value = new float3(0,0,0);
    public static float3 max_value = new float3(1,0,0);
    public static float Delay = 0.15625f;
    public static float3 Global;
    public static float3 vecOffset = new float3(0.125f, 0f, 0f);


    /// <summary>
    /// Returns the mesh for the given string
    /// </summary>
    /// <param name="protoName">Component name</param>
    /// <returns></returns>
    public static MeshInstanceRenderer GetLookFromPrototype(string protoName)
    {
        var proto = GameObject.Find(protoName);
        var result = proto.GetComponent<MeshInstanceRendererComponent>().Value;
        Object.Destroy(proto);
        return result;
    }

    public static void DestroySphere(string protoName)
    {
        var proto = GameObject.Find(protoName);
        Object.Destroy(proto);
    }

    public static float3 SetPosition()
    {
        float x = UnityEngine.Random.value;
        return new float3(x, 1, Evaluation(x));
    }

    public static float Evaluation(float x)
    {
        return (Mathf.Pow(2, -2 * Mathf.Pow((x - 0.1f) / 0.9f, 2))
                * Mathf.Pow(Mathf.Sin(5 * Mathf.PI * x), 2));
    }

    // Am I crazy?
    public const int False = 0;
    public const int True = 1;

    // Edges
    public const int Top = 0;
    public const int Left = 1;
    public const int Right = 2;
    public const int Bottom = 3;
}
