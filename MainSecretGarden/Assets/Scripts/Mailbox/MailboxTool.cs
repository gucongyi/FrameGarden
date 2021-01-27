using Company.Cfg;
using Cysharp.Threading.Tasks;
using Game.Protocal;
using Google.Protobuf;
using Google.Protobuf.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 邮箱工具
/// </summary>
public static class MailboxTool
{
    #region 字段
    /// <summary>
    /// 是否已经获取邮件信息
    /// </summary>
    public static bool _isGetMaliData;
    /// <summary>
    /// 是否开启邮件红点
    /// </summary>
    public static bool _isOpenMailTag = false;
    /// <summary>
    /// 邮件推送回调
    /// </summary>
    static Action _pushAction;
    /// <summary>
    /// 邮件原始信息
    /// </summary>
    public static Dictionary<int, CSMailStruct> _mailDic = new Dictionary<int, CSMailStruct>();
    /// <summary>
    /// 推送过来的邮件
    /// </summary>
    public static Dictionary<int, MailStruct> _pushMailDic = new Dictionary<int, MailStruct>();
    #endregion
    /// <summary>
    /// 服务推送来的邮件
    /// </summary>
    public static void ReceptionPushData(IMessage msg)
    {
        _isGetMaliData = false;
        GetMailboxData((data) => { RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Mailbox); });
    }
    /// <summary>
    /// 服务推送来的邮件
    /// </summary>
    public static void ReceptionPushDataTwo(IMessage msg)
    {
        if (msg is SCSendMailPushMsg)
        {
            SCSendMailPushMsg dataBox = msg as SCSendMailPushMsg;

            MailStruct data = dataBox.MailInfoMsg;
            if (!_pushMailDic.ContainsKey(data.MailID))
            {
                _pushMailDic.Add(data.MailID, data);
                if (!_mailDic.ContainsKey(data.MailID))
                {
                    CSMailStruct cSMailStruct = new CSMailStruct();
                    cSMailStruct.MailId = data.MailID;
                    cSMailStruct.State = 1;
                    cSMailStruct.Message = data.MailContent;
                    cSMailStruct.Title = data.MailTitle;
                    cSMailStruct.SubheadID = data.SubheadID;
                    cSMailStruct.Addresser = data.Addresser;
                    cSMailStruct.ConfigId = data.ConfigID;
                    cSMailStruct.DisabledTime = data.DisabledTime;
                    cSMailStruct.HaveAccessory = data.PropInfo != null && data.PropInfo.Count > 0;
                    _mailDic.Add(cSMailStruct.MailId, cSMailStruct);

                    if (MailboxController.Instance != null)
                    {
                        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Mailbox);
                    }
                    else
                    {
                        _pushAction?.Invoke();
                    }
                }
                else
                {
                    Debug.Log("推送邮件id跟现有邮件id重复：" + data.MailID);
                }
            }
            else
            {
                Debug.Log("推送邮件重复：" + data.MailID);
            }
        }
        else
        {
            Debug.Log("推送邮件数据类型出错");
        }
    }
    /// <summary>
    /// 获取字符串前面几位字符
    /// </summary>
    /// <param name="str"></param>
    /// <param name="number"></param>
    /// <returns></returns>
    public static string GetStringInFrontOfAFew(string str, int number)
    {
        char[] strArray = str.ToCharArray();

        string newStr = "";
        int index = number;
        if (number >= strArray.Length)
        {
            index = strArray.Length;
        }


        for (int i = 0; i < index; i++)
        {
            newStr += strArray[i];
        }

        return newStr;
    }
    /// <summary>
    /// 获取剩余时间
    /// </summary>
    /// <param name="remainTime"></param>
    /// <returns></returns>
    public static string GetTimeRemaining(float remainTime)
    {
        float RemainTime = remainTime;
        //天
        int day = (int)(RemainTime / 86400);
        //小时
        int h = (int)(RemainTime / 3600);
        RemainTime = RemainTime % 3600;
        //分
        int m = (int)(RemainTime / 60);
        //秒
        RemainTime = RemainTime % 60;
        int s = (int)RemainTime;

        string showStr = "";
        if (day > 1)
        {
            showStr = StaticData.GetMultilingual(120167) + day + StaticData.GetMultilingual(120168);
        }
        else
        {
            if (h > 1)
            {
                showStr = StaticData.GetMultilingual(120167) + h + StaticData.GetMultilingual(120169);
            }
            else if (m > 1)
            {
                showStr = StaticData.GetMultilingual(120167) + m + StaticData.GetMultilingual(120170);
            }
        }

        return showStr;
    }
    /// <summary>
    /// 获取玩家邮件数据
    /// </summary>
    /// <param name="endAction"></param>
    public static void GetMailboxData(Action<List<MailData>> endAction)
    {
        if (_isGetMaliData)
        {
            List<MailData> mailDatas = new List<MailData>();

            foreach (var item in _mailDic)
            {
                MailData mailData = new MailData();
                mailData.Initial(item.Value);
                mailDatas.Add(mailData);
            }
            endAction?.Invoke(mailDatas);
            return;
        }
        else
        {
            CSEmptyMail cSEmptyMail = new CSEmptyMail();
            List<MailData> mailDatas = new List<MailData>();
            ProtocalManager.Instance().SendCSEmptyMail(cSEmptyMail, (data) =>
            {
                _mailDic.Clear();
                if (data != null && data.Mail != null && data.Mail.Count > 0)
                {
                    for (int i = 0; i < data.Mail.Count; i++)
                    {
                        CSMailStruct cSMailStruct = data.Mail[i];
                        MailData mailData = new MailData();
                        mailData.Initial(cSMailStruct);
                        mailDatas.Add(mailData);
                        _mailDic.Add(cSMailStruct.MailId, cSMailStruct);
                    }
                    _isGetMaliData = true;
                }
                else
                {

                    //for (int i = 0; i < 10; i++)
                    //{
                    //    CSMailStruct cSMailStruct = new CSMailStruct();
                    //    cSMailStruct.Addresser = 1;
                    //    if (i == 0)
                    //    {
                    //        cSMailStruct.HaveAccessory = true;
                    //    }
                    //    else
                    //    {
                    //        cSMailStruct.HaveAccessory = false;
                    //    }
                    //    cSMailStruct.Message = "测试数据";
                    //    cSMailStruct.MailId = i;
                    //    cSMailStruct.Title = "测试邮件" + i;
                    //    cSMailStruct.State = 1;
                    //    cSMailStruct.DisabledTime = TimeHelper.ServerTimeStampNow;

                    //    MailData mailData = new MailData();
                    //    mailData.Initial(cSMailStruct);
                    //    mailDatas.Add(mailData);
                    //    _mailDic.Add(i, cSMailStruct);
                    //}
                    //_isGetMaliData = true;

                    Debug.Log("没有邮件！");
                }
                endAction?.Invoke(mailDatas);
            }, (er) =>
            {
                Debug.LogError("邮件数据获取失败：" + "Code:" + er.webErrorCode + "Message:" + er.ErrorMessage);
                _isGetMaliData = false;
                endAction?.Invoke(mailDatas);
            });

        }


    }
    /// <summary>
    /// 获取邮件附件信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="endAction"></param>
    public static void GetMailAccessory(MailData mailData, Action<MailData, List<CSMailAccessory>> endAction)
    {
        if (_pushMailDic.ContainsKey(mailData._mailID))
        {
            RepeatedField<SCPropStruct> accessorys = _pushMailDic[mailData._mailID].PropInfo;
            List<CSMailAccessory> cSMailAccessories = new List<CSMailAccessory>();
            for (int i = 0; i < accessorys.Count; i++)
            {
                SCPropStruct accessory = accessorys[i];
                CSMailAccessory cSMailAccessory = new CSMailAccessory();
                cSMailAccessory.GoodsId = accessory.GoodsId;
                cSMailAccessory.GoodsNum = accessory.GoodsNum;
                cSMailAccessories.Add(cSMailAccessory);
            }
            endAction?.Invoke(mailData, cSMailAccessories);
        }
        else
        {
            CSAccessoryInfo cSAccessoryInfo = new CSAccessoryInfo();
            cSAccessoryInfo.MailId = mailData._mailID;
            ProtocalManager.Instance().SendCSAccessoryInfo(cSAccessoryInfo, (data) =>
            {
                Debug.Log("获取信件附件信息成功");
                List<CSMailAccessory> cSMailAccessories = new List<CSMailAccessory>();
                if (data == null)
                {

                    //CSMailAccessory cSMailAccessory = new CSMailAccessory();
                    //cSMailAccessory.GoodsId = 1000001;
                    //cSMailAccessory.GoodsNum = 10;
                    //cSMailAccessories.Add(cSMailAccessory);

                    //CSMailAccessory cSMailAccessory1 = new CSMailAccessory();
                    //cSMailAccessory1.GoodsId = 1000002;
                    //cSMailAccessory1.GoodsNum = 12;
                    //cSMailAccessories.Add(cSMailAccessory1);

                    //CSMailAccessory cSMailAccessory2 = new CSMailAccessory();
                    //cSMailAccessory2.GoodsId = 2000002;
                    //cSMailAccessory2.GoodsNum = 12;
                    //cSMailAccessories.Add(cSMailAccessory2);

                    //CSMailAccessory cSMailAccessory3 = new CSMailAccessory();
                    //cSMailAccessory3.GoodsId = 2000003;
                    //cSMailAccessory3.GoodsNum = 12;
                    //cSMailAccessories.Add(cSMailAccessory3);

                    //CSMailAccessory cSMailAccessory4 = new CSMailAccessory();
                    //cSMailAccessory4.GoodsId = 2000005;
                    //cSMailAccessory4.GoodsNum = 20;
                    //cSMailAccessories.Add(cSMailAccessory4);

                }
                else
                {
                    for (int i = 0; i < data.Goods.Count; i++)
                    {
                        cSMailAccessories.Add(data.Goods[i]);
                    }
                }

                endAction?.Invoke(mailData, cSMailAccessories);

            }, (er) =>
            {
                Debug.Log("获取信件附件信息失败");
            });
        }

    }
    /// <summary>
    /// 领取邮件中的附件
    /// </summary>
    /// <param name="id"></param>
    /// <param name="endActionk"></param>
    public static void GetMailAccessoryInWareHouse(int id, Action<List<int>, List<CSMailAccessory>> endActionk)
    {
        //领取附件
        CSAccessoryInWarehouse cSAccessoryInWarehouse = new CSAccessoryInWarehouse();
        cSAccessoryInWarehouse.MailId = id;
        ProtocalManager.Instance().SendCSAccessoryInWarehouse(cSAccessoryInWarehouse, (data) =>
        {
            Debug.Log("领取成功");


            List<int> ids = new List<int>();
            for (int i = 0; i < data.MailId.Count; i++)
            {
                ids.Add(data.MailId[i]);
            }

            List<CSMailAccessory> cSMailAccessories = new List<CSMailAccessory>();
            for (int i = 0; i < data.GoodsInfo.Count; i++)
            {
                cSMailAccessories.Add(data.GoodsInfo[i]);
            }
            List<CSWareHouseStruct> datas = new List<CSWareHouseStruct>();
            for (int i = 0; i < cSMailAccessories.Count; i++)
            {
                CSWareHouseStruct wareHouseData = new CSWareHouseStruct();
                wareHouseData.GoodId = cSMailAccessories[i].GoodsId;
                wareHouseData.GoodNum = cSMailAccessories[i].GoodsNum;
                datas.Add(wareHouseData);
            }

            if (datas != null && datas.Count > 0)
            {
                StaticData.OpenCommonReceiveAwardTips(StaticData.GetMultilingual(120265), StaticData.GetMultilingual(120119), "", () =>
                {
                    Debug.Log("弹窗结束");
                }, null, datas);
            }

            GameSoundPlayer.Instance.PlaySoundEffect(MusicHelper.SoundEffectEarnRewards);
            endActionk?.Invoke(ids, cSMailAccessories);



        }, (er) =>
        {
            Debug.Log("领取失败Code:" + er.webErrorCode + "Message:" + er.ErrorMessage);
        });
    }
    /// <summary>
    /// 打开信件
    /// </summary>
    /// <param name="mailData"></param>
    /// <returns></returns>
    public static void OpenMail(MailData mailData)
    {

        if (mailData._isHaveAccessory)
        {
            GetMailAccessory(mailData, CreateMail);
        }
        else
        {
            CreateMail(mailData);
        }

    }
    /// <summary>
    /// 修改信件状态
    /// </summary>
    /// <param name="mailDic"></param>
    public static void ChangeMailType(Dictionary<int, MailData> mailDic, Action endAction)
    {
        List<MailData> mailDatas = new List<MailData>();
        foreach (var item in mailDic)
        {
            if (_mailDic.ContainsKey(item.Key))
            {
                if ((MailState)_mailDic[item.Key].State != item.Value._type)
                {
                    mailDatas.Add(item.Value);
                }
            }
        }
        if (mailDatas != null && mailDatas.Count > 0)
        {
            CSChangeMailState cSChangeMailState = new CSChangeMailState();
            for (int i = 0; i < mailDatas.Count; i++)
            {
                MailData mailData = mailDatas[i];
                CSMailStateStruct cSMailStateStruct = new CSMailStateStruct();
                cSMailStateStruct.MailId = mailData._mailID;
                cSMailStateStruct.MailState = (int)mailData._type;
                cSChangeMailState.MailInfo.Add(cSMailStateStruct);
            }
            ProtocalManager.Instance().SendCSChangeMailState(cSChangeMailState, (data) =>
            {
                Debug.Log("修改成功");
                //修改本地数据
                for (int i = 0; i < mailDatas.Count; i++)
                {
                    MailData madata = mailDatas[i];
                    if (_mailDic.ContainsKey(madata._mailID))
                    {
                        if (madata._type == MailState.DeleteState)
                        {
                            _mailDic.Remove(madata._mailID);
                        }
                        else
                        {
                            _mailDic[madata._mailID].State = (int)madata._type;
                        }
                    }
                }

                endAction?.Invoke();
            }, (er) =>
            {
                Debug.Log("修改失败Code:" + er.webErrorCode + "Message:" + er.ErrorMessage);
            });
        }
        else
        {
            endAction?.Invoke();
        }



    }
    /// <summary>
    /// 创建信件
    /// </summary>
    /// <param name="mailData"></param>
    /// <param name="cSMailAccessories"></param>
    /// <returns></returns>
    public static async void CreateMail(MailData mailData, List<CSMailAccessory> cSMailAccessories = null)
    {
        GameObject obj = await UIComponent.CreateUIAsync(UIType.Mail);
        MailController mailController = obj.GetComponent<MailController>();
        mailController.ShowMail(mailData, cSMailAccessories);
    }
    /// <summary>
    /// 移动ui到指定位置
    /// </summary>
    /// <param name="tage"></param>
    /// <param name="tagePoint"></param>
    /// <param name="endAction"></param>
    public static IEnumerator MoveUI(Transform tage, Vector3 tagePoint, Action endAction, float speed)
    {
        while (Vector3.Distance(tage.localPosition, tagePoint) > 3f)
        {
            tage.localPosition = Vector3.MoveTowards(tage.localPosition, tagePoint, Time.deltaTime * speed);
            yield return new WaitForSeconds(Time.deltaTime * 0.01f);
        }
        endAction?.Invoke();
    }
    /// <summary>
    /// 移动ui到指定位置
    /// </summary>
    /// <param name="tage"></param>
    /// <param name="tagePoint"></param>
    /// <param name="endAction"></param>
    public static IEnumerator MoveUIs(List<Transform> tages, List<Vector3> tagePoints, Action endAction, float speed)
    {
        while (Vector3.Distance(tages[0].localPosition, tagePoints[0]) > 3f)
        {
            for (int i = 0; i < tages.Count; i++)
            {
                tages[i].localPosition = Vector3.MoveTowards(tages[i].localPosition, tagePoints[i], Time.deltaTime * speed);
            }
            yield return new WaitForSeconds(Time.deltaTime * 0.01f);
        }
        endAction?.Invoke();
    }
    /// <summary>
    /// 移动ui到指定位置
    /// </summary>
    /// <param name="tage"></param>
    /// <param name="tagePoint"></param>
    /// <param name="endAction"></param>
    public static IEnumerator MoveUITwo(Transform tage, Vector3 tagePoint, Action endAction, float speed, float triggerValue)
    {
        bool _isTrigger = false;
        while (tage != null && Vector3.Distance(tage.localPosition, tagePoint) > 3f)
        {
            tage.localPosition = Vector3.MoveTowards(tage.localPosition, tagePoint, Time.deltaTime * speed);
            if (Vector3.Distance(tage.localPosition, tagePoint) <= triggerValue && !_isTrigger)
            {
                endAction?.Invoke();
                _isTrigger = true;
            }
            yield return new WaitForSeconds(Time.deltaTime * 0.01f);
        }
    }
    /// <summary>
    /// 修改ui 尺寸
    /// </summary>
    /// <param name="tage"></param>
    /// <param name="scale"></param>
    /// <param name="endAction"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    public static IEnumerator ChangScale(Transform tage, Vector3 scale, Action endAction, float speed)
    {

        while (Vector3.Distance(tage.localScale, scale) > 0.1f)
        {
            tage.localScale = Vector3.MoveTowards(tage.localScale, scale, Time.deltaTime * speed);
            yield return new WaitForSeconds(Time.deltaTime * 0.01f);
        }
        endAction?.Invoke();
    }
    /// <summary>
    /// 修改ui 尺寸
    /// </summary>
    /// <param name="tage"></param>
    /// <param name="scale"></param>
    /// <param name="endAction"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    public static IEnumerator ChangScale(List<Transform> tage, Vector3 scale, Action endAction, float speed)
    {

        while (Vector3.Distance(tage[0].localScale, scale) > 0.1f)
        {
            for (int i = 0; i < tage.Count; i++)
            {
                tage[i].localScale = Vector3.MoveTowards(tage[i].localScale, scale, Time.deltaTime * speed);
            }

            yield return new WaitForSeconds(Time.deltaTime * 0.01f);
        }
        endAction?.Invoke();
    }
    ///// <summary>  
    ///// 时间戳Timestamp转换成日期  
    ///// </summary>  
    ///// <param name="timeStamp"></param>  
    ///// <returns></returns>  
    //public static DateTime GetDateTime(long timeStamp)
    //{
    //    DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
    //    long lTime = timeStamp;// ((long)timeStamp * 10000000);
    //    TimeSpan toNow = new TimeSpan(lTime);
    //    DateTime targetDt = dtStart.Add(toNow);
    //    return targetDt;
    //}
    /// <summary> 
    /// 时间戳转为C#格式时间 
    /// </summary> 
    /// <param name="timeStamp">Unix时间戳格式</param> 
    /// <returns>C#格式时间</returns> 
    public static DateTime GetDateTime(long timeStamp)
    {
        DateTime time = new DateTime();
        try
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            time = dtStart.Add(toNow);
        }
        catch
        {
            time = DateTime.Now.AddDays(-30);
        }
        return time;
    }
    /// <summary>  
    /// 时间戳Timestamp转换成日期  
    /// </summary>  
    /// <param name="timeStamp"></param>  
    /// <returns></returns>  
    public static DateTime GetDateTime(string timeStamp)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        long lTime = long.Parse(timeStamp + "0000000");
        TimeSpan toNow = new TimeSpan(lTime);
        DateTime targetDt = dtStart.Add(toNow);
        return dtStart.Add(toNow);
    }
    /// <summary>
    /// 初始更新任务标签
    /// </summary>
    public static void InitialUpdateTaskTag()
    {
        GetMailboxData(InitialUpdateTaskTagAction);
    }
    /// <summary>
    /// 初始更新标签回调
    /// </summary>
    /// <param name="isSucceed"></param>
    /// <param name="datas"></param>
    public static void InitialUpdateTaskTagAction(List<MailData> mailDatas)
    {
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Mailbox);
    }

    /// <summary>
    /// 是否开关红点
    /// </summary>
    /// <returns></returns>
    public static bool IsOpenDot()
    {
        _isOpenMailTag = false;
        foreach (var item in _mailDic)
        {
            if ((MailState)item.Value.State == MailState.UnReadState || (MailState)item.Value.State == MailState.ReadUnAlreadyState)
            {
                _isOpenMailTag = true;
            }
        }
        return _isOpenMailTag;
    }
    /// <summary>
    /// 注册邮件推送回调
    /// </summary>
    /// <param name="action"></param>
    public static void RegisterPushAction(Action action)
    {
        _pushAction = action;
    }
}
/// <summary>
/// 邮件数据
/// </summary>
public class MailData
{
    /// <summary>
    /// 邮件id
    /// </summary>
    public int _mailID;
    /// <summary>
    /// 邮件标题
    /// </summary>
    public string _mailName;
    /// <summary>
    /// 邮件副标题
    /// </summary>
    public string _subhead;
    /// <summary>
    /// 发件人
    /// 1：系统 2：管理员
    /// </summary>
    public int _addressor;
    /// <summary>
    /// 邮件到期时间
    /// </summary>
    public long _timestamp;
    /// <summary>
    /// 邮件内容
    /// </summary>
    public string _mailContent;
    /// <summary>
    /// 是否拥有附件
    /// </summary>
    public bool _isHaveAccessory;
    /// <summary>
    /// 邮件状态
    /// 1：未读 2：已读未领取 3：已读以领取 4：删除
    /// </summary>
    public MailState _type;
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="data"></param>
    public void Initial(CSMailStruct data)
    {
        if (data.ConfigId != 0)
        {
            MailDefine mailDefine = StaticData.configExcel.GetMailByMailId(data.ConfigId);
            if (mailDefine != null)
            {
                _mailName = StaticData.GetMultilingual(mailDefine.Title);
                _mailContent = StaticData.GetMultilingual(mailDefine.Message);
                _subhead = StaticData.GetMultilingual(mailDefine.Subtitle);
            }
            else
            {
                _mailName = "获取不到配置数据,邮件id：" + data.MailId;
                _mailContent = "获取不到配置数据,邮件配置id：" + data.ConfigId;
                _subhead = "获取不到副标题";
            }
        }
        else
        {
            _mailName = StaticData.GetMultilingual(data.Title);
            _mailContent = StaticData.GetMultilingual(data.Message);
            _subhead = StaticData.GetMultilingual(data.SubheadID); ;

        }
        _mailID = data.MailId;

        _timestamp = data.DisabledTime;

        _addressor = data.Addresser;
        _type = (MailState)data.State;
        if (data.HaveAccessory == false)
        {
            _isHaveAccessory = false;
        }
        else
        {
            _isHaveAccessory = data.HaveAccessory;
        }

    }
}
/// <summary>
/// 邮件附件Item
/// </summary>
public class MailAccessoryItem
{
    #region 字段
    Transform _itemTra;
    /// <summary>
    /// icon
    /// </summary>
    Image _iconImage;
    /// <summary>
    /// 文字显示
    /// </summary>
    Text _showText;
    /// <summary>
    /// 数量显示
    /// </summary>
    Text _number;
    /// <summary>
    /// item数据
    /// </summary>
    CSMailAccessory _data;
    /// <summary>
    /// 道具配置
    /// </summary>
    GameItemDefine _gameItemDefine;
    /// <summary>
    /// ui显示控制
    /// </summary>
    CanvasGroup _canvasGroup;
    /// <summary>
    /// 飞入item
    /// </summary>
    Transform _iconBoxTwo;
    Image _iconTwo;
    Transform _tageRight;
    Transform _tageLeft;
    /// <summary>
    /// 是否已经初始化
    /// </summary>
    bool _isInitial = false;
    float _moveSpeed = 300f;
    #endregion
    #region 属性
    /// <summary>
    /// item数据
    /// </summary>
    public CSMailAccessory _Data { get { return _data; } }

