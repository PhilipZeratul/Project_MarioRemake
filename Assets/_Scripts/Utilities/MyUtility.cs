using UnityEngine;
using System;


public static class MyUtility
{
    public static bool NearlyEqual (float a, float b, float epslon = 0.0001f)
    {
        return Mathf.Abs(a - b) < epslon;
    }

    public static T GetComponentRecursiveUp<T>(this GameObject go)
    {
        T component = go.GetComponent<T>();
        if (component == null && go.transform.parent != null)
            component = GetComponentRecursiveUp<T>(go.transform.parent.gameObject);

        return component;
    }
}
