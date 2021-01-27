using Company.Cfg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using System;
/// <summary>
/// 大富翁结算界面
/// </summary>
public class UISettlement : MonoBehaviour
{
    /// <summary>
    /// item路径
    /// </summary>
    private string _itemPath = "UIItem";

    private Transform _content;
    private Button _butClose;
    private Image _bg;
    private Image _title;
    private Text _tips;


    //暂存item的链表 用于动画
    private List<GameObject> itemAnim = new List<GameObject>();
    //判断动画是否播放完
    private bool _isFinishAnim = false;
    //用于物品展示动画计时
    private float timer = 0;

    private void OnEnable()
    {
        if (_bg != null)
        {
            //播放动画
            PlayInitAnim();
        }

    }

    private void Init() 
    {
        if (_butClose != null)
            return;

        _butClose = transform.Find("BGClose").GetComponent<Button>();
        _content = transform.Find("BG/ScrollView/Viewport/Content");
        _bg = transform.Find("BG").GetComponent<Image>();
        _title = transform.Find("Title").GetComponent<Image>();
        _tips = transform.Find("Tips").GetComponent<Text>();
        _butClose.onClick.RemoveAllListeners();
        _butClose.onClick.AddListener(OnClickConfirm);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="items">id + 数量</param>
    public void Initial(Dictionary<int, int> items) 
    {
        Init();

        for (int i = 0; i < _content.childCount; i++)
        {
            Destroy(_content.GetChild(i).gameObject);
        }

        InitialItems(items);

        //播放动画
        PlayInitAnim();
    }

    private async void InitialItems(Dictionary<int, int> items) 
    {
        GameObject obj = await ABManager.GetAssetAsync<GameObject>(_itemPath);

        // 11- 26确定不加经验了
        //GameObject itemObj = Instantiate(obj, _content);
        //SetItemInfo(itemObj, StaticData.configExcel.GetVertical().ExpId, ZillionaireToolManager.GetMapExp());

        GameObject itemObj = null;
        foreach (var item in items) 
        {
            itemObj = Instantiate(obj, _content);
            SetItemInfo(itemObj, item.Key, item.Value);

            //添加进用于动画的链表
            itemAnim.Add(itemObj);
        }
    }
    /// <summary>
    /// 设置item信息
    /// </summary>
    /// <param name="itemObj"></param>
    /// <param name="id"></param>
    /// <param name="num"></param>
    private void SetItemInfo(GameObject itemObj, int id, int num) 
    {
        GameItemDefine itemData = StaticData.configExcel.GetGameItemByID(id);

        //name
        itemObj.transform.Find("BG/Name").GetComponent<Text>().text = LocalizationDefineHelper.GetStringNameById(itemData.ItemName);
        //icon
        Sprite icon = ABManager.GetAsset<Sprite>(itemData.Icon);
        Transform iconTra = itemObj.transform.Find("BG/Icon");
        iconTra.GetComponent<Image>().sprite = icon;
        iconTra.GetComponent<Image>().SetNativeSize();
        //num
        itemObj.transform.Find("BG/Num").GetComponent<Text>().text = num.ToString();
    }

    /// <summary>
    /// 点击确认
    /// </summary>
    private void OnClickConfirm() 
    {
        if (_isFinishAnim == false) return;

        UIComponent.HideUI(UIType.UISettlement);
        //回到主界面
        ZillionaireUIManager._instance.ReturnHomePage();
    }

    /// <summary>
    /// 播放界面初始动画
    /// </summary>
    private async void PlayInitAnim()
    {
        //幕布向下
        DOTween.To(() => _bg.transform.localPosition, pos => _bg.transform.localPosition = pos, new Vector3(0, 114, 0), 0.4f).SetEase(Ease.InQuint);
        await UniTask.Delay(TimeSpan.FromSeconds(0.5));
        //奖励文字显示+物品展示+文字提示
        DOTween.To(() => _title.color, a => _title.color = a, new Color(255, 255, 255, 255), 0.1f).SetEase(Ease.InQuint).OnComplete(()=> { PlayItem(); });
    }

    /// <summary>
    /// 播放物品展示动画
    /// </summary>
    private async void PlayItem()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        foreach (var item in itemAnim)
        {
            item.transform.Find("BG").GetComponent<Image>().rectTransform.DOScale(new Vector3(1, 1, 1), 0.2f);
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        }
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        //文字提示显示
        DOTween.To(() => _tips.color, a => _tips.color = a, new Color(255, 255, 255, 255), 1f).SetEase(Ease.Linear).OnComplete(() => { _isFinishAnim = true; });
    }

    //动画还原
    private void OnDisable()
    {
        //幕布向上
        DOTween.To(() => _bg.transform.localPosition, pos => _bg.transform.localPosition = pos, new Vector3(0, 2510, 0), 0.1f);
        //奖励文字隐藏
        DOTween.To(() => _title.color, a => _title.color = a, new Color(255, 255, 255, 0), 0.1f);
        //文字提示隐藏
        DOTween.To(() => _tips.color, a => _tips.color = a, new Color(255, 255, 255, 0), 1f).OnComplete(() => { _isFinishAnim = false; });
        //删除奖励物品
        Transform obj =transform.Find("BG/ScrollView/Viewport/Content");
        for (int i = 0; i < obj.childCount; i++)
        {
            Destroy(obj.GetChild(i).gameObject);
        }
        itemAnim.Clear();
    }
}
