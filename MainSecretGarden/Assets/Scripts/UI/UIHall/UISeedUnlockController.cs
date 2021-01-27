using Company.Cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 种子解锁 控制
/// </summary>
public class UISeedUnlockController : MonoBehaviour
{
    #region 变量

    private Transform _bgTra;

    private Transform _unlockItemContent;
    private Transform _miniList;

    private Button _butCancel;
    private Button _butBuy;

    private string _itemPath = "UISeedUnlockItem";

    #endregion

    #region 方法

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        _bgTra = transform.Find("BG_Image");
        if (_bgTra != null)
            UniversalTool.ReadyPopupAnim(_bgTra);
    }

    private void OnEnable()
    {
        if (_bgTra != null)
            UniversalTool.StartPopupAnim(_bgTra);
    }

    private void OnDisable()
    {
        if (_bgTra != null)
            UniversalTool.ReadyPopupAnim(_bgTra);
    }

    private void Init() 
    {
        if (_unlockItemContent != null)
            return;

        _unlockItemContent = transform.Find("BG_Image/ScrollView/Viewport/Content");
        _miniList = transform.Find("BG_Image/MiniList");

        _butCancel = transform.Find("BG_Image/But_Cancel").GetComponent<Button>();
        _butBuy = transform.Find("BG_Image/But_Buy").GetComponent<Button>();

        _butCancel.onClick.RemoveAllListeners();
        _butCancel.onClick.AddListener(() => { 
            UniversalTool.CancelPopAnim(_bgTra, OnClickCancel); } );

        _butBuy.onClick.RemoveAllListeners();
        _butBuy.onClick.AddListener(() => { 
            UniversalTool.CancelPopAnim(_bgTra, OnClickEnter); } );
    }

    public async void InitValue(List<GameItemDefine> unlockCrops) 
    {
        Init();

        //移除上次的物品
        for (int i = 0; i < _unlockItemContent.childCount; i++)
        {
            Destroy(_unlockItemContent.GetChild(i).gameObject);
        }
        for (int i = 0; i < _miniList.childCount; i++)
        {
            Destroy(_miniList.GetChild(i).gameObject);
        }

        Transform parent = null;
        //
        if (unlockCrops.Count <= 4)
        {
            parent = _miniList;
            _miniList.gameObject.SetActive(true);
            _unlockItemContent.parent.parent.gameObject.SetActive(false);
        }
        else
        {
            parent = _unlockItemContent;
            _miniList.gameObject.SetActive(false);
            _unlockItemContent.parent.parent.gameObject.SetActive(true);
        }

        GameObject obj = await ABManager.GetAssetAsync<GameObject>(_itemPath);
        for (int i = 0; i < unlockCrops.Count; i++)
        {
            SpawnItem(unlockCrops[i], obj, parent);
        }

    }

    private async void SpawnItem(GameItemDefine item, GameObject obj, Transform parent) 
    {
        GameObject itemObj = Instantiate(obj, parent);
        itemObj.transform.Find("BG/Icon").GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(item.Icon);
        itemObj.transform.Find("BG/Name").GetComponent<Text>().text = StaticData.GetMultilingual(item.ItemName);
    }

    private void OnClickCancel() 
    {
        UIComponent.RemoveUI(UIType.UISeedUnlock);
    }

    private async void OnClickEnter()
    {

        //新手引导标记完成
        if (StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.isCurrStepGuiding)
        {
            GuideCanvasComponent._instance.SetLittleStepFinish();
        }
        //需要打开界面
        await StaticData.OpenShopUI(0);
        OnClickCancel();
    }



    #endregion
}
