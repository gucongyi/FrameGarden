using Company.Cfg;
using Game.Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 道具使用弹窗
/// 2020/9/24 huangjiangdong
/// </summary>
public class WarehouseUseController : MonoBehaviour
{
    #region 字段

    /// <summary>
    /// 输入框组件
    /// </summary>
    UISetAmountComponent _uISetAmountComponent;
    /// <summary>
    /// 遮罩按钮
    /// </summary>
    Button _maskBtn;
    /// <summary>
    /// 名字显示
    /// </summary>
    Text _nameText;
    /// <summary>
    /// 拥有
    /// </summary>
    Text _haveNumberText;
    /// <summary>
    /// 使用效果
    /// </summary>
    Text _effectText;
    /// <summary>
    /// icon显示
    /// </summary>
    Image _iconImage;
    /// <summary>
    /// 使用
    /// </summary>
    Button _useBtn;
    /// <summary>
    /// 取消
    /// </summary>
    Button _cancelBtn;
    /// <summary>
    /// 当前数据
    /// </summary>
    GoodsData _data;
    /// <summary>
    /// 当前卖出数量
    /// </summary>
    int _currNumber;
    /// <summary>
    /// 初始化
    /// </summary>
    bool _isInitial = false;
    #endregion
    #region 函数
    /// <summary>
    /// 初始化组件
    /// </summary>
    public void Initial()
    {
        _uISetAmountComponent = transform.Find("Details/UISetAmount").GetComponent<UISetAmountComponent>();
        _maskBtn = transform.Find("Mask").GetComponent<Button>();
        _nameText = transform.Find("Details/Name/Text").GetComponent<Text>();
        _haveNumberText = transform.Find("Details/HaveNumber/Text").GetComponent<Text>();
        _effectText = transform.Find("Details/Price/Text").GetComponent<Text>();
        _iconImage = transform.Find("Details/IconBox/Icon").GetComponent<Image>();
        _useBtn = transform.Find("Details/BottomBtnBox/SaleBtn").GetComponent<Button>();
        _cancelBtn = transform.Find("Details/BottomBtnBox/CancelBtn").GetComponent<Button>();

        _maskBtn.onClick.RemoveAllListeners();
        _maskBtn.onClick.AddListener(Close);
        _cancelBtn.onClick.RemoveAllListeners();
        _cancelBtn.onClick.AddListener(Close);

        _uISetAmountComponent._minValue = 1;
        _uISetAmountComponent.act = Use;
        _uISetAmountComponent._changeAction = Change;
        SetPanelMultilingual();
    }
    /// <summary>
    /// 展示数据
    /// </summary>
    /// <param name="data"></param>
    public async void ShowData(GoodsData data)
    {
        if (_isInitial == false)
        {
            Initial();
        }
        _data = data;

        _nameText.text = StaticData.GetMultilingual(_data._data.ItemName);
        _haveNumberText.text = _data._number.ToString(); ;

        //设置输入框默认显示钥匙数量Placeholder
        _uISetAmountComponent.inputFiled.text = "1";
        _uISetAmountComponent.inputFiled.placeholder.GetComponent<Text>().text = "1";

        switch (_data._data.Use)
        {
            case Company.Cfg.GoodsUseType.ExtendBag:
                _effectText.transform.parent.Find("Title").GetComponent<Text>().text = StaticData.GetMultilingual(120232);

                ExtendBagIDCount extendBagIDCount = StaticData.configExcel.GetVertical().UserExtendBag;
                if (extendBagIDCount.GoodsID == _data._id)
                {
                    int count = extendBagIDCount.Count;
                    _effectText.text = count.ToString();
                }
                break;
            case Company.Cfg.GoodsUseType.GiftBox:
                PackageDefine packageDefine = null;
                _effectText.transform.parent.Find("Title").GetComponent<Text>().text = StaticData.GetMultilingual(120233);
                for (int i = 0; i < StaticData.configExcel.Package.Count; i++)
                {
                    PackageDefine PackageData = StaticData.configExcel.Package[i];
                    if (PackageData.BoxID == _data._data.ID)
                    {
                        packageDefine = PackageData;
                    }
                }
                int index = 0;
                if (packageDefine != null)
                {
                    for (int i = 0; i < packageDefine.BoxAward.Count; i++)
                    {
                        index = index + (int)packageDefine.BoxAward[i].Count;
                    }
                    _effectText.text = index.ToString();
                }
                break;
        }

        //设置最多能输入
        switch (data._data.Use)
        {

            case Company.Cfg.GoodsUseType.ExtendBag:
                int max = StaticData.configExcel.GetVertical().WarehouseTotal - StaticData.playerInfoData.userInfo.WarehouseCount;
                int number = StaticData.configExcel.GetVertical().UserExtendBag.Count;
                int index = max / number;
                if (index <= (int)data._number)
                {
                    _uISetAmountComponent._maxValue = index;
                }
                else
                {
                    _uISetAmountComponent._maxValue = (int)data._number;
                }
               
                if (max == 0)
                {
                    _uISetAmountComponent._defaultValue = "0";
                    _uISetAmountComponent._minValue = 0;
                    //设置输入框默认显示钥匙数量Placeholder
                    _uISetAmountComponent.inputFiled.text = "0";
                    _uISetAmountComponent.inputFiled.placeholder.GetComponent<Text>().text = "0";
                }
                break;
            default:
                _uISetAmountComponent._maxValue = (int)data._number;
                break;
        }
        Sprite iconSprite= null;
        try
        {
            iconSprite= await ABManager.GetAssetAsync<Sprite>(_data._data.Icon);
        }
        catch (System.Exception er)
        {
            Debug.Log("获取道具icon失败");
        }
        _iconImage.sprite = iconSprite;
        //_iconImage.SetNativeSize();


        gameObject.SetActive(true);

    }
    /// <summary>
    /// 确认使用
    /// </summary>
    /// <param name="number"></param>
    void Use(int number)
    {
        GoodsData goodsData = new GoodsData();
        goodsData._id = _data._id;
        goodsData._number = number;
        _currNumber = number;
        WarehouseTool.UseGoods(goodsData, UseResult);
    }
    /// <summary>
    /// 使用成功回调
    /// </summary>
    /// <param name="isSucceed"></param>
    public void UseResult(bool isSucceed, SCUseWarehouseGoods data, WebErrorCode code)
    {
        if (isSucceed)
        {
            switch (_data._data.Use)
            {
                case Company.Cfg.GoodsUseType.ExtendBag:
                    ExtendBagIDCount extendBagIDCount = StaticData.configExcel.GetVertical().UserExtendBag;
                    if (extendBagIDCount.GoodsID == _data._id)
                    {
                        int number = extendBagIDCount.Count;
                        number = number * _currNumber;
                        StaticData.playerInfoData.userInfo.WarehouseCount = StaticData.playerInfoData.userInfo.WarehouseCount + number;
                    }
                    Debug.Log("扩容使用完毕");
                    break;
                case Company.Cfg.GoodsUseType.GiftBox:

                    List<CSWareHouseStruct> newGoods = new List<CSWareHouseStruct>();
                    for (int i = 0; i < data.GetGoodsInfo.Count; i++)
                    {
                        SCBuyGoodsStruct sCBuyGoodsStruct = data.GetGoodsInfo[i];
                        //获取这次获得数量
                        int number = sCBuyGoodsStruct.Count;
                        number = number - StaticData.GetWareHouseItem(sCBuyGoodsStruct.GoodsId).GoodNum;
                        CSWareHouseStruct goodsData = new CSWareHouseStruct();
                        goodsData.GoodId = sCBuyGoodsStruct.GoodsId;
                        goodsData.GoodNum = number;
                        newGoods.Add(goodsData);
                        StaticData.UpdateWareHouseItems(sCBuyGoodsStruct.GoodsId, sCBuyGoodsStruct.Count);
                    }
                    StaticData.OpenReceiveAward(newGoods, _data._id);
                    Debug.Log("礼盒使用完毕");
                    break;
            }
            StaticData.UpdateWareHouseItem(_data._id, -_currNumber);
            Close();
            WarehouseController.Instance.RefreshNewData();
        }
        else
        {
            if (code == WebErrorCode.Good_Warehouse_Insufficient)
            {
                StaticData.CreateToastTips("仓库空间不足！领取礼盒失败！");
            }
            else
            {
                Debug.Log("礼盒使用失败");
            }
        }
    }

