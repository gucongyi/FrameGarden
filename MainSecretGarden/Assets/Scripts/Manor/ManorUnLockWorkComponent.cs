using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManorUnLockWorkComponent : MonoBehaviour
{
    public GameObject goWorker;
    public List<ManorRegionIdComponent> listPointTransUnLockWork;

    internal void SetWorker(int regionId)
    {
        for (int i = 0; i < listPointTransUnLockWork.Count; i++)
        {
            if (listPointTransUnLockWork[i].regionId == regionId)
            {
                goWorker.transform.localPosition = listPointTransUnLockWork[i].transform.localPosition;
                goWorker.SetActive(true);
                break;
            }
        }
    }
    public void FinishWork()
    {
        goWorker.SetActive(false);
    }
}
