using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManorUnLockWorkShedComponent : MonoBehaviour
{
    public GameObject goWorkShed;
    public List<ManorRegionIdComponent> listPointTransUnLockWorkShed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (goWorkShed!=null&& StaticData.GetUIWorldHandleComponent()!=null)
        {
            StaticData.GetUIWorldHandleComponent().SetWorkShedHandleFinger(goWorkShed);
        }
    }

    internal void SetWorkShed(int regionId)
    {
        for (int i = 0; i < listPointTransUnLockWorkShed.Count; i++)
        {
            if (listPointTransUnLockWorkShed[i].regionId== regionId)
            {
                goWorkShed.transform.localPosition = listPointTransUnLockWorkShed[i].transform.localPosition;
                goWorkShed.SetActive(true);
                Root2dSceneManager._instance.UpdateSortLayer(true, true, true);
                break;
            }
        }
    }
    public void ClearWorkShed()
    {
        goWorkShed.SetActive(false);
    }
}
