using Company.Cfg;
using Game.Protocal;
using Google.Protobuf.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// 仓库管理器
/// 2020/7/29 huangjiangdong
/// </summary>
public static class WarehouseTool
{
    #region 字段
    /// <summary>
    /// 物品是否新获得本地储存标签
    /// </summary>
    const string SAVE_DATA = "SaveGoodsData";
    /// <summary>
    /// 宝箱解锁信息
    /// </summary>
    public static Dictionary<int, long> _unlockTreasureChestIds = new Dictionary<int, long>();
    #endregion

    #region 函数

    #region 与服务器交互
    /// <summary>
    /// 使用
    /// </summary>
    public static void UseGoods(GoodsData goodsData)
    {
        GoodsData tageData = goodsData;
    }
    /// <summary>
    /// 卖出
    /// </summary>
    public static void OnSale(GoodsData goodsData, int number, Action<bool> saleAction)
    {
        //创建上传数据
        CSGoodSellData goodSellData = new CSGoodSellData();
        CSGoodStruct cSGoodStruct = new CSGoodStruct();
        //获取当前点击数据
        cSGoodStruct.GoodId = goodsData._id;
        cSGoodStruct.GoodNum = number;
        goodSellData.GoodSellInfo.Add(cSGoodStruct);

        ProtocalManager.Instance().SendCSGoodSellData(goodSellData, (SCGoodSellData data) =>
        {
            //更新玩家的金币和钻石数据
            for (int i = 0; i < data.GoodSellInfo.Count; i++)
            {
                int id = data.GoodSellInfo[i].GoodId;
                StaticData.UpdateWareHouseItems(id, data.GoodSellInfo[i].GoodNum);
                Debug.Log("刷新金币：" + id + "   Number:" + data.GoodSellInfo[i].GoodNum);
            }
            saleAction(true);
            //修改本地数据
            StaticData.UpdateWareHouseItem(goodsData._id, -number);
            Debug.Log("卖出物品成功ID：" + goodsData._id + "   Number:" + number);
        }, (ErrorInfo er) =>
        {
            saleAction(false);
            Debug.Log("卖出物品失败ID：" + goodsData._id + "   Number:" + number + "  ErrorMessage:" + er.ErrorMessage);
        });
    }
    /// <summary>
    /// 卖出(一键卖出)
    /// </summary>
    public static void OnSale(List<GoodsData> goodsDatas, Action<bool, List<int>> saleAction)
    {
        //创建上传数据
        CSGoodSellData goodSellData = new CSGoodSellData();
        List<int> ids = new List<int>();
        for (int i = 0; i < goodsDatas.Count; i++)
        {
            CSGoodStruct cSGoodStruct = new CSGoodStruct();
            cSGoodStruct.GoodId = goodsDatas[i]._id;
            goodSellData.GoodSellInfo.Add(cSGoodStruct);
            ids.Add(goodsDatas[i]._id);
        }

        ProtocalManager.Instance().SendCSGoodSellData(goodSellData, (SCGoodSellData data) =>
        {
            saleAction?.Invoke(true, ids);
            //更新玩家的金币和钻石数据
            for (int i = 0; i < data.GoodSellInfo.Count; i++)
            {
                int id = data.GoodSellInfo[i].GoodId;
                StaticData.UpdateWareHouseItems(id, data.GoodSellInfo[i].GoodNum);
            }
            for (int i = 0; i < goodsDatas.Count; i++)
            {
                StaticData.UpdateWareHouseItems(goodsDatas[i]._id, 0);
            }
            Debug.Log("一键卖出成功：" + data.GoodSellInfo.Count);
        }, (ErrorInfo er) =>
        {
            saleAction?.Invoke(false, ids);
            Debug.Log("一键卖出失败：" + er.ErrorMessage);
        });
    }
    /// <summary>
    /// 更新物品锁定信息
    /// </summary>
    /// <param name="goodsDatas"></param>
    public static void UpdateAllLock(List<GoodsData> goodsDatas, Action<bool> result)
    {
        CSGoodLock cSGoodLock = new CSGoodLock();
        for (int i = 0; i < goodsDatas.Count; i++)
        {
            cSGoodLock.GoodId.Add(goodsDatas[i]._id);
            //修改本地数据
            StaticData.UpdateWareHouseItem(goodsDatas[i]._id, 0, goodsDatas[i]._isLock, true);
        }
        ProtocalManager.Instance().SendCSGoodLock(cSGoodLock, (errorInfo) =>
        {
            result(true);
            Debug.Log("更新锁定信息成功");
        }, (errorInfo) =>
        {
            result(false);
            Debug.Log("更新锁定信息失败");
        });

    }
    /// <summary>
    /// 获取已解锁宝箱id
    /// </summary>
    public static void GetUnlockTreasureChestID()
    {
        CSEmptyWarehouseUnlockInfo cSEmptyWarehouseUnlockInfo = new CSEmptyWarehouseUnlockInfo();

        ProtocalManager.Instance().SendCSEmptyWarehouseUnlockInfo(cSEmptyWarehouseUnlockInfo, (errorInfo) =>
        {

            if (errorInfo != null && errorInfo.UnlockInfo != null)
            {
                Dictionary<int, long> ids = new Dictionary<int, long>();
                for (int i = 0; i < errorInfo.UnlockInfo.Count; i++)
                {
                    //ids.Add(errorInfo.UnlockInfo[i].BoxId, errorInfo.UnlockInfo[i].UnlockTime);
                    if (_unlockTreasureChestIds.ContainsKey(errorInfo.UnlockInfo[i].BoxId))
                    {
                        _unlockTreasureChestIds[errorInfo.UnlockInfo[i].BoxId] = errorInfo.UnlockInfo[i].UnlockTime;
                    }
                    else
                    {
                        _unlockTreasureChestIds.Add(errorInfo.UnlockInfo[i].BoxId, errorInfo.UnlockInfo[i].UnlockTime);
                    }

                }
                WarehouseController.Instance.RefreshData();
            }
            Debug.Log("成功获取已经解锁宝箱id");
        }, (errorInfo) =>
        {

            Debug.Log("获取已解锁宝箱id失败");
        });
    }
    /// <summary>
    /// 修改宝箱解锁信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="unlockTime"></param>
    public static void ChangeUnlockTreasureChestID(int id, long unlockTime)
    {
        if (_unlockTreasureChestIds.ContainsKey(id))
        {
            _unlockTreasureChestIds[id] = unlockTime;
        }
        else
        {
            _unlockTreasureChestIds.Add(id, unlockTime);
        }
    }
    /// <summary>
    /// 钥匙解锁对应宝箱
    /// </summary>
    /// <param name="treasureChestId"></param>
    /// <param name="resultAction"></param>
    public static void UnlockTreasureChest(int treasureChestId, UnlockTreasureChest unlockTreasureType, Action<bool, long> resultAction)
    {
        CSTreasureChest cSTreasureChest = new CSTreasureChest();
        cSTreasureChest.GoodId = treasureChestId;
        cSTreasureChest.ConsumptionType = unlockTreasureType;
        ProtocalManager.Instance().SendCSTreasureChest(cSTreasureChest, (errorInfo) =>
        {

            Debug.Log("宝箱解锁成功成功，Type:" + unlockTreasureType + "   ID:" + treasureChestId);
            if (errorInfo == null)
            {
                Debug.Log("钥匙解锁");
                resultAction(true, 0);
            }
            else
            {
                Debug.Log("时间解锁");
                resultAction(true, errorInfo.UnlockTime);
            }

        }, (errorInfo) =>
        {
            resultAction(false, 0);
            Debug.Log("宝箱解锁成功失败，Type:" + unlockTreasureType + "   ID:" + treasureChestId);
        });
    }
    /// <summary>
    /// 领取宝箱
    /// </summary>
    /// <param name="id"></param>
    /// <param name="resultAction"></param>
    public static void GetAward(int id, Action<List<CSWareHouseStruct>> resultAction)
    {
        CSGetTreasureChestAward cSGetTreasureChestAward = new CSGetTreasureChestAward();
        cSGetTreasureChestAward.GoodId = id;
        ProtocalManager.Instance().SendCSGetTreasureChestAward(cSGetTreasureChestAward, (errorInfo) =>
        {
            List<CSWareHouseStruct> goods = new List<CSWareHouseStruct>();
            for (int i = 0; i < errorInfo.UsePropInfo.Count; i++)
            {
                goods.Add(errorInfo.UsePropInfo[i]);
                StaticData.UpdateWareHouseItem(errorInfo.UsePropInfo[i].GoodId, errorInfo.UsePropInfo[i].GoodNum);
            }
            resultAction?.Invoke(goods);
            StaticData.DataDot(DotEventId.OpenGiftBoxSucc);
            Debug.Log("领取宝箱奖品成功ID:" + id);
        }, (errorInfo) =>
        {
            resultAction?.Invoke(null);
            Debug.Log("领取宝箱奖品失败ID:" + id);
        });
    }
    /// <summary>
    /// 广告加速
    /// </summary>
    /// <param name="goodId"></param>
    /// <param name="resultAction"></param>
    public static void TreasureChestAdvertisingSpeedUp(int goodId, Action<bool, long> resultAction)
    {
        CSTreasureChestSpeed cSEmptyTreasureChestSpeed = new CSTreasureChestSpeed();
        cSEmptyTreasureChestSpeed.GoodId = goodId;
        ProtocalManager.Instance().SendCSTreasureChestSpeed(cSEmptyTreasureChestSpeed, (succ) =>
        {
            Debug.Log("宝箱广告加速成功");
            resultAction(true, succ.Time);
        }, (er) =>
        {
            if (er.webErrorCode == WebErrorCode.Advertising_Speed_Anomaly)
            {
                Debug.Log("操作频繁");
                resultAction(false, -1);
            }
            else
            {
                resultAction(false, 0);
            }
            Debug.Log("宝箱广告加速失败Code:" + er.webErrorCode + "   message:" + er.ErrorMessage);


        }, false);
    }
    /// <summary>
    /// 使用物品
    /// </summary>
    /// <param name="goodsData"></param>
    /// <param name="resultAction"></param>
    public static void UseGoods(GoodsData goodsData, Action<bool, SCUseWarehouseGoods, WebErrorCode> resultAction)
    {

        CSUseWarehouseGoods cSUseWarehouseGoods = new CSUseWarehouseGoods();
        cSUseWarehouseGoods.GoodsId = goodsData._id;
        cSUseWarehouseGoods.GoodNum = (int)goodsData._number;


        ProtocalManager.Instance().SendCSUseWarehouseGoods(cSUseWarehouseGoods, (data) =>
        {
            Debug.Log("返回成功：" + data);
            resultAction?.Invoke(true, data, WebErrorCode.None);

        }, (er) =>
        {
            resultAction?.Invoke(false, null, er.webErrorCode);
        }, false);
    }
    #endregion
    /// <summary>
    /// 解析数据
    /// </summary>
    /// <param name="cSWareHouseStructs"></param>
    public static void ParseData(RepeatedField<CSWareHouseStruct> cSWareHouseStructs, Action<List<GoodsData>, Dictionary<int, GoodsData>> AcceptCurrTotalDataAction)
    {
        List<GoodsData> goods = new List<GoodsData>();
        for (int i = 0; i < cSWareHouseStructs.Count; i++)
        {
            CSWareHouseStruct cSData = cSWareHouseStructs[i];
            if (cSData.GoodNum > 0 && IsBePutInStorage(cSData.GoodId))
            {
                GoodsData goodsData = new GoodsData();
                goodsData.Initial(cSData);
                goods.Add(goodsData);
            }

        }
        DataSort(goods, AcceptCurrTotalDataAction);
    }
    /// <summary>
    /// 数据排序
    /// </summary>
    /// <param name="goodsDatas"></param>
    public static void DataSort(List<GoodsData> goodsDatas, Action<List<GoodsData>, Dictionary<int, GoodsData>> AcceptCurrTotalDataAction)
    {
        List<GoodsData> datas = new List<GoodsData>();
        datas = goodsDatas;
        for (int i = 0; i < datas.Count; i++)
        {
            GoodsData data = datas[i];
            int index = i;

            //找出权值最大的物品
            for (int j = i + 1; j < datas.Count; j++)
            {
                //比较权值
                if (datas[i]._rarityValue < datas[j]._rarityValue)
                {
                    index = j;
                }
            }
            //交换
            if (index != i)
            {
                GoodsData ephemeral = datas[i];
                datas[i] = datas[index];
                datas[index] = ephemeral;
            }
        }
        //分辨新物品
        GetDistinguishDatas(datas, AcceptCurrTotalDataAction);
    }
    /// <summary>
    /// 区分是否是新物品
    /// </summary>
    /// <param name="datas"></param>
    static void GetDistinguishDatas(List<GoodsData> datas, Action<List<GoodsData>, Dictionary<int, GoodsData>> AcceptCurrTotalDataAction)
    {
        List<GoodsData> totalGiids = new List<GoodsData>();
        Dictionary<int, GoodsData> totaDataDic = new Dictionary<int, GoodsData>();

        //新获得物品
        List<GoodsData> newDatas = new List<GoodsData>();
        //已有物品
        List<GoodsData> oldDatas = new List<GoodsData>();
        //获取本地储存物品标记
        string saveValue = PlayerPrefs.GetString(SAVE_DATA, "");
        List<int> ids = SplitString(saveValue);

        for (int i = 0; i < datas.Count; i++)
        {
            if (ids.Contains(datas[i]._id))
            {
                datas[i]._isNewData = false;
                oldDatas.Add(datas[i]);
            }
            else
            {
                datas[i]._isNewData = true;
                newDatas.Add(datas[i]);
            }
        }

        for (int i = 0; i < newDatas.Count; i++)
        {
            totalGiids.Add(newDatas[i]);
            totaDataDic.Add(newDatas[i]._id, newDatas[i]);
        }

        for (int i = 0; i < oldDatas.Count; i++)
        {
            totalGiids.Add(oldDatas[i]);
            totaDataDic.Add(oldDatas[i]._id, oldDatas[i]);
        }

        AcceptCurrTotalDataAction?.Invoke(totalGiids, totaDataDic);
    }
    /// <summary>
    /// 根据商品类型划分商品
    /// </summary>
    /// <param name="totalGiids"></param>
    /// <param name="oldDatas"></param>
    public static void GetDistinguishDataType(List<GoodsData> totalGiids, Action<List<GoodsData>, List<GoodsData>, List<GoodsData>, List<GoodsData>, List<GoodsData>> AcceptDistinguishDataTypeAction)
    {
        List<GoodsData> seeds = new List<GoodsData>();
        List<GoodsData> fruits = new List<GoodsData>();
        List<GoodsData> decorates = new List<GoodsData>();
        List<GoodsData> gameItems = new List<GoodsData>();
        List<GoodsData> treasureChests = new List<GoodsData>();

        for (int i = 0; i < totalGiids.Count; i++)
        {
            GoodsData data = totalGiids[i];
            switch (data._data.KnapsackType)
            {
                case KnapsackType.None:
                    break;
                case KnapsackType.Seed:
                    seeds.Add(data);
                    break;
                case KnapsackType.Fruit:
                    fruits.Add(data);
                    break;
                case KnapsackType.Prop:
                    //分离宝箱
                    if (data._data.ItemType == TypeGameItem.TreasureChest)
                    {
                        treasureChests.Add(data);
                    }
                    else
                    {
                        gameItems.Add(data);
                    }
                    break;
                case KnapsackType.Decorate:
                    decorates.Add(data);
                    break;
            }

        }
        AcceptDistinguishDataTypeAction?.Invoke(seeds, fruits, decorates, gameItems, treasureChests);
    }
    /// <summary>
    /// 获取宝箱数据
    /// </summary>
    /// <param name="datas"></param>
    /// <returns></returns>
    public static List<GoodsData> GetTreasureChests(List<GoodsData> datas)
    {
        List<GoodsData> treasureChests = new List<GoodsData>();
        for (int i = 0; i < datas.Count; i++)
        {
            if (datas[i]._data.KnapsackType == KnapsackType.Prop && datas[i]._data.ItemType == TypeGameItem.TreasureChest)
            {
                treasureChests.Add(datas[i]);
            }
        }

        return treasureChests;
    }
    /// <summary>
    /// 保存物品标记到本地
    /// </summary>
    /// <param name="data"></param>
    public static void SetGoodsDataNew(GoodsData data)
    {
        if (DataIsSave(data))
        {
            return;
        }
        string dataId = data._id.ToString();
        string oldStr = PlayerPrefs.GetString(SAVE_DATA, "");

        PlayerPrefs.SetString(SAVE_DATA, oldStr + dataId + ',');
    }
    /// <summary>
    /// 是否获得过此物品
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static bool DataIsSave(GoodsData data)
    {
        int dataId = data._id;
        string saveValue = PlayerPrefs.GetString(SAVE_DATA, "");
        List<int> ids = SplitString(saveValue);
        return ids.Contains(dataId);
    }
    /// <summary>
    /// 保存宝箱数量
    /// </summary>
    /// <param name="id"></param>
    /// <param name="number"></param>
    public static void SaveTreasureChaestsValue(int id, int number)
    {
        PlayerPrefs.SetInt(SAVE_DATA + id, number);
    }
    /// <summary>
    /// 获取宝箱保存数量
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static int GetTreasureChaestsValue(int id)
    {
        int saveValue = PlayerPrefs.GetInt(SAVE_DATA + id, 0);
        return saveValue;
    }
    /// <summary>
    /// 分割字符串
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    static List<int> SplitString(string str)
    {

        List<int> ids = new List<int>();
        if (string.IsNullOrEmpty(str))
        {
            return ids;
        }
        string[] strArray = str.Split(',');

        for (int i = 0; i < strArray.Length; i++)
        {
            int id = 0;
            if (int.TryParse(strArray[i], out id))
            {
                ids.Add(id);
            }
        }

        return ids;

    }
    /// <summary>
    /// 分割字符串
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static List<string> SplitStringStr(string str, char separator)
    {

        List<string> ids = new List<string>();
        if (string.IsNullOrEmpty(str))
        {
            return ids;
        }
        string[] strArray = str.Split(separator);

        for (int i = 0; i < strArray.Length; i++)
        {
            ids.Add(strArray[i]);
        }

        return ids;

    }
    /// <summary>
    /// 获取当前仓库总价
    /// </summary>
    /// <returns></returns>
    public static string GetCurrWareHouseTotalPrices(List<GoodsData> currGoodsDatas)
    {
        string str = "";

        long totalPrices = 0;
        for (int i = 0; i < currGoodsDatas.Count; i++)
        {
            GoodsData data = currGoodsDatas[i];
            if (!data._isLock && data._data.PriceSell != null && data._data.PriceSell.Count != 0)
            {
                int prices = 0;

                for (int e = 0; e < data._data.PriceSell.Count; e++)
                {
                    if (data._data.PriceSell[e].IdGameItem == StaticData.configExcel.GetVertical().GoldGoodsId)
                    {
                        prices = data._data.PriceSell[0].Price;
                    }
                }
                totalPrices = totalPrices + (prices * data._number);
            }
        }
        str = str + totalPrices.ToString();

        return str;

    }
    /// <summary>
    /// 判断该id是否是金币
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool IsGold(int id)
    {
        if (id == StaticData.configExcel.GetVertical().GoldGoodsId)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    /// <summary>
    /// 判断该id是否是钻石
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool IsJewel(int id)
    {
        if (id == StaticData.configExcel.GetVertical().JewelGoodsId)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    /// <summary>
    /// 判断该物品是否可以入库
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool IsBePutInStorage(int id)
    {
        GameItemDefine gameItemDefine = GetGameItemData(id);
        if (gameItemDefine == null)
        {
            return false;
        }
        if (gameItemDefine.KnapsackType == KnapsackType.None)
        {
            return false;
        }
        else
        {
            return true;
        }

    }
    /// <summary>
    /// 判断该商品是否可卖
    /// </summary>
    /// <param name="goodsData"></param>
    /// <returns></returns>
    public static bool IsGoodsSell(GoodsData goodsData)
    {
        return goodsData._data.PriceSell != null && goodsData._data.PriceSell.Count != 0;
    }
    /// <summary>
    /// 根据物品id获取道具配置数据
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static GameItemDefine GetGameItemData(int id)
    {
        GameItemDefine data = null;
        for (int i = 0; i < StaticData.configExcel.GameItem.Count; i++)
        {
            GameItemDefine gameItemDefine = StaticData.configExcel.GameItem[i];
            if (gameItemDefine.ID == id)
            {
                data = gameItemDefine;
            }
        }

        return data;
    }
    /// <summary>
    /// 获取仓库中对应道具的数量
    /// </summary>
    /// <returns></returns>
    public static int GetWareHouseGold(int id)
    {
        var item = StaticData.GetWareHouseItem(id);
        if (item == null)
            return 0;
        return item.GoodNum;
    }
    /// <summary>
    /// 根据道具获取对应宝箱配置数据
    /// </summary>
    /// <param name="id"></param>
    public static PackageDefine GetTreasureChestConfig(int id)
    {
        for (int i = 0; i < StaticData.configExcel.Package.Count; i++)
        {
            PackageDefine data = StaticData.configExcel.Package[i];
            if (data.BoxID == id)
            {
                return data;
            }
        }

        return null;

    }
    /// <summary>
    /// 根据宝箱id获取对应宝箱配置数据
    /// </summary>
    /// <param name="id"></param>
    public static PackageDefine GetTreasureChestConfigData(int id)
    {
        for (int i = 0; i < StaticData.configExcel.Package.Count; i++)
        {
            PackageDefine data = StaticData.configExcel.Package[i];
            if (data.ID == id)
            {
                return data;
            }
        }

        return null;

    }
    /// <summary>
    /// 根据宝箱id获取宝箱稀有等级
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static TypeRarity GetTreasureChestRareGrade(int id)
    {
        GameItemDefine boxIdKeysTime = GetGameItemData(id);
        if (boxIdKeysTime == null)
        {
            return TypeRarity.None;
        }
        return boxIdKeysTime.Rarity;


    }
    /// <summary>
    /// 判断是否有解锁的宝箱
    /// </summary>
    public static bool IsThereAnUnlock(int id)
    {
        bool isUnlock = false;
        if (_unlockTreasureChestIds.ContainsKey(id) && _unlockTreasureChestIds[id] != 0)
        {
            //计算当前服务器时间
            long CurrTimeStampServer = TimeHelper.ServerTimeStampNow;
            //计算剩余时间
            long CurrRemainTime = _unlockTreasureChestIds[id] - CurrTimeStampServer;
            if (CurrRemainTime <= 0)
            {
                isUnlock = true;
            }
        }
        return isUnlock;
    }
    /// <summary>
    /// 判断是否有正在时间解锁的宝箱
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool IsTimeThereAnUnlock(int id)
    {
        bool isUnlock = false;
        if (_unlockTreasureChestIds.ContainsKey(id) && _unlockTreasureChestIds[id] != 0)
        {
            //计算当前服务器时间
            long CurrTimeStampServer = TimeHelper.ServerTimeStampNow;
            //计算剩余时间
            long CurrRemainTime = _unlockTreasureChestIds[id] - CurrTimeStampServer;

            Debug.Log("正在解锁：" + id);
            Debug.Log("Time：" + CurrRemainTime);
            Debug.Log("宝箱解锁时间：" + _unlockTreasureChestIds[id]);
            Debug.Log("当前服务器时间：" + CurrTimeStampServer);
            if (CurrRemainTime > 0)
            {
                isUnlock = true;
            }
        }
        return isUnlock;
    }
    /// <summary>
    /// 获取正在时间解锁的宝箱的时间戳
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static long GetTimeThereAnUnlockTime(int id)
    {
        if (_unlockTreasureChestIds.ContainsKey(id))
        {
            return _unlockTreasureChestIds[id];
        }
        return 0;
    }
    /// <summary>
    /// 判断该商品是否是新商品
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static bool IsNewGoods(GoodsData data)
    {
        //获取本地储存物品标记
        string saveValue = PlayerPrefs.GetString(SAVE_DATA, "");
        List<int> ids = SplitString(saveValue);
        if (ids.Contains(data._id))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    /// <summary>
    /// 判断是否开启红点
    /// </summary>
    /// <returns></returns>
    public static bool IsOpenDot()
    {
        List<GoodsData> goods = new List<GoodsData>();
        for (int i = 0; i < StaticData.playerInfoData.userInfo.WareHoseInfo.Count; i++)
        {
            CSWareHouseStruct cSData = StaticData.playerInfoData.userInfo.WareHoseInfo[i];
            if (cSData.GoodNum > 0 && IsBePutInStorage(cSData.GoodId))
            {
                GoodsData goodsData = new GoodsData();
                goodsData.Initial(cSData);
                goods.Add(goodsData);
            }
        }
        for (int i = 0; i < goods.Count; i++)
        {
            if (goods[i]._data.ItemType != TypeGameItem.TreasureChest && IsNewGoods(goods[i]))
            {
                return true;
            }
        }
        List<GoodsData> treasureChaests = GetTreasureChests(goods);
        Dictionary<int, int> treasureChaestsIndex = new Dictionary<int, int>();
        for (int i = 0; i < treasureChaests.Count; i++)
        {
            GoodsData treasureChaestsData = treasureChaests[i];
            TypeRarity key = treasureChaestsData._data.Rarity;
            int keyNumber = (int)key;
            if (treasureChaestsIndex.ContainsKey(keyNumber))
            {
                int index = treasureChaestsIndex[keyNumber];
                treasureChaestsIndex[keyNumber] = index + 1;
            }
            else
            {
                treasureChaestsIndex.Add(keyNumber, 1);
            }
        }
        foreach (var item in treasureChaestsIndex)
        {
            int index = GetTreasureChaestsValue(item.Key);
            if (index < item.Value)
            {
                return true;
            }
        }
        //for (int i = 0; i < treasureChaests.Count; i++)
        //{
        //    if (IsThereAnUnlock(treasureChaests[i]._id))
        //    {
        //        return true;
        //    }
        //}
        return false;
    }
    #endregion
}
/// <summary>
/// 商品数据
/// </summary>
public class GoodsData
{
    #region 字段
    /// <summary>
    /// 商品id
    /// </summary>
    public int _id;
    /// <summary>
    /// 商品数量
    /// </summary>
    public long _number;
    /// <summary>
    /// 商品锁定状态
    /// </summary>
    public bool _isLock;
    /// <summary>
    /// 是否是新获得的数据
    /// </summary>
    public bool _isNewData;
    /// <summary>
    /// 对应配置数据
    /// </summary>
    public GameItemDefine _data;
    /// <summary>
    /// 排序权值
    /// </summary>
    public int _rarityValue
    {
        get
        {
            //Debug.Log(_id + "" + _data);
            return (int)_data.Rarity;
        }
    }
    #endregion
    #region 函数
    /// <summary>
    /// 初始化商品数据
    /// </summary>
    /// <param name="sCGoodInfo"></param>
    public void Initial(CSWareHouseStruct houseStruct)
    {
        _id = houseStruct.GoodId;
        _number = houseStruct.GoodNum;
        _isLock = houseStruct.IsLock;
        _data = WarehouseTool.GetGameItemData(_id);
    }
    /// <summary>
    /// 初始话商品
    /// </summary>
    /// <param name="houseStruct"></param>
    /// <param name="number"></param>
    public void Initial(SCBuyGoodsStruct houseStruct, int number)
    {
        _id = houseStruct.GoodsId;
        _number = number;
        _isLock = false;
        _data = WarehouseTool.GetGameItemData(_id);
    }
    public GoodsData CopyThis()
    {
        GoodsData goodsData = new GoodsData();
        goodsData._id = _id;
        goodsData._number = _number;
        goodsData._isLock = _isLock;
        goodsData._isNewData = _isNewData;
        goodsData._data = _data;
        return goodsData;
    }
    #endregion
}



