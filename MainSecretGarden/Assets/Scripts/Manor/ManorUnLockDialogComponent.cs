using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManorUnLockDialogComponent : MonoBehaviour
{
    public List<ManorRegionIdComponent> listPointTransUnLockDiaog;
    public GameObject goRootDialogPerson;
    public Vector3 SetDialog(int regionId)
    {
        Vector3 willLocalPos = Vector3.zero;
        for (int i = 0; i < listPointTransUnLockDiaog.Count; i++)
        {
            if (listPointTransUnLockDiaog[i].regionId == regionId)
            {
                goRootDialogPerson.transform.localPosition = listPointTransUnLockDiaog[i].transform.localPosition;
                willLocalPos = goRootDialogPerson.transform.localPosition;
                goRootDialogPerson.SetActive(true);
                break;
            }
        }
        return willLocalPos;
    }

    public Vector3 GetWillSetPos(int regionId)
    {
        Vector3 willLocalPos = Vector3.zero;
        for (int i = 0; i < listPointTransUnLockDiaog.Count; i++)
        {
            if (listPointTransUnLockDiaog[i].regionId == regionId)
            {
                willLocalPos = listPointTransUnLockDiaog[i].transform.localPosition;
                break;
            }
        }
        return willLocalPos;
    }
    public void FinishDialog()
    {
        goRootDialogPerson.SetActive(false);
    }
}
