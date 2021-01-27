using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
/// <summary>
/// 章节模块基础类
/// </summary>
public class ChapterControllerBasics : MonoBehaviour
{
    #region 字段
    /// <summary>
    /// 下一步 回调
    /// </summary>
    public Action _nextCallback;
    /// <summary>
    /// 设置下一步跳转id
    /// </summary>
    public Action<int> _setNextIndex;
    /// <summary>
    /// 模块画布整体控制
    /// </summary>
    CanvasGroup _canvasGroup;
    /// <summary>
    /// 是否淡出
    /// </summary>
    [SerializeField]
    bool _isFadeOut = true;
    /// <summary>
    /// 是否初始化完毕
    /// </summary>
    bool _isInitial = false;
    #endregion
    #region 属性

    #endregion
    #region 函数
    void Awake()
    {

    }
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// 初始化组件
    /// </summary>
    public virtual void Initial()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _isInitial = true;
    }
    /// <summary>
    /// 下一步
    /// </summary>
    public virtual async void NextStep()
    {
        if (_isFadeOut)
        {
            if (_canvasGroup != null)
            {
                await ChapterTool.FadeInFadeOut(_canvasGroup, 0, 0.25f, new CancellationTokenSource(), () =>
                {
                    _nextCallback?.Invoke();
                    Dispose();
                });
            }
            else
            {
                _nextCallback?.Invoke();
                Dispose();
            }

        }
        else
        {
            _nextCallback?.Invoke();
            Dispose();
        }

    }
    /// <summary>
    /// 重置
    /// </summary>
    public virtual void ResetModule()
    {

    }
    /// <summary>
    /// 退出到主页
    /// </summary>
    public virtual void QuitHomepage()
    {
        OpenModule(false);
        ChapterModuleManager._Instance.Dispose();
        GameSoundPlayer.Instance.PlayBgMusic(MusicHelper.BgMusicLobby);
        //Dispose();
    }
    /// <summary>
    /// 开关自己
    /// </summary>
    /// <param name="isOpen"></param>
    public virtual void OpenModule(bool isOpen)
    {
        gameObject.SetActive(isOpen);
    }
    /// <summary>
    /// 卸载
    /// </summary>
    public virtual void Dispose()
    {
        Destroy(gameObject);
    }
    /// <summary>
    /// 设置下一步跳转的id
    /// </summary>
    /// <param name="index"></param>
    public virtual void SetNextIndex(int index)
    {
        _setNextIndex?.Invoke(index);
    }
    #endregion
}
