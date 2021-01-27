using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 邮箱计时器
/// </summary>
public class MailBoxTimeController : MonoBehaviour
{
    #region 字段
    /// <summary>
    /// 邮件时间戳字典
    /// </summary>
    Dictionary<int, Text> _mailTimerDic = new Dictionary<int, Text>();
    /// <summary>
    /// 是否已经初始化
    /// </summary>
    bool _isInitial = false;
    #endregion
    #region 函数
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_mailTimerDic != null && _mailTimerDic.Count > 0)
        {
            //Debug.Log("当前计时个数：" + _mailTimerDic.Count);
            ChangeTime();
        }

    }

    /// <summary>
    /// 修改邮件剩余时间
    /// </summary>
    void ChangeTime()
    {
        foreach (var item in _mailTimerDic)
        {

            //计算当前服务器时间
            long CurrTimeStampServer = TimeHelper.ServerTimeStampNow;
            //计算剩余时间
            long CurrRemainTime = MailboxController.Instance.GetData(item.Key)._timestamp - CurrTimeStampServer;

            float remainingTime = CurrRemainTime / 1000f;

            remainingTime = (remainingTime - Time.deltaTime);

            if (remainingTime <= 0)
            {
                //Debug.Log("倒计时结束");
            }
            else
            {
                //Debug.Log("修改剩余时间");
                item.Value.text = MailboxTool.GetTimeRemaining(remainingTime);
            }
        }
    }

    public void SetCountDown(int id, Text showText)
    {

        if (_mailTimerDic.ContainsKey(id))
        {
            _mailTimerDic[id] = showText;
        }
        else
        {
            _mailTimerDic.Add(id, showText);
        }

    }


    public void CloseCountDown(int id)
    {
        if (_mailTimerDic.ContainsKey(id))
        {
            _mailTimerDic.Remove(id);
        }
    }
    #endregion
}
