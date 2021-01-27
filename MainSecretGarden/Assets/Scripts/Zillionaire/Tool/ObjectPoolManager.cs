using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 对象池管理器
/// </summary>
class ObjectPoolManager : MonoBehaviour
{

    #region 变量

    private static ObjectPoolManager _instance;
    private Transform _objParent;

    /// <summary>
    /// 对象池
    /// </summary>
    [SerializeField]
    private Dictionary<string, List<GameObject>> _objectPool = new Dictionary<string, List<GameObject>>();

    /// <summary>
    /// 等待释放时间 300s
    /// </summary>
    private int _waitResectObjectTime = 300;
    /// <summary>
    /// 等待释放的对象 每300s释放一次
    /// </summary>
    private Dictionary<GameObject, int> _waitResectObject = new Dictionary<GameObject, int>();
    #endregion

    #region 属性

    public static ObjectPoolManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("RolePos").AddComponent<ObjectPoolManager>();
            }

            return _instance;
        }
    }

    private Transform objParent 
    {
        get 
        {
            if (_objParent == null) 
            {
                CreatParent();
            }

            return _objParent;
        }
    }

    #endregion

    #region 方法

    private float _timer = 0.0f;

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= 1.0f)
        {
            _timer -= 1.0f;
            UpdateReleaseObject();
        }
    }

    private void CreatParent() 
    {
        _objParent = new GameObject("ObjectPoolPar").transform;
        _objParent.SetParent(transform);
    }

    /// <summary>
    /// 创建对象
    /// </summary>
    /// <param name="objectPath"></param>
    /// <returns></returns>
    public async Task<GameObject> CreatObject(string objectPath, Transform parent) 
    {

        GameObject obj = null;
        //判断对象对象池中是否存在
        if (_objectPool.ContainsKey(objectPath) && _objectPool[objectPath].Count > 0)
        {
            Debug.Log("创建对象 从对象池中获取!");
            obj = _objectPool[objectPath][0];
            _objectPool[objectPath].RemoveAt(0);
            //
            if (_waitResectObject.ContainsKey(obj)) 
            {
                _waitResectObject.Remove(obj);
            }
        }
        else 
        {
            Debug.Log("创建对象 创建新对象!");
            var abObj = await ABManager.GetAssetAsync<GameObject>(objectPath);
            obj = SpawnObject(abObj);
        }
        obj.transform.SetParent(parent);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// 创建对象
    /// </summary>
    /// <returns></returns>
    public GameObject CreatObject(string key, GameObject perfab, Transform parent)
    {
        GameObject obj = null;
        if (_objectPool.ContainsKey(key) && _objectPool[key].Count > 0)
        {
            obj = _objectPool[key][0];
            _objectPool[key].RemoveAt(0);
            //
            if (_waitResectObject.ContainsKey(obj))
            {
                _waitResectObject.Remove(obj);
            }
        }
        else 
        {
            obj = SpawnObject(perfab);
        }
        obj.transform.SetParent(parent);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        obj.SetActive(true);
        return obj;
    }

    /// <summary>
    /// 回收对象
    /// </summary>
    /// <param name="objectPath"></param>
    /// <param name="obj"></param>
    public void RecycleObject(string objectPath, GameObject obj) 
    {
        Debug.Log("回收对象");
        obj.transform.SetParent(objParent);
        obj.SetActive(false);
        //
        if (_objectPool.ContainsKey(objectPath))
        {
            _objectPool[objectPath].Add(obj);
        }
        else 
        {

            List<GameObject> objects = new List<GameObject>();
            objects.Add(obj);
            _objectPool.Add(objectPath, objects);
        }

        //
        if (!_waitResectObject.ContainsKey(obj))
        {
            _waitResectObject.Add(obj, 0); 
        }
    }

    /// <summary>
    /// 更新/检测 释放对象
    /// </summary>
    public void UpdateReleaseObject() 
    {
        if (_waitResectObject.Count <= 0)
            return;

        List<GameObject> waitResectObjectList = new List<GameObject>();
        List<GameObject> objectList = new List<GameObject>();
        foreach (var item in _waitResectObject)
        {
            if (item.Value >= _waitResectObjectTime)
            {
                waitResectObjectList.Add(item.Key);
            }
            else 
            {
                objectList.Add(item.Key);
            }
        }
        //
        while (waitResectObjectList.Count > 0) 
        {
            _waitResectObject.Remove(waitResectObjectList[0]);
            GameObject obj = waitResectObjectList[0];
            waitResectObjectList.RemoveAt(0);
            Destroy(obj);
        }
        while (objectList.Count > 0)
        {
            _waitResectObject[objectList[0]] += 1;
            objectList.RemoveAt(0);
        }
    }

    /// <summary>
    /// 创建对象
    /// </summary>
    /// <param name="abObj"></param>
    /// <returns></returns>
    private GameObject SpawnObject(GameObject abObj) 
    {
        var obj = GameObject.Instantiate(abObj);
        obj.SetActive(false);
        return obj;
    }

    #endregion
}

