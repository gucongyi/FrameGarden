using Company.Cfg;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 功能开启展示
/// </summary>
public class UIOpenFunctionController : MonoBehaviour
{
    #region 变量

    private Text _title;
    private Text _functionName;
    private Image _icon;

    private Button _butClose;

    private Transform _tra;
    private RawImage _effect;

    private GameObject _butterfly;
    #endregion


    #region 方法

    private void Awake()
    {
        _tra = transform.Find("BG_Image");
        _effect = transform.Find("BG_Image/EffectImage").GetComponent<RawImage>();
        UniversalTool.ReadyPopupAnim(_tra);
    }

    private void OnEnable()
    {
        SpawnHuDie();
        UniversalTool.StartPopupAnim(_tra);
    }
    private void OnDisable()
    {
    }

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
        if (_title != null)
            return;
        _title = transform.Find("BG_Image/Title").GetComponent<Text>();
        _functionName = transform.Find("BG_Image/NameBG/FunctionName").GetComponent<Text>();
        _icon = transform.Find("BG_Image/IconBG/Icon").GetComponent<Image>();

        _butClose = transform.Find("BG").GetComponent<Button>();
        _butClose.onClick.RemoveAllListeners();
        _butClose.onClick.AddListener(OnClickClose);
    }

    public async void InitValue(OpenFunctionDefine funcInfo) 
    {
        Init();

        //_title.text = LocalizationDefineHelper.GetImageNameById(120293);//新功能开启啦！
        //var funcInfo = StaticData.configExcel.GetOpenFunctionByFunctionID(funcID);
        _functionName.text = LocalizationDefineHelper.GetImageNameById(funcInfo.FunctionName);
        _icon.sprite = await ABManager.GetAssetAsync<Sprite>(funcInfo.FunctionIcon);
        _icon.SetNativeSize();
        //await UniTask.Delay(200);
    }

    private void OnClickClose() 
    {
        UniversalTool.CancelPopAnim(_tra, UIClose);
    }

    private void UIClose() 
    {
        HideDutterfly();
        UIComponent.RemoveUI(UIType.UIOpenFunction);
    }


    public async void SpawnHuDie()
    {
        _effect.texture.width = 1024;
        _effect.texture.height = 1024;

        Transform parent = UIRoot.instance.GetUIRootCanvas().transform.parent;
        string perfabName = "HuDie";
        var obj = await ABManager.GetAssetAsync<GameObject>(perfabName);
        _butterfly = Instantiate(obj, parent);
    }

    private void HideDutterfly() 
    {
        _butterfly.SetActive(false);
        Destroy(_butterfly);
        _butterfly = null;
    }

    #endregion
}
