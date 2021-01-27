using Company.Cfg;
using Game.Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 领奖界面奖品展示item
/// </summary>
public class ReceiveAwardGiftItemController : MonoBehaviour
{
    #region 字段
    /// <summary>
    /// 背景图片精灵集合
    /// </summary>
    [SerializeField]
    List<Sprite> _gradeSprites = new List<Sprite>();
    /// <summary>
    /// 遮罩图片精灵集合
    /// </summary>
    [SerializeField]
    List<Sprite> _maskSprites = new List<Sprite>();
    /// <summary>
    /// 背景
    /// </summary>
    Image _bgIcon;
    /// <summary>
    /// 遮罩
    /// </summary>
    Image _maskImage;
    /// <summary>
    /// 商品icon
    /// </summary>
    Image _icon;
    /// <summary>
    /// 数量
    /// </summary>
    Text _showNumberText;
    /// <summary>
    /// 数量
    /// </summary>
    Text _showNameText;
    /// <summary>
    /// 等级
    /// </summary>
    Text _showGradeText;
    /// <summary>
    /// 当前展示数据
    /// </summary>
    CSWareHouseStruct _data;
    /// <summary>
    /// 显示控制器
    /// </summary>
    CanvasGroup _canvasGroup;
    /// <summary>
    /// 是否已经初始化
    /// </summary>
    bool _isInitial = false;
    #endregion
    #region 函数
    private void Awake()
    {
        if (!_isInitial)
        {
            Initial();
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// 初始化组件
    /// </summary>
    public void Initial()
    {
        _bgIcon = transform.Find("Bg").GetComponent<Image>();
        _maskImage = transform.Find("Mask").GetComponent<Image>();
        _icon = transform.Find("Icon").GetComponent<Image>();
        Transform tra = transform.Find("ShowNumberBg");
        _showNumberText = tra.Find("ShowNumber").GetComponent<Text>();
        Transform a = transform.Find("ShowNameberBg/ShowName");
        _showNameText = a.GetComponent<Text>();
        Transform grade = transform.Find("Grade/ShowGrade");
        _showGradeText = grade.GetComponent<Text>();

        _canvasGroup = transform.GetComponent<CanvasGroup>();
        _isInitial = true;
    }
    /// <summary>
    /// 展示奖品
    /// </summary>
    /// <param name="data"></param>
    public async void ShowPrize(CSWareHouseStruct data)
    {
        if (!_isInitial)
        {
            Initial();
        }
        _data = data;
        _showNumberText.text = data.GoodNum.ToString();
        GameItemDefine gameItemDefine = WarehouseTool.GetGameItemData(data.GoodId);
        if (gameItemDefine != null)
        {
            _showNameText.text = StaticData.GetMultilingual(gameItemDefine.ItemName);
            Transform grade = transform.Find("Grade");
            grade.gameObject.SetActive(gameItemDefine.Grade > 0);

            _showGradeText.text = gameItemDefine.Grade.ToString();
        
            //根据稀有度设置背景
            int index = 0;
            switch (gameItemDefine.Rarity)
            {
                case TypeRarity.None:
                case TypeRarity.Primary:
                    index = 0;
                    break;
                case TypeRarity.Intermediate:
                case TypeRarity.Senior:
                    index = (int)gameItemDefine.Rarity - 1;
                    break;
            }
            _bgIcon.sprite = _gradeSprites[index];
            _maskImage.sprite = _maskSprites[index];
        }
        else
        {
            _showNameText.text = "获取不到配置数据";
            _showGradeText.text = "获取不到配置数据";
        }
        _icon.sprite = await ABManager.GetAssetAsync<Sprite>(gameItemDefine.Icon);
        gameObject.SetActive(true);
    }
    /// <summary>
    /// 是否显示
    /// </summary>
    /// <param name="isShow"></param>
    public void Show(bool isShow)
    {
        if (isShow)
        {
            _canvasGroup.alpha = 1;
        }
        else
        {
            _canvasGroup.alpha = 0;
        }

    }

    #endregion
}