    /// <summary>
    /// 个数变化
    /// </summary>
    /// <param name="number"></param>
    void Change(int number)
    {
        int numberValue = 0;
        switch (_data._data.Use)
        {
            case Company.Cfg.GoodsUseType.ExtendBag:
                ExtendBagIDCount extendBagIDCount = StaticData.configExcel.GetVertical().UserExtendBag;
                if (extendBagIDCount.GoodsID == _data._id)
                {
                    int count = extendBagIDCount.Count;
                    numberValue = number * count;
                }
                break;
            case Company.Cfg.GoodsUseType.GiftBox:
                PackageDefine packageDefine = null;

                for (int i = 0; i < StaticData.configExcel.Package.Count; i++)
                {
                    PackageDefine data = StaticData.configExcel.Package[i];
                    if (data.BoxID == _data._data.ID)
                    {
                        packageDefine = data;
                    }
                }
                int index = 0;
                if (packageDefine != null)
                {
                    for (int i = 0; i < packageDefine.BoxAward.Count; i++)
                    {
                        index = index + (int)packageDefine.BoxAward[i].Count;
                    }
                }
                numberValue = index * number;
                break;
        }

        _effectText.text = numberValue.ToString();
    }

    /// <summary>
    /// 关闭
    /// </summary>
    private void Close()
    {
        _data = null;
        _iconImage.sprite = null;
        _uISetAmountComponent._minValue = 0;
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 设置多语言
    /// </summary>
    public void SetPanelMultilingual()
    {
       
        _cancelBtn.transform.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120075);
        _useBtn.transform.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120090);
        _haveNumberText.transform.parent.Find("Title").GetComponent<Text>().text = StaticData.GetMultilingual(120226);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion
}
