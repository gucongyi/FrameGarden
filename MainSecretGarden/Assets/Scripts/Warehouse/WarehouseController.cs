using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 仓库控制器
/// 2020/7/29 huangjiangdong
/// </summary>
public class WarehouseController : MonoBehaviour
{

    #region 字段
    public static WarehouseController Instance;
    /// <summary>
    /// 物品总表
    /// </summary>
    List<GoodsData> _totalGiids = new List<GoodsData>();
    /// <summary>
    /// 物品总表字典（物品id为key）（原始数据不变）
    /// </summary>
    Dictionary<int, GoodsData> _totaDataDic = new Dictionary<int, GoodsData>();
    /// <summary>
    /// 种子仓库
    /// </summary>
    List<GoodsData> _seeds = new List<GoodsData>();
    /// <summary>
    /// 果实仓库
    /// </summary>
    List<GoodsData> _fruits = new List<GoodsData>();
    /// <summary>
    /// 道具仓库
    /// </summary>
    List<GoodsData> _gameItems = new List<GoodsData>();
    /// <summary>
    /// 装饰物仓库
    /// </summary>
    List<GoodsData> _decorates = new List<GoodsData>();
    /// <summary>
    /// 宝箱字典 根据稀有度排序
    /// </summary>
    Dictionary<int, List<GoodsData>> _treasureChestDic = new Dictionary<int, List<GoodsData>>();
    /// <summary>
    /// 当前展示仓库数据
    /// </summary>
    List<GoodsData> _currGoodsDatas = new List<GoodsData>();
    /// <summary>
    /// 详情弹窗
    /// </summary>
    WarehouseDetailsController _detailsController;
    /// <summary>
    /// 卖出弹窗
    /// </summary>
    WarehouseSaleController _saleController;
    /// <summary>
    /// 道具使用弹窗
    /// </summary>
    WarehouseUseController _warehouseUseController;
    /// <summary>
    /// 当前点击item
    /// </summary>
    WarehouseItem _currClickItme;
    /// <summary>
    /// 当前仓库类型
    /// 0:种子 1：果实 2：道具 3：装饰
    /// </summary>
    int _currWareHouseType = -1;
    /// <summary>
    /// 选项按钮集合
    /// </summary>
    [SerializeField]
    List<WarehouseSelectItemController> _selectBtnList = new List<WarehouseSelectItemController>();
    /// <summary>
    /// 选项按钮左边空距
    /// </summary>
    [SerializeField]
    float _selectBtnLeftPadding;
    /// <summary>
    /// 选项按钮之间的间隔
    /// </summary>
    [SerializeField]
    float _selectBtnInterval;
    /// <summary>
    /// 选项按钮父级
    /// </summary>
    [SerializeField]
    RectTransform _selectBoxRect;
    /// <summary>
    /// 滑动列表
    /// </summary>
    LoopScrollRect _loopScrollRect = null;
    /// 一键卖出
    /// </summary>
    Button _oneKeySaleBtn;
    /// <summary>
    /// 分割线
    /// </summary>
    RectTransform _lineRect;
    /// <summary>
    /// 显示容量
    /// </summary>
    Text _showCapacityText;
    /// <summary>
    /// 显示总价值
    /// </summary>
    Text _showPriceText;
    /// <summary>
    /// 关闭按钮
    /// </summary>
    Button _closeBtn;
    /// <summary>
    /// 宝箱box
    /// </summary>
    Transform _treasureBoxTra;
    /// <summary>
    /// 宝箱按钮字典
    /// </summary>
    Dictionary<int, WarehouseTreasureChest> _warehouseTreasureChestDic = new Dictionary<int, WarehouseTreasureChest>();
    /// <summary>
    /// 宝箱关闭按钮字典
    /// </summary>
    Dictionary<int, Transform> _warehouseTreasureChestNoDic = new Dictionary<int, Transform>();
    WarehouseTreasureChest _lowClass;
    Transform _lowClassNo;
    /// <summary>
    /// 中级宝箱
    /// </summary>
    WarehouseTreasureChest _middleRank;
    Transform _middleRankNo;
    /// <summary>
    /// 高级宝箱
    /// </summary>
    WarehouseTreasureChest _highClass;
    Transform _highClassNo;
    /// <summary>
    /// 宝箱遮罩按钮
    /// </summary>
    Button _treasureChestMaskBtn;
    /// <summary>
    /// 仓库显示范围
    /// </summary>
    RectTransform _itemBoxRect;
    /// <summary>
    /// 仓库类型
    /// 0:种子 1：果实 2：道具 3：装饰
    /// </summary>
    int _warehouseType = 0;
    /// <summary>
    /// 是否初始化
    /// </summary>
    bool _isInitial = false;
    /// <summary>
    /// 仓库面板背景
    /// </summary>
    [SerializeField]
    List<Sprite> _bgSprites = new List<Sprite>();
    /// <summary>
    /// 一键卖出按钮图片
    /// </summary>
    [SerializeField]
    List<Sprite> _onKeyBtnSprites = new List<Sprite>();
    /// <summary>
    /// 循环列表item引用
    /// </summary>
    [SerializeField]
    WarehouseItem _listItem;
    /// <summary>
    /// 面板背景
    /// </summary>
    Image _bgImage;
    #endregion
    #region 属性
    /// <summary>
    /// 当前仓库类型
    /// </summary>
    public int _WarehouseType { get { return _warehouseType; } }
    #endregion
    #region 函数
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        UniversalTool.ReadyUIAnimTwo(transform.Find("Box").GetComponent<CanvasGroup>(), transform.Find("Box"));
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!_isInitial)
        {
            Initial();
        }
    }
    // Update is called once per frame
    void Update()
    {
    }
    private void OnEnable()
    {
        UniversalTool.StartUIAnimTwo(transform.Find("Box").GetComponent<CanvasGroup>(), transform.Find("Box"));
    }
    private void OnDisable()
    {
        UniversalTool.ReadyUIAnimTwo(transform.Find("Box").GetComponent<CanvasGroup>(), transform.Find("Box"));
    }
    /// <summary>
    /// 初始控制器
    /// </summary>
    public void Initial()
    {
        _bgImage = transform.Find("Box").GetComponent<Image>();
        _loopScrollRect = transform.Find("Box/Itembox").GetComponent<LoopScrollRect>();
        _showCapacityText = transform.Find("Box/ShowCapacity/Number").GetComponent<Text>();
        _closeBtn = transform.Find("Box/CloseBtn").GetComponent<Button>();
        _closeBtn.onClick.RemoveAllListeners();
        _closeBtn.onClick.AddListener(Close);

        _showPriceText = transform.Find("Box/ShowPrice/Price").GetComponent<Text>();
        _oneKeySaleBtn = transform.Find("Box/SaleButton").GetComponent<Button>();
        _lineRect = transform.Find("Box/Line").GetComponent<RectTransform>();
        _treasureBoxTra = transform.Find("Box/TreasureBox");
        _lowClass = _treasureBoxTra.Find("LowClass").GetComponent<WarehouseTreasureChest>();
        _lowClassNo = _treasureBoxTra.Find("LowClassNo");
        _middleRank = _treasureBoxTra.Find("MiddleRank").GetComponent<WarehouseTreasureChest>();
        _middleRankNo = _treasureBoxTra.Find("MiddleRankNo");
        _highClass = _treasureBoxTra.Find("HighClass").GetComponent<WarehouseTreasureChest>();
        _highClassNo = _treasureBoxTra.Find("HighClassNo");
        _warehouseTreasureChestDic.Clear();
        _warehouseTreasureChestDic.Add(1, _lowClass);
        _warehouseTreasureChestDic.Add(2, _middleRank);
        _warehouseTreasureChestDic.Add(3, _highClass);
        _warehouseTreasureChestNoDic.Add(1, _lowClassNo);
        _warehouseTreasureChestNoDic.Add(2, _middleRankNo);
        _warehouseTreasureChestNoDic.Add(3, _highClassNo);
        _treasureChestMaskBtn = _treasureBoxTra.Find("Mask").GetComponent<Button>();
        _itemBoxRect = transform.Find("Box/Itembox").GetComponent<RectTransform>();


        _oneKeySaleBtn.onClick.RemoveAllListeners();
        _oneKeySaleBtn.onClick.AddListener(OneKeySale);
        SetPanelMultilingual();
        WarehouseTool.GetUnlockTreasureChestID();
        _isInitial = true;

    }
    public void Show(int type)
    {
        _warehouseType = type;
        //获取仓库数据
        WarehouseTool.ParseData(StaticData.playerInfoData.userInfo.WareHoseInfo, AcceptCurrTotalData);
    }
    #region 处理数据
    /// <summary>
    /// 接收解析后并排序分辨完新旧的仓库数据
    /// </summary>
    /// <param name="totalGiids"></param>
    /// <param name="totaDataDic"></param>
    void AcceptCurrTotalData(List<GoodsData> totalGiids, Dictionary<int, GoodsData> totaDataDic)
    {
        _totalGiids.Clear();
        _totaDataDic.Clear();
        foreach (var item in totaDataDic)
        {
            _totaDataDic.Add(item.Key, item.Value.CopyThis());
            _totalGiids.Add(item.Value.CopyThis());
        }
        WarehouseTool.GetDistinguishDataType(_totalGiids, AcceptDistinguishDataType);
    }
    /// <summary>
    /// 接受划分类型后的仓库数据
    /// </summary>
    /// <param name="seeds"></param>
    /// <param name="fruits"></param>
    /// <param name="decorates"></param>
    /// <param name="gameItems"></param>
    void AcceptDistinguishDataType(List<GoodsData> seeds, List<GoodsData> fruits, List<GoodsData> decorates, List<GoodsData> gameItems, List<GoodsData> treasureChests)
    {
        if (!_isInitial)
        {
            Initial();
        }

        _seeds.Clear();
        _seeds.AddRange(seeds);
        _fruits.Clear();
        _fruits.AddRange(fruits);
        _decorates.Clear();
        _decorates.AddRange(decorates);
        _gameItems.Clear();
        _gameItems.AddRange(gameItems);
        _treasureChestDic.Clear();
        for (int i = 0; i < treasureChests.Count; i++)
        {
            GoodsData goodsData = treasureChests[i];
            int key = (int)goodsData._data.Rarity;
            if (!_treasureChestDic.ContainsKey(key))
            {
                List<GoodsData> goods = new List<GoodsData>();
                goods.Add(goodsData);
                _treasureChestDic.Add(key, goods);
            }
            else
            {
                _treasureChestDic[key].Add(goodsData);
            }
        }
        if (_currWareHouseType == -1)
        {
            for (int i = 0; i < _selectBtnList.Count; i++)
            {
                bool isOpenToggle = false;
                if (i == _warehouseType)
                {
                    isOpenToggle = true;
                }
                _selectBtnList[i].Initial(SelectBtnRank, isOpenToggle);
            }
            //SelectBtnRank(_warehouseType);
        }
        SelectBtnRank(_warehouseType);
        //RefreshData();
    }
    /// <summary>
    /// 根据类型返回数据
    /// </summary>
    /// <param name="type"></param>
    public int ChangeCurrShowGoodsData(int type)
    {
        _currGoodsDatas.Clear();
        _currWareHouseType = type;
        switch (_currWareHouseType)
        {
            case 0:
                _currGoodsDatas.AddRange(_seeds);
                break;
            case 1:
                _currGoodsDatas.AddRange(_fruits);
                break;
            case 2:
                _currGoodsDatas.AddRange(_gameItems);
                break;
            case 3:
                _currGoodsDatas.AddRange(_decorates);
                break;
        }
        return _currGoodsDatas.Count;
    }
    #endregion
    #region 宝箱
    /// <summary>
    /// 刷新宝箱
    /// </summary>
    public void RefreshTreasureChest()
    {
        for (int i = 0; i < 3; i++)
        {
            int key = i + 1;
            if (_treasureChestDic.ContainsKey(key))
            {
                _warehouseTreasureChestDic[key].InitialData(key, _treasureChestDic[key]);
                _warehouseTreasureChestNoDic[key].gameObject.SetActive(false);
            }
            else
            {
                _warehouseTreasureChestDic[key].InitialData(key, null);
                _warehouseTreasureChestNoDic[key].gameObject.SetActive(true);
            }
        }
    }
    /// <summary>
    /// 消耗掉一个对应宝箱
    /// </summary>
    /// <param name="id"></param>
    public void ConsumeTreasureChest(int id)
    {
        GoodsData goodsData = new GoodsData();
        int key = 0;
        int index = 0;
        foreach (var item in _treasureChestDic)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                if (item.Value[i]._id == id)
                {
                    key = item.Key;
                    index = i;
                    goodsData = item.Value[i];
                }
            }
        }
        if (goodsData != null)
        {
            goodsData._number = goodsData._number - 1;
            StaticData.UpdateWareHouseItems(goodsData._id, (int)goodsData._number);
            if (goodsData._number <= 0)
            {
                _treasureChestDic[key].RemoveAt(index);
                StaticData.UpdateWareHouseItems(goodsData._id, 0);
            }
            WarehouseTool.ChangeUnlockTreasureChestID(goodsData._id, 0);
        }
    }
    /// <summary>
    /// 开关宝箱遮罩按钮
    /// </summary>
    /// <param name="isOpen"></param>
    public void OpenTreasureChestMaskBtn(bool isOpen)
    {
        _treasureChestMaskBtn.gameObject.SetActive(isOpen);
    }
    #endregion
    /// <summary>
    /// 获取当前item展示数据
    /// </summary>
    /// <param name="listIndex"></param>
    /// <returns></returns>
    public GoodsData ItemDataShow(int listIndex)
    {
        if (listIndex < _currGoodsDatas.Count)
        {
            return _currGoodsDatas[listIndex];
        }
        return null;
    }
    /// <summary>
    /// 获取当前item数据
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public GoodsData GetCurrItmeData()
    {
        return _currClickItme._Data;
    }
    /// <summary>
    /// 获取仓库容量
    /// </summary>
    /// <returns></returns>
    public string GetWareHouseMaxCount()
    {
        string str;
        int number = 0;
        for (int i = 0; i < _totalGiids.Count; i++)
        {
            if (_totalGiids[i]._number > 0)
            {
                number = number + 1;
            }
        }
        str = number + "/" + StaticData.playerInfoData.userInfo.WarehouseCount;
        return str;
    }
    /// <summary>
    /// 打开物品详情弹窗
    /// </summary>
    /// <param name="goodsData"></param>
    public async void OpenDetailsPopUp(GoodsData goodsData, int dataIndex, WarehouseItem currItem)
    {
        if (_detailsController == null)
        {
            await CreationDetailsPopUp();
        }
        _currClickItme = currItem;
        _detailsController.ShowData(goodsData);
    }
    /// <summary>
    /// 生详情成弹窗
    /// </summary>
    async Task CreationDetailsPopUp()
    {
        GameObject popUpObj = await ABManager.GetAssetAsync<GameObject>("WarehouseDetails");
        _detailsController = GameObject.Instantiate(popUpObj, transform).GetComponent<WarehouseDetailsController>();
        _detailsController.Initial();
    }
    /// <summary>
    /// 打开卖出弹窗
    /// </summary>
    public async void OpenSalePopUp()
    {
        if (_saleController == null)
        {
            await CreationalePopUp();
        }
        _saleController.ShowData(_currClickItme._Data);
    }
    /// <summary>
    /// 生成卖出弹窗
    /// </summary>
    async Task CreationalePopUp()
    {
        GameObject popUpObj = await ABManager.GetAssetAsync<GameObject>("WarehouseSale");
        _saleController = GameObject.Instantiate(popUpObj, transform).GetComponent<WarehouseSaleController>();
        _saleController.Initial();
    }
    /// <summary>
    /// 打开物品使用弹窗
    /// </summary>
    /// <param name="goodsData"></param>
    public async void OpenWarehouseUse(GoodsData goodsData)
    {
        if (_warehouseUseController == null)
        {
            await CreationaWarehouseUse();
        }
        _warehouseUseController.ShowData(goodsData);
    }
    /// <summary>
    /// 生成物品使用弹窗
    /// </summary>
    async Task CreationaWarehouseUse()
    {
        GameObject popUpObj = await ABManager.GetAssetAsync<GameObject>("WarehouseUse");
        _warehouseUseController = GameObject.Instantiate(popUpObj, transform).GetComponent<WarehouseUseController>();
        _warehouseUseController.Initial();
    }
    /// <summary>
    /// 刷新当前本地锁定数据
    /// </summary>
    /// <param name="goodsData"></param>
    /// <param name="islock"></param>
    void RefreshCurrDataLock(GoodsData goodsData, bool islock)
    {
        GoodsData currData = null;
        for (int i = 0; i < _currGoodsDatas.Count; i++)
        {
            if (_currGoodsDatas[i]._id == goodsData._id)
            {
                _currGoodsDatas[i]._isLock = islock;
            }
        }

        List<GoodsData> datas = null;
        //同步数据信息
        switch (_currWareHouseType)
        {
            case 0:
                datas = _seeds;
                break;
            case 1:
                datas = _fruits;
                break;
            case 2:
                datas = _gameItems;
                break;
            case 3:
                datas = _decorates;
                break;
        }
        if (datas != null)
        {
            for (int i = 0; i < datas.Count; i++)
            {
                if (datas[i]._id == goodsData._id)
                {
                    datas[i]._isLock = islock;
                }
            }
        }
    }
    /// <summary>
    /// 更新全部的锁定信息
    /// </summary>
    public void UpdateAllLock(Action action = null)
    {
        List<GoodsData> changeDatas = new List<GoodsData>();
        changeDatas.AddRange(GetChangeLockDatas(_seeds));
        changeDatas.AddRange(GetChangeLockDatas(_fruits));
        changeDatas.AddRange(GetChangeLockDatas(_gameItems));
        changeDatas.AddRange(GetChangeLockDatas(_decorates));
        WarehouseTool.UpdateAllLock(changeDatas, (isSucceed) =>
        {
            if (isSucceed)
            {
                action?.Invoke();
            }
        });
    }
    /// <summary>
    /// 获取锁定信息有变化的数据
    /// </summary>
    /// <param name="tageDatas"></param>
    /// <returns></returns>
    List<GoodsData> GetChangeLockDatas(List<GoodsData> tageDatas)
    {
        List<GoodsData> changeDatas = new List<GoodsData>();

        for (int i = 0; i < tageDatas.Count; i++)
        {
            GoodsData seedsData = tageDatas[i];
            if (_totaDataDic.ContainsKey(seedsData._id))
            {
                if (_totaDataDic[seedsData._id]._isLock != seedsData._isLock)
                {
                    changeDatas.Add(seedsData);
                }
            }
        }

        return changeDatas;
    }
    /// <summary>
    /// 更改列表
    /// </summary>
    /// <param name="coutn"></param>
    void ChangeList(int coutn)
    {
        _loopScrollRect.ClearCells();
        _loopScrollRect.totalCount = coutn;
        //添加循环列表itme引用丢失防误
        if (_loopScrollRect.prefabSource.prefabGO == null)
        {
            _loopScrollRect.prefabSource.prefabGO = _listItem.gameObject;
        }
        _loopScrollRect.RefillCells();
    }
    /// <summary>
    /// 选项按钮排序
    /// </summary>
    /// <param name="index"></param>
    public void SelectBtnRank(int index)
    {

        RectTransform tage = _selectBtnList[index]._ThisRect;
        if (!tage.GetComponent<Toggle>().isOn)
        {
            tage.GetComponent<Toggle>().isOn = true;
        }
        //tage.SetSiblingIndex(_selectBtnList.Count - 1);
        //List<RectTransform> surplusItem = new List<RectTransform>();
        //for (int i = _selectBtnList.Count - 1; i > 0; i--)
        //{
        //    if (i != index)
        //    {
        //        surplusItem.Add(_selectBtnList[i]._ThisRect);
        //    }
        //}

        //for (int i = 0; i < surplusItem.Count; i++)
        //{
        //    surplusItem[i].SetSiblingIndex(i);
        //}
        RefreshData(index);
        SetRedDot();
        //RefreshUI();

    }
    /// <summary>
    /// 设置红点初始状态
    /// </summary>
    public void SetRedDot()
    {
        for (int i = 0; i < _selectBtnList.Count; i++)
        {

            WarehouseSelectItemController topBtnItem = _selectBtnList[i];

            List<GoodsData> goodsDatas = new List<GoodsData>();
            bool isOpenRedDot = false;
            // 0:种子 1：果实 2：道具 3：装饰
            switch (topBtnItem._WarehouseIndx)
            {
                case 0:
                    isOpenRedDot = IsHaveNewData(_seeds);
                    break;
                case 1:
                    isOpenRedDot = IsHaveNewData(_fruits);
                    break;
                case 2:
                    isOpenRedDot = IsHaveNewData(_gameItems);
                    if (!isOpenRedDot)
                    {
                        isOpenRedDot = IsHaveNewTreasureChaests();
                    }
                    if (!isOpenRedDot)
                    {
                        isOpenRedDot = IsHaveUnlockTreasureChaests();
                    }
                    break;
                case 3:
                    isOpenRedDot = IsHaveNewData(_decorates);
                    break;
            }
            topBtnItem.OpenRedDot(isOpenRedDot);
        }
    }
    /// <summary>
    /// 判断是否有新道具
    /// </summary>
    /// <param name="goodsDatas"></param>
    /// <returns></returns>
    public bool IsHaveNewData(List<GoodsData> goodsDatas)
    {
        bool isNewData = false;
        for (int i = 0; i < goodsDatas.Count; i++)
        {
            if (!WarehouseTool.DataIsSave(goodsDatas[i]))
            {
                isNewData = true;
                return isNewData;
            }
        }
        return isNewData;
    }
    /// <summary>
    /// 是否有解锁了的宝箱
    /// </summary>
    /// <returns></returns>
    public bool IsHaveUnlockTreasureChaests()
    {

        bool isHave = false;
        foreach (var item in _treasureChestDic)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                if (WarehouseTool.IsThereAnUnlock(item.Value[i]._id))
                {
                    isHave = true;
                    return isHave;
                }
            }
        }
        return isHave;
    }
    /// <summary>
    /// 是否有新宝箱
    /// </summary>
    /// <returns></returns>
    public bool IsHaveNewTreasureChaests()
    {

        bool isHave = false;
        foreach (var item in _treasureChestDic)
        {
            if (item.Value != null && item.Value.Count > 0)
            {
                int oldNumber = WarehouseTool.GetTreasureChaestsValue(item.Key);
                if (oldNumber < item.Value.Count)
                {
                    isHave = true;
                    return isHave;
                }
            }
        }
        return isHave;
    }
    /// <summary>
    /// 批量更新本地新物品标记
    /// </summary>
    /// <param name="goodsDatas"></param>
    public void UpdateItemNewTage(int type)
    {
        List<GoodsData> goodsDatas = new List<GoodsData>();
        // 0:种子 1：果实 2：道具 3：装饰
        switch (type)
        {
            case 0:
                goodsDatas.AddRange(_seeds);
                break;
            case 1:
                goodsDatas.AddRange(_fruits);
                break;
            case 2:
                goodsDatas.AddRange(_gameItems);
                break;
            case 3:
                goodsDatas.AddRange(_decorates);
                break;
        }

        for (int i = 0; i < goodsDatas.Count; i++)
        {
            WarehouseTool.SetGoodsDataNew(goodsDatas[i]);
        }

    }
    /// <summary>
    /// 刷新ui位置
    /// </summary>
    void RefreshUI()
    {
        float parentRadius = _selectBoxRect.rect.width / 2;
        float originXCoord = _selectBoxRect.localPosition.x - parentRadius + _selectBtnLeftPadding;
        for (int i = 0; i < _selectBtnList.Count; i++)
        {
            float width = _selectBtnList[i]._ThisRect.rect.width;
            float radius = width / 2;
            float newXCoord = originXCoord + radius;
            Vector2 newCoord = new Vector2(newXCoord, _selectBtnList[i]._ThisRect.localPosition.y);
            _selectBtnList[i]._ThisRect.localPosition = newCoord;
            originXCoord = originXCoord + width - _selectBtnInterval;
        }
    }
    /// <summary>
    /// 刷新数据
    /// </summary>
    /// <param name="selectType"></param>
    public void RefreshData(int selectType)
    {
        _warehouseType = selectType;
        _showCapacityText.text = GetWareHouseMaxCount();
        int dataCount = ChangeCurrShowGoodsData(selectType);
        //修改仓库样式
        ChangeWarehouseStyle(selectType);

        _showPriceText.text = WarehouseTool.GetCurrWareHouseTotalPrices(_currGoodsDatas);
        ChangeList(dataCount);
        SetRedDot();
    }
    /// <summary>
    /// 修改仓库样式
    /// </summary>
    /// <param name="selectType"></param>
    public void ChangeWarehouseStyle(int selectType)
    {
        Transform showCapacityTextParent = _showCapacityText.transform.parent;
        Transform piceTra = _showPriceText.transform.parent;
        // 0:种子 1：果实 2：道具 3：装饰
        switch (selectType)
        {
            case 0:
                _bgImage.sprite = _bgSprites[1];
                _treasureBoxTra.gameObject.SetActive(false);
                _itemBoxRect.sizeDelta = new Vector2(_itemBoxRect.sizeDelta.x, 1722f);
                _itemBoxRect.localPosition = new Vector3(0, -11.9f);
                showCapacityTextParent.localPosition = new Vector3(452, -930);
                _lineRect.localPosition = new Vector3(0, -930);
                piceTra.localPosition = new Vector3(0, -930);
                piceTra.gameObject.SetActive(true);
                _oneKeySaleBtn.gameObject.SetActive(false);
                _lineRect.gameObject.SetActive(true);
                break;
            case 1:
                _bgImage.sprite = _bgSprites[0];
                _treasureBoxTra.gameObject.SetActive(false);
                _itemBoxRect.sizeDelta = new Vector2(_itemBoxRect.sizeDelta.x, 1495);
                _itemBoxRect.localPosition = new Vector3(0, 101);
                showCapacityTextParent.localPosition = new Vector3(452, -697);
                piceTra.gameObject.SetActive(true);
                _lineRect.gameObject.SetActive(true);
                _lineRect.localPosition = new Vector3(0, -697);
                piceTra.localPosition = new Vector3(0, -697);
                _oneKeySaleBtn.gameObject.SetActive(true);
                if (GetYouCanSell().Count > 0)
                {
                    _oneKeySaleBtn.enabled = true;
                    _oneKeySaleBtn.transform.GetComponent<Image>().sprite = _onKeyBtnSprites[0];
                }
                else
                {
                    _oneKeySaleBtn.enabled = false;
                    _oneKeySaleBtn.transform.GetComponent<Image>().sprite = _onKeyBtnSprites[1];
                }
                break;
            case 2:
                _bgImage.sprite = _bgSprites[1];
                _treasureBoxTra.gameObject.SetActive(true);
                _itemBoxRect.sizeDelta = new Vector2(_itemBoxRect.sizeDelta.x, 1342.285f);
                _itemBoxRect.localPosition = new Vector3(0, -203.8f);
                showCapacityTextParent.localPosition = new Vector3(0, -930);
                RefreshTreasureChest();
                piceTra.gameObject.SetActive(false);
                _lineRect.gameObject.SetActive(false);
                _oneKeySaleBtn.gameObject.SetActive(false);
                break;
            case 3:
                _bgImage.sprite = _bgSprites[1];
                _treasureBoxTra.gameObject.SetActive(false);
                _itemBoxRect.sizeDelta = new Vector2(_itemBoxRect.sizeDelta.x, 1736.409f);
                _itemBoxRect.localPosition = new Vector3(0, -19);
                showCapacityTextParent.localPosition = new Vector3(0, -930);
                piceTra.gameObject.SetActive(false);
                _oneKeySaleBtn.gameObject.SetActive(false);
                _lineRect.gameObject.SetActive(false);
                break;
        }
    }
    /// <summary>
    /// 刷新数据
    /// </summary>
    /// <param name="selectType"></param>
    public void RefreshData()
    {
        RefreshData(_currWareHouseType);
    }
    /// <summary>
    /// 刷新数据
    /// </summary>
    /// <param name="selectType"></param>
    public void RefreshNewData()
    {
        //获取仓库数据
        WarehouseTool.ParseData(StaticData.playerInfoData.userInfo.WareHoseInfo, AcceptCurrTotalData);
    }
    /// <summary>
    /// 卖出成功后刷新数据
    /// </summary>
    /// <param name="number"></param>
    public void SaleRefreshData(int number)
    {
        _detailsController.Close();
        //获取当前点击数据
        GoodsData goodsData = GetCurrItmeData();
        goodsData._number = goodsData._number - number;
        if (goodsData._number <= 0)
        {
            RemoveData(_currWareHouseType, _currClickItme._Data);
        }
        RefreshData(_currWareHouseType);
    }
    /// <summary>
    /// 删除对应数据
    /// </summary>
    /// <param name="type"></param>
    /// <param name="index"></param>
    void RemoveData(int type, GoodsData data)
    {
        _currGoodsDatas.Remove(_currClickItme._Data);
        //同步数据信息
        switch (_currWareHouseType)
        {
            case 0:
                _seeds.Remove(data);
                break;
            case 1:
                _fruits.Remove(data);
                break;
            case 2:
                _gameItems.Remove(data);
                break;
            case 3:
                _decorates.Remove(data);
                break;
        }

    }
    /// <summary>
    /// 删除对应商品id的物品
    /// </summary>
    /// <param name="ids"></param>
    void RemoveData(int type, int id)
    {
        GoodsData tageData = null;
        for (int i = 0; i < _currGoodsDatas.Count; i++)
        {
            if (_currGoodsDatas[i]._id == id)
            {
                tageData = _currGoodsDatas[i];
                tageData._number = 0;
            }
        }
        if (tageData != null)
        {
            _currGoodsDatas.Remove(tageData);
            switch (type)
            {
                case 0:
                    _seeds.Remove(tageData);
                    break;
                case 1:
                    _fruits.Remove(tageData);
                    break;
                case 2:
                    _gameItems.Remove(tageData);
                    break;
                case 3:
                    _decorates.Remove(tageData);
                    break;
            }
        }
    }
    /// <summary>
    /// 一键卖出
    /// </summary>
    public void OneKeySale()
    {
        //打开提示弹窗
        StaticData.OpenCommonTips(StaticData.GetMultilingual(120236), 120016, AffirmOneKeySale, CancelOneKeySale, 120075);
    }
    /// <summary>
    /// 确认一键卖出
    /// </summary>
    public void AffirmOneKeySale()
    {
        //筛选出没有锁定的数据
        List<GoodsData> sales = GetYouCanSell();
        if (sales != null && sales.Count != 0)
        {
            WarehouseTool.OnSale(sales, AKeySelling);
        }
    }
    /// <summary>
    /// 获取可以卖出的商品
    /// </summary>
    /// <returns></returns>
    public List<GoodsData> GetYouCanSell()
    {

        //筛选出没有锁定的数据
        List<GoodsData> sales = new List<GoodsData>();
        for (int i = 0; i < _currGoodsDatas.Count; i++)
        {
            //没有锁定才能卖
            if (!_currGoodsDatas[i]._isLock && WarehouseTool.IsGoodsSell(_currGoodsDatas[i]))
            {
                sales.Add(_currGoodsDatas[i]);
            }
        }
        //获取锁定信息有变化的数据
        List<GoodsData> lockDatas = new List<GoodsData>();
        for (int i = 0; i < _currGoodsDatas.Count; i++)
        {
            GoodsData data = _currGoodsDatas[i];
            //判断是否是有效数据
            if (_totaDataDic.ContainsKey(data._id))
            {
                //判断锁定信息是否有变化
                if (_totaDataDic[data._id]._isLock != data._isLock)
                {
                    lockDatas.Add(data);
                }
            }
        }

        return sales;
    }
    /// <summary>
    /// 取消一键卖出
    /// </summary>
    public void CancelOneKeySale()
    {

        Debug.Log("一键卖出取消");
    }
    /// <summary>
    /// 一键卖出回调
    /// </summary>哦
    /// <param name="isSucceed"></param>
    public void AKeySelling(bool isSucceed, List<int> saleIds)
    {
        if (isSucceed)
        {
            for (int i = 0; i < saleIds.Count; i++)
            {
                RemoveData(_currWareHouseType, saleIds[i]);
            }
            RefreshData(_currWareHouseType);
        }
    }
    /// <summary>
    /// 关闭所有宝箱计时器
    /// </summary>
    public void CloseAllTime()
    {
        foreach (var item in _warehouseTreasureChestDic)
        {
            item.Value.CloseTime();
        }
    }
    /// <summary>
    /// 关闭仓库
    /// </summary>
    public void Close()
    {
        UniversalTool.CancelUIAnimTwo(transform.Find("Box").GetComponent<CanvasGroup>(), transform.Find("Box"), () =>
        {
            UpdateAllLock();
            OpenTreasureChestMaskBtn(false);
            CloseAllTime();
            UIComponent.HideUI(UIType.Warehouse);
            RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Warehouse);
        });

    }
    /// <summary>
    /// 关闭仓库
    /// </summary>
    /// <param name="action"></param>
    public void Remove()
    {
        UpdateAllLock();
        CloseAllTime();
        UIComponent.RemoveUI(UIType.Warehouse);
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Warehouse);
    }
    /// <summary>
    /// 设置多语言显示
    /// </summary>
    void SetPanelMultilingual()
    {
        _showCapacityText.transform.parent.Find("Title").GetComponent<Text>().text = StaticData.GetMultilingual(120092);
        _showPriceText.transform.parent.Find("Title").GetComponent<Text>().text = StaticData.GetMultilingual(120093);
        _oneKeySaleBtn.transform.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120094);
    }
    #endregion
}
