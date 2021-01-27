using Live2D.Cubism.Framework.LookAt;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// live2d 角色眼睛看向的目标
/// </summary>
public class CubismLookTarget : MonoBehaviour, ICubismLookTarget
{
    #region 变量

    private Vector3 _initPos = Vector3.zero;
    private bool _isActive = false;

    private Vector3 _mouseButtonDownPoint = Vector3.zero;

    #endregion

    void Start() 
    {
        DontDestroyOnLoad(gameObject);
    }

    public void InitData(Vector3 pos, bool isActive) 
    {
        _initPos = pos;
        _isActive = isActive;

    }

    /// <summary>
    /// 设置角色看向目标是否激活
    /// </summary>
    public void SetLookTargetActive(bool isActive) 
    {
        _isActive = isActive;
    }

    /// <summary>
    /// 获取目标点位置
    /// </summary>
    /// <returns></returns>
    public Vector3 GetPosition()
    {
        if (!Input.GetMouseButton(0))
            return _initPos;

        var targetPosition = Input.mousePosition;
        //targetPosition = (Camera.main.ScreenToViewportPoint(targetPosition) * 2) - Vector3.one;
        targetPosition = Camera.main.ScreenToWorldPoint(targetPosition) ;
        return targetPosition;
    }

    /// <summary>
    /// 功能是否被激活
    /// </summary>
    /// <returns></returns>
    public bool IsActive()
    {
        return _isActive;
    }

}
