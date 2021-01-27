using Cysharp.Threading.Tasks;
using Game.Protocal;
using Google.Protobuf.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 传入到NPC详细界面（好感数据）
/// </summary>
public class Favorable
{
    /// <summary>
    /// NPCid(读表)
    /// </summary>
    public int NPCId;
    /// <summary>
    /// NPC对玩家好感
    /// </summary>
    public int NPC2PlayerValue = 0;
    /// <summary>
    /// 玩家对NPC好感
    /// </summary>
    public int Player2NPCValue = 0;
    /// <summary>
    /// 心动值(两好感值较低的那个值为心动值)
    /// </summary>
    public int cardiacValue = 0;
    /// <summary>
    /// 头像
    /// </summary>
    public string headPortrait;
    /// <summary>
    /// 人物立绘
    /// </summary>  
    public string NpcSetPainting;

    //等级//下一个升级的经验值
    public Favorable() { }

    /// <summary>
    /// 属性赋值
    /// </summary>
    /// <param name="favorable"></param>
    public void FieldAssignment(Favorable favorable)
    {
        this.NPCId = favorable.NPCId;
        this.NPC2PlayerValue = favorable.NPC2PlayerValue;
        this.Player2NPCValue = favorable.Player2NPCValue;
        this.cardiacValue = favorable.cardiacValue;
        this.headPortrait = favorable.headPortrait;
        this.NpcSetPainting = favorable.NpcSetPainting;
    }

}

/// <summary>
/// 好感礼物道具
/// </summary>
public class ImpulseGiftGoods
{
    public int goodsID;
    public int goodsName;//道具名称(存id到多语言表里获取)
    public string goodsIcon;
    public int goodsDescription;//道具描述(存id到多语言表里获取)
    public List<Company.Cfg.GoodIDCount> goodsPriceBuy;//礼物单价
    public int goodsAmount = 0;//道具库存数量
    public int playerAddValue = 0;//玩家增加的好感值
    public int NpcAddValue = 0;//NPC增加的好感值
    public bool isPitchOn;//是否选中
}

public class NPCDialogue
{
    public int NPCID;
    public List<int> dialogue = new List<int>();
}

/// <summary>
/// 心动时刻
/// </summary>
public class UIImpulseComponent : MonoBehaviour
{
    //玩家有多少好感值(可操作)（在道具里）
    int favorableValue;
    //好感值列表 (服务器一开始传入)自己保存在本地
    List<Favorable> favorableList = new List<Favorable>();
    public List<ImpulseGiftGoods> ImpulseGiftGoods = new List<ImpulseGiftGoods>();//使用或购买物品后的礼物列表

    ImpulesMainView impulesMainView;
    ImpulesNPCView impulesNPCView;
    //骚话
    public List<NPCDialogue> npcId = new List<NPCDialogue>();

    void FindComponent()
    {
        //StaticData.playerInfoData.userInfo.
        impulesMainView = transform.Find("MainView").GetComponent<ImpulesMainView>();
        impulesNPCView = transform.Find("NPCView").GetComponent<ImpulesNPCView>();
        impulesMainView.Init(impulesNPCView);
        impulesNPCView.Init(impulesMainView);
    }

    public void OpenImpulseView()
    {
        FindComponent();
        GetNPCID();
        GetServerInfo();
        JoinViewRefreshGift();
        //如果在隐藏中就刷新主界面
        if (UIComponent.GetComponentHaveExist<UIImpulseComponent>(UIType.UIImpulse) != null)
        {
            impulesMainView.ShowView(0);
        }
    }
    ////每次打开界面的时候获取礼物数据
    //private void OnEnable()
    //{
    //    Debug.LogError("运行了Enable");
    //    JoinViewRefreshGift();
    //    //如果在隐藏中就刷新主界面
    //    if (UIComponent.GetComponentHaveExist<UIImpulseComponent>(UIType.UIImpulse) != null)
    //    {
    //        impulesMainView.ShowView(0);
    //    }
    //}

    void GetNPCID()
    {
        foreach (var item in StaticData.configExcel.ImpulseText)
        {
            bool isSave = false;
            for (int i = 0; i < npcId.Count; i++)
            {
                if (npcId[i].NPCID != item.HeroId)
                    continue;

                npcId[i].dialogue.Add(item.ImpulseTextId);
                isSave = true;
                break;
            }
            if (isSave != true)
            {
                NPCDialogue nPCDialogue = new NPCDialogue();
                nPCDialogue.NPCID = item.HeroId;
                nPCDialogue.dialogue.Add(item.ImpulseTextId);
                npcId.Add(nPCDialogue);
            }
        }
    }

