using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.Raycasting;
using System.ComponentModel.Design;
using System;

/// <summary>
/// live2d 点击角色
/// </summary>
public class RaycastHitPlayer : MonoBehaviour
{
    #region 变量

    /// <summary>
    /// 点击检测点名称
    /// </summary>
    [SerializeField]
    private DrawableNama[] _doRaycastNameList;

    /// <summary>
    /// <see cref="CubismModel"/> to cast rays against.
    /// </summary>
    [SerializeField]
    public CubismModel Model;

    /// <summary>
    /// <see cref="CubismRaycaster"/> attached to <see cref="Model"/>.
    /// </summary>
    private CubismRaycaster Raycaster { get; set; }

    /// <summary>
    /// Buffer for raycast results.
    /// </summary>
    private CubismRaycastHit[] Results { get; set; }


    /// <summary>
    /// 通知播放动画
    /// </summary>
    private Action<HitLocation> _notifyPlayAnim;

    #endregion

    #region 函数

    #region Unity Event Handling

    /// <summary>
    /// Called by Unity. Initializes instance.
    /// </summary>
    private void Start()
    {
        Model = this.GetComponent<CubismModel>();
        Raycaster = Model.GetComponent<CubismRaycaster>();
        Results = new CubismRaycastHit[4];
    }

    /// <summary>
    /// Called by Unity. Triggers raycasting.
    /// </summary>
    private void Update()
    {
        //// Return early in case of no user interaction.
        //if (!Input.GetMouseButtonDown(0))
        //{
        //    return;
        //}

        //DoRaycast();
    }
    /// <summary>
    /// 数据初始化
    /// </summary>
    /// <param name="action"></param>
    public void Init(Action<HitLocation> action) 
    {
        _notifyPlayAnim = action;
    }

    #endregion

    /// <summary>
    /// 查询当前点击的部位的名称是否在点击列表中
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private HitLocation FindDrawableNamaInDoRaycastNameList(string name) 
    {
        foreach (var item in _doRaycastNameList)
        {
            if (item.Name == name)
                return item.Type;
        }
        return 0;
    }

    /// <summary>
    /// 点击角色部位检测
    /// </summary>
    public void DoRaycast()
    {
        // Cast ray from pointer position.
//        var pos = Vector3.zero;
//#if UNITY_EDITOR
//        pos = Input.mousePosition;
//#else
//        pos = Input.touches[0].deltaPosition;
//#endif
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hitCount = Raycaster.Raycast(ray, Results);


        // Return early if nothing was hit.
        if (hitCount == 0)
        {
            return;
        }

        if (_doRaycastNameList.Length <= 0)
            return;

        //Results[i].Drawable.name
        for (int i = 0; i < hitCount; i++)
        {
            HitLocation type = FindDrawableNamaInDoRaycastNameList(Results[i].Drawable.name);
            if (type != HitLocation.Loc_None)
            {
                _notifyPlayAnim?.Invoke(type);
                return;
            }
        }
    }


    #endregion


}

[Serializable]
public class DrawableNama 
{
    /// <summary>
    /// 部位名称
    /// </summary>
    public string Name;
    /// <summary>
    /// 部位类型 1头部 2身体（上） 3腿部（下）
    /// </summary>
    public HitLocation Type;
}

[Serializable]
public enum HitLocation
{
    Loc_None,
    /// <summary>
    /// 头发
    /// </summary>
    Loc_Hair,
    /// <summary>
    /// 衣服
    /// </summary>
    Loc_Clothes,
    /// <summary>
    /// 下装
    /// </summary>
    Loc_Bottoms
}


