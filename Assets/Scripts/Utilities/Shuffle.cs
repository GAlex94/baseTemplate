using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UtilityDG
{
    public static void Shuffle<T>(T[] objects)
    {
        for (int t = 0; t < objects.Length; t++)
        {
            T tmp = objects[t];
            int r = UnityEngine.Random.Range(t, objects.Length);
            objects[t] = objects[r];
            objects[r] = tmp;
        }
    }

    public static void Shuffle<T>(List<T> objects)
    {
        for (int t = 0; t < objects.Count; t++)
        {
            T tmp = objects[t];
            int r = UnityEngine.Random.Range(t, objects.Count);
            objects[t] = objects[r];
            objects[r] = tmp;
        }
    }
}
