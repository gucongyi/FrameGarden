using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UICameraManager : MonoBehaviour
{
    public static UICameraManager _instance;
    /// <summary>
    /// ui相机原始位置
    /// </summary>
    Vector3 _uiCameraOldVector;
    /// <summary>
    /// ui相机原始旋转
    /// </summary>
    Quaternion _uiCameraOldRotation;
    /// <summary>
    /// ui相机原始尺寸
    /// </summary>
    Vector3 _uiCameraOldScale;
    /// <summary>
    /// ui相机是否被占用
    /// </summary>
    bool _isUiCamerAoccupy = false;
    private void Awake()
    {
        _instance = this;
        SetDefault();
    }
    public void SetAddWithBaseType()
    {
        //设置相机
        Camera.main.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
        Camera.main.cullingMask = 1 << 5;//只渲染UI层
    }
    public void SetDefault()
    {
        //设置相机
        Camera.main.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Base;
        Camera.main.cullingMask = -1;//渲染所有层
    }
    /// <summary>
    /// 获取ui相机
    /// 2020/11/9 huangjiangdong
    /// </summary>
    /// <returns></returns>
    public Camera GetUiCamera()
    {
        if (_isUiCamerAoccupy)
        {
            return null;
        }
        _uiCameraOldVector = Camera.main.transform.position;
        _uiCameraOldRotation = Camera.main.transform.rotation;
        _uiCameraOldScale = Camera.main.transform.localScale;
        _isUiCamerAoccupy = true;
        return Camera.main;
    }
    /// <summary>
    /// 恢复ui主相机
    /// 2020/11/9 huangjiangdong
    /// </summary>
    public void RestoreUiCamera()
    {
        Camera.main.transform.position = _uiCameraOldVector;
        Camera.main.transform.rotation = _uiCameraOldRotation;
        Camera.main.transform.localScale = _uiCameraOldScale;
        _uiCameraOldVector = Vector3.zero;
        _uiCameraOldRotation = new Quaternion();
        _uiCameraOldScale = Vector3.one;
        _isUiCamerAoccupy = false;
    }
}
