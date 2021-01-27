using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
/// <summary>
/// 仓库物品item
/// 2020/7/31 huangjiangdong
/// </summary>
public class WarehouseItem : MonoBehaviour, InterfaceScrollCell
{
    #region 字段
    /// <summary>
    /// 物品icon
    /// </summary>
    Image _iconImage;
    /// <summary>
    /// 上新标记
    /// </summary>
    Transform _newLabelTra;
    /// <summary>
    /// 稀有度标记
    /// </summary>
    Transform _rarityLabelTra;
    /// <summary>
    /// 锁
    /// </summary>
    Transform _lockLabelTra;
    /// <summary>
    /// 等级
    /// </summary>
    Transform _gradeLabelTra;
    /// <summary>
    /// 等级显示text
    /// </summary>
    Text _gradeLabelText;
    /// <summary>
    /// 数量显示text
    /// </summary>
    Text _numberText;
    /// <summary>
    /// 名字显示Text
    /// </summary>
    Text _nameText;
    /// <summary>
    /// 当前展示物品数据
    /// </summary>
    GoodsData _currShowData;
    /// <summary>
    /// 当前数据下标
    /// </summary>
    int _currDataIndex;
    /// <summary>
    /// 点击按钮
    /// </summary>
    Button _clickBtn;
    /// <summary>
    /// 是否初始化
    /// </summary>
    bool _isInitial = false;
    #endregion
    #region 属性
    public GoodsData _Data { get { return _currShowData; } }
    #endregion
    #region 函数


    private void Awake()
    {
        if (!_isInitial)
        {
            Initial();
        }

    }
    /// <summary>
    /// 初始化
    /// </summary>
    void Initial()
    {
        _iconImage = transform.Find("IconBox/Icon").GetComponent<Image>();
        _newLabelTra = transform.Find("NewLabel");
        _rarityLabelTra = transform.Find("RarityLabel");
        _lockLabelTra = transform.Find("LockLabel");
        _gradeLabelTra = transform.Find("GradeLabel");
        _gradeLabelText = _gradeLabelTra.Find("GradeValueText").GetComponent<Text>();
        _numberText = transform.Find("NumberText").GetComponent<Text>();
        _nameText = transform.Find("NameText").GetComponent<Text>();
        _clickBtn = transform.GetComponent<Button>();
        _clickBtn.onClick.RemoveAllListeners();
        _clickBtn.onClick.AddListener(OnClick);
        _isInitial = true;
    }
    /// <summary>
    /// 设置item数据
    /// </summary>
    /// <param name="idx"></param>
    public void ScrollCellIndex(int idx)
    {
        GetData(idx);
    }
    /// <summary>
    /// 获取对应数据
    /// </summary>
    /// <param name="index"></param>
    void GetData(int index)
    {
        _currShowData = WarehouseController.Instance.ItemDataShow(index);
        if (_currShowData != null)
        {
            _currDataIndex = index;
            ShwoData();
        }
    }
    /// <summary>
    /// 展示数据
    /// </summary>
    async void ShwoData()
    {
        if (_currShowData == null)
        {
            return;
        }

        _newLabelTra.gameObject.SetActive(!WarehouseTool.DataIsSave(_currShowData));
        //_newLabelTra.gameObject.SetActive(false);//_newLabelTra.gameObject.SetActive(_currShowData._isNewData);
        switch (_currShowData._data.Rarity)
        {
            case Company.Cfg.TypeRarity.None:
            case Company.Cfg.TypeRarity.Primary:
                _rarityLabelTra.gameObject.SetActive(true);
                _rarityLabelTra.Find("One").gameObject.SetActive(true);
                _rarityLabelTra.Find("Two").gameObject.SetActive(false);
                _rarityLabelTra.Find("Therr").gameObject.SetActive(false);
                break;
            case Company.Cfg.TypeRarity.Intermediate:
                _rarityLabelTra.gameObject.SetActive(true);
                _rarityLabelTra.Find("One").gameObject.SetActive(false);
                _rarityLabelTra.Find("Two").gameObject.SetActive(true);
                _rarityLabelTra.Find("Therr").gameObject.SetActive(false);
                break;
            case Company.Cfg.TypeRarity.Senior:
                _rarityLabelTra.gameObject.SetActive(true);
                _rarityLabelTra.Find("One").gameObject.SetActive(false);
                _rarityLabelTra.Find("Two").gameObject.SetActive(false);
                _rarityLabelTra.Find("Therr").gameObject.SetActive(true);
                break;
        }
        _lockLabelTra.gameObject.SetActive(_currShowData._isLock);

        if (_currShowData._data.Grade > 0)
        {
            _gradeLabelTra.gameObject.SetActive(true);
            _gradeLabelTra.Find("GradeText").GetComponent<Text>().text = StaticData.GetMultilingual(120035);
            _gradeLabelText.text = _currShowData._data.Grade.ToString();
        }
        else
        {
            _gradeLabelTra.gameObject.SetActive(false);
        }

        _numberText.text = _currShowData._number.ToString();
        Sprite iconSprite = null;
        try
        {
            iconSprite = await ABManager.GetAssetAsync<Sprite>(_currShowData._data.Icon);
        }
        catch (System.Exception er)
        {
            Debug.Log("道具icon未找到");
        }

        _iconImage.sprite = iconSprite;

        _nameText.text = StaticData.GetMultilingual(_currShowData._data.ItemName);
        //_iconImage.SetNativeSize();
    }
    /// <summary>
    /// 按钮点击
    /// </summary>
    public void OnClick()
    {
        WarehouseController.Instance.OpenDetailsPopUp(_currShowData, _currDataIndex, this);
        WarehouseTool.SetGoodsDataNew(_currShowData);
    }

    /// <summary>
    /// 刷新数据显示
    /// </summary>
    public void RefreshDataShow()
    {
        GetData(_currDataIndex);
    }
    #endregion
}
