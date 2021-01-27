using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 仓库物品详情弹窗
/// 2020/8/4 huangjiangdong
/// </summary>
public class WarehouseDetailsController : MonoBehaviour
{
    #region 字段
    /// <summary>
    /// 遮罩按钮
    /// </summary>
    Button _maskBtn;
    /// <summary>
    /// 内容box
    /// </summary>
    Transform _detailsTra;
    /// <summary>
    /// 内容BoxImage
    /// </summary>
    Image _detailsImage;
    /// <summary>
    /// 名字显示text
    /// </summary>
    Text _nameText;
    /// <summary>
    /// Icon显示
    /// </summary>
    Image _iconImage;
    /// <summary>
    /// 珍稀标签
    /// </summary>
    Image _valuableTableTra;
    /// <summary>
    /// 等级显示
    /// </summary>
    Text _gradeText;
    /// <summary>
    /// 果实信息box
    /// </summary>
    Transform _fruitInformationBoxTra;
    /// <summary>
    /// 拥有数量
    /// </summary>
    Text _haveNumber;
    /// <summary>
    /// 数量显示
    /// </summary>
    Text _numberText;
    /// <summary>
    /// 收获经验显示
    /// </summary>
    Text _reapExperienceText;
    /// <summary>
    /// 成熟时间显示
    /// </summary>
    Text _matureTimeText;
    /// <summary>
    /// 果实单价
    /// </summary>
    Text _priceText;
    /// <summary>
    /// 描述显示
    /// </summary>
    Text _desText;
    /// <summary>
    /// 底部按钮盒子
    /// </summary>
    Transform _bottomBtnBox;
    /// <summary>
    /// 卖出按钮
    /// </summary>
    Button _saleBtn;
    /// <summary>
    /// 锁定按钮
    /// </summary>
    Button _lockBtn;
    /// <summary>
    /// 取消锁定按钮
    /// </summary>
    Button _noLockBtn;
    /// <summary>
    /// 使用按钮
    /// </summary>
    Button _useBtn;
    /// <summary>
    /// 当前展示数据
    /// </summary>
    GoodsData _currData;
    /// <summary>
    /// 关闭按钮
    /// </summary>
    Button _closeBtn;
    /// <summary>
    /// 珍稀标签图片精灵
    /// </summary>
    [SerializeField]
    List<Sprite> _valuableTableSprites = new List<Sprite>();
    /// <summary>
    /// 是否初始化
    /// </summary>
    bool _isInitial = false;
    #endregion
    #region 函数
    /// <summary>
    /// 初始化
    /// </summary>
    public void Initial()
    {

        /// <summary>
        /// 遮罩按钮
        /// </summary>
        _maskBtn = transform.Find("Mask").GetComponent<Button>();
        /// <summary>
        /// 内容box
        /// </summary>
        _detailsTra = transform.Find("Details");
        _detailsImage = _detailsTra.GetComponent<Image>();
        /// <summary>
        /// 名字显示text
        /// </summary>
        _nameText = _detailsTra.Find("Name/Text").GetComponent<Text>();
        /// <summary>
        /// Icon显示
        /// </summary>
        _iconImage = _detailsTra.Find("IconBox/Mask/Icon").GetComponent<Image>();
        /// <summary>
        /// 珍稀标签
        /// </summary>
        _valuableTableTra = _detailsTra.Find("IconBox/Mask").GetComponent<Image>();
        /// <summary>
        /// 等级显示
        /// </summary>
        _gradeText = _detailsTra.Find("GradeBg/Grade/Text").GetComponent<Text>();
        _fruitInformationBoxTra = _detailsTra.Find("FruitInformationBox");
        /// <summary>
        /// 数量显示
        /// </summary>
        _numberText = _fruitInformationBoxTra.Find("NumberBox/Number").GetComponent<Text>();
        _reapExperienceText = _fruitInformationBoxTra.Find("ReapExperience/Number").GetComponent<Text>();
        _matureTimeText = _fruitInformationBoxTra.Find("MatureTime/Number").GetComponent<Text>();
        _priceText = _fruitInformationBoxTra.Find("Price/Number").GetComponent<Text>();
        _haveNumber = _detailsTra.Find("GradeBg/HaveNumber/Text").GetComponent<Text>();
        /// <summary>
        /// 描述显示
        /// </summary>
        _desText = _detailsTra.Find("Des").GetComponent<Text>();
        //底部按钮
        _bottomBtnBox = _detailsTra.Find("BottomBtnBox");
        //卖出按钮
        _saleBtn = _bottomBtnBox.Find("SaleBtn").GetComponent<Button>();
        _saleBtn.onClick.RemoveAllListeners();
        _saleBtn.onClick.AddListener(OnClickSale);
        //锁定按钮
        _lockBtn = _bottomBtnBox.Find("LockBtn").GetComponent<Button>();
        _lockBtn.onClick.RemoveAllListeners();
        _lockBtn.onClick.AddListener(OnClickLock);
        //取消锁定
        _noLockBtn = _bottomBtnBox.Find("NoLockBtn").GetComponent<Button>();
        _noLockBtn.onClick.RemoveAllListeners();
        _noLockBtn.onClick.AddListener(OnClickNoLock);
        //使用按钮
        _useBtn = _bottomBtnBox.Find("UseBtn").GetComponent<Button>();
        _useBtn.onClick.RemoveAllListeners();
        _useBtn.onClick.AddListener(OnClickUse);
        //关闭按钮
        _closeBtn = _detailsTra.Find("CloseBtn").GetComponent<Button>();
        _closeBtn.onClick.RemoveAllListeners();
        _closeBtn.onClick.AddListener(Close);
        _maskBtn.onClick.RemoveAllListeners();
        _maskBtn.onClick.AddListener(Close);
        SetPanelMultilingual();
        _isInitial = true;
    }
    /// <summary>
    /// 关闭
    /// </summary>
    public void Close()
    {
        _currData = null;
        if (transform==null)
        {
            return;
        }
        if (_iconImage != null)
        {
            _iconImage.sprite = null;
        }
        if (_gradeText != null)
        {
            _gradeText.text = "";
        }
      
        if (_numberText != null)
        {
            _numberText.text = "";
        }
      
        if (_desText != null)
        {
            _desText.text = "";
        }
        if (gameObject != null)
        {
            gameObject.SetActive(false);
        }
        WarehouseController.Instance.RefreshData();
    }
    /// <summary>
    /// 展示数据
    /// </summary>
    /// <param name="goodsData"></param>
    public async void ShowData(GoodsData goodsData)
    {
        if (!_isInitial)
        {
            Initial();
        }
        _currData = goodsData;
        _nameText.text = StaticData.GetMultilingual(_currData._data.ItemName);
        RectTransform desRect = _desText.transform.GetComponent<RectTransform>();
        if (_currData._data.ItemType == Company.Cfg.TypeGameItem.Seed)
        {
            /// 收获经验显示
            _reapExperienceText.text = StaticData.GetSeedExperience(_currData._data.ID).ToString();

            /// 成熟时间显示
            _matureTimeText.text = StaticData.GetSeedRipeningTime(_currData._data.ID);
            ///获取结果数量
            _numberText.text = StaticData.GetSeedFruitNumber(_currData._data.ID).ToString();
            //获取果实单价
            _priceText.text = StaticData.GetSeedEstimateTheValueOf(_currData._data.ID).ToString();
            _fruitInformationBoxTra.gameObject.SetActive(true);
            desRect.sizeDelta = new Vector2(desRect.sizeDelta.x, 169.2016f);
            desRect.localPosition = new Vector3(desRect.localPosition.x, -239.3992f);

        }
        else
        {

            _fruitInformationBoxTra.gameObject.SetActive(false);
            desRect.sizeDelta = new Vector2(desRect.sizeDelta.x, 264.9064f);
            desRect.localPosition = new Vector3(desRect.localPosition.x, 14.2f);
        }

        Sprite iconSprite = null;
        try
        {
            iconSprite = await ABManager.GetAssetAsync<Sprite>(_currData._data.Icon);
        }
        catch (Exception er)
        {
            Debug.Log("获取道具icon失败");
        }
        _iconImage.sprite = iconSprite;

        switch (_currData._data.Rarity)
        {
            case Company.Cfg.TypeRarity.None:
                _valuableTableTra.sprite = _valuableTableSprites[0];
                break;
            case Company.Cfg.TypeRarity.Primary:
                _valuableTableTra.sprite = _valuableTableSprites[0];
                break;
            case Company.Cfg.TypeRarity.Intermediate:
                _valuableTableTra.sprite = _valuableTableSprites[1];
                break;
            case Company.Cfg.TypeRarity.Senior:
                _valuableTableTra.sprite = _valuableTableSprites[2];
                break;
        }
        if (_currData._data.Grade > 0)
        {
            _gradeText.text = _currData._data.Grade.ToString();
            _gradeText.transform.parent.gameObject.SetActive(true);
        }
        else
        {
            _gradeText.transform.parent.gameObject.SetActive(false);
        }

        _haveNumber.text = _currData._number.ToString();

        _desText.text = StaticData.GetMultilingual(_currData._data.Description);
        bool isSell = WarehouseTool.IsGoodsSell(_currData);

        _saleBtn.gameObject.SetActive(isSell);
        if (isSell)
        {
            _lockBtn.gameObject.SetActive(!_currData._isLock);
            _noLockBtn.gameObject.SetActive(_currData._isLock);
        }
        else
        {
            _lockBtn.gameObject.SetActive(false);
            _noLockBtn.gameObject.SetActive(false);
        }

        _useBtn.gameObject.SetActive(_currData._data.Use != 0);
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 使用
    /// </summary>
    private async void OnClickUse()
    {

        //判断使用类型
        switch (_currData._data.Use)
        {
            case Company.Cfg.GoodsUseType.None:
                Debug.Log("没有使用类型");
                break;
            case Company.Cfg.GoodsUseType.StarNote:
                Debug.Log("星币");

                await StaticData.JumpToSceneState(WarehouseController.Instance.Remove, StaticData.SceneState.ManorSelf, () => { Debug.Log("跳转章节番外结束"); });
                break;
            case Company.Cfg.GoodsUseType.Manure:

                await StaticData.JumpToSceneState(WarehouseController.Instance.Remove, StaticData.SceneState.ManorSelf, () => { Debug.Log("化肥跳转庄园结束"); });
                break;
            case Company.Cfg.GoodsUseType.ZillionaireTessera:
                //功能是否开启
                if (!StaticData.IsOpenFunction(10011))
                {
                    return;
                }
                await StaticData.JumpToSceneState(WarehouseController.Instance.Remove, StaticData.SceneState.RichMan, () => { Debug.Log("跳转大富翁结束"); });
                break;
            case Company.Cfg.GoodsUseType.ExtendBag:
                WarehouseController.Instance.OpenWarehouseUse(_currData);
                Close();
                break;
            case Company.Cfg.GoodsUseType.GiftBox:
                WarehouseController.Instance.OpenWarehouseUse(_currData);
                Close();
                break;
            case Company.Cfg.GoodsUseType.Seed:
                await StaticData.JumpToSceneState(WarehouseController.Instance.Remove, StaticData.SceneState.ManorSelf, () => { Debug.Log("种子跳转庄园结束"); });
                break;
        }
      
    }
    /// <summary>
    /// 取消锁定
    /// </summary>
    private void OnClickNoLock()
    {
        _noLockBtn.gameObject.SetActive(false);
        _lockBtn.gameObject.SetActive(true);
        _currData._isLock = false;
        WarehouseController.Instance.RefreshData();
    }
    /// <summary>
    /// 锁定
    /// </summary>
    private void OnClickLock()
    {
        _lockBtn.gameObject.SetActive(false);
        _noLockBtn.gameObject.SetActive(true);
        _currData._isLock = true;
        WarehouseController.Instance.RefreshData();
    }
    /// <summary>
    /// 卖出
    /// </summary>
    private void OnClickSale()
    {
        WarehouseController.Instance.OpenSalePopUp();
        Close();
    }
    /// <summary>
    /// 设置多语言
    /// </summary>
    void SetPanelMultilingual()
    {
        _detailsTra.Find("FruitInformationBox/NumberBox/Text").GetComponent<Text>().text = StaticData.GetMultilingual(120228);
        _detailsTra.Find("FruitInformationBox/ReapExperience/Text").GetComponent<Text>().text = StaticData.GetMultilingual(120229);
        _detailsTra.Find("FruitInformationBox/MatureTime/Text").GetComponent<Text>().text = StaticData.GetMultilingual(120230);
        _detailsTra.Find("FruitInformationBox/Price/Text").GetComponent<Text>().text = StaticData.GetMultilingual(120231);
        _detailsTra.Find("GradeBg/HaveNumber/Title").GetComponent<Text>().text = StaticData.GetMultilingual(120226);
        _detailsTra.Find("GradeBg/Grade/Title").GetComponent<Text>().text = StaticData.GetMultilingual(120035);
        _lockBtn.transform.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120087);
        _noLockBtn.transform.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120088);
        _saleBtn.transform.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120089);
        _useBtn.transform.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120090);
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
