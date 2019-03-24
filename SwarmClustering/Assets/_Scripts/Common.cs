using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
public class Common
{
    public const int loop_limit = 100;
    public static int width = 200;
    public static int height = 200;
    public static int max_value = width * height - 1;
    public static float Delay = 0.15625f;


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

    public static float3 GetGridLocation(int position)
    {
        return new float3(position / width, 1, position % width);
    }

    public static int GetGridIndex(float3 position)
    {
        return (int)(position.x * width + position.z);
    }

    // Ball colors
    public const int Red = 0;
    public const int Blue = 1;

    // Am I crazy?
    public const int False = 0;
    public const int True = 1;
}
