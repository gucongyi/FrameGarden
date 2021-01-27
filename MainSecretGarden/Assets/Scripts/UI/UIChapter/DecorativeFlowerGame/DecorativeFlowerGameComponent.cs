using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Quick.UI;
using Company.Cfg;

public class FlowPoint
{
    public Transform trans;
    public bool isEnsconce;//是否安置

    public FlowPoint(Transform pos, bool isEnsconce = false)
    {
        this.trans = pos;
        this.isEnsconce = isEnsconce;
    }
}

public class DecorativeFlowerGameComponent : MonoBehaviour
{
    public enum PlotBranch
    {
        Flower,
        Vase
    }
    public PlotBranch branch;//外部传入赋值

    Button _backBtn;
    Button _finishBtn;

    bool _isDecorativeState;//是否进入装饰状态
    public Image _vase;//花瓶or花束图片
    public Image _silkRibbon;//花束特有的丝带图片

    public int _squidItemMaxAmount = 20;//花枝最大数
    public int _appliqueItemMaxAmount = 10;//贴花最大数
    public int _decorativeItemMaxAmount = 10;//装饰品最大数
    public List<FlowPoint> _flowerPos;//存放花枝
    public List<GameObject> _appliqueItemList;//存放贴花
    public List<GameObject> _decorativeItemList;//存放装饰品

    [SerializeField] List<Tab> _tab;//4个tab里的内容 Inspector面板赋值
    [SerializeField] List<GameObject> _pos;//固定的20点 Inspector面板赋值
    //渐显
    CanvasGroup canvasGroup;

    private void Start()
    {
        //查询引用
        ComponentFindQuote();
        //注册事件 
        RegisteredEvents();
        //初始化
        Init();
        //国际化
        GetText(branch);
        //开场动画
        OpeningAnima();
    }

    void ComponentFindQuote()
    {
        _backBtn = transform.Find("BackBtn").GetComponent<Button>();
        _finishBtn = transform.Find("FinishBtn").GetComponent<Button>();
        _vase = transform.Find("vase/vaseImage").GetComponent<Image>();
        _silkRibbon = transform.Find("vase/SilkRibbon").GetComponent<Image>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void RegisteredEvents()
    {
        _backBtn.onClick.RemoveAllListeners();
        _backBtn.onClick.AddListener(() => UIComponent.RemoveUI(UIType.DecorativeFlowerGame));

        _finishBtn.onClick.RemoveAllListeners();
        _finishBtn.onClick.AddListener(() => UIComponent.RemoveUI(UIType.DecorativeFlowerGame));
        //StaticData.configExcel
    }

    private void Init()
    {
        canvasGroup.alpha = 0;
        //初始20个花枝点
        _flowerPos = new List<FlowPoint>(_squidItemMaxAmount);
        foreach (var item in _pos)
        {
            FlowPoint flowPoint = new FlowPoint(item.transform);
            _flowerPos.Add(flowPoint);
        }
        //初始化tabItem
        //branch = PlotBranch.Flower;
        List<DecorativeFlowerGameDefine> branchList = StaticData.configExcel.DecorativeFlowerGame;
        if (branch == PlotBranch.Vase)
        {//花瓶 移除只属于花束的道具
            _silkRibbon.gameObject.SetActive(false);//隐藏花束缎带
            foreach (var item in new List<DecorativeFlowerGameDefine>(branchList))
            {
                if (item.ItemBelongTo == 2)
                    branchList.Remove(item);
            }
        }
        else
        {//花束 移除花瓶
            foreach (var item in new List<DecorativeFlowerGameDefine>(branchList))
            {
                if (item.ItemBelongTo == 1)
                    branchList.Remove(item);
            }
        }
        List<DecorativeFlowerGameDefine> typeList = new List<DecorativeFlowerGameDefine>();
        for (int i = 0; i < _tab.Count; i++)
        {
            Transform parent = _tab[i].transform.Find("Page/Scroll/ViewMask/Content");
            GameObject go;
            if (i == 2 && branch == PlotBranch.Flower)
                go = parent.Find("flowItem").gameObject;
            else
                go = parent.Find("Item").gameObject;
            typeList.Clear();
            //数据按类型分开
            foreach (var item in branchList)
            {
                if (i == 0 || i == 1 || (i == 2 && branch == PlotBranch.Vase))
                {
                    if (item.ItemType == i + 1)
                        typeList.Add(item);
                }
                else
                {
                    if (item.ItemType == i + 2)
                        typeList.Add(item);
                }
            }
            for (int j = 0; j < typeList.Count; j++)
            {
                GameObject g = Instantiate(go);
                //go.GetComponent<Image>().sprite;//替换item图片
                g.transform.Find("Name").GetComponent<Text>().text = typeList[j].ItemNotes;
                g.transform.SetParent(parent);
                g.transform.localPosition = Vector3.zero;
                g.transform.localScale = Vector3.one;
                g.SetActive(true);
            }
        }
    }

    void OpeningAnima()
    {
        canvasGroup.DOFade(1, 0.5f);
    }

    void GetText(PlotBranch branch)
    {
        List<Text> texts = new List<Text>();
        texts.Clear();
        for (int i = 0; i < _tab.Count; i++)
        {
            texts.Add(_tab[i].transform.Find("Label").GetComponent<Text>());
        }
        if (branch == PlotBranch.Vase)
        {
            texts[0].text = "花瓶";
            texts[2].text = "贴花";
        }
        else
        {
            texts[0].text = "包装纸";
            texts[2].text = "丝带";
        }
        texts[1].text = "花枝";//StaticData.GetMultilingual()
        texts[3].text = "装饰";
    }

    public void ChangeVase(Sprite sprite)
    {
        _vase.sprite = sprite;
    }
    public void ChangeSilkRibbon(Sprite sprite)
    {
        _silkRibbon.sprite = sprite;
    }
}
