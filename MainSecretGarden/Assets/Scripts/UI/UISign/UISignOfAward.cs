using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Company.Cfg;
using UnityEngine.UI;
using Game.Protocal;

public class UISignOfAward : MonoBehaviour, InterfaceScrollCell
{
    public UISignComponent uISignComponent;

    //组件
    private Transform _icon;
    private Transform _iconName;
    private Transform _Number;
    private Transform _NumberOfDays;
    private Transform _NotGet;
    private Transform _IsGet;

    //数据
    private string iconName;

    private int AccuAwardDay = 0;
    private int AccuAwardId = 0;
    private int AccuAwardCount = 0;

    //配置
    private SignInDefine signInfo;

    //判断变量
    private bool isCanClick = true;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _icon = transform.Find("Icon");
        _iconName = transform.Find("IconName/Text");
        _Number = transform.Find("Number");
        _NumberOfDays = transform.Find("NumberOfDays/Text");
        _NotGet = transform.Find("NotGet");
        _IsGet = transform.Find("IsGet");

        transform.GetComponent<Button>().onClick.RemoveAllListeners();
        transform.GetComponent<Button>().onClick.AddListener(OnButtonAccuClick);
    }

    public async void ScrollCellIndex(int idx)
    {
        Init();

        if (uISignComponent == null)
        {
            uISignComponent = UIComponent.GetComponentHaveExist<UISignComponent>(UIType.UISign);
        }

        signInfo = uISignComponent.listSign[idx];

        var itemConfig = StaticData.configExcel.GetGameItemByID(signInfo.PhaseAwardId);

        AccuAwardDay = signInfo.PhaseID;
        AccuAwardId = signInfo.PhaseAwardId;
        AccuAwardCount = signInfo.PhaseAwardNum;

        //判断累积奖励是否领取
        JudgeAccuAward();

        //道具名字
        iconName = itemConfig.Icon;
        _iconName.GetComponent<Text>().text = StaticData.GetMultilingual(itemConfig.ItemName);

        //道具图片
        _icon.GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(iconName);

        //道具数量
        _Number.GetComponent<Text>().text = signInfo.PhaseAwardNum.ToString();

        //获得天数
        _NumberOfDays.GetComponent<Text>().text = string.Format(LocalizationDefineHelper.GetStringNameById(120151), signInfo.PhaseID.ToString());
    }

    //点击领取累积签到奖励
    private void OnButtonAccuClick()
    {
        if (isCanClick == false)
        {
            Debug.Log("不能点击");
            return;
        }

        CSAccumulateSignIn csAccumulateSignIn = new CSAccumulateSignIn()
        {
            AccumulateDay = AccuAwardDay
        };
        ProtocalManager.Instance().SendCSAccumulateSignIn(csAccumulateSignIn, (scEmtpyAccumulateSignIn) =>
        {
            StaticData.CreateToastTips("领取奖励成功");
            //奖励入库
            StaticData.UpdateWareHouseItem(AccuAwardId, AccuAwardCount);
            //更改累计签到奖励记录
            StaticData.playerInfoData.userInfo.SignInInfo.Add(new SCSignInStruct()
            {
                //DayNumber = StaticData.playerInfoData.userInfo.SignDays,
                DayNumber = signInfo.PhaseID,
                SignInTime = TimeHelper.ServerTimeStampNow
            });
            //更新累计签到UI
            JudgeAccuAward();
        }, (error) => { });
    }

    //判断累积奖励是否领取
    public void JudgeAccuAward()
    {
        if (signInfo.PhaseID <= StaticData.playerInfoData.userInfo.SignDays)//奖励天数小于签到天数
        {
            for (int i = 0; i < StaticData.playerInfoData.userInfo.SignInInfo.Count; i++)
            {
                var data = StaticData.playerInfoData.userInfo.SignInInfo[i];
                if (data.DayNumber == signInfo.PhaseID)//如果数组里面有相同字段 就说明之前领取过了
                {
                    _NotGet.gameObject.SetActive(false);
                    _IsGet.gameObject.SetActive(true);
                    isCanClick = false;
                    return;
                }
            }
            _NotGet.gameObject.SetActive(true);
            _IsGet.gameObject.SetActive(false);
            isCanClick = true;
        }
        else
        {
            _NotGet.gameObject.SetActive(false);
            _IsGet.gameObject.SetActive(false);
            isCanClick = false;
        }
    }
}
