
using System;
using UnityEngine;
using UnityEngine.UI;
public class TimeCountDownComponent : MonoBehaviour,IDisposable
{
    Action<float> OnActionOnSecondPass;
    Action<float> OnActionOnFramePass;
    float timeCountDown;
    Action OnTimeCountDownCompelete;
    Action<GameObject> OnTimeCountDownGoCompelete;
    float timeCount;
    float timeOneSecondCount;
    bool isUnscaledDeltaTime;
    public void Update()
    {
        if (timeCount <= 0f)
        {
            return;
        }
        if (isUnscaledDeltaTime)
        {
            timeCount -= Time.unscaledDeltaTime;
            timeOneSecondCount += Time.unscaledDeltaTime;
        }
        else
        {
            timeCount -= Time.deltaTime;
            timeOneSecondCount += Time.deltaTime;
        }
        ShowEachFrame();
        if (timeOneSecondCount > 1f)
        {
            timeOneSecondCount = 0f;
            showSeconds();
        }
        if (timeCount <= 0f)
        {
            //倒计时时间到了，回调
            OnTimeCountDownCompelete?.Invoke();
            OnTimeCountDownGoCompelete?.Invoke(gameObject);
            showSeconds();
        }
    }
    public void Init(float timeCountDown, bool isUnscaledDeltaTime, Action OnTimeCountDownCompelete,Action<float> OnActionOnSecondPass)
    {
        this.OnActionOnSecondPass = OnActionOnSecondPass;
        this.OnTimeCountDownCompelete = OnTimeCountDownCompelete;
        this.isUnscaledDeltaTime = isUnscaledDeltaTime;
        this.timeCount = timeCountDown;
        timeOneSecondCount = 0f;
        //初始的时候显示一次时间
        showSeconds();
    }
    public void Init(float timeCountDown, bool isUnscaledDeltaTime, Action<GameObject> OnTimeCountDownCompelete, Action<float> OnActionOnSecondPass)
    {
        this.OnActionOnSecondPass = OnActionOnSecondPass;
        this.OnTimeCountDownGoCompelete = OnTimeCountDownCompelete;
        this.isUnscaledDeltaTime = isUnscaledDeltaTime;
        this.timeCount = timeCountDown;
        timeOneSecondCount = 0f;
        //初始的时候显示一次时间
        showSeconds();
    }
    public void InitSecondsOnFrameCallBack(float timeCountDown, bool isUnscaledDeltaTime, Action<GameObject> OnTimeCountDownCompelete, Action<float> OnActionOnFramePass)
    {
        this.OnActionOnFramePass = OnActionOnFramePass;
        this.OnTimeCountDownGoCompelete = OnTimeCountDownCompelete;
        this.isUnscaledDeltaTime = isUnscaledDeltaTime;
        this.timeCount = timeCountDown;
        //初始的时候显示一次时间
        ShowEachFrame();
    }

    public void ShowEachFrame()
    {
        if (OnActionOnFramePass != null)
        {
            OnActionOnFramePass.Invoke(timeCount);
        }
    }
    public void showSeconds()
    {
        if (OnActionOnSecondPass != null)
        {
            OnActionOnSecondPass.Invoke(timeCount);
        }
    }
    public void Dispose()
    {
        OnTimeCountDownCompelete = null;
        OnTimeCountDownGoCompelete = null;
        timeCount = 0f;
        timeOneSecondCount = 0f;
    }
}