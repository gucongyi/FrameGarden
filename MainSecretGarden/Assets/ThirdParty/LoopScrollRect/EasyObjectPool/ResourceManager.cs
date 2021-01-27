﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;


namespace SG
{
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public class ResourceManager : MonoBehaviour
    {
        //obj pool
        private Dictionary<string, Pool> poolDict = new Dictionary<string, Pool>();

        private static ResourceManager mInstance = null;

        public static ResourceManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    GameObject go = new GameObject("ResourceManager", typeof(ResourceManager));
                    go.transform.localPosition = new Vector3(0, 0, 0);
                    // Kanglai: if we have `GO.hideFlags |= HideFlags.DontSave;`, we will encounter Destroy problem when exit playing
                    // However we should keep using this in Play mode only!
                    mInstance = go.GetComponent<ResourceManager>();

                    //if (Application.isPlaying)
                    //{
                    //    DontDestroyOnLoad(mInstance.gameObject);
                    //}
                    //else
                    //{
                    //    Debug.LogWarning("[ResourceManager] You'd better ignore ResourceManager in Editor mode");
                    //}
                }

                return mInstance;
            }
        }
        private void OnDestroy()
        {
            poolDict.Clear();
            mInstance = null;
        }
        public void InitPool(GameObject prefabGO, int size, PoolInflationType type = PoolInflationType.DOUBLE)
        {
            if (prefabGO == null)
            {
                Debug.LogError("[ResourceManager] Invalide prefab name for pooling : prefab is null");
                return;
            }
            if (poolDict.ContainsKey(prefabGO.name))
            {
                return;
            }
            poolDict[prefabGO.name] = new Pool(prefabGO.name, prefabGO, gameObject, size, type);
        }

        /// <summary>
        /// Returns an available object from the pool 
        /// OR null in case the pool does not have any object available & can grow size is false.
        /// </summary>
        /// <param name="poolName"></param>
        /// <returns></returns>
        public GameObject GetObjectFromPool(GameObject prefabGO, bool autoActive = true)
        {
            GameObject result = null;
            string poolName = prefabGO.name;
            if (!poolDict.ContainsKey(poolName))
            {
                InitPool(prefabGO,5,PoolInflationType.INCREMENT);
            }

            if (poolDict.ContainsKey(poolName))
            {
                Pool pool = poolDict[poolName];
                result = pool.NextAvailableObject(autoActive);
                //scenario when no available object is found in pool
#if UNITY_EDITOR
                if (result == null)
                {
                    Debug.LogWarning("[ResourceManager]:No object available in " + poolName);
                }
#endif
            }
#if UNITY_EDITOR
            else
            {
                Debug.LogError("[ResourceManager]:Invalid pool name specified: " + poolName);
            }
#endif
            return result;
        }

        /// <summary>
        /// Return obj to the pool
        /// </summary>
        /// <param name="go"></param>
        public void ReturnObjectToPool(GameObject go)
        {
            PoolObject po = go.GetComponent<PoolObject>();
            if (po == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("Specified object is not a pooled instance: " + go.name);
#endif
            }
            else
            {
                Pool pool = null;
                if (poolDict.TryGetValue(po.poolName, out pool))
                {
                    pool.ReturnObjectToPool(po);
                }
                else
                {
                    Destroy(go);
                }
            }
        }

        /// <summary>
        /// Return obj to the pool
        /// </summary>
        /// <param name="t"></param>
        public void ReturnTransformToPool(Transform t)
        {
            if (t == null)
            {
#if UNITY_EDITOR
                Debug.LogError("[ResourceManager] try to return a null transform to pool!");
#endif
                return;
            }
            ReturnObjectToPool(t.gameObject);
        }
    }
}