using Company.Cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 控制显示道具信息
/// </summary>
public class UIItemShow : MonoBehaviour
{
    #region 变量

    private Text _name;
    private Text _num;
    private Text _ownAndDemandValue;

    private GameObject _lv;
    private Text _lvNum;

    private GameObject _level1;
    private Image _level1Icon;

    private GameObject _level2;
    private Image _level2Icon;

    private GameObject _level3;
    private Image _level3Icon;




    #endregion

    #region 函数

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Init() 
    {
        if (_name != null)
            return;

        _name = transform.Find("BG/NameBG/Name").GetComponent<Text>();
        _num = transform.Find("BG/Num").GetComponent<Text>();
        _ownAndDemandValue = transform.Find("BG/Value").GetComponent<Text>();

        _lv = transform.Find("BG/LvBG").gameObject;
        _lvNum = transform.Find("BG/LvBG/Lv").GetComponent<Text>();

        _level1 = transform.Find("BG/Level1").gameObject;
        _level1Icon = transform.Find("BG/Level1/BGIcon/Icon").GetComponent<Image>();

        _level2 = transform.Find("BG/Level2").gameObject;
        _level2Icon = transform.Find("BG/Level2/BGIcon/Icon").GetComponent<Image>();

        _level3 = transform.Find("BG/Level3").gameObject;
        _level3Icon = transform.Find("BG/Level3/BGIcon/Icon").GetComponent<Image>();

        _ownAndDemandValue.gameObject.SetActive(false);
    }

    /// <summary>
    /// 初始化值 数量+名称+图片+等级
    /// </summary>
    /// <param name="id"></param>
    /// <param name="num"></param>
    /// <param name="localScale"></param>
    public async void InitValue(int id, int num, float localScale = 0.0f, bool isShowLv = false) 
    {
        Init();

        GameItemDefine itemData = StaticData.configExcel.GetGameItemByID(id);
        
        _name.text = LocalizationDefineHelper.GetStringNameById(itemData.ItemName);
        Sprite icon = await ZillionaireToolManager.LoadItemSpriteByIconName(itemData.Icon);

        _lv.SetActive(isShowLv);
        if (isShowLv) 
        {
            _lvNum.text = itemData.Grade.ToString();
        }
        _level1.SetActive(false);
        _level2.SetActive(false);
        _level3.SetActive(false);
        switch (itemData.Rarity)
        {
            case TypeRarity.Intermediate:
                _level2.SetActive(true);
                _level2Icon.sprite = icon;
                _level2Icon.SetNativeSize();
                break;
            case TypeRarity.Senior:
                _level3.SetActive(true);
                _level3Icon.sprite = icon;
                _level3Icon.SetNativeSize();
                break;
            default:
                _level1.SetActive(true);
                _level1Icon.sprite = icon;
                _level1Icon.SetNativeSize();
                break;
        }

        if (num <= 0)
            _num.gameObject.SetActive(false);

        _num.text = num.ToString();

        if (localScale != 0.0f) 
        {
            gameObject.transform.localScale = new Vector3(localScale, localScale);
        }
    }

    /// <summary>
    /// 订单界面显示 
    /// </summary>
    /// <param name="itemData"></param>
    /// <param name="num"></param>
    public async void InitValue(GameItemDefine itemData, string ownAndDemand) 
    {
        Init();

        _name.gameObject.SetActive(false);
        _name.gameObject.transform.parent.gameObject.SetActive(false);
        _num.gameObject.SetActive(false);
        _lv.SetActive(false);

        Sprite icon = await ZillionaireToolManager.LoadItemSpriteByIconName(itemData.Icon);

        _level1.SetActive(false);
        _level2.SetActive(false);
        _level3.SetActive(false);
        switch (itemData.Rarity)
        {
            case TypeRarity.Intermediate:
                _level2.SetActive(true);
                _level2Icon.sprite = icon;
                _level2Icon.SetNativeSize();
                break;
            case TypeRarity.Senior:
                _level3.SetActive(true);
                _level3Icon.sprite = icon;
                _level3Icon.SetNativeSize();
                break;
            default:
                _level1.SetActive(true);
                _level1Icon.sprite = icon;
                _level1Icon.SetNativeSize();
                break;
        }

        _ownAndDemandValue.gameObject.SetActive(true);
        _ownAndDemandValue.text = ownAndDemand;
    }

    #endregion
}
