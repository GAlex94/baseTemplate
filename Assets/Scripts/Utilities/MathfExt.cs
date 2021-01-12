using UnityEngine;

public class MathfExt : MonoBehaviour
{
    public static int RoundBy(float value, int round)
    {
        return (int)(Mathf.Round(value / round) * round);
    }

    public static float RoundFloatBy(float value, int round)
    {
        return Mathf.Round(value * round) / round;
    }

    public static int CeilBy(float value, int round)
    {
        return (int)(Mathf.Ceil(value / round) * round);
    }

    public static float CeilFloatBy(float value, int round)
    {
        return Mathf.Ceil(value * round) / round;
    }


    public static int FloorBy(float value, int round)
    {
        return (int)(Mathf.Floor(value / round) * round);
    }
}