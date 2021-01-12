using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationExt
{
    public static string version
    {
        get
        {
            return Debug.isDebugBuild ? string.Format("test_{0}", Application.version) : Application.version;
        }
        
    }
}
