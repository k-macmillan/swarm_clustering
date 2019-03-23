using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;
public class Common
{
    public const int loop_limit = 100;

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
        return new float3(position / Bootstrap.width, 1, position % Bootstrap.width);
    }

    public static int GetGridIndex(float3 position)
    {
        return (int)(position.x * Bootstrap.width + position.z);
    }

    // Ball colors
    public const int Red = 0;
    public const int Blue = 1;

    // Am I crazy?
    public const int False = 0;
    public const int True = 1;
}
