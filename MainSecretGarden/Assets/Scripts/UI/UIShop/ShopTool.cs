using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Company.Cfg;
using System.Text;

public class ShopTool : MonoBehaviour
{

    #region 字段
    //玩家查看商城数据
    const string SAVE_DATA_ShopPlayer = "SavePlayerLookData";

    //商城上新/促销数据
    const string SAVE_DATA_ShopItem = "SaveShopItem";
    const string SAVE_DATA_ShopSeed = "SaveShopSeed";
    const string SAVE_DATA_ShopOrnament = "SaveShopOrnament";
    #endregion

    //是否开启商城红点
    public static bool _isOpenShopReDot=false;

    //种子红点 道具红点 装饰红点
    public static bool _isOpenSeedRed = false;
    public static bool _isOpenItemRed = false;
    private static bool _isOpenOrnament = false;

    //商城是否上新/促销
    private static bool _isNewOrPromotion = false;

    //玩家是否查看过商城
    public static bool isLookStore = true;

    //商店种子、道具、装饰数据---用于比较保存本地的数据
    private static List<StoreDefine> listShopSeedList=new List<StoreDefine>();
    private static List<StoreDefine> listShopItemList=new List<StoreDefine>();
    private static List<StoreDefine> listShopOrnamentList=new List<StoreDefine>();


    //判断是否保存数据
    static List<bool> seedIdValue = new List<bool>();
    static List<bool> itemIdValue = new List<bool>();
    static List<bool> ornamentIdValue = new List<bool>(); 



    //初始化当前玩家商城查看状态
    private static void InitShopRedDotState()
    {

        #region 判断是否上新 促销

        if (seedIdValue.Count != 0)
        {
            //清除数据 
            seedIdValue.Clear();
            itemIdValue.Clear();
            ornamentIdValue.Clear();
        }

        if (listShopSeedList.Count != 0)
        {
            listShopSeedList.Clear();
            listShopItemList.Clear();
            listShopOrnamentList.Clear();
        }

        //判断是否保存数据
        foreach (var item in GetListy(SAVE_DATA_ShopSeed))
        {
            seedIdValue.Add(item);
        }

        foreach (var item in GetListy(SAVE_DATA_ShopItem))
        {
            itemIdValue.Add(item);
        }

        foreach (var item in GetListy(SAVE_DATA_ShopOrnament))
        {
            ornamentIdValue.Add(item);
        }


        if (seedIdValue.Count == 0 || itemIdValue.Count == 0 || ornamentIdValue.Count == 0)//玩家第一次进入 商店提醒
        {
            _isOpenShopReDot = true;
            return;
        }

        //当前的商店数据
        var storeC = StaticData.configExcel.Store;
        foreach (var elem in storeC)
        {
            switch (elem.MallType)
            {
                case ShopItemType.SeedStore:
                    listShopSeedList.Add(elem);
                    break;
                case ShopItemType.PropStore:
                    listShopItemList.Add(elem);
                    break;
                case ShopItemType.DecorateStore:
                    listShopOrnamentList.Add(elem);
                    break;
            }
        }


        //数据比较 新添加物品 上新 促销
        if (seedIdValue.Count < listShopSeedList.Count) //新物品添加
        {
            _isOpenSeedRed = true;//打开红点
        }
        else//没新物品添加 就判断是否是上新或促销状态
        {
            _isOpenSeedRed = false;
            for (int i = 0; i < seedIdValue.Count; i++)
            {
                if (seedIdValue[i]!= NewAndPromotion(listShopSeedList[i])) //和之前的状态发生变化
                {
                    if (seedIdValue[i] == false && NewAndPromotion(listShopSeedList[i]) == true)//新上新或新促销
                    {
                        _isOpenSeedRed = true;//打开红点
                        break;
                    }
                }
            }
        }

        if(itemIdValue.Count<listShopItemList.Count)
        {
            _isOpenItemRed = true;//打开红点
        }
        else
        {
            _isOpenItemRed = false;
            for (int i = 0; i < itemIdValue.Count; i++)
            {
                if (itemIdValue[i] != NewAndPromotion(listShopItemList[i])) //和之前的状态发生变化
                {
                    if (itemIdValue[i] == false && NewAndPromotion(listShopItemList[i]) == true)//新上新或新促销
                    {
                        _isOpenItemRed = true;//打开红点
                        break;
                    }
                }
            }
        }

        if (ornamentIdValue.Count < listShopOrnamentList.Count)
        {
            _isOpenOrnament = true;//打开红点
        }
        else
        {
            _isOpenOrnament = false;
            for (int i = 0; i < ornamentIdValue.Count; i++)
            {
                if (ornamentIdValue[i] != NewAndPromotion(listShopOrnamentList[i])) //和之前的状态发生变化
                {
                    if (ornamentIdValue[i] == false && NewAndPromotion(listShopOrnamentList[i]) == true)//新上新或新促销
                    {
                        _isOpenOrnament = true;//打开红点
                        break;
                    }
                }
            }
        }

        #endregion 


        if (_isOpenSeedRed == true || _isOpenItemRed == true || _isOpenOrnament == true)//满足其中一个
        {
            _isNewOrPromotion = true;
            isLookStore = false;//玩家没查看商店;
        }
        else
        {
            _isNewOrPromotion = false;
            isLookStore = true;
        }

        //玩家是否查看商店保存
        LookDataSave();


        //玩家是否查看过
        string isLookValue = PlayerPrefs.GetString(SAVE_DATA_ShopPlayer);
        if (isLookValue == "True")
        {
            _isOpenShopReDot = false;
        }
        else if (isLookValue == "False" && _isNewOrPromotion)//玩家没有查看并且没有上新或促销
        {
            _isOpenShopReDot = true;
        }
        else
        {
            _isOpenShopReDot = false;
        }
    }

    
    public static bool IsOpenRedDot()
    {
        InitShopRedDotState();
        return _isOpenShopReDot;
    }