    public Transform _iconTwoTra { get { return _iconTwo.transform; } }
    #endregion
    #region 函数

    public void Initial(Transform tra)
    {
        _itemTra = tra;
        _iconImage = _itemTra.Find("Box/IconBox/Icon").GetComponent<Image>();
        _showText = _itemTra.Find("Box/NameBox/Name").GetComponent<Text>();
        _number = _itemTra.Find("Box/Number").GetComponent<Text>();
        _canvasGroup = _itemTra.Find("Box").GetComponent<CanvasGroup>();
        _iconBoxTwo = _itemTra.Find("AccessoryItemIcon");
        _iconTwo = _iconBoxTwo.Find("Icon").GetComponent<Image>();

        _isInitial = true;
    }

    /// <summary>
    /// 初始化item
    /// </summary>
    /// <param name="data"></param>
    public async void Show(Transform itemTra, CSMailAccessory data, Transform tageRight, Transform tageLeft, bool isGet)
    {
        if (!_isInitial)
        {
            Initial(itemTra);
        }
        _data = data;
        _gameItemDefine = WarehouseTool.GetGameItemData(_data.GoodsId);
        _showText.text = StaticData.GetMultilingual(_gameItemDefine.ItemName);
        _number.text = "X" + _data.GoodsNum;
        _tageRight = tageRight;
        _tageLeft = tageLeft;
        //if (isGet)
        //{
        //    _showText.text = "已领取";//StaticData.GetMultilingual(_gameItemDefine.ItemName);
        //}
        //else
        //{
        //    _showText.text = StaticData.GetMultilingual(_gameItemDefine.ItemName);
        //}


        _iconImage.sprite = null;
        _iconImage.sprite = await ABManager.GetAssetAsync<Sprite>(_gameItemDefine.Icon);
        //_iconImage.SetNativeSize();
        _iconTwo.sprite = null;
        _iconTwo.sprite = _iconImage.sprite;
        _iconTwo.SetNativeSize();

        _itemTra.gameObject.SetActive(true);
        ResetAccessoryItemIconPoint();
        Show(false);
    }
    /// <summary>
    /// 刷新item显示
    /// </summary>
    public void RefreshAccessoryItem(bool isGet)
    {
        //if (isGet)
        //{
        //    _showText.text = "已领取";//StaticData.GetMultilingual(_gameItemDefine.ItemName);
        //}
        //else
        //{
        //    _showText.text = StaticData.GetMultilingual(_gameItemDefine.ItemName);
        //}
    }
    /// <summary>
    /// 是否显示
    /// </summary>
    /// <param name="isShow"></param>
    public void Show(bool isShow)
    {
        if (isShow)
        {
            _canvasGroup.alpha = 1;
        }
        else
        {
            _canvasGroup.alpha = 0;
        }

    }
    /// <summary>
    /// 销毁
    /// </summary>
    public void Destroy()
    {
        GameObject.Destroy(_itemTra.gameObject);
        GameObject.Destroy(_iconBoxTwo.gameObject);
        GameObject.Destroy(_iconTwo.gameObject);
        _data = null;
        _gameItemDefine = null;
    }
    /// <summary>
    /// icon移动
    /// </summary>
    /// <param name="endAction"></param>
    /// <returns></returns>
    public IEnumerator MoveIcon(Action endAction)
    {
        _iconBoxTwo.SetParent(_itemTra);
        _iconBoxTwo.gameObject.SetActive(true);
        while (Vector3.Distance(_iconBoxTwo.localPosition, Vector3.zero) > 0.1f)
        {
            _iconBoxTwo.transform.localPosition = Vector3.MoveTowards(_iconBoxTwo.localPosition, Vector3.zero, _moveSpeed);
            yield return new WaitForSeconds(0.01f);
        }
        endAction?.Invoke();
        _iconBoxTwo.gameObject.SetActive(false);
        Show(true);
    }
    /// <summary>
    /// icon移动
    /// </summary>
    /// <param name="endAction"></param>
    /// <returns></returns>
    public IEnumerator QuitMoveIcon(Action endAction)
    {
        _iconBoxTwo.SetParent(_tageLeft);
        _iconBoxTwo.gameObject.SetActive(true);
        Show(false);
        while (Vector3.Distance(_iconBoxTwo.localPosition, Vector3.zero) > 0.1f)
        {
            _iconBoxTwo.transform.localPosition = Vector3.MoveTowards(_iconBoxTwo.localPosition, Vector3.zero, _moveSpeed);
            yield return new WaitForSeconds(0.01f);
        }
        endAction?.Invoke();
        _iconBoxTwo.gameObject.SetActive(false);

    }
    public void ResetAccessoryItemIconPoint()
    {
        _iconBoxTwo.SetParent(_tageRight);
        _iconBoxTwo.localPosition = Vector3.zero;
    }
    #endregion
}

