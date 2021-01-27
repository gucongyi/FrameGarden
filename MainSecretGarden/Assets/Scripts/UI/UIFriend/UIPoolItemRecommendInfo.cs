using Game.Protocal;
using Google.Protobuf.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoolItemRecommendInfo : MonoBehaviour, InterfaceScrollCell
{
    public UIFriendComponent uiFriendComponent;
    private RepeatedField<SCFriendInfo> listRecommendInfo = new RepeatedField<SCFriendInfo>();
    private SCFriendInfo scOtherStruct;
    private SCSearch scSearch;

    private Transform _btnApply;
    private Transform _levelNum;
    private Transform _nickname;
    private Transform _online;
    private Transform _onlineBg;
    private Transform _outline;
    private Transform _outlineBg;
    private Transform _statusTip;
    private Transform _statusTipText;
    Image _iconImage;
    private enum Friendship
    {
        Normal = 0,
        IsApply = 1,
        IsFriend = 2
    }
    void Awake()
    {
        Initial();
    }
    void Start()
    {
        RegisterEventListener();

    }
    private void Initial()
    {
        _btnApply = transform.Find("BtnApply");
        _levelNum = transform.Find("IconBox/IconBox/Grade/Text");
        _nickname = transform.Find("Nickname");
        _online = transform.Find("Status/Online");
        _onlineBg = transform.Find("Background/OnlineBg");
        _outline = transform.Find("Status/Outline");
        _outlineBg = transform.Find("Background/OutlineBg");
        _statusTip = transform.Find("StatusTip");
        _statusTipText = transform.Find("StatusTip/Text");
        _iconImage = transform.Find("IconBox/IconBox/IconBg/Icon").GetComponent<Image>();
        SetMultilingual();
    }
    private void RegisterEventListener()
    {
        _btnApply.GetComponent<Button>().onClick.RemoveAllListeners();
        _btnApply.GetComponent<Button>().onClick.AddListener(OnApplyFriend);
    }
    private void OnApplyFriend()
    {
        // 正式请求
        StaticData.DebugGreen($"申请添加好友~~~:{scOtherStruct.Uid}");
        CSApply csApply = new CSApply()
        {
            OperationUid = scOtherStruct.Uid
        };
        ProtocalManager.Instance().SendCSApply(csApply, (serverRes) =>
        {
            SetFriendship(Friendship.IsApply);
            string tips = LocalizationDefineHelper.GetStringNameById(120147);
            StaticData.CreateToastTips(tips);
        }, (error) => { });
    }
    /// <summary>
    /// 生成好友列表中LoopItem后的回调函数
    /// 改变每个LoopItem的参数
    /// </summary>
    /// <param name="idx"></param>
    public async void ScrollCellIndex(int idx)
    {
        SetFriendship(Friendship.Normal);
        if (uiFriendComponent.isRecommendUI)
        {
            listRecommendInfo = StaticData.playerInfoData.listRecommendInfo;

            scOtherStruct = listRecommendInfo[idx];
            if (string.IsNullOrEmpty(scOtherStruct.ImageAddress))
            {
                _iconImage.sprite = ChatTool.GetIcon(scOtherStruct.FriendImage);
            }
            else
            {
                _iconImage.sprite = await ChatTool.GetIcon(scOtherStruct.ImageAddress);
            }

            //根据经验计算等级
            int level = StaticData.GetPlayerLevelByExp(scOtherStruct.FriendExperience);
            _levelNum.GetComponent<Text>().text = level.ToString();
            //昵称
            _nickname.GetComponent<Text>().text = scOtherStruct.FriendName;
            //_nickname.GetComponent<Text>().text = scOtherStruct.Uid.ToString();
        }
        else
        {
            scSearch = uiFriendComponent.scSearchInfo;
            if (scSearch != null)
            {
                //根据经验计算等级
                int level = StaticData.GetPlayerLevelByExp(scSearch.Search[idx].FriendExperience);
                _levelNum.GetComponent<Text>().text = level.ToString();

                //昵称
                _nickname.GetComponent<Text>().text = scSearch.Search[idx].FriendName;
                if (string.IsNullOrEmpty(scSearch.Search[idx].ImageAddress))
                {
                    _iconImage.sprite = ChatTool.GetIcon(scSearch.Search[idx].FriendImage);
                }
                else
                {
                    _iconImage.sprite = await ChatTool.GetIcon(scSearch.Search[idx].ImageAddress);
                }

                //_nickname.GetComponent<Text>().text = scSearch.Search[idx].Uid.ToString();
                if (scSearch.Search[idx].IsApply)
                {
                    SetFriendship(Friendship.IsApply);
                }
                else
                {
                    scOtherStruct = new SCFriendInfo()
                    {
                        Uid = scSearch.Search[idx].Uid,
                        FriendName = scSearch.Search[idx].FriendName,
                        FriendExperience = scSearch.Search[idx].FriendExperience,
                        FriendImage = scSearch.Search[idx].FriendImage,
                        Online = scSearch.Search[idx].Online
                    };
                }
                //判定是否在好友列表中
                RepeatedField<SCFriendInfo> listFriendInfo = StaticData.playerInfoData.listFriendInfo;
                foreach (SCFriendInfo friendInfo in listFriendInfo)
                {
                    if (friendInfo.Uid == scSearch.Search[idx].Uid)
                    {
                        SetFriendship(Friendship.IsFriend);
                    }
                }
            }
            else
            {
                //需添加空背景，todo
                StaticData.DebugGreen("搜索玩家结果为空~~~");
            }
        }
        if ((scOtherStruct != null && scOtherStruct.Online) || (scSearch != null && scSearch.Search[idx].Online))
        {
            _online.gameObject.SetActive(true);
            _onlineBg.gameObject.SetActive(true);
            _outline.gameObject.SetActive(false);
            _outlineBg.gameObject.SetActive(false);
        }
        else
        {
            _outline.gameObject.SetActive(true);
            _outlineBg.gameObject.SetActive(true);
            _online.gameObject.SetActive(false);
            _onlineBg.gameObject.SetActive(false);
        }
    }
    private void SetFriendship(Friendship friendship)
    {
        //多语言，todo
        switch (friendship)
        {
            case Friendship.Normal:
                _btnApply.gameObject.SetActive(true);
                _statusTip.gameObject.SetActive(false);
                break;
            case Friendship.IsApply:
                _btnApply.gameObject.SetActive(false);
                _statusTip.gameObject.SetActive(true);
                _statusTipText.GetComponent<Text>().text = StaticData.GetMultilingual(120251);
                break;
            case Friendship.IsFriend:
                _btnApply.gameObject.SetActive(false);
                _statusTip.gameObject.SetActive(true);
                _statusTipText.GetComponent<Text>().text = StaticData.GetMultilingual(120252);
                break;
        }
    }

    /// <summary>
    /// 设置多语言
    /// </summary>
    public void SetMultilingual()
    {
        _online.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120110);
        _outline.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120111);
        _btnApply.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120114);
    }
}
