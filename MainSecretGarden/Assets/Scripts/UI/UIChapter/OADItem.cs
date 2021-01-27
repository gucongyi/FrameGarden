using Company.Cfg;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum OADItemType
{
    AD,
    Money,
    Unlock
}
public class OADItem : MonoBehaviour, InterfaceScrollCell
{
    [SerializeField]
    OADItemType type;
    int ADcount;//只有是广告类型时才生效
    Image ItemPicture_image;//itemPic
    Image New_image;//上新标记
    Text OADName_Text;
    RectTransform start;
    Button start_Btn;//解锁后显示的开始按钮
    Text startWatch_text;//观看文本
    RectTransform unlock;
    Button unlock_Btn;//显示解锁条件的按钮
    Image unlockIcon;//解锁Icon(紫星币/广告)
    Text watchCount;//看视频次数或者需要的紫星币数量
    [SerializeField]
    /// <summary>
    /// 当前展示物品数据
    /// </summary>
    ExtraStoryDefine _currShowData;
    [SerializeField]
    /// <summary>
    /// 当前数据下标
    /// </summary>
    int _currDataIndex;
    /// <summary>
    /// 是否初始化
    /// </summary>
    bool _isInitial = false;

    /// <summary>
    /// 上新逻辑（存本地）
    /// </summary>
    int UpNow = 1;
    /// <summary>
    /// 未解锁红点
    /// </summary>
    Image NoUnlockRedDot;
    /// <summary>
    /// 已解锁红点
    /// </summary>
    Image UnlockRedDot;

    private void Awake()
    {
        if (!_isInitial)
        {
            Initial();
        }
    }

    private void Initial()
    {
        ItemPicture_image = transform.Find("Box/Picture").GetComponent<Image>();
        New_image = transform.Find("New").GetComponent<Image>();
        OADName_Text = transform.Find("OADName_text").GetComponent<Text>();
        start = transform.Find("startBtn") as RectTransform;
        start_Btn = transform.Find("startBtn/Btn").GetComponent<Button>();
        startWatch_text = transform.Find("startBtn/startWatch_text").GetComponent<Text>();
        unlock = transform.Find("unlockBtn") as RectTransform;
        unlock_Btn = transform.Find("unlockBtn/Btn").transform.GetComponent<Button>();
        unlockIcon = transform.Find("unlockBtn/Icon").transform.GetComponent<Image>();
        watchCount = transform.Find("unlockBtn/WatchCount").transform.GetComponent<Text>();

        unlock_Btn.onClick.RemoveAllListeners();
        unlock_Btn.onClick.AddListener(OnClickUnlockBtn);
        start_Btn.onClick.RemoveAllListeners();
        start_Btn.onClick.AddListener(OnClickWatchBtn);

        NoUnlockRedDot = unlock.transform.Find("redDot").GetComponent<Image>();
        UnlockRedDot = start.transform.Find("redDot").GetComponent<Image>();

        _isInitial = true;
    }

    public void ScrollCellIndex(int index)
    {
        _currShowData = UIChapterComponent.Instance.ItemDataShow(index);
        if (_currShowData != null)
        {
            _currDataIndex = index;
            ShwoData();
        }
    }