    /// <summary>
    /// 获取服务器上的一些数据
    /// </summary>
    void GetServerInfo()
    {
        if (StaticData.playerInfoData.favorableData.Count <= 0)//如果本地没有数据就请求并记录本地  只请求一次
        {
            //默认值（配置表里的数据）
            foreach (var item in StaticData.configExcel.HallRole)
            {
                if (!item.isImpulseRole) continue;//如果不是心动时刻的角色就下一个
                Favorable favorable = new Favorable();
                favorable.NPCId = item.ID;
                favorable.NpcSetPainting = item.SetPainting;
                favorable.headPortrait = item.HeadPortraitImage;
                favorableList.Add(favorable);
            }

            CSEmptyFavorableInfo cSEmptyFavorableInfo = new CSEmptyFavorableInfo();
            //bool receive = false;等待返回
            ProtocalManager.Instance().SendCSEmptyFavorableInfo(cSEmptyFavorableInfo, (SCFavorableInfo FavorableInfo) =>
            {
                Init(FavorableInfo);
                StaticData.playerInfoData.favorableData = favorableList;//数据保存到本地
            }, (ErrorInfo e) => { Debug.Log("请求失败"); });
            //await UniTask.WaitUntil(()=>receive==true);
        }
        else
        {
            Debug.Log("本地已缓存好感数据");
            favorableList = StaticData.playerInfoData.favorableData;
            impulesMainView.OpenView(favorableList);
        }
        //JoinViewRefreshGift();
    }
    //显示主界面
    private void Init(SCFavorableInfo FavorableInfo)
    {
        //获取好感值(目前还没存男主立绘 TODO)
        if (FavorableInfo != null)
            foreach (var item in FavorableInfo.FavorableInfo)
            {
                for (int i = 0; i < favorableList.Count; i++)
                {
                    if (item.HeroId != favorableList[i].NPCId) continue;
                    favorableList[i].NPC2PlayerValue = item.HeroPlayerValue;
                    favorableList[i].Player2NPCValue = item.PlayerHeroValue;
                    favorableList[i].cardiacValue = favorableList[i].NPC2PlayerValue <= favorableList[i].Player2NPCValue ? favorableList[i].NPC2PlayerValue : favorableList[i].Player2NPCValue;
                }
            }
        impulesMainView.OpenView(favorableList);
    }

    /// <summary>
    /// 进入心动界面时刷新礼物信息
    /// </summary>
    void JoinViewRefreshGift()
    {
        //获取礼物数据
        if (ImpulseGiftGoods.Count <= 0)
        {//获取所有心动礼物 (从道具表)
            foreach (var item in StaticData.configExcel.GameItem)//所有礼物
            {
                if (item.ItemType != Company.Cfg.TypeGameItem.HeartGift)
                    continue;
                ImpulseGiftGoods giftGoods = new ImpulseGiftGoods();
                giftGoods.goodsID = item.ID;
                giftGoods.goodsName = item.ItemName;
                giftGoods.goodsIcon = item.Icon;
                giftGoods.goodsDescription = item.Description;
                ImpulseGiftGoods.Add(giftGoods);
            }
            //获取礼物价格 (从商店表)
            foreach (var item in StaticData.configExcel.Store)
            {
                for (int i = 0; i < ImpulseGiftGoods.Count; i++)
                {
                    if (item.GoodId != ImpulseGiftGoods[i].goodsID)
                        continue;
                    ImpulseGiftGoods[i].goodsPriceBuy = item.OriginalPrice;
                    break;
                }
            }
            //获取礼物具体好感数值（从心动礼物表）
            foreach (var item in StaticData.configExcel.ImpulseGift)
            {
                for (int i = 0; i < ImpulseGiftGoods.Count; i++)
                {
                    if (item.ID != ImpulseGiftGoods[i].goodsID)
                        continue;
                    ImpulseGiftGoods[i].playerAddValue = item.PlayerAddFavorableValue;
                    ImpulseGiftGoods[i].NpcAddValue = item.NPCAddFavorableValue;
                    break;
                }
            }

        }
        //从仓库中获取礼物数量
        RefreshLastGift();
    }

    /// <summary>
    /// 购买礼物后 刷新（前端缓存）礼物列表
    /// </summary>
    public void BuyRefreshGift(int giftID, int amount)
    {
        foreach (var item in ImpulseGiftGoods)
        {
            if (item.goodsID == giftID)
            {
                item.goodsAmount += amount;
            }
        }
    }

    /// <summary>
    /// 刷新仓库中拥有的礼物（送礼失败后刷新礼物列表）
    /// </summary>
    public void RefreshLastGift()
    {
        foreach (var item in ImpulseGiftGoods)
        {
            var goods = StaticData.GetWareHouseItem(item.goodsID);
            if (goods != null)
            {
                item.goodsAmount = goods.GoodNum;
            }
        }
    }
}

