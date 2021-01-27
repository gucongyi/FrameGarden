using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class FindGuidePath : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StringBuilder path = new StringBuilder();
        List<string> pathString = new List<string>();
        Transform trans = transform;
        pathString.Add(trans.name);
        while (trans.parent!=null)
        {
            trans = trans.parent;
            pathString.Add(trans.name);
        }
        for (int i = pathString.Count-1; i >=0 ; i--)
        {
            if (i > 0)
            {
                path.Append(pathString[i] + "/");
            }
            else
            {
                path.Append(pathString[i]);
            }
        }
        StaticData.DebugGreen(path.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
