using Company.Cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 宝箱解锁界面item
/// </summary>
public class ShowPrizeItemController : MonoBehaviour
{

    #region 字段
    Image _icon;
    Text _nameText;
    int _dataId;
    bool _isInitial = false;
    #endregion
    /// <summary>
    /// 初始话
    /// </summary>
    public void Initial()
    {
        _icon = transform.Find("IconBox/Icon").GetComponent<Image>();
        _nameText = transform.Find("Text").GetComponent<Text>();
        _isInitial = true;
    }
    private void Awake()
    {
        Initial();
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
    /// 展示数据
    /// </summary>
    /// <param name="probabilityReward"></param>
    public async void ShowData(int awardId)
    {
        if (!_isInitial)
        {
            Initial();
        }
        gameObject.SetActive(true);
        _dataId = awardId;
        GameItemDefine gameItemDefine = WarehouseTool.GetGameItemData(_dataId);
        if (gameItemDefine==null)
        {
            Debug.Log("获取道具配置数据失败：" + _dataId);
            return;
        }
        _icon.sprite = null;
        Sprite iconSprite = null;
        try
        {
            iconSprite = await ABManager.GetAssetAsync<Sprite>(gameItemDefine.Icon);
        }
        catch (System.Exception)
        {
            Debug.Log("获取道具icon失败：" + gameItemDefine.ID);
        }
        _icon.sprite = iconSprite;
        //_icon.SetNativeSize();

        _nameText.text = StaticData.GetMultilingual(gameItemDefine.ItemName);

    }
}
