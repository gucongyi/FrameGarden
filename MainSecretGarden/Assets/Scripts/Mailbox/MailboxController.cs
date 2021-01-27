using Cysharp.Threading.Tasks;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 邮箱控制器
/// 2020/10/19 huangjiangdong
/// </summary>
public class MailboxController : MonoBehaviour
{
    #region 字段
    public static MailboxController Instance;
    /// <summary>
    /// ui整体控制器
    /// </summary>
    CanvasGroup _uiIntegralController;
    /// <summary>
    /// 滑动组件
    /// </summary>
    LoopScrollRect _loopScrollRect;
    /// <summary>
    /// 邮箱计时器
    /// </summary>
    MailBoxTimeController _mailBoxTimeController;
    /// <summary>
    /// 顶部栏
    /// </summary>
    RectTransform _topRectTra;
    /// <summary>
    /// 退出按钮
    /// </summary>
    Button _outBtn;
    /// <summary>
    /// 标题
    /// </summary>
    Transform _titleBgTra;
    /// <summary>
    /// 标题Text
    /// </summary>
    Text _titleText;
    /// <summary>
    /// 内容栏
    /// </summary>
    RectTransform _contentRectTra;
    /// <summary>
    /// 底部栏
    /// </summary>
    RectTransform _bottomRectTra;
    /// <summary>
    /// 一键删除按钮
    /// </summary>
    Button _akeyToDelete;
    /// <summary>
    /// 一键领取
    /// </summary>
    Button _akeyToGet;
    /// <summary>
    /// 彩条
    /// </summary>
    Transform _colourBar;
    /// <summary>
    /// 所有邮件集合
    /// </summary>
    List<MailData> _mailDatas = new List<MailData>();
    /// <summary>
    /// 根据邮件id储存的数据
    /// </summary>
    Dictionary<int, MailData> _mailDataDic = new Dictionary<int, MailData>();
    /// <summary>
    /// 所有邮件剩余时间字典
    /// </summary>
    Dictionary<int, float> _mailRemainingDic = new Dictionary<int, float>();
    [SerializeField]
    List<Sprite> _btnSprite = new List<Sprite>();

