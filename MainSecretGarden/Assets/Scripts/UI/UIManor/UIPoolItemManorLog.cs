using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoolItemManorLog : MonoBehaviour, InterfaceScrollCell
{
    /// <summary>
    /// 
    /// </summary>
    [SerializeField]
    private UIManorLog _uiManorLog;

    [SerializeField]
    private Text _friendLv;
    [SerializeField]
    private Text _friendName;
    [SerializeField]
    private Image _friednIcon;

    [SerializeField]
    private Text _time;
    [SerializeField]
    private Text _content;
    [SerializeField]
    private Text _contentTitle;

    [SerializeField]
    private Button _butExtract;
    [SerializeField]
    private Button _butVisit;


    private long _friendID;
    //庄园好友信息
    private FriendStealInfo curFriendStealInfo = new FriendStealInfo();

    /// <summary>
    /// 添加事件
    /// </summary>
    private void AddEvent() 
    {
        _butExtract.onClick.RemoveAllListeners();
        _butExtract.onClick.AddListener(OnClickEnterFriendManor);
        _butVisit.onClick.RemoveAllListeners();
        _butVisit.onClick.AddListener(OnClickEnterFriendManor);
    }

    /// <summary>
    /// 设置好友头像
    /// </summary>
    /// <param name="friendIconID"></param>
    private async void SetFriendIcon(int friendIconID) 
    {
        string path = string.Empty;
        path = StaticData.configExcel.GetPlayerAvatarByID(friendIconID).Icon;
        _friednIcon.sprite = await ABManager.GetAssetAsync<Sprite>(path);
    }

    public void ScrollCellIndex(int idx)
    {
        _uiManorLog = UIComponent.GetComponentHaveExist<UIManorLog>(UIType.UIManorLog);
        AddEvent();

        var logInfo = _uiManorLog.listLogs[idx];

        _friendID = logInfo.Uid;
        //var friendInfo =  StaticData.GetFriendData(_friendID);
        _friendLv.text = StaticData.GetPlayerLevelAndCurrExp(logInfo.Experience).level.ToString();
        _friendName.text = logInfo.Account;
        SetFriendIcon(logInfo.Image);

        curFriendStealInfo.nickname= logInfo.Account;
        curFriendStealInfo.uid= logInfo.Uid;
        curFriendStealInfo.headIcon = logInfo.Image;
        curFriendStealInfo.playerLevelAndCurrExp = StaticData.GetPlayerLevelAndCurrExp(logInfo.Experience);

        _butExtract.gameObject.SetActive(false);
        if (logInfo.IsSteal)
            _butExtract.gameObject.SetActive(true);

        string timeStr = TimeHelper.ServerTime(logInfo.StealTime, "yyyy-MM-dd HH:mm:ss");
        _time.text = timeStr;
        string tips = string.Empty;
        string titleTips = string.Empty;
        if (logInfo.StealType == ManorStealType.DiglettType)
        {
            titleTips = LocalizationDefineHelper.GetStringNameById(120287);
            tips = LocalizationDefineHelper.GetStringNameById(120288);//logInfo.Account + "抓捕了地鼠！";
        }
        else 
        {
            titleTips = LocalizationDefineHelper.GetStringNameById(120285);

            for (int i = 0; i < logInfo.StealCropInfo.Count; i++)
            {
                if (i >= 4) 
                {
                    tips = tips + "...";
                    break;
                }
                //获取道具名称
                int languageId = StaticData.configExcel.GetGameItemByID(logInfo.StealCropInfo[i].GoodId).ItemName;
                string itemName = LocalizationDefineHelper.GetStringNameById(languageId);
                if (string.IsNullOrEmpty(tips))
                {
                    tips = itemName + "*" + logInfo.StealCropInfo[i].GoodNum;
                }
                else 
                {
                    tips = tips + "," + itemName + "*" + logInfo.StealCropInfo[i].GoodNum;
                }
                
            }
        }

        _contentTitle.text = titleTips;
        _content.text = tips;
    }


    private void OnClickEnterFriendManor() 
    {
        //设置偷取好友的个人信息
        StaticData.curFriendStealInfo = curFriendStealInfo;
        _uiManorLog.CloseAndEnterFriendManor(_friendID);
    }
}
