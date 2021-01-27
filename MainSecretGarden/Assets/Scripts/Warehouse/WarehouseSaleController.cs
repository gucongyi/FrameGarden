using Company.Cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 仓库卖出物品弹窗
/// 2020/8/5 huangjiangdong
/// </summary>
public class WarehouseSaleController : MonoBehaviour
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
    /// icon显示
    /// </summary>
    Image _iconImage;
    /// <summary>
    /// 总价
    /// </summary>
    Text _totalPrices;
    /// <summary>
    /// 取消
    /// </summary>
    Button _cancelBtn;
    /// <summary>
    /// 拥有
    /// </summary>
    Text _haveNumberText;
    /// <summary>
    /// 单价
    /// </summary>
    Text _priceText;
    /// <summary>
    /// 卖出
    /// </summary>
    Button _saleBtn;
    /// <summary>
    /// 标题box
    /// </summary>
    Transform _titleTra;
    /// <summary>
    /// 标题
    /// </summary>
    Text _titleText;
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
        _iconImage = transform.Find("Details/IconBox/Icon").GetComponent<Image>();
        _totalPrices = transform.Find("Details/Total").GetComponent<Text>();
        _cancelBtn = transform.Find("Details/BottomBtnBox/CancelBtn").GetComponent<Button>();
        _saleBtn = transform.Find("Details/BottomBtnBox/SaleBtn").GetComponent<Button>();
        _haveNumberText= transform.Find("Details/HaveNumber/Text").GetComponent<Text>();
        _priceText = transform.Find("Details/Price/Text").GetComponent<Text>();

        _titleTra= transform.Find("Details/Title");
        _titleText = _titleTra.Find("Text").GetComponent<Text>();
        _cancelBtn.onClick.RemoveAllListeners();
        _cancelBtn.onClick.AddListener(Close);
        _maskBtn.onClick.RemoveAllListeners();
        _maskBtn.onClick.AddListener(Close);
        _uISetAmountComponent._minValue = 1;
        _uISetAmountComponent.act = Sale;
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
        _uISetAmountComponent._maxValue = (int)_data._number;
        _iconImage.sprite = null;
        _iconImage.sprite = await ABManager.GetAssetAsync<Sprite>(_data._data.Icon);
        //_iconImage.SetNativeSize();

         _haveNumberText.text = _data._number.ToString(); ;
         _priceText.text=data._data.PriceSell[0].Price.ToString();
        _uISetAmountComponent.inputFiled.text = "1";
        DealClass dealClass = null;
        for (int i = 0; i < _data._data.PriceSell.Count; i++)
        {
            if (_data._data.PriceSell[i].IdGameItem == StaticData.configExcel.GetVertical().GoldGoodsId)
            {
                dealClass = _data._data.PriceSell[i];
            }
        }
        if (dealClass != null)
        {
            int price = dealClass.Price;
            _totalPrices.text = price.ToString();
        }
        else
        {
            _totalPrices.text = "0";
        }
        gameObject.SetActive(true);

    }
    /// <summary>
    /// 确认卖出
    /// </summary>
    /// <param name="number"></param>
    void Sale(int number)
    {
        _currNumber = number;
        WarehouseTool.OnSale(WarehouseController.Instance.GetCurrItmeData(), number, SaleResult);
    }
    /// <summary>
    /// 卖出成功回调
    /// </summary>
    /// <param name="isSucceed"></param>
    public void SaleResult(bool isSucceed)
    {
        if (isSucceed)
        {
            WarehouseController.Instance.SaleRefreshData(_currNumber);
            StaticData.CreateToastTips(StaticData.GetMultilingual(120239));
            Close();
            
        }
        else
        {
            StaticData.CreateToastTips(StaticData.GetMultilingual(120240));
        }
    }
    /// <summary>
    /// 个数变化
    /// </summary>
    /// <param name="number"></param>
    void Change(int number)
    {

        DealClass dealClass = null;

        for (int i = 0; i < _data._data.PriceSell.Count; i++)
        {
            if (_data._data.PriceSell[i].IdGameItem == StaticData.configExcel.GetVertical().GoldGoodsId)
            {
                dealClass = _data._data.PriceSell[i];
            }
        }
        if (dealClass != null)
        {
            int price = dealClass.Price * number;
            _totalPrices.text = price.ToString();
        }
    }
    /// <summary>
    /// 关闭
    /// </summary>
    private void Close()
    {
        _data = null;
        _iconImage.sprite = null;
        _nameText.text = "";
        _totalPrices.text = "";
        _uISetAmountComponent._minValue = 0;

        gameObject.SetActive(false);
    }
    void SetPanelMultilingual()
    {
        _cancelBtn.transform.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120075);
        _saleBtn.transform.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120089);
        _haveNumberText.transform.parent.Find("Title").GetComponent<Text>().text = StaticData.GetMultilingual(120226);
        _priceText.transform.parent.Find("Title").GetComponent<Text>().text = StaticData.GetMultilingual(120227);
        _titleText.text= StaticData.GetMultilingual(120283);
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
