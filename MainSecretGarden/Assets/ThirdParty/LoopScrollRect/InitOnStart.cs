using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace SG
{
    [RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
    [DisallowMultipleComponent]
    public class InitOnStart : MonoBehaviour
    {
        public int totalCount = -1;
        LoopScrollRect ls = null;
        void Start()
        {
            ls = GetComponent<LoopScrollRect>();
            ls.totalCount = totalCount;
            ls.RefillCells();
        }

        public void BeginInCount(int beginCount)
        {
            Debug.Log("BeginInCount = "+ beginCount);
            ls.RefillCells(beginCount);
        }
    }
}