using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 仓库选择按钮
/// </summary>
public class WarehouseSelectItemController : MonoBehaviour
{
    #region 字段
    /// <summary>
    /// 对应仓库下标
    /// </summary>
    [SerializeField]
    int _warehouseIndx;
    /// <summary>
    /// 按钮显示文字
    /// </summary>
    [SerializeField]
    int _btnName;
    /// <summary>
    /// 未选中时字体尺寸
    /// </summary>
    [SerializeField]
    int _noSelectFontSize;
    /// <summary>
    /// 选中时字体尺寸
    /// </summary>
    [SerializeField]
    int _selectFontSize;
    /// <summary>
    /// 未选中的图片image
    /// </summary>
    Image _noSelectImage;
    /// <summary>
    /// 选中的图片image
    /// </summary>
    Image _selectImage;
    /// <summary>
    /// 线条
    /// </summary>
    RectTransform _lineTra;
    /// <summary>
    /// 按钮文字输出文本
    /// </summary>
    Text _showText;
    /// <summary>
    /// 自身ui组件
    /// </summary>
    RectTransform _thisRect;
    /// <summary>
    /// 勾选组件
    /// </summary>
    Toggle _toggle;
    /// <summary>
    /// 未选中时的尺寸
    /// </summary>
    Vector2 _noSelectSize;
    /// <summary>
    /// 选中时的尺寸
    /// </summary>
    Vector2 _selectSize;
    /// <summary>
    /// 选中回调
    /// </summary>
    Action<int> _selectAction;
    /// <summary>
    /// 红点
    /// </summary>
    RectTransform _redDotRect;
    #endregion
    #region 属性
    /// <summary>
    /// 自身ui组件
    /// </summary>
    public RectTransform _ThisRect { get { return _thisRect; } }
    /// <summary>
    /// 勾选组件
    /// </summary>
    public Toggle _Toggle { get { return _toggle; } }
    /// <summary>
    /// 仓库下标
    /// </summary>
    public int _WarehouseIndx { get { return _warehouseIndx; } }
    #endregion
    #region 函数
    // Start is called before the first frame update
    void Start()
    {

    }
    /// <summary>
    /// 初始化
    /// </summary>
    public void Initial(Action<int> selectAction, bool isOpenToggle = false)
    {
        _thisRect = GetComponent<RectTransform>();
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.RemoveListener(SelectWaerhouse);
        _toggle.onValueChanged.AddListener(SelectWaerhouse);
        _showText = transform.Find("Label").GetComponent<Text>();
        _noSelectImage = transform.Find("Background").GetComponent<Image>();
        _selectImage = transform.Find("Background/Checkmark").GetComponent<Image>();
        _lineTra = transform.Find("LineBox").GetComponent<RectTransform>();
        _redDotRect = transform.Find("RedDot").GetComponent<RectTransform>();
        _selectAction = selectAction;
        //获取未选中时的尺寸
        RectTransform noSelectRect = transform.Find("Background").GetComponent<RectTransform>();
        _noSelectSize = new Vector2(noSelectRect.rect.width, noSelectRect.rect.height);
        //获取选中时的尺寸
        RectTransform selectRect = transform.Find("Background/Checkmark").GetComponent<RectTransform>();
        _selectSize = new Vector2(selectRect.rect.width, selectRect.rect.height);

        _Toggle.isOn = isOpenToggle;
        _showText.text = StaticData.GetMultilingual(_btnName);
    }
    /// <summary>
    /// 点击回调
    /// </summary>
    /// <param name="arg0"></param>
    private void SelectWaerhouse(bool arg0)
    {
        if (arg0)
        {
            _thisRect.sizeDelta = _selectSize;
            _showText.fontSize = _selectFontSize;
            _showText.color = new Color32(255, 255, 255, 255);
            _noSelectImage.raycastTarget = false;
            _selectImage.raycastTarget = true;
            _selectAction?.Invoke(_warehouseIndx);
            _lineTra.gameObject.SetActive(false);
            if (_warehouseIndx==2&&WarehouseController.Instance.IsHaveNewTreasureChaests())
            {
                OpenRedDot(true);
            }
            else
            {
                OpenRedDot(false);
            }
          
        }
        else
        {
            if (_selectImage.raycastTarget)
            {
                WarehouseController.Instance.UpdateItemNewTage(_warehouseIndx);
            }
            _thisRect.sizeDelta = _noSelectSize;
            _showText.fontSize = _noSelectFontSize;
            _showText.color = new Color32(139, 162, 240, 255);
            _noSelectImage.raycastTarget = true;
            _selectImage.raycastTarget = false;
            _lineTra.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// 开关红点
    /// </summary>
    /// <param name="isOpen"></param>
    public void OpenRedDot(bool isOpen)
    {
        if (_redDotRect==null)
        {
            _redDotRect= transform.Find("RedDot").GetComponent<RectTransform>();
        }
        _redDotRect.gameObject.SetActive(isOpen);
    }
    /// <summary>
    /// 设置勾选按钮
    /// </summary>
    /// <param name="isSelect"></param>
    public void SetTogge(bool isSelect)
    {
        _toggle.isOn = isSelect;
    }
    // Update is called once per frame
    void Update()
    {

    }
    #endregion
}
