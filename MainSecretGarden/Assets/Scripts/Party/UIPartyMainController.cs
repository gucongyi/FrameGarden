using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 晚会游戏界面 主界面
/// </summary>
public class UIPartyMainController : MonoBehaviour
{
    /// <summary>
    /// 单例字段
    /// </summary>
    public static UIPartyMainController _instance;
    private void Awake()
    {
        _instance=this;
    }
    #region 变量

    private Button _butQuitParty;
    public Button _butInGuess;
    public Text countDownText;
    public Text partyCountDownText;

    private int _expTime = 300;
    private float _expTimer = 0;

    #endregion

    #region 属性
    #endregion

    #region 函数

    // Start is called before the first frame update
    void Start()
    {
        Init();

        //便于及时获取竞猜当前阶段和倒计时时间差
        PartyGuessTime.Instance.GuessTimeControl();


        //先进行一次判断 看是否改变竞猜按钮状态
        ChangeGuessState();


    }

    //测试 模拟晚会快结束
    float i = 0;


    // Update is called once per frame
    void Update()
    {
        ExpTimer();

        //模拟在160秒后推送晚会快结束
        if (i < 160)
        {
            //竞猜倒计时
            PartyManager._instance.GuessCountDown();
            i += Time.deltaTime;

            //模拟获得晚会结束时间推送
            PartyManager._instance.TestPushPartyActivityFinish();
            return;
        }

        PartyManager._instance.PartyCountDown();
    }

    private void Init() 
    {
        countDownText = transform.Find("CountDown/number").GetComponent<Text>();
        partyCountDownText = transform.Find("PartyCountDown/number").GetComponent<Text>();

        _butQuitParty = transform.Find("But_QuitParty").GetComponent<Button>();

        _butQuitParty.onClick.RemoveAllListeners();
        _butQuitParty.onClick.AddListener(OnClickQuitParty);


        //修改_SZY_2020/10/27
        _butInGuess = transform.Find("But_InGuess").GetComponent<Button>();

        _butInGuess.onClick.RemoveAllListeners();
        _butInGuess.onClick.AddListener(OnClickInGuess);


    }
    public void InitValue() 
    {

    }

    /// <summary>
    /// 经验
    /// </summary>
    private void ExpTimer() 
    {
        _expTimer += Time.deltaTime;
        if (_expTimer >= _expTime) 
        {
            _expTimer -= _expTime;
            ReceiveExperience();
        }
    }

    /// <summary>
    /// 领取经验
    /// </summary>
    private void ReceiveExperience() 
    {
        Debug.Log("领取经验成功");

        //每次的经验数
        int num=5;

        //StaticData.AddPlayerExp(num);
    }


    /// <summary>
    /// 退出游戏
    /// </summary>
    private void OnClickQuitParty() 
    {
        PartyServerDockingManager.NotifyServerQuitRoom(QuitPartySuccess, QuitPartyFailed);
    }

    private void QuitPartySuccess() 
    {
        PartyManager._instance.QuitRoom();
        PartyManager._instance.stayRoom = false;
    }

    private void QuitPartyFailed()
    {

    }

    /// <summary>
    /// 显示竞猜界面 修改_SZY_2020/10/27
    /// </summary>
    private async void OnClickInGuess()
    {
        Debug.Log("点击成功：" + _butInGuess.name);
        await StaticData.OpenGuess();
    }


    //可以下注 修改_SZY_2020/11/5
    private void CanGuess()
    {
        _butInGuess.interactable = true;
        Debug.Log("竞猜结束，现在可以下注");
    }
    //停止下注 修改_SZY_2020/11/5
    private void StopGuess()
    {
        _butInGuess.interactable = false;
        Debug.Log("现在停止下注,开始竞猜");
    }

    //改变Guess按钮的状态,通过isCanGuess来改变 修改_SZY_2020/11/5
    public void ChangeGuessState()
    {
        if (PartyGuessManager.Instance.isCanGuess)
        {
            CanGuess();
        }
        else
        {
            StopGuess();
        }
    }
    #endregion
}