    //信息更新
    void ShwoData()
    {
        if (_currShowData == null)
        {
            return;
        }
        bool isShowRedDot;
        //红点逻辑
        if (ChapterHelper.localOADList.Contains(_currShowData.ExtraStoryId))
        {
            isShowRedDot = false;
        }
        else
        {
            isShowRedDot = true;
        }

        //上新逻辑
        UpNow = PlayerPrefs.GetInt($"OAD+{_currShowData.ExtraStoryId}");
        if (UpNow == 2)
        {//上新逻辑关闭
            New_image.gameObject.SetActive(false);
        }
        else
        {
            New_image.gameObject.SetActive(true);
        }


        try
        {
            ItemPicture_image.sprite = ABManager.GetAsset<Sprite>(_currShowData.picture);
        }
        catch
        {
            Debug.LogError("番外icon没找到");
        }

        bool isUnlock = UIChapterComponent.Instance.OADID.Contains(_currShowData.ExtraStoryId.ToString()) ? true : false;//在列表里则解锁
        if (!isUnlock)
        {
            if (_currShowData.Price.Count != 0)//紫星币
                type = OADItemType.Money;
            else//AD
            {
                type = OADItemType.AD;
                if (UIChapterComponent.Instance.ADList != null)//没看过广告的用户 请求回返空
                    foreach (var item in UIChapterComponent.Instance.ADList)
                    {
                        if (item.GoodsId != _currShowData.ExtraStoryId) continue;
                        ADcount = item.AdvNum;
                    }
            }
        }
        else
            type = OADItemType.Unlock;

        string OADName = ChapterHelper.GetOADDialogueString(ChapterHelper.GetOADData(_currShowData.ExtraStoryName));
        this.OADName_Text.text = OADName;

        if (type == OADItemType.AD && this.ADcount >= _currShowData.AdvertisingNum)
        {
            type = OADItemType.Unlock;
        }

        switch (type)
        {
            case OADItemType.AD:
                ChapterHelper.SetActive(unlock.gameObject, true);
                ChapterHelper.SetActive(start.gameObject, false);

                //unlockIcon.sprite = ABManager.GetAsset<Sprite>("");//广告Icon图片
                watchCount.text = $"{this.ADcount}/{_currShowData.AdvertisingNum}";

                NoUnlockRedDot.gameObject.SetActive(isShowRedDot);
                break;
            case OADItemType.Money:
                ChapterHelper.SetActive(unlock.gameObject, true);
                ChapterHelper.SetActive(start.gameObject, false);

                unlockIcon.sprite = ABManager.GetAsset<Sprite>("dj_zjb");//紫星币Icon图片
                watchCount.text = _currShowData.Price[0].Count.ToString();
                //颜色变化（不足为红够了变白）

                NoUnlockRedDot.gameObject.SetActive(isShowRedDot);
                break;
            case OADItemType.Unlock:
                ChapterHelper.SetActive(start.gameObject, true);
                ChapterHelper.SetActive(unlock.gameObject, false);

                UnlockRedDot.gameObject.SetActive(isShowRedDot);

                startWatch_text.text = StaticData.GetMultilingual(120292);//国际化获取
                break;
        }
    }

    /// <summary>
    /// 点击解锁按钮
    /// </summary>
    void OnClickUnlockBtn()
    {
        switch (type)
        {
            case OADItemType.AD:
                UnlockViewAD(this._currShowData.ExtraStoryId);
                break;
            case OADItemType.Money:
                UIChapterComponent.Instance.OpenBuyOAD(this._currShowData, this._currDataIndex, this, ShwoData);
                break;
        }
        ChapterHelper.ReduceRedDot(this._currShowData.ExtraStoryId, false);
        UIChapterComponent.Instance.ShowOADBtnRedDot();
        NoUnlockRedDot.gameObject.SetActive(false);
    }
    /// <summary>
    /// 点击观看按钮
    /// </summary>
    void OnClickWatchBtn()
    {
        UIChapterComponent.Instance.OpenOAD(this._currShowData);
        PlayerPrefs.SetInt($"OAD+{_currShowData.ExtraStoryId}", 2);
        ShwoData();
        UnlockRedDot.gameObject.SetActive(false);
    }

    //AD弹出
    void UnlockViewAD(int OADid)
    {
        StaticData.OpenAd("OADAd", (code, msg) =>
        {
            if (code == 1)
            {
                //成功请求
                CSAdvExtraStory sCEmtpyAdvExtraStory = new CSAdvExtraStory() { ExtraStoryId = OADid };
                ProtocalManager.Instance().SendCSAdvExtraStory(sCEmtpyAdvExtraStory, (x) =>
                {
                    Debug.Log($"观看广告+{this.ADcount}次");
                    this.ADcount += 1;
                    if (this.ADcount >= _currShowData.AdvertisingNum)
                    {//观看广告次数达到
                        ChapterHelper.UnlockOAD(_currShowData.ExtraStoryId);
                    }
                    bool isAddCount = false;
                    foreach (var item in StaticData.playerInfoData.userInfo.AdvInfo)
                    {
                        if (item.GoodsId != OADid) continue;
                        isAddCount = true;
                        item.AdvNum += 1;
                    }
                    if (!isAddCount)
                    {
                        CSAdvStruct cSAdv = new CSAdvStruct()
                        {
                            GoodsId = OADid,
                            AdvNum = this.ADcount
                        };
                        StaticData.playerInfoData.userInfo.AdvInfo.Add(cSAdv);
                    };
                    ShwoData();

                }, (ErrorInfo e) =>
                {
                    Debug.LogError("广告观看请求失败");//TODO？
                });
            }
            else
            {
                //
            }
        });
    }

    /// <summary>
    /// 上新逻辑
    /// </summary>
    private void ShowUpNew()
    {
        //PlayerPrefs.SetInt("", 2);


        //var nowTime = TimeHelper.ServerTimeStampNow;
        ////判定上新状态
        //if (nowTime > _currShowData.UpNewTimeBegin && nowTime < _currShowData.UpNewTimeEnd)
        //{
        //    New_image.gameObject.SetActive(true);
        //}
        //else
        //{
        //    New_image.gameObject.SetActive(false);
        //}
    }
}
