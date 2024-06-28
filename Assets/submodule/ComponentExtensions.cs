using System;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentExtensions
{
    public static T GetOrAddComponent<T>(this Component component) where T : Component => component.GetComponent<T>() ?? component.gameObject.AddComponent<T>();
}

public static class UtilityExtensions
{
    public static T[] GetComponentsOnlyInChildren<T>(this MonoBehaviour script, bool isRecursive = false) where T : class
    {
        if (isRecursive)
            return script.GetComponentsOnlyInChildren_Recursive<T>();
        return script.GetComponentsOnlyInChildren_NonRecursive<T>();
    }

    #region Find NonRecursive
    public static T[] GetComponentsOnlyInChildren_NonRecursive<T>(Transform parent) where T : class
    {
        if (parent.childCount <= 0) return null;

        List<T> output = new List<T>();
        T component;

        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).TryGetComponent<T>(out component) == true)
            {
                output.Add(component);
            }
        }
        if (output.Count > 0)
            return output.ToArray();

        return null;
    }

    public static T GetComponentOnlyInChildren_NonRecursive<T>(Transform parent) where T : class
    {
        if (parent.childCount <= 0) return null;

        T output = null;

        for (int i = 0; i < parent.childCount; i++)
        {
            var component = parent.GetChild(i).GetComponent<T>();
            if (component != null)
                output = component;
        }
        if (output != null)
            return output;

        return null;
    }

    static T[] GetComponentsOnlyInChildren_NonRecursive<T>(this MonoBehaviour parent) where T : class
    {
        if (parent.transform.childCount <= 0) return null;

        var output = new List<T>();

        for (int i = 0; i < parent.transform.childCount; i++)
        {
            var component = parent.transform.GetChild(i).GetComponent<T>();
            if (component != null)
                output.Add(component);
        }
        if (output.Count > 0)
            return output.ToArray();

        return null;
    }
    #endregion

    #region Find Recursive

    static T[] GetComponentsOnlyInChildren_Recursive<T>(this MonoBehaviour parent) where T : class
    {
        if (parent.transform.childCount <= 0) return null;

        var transforms = new HashSet<Transform>(parent.GetComponentsInChildren<Transform>());
        transforms.Remove(parent.transform);

        var output = new List<T>();
        foreach (var child in transforms)
        {
            var component = child.GetComponent<T>();
            if (component != null)
            {
                output.Add(component);
            }
        }

        if (output.Count > 0)
            return output.ToArray();

        return null;
    }
    #endregion

    #region Find With Tag
    public static T[] GetComponentsOnlyInChildrenWithTag_Recursive<T>(Transform parent, string tag) where T : class
    {
        if (parent.transform.childCount <= 0) return null;

        var transforms = new HashSet<Transform>(parent.GetComponentsInChildren<Transform>());
        transforms.Remove(parent.transform);

        var output = new List<T>();
        foreach (var child in transforms)
        {
            if(child.tag != tag)
                continue;
            
            var component = child.GetComponent<T>();
            if (component != null)
            {
                output.Add(component);
            }
        }

        if (output.Count > 0)
            return output.ToArray();

        return null;
    }
    public static T GetComponentOnlyInChildrenWithTag_Recursive<T>(Transform parent, string tag) where T : class
    {
        if (parent.transform.childCount <= 0) return null;

        var transforms = new HashSet<Transform>(parent.GetComponentsInChildren<Transform>());
        transforms.Remove(parent.transform);

        T output = null;
        foreach (var child in transforms)
        {
            if (child.tag != tag)
                continue;

            var component = child.GetComponent<T>();
            if (component != null)
            {
                output = component;
            }
        }

        if (output != null)
            return output;

        return null;
    }

    #endregion

    /* This is still fastest...
     * 
     foreach (var i in GetComponentsInChildren<INotify>()
     {
        if (i == GetComponent<INotify>()) continue;
        i.Notify();
     }
     * 
     */

    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static Texture2D ResizeTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        if (result == null)
            return null;
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }
    public static Sprite ConvertToSprite(this Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }
}