using Game.Protocal;
using Google.Protobuf.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoolItemFriendApplyInfo : MonoBehaviour, InterfaceScrollCell
{
    public UIFriendComponent uiFriendComponent;

    private SCFriendInfo scOtherStruct;
    private int itemIndex;

    private Transform _btnAgree;
    private Transform _btnRefuse;
    private Transform _levelNum;
    private Transform _online;
    private Transform _onlineBg;
    private Transform _outline;
    private Transform _outlineBg;
    private Transform _nickname;
    Image _iconImage;
    void Start()
    {
        Initial();
    }
    private void Initial()
    {
        if (_btnAgree == null)
        {
            _btnAgree = transform.Find("BtnAgree");
            _btnRefuse = transform.Find("BtnRefuse");
            _levelNum = transform.Find("IconBox/IconBox/Grade/Text");
            _online = transform.Find("Status/Online");
            _onlineBg = transform.Find("Background/OnlineBg");
            _outline = transform.Find("Status/Outline");
            _outlineBg = transform.Find("Background/OutlineBg");
            _nickname = transform.Find("Nickname");
            _iconImage = transform.Find("IconBox/IconBox/IconBg/Icon").GetComponent<Image>();
            RegisterEventListener();
        }
        SetMultilingual();
    }
    private void RegisterEventListener()
    {
        _btnAgree.GetComponent<Button>().onClick.RemoveAllListeners();
        _btnAgree.GetComponent<Button>().onClick.AddListener(OnAgreeFriend);
        _btnRefuse.GetComponent<Button>().onClick.RemoveAllListeners();
        _btnRefuse.GetComponent<Button>().onClick.AddListener(OnRefuseFriend);
    }
    private void OnRefuseFriend()
    {
        //正式请求
        CSRepulse cSRepulse = new CSRepulse()
        {
            OperationUid = scOtherStruct.Uid
        };
        ProtocalManager.Instance().SendCSRepulse(cSRepulse, (serverRes) =>
        {
            StaticData.playerInfoData.listApplyInfo.RemoveAt(itemIndex);
            Destroy(gameObject);
        }, (error) => { });
    }
    private void OnAgreeFriend()
    {
        // 正式请求
        CSAccept csAccept = new CSAccept()
        {
            OperationUid = scOtherStruct.Uid
        };
        ProtocalManager.Instance().SendCSAccept(csAccept, (serverRes) =>
        {
            StaticData.playerInfoData.listApplyInfo.RemoveAt(itemIndex);
            StaticData.playerInfoData.listFriendInfo.Add(new SCFriendInfo()
            {
                Uid = scOtherStruct.Uid,
                FriendName = scOtherStruct.FriendName,
                FriendExperience = scOtherStruct.FriendExperience,
                FriendImage = scOtherStruct.FriendImage,
                Online = scOtherStruct.Online
            });
            uiFriendComponent.GenerateApplyListUI();
            //Destroy(gameObject);
        }, (error) => { });
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
        scOtherStruct = StaticData.playerInfoData.listApplyInfo[idx];
        //根据经验计算等级
        int level = StaticData.GetPlayerLevelByExp(scOtherStruct.FriendExperience);
        _levelNum.GetComponent<Text>().text = level.ToString();
        if (string.IsNullOrEmpty(scOtherStruct.ImageAddress))
        {
            _iconImage.sprite = ChatTool.GetIcon(scOtherStruct.FriendImage);
        }
        else
        {
            _iconImage.sprite = await ChatTool.GetIcon(scOtherStruct.ImageAddress);
        }

        //多语言，todo
        if (scOtherStruct.Online == true)
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
        _nickname.GetComponent<Text>().text = scOtherStruct.FriendName;
        //_nickname.GetComponent<Text>().text = scOtherStruct.Uid.ToString();
    }

    /// <summary>
    /// 设置多语言
    /// </summary>
    public void SetMultilingual()
    {
        _online.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120110);
        _outline.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120111);
        _btnAgree.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120112);
        _btnRefuse.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120113);
    }
}
