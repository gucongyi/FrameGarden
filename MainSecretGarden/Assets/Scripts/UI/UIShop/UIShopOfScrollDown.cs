using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShopOfScrollDown : MonoBehaviour
{
    public LoopVerticalScrollRect lsOrnamentList;
    public Transform content;

    private int index;//移动的物品下标

    private int moveNum;//移动的数量

    private void Start()
    {
        moveNum = content.childCount;
    }

    public void ScroToDown()
    {
        index = content.GetChild(0).GetComponent<UIOrnament>().index+moveNum;

        lsOrnamentList.SrollToCell(index, 2500);
    }
}