/// <summary>
/// 好感帮助类
/// </summary>
public class ImpulseHelper
{
    /// <summary>
    /// 好感值存入缓存（外部初始化好感列表）章节用到
    /// </summary>
    public static void CacheFavorable()
    {
        List<Favorable> favorableList = new List<Favorable>();
        //好感列表赋初值
        foreach (var item in StaticData.configExcel.HallRole)
        {
            if (!item.isImpulseRole) continue;//如果不是心动时刻的角色就下一个
            Favorable favorable = new Favorable();
            favorable.NPCId = item.ID;
            favorable.NpcSetPainting = item.SetPainting;
            favorable.headPortrait = item.HeadPortraitImage;
            favorableList.Add(favorable);
        }
        CSEmptyFavorableInfo cSEmptyFavorableInfo = new CSEmptyFavorableInfo();
        ProtocalManager.Instance().SendCSEmptyFavorableInfo(cSEmptyFavorableInfo, (SCFavorableInfo FavorableInfo) =>
        {
            if (FavorableInfo != null)//赋保存后的值
                foreach (var item in FavorableInfo.FavorableInfo)
                {
                    for (int i = 0; i < favorableList.Count; i++)
                    {
                        if (item.HeroId != favorableList[i].NPCId) continue;
                        favorableList[i].NPC2PlayerValue = item.HeroPlayerValue;
                        favorableList[i].Player2NPCValue = item.PlayerHeroValue;
                        favorableList[i].cardiacValue = favorableList[i].NPC2PlayerValue <= favorableList[i].Player2NPCValue ? favorableList[i].NPC2PlayerValue : favorableList[i].Player2NPCValue;
                    }
                }
            StaticData.playerInfoData.favorableData = favorableList;//数据保存到本地
            Debug.Log("好感列表初始化成功");
        }, (ErrorInfo e) => { Debug.LogError("请求初始化好感请求失败"); });
    }

    /// <summary>
    /// 选项加好感
    /// </summary>
    /// <param name="ChapterOptionId">章节选项id</param>
    public static void OptionAddFavorable(int ChapterOptionId)
    {
        Debug.Log($"选项增加好感度{ChapterOptionId}");
        CSSectionAddFavorable cSSectionAddFavorable = new CSSectionAddFavorable() { SectionOptionId = ChapterOptionId };
        ProtocalManager.Instance().SendCSSectionAddFavorable(cSSectionAddFavorable, (SCEmtpySectionAddFavorable sCEmtpySectionAddFavorable) =>
        {
            if (StaticData.playerInfoData.favorableData.Count <= 0)
            {//不特殊情况不会运行
                CacheFavorable();
            }
            else
            {//本地缓存
                foreach (var dialogueId in StaticData.configExcel.FavorableValue)
                {
                    if (ChapterOptionId != dialogueId.DialogueId)
                        continue;
                    foreach (var item in StaticData.playerInfoData.favorableData)
                    {
                        if (dialogueId.HeroId != item.NPCId)
                            continue;
                        item.NPC2PlayerValue += dialogueId.FavorableValue;
                        //好感增加提示
                        var Role = StaticData.configExcel.GetHallRoleByID(item.NPCId);
                        StaticData.CreateToastTips($"{Role.Name}好感度+{dialogueId.FavorableValue}");
                        break;
                    }
                    break;
                }
            }
            Debug.Log("请求成功");
        },
        (ErrorInfo e) =>
        {
            if (e.ErrorMessage == "服务器异常400008")
                Debug.Log("已选过该选项");
            else
                Debug.LogError("请求失败");
        }, false);
    }

    //排序
    public static List<Favorable> ImpulseSort(List<Favorable> favorableList)
    {
        if (favorableList.Count >= 2)
            favorableList.Sort((a, b) =>
            {
                return b.cardiacValue.CompareTo(a.cardiacValue);
            });
        return favorableList;
    }

    /// <summary>
    /// 获取送礼物时的对话
    /// </summary>
    public static string GetNPCDialogue(int dialogueID)
    {
        var dialogue = StaticData.configExcel.GetImpulseTextByImpulseTextId(dialogueID);

        switch (StaticData.linguisticType)
        {
            case Company.Cfg.LinguisticType.Simplified:
                return dialogue.SimplifiedChinese;
            case Company.Cfg.LinguisticType.Complex:
                return dialogue.TraditionalChinese;
            case Company.Cfg.LinguisticType.English:
                return dialogue.English;
            default:
                return dialogue.SimplifiedChinese;
        }
    }

}
