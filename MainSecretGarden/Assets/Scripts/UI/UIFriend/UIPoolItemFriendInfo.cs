using Game.Protocal;
using Google.Protobuf.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoolItemFriendInfo : MonoBehaviour, InterfaceScrollCell
{
    public UIFriendComponent uiFriendComponent;
    [HideInInspector]
    public SCFriendInfo scFriendInfo;
    public int itemIndex;

    private Transform _btnDelete;
    private Transform _btnChat;
    private Transform _tipDeleteFriend;
    private Transform _tipDeleteFriendText;
    private Transform _levelNum;
    private Transform _online;
    private Transform _onlineBg;
    private Transform _outline;
    private Transform _outlineBg;
    private Transform _nickname;
    private Transform _visitManor;
    /// <summary>
    /// 偷取按钮
    /// </summary>
    Button _stealBtn;
    Image _iconImage;

    void Start()
    {
        Initial();
    }
    private void Initial()
    {
        if (_btnDelete == null)
        {
            _btnDelete = transform.Find("BtnDelete");
            _btnChat = transform.Find("BtnChat");
            _tipDeleteFriend = uiFriendComponent.transform.Find("TipDeleteFriend");
            _tipDeleteFriendText = _tipDeleteFriend.transform.Find("TipText");
            _levelNum = transform.Find("IconBox/IconBox/Grade/Text");
            _online = transform.Find("Status/Online");
            _onlineBg = transform.Find("Background/OnlineBg");
            _outline = transform.Find("Status/Outline");
            _outlineBg = transform.Find("Background/OutlineBg");
            _nickname = transform.Find("Nickname");
            _visitManor = transform.Find("VisitManor");
            _stealBtn = transform.Find("Steal").GetComponent<Button>();
            _iconImage = transform.Find("IconBox/IconBox/IconBg/Icon").GetComponent<Image>();
            RegisterEventListener();
        }
        SetMultilingual();
    }
    private void RegisterEventListener()
    {
        _btnDelete.GetComponent<Button>().onClick.RemoveAllListeners();
        _btnDelete.GetComponent<Button>().onClick.AddListener(OnDeleteFriend);
        _btnChat.GetComponent<Button>().onClick.RemoveAllListeners();
        _btnChat.GetComponent<Button>().onClick.AddListener(OnButtonChatClick);
        _visitManor.GetComponent<Button>().onClick.RemoveAllListeners();
        _visitManor.GetComponent<Button>().onClick.AddListener(OnEnterManor);
    }
    private void OnButtonChatClick()
    {
        StaticData.OpenPrivateChat(scFriendInfo.Uid, scFriendInfo.FriendImage, scFriendInfo.ImageAddress, scFriendInfo.FriendName, scFriendInfo.FriendExperience);
    }
    private void OnDeleteFriend()
    {
        //_tipDeleteFriend.gameObject.SetActive(true);
        uiFriendComponent.focusDeleteItem = gameObject;
        string tips = string.Format(LocalizationDefineHelper.GetStringNameById(120118), scFriendInfo.FriendName);
        StaticData.OpenCommonTips(tips, 120119, () => { uiFriendComponent.OnClickDeleteFriend(); }, () => { Debug.Log("点击取消"); }, 120075);
        //_tipDeleteFriendText.GetComponent<Text>().text = tips;

    }
    private async void OnEnterManor()
    {
        //功能是否开启
        if (!StaticData.IsOpenFunction(10013))
        {
            return;
        }
        UIComponent.HideUI(UIType.UIFriend);
        //StaticData.OpenFriend(true);
        UIManorComponent uiManorComponent = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
        if (uiManorComponent == null)
        {
            await UIComponent.CreateUIAsync(UIType.UIManor);
            uiManorComponent = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
        }
        StaticData.curFriendStealInfo = new FriendStealInfo()
        {
            nickname = scFriendInfo.FriendName,
            headIcon = scFriendInfo.FriendImage,
            playerLevelAndCurrExp = StaticData.GetPlayerLevelAndCurrExp(scFriendInfo.FriendExperience)
        };
        //新手引导完成
        if (StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.isCurrStepGuiding)
        {
            GuideCanvasComponent._instance.SetLittleStepFinish();
        }
        uiManorComponent.OnButtonEnterFriendManor(scFriendInfo.Uid);
    }
    /// <summary>
    /// 生成好友列表中LoopItem后的回调函数
    /// 改变每个LoopItem的参数
    /// </summary>
    /// <param name="idx"></param>
    public async void ScrollCellIndex(int idx)
    {
        Initial();
        itemIndex = idx;
        scFriendInfo = StaticData.playerInfoData.listFriendInfo[idx];
        //根据经验计算等级
        int level = StaticData.GetPlayerLevelByExp(scFriendInfo.FriendExperience);
        _levelNum.GetComponent<Text>().text = level.ToString();

        if (string.IsNullOrEmpty(scFriendInfo.ImageAddress))
        {
            _iconImage.sprite = ChatTool.GetIcon(scFriendInfo.FriendImage);
        }
        else
        {
            _iconImage.sprite = await ChatTool.GetIcon(scFriendInfo.ImageAddress);
        }

        if (scFriendInfo.Online == true)
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
        //昵称
        _stealBtn.gameObject.SetActive(IsSteal(scFriendInfo));

        _nickname.GetComponent<Text>().text = scFriendInfo.FriendName;
        if (scFriendInfo.Uid == StaticData.configExcel.Vertical[0].SpecialUid)
        {
            _btnDelete.gameObject.SetActive(false);
        }
        else
        {
            _btnDelete.gameObject.SetActive(true);
        }
        //_nickname.GetComponent<Text>().text = scFriendInfo.Uid.ToString();
    }
    /// <summary>
    /// 是否可以偷取
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool IsSteal(SCFriendInfo data)
    {
        FriendStealInfo friendStealInfo = null;
        for (int i = 0; i < StaticData.playerInfoData.listFriendStealInfo.Count; i++)
        {
            if (data.Uid == StaticData.playerInfoData.listFriendStealInfo[i].uid)
            {
                friendStealInfo = StaticData.playerInfoData.listFriendStealInfo[i];
            }
        }
        bool isSteal = false;
        if (friendStealInfo != null && friendStealInfo.isSteal)
        {
            isSteal = true;
        }
        return isSteal;
    }
    /// <summary>
    /// 设置多语言
    /// </summary>
    public void SetMultilingual()
    {
        _online.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120110);
        _outline.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120111);
        _btnDelete.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120250);
        _visitManor.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120284);
    }
}