    float _animationSpeed = 8000f;
    /// <summary>
    /// 是否初始化完毕
    /// </summary>
    bool _isInitial = false;
    #endregion
    #region 函数
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!_isInitial)
        {
            Initial();
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Show()
    {
        if (!_isInitial)
        {
            Initial();
        }
        MailboxTool.RegisterPushAction(PushData);

        MailboxTool.GetMailboxData(GetMailData);

    }
    /// <summary>
    /// 有推送邮件时的更新
    /// </summary>
    public void PushData()
    {
        MailboxTool.GetMailboxData(GetMailDataUpdate);
    }
    /// <summary>
    /// 设置底部按钮样式
    /// </summary>
    public void SetBtnSprite()
    {
        List<MailData> deletes = GetDeleteMails();
        if (deletes == null || deletes.Count <= 0)
        {
            _akeyToDelete.transform.GetComponent<Image>().sprite = _btnSprite[0];
            _akeyToDelete.enabled = false;
        }
        else
        {
            _akeyToDelete.transform.GetComponent<Image>().sprite = _btnSprite[1];
            _akeyToDelete.enabled = true;
        }


        List<MailData> getDatas = GetAllCanGetMails();
        if (getDatas == null || getDatas.Count <= 0)
        {
            _akeyToGet.transform.GetComponent<Image>().sprite = _btnSprite[0];
            _akeyToGet.enabled = false;
        }
        else
        {
            _akeyToGet.transform.GetComponent<Image>().sprite = _btnSprite[2];
            _akeyToGet.enabled = true;
        }

    }
    /// <summary>
    /// 初始化组件
    /// </summary>
    void Initial()
    {
        _uiIntegralController = GetComponent<CanvasGroup>();
        // 邮箱计时器
        _mailBoxTimeController = transform.Find("Timer").GetComponent<MailBoxTimeController>();
        // 顶部栏
        _topRectTra = transform.Find("Top").GetComponent<RectTransform>();
        // 退出按钮
        _outBtn = _topRectTra.Find("OutBtn").GetComponent<Button>();
        // 标题
        _titleBgTra = _topRectTra.Find("TitleBg");
        // 标题Text
        _titleText = _titleBgTra.Find("Title").GetComponent<Text>();
        // 内容栏
        _contentRectTra = transform.Find("Content").GetComponent<RectTransform>();
        // 滑动组件
        _loopScrollRect = _contentRectTra.GetComponent<LoopScrollRect>();
        // 底部栏
        _bottomRectTra = transform.Find("Bottom").GetComponent<RectTransform>();
        // 一键删除按钮
        _akeyToDelete = _bottomRectTra.Find("AkeyToDelete").GetComponent<Button>();
        //一键领取按钮
        _akeyToGet = _bottomRectTra.Find("AkeyToGet").GetComponent<Button>();
        //彩条
        _colourBar = transform.Find("ColourBar");

        _outBtn.onClick.RemoveListener(OnCliclOut);
        _outBtn.onClick.AddListener(OnCliclOut);

        _akeyToDelete.onClick.RemoveListener(OnClickAKeyToDeleteBtn);
        _akeyToDelete.onClick.AddListener(OnClickAKeyToDeleteBtn);
        _akeyToGet.onClick.RemoveListener(OnClickAKeyToToGetBtn);
        _akeyToGet.onClick.AddListener(OnClickAKeyToToGetBtn);

        SetPanelMultilingual();
        _isInitial = true;
    }
    /// <summary>
    /// 一键领取
    /// </summary>
    private void OnClickAKeyToToGetBtn()
    {
        List<MailData> mailDatas = GetAllCanGetMails();
        if (mailDatas == null || mailDatas.Count <= 0)
        {
            return;
        }
        MailboxTool.GetMailAccessoryInWareHouse(0, GetMailAccessoryInWareHouseAction);
    }
    /// <summary>
    /// 获取全部可领取邮件数据
    /// </summary>
    /// <returns></returns>
    public List<MailData> GetAllCanGetMails()
    {
        List<MailData> mailDatas = new List<MailData>();
        if (_mailDatas != null && _mailDatas.Count > 0)
        {
            for (int i = 0; i < _mailDatas.Count; i++)
            {
                if (_mailDatas[i]._isHaveAccessory && (_mailDatas[i]._type == MailState.ReadUnAlreadyState || _mailDatas[i]._type == MailState.UnReadState))
                {
                    mailDatas.Add(_mailDatas[i]);
                }
            }
        }
        return mailDatas;
    }
    /// <summary>
    /// 领取附件回调
    /// </summary>
    void GetMailAccessoryInWareHouseAction(List<int> ids, List<CSMailAccessory> cSMails)
    {
        //List<MailData> mailDatas = new List<MailData>();

        //for (int i = 0; i < ids.Count; i++)
        //{
        //    RefreshMailType(ids[i], 3);
        //}
        RefreshMailType(ids, MailState.ReadAlreadyState);
        //更新玩家仓库数据
        for (int i = 0; i < cSMails.Count; i++)
        {
            StaticData.UpdateWareHouseItem(cSMails[i].GoodsId, cSMails[i].GoodsNum);
        }
    }
    /// <summary>
    /// 一键删除
    /// </summary>
    private void OnClickAKeyToDeleteBtn()
    {
        List<MailData> mailDatas = GetDeleteMails();


        if (mailDatas != null && mailDatas.Count > 0)
        {
            RefreshMailType(mailDatas, MailState.DeleteState);
        }

    }
    /// <summary>
    /// 获取全部可删除邮件
    /// </summary>
    /// <returns></returns>
    public List<MailData> GetDeleteMails()
    {
        List<MailData> mailDatas = new List<MailData>();
        foreach (var item in _mailDataDic)
        {
            MailData data = item.Value;
            if (data._type == MailState.ReadAlreadyState)
            {
                mailDatas.Add(data);
            }
        }
        return mailDatas;
    }
    /// <summary>
    /// 点击退出
    /// </summary>
    private void OnCliclOut()
    {
        CloseAnimation();

    }
    /// <summary>
    /// 初始化计时器
    /// </summary>
    void InitialTimer()
    {
        if (_mailDatas != null && _mailDatas.Count > 0)
        {
            _mailBoxTimeController.gameObject.SetActive(true);
        }
        else
        {
            _mailBoxTimeController.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 获取邮件数据
    /// </summary>
    async void GetMailData(List<MailData> mailDatas)
    {
        MailboxTool._isGetMaliData = true;
        _mailDatas.Clear();

        //_mailDataDic.Clear();


        Debug.Log("获取邮件：" + mailDatas.Count + "封");
        _mailDatas.AddRange(Sort(mailDatas));
        for (int i = 0; i < _mailDatas.Count; i++)
        {
            if (!_mailDataDic.ContainsKey(_mailDatas[i]._mailID))
            {
                _mailDataDic.Add(_mailDatas[i]._mailID, _mailDatas[i]);
            }

        }

        ChangeList(_mailDatas.Count);
        await UniTask.Delay(TimeSpan.FromMilliseconds(100));
        //_akeyToDelete.gameObject.SetActive(IsShowAKeyToDeleteBtn());
        InitialTimer();
        ResetPoint();
        SetBtnSprite();
        StartAnimation();
    }
    /// <summary>
    /// 获取邮件数据
    /// </summary>
    async void GetMailDataUpdate(List<MailData> mailDatas)
    {
        MailboxTool._isGetMaliData = true;
        _mailDatas.Clear();

        Debug.Log("获取邮件：" + mailDatas.Count + "封");
        _mailDatas.AddRange(Sort(mailDatas));
        for (int i = 0; i < _mailDatas.Count; i++)
        {
            if (!_mailDataDic.ContainsKey(_mailDatas[i]._mailID))
            {
                _mailDataDic.Add(_mailDatas[i]._mailID, _mailDatas[i]);
            }

        }
        ChangeList(_mailDatas.Count);
        await UniTask.Delay(TimeSpan.FromMilliseconds(100));
        InitialTimer();
        SetBtnSprite();
    }
    /// <summary>
    /// 排序
    /// </summary>
    /// <param name="mailDatas"></param>
    /// <returns></returns>
    List<MailData> Sort(List<MailData> mailDatas)
    {

        //未读的邮件
        List<MailData> unreads = new List<MailData>();
        //已读的邮件
        List<MailData> reads = new List<MailData>();
        //未领取
        List<MailData> unclaimeds = new List<MailData>();

        List<MailData> newdatas = new List<MailData>();
        mailDatas.Sort((a, b) => a._timestamp.CompareTo(b._timestamp));



        for (int i = 0; i < mailDatas.Count; i++)
        {
            MailData mailData = mailDatas[i];
            switch (mailData._type)
            {
                case MailState.None:
                    break;
                case MailState.UnReadState:
                    unreads.Add(mailData);
                    break;
                case MailState.ReadAlreadyState:
                    reads.Add(mailData);
                    break;
                case MailState.ReadUnAlreadyState:
                    unclaimeds.Add(mailData);
                    break;
                case MailState.DeleteState:
                    Debug.Log("删除了");
                    break;
            }
            Debug.Log("时间戳：" + mailDatas[i]._timestamp);
        }

        newdatas.AddRange(unreads);
        newdatas.AddRange(unclaimeds);
        newdatas.AddRange(reads);
        return newdatas;
    }
    /// <summary>
    /// 根据下标获取邮件数据
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public MailData GetItemData(int index)
    {
        return _mailDatas[index];
    }
    /// <summary>
    /// 根据邮件id 获取邮件数据
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public MailData GetData(int id)
    {
        if (_mailDataDic.ContainsKey(id))
        {
            return _mailDataDic[id];
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// 更改列表
    /// </summary>
    /// <param name="coutn"></param>
    void ChangeList(int coutn)
    {
        _loopScrollRect.ClearCells();
        _loopScrollRect.totalCount = coutn;
        _loopScrollRect.RefillCells();
    }
    /// <summary>
    /// 设置倒计时
    /// </summary>
    /// <param name="id"></param>
    public void SetCountDown(int id, Text showText)
    {
        _mailBoxTimeController.SetCountDown(id, showText);
    }
    /// <summary>
    /// 关闭倒计时
    /// </summary>
    /// <param name="id"></param>
    public void CloseCountDown(int id)
    {
        _mailBoxTimeController.CloseCountDown(id);
    }
    /// <summary>
    /// 打开邮件
    /// </summary>
    public void OpenMail(int mailId)
    {
        if (_mailDataDic.ContainsKey(mailId))
        {
            _titleBgTra.gameObject.SetActive(false);
            MoveMail(-1120f, false, () => { FollowUpAnimation(new Vector3(-1242f, _colourBar.localPosition.y), Vector3.zero, () => { MailboxTool.OpenMail(_mailDataDic[mailId]); }); });
            //StartCoroutine(MailboxTool.MoveUI(_topRectTra, new Vector3(-1242f, _topRectTra.localPosition.y), () => { MoveMail(-1120f, false, () => { FollowUpAnimation(new Vector3(-1242f, _colourBar.localPosition.y), Vector3.zero, () => { MailboxTool.OpenMail(_mailDataDic[mailId]); }); }); }, _animationSpeed));
        }
    }
    /// <summary>
    /// 刷新信件item信息
    /// </summary>
    /// <param name="id"></param>
    public void RefreshMailType(int id, MailState type)
    {
        if (_mailDataDic.ContainsKey(id))
        {
            List<MailData> mailDatas = new List<MailData>();
            MailData mailData = _mailDataDic[id];
            mailData._type = type;

            if (type == MailState.DeleteState)
            {
                _mailDatas.Remove(mailData);
                _mailBoxTimeController.CloseCountDown(mailData._mailID);
            }
            for (int i = 0; i < _mailDatas.Count; i++)
            {
                mailDatas.Add(_mailDataDic[_mailDatas[i]._mailID]);
            }

            GetMailData(mailDatas);
        }

    }
    /// <summary>
    /// 刷新信件item信息
    /// </summary>
    /// <param name="id"></param>
    public void RefreshMailType(List<int> ids, MailState type)
    {

        for (int i = 0; i < ids.Count; i++)
        {
            int id = ids[i];
            if (_mailDataDic.ContainsKey(id))
            {

                MailData mailData = _mailDataDic[id];
                mailData._type = type;
                if (type == MailState.DeleteState)
                {
                    _mailDatas.Remove(mailData);
                    _mailBoxTimeController.CloseCountDown(mailData._mailID);
                }
            }
        }

        List<MailData> mailDatas = new List<MailData>();
        for (int i = 0; i < _mailDatas.Count; i++)
        {
            mailDatas.Add(_mailDataDic[_mailDatas[i]._mailID]);
        }

        GetMailData(mailDatas);
    }
    /// <summary>
    /// 刷新信件item信息
    /// </summary>
    /// <param name="id"></param>
    public void RefreshMailType(List<MailData> mailDatas, MailState type)
    {
        if (mailDatas == null || mailDatas.Count <= 0)
        {
            return;
        }
        List<MailData> mailDatasNew = new List<MailData>();
        for (int i = 0; i < mailDatas.Count; i++)
        {
            MailData data = mailDatas[i];
            if (_mailDataDic.ContainsKey(data._mailID))
            {

                MailData mailData = _mailDataDic[data._mailID];
                mailData._type = type;

                if (type == MailState.DeleteState)
                {
                    _mailDatas.Remove(mailData);
                    _mailBoxTimeController.CloseCountDown(mailData._mailID);
                }
            }
        }

        for (int i = 0; i < _mailDatas.Count; i++)
        {
            mailDatasNew.Add(_mailDatas[i]);
        }
        GetMailData(mailDatasNew);


    }
    /// <summary>
    /// 是否显示一键删除
    /// </summary>
    /// <returns></returns>
    bool IsShowAKeyToDeleteBtn()
    {
        for (int i = 0; i < _mailDatas.Count; i++)
        {
            if (_mailDatas[i]._type == MailState.ReadAlreadyState)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 重置ui位置便于开始动画播放
    /// </summary>
    void ResetPoint()
    {
        _uiIntegralController.interactable = false;
        //_topRectTra.localPosition = new Vector3(1242f, _topRectTra.localPosition.y);
        _topRectTra.gameObject.SetActive(false);
        Transform cont = _contentRectTra.Find("Content");
        cont.localPosition = new Vector3(1120f, cont.localPosition.y);
        _colourBar.localPosition = new Vector3(1242f, _colourBar.localPosition.y);
        ContentSizeFitter contentSizeFitter = cont.GetComponent<ContentSizeFitter>();
        VerticalLayoutGroup verticalLayoutGroup = cont.GetComponent<VerticalLayoutGroup>();
        contentSizeFitter.enabled = true;
        verticalLayoutGroup.enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(cont.GetComponent<RectTransform>());


        _akeyToDelete.transform.localScale = Vector3.zero;
        _akeyToGet.transform.localScale = Vector3.zero;
    }
    /// <summary>
    /// 开始动画
    /// </summary>
    public void StartAnimation()
    {
        _topRectTra.gameObject.SetActive(true);
        _titleBgTra.gameObject.SetActive(true);
        MoveMail(-1120f, true, () => { FollowUpAnimation(new Vector3(0, _colourBar.localPosition.y), Vector3.one, () => { _uiIntegralController.interactable = true; }); });
    }
    /// <summary>
    /// 移动信件
    /// </summary>
    /// <param name="x"></param>
    /// <param name="isRecover"></param>
    /// <param name="endAction"></param>
    void MoveMail(float x, bool isRecover, Action endAction)
    {
        Transform cont = _contentRectTra.Find("Content");
        ContentSizeFitter contentSizeFitter = cont.GetComponent<ContentSizeFitter>();
        VerticalLayoutGroup verticalLayoutGroup = cont.GetComponent<VerticalLayoutGroup>();
        contentSizeFitter.enabled = false;
        verticalLayoutGroup.enabled = false;
        List<Transform> childTra = new List<Transform>();
        for (int i = 0; i < cont.childCount; i++)
        {
            childTra.Add(cont.GetChild(i));
        }
        if (childTra != null && childTra.Count > 0)
        {
            MoveMail(childTra, x, isRecover, endAction, 0);
        }
        else
        {
            endAction?.Invoke();
        }
    }
    /// <summary>
    /// 移动邮件
    /// </summary>
    /// <param name="tages"></param>
    /// <param name="x"></param>
    /// <param name="isRecover"></param>
    /// <param name="endAction"></param>
    /// <param name="index"></param>
    void MoveMail(List<Transform> tages, float x, bool isRecover, Action endAction, int index)
    {
        if (index < tages.Count - 1)
        {
            StartCoroutine(MailboxTool.MoveUITwo(tages[index], new Vector3(x, tages[index].localPosition.y), () =>
            {
                index = index + 1;
                MoveMail(tages, x, isRecover, endAction, index);
            }, _animationSpeed, 150f));
        }
        else if (index == tages.Count - 1)
        {
            StartCoroutine(MailboxTool.MoveUI(tages[index], new Vector3(x, tages[index].localPosition.y), () =>
            {
                Transform cont = _contentRectTra.Find("Content");
                ContentSizeFitter contentSizeFitter = cont.GetComponent<ContentSizeFitter>();
                VerticalLayoutGroup verticalLayoutGroup = cont.GetComponent<VerticalLayoutGroup>();
                if (isRecover)
                {
                    cont.localPosition = new Vector3(0, cont.localPosition.y);
                    contentSizeFitter.enabled = true;
                    verticalLayoutGroup.enabled = true;
                }
                endAction?.Invoke();
            }, _animationSpeed));
            Debug.Log("最后一封");
        }
    }
    /// <summary>
    /// 移动彩条
    /// </summary>
    /// <param name="tage"></param>
    /// <param name="tageScale"></param>
    /// <param name="endAction"></param>
    void FollowUpAnimation(Vector3 tage, Vector3 tageScale, Action endAction)
    {
        StartCoroutine(MailboxTool.MoveUI(_colourBar, tage, () =>
        {
            ChangBtnScale(tageScale, endAction);
        }, _animationSpeed));
    }
    /// <summary>
    /// 修改按钮尺寸
    /// </summary>
    /// <param name="tageScale"></param>
    /// <param name="endAction"></param>
    void ChangBtnScale(Vector3 tageScale, Action endAction)
    {
        List<Transform> tras = new List<Transform>();
        tras.Add(_akeyToDelete.transform);
        tras.Add(_akeyToGet.transform);
        //StartCoroutine(MailboxTool.ChangScale(tras, tageScale, null, _animationSpeed));
        StartCoroutine(MailboxTool.ChangScale(tras, tageScale, () =>
        {
            endAction?.Invoke();
            Debug.Log("展开按钮完毕");
        }, 10f));
    }
    /// <summary>
    /// 关闭动画
    /// </summary>
    void CloseAnimation()
    {
        _topRectTra.gameObject.SetActive(false);
        MoveMail(-1120f, false, () =>
        {
            FollowUpAnimation(new Vector3(-1242f, _colourBar.localPosition.y), Vector3.zero, () =>
            {
                MailboxTool.ChangeMailType(_mailDataDic, () =>
                {
                    StopAllCoroutines();
                    MailboxTool._isGetMaliData = false;
                    RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Mailbox);
                    MailboxTool.RegisterPushAction(null);
                    UIComponent.HideUI(UIType.Mailbox);
                });
            });
        });
    }
    /// <summary>
    /// 设置多语言显示
    /// </summary>
    void SetPanelMultilingual()
    {
        _titleText.text = StaticData.GetMultilingual(120162);
        _akeyToDelete.transform.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120163);
        _akeyToGet.transform.Find("Text").GetComponent<Text>().text = StaticData.GetMultilingual(120164);
    }
    #endregion
}