    //查看物品保存到本地   是否进入过商城查看
    public static void LookDataSave()
    {
        PlayerPrefs.SetString(SAVE_DATA_ShopPlayer, isLookStore.ToString());
    }

    //保存商店数据
    public static void SavaShopData()
    {

        if (listShopSeedList.Count == 0 || listShopItemList.Count == 0 || listShopOrnamentList.Count == 0)
        {
            //当前的商店数据
            var storeC = StaticData.configExcel.Store;
            foreach (var elem in storeC)
            {
                switch (elem.MallType)
                {
                    case ShopItemType.SeedStore:
                        listShopSeedList.Add(elem);
                        break;
                    case ShopItemType.PropStore:
                        listShopItemList.Add(elem);
                        break;
                    case ShopItemType.DecorateStore:
                        listShopOrnamentList.Add(elem);
                        break;
                }
            }
        }

        SetList(SAVE_DATA_ShopSeed, listShopSeedList);

        SetList(SAVE_DATA_ShopItem, listShopItemList);

        SetList(SAVE_DATA_ShopOrnament, listShopOrnamentList);

        //清空链表
        listShopItemList.Clear();
        listShopSeedList.Clear();
        listShopOrnamentList.Clear();
    }

    //删除商店数据
    public static void DeleteShopData()
    {
        PlayerPrefs.DeleteKey(SAVE_DATA_ShopSeed);
        PlayerPrefs.DeleteKey(SAVE_DATA_ShopItem);
        PlayerPrefs.DeleteKey(SAVE_DATA_ShopOrnament);

        Debug.Log(GetListy(SAVE_DATA_ShopSeed).Length);
    }

    //判断是否有促销或者上新
    private static bool NewAndPromotion(StoreDefine storeDefine)
    {
        var nowTime = TimeHelper.ServerTimeStampNow;
        if (nowTime> storeDefine.UpNewTimeBegin&& nowTime < storeDefine.UpNewTimeEnd) //处于上新
        {
            if(nowTime > storeDefine.PromotionTimeBegin && nowTime < storeDefine.PromotionTimeEnd) //处于上新和促销
            {
                return true;
            }
            else
            {
                return true;
            }
        }
        else
        {
            if (nowTime > storeDefine.PromotionTimeBegin && nowTime < storeDefine.PromotionTimeEnd) //处于促销
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


    //保存链表工具（保存的是物品的上新和促销状态）
    private static void SetList(string key, List<StoreDefine> list)
    {
        if (list.Count == 0) return;

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < list.Count - 1; i++)
        {
            sb.Append(NewAndPromotion(list[i])).Append("|");
        }
        sb.Append(NewAndPromotion(list[list.Count - 1]));

        PlayerPrefs.SetString(key, sb.ToString());

    }

    //取出链表工具(取出的是物品的上新和促销状态)
    public static bool[] GetListy(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            string[] stringArray = PlayerPrefs.GetString(key).Split("|"[0]);
            bool[] intArray = new bool[stringArray.Length];
            for (int i = 0; i < stringArray.Length; i++)
                intArray[i] = Convert.ToBoolean(stringArray[i]);
            return intArray;
        }
        return new bool[0];
    }
}
