using Unity.Rendering;
using UnityEngine;
public class Common
{
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
}
