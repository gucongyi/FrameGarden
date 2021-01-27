using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class LoopScrollPrefabSource 
    {
        public GameObject prefabGO;
        public int poolSize = 5;

        private bool inited = false;

        public virtual GameObject GetObject()
        {
            if (!inited)
            {
                SG.ResourceManager.Instance.InitPool(prefabGO, poolSize);
                inited = true;
            }
            return SG.ResourceManager.Instance.GetObjectFromPool(prefabGO);
        }
        public virtual void ReturnObject(Transform go)
        {
            go.SendMessage("ScrollCellReturn", SendMessageOptions.DontRequireReceiver);
            SG.ResourceManager.Instance.ReturnObjectToPool(go.gameObject);
        }
    }
}
