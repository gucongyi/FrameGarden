using System.Collections.Generic;
using UnityEngine;

public class ReferenceCollector : MonoBehaviour
{
    public List<Object> listKeyValue = new List<Object>();
    public T Get<T>(string key) where T: Object
    {
        T value = default(T);
        if (listKeyValue.Count > 0)
        {
            foreach (var eachItem in listKeyValue)
            {
                if (eachItem.name == key)
                {
                    value = eachItem as T;
                    break;
                }
            }
        }
        return value;
    }
}