using DG.Tweening;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 心动时刻NPC界面
/// </summary>
public class ImpulesNPCView : MonoBehaviour
{
    ImpulesMainView impulesMainView;

    float moveAnimaTime = 0.3f;
    float ShowValue = 0f;
    float closeValue = 1242f;

    #region NpcView
    RawImage setPainting;//立绘
    GiftAddValue npc2Player;//送礼属性值
    GiftAddValue player2Npc;//送礼属性值

    Button back_btn;//npc界面返回按钮     //退出界面时请求存储，失败则刷新当前界面

    Text npcFavorableValue;//npc好感数值

    Text playerFavorableValue;//玩家好感数值

    Text cardiacValue;//心动值
    Button intimateDating;//亲密约会
    Text DateText;
    public LoopHorizontalScrollRect giftList;
    #endregion

    public UIImpulseComponent uIImpulseComponent;

    Favorable LastFavorableState = new Favorable();//记录一开始好感值(用于返回失败重新刷新界面)
    Favorable NewFavorableState = new Favorable();//赠送礼物后改变的好感值

    public ImpulseGiftGoods curGiftGoods;//当前选中的道具
    ImpulesGiftItem curGiftInstance;//选中道具的实例
    public UISetAmountComponent setAmount;

    public BuyGiftAmountChoice buyView;//购买界面

    void FindComponent()
    {
        //npcView
        back_btn = transform.Find("Top/Back_btn").GetComponent<Button>();

        setPainting = transform.Find("NPCLive2D").GetComponent<RawImage>();
        npc2Player = transform.Find("Top/npcInfo/AddValue").GetComponent<GiftAddValue>();
        player2Npc = transform.Find("Top/playerInfo/AddValue").GetComponent<GiftAddValue>();

        npcFavorableValue = transform.Find("Top/npcInfo/Num").GetComponent<Text>();
        playerFavorableValue = transform.Find("Top/playerInfo/Num").GetComponent<Text>();
        //心动值
        cardiacValue = transform.Find("Top/CardiacValue/box/Text").GetComponent<Text>();
        //约会按钮
        intimateDating = transform.Find("Top/DateBtn").GetComponent<Button>();
        DateText = intimateDating.transform.Find("Text").GetComponent<Text>();

        giftList = transform.Find("Bottom/LoopGiftList").GetComponent<LoopHorizontalScrollRect>();
        setAmount.act = GiveGifts;
    }

    public void Init(ImpulesMainView impulesMainView)
    {
        this.impulesMainView = impulesMainView;
        FindComponent();
        RegisteredEvents();
        //loopScrollSizeAdjust();
    }

    void RegisteredEvents()
    {
        back_btn.onClick.AddListener(GivingGiftAndBackMainView);
    }

    //显示界面 同时动画
    public void OpenView(Favorable favorable)
    {
        this.LastFavorableState.FieldAssignment(favorable);
        this.NewFavorableState.FieldAssignment(favorable);

        giftInfo.NpcID = favorable.NPCId;//记录礼物赠送的对象id
        //移动动画
        this.transform.DOLocalMoveX(ShowValue, moveAnimaTime);
        //刷新数值info
        GetNpcAndPlayerInfo(favorable);
        //选中
        PitchOnBtn();
        //刷新礼物列表
        ShowGiftList();
        //ShowPitchOn(uIImpulseComponent.ImpulseGiftGoods[0], giftList.content.GetChild(0).GetComponent<ImpulesGiftItem>());
        ShowPitchOn(uIImpulseComponent.ImpulseGiftGoods[0], giftList.content.GetChild(0).GetComponent<ImpulesGiftItem>());
    }

    /// <summary>
    /// 获取玩家对该NPC的信息，刷新界面数值
    /// </summary>
    /// <param name="favorable"></param>
    void GetNpcAndPlayerInfo(Favorable favorable)
    {
        npcFavorableValue.text = favorable.NPC2PlayerValue.ToString();
        playerFavorableValue.text = favorable.Player2NPCValue.ToString();

        cardiacValue.text = favorable.cardiacValue.ToString();

    }

    /// <summary>
    /// 显示礼物列表（滚动列表）
    /// </summary>
    void ShowGiftList()
    {
        giftList.ClearCells();
        //giftList.totalCount = uIImpulseComponent.ImpulseGiftGoods.Count;
        giftList.totalCount = uIImpulseComponent.ImpulseGiftGoods.Count;
        giftList.RefillCells();
    }
    /// <summary>
    /// 刷新列表
    /// </summary>
    void ReshList(int amount)
    {
        //更新道具信息
        uIImpulseComponent.BuyRefreshGift(curGiftGoods.goodsID, amount);
        //更新道具实例信息
        if (curGiftInstance.itemInfo.goodsID == curGiftGoods.goodsID)
        {
            curGiftInstance.itemInfo = curGiftGoods;
            curGiftInstance.RefreshAmount();
        }
        StaticData.CreateToastTips("购买成功，快去送给他吧");//TODO
    }

    //滚动列表需求
    public ImpulseGiftGoods GetGiftItem(int index)
    {
        if (uIImpulseComponent.ImpulseGiftGoods.Count >= index)
            return uIImpulseComponent.ImpulseGiftGoods[index];
        else
            return null;
    }
    /// <summary>
    /// 礼物列表大小自适应
    /// </summary>
    private void loopScrollSizeAdjust()
    {
        if (Screen.height < 2688)
        {
            var deltaHeight = (2688 - Screen.height * 1242 / Screen.width) / 2;
            giftList.transform.GetComponent<RectTransform>().sizeDelta -= new Vector2(0, deltaHeight);
        }
    }

    /// <summary>
    /// 请求赠送礼物并返回主界面
    /// </summary>
    void GivingGiftAndBackMainView()
    {
        //请求
        if (giftInfo.giftInfo.Count > 0)//有送礼物(请求成功后清除列表)
        {
            CSSendNPCGift cSSendNPCGift = new CSSendNPCGift() { NPCId = giftInfo.NpcID };
            foreach (var item in giftInfo.giftInfo)
            {
                cSSendNPCGift.SendGiftInfo.Add(item);
            }
            //最后一个参数false 则请求失败后自己处理
            ProtocalManager.Instance().SendCSSendNPCGift(cSSendNPCGift, (SCEmptySendNPCGift FavorableInfo) =>
            {//成功就成功返回主界面
                this.LastFavorableState.FieldAssignment(this.NewFavorableState);

                foreach (var item in StaticData.playerInfoData.favorableData)
                {
                    if (item.NPCId != this.NewFavorableState.NPCId) continue;
                    item.FieldAssignment(this.NewFavorableState);
                }
                //前端自己处理前端仓库数据
                foreach (var item in giftInfo.giftInfo)
                {
                    var goods = StaticData.GetWareHouseItem(item.GoodId);
                    if (goods != null)
                        goods.GoodNum -= item.GoodNum;
                }
                giftInfo.giftInfo.Clear();//清除数据
                //删除属性计时器
                if (npc2Player.timeCountDown != null)
                {
                    GameObject.Destroy(npc2Player.timeCountDown.gameObject);
                    npc2Player.timeCountDown = null;
                }
                if (player2Npc.timeCountDown != null)
                {
                    GameObject.Destroy(player2Npc.timeCountDown.gameObject);
                    player2Npc.timeCountDown = null;
                }
                this.transform.DOLocalMoveX(closeValue, moveAnimaTime);
                impulesMainView.ShowView();
            }, (ErrorInfo e) =>
            {//失败再刷新页面
                uIImpulseComponent.RefreshLastGift();//失败后刷新仓库礼物道具
                ShowGiftList();//刷新列表
                GetNpcAndPlayerInfo(this.LastFavorableState);//回到初入界面的好感值
                this.NewFavorableState.FieldAssignment(this.LastFavorableState);//前端缓存的好感值同时回到最初
                giftInfo.giftInfo.Clear();//清除数据
                Debug.LogError("服务器赠送礼物错误");
            }, false);
        }
        else
        {
            this.transform.DOLocalMoveX(closeValue, moveAnimaTime);
            impulesMainView.ShowView();
        }
    }

    GiveGiftInfo giftInfo = new GiveGiftInfo();//玩家对某个NPC增送的礼物信息
    /// <summary>
    /// 赠送礼物
    /// </summary>
    public void GiveGift(int goodsID, int goodsAmount, int NpcFavorable, int PlayerFavorable)
    {
        bool isGive = false;
        foreach (var item in giftInfo.giftInfo)//赠送礼物列表里有该道具是只增数量
        {
            if (item.GoodId == goodsID)
                isGive = true;
            if (isGive)
            {
                item.GoodNum += goodsAmount;
                break;
            }
        }
        if (!isGive)//礼物列表里没有时添加到列表里
        {
            CSGoodStruct gift = new CSGoodStruct();
            gift.GoodId = goodsID;
            gift.GoodNum = goodsAmount;
            giftInfo.giftInfo.Add(gift);
        }
        //好感度增加动画TODO
        AddFavorable(goodsAmount, NpcFavorable, PlayerFavorable);
    }

    //前端界面好感值增加（前端缓存表现）
    void AddFavorable(int amount, int NpcFavorable, int PlayerFavorable)
    {
        NewFavorableState.NPC2PlayerValue += NpcFavorable * amount;
        NewFavorableState.Player2NPCValue += PlayerFavorable * amount;
        NewFavorableState.cardiacValue = NewFavorableState.NPC2PlayerValue < NewFavorableState.Player2NPCValue ? NewFavorableState.NPC2PlayerValue : NewFavorableState.Player2NPCValue;

        RefreshNpcAndPlayerInfoAnima(NewFavorableState);//界面动画
    }

    /// <summary>
    /// 刷新界面好感动画
    /// </summary>
    /// <param name="favorable"></param>
    void RefreshNpcAndPlayerInfoAnima(Favorable favorable)//TODO
    {
        //int npcLevel = CalculateFavorableLevel(favorable.NPC2PlayerValue);
        //npcFavorableLevel.text = $"Lv.{npcLevel.ToString()}";//npc好感等级
        //除去等级剩余经验
        //float curNPCExp = GetCurLevelEXP(favorable.NPC2PlayerValue, npcLevel);
        //npcFavorableValue.text = $"{curNPCExp}/{GetTotalExperience(npcLevel)}";//npc好感数值
        //npcExpBar.fillAmount = curNPCExp / GetTotalExperience(npcLevel);//npc经验条

        //int playerLevel = CalculateFavorableLevel(favorable.Player2NPCValue);
        //playerFavorableLevel.text = $"Lv.{playerLevel.ToString()}";//玩家好感等级
        //float curPlayerExp = GetCurLevelEXP(favorable.Player2NPCValue, playerLevel);//除去等级剩余经验
        //playerFavorableValue.text = $"{curPlayerExp}/{GetTotalExperience(playerLevel)}";//玩家好感数值
        //playerExpBar.fillAmount = curPlayerExp / GetTotalExperience(playerLevel);//玩家经验条
        npcFavorableValue.text = favorable.NPC2PlayerValue.ToString();
        playerFavorableValue.text = favorable.Player2NPCValue.ToString();
        cardiacValue.text = favorable.cardiacValue.ToString();
    }

    /// <summary>
    /// 显示送礼对话
    /// </summary>
    public void ShowGiftDialogue(int amount, int npc2PlayerAddValue, int player2NpcAddValue)
    {
        npc2Player.ShowGiftDialogue(npc2PlayerAddValue * amount);
        player2Npc.ShowGiftDialogue(player2NpcAddValue * amount);
    }
    /// <summary>
    /// 更新心动值
    /// </summary>
    public void UpdateCardiacValue()
    {
        foreach (var item in StaticData.playerInfoData.favorableData)
        {
            if (item.NPCId != NewFavorableState.NPCId)
                continue;
            cardiacValue.text = item.cardiacValue.ToString();
        }
    }
    //按钮选中
    public void PitchOnBtn()
    {//每次进入时默认选中第一个
        foreach (var item in uIImpulseComponent.ImpulseGiftGoods)
        {
            item.isPitchOn = false;
        }
        uIImpulseComponent.ImpulseGiftGoods[0].isPitchOn = true;
    }
    /// <summary>
    /// 显示选中的效果
    /// </summary>
    public void ShowPitchOn(ImpulseGiftGoods item, ImpulesGiftItem giftItem)
    {
        //setAmount.inputFiled.text = "1";//不再把数量重置为1
        curGiftGoods = item;
        curGiftInstance = giftItem;
    }
    //赠送按钮
    void GiveGifts(int number)
    {
        if (curGiftGoods.goodsAmount >= number)
        {
            this.GiveGift(curGiftGoods.goodsID, number, curGiftGoods.NpcAddValue, curGiftGoods.playerAddValue);
            curGiftGoods.goodsAmount -= number;
            ////查找索引
            //int index = uIImpulseComponent.ImpulseGiftGoods.IndexOf(curGiftGoods);
            //giftList.RefillCells(index);
            if (curGiftInstance.itemInfo.goodsID == curGiftGoods.goodsID)
            {
                curGiftInstance.itemInfo = curGiftGoods;
                curGiftInstance.RefreshAmount();
            }
            //显示对话
            ShowGiftDialogue(number, curGiftGoods.NpcAddValue, curGiftGoods.playerAddValue);
        }
        else if (curGiftGoods.goodsID != StaticData.configExcel.GetVertical().LoginGetFavorableValue.ID)
        {
            buyView.ShowBuyView(curGiftGoods, Mathf.Abs(curGiftGoods.goodsAmount - number),
    ReshList);//打开购买界面 
        }
        else
        {
            StaticData.CreateToastTips("该物品请通过每日领取获得");//TODO
        }

    }
}

/// <summary>
/// 礼物信息结构
/// </summary>
public class GiveGiftInfo
{
    public int NpcID;
    public List<CSGoodStruct> giftInfo = new List<CSGoodStruct>();
}