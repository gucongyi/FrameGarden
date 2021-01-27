using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollIndexCallback3 : MonoBehaviour,InterfaceScrollCell
{
    public Text text;
    public void ScrollCellIndex(int idx)
    {
        string name = "Cell " + idx.ToString();
        if (text != null)
        {
            text.text = name;
        }
    }
}
