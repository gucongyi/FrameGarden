using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 旋转效果
/// </summary>
public class UIRotate : MonoBehaviour
{
    #region 属性

    [SerializeField]
    private GameObject _roleIcon;

    private int _halfSize;
    private GameObject[] _roleList;
    private Dictionary<int, GameObject> _roleDefIndexList = new Dictionary<int, GameObject>();

    /// <summary>
    /// 半径
    /// </summary>
    [SerializeField]
    private int _rotR = 300;

    /// <summary>
    /// 间隔角度
    /// </summary>
    private float _rotAngle;

    #endregion

    #region 方法

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Init() 
    {
        //获取角色数组长度
        int roleCount = 7;


        if (roleCount % 2 == 0)//添加一个默认选项
            roleCount += 1;

        //计算中心点
        _halfSize = (roleCount - 1) / 2;

        //园内角度
        _rotAngle = 360 / roleCount;
        _roleList = new GameObject[roleCount];

        for (int i = 0; i < roleCount; i++)
        {
            _roleList[i] = Instantiate(_roleIcon, transform);
            _roleList[i].SetActive(true);
            _roleList[i].transform.Find("Num").GetComponent<Text>().text = i.ToString();
            _roleDefIndexList.Add(i, _roleList[i]);
            SetPosition(i);
            SetDeepin(i);
        }

        Debug.Log("UIRotate init");
    }

    /// <summary>
    /// 设置物体的位置
    /// </summary>
    /// <param name="index"> 在园中的位置</param>
    private void SetPosition(int index) 
    {
        float x = 0;
        float z = 0;
        if (index < _halfSize)
        {
            int id = _halfSize - index;
            x = _rotR * Mathf.Sin(_rotAngle * id);
            z = -_rotR * Mathf.Cos(_rotAngle * id);
        }
        else if (index > _halfSize)
        {
            int id = index - _halfSize;
            x = -_rotR * Mathf.Sin(_rotAngle * id);
            z = -_rotR * Mathf.Cos(_rotAngle * id);
        }
        else
        {
            x = 0;
            z = -_rotR;
        }

        Debug.Log("uiRotate index + x = "+ index+"* "+ x);
        //_roleList[index]
        Tweener tweener = _roleList[index].transform.GetComponent<RectTransform>().DOLocalMove(new Vector3(x, 0, z), 0.6f);


        if (index != _halfSize)
        {
            _roleList[index].GetComponent<Image>().color = Color.green;
        }
        else 
        {
            _roleList[index].GetComponent<Image>().color = Color.white;
        }
            
    }

    /// <summary>
    /// 设置层级
    /// </summary>
    /// <param name="index"></param>
    private void SetDeepin(int index) 
    {
        int deepin = 0;
        if (index < _halfSize)
        {
            deepin = index;
        }
        else if (index > _halfSize)
        {
            deepin = _roleList.Length - (1 + index);
        }
        else
        {
            deepin = _halfSize;
        }
        _roleList[index].transform.GetComponent<RectTransform>().SetSiblingIndex(deepin);
    }

    /// <summary>
    /// 获取角色现在的下标
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    private int GetRoleIndex(GameObject role) 
    {
        for (int i = 0; i < _roleList.Length; i++)
        {
            if (role == _roleList[i])
                return i;
        }
        return -1;
    }

    /// <summary>
    /// 获取角色默认下标
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    private int GetRoleDefIndex(GameObject role) 
    {
        foreach (var item in _roleDefIndexList) 
        {
            if (item.Value == role)
                return item.Key;
        }

        return -1;
    }

    /// <summary>
    /// 向后翻页
    /// </summary>
    public void OnNext()
    {
        var length = _roleList.Length;
        for (var i = 0; i < length; i++)
        {
            var temp = _roleList[i];
            _roleList[i] = _roleList[length - 1];
            _roleList[length - 1] = temp;
        }
        for (var i = 0; i < length; i++)
        {
            SetPosition(i);
            SetDeepin(i);
        }
    }

    public void OnForward() 
    {
        var length = _roleList.Length;
        for (var i = length - 1; i >= 0 ; i--)
        {
            var temp = _roleList[i];
            _roleList[i] = _roleList[length - 1];
            _roleList[length - 1] = temp;
        }
        for (var i = 0; i < length; i++)
        {
            SetPosition(i);
            SetDeepin(i);
        }
    }

    public void OnClickIndex(int targetIndex) 
    {

        if (!_roleDefIndexList.ContainsKey(targetIndex))
            return;
        var role = _roleDefIndexList[targetIndex];
        var index = GetRoleIndex(role);
        if (index == _halfSize)
            return;
        if (index < _halfSize)
        {
            for (int i = 0; i < _halfSize - index; i++)
            {
                OnNext();
            }
        }
        else if (index > _halfSize)
        {
            for (int i = 0; i < index - _halfSize; i++)
            {
                OnForward();
            }
        }
    }



    #endregion



}
