using UnityEngine;


public static class MyUtility
{
    public static bool NearlyEqual (float a, float b, float epslon = 0.0001f)
    {
        return Mathf.Abs(a - b) < epslon;
    }
}
