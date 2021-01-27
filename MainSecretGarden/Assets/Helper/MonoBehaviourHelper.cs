using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class MonoBehaviourHelper
{
    public static T CreateTempComponent<T>() where T:Component
    {
        var go=new GameObject();
        var com=go.AddComponent<T>();
        var name = $"temp{typeof(T).ToString()}";
        return com;
    }
}
