using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;


public class Common
{
    public const int balls = 100;
    public const int ants = 500;
    public const int loop_limit = 100;

    public static int width = 200;
    public static int height = 200;
    public static int max_value = width * height - 1;
    public static int radius = 1;
    public static float Delay = 0.15625f;
    public static float3 vecOffset = new float3(0.125f, 0f, 0f);
    public static float[] areaValue = { 0f, 9f, 11.75f, 12.63888f, 13.13888f, 13.45888f };
    //public static float[] areaValue = { 0f, 9f, 13f, 15.66667f, 17.66667f, 19.26667f };


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
        return new float3(position % width, 1, position / height);
    }

    public static int GetGridIndex(float3 position)
    {
        return (int)(position.z * width + position.x);
    }

    public static int GetGridIndex(int x, int z)
    {
        return z * width + x;
    }

    

    // Ball colors
    public const int Red = 0;
    public const int Blue = 1;
    public const int Green = 2;
    public const int Yellow = 3;
    public const int Purple = 4;

    // Am I crazy?
    public const int False = 0;
    public const int True = 1;

    // Edges
    public const int Top = 0;
    public const int Left = 1;
    public const int Right = 2;
    public const int Bottom = 3;
}
