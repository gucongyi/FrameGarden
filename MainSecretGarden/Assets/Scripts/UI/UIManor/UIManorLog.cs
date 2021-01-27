using Game.Protocal;
using Google.Protobuf.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManorLog : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private Transform _tra;

    /// <summary>
    /// 循环列表
    /// </summary>
    [SerializeField]
    private LoopVerticalScrollRect lsManorLog;
    /// <summary>
    /// 背景关闭按钮
    /// </summary>
    [SerializeField]
    private Button _bgMask;

    [SerializeField]
    private Button _butClose;

    [SerializeField]
    private GameObject _nullTips;

    public RepeatedField<CSManorLogsStruct> listLogs = new RepeatedField<CSManorLogsStruct>();

    private void Init() 
    {
        _bgMask.onClick.RemoveAllListeners();
        _bgMask.onClick.AddListener(OnClickClose);
        _butClose.onClick.RemoveAllListeners();
        _butClose.onClick.AddListener(OnClickClose);
    }

    void Awake()
    {
        Init();
        UniversalTool.ReadyUIAnimTwo(_canvasGroup, _tra);
    }
    private void OnEnable()
    {
        GetManorLogInfo();
    }
    private void OnClickClose() 
    {
        UniversalTool.CancelUIAnimTwo(_canvasGroup, _tra, UIClose);
    }

    private void UIClose() 
    {
        UIComponent.RemoveUI(UIType.UIManorLog);
    }
    private void GetManorLogInfo() 
    {
        lsManorLog.ClearCells();
        //请求服务器，庄园日志，todo
        CSEmptyManorLogs csEmptyManorLogs = new CSEmptyManorLogs();
        ProtocalManager.Instance().SendCSEmptyManorLogs(csEmptyManorLogs, (scManorLogs) =>
        {
            if (scManorLogs != null)
            {
                listLogs = scManorLogs.StealInfo;
            }
            RefreshLogList();
            //
            UniversalTool.StartUIAnimTwo(_canvasGroup, _tra);
        }, (error) =>
        {
            StaticData.DebugGreen($"{error}");
        }, false);
    }
    private void RefreshLogList()
    {
        lsManorLog.totalCount = listLogs.Count;
        lsManorLog.RefillCells();

        _nullTips.SetActive(false);
        if (listLogs.Count <= 0)
            _nullTips.SetActive(true);
    }

    /// <summary>
    /// 关闭界面进入好友庄园
    /// </summary>
    /// <param name="friendID"></param>
    public void CloseAndEnterFriendManor(long friendID) 
    {
        OnClickClose();
        UIManorComponent uiManorComponent = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
        uiManorComponent.OnButtonEnterFriendManor(friendID);
    }

}
