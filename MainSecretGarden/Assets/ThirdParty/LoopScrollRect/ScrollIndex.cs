using System;
using UnityEngine;

public class ScrollIndex : MonoBehaviour,InterfaceScrollCell
{
    public Action<int> IndexAction;
    int index;
    public string info;

    public void ScrollCellIndex(int idx)
    {
        index = idx;
        IndexAction?.Invoke(index);
    }
}
