using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Protocal;

public class UIPartyGuessController : MonoBehaviour
{
    /// <summary>
    /// 单例字段
    /// </summary>
    private static UIPartyGuessController instance;

    public static UIPartyGuessController Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    #region 变量 

    //WG--乌龟
    private Toggle _toggleWG1;
    private Toggle _toggleWG2;
    private Toggle _toggleWG3;
    private Text PollsText;
    private Button _butSure;
    private Button _butReturn;

    //已拥有票数
    private Text HasPolls;


    //保存所选择乌龟的名字
    public string _WGName;

    //保存所选择的票数
    public int _polls;

    //保存玩家拥有的票数
    public int _hasPolls;



    #endregion
    // Start is called before the first frame update
    void Start()
    {
        Init();

        //获取玩家当前的票数
        _hasPolls=PartyGuessController.Instance.GetNowPlayerPolls();

        Debug.Log("初始化成功下注界面");
    }


    //竞猜界面初始化
    private void Init()
    {
        _toggleWG1 = transform.Find("BG/Group/Tog_WG1").GetComponent<Toggle>();
        _toggleWG1.onValueChanged.RemoveAllListeners();
        _toggleWG1.onValueChanged.AddListener((bool value)=>OnClickWG(_toggleWG1.name,value));


        _toggleWG2 = transform.Find("BG/Group/Tog_WG2").GetComponent<Toggle>();
        _toggleWG2.onValueChanged.RemoveAllListeners();
        _toggleWG2.onValueChanged.AddListener((bool value) => OnClickWG(_toggleWG2.name, value));


        _toggleWG3 = transform.Find("BG/Group/Tog_WG3").GetComponent<Toggle>();
        _toggleWG3.onValueChanged.RemoveAllListeners();
        _toggleWG3.onValueChanged.AddListener((bool value) => OnClickWG(_toggleWG3.name, value));


        PollsText = transform.Find("BG/Polls/number").GetComponent<Text>();


        _butSure = transform.Find("BG/But_Sure").GetComponent<Button>();
        _butSure.onClick.RemoveAllListeners();
        _butSure.onClick.AddListener(OnClickSure);



        HasPolls = transform.Find("BG/HasPolls/number").GetComponent<Text>();

        _butReturn = transform.Find("Return").GetComponent<Button>();
        _butReturn.onClick.RemoveAllListeners();
        _butReturn.onClick.AddListener(StaticData.CloseGuess);

    }
    /// <summary>
    /// 点击选择乌龟 获取乌龟ID
    /// </summary>
    /// <param name="WGName"></param>
    /// <param name="value"></param>
    private void OnClickWG(string WGName,bool value)
    {
        _WGName = WGName;
    }


    /// <summary>
    /// 点击确定 获取当前票数，保存到本地
    /// </summary>
    private void OnClickSure()
    {
        _polls = int.Parse(PollsText.text);


        //点击确实是进行判断玩家是否进行下注
        if (_toggleWG1.isOn==false&&_toggleWG2.isOn==false&&_toggleWG3.isOn==false)
        {
            Debug.Log("请选择想投注的乌龟");
            PartyGuessController.Instance.isGuessPalyer = false;
            return;
        }
        if (_polls <= 0)
        {
            Debug.Log("请投注相应的票数");
            PartyGuessController.Instance.isGuessPalyer = false;
            return;
        }

        PartyGuessController.Instance.isGuessPalyer = true;

        //下注信息保存
        DownLoadDic();
    }

    /// <summary>
    /// 保存到本地字典中
    /// </summary>
    private void DownLoadDic()
    {
        string Now_WgName = "";

        Debug.Log("点击确认成功！！！");
        Debug.Log("票数：" + _polls + "_" + "乌龟：" + _WGName);


        //判断字典中是否有相同的key值，没有就新添加，有就修改之前的value值
        if (Now_WgName != _WGName)
        {

            Now_WgName = _WGName;

            PartyGuessInfo.Instance.Wg_Name = _WGName;
            PartyGuessInfo.userGuessInfoDic.Add(Now_WgName, _polls);
        }
        else
        {
            PartyGuessInfo.userGuessInfoDic[Now_WgName] = _polls;
        }
    }

    public void SetHasPolls(string number)
    {
        HasPolls.text = number;
    }

}
