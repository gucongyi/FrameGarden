using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManorZoomNotAffectWorldPos : MonoBehaviour
{
    Transform trans =null;
    public void SetWorldPos(Transform transFollow)
    {
        this.trans = transFollow;
    }
    private void Update()
    {
        if (trans != null)
        {
            transform.position = trans.position;
        }
    }
}
