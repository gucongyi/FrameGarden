using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SpriteTex
{
    oneLine,
    twoLine,
    threeLine
}
/// <summary>
/// 拼图游戏总控制器
/// </summary>
public class JigsawGameController : MonoBehaviour
{
    private Text _timer;
    private Button _exitBtn;
    private Button _startBtn;
    private Button _saveBtn;
    private Button _reBtn;
    private Button _nextBtn;
    private GameObject _maskLayer;

    public int[] curLevel;//当前图片索引
    public int curCut = 4;//切割成几乘几的图

    public JigsawGenerator jg;

    public SpriteTex line;

    private void Start()
    {
        //获取组件引用
        ComponentFindQuote();
        //事件注册
        RegisteredEvents();
        _nextBtn.interactable = false;
        SetTex();
    }

    /// <summary>
    /// 获取组件引用
    /// </summary>
    void ComponentFindQuote()
    {
        _timer = transform.Find("Timer").GetComponent<Text>();
        _exitBtn = transform.Find("ExitBtn").GetComponent<Button>();
        _startBtn = transform.Find("StartBtn").GetComponent<Button>();
        _saveBtn = transform.Find("Bottom/SaveBtn").GetComponent<Button>();
        _reBtn = transform.Find("Bottom/ReBtn").GetComponent<Button>();
        _nextBtn = transform.Find("Bottom/NextBtn").GetComponent<Button>();
        _maskLayer = transform.Find("maskLayer").gameObject;
    }
    /// <summary>
    /// 按钮事件注册
    /// </summary>
    void RegisteredEvents()
    {
        _startBtn.onClick.RemoveAllListeners();
        _startBtn.onClick.AddListener(StartBtn);
        _exitBtn.onClick.RemoveAllListeners();
        _exitBtn.onClick.AddListener(ExitGame);
        _saveBtn.onClick.RemoveAllListeners();
        _saveBtn.onClick.AddListener(SaveBtn);
        _reBtn.onClick.RemoveAllListeners();
        _reBtn.onClick.AddListener(StartBtn);
        _nextBtn.onClick.RemoveAllListeners();
        _nextBtn.onClick.AddListener(NextGame);
    }
    //开始按钮事件
    void StartBtn()
    {
        _startBtn.gameObject.SetActive(false);
        Init();
        _maskLayer.gameObject.SetActive(true);
        _timer.gameObject.SetActive(true);
        StaticData.CreateTimerRebackMilliSeconds(5f, true, (go) => { _timer.gameObject.SetActive(false); _maskLayer.gameObject.SetActive(false); jg.Disperse();/* 分散 TODO*/ },
    (timeCount) =>
    {
        _timer.text = string.Format("00:0{0:0.00}", timeCount);
    });
    }
    //返回大厅事件
    void ExitGame()
    {
        UIComponent.RemoveUI(UIType.JigsawGame);
        //直接退出章节 TODO
    }
    //拼完点击下一步事件
    void NextGame()
    {
        UIComponent.RemoveUI(UIType.JigsawGame);
    }
    //保存按钮事件 TODO
    void SaveBtn()
    {

    }

    private void Init()
    {
        //初始化及重新开始
        jg.Clear();
        jg.Split(curCut);
        jg.Shuffle(curCut);
    }

    void SetTex()
    {
        switch (line)
        {
            case SpriteTex.oneLine:
                jg._spriteName = "dfwxz_haitanditu";
                break;
            case SpriteTex.twoLine:
                jg._spriteName = "dfwyx_jinbi";
                break;
            case SpriteTex.threeLine:
                jg._spriteName = "dfwyx_zhuanshi";
                break;
        }
    }

    private void Update()
    {
        if (jg.isReachAStandard)
        {
            _nextBtn.interactable = true;
        }
    }
}
