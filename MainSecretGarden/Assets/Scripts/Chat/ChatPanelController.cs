using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 聊天面板
/// </summary>
public class ChatPanelController : MonoBehaviour
{
    #region 字段
    public static ChatPanelController _Instance;
    /// <summary>
    /// 背景板
    /// </summary>
    Transform _bgTra;
    /// <summary>
    /// 关闭按钮
    /// </summary>
    Button _closeBtn;
    /// <summary>
    /// 顶部栏
    /// </summary>
    Transform _topTra;
    ToggleGroup _topToggleGroup;
    /// <summary>
    /// 顶部栏滑动组件
    /// </summary>
    ScrollRect _topScrollRect;
    /// <summary>
    /// 顶部私聊头像栏
    /// </summary>
    Transform _topTwoTra;
    ToggleGroup _topTwoToggleGroup;
    /// <summary>
    /// 顶部私聊头像栏滑动组件
    /// </summary>
    ScrollRect _topTwoScrollRect;
    /// <summary>
    /// 信息显示区域
    /// </summary>
    RectTransform _itemBoxTra;
    /// <summary>
    /// 信息显示父级
    /// </summary>
    RectTransform _itemBoxContentRect;
    /// <summary>
    /// 信息显示区域循环列表
    /// </summary>
    LoopScrollRect _loopScrollRect = null;
    /// <summary>
    /// 循环列表滑动状态检测
    /// </summary>
    ChatSlideListStateInspection _chatSlideListStateInspection;
    /// <summary>
    /// 底部栏
    /// </summary>
    Transform _bottomTra;
    /// <summary>
    /// 底部信息输入框
    /// </summary>
    InputField _bottomInputField;
    /// <summary>
    /// 发送按钮
    /// </summary>
    Button _sendBtn;
    /// <summary>
    /// 发送按钮文字显示
    /// </summary>
    Text _sendBtnText;
    /// <summary>
    /// 弹幕开关
    /// </summary>
    Toggle _barrageToggle;
    /// <summary>
    /// 弹幕开关ui组件
    /// </summary>
    RectTransform _barrageToggleRect;
    /// <summary>
    /// 弹幕开关效果球
    /// </summary>
    RectTransform _barrageToggleTageTra;
    /// <summary>
    /// 新消息提示按钮
    /// </summary>
    Button _newMessageBtn;
    /// <summary>
    /// 新消息提示按钮文字显示
    /// </summary>
    Text _newMessageBtnText;
    /// <summary>
    /// 发送按钮遮罩
    /// </summary>
    Image _sendBtnMaskImage;
    /// <summary>
    /// 顶部按钮母体
    /// </summary>
    ChatTopBtnItemController _chatTopBtnItem;
    /// <summary>
    /// 顶部按钮字典
    /// </summary>
    Dictionary<int, ChatTopBtnItemController> _chatTopBtnItemControllerDic = new Dictionary<int, ChatTopBtnItemController>();
    /// <summary>
    /// 私聊头像按钮母体
    /// </summary>
    PrivateChatIconItemController _privateChatIconItem;
    /// <summary>
    /// 私聊头像按钮字典
    /// </summary>
    Dictionary<long, PrivateChatIconItemController> _privateChatIconItemControllerDic = new Dictionary<long, PrivateChatIconItemController>();
    /// <summary>
    /// 当前频道类型
    /// </summary>
    int _currType;
    /// <summary>
    /// 当前私聊对象标记
    /// </summary>
    long _currPrivateChatIndex;
    /// <summary>
    /// 输入信息
    /// </summary>
    string _InputFieldStr;
    /// <summary>
    /// 计时器
    /// </summary>
    TimeCountDownComponent _timeCountDownComponent;
    /// <summary>
    /// 是否处于时间解锁中
    /// </summary>
    bool _isBeTimeUnlock = false;
    /// <summary>
    /// 是否已经初始化
    /// </summary>
    bool _isInitial = false;
    #endregion
    #region 属性
    /// <summary>
    /// 内容滑动组件
    /// </summary>
    public LoopScrollRect _LoopScrollRect { get { return _loopScrollRect; } }
    /// <summary>
    /// 当前私聊对象标记
    /// </summary>
    public long _CurrPrivateChatIndex { get { return _currPrivateChatIndex; } }
    /// <summary>
    /// 信息显示区域
    /// </summary>
    public RectTransform _ItemBoxTra { get { return _itemBoxTra; } }
    /// <summary>
    /// 当前频道类型
    /// </summary>
    public int _CurrType { get { return _currType; } }
    #endregion
    #region 函数
    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
        }

        if (!_isInitial)
        {
            Initial();
        }
        if (_bgTra != null)
            UniversalTool.ReadyUIAnimTwo(GetComponent<CanvasGroup>(), _bgTra);
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }
    private void OnEnable()
    {
        if (_bgTra != null)
            UniversalTool.StartUIAnimTwo(GetComponent<CanvasGroup>(), _bgTra);
    }
    private void OnDisable()
    {
        if (_bgTra != null)
            UniversalTool.ReadyUIAnimTwo(GetComponent<CanvasGroup>(), _bgTra);
    }
    /// <summary>
    /// 初始化组件
    /// </summary>
    void Initial()
    {
        _bgTra = transform.Find("bg");
        _topTra = _bgTra.Find("Top");
        _closeBtn = _topTra.Find("CloseButton").GetComponent<Button>();
        _topToggleGroup = _topTra.GetComponent<ToggleGroup>();
        _topScrollRect = _topTra.Find("Scroll View").GetComponent<ScrollRect>();
        _topTwoTra = _bgTra.Find("TopTwo");
        _topTwoToggleGroup = _topTwoTra.GetComponent<ToggleGroup>();
        _topTwoScrollRect = _topTwoTra.Find("Scroll View").GetComponent<ScrollRect>();
        _itemBoxTra = _bgTra.Find("Itembox").GetComponent<RectTransform>();
        _itemBoxContentRect = _itemBoxTra.Find("Content").GetComponent<RectTransform>();
        _loopScrollRect = _itemBoxTra.GetComponent<LoopScrollRect>();
        _chatSlideListStateInspection = _itemBoxTra.Find("Content").GetComponent<ChatSlideListStateInspection>();
        _bottomTra = _bgTra.Find("Bottom");
        _bottomInputField = _bottomTra.Find("InputField").GetComponent<InputField>();
        _sendBtn = _bottomTra.Find("SendBtn").GetComponent<Button>();
        _sendBtnMaskImage = _sendBtn.transform.Find("Mask").GetComponent<Image>();
        _sendBtnText = _sendBtn.transform.Find("Text").GetComponent<Text>();
        _barrageToggle = _bottomTra.Find("BarrageToggle").GetComponent<Toggle>();
        _barrageToggleRect = _barrageToggle.transform.GetComponent<RectTransform>();
        _barrageToggleTageTra = _barrageToggleRect.Find("Tage").GetComponent<RectTransform>();
        _newMessageBtn = _bottomTra.Find("NewMessageBtn").GetComponent<Button>();
        _newMessageBtnText = _newMessageBtn.transform.Find("Text").GetComponent<Text>();
        _chatTopBtnItem = _bgTra.Find("ChatTopBtnItem").GetComponent<ChatTopBtnItemController>();
        _privateChatIconItem = _bgTra.Find("PrivateChatIconItem").GetComponent<PrivateChatIconItemController>();

        _barrageToggle.onValueChanged.RemoveAllListeners();
        _barrageToggle.onValueChanged.AddListener(ClickBarrageToggle);

        _closeBtn.onClick.RemoveAllListeners();
        _closeBtn.onClick.AddListener(ClickCloseBtn);

        _newMessageBtn.onClick.RemoveAllListeners();
        _newMessageBtn.onClick.AddListener(ClickNewMessageBtn);

        _bottomInputField.onValueChanged.RemoveAllListeners();
        _bottomInputField.onValueChanged.AddListener(OnInputFieldChange);
        _bottomInputField.onEndEdit.RemoveAllListeners();
        _bottomInputField.onEndEdit.AddListener(OnInputField);

        _sendBtn.onClick.RemoveAllListeners();
        _sendBtn.onClick.AddListener(OnSendBtn);
        SetPanelMultilingual();
        _isInitial = true;
    }
    /// <summary>
    /// 初始化面板
    /// </summary>
    public void InitialPanel()
    {
        CleanTopDic();
        CreateTopBtn(0, StaticData.GetMultilingual(120171));
        CreateTopBtn(1, StaticData.GetMultilingual(120172));
        if (ChatTool._IsBeRoom)
        {
            CreateTopBtn(2, StaticData.GetMultilingual(120173));
        }
        _barrageToggle.isOn = ChatTool.GetBulletScreenOnOff();
        SetBarrageToggleTageTra(_barrageToggle.isOn);
        OpenNewMessageBtn(false);
        ChatTool.OpenBulletScreenOnOff(false);
        ChatTool.EnrollAction(RefreshWorldChat, RefreshRoomChat, RefreshPrivateChat);
        _chatTopBtnItemControllerDic[1].OpenUpdateLabelTra(ChatTool.IsNewMessagePrivateChat());

        long currTime = TimeHelper.ServerTimeStampNow;
        float currRemainingTime = (currTime - ChatTool._chatLeaveTime);
        currRemainingTime = currRemainingTime / 1000;
        if (currRemainingTime >= ChatTool._sendRemainingTime)
        {
            ChatTool._isOverCd = true;
            _isBeTimeUnlock = false;
            _sendBtnMaskImage.fillAmount = 0;
            if (!_sendBtn.enabled)
            {
                _sendBtn.enabled = true;
            }
        }
        else
        {
            if (_timeCountDownComponent == null)
            {
                CreationTimer();
            }
            _isBeTimeUnlock = true;
            float TimeThanColumn = ChatTool._sendRemainingTime - currRemainingTime / StaticData.configExcel.GetVertical().ChatSendCD;
            _sendBtnMaskImage.fillAmount = TimeThanColumn;
            StartCountingTime(ChatTool._sendRemainingTime - currRemainingTime);
        }

    }
    /// <summary>
    /// 创建空白计时器
    /// </summary>
    public void CreationTimer()
    {
        _timeCountDownComponent = StaticData.CreateTimer(100000 / 1000f, false, (go) =>
        {

        },
        (remainTime) =>
        {

        }, "WarehouseTreasureChest");
        _timeCountDownComponent.transform.SetParent(transform);
    }
    /// <summary>
    /// 开始计时
    /// </summary>
    public void StartCountingTime(float cdValue)
    {
        ChatTool._sendRemainingTime = 0;
        _timeCountDownComponent.InitSecondsOnFrameCallBack(cdValue, false, (go) =>
        {
            _isBeTimeUnlock = false;
            _sendBtn.enabled = true;
            ChatTool._sendRemainingTime = 0;
            Debug.Log("计时结束1");
            ChatTool._isOverCd = true;
            Destroy(_timeCountDownComponent.gameObject);
        },
        (remainTime) =>
        {
            _isBeTimeUnlock = true;
            //记录剩余时间
            ChatTool._sendRemainingTime = remainTime;

            float TimeThanColumn = ChatTool._sendRemainingTime / StaticData.configExcel.GetVertical().ChatSendCD;
            _sendBtnMaskImage.fillAmount = TimeThanColumn;

            Debug.Log("剩余时间：" + ChatTool._sendRemainingTime);

        });

    }
    /// <summary>
    /// 清理顶部按钮
    /// </summary>
    void CleanTopDic()
    {
        foreach (var item in _chatTopBtnItemControllerDic)
        {
            item.Value.Destroy();
        }
        _chatTopBtnItemControllerDic.Clear();
    }
    /// <summary>
    /// 创建顶部按钮
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    void CreateTopBtn(int index, string name)
    {
        GameObject obj = GameObject.Instantiate(_chatTopBtnItem.gameObject, _topScrollRect.content);
        ChatTopBtnItemController chatTopBtnItemController = obj.GetComponent<ChatTopBtnItemController>();
        chatTopBtnItemController.SetBtnData(index, name, _topToggleGroup);
        _chatTopBtnItemControllerDic.Add(index, chatTopBtnItemController);
    }
    /// <summary>
    /// 创建私聊头像按钮
    /// </summary>
    /// <param name="data"></param>
    void CreatePrivateChatItem(PrivateChatSaveInfo data)
    {
        if (_privateChatIconItemControllerDic.ContainsKey(data.PrivateChatRoleUid))
        {
            return;
        }
        GameObject obj = GameObject.Instantiate(_privateChatIconItem.gameObject, _topTwoScrollRect.content);
        PrivateChatIconItemController privateChatIconItemController = obj.GetComponent<PrivateChatIconItemController>();
        privateChatIconItemController.SetData(data, _topTwoToggleGroup);
        _privateChatIconItemControllerDic.Add(data.PrivateChatRoleUid, privateChatIconItemController);
    }
    /// <summary>
    /// 创建所有的私聊头像按钮
    /// </summary>
    void CreatePrivateChatItem()
    {
        CleanPrivateChatItem();
        for (int i = 0; i < ChatTool._PrivateChannelMessages.Count; i++)
        {
            CreatePrivateChatItem(ChatTool._PrivateChannelMessages[i]);
        }
    }
    /// <summary>
    /// 清理私聊头像按钮
    /// </summary>
    void CleanPrivateChatItem()
    {
        foreach (var item in _privateChatIconItemControllerDic)
        {
            item.Value.Dispose();
        }
        _privateChatIconItemControllerDic.Clear();
    }
    /// <summary>
    /// 输入聊天内容
    /// </summary>
    /// <param name="arg0"></param>
    private void OnInputField(string arg0)
    {
        Debug.Log("输入改变 :" + arg0);
        if (arg0.Length > StaticData.configExcel.GetVertical().ChatMessageLength)
        {
            _bottomInputField.text = _InputFieldStr;
            return;
        }
        _InputFieldStr = arg0;
    }
    private void OnInputFieldChange(string arg0)
    {
        //Debug.Log("输入改变 :" + arg0);
        if (arg0.Length > StaticData.configExcel.GetVertical().ChatMessageLength)
        {
            _bottomInputField.text = _InputFieldStr;
            return;
        }
        _InputFieldStr = arg0;
    }
    /// <summary>
    /// 清空输入信息
    /// </summary>
    private void ClearInputField()
    {
        _InputFieldStr = null;
        _bottomInputField.text = _InputFieldStr;
    }
    /// <summary>
    /// 发送输入信息
    /// </summary>
    private void OnSendBtn()
    {
        if (string.IsNullOrWhiteSpace(_InputFieldStr))
        {
            ClearInputField();
            return;
        }
        if (ShieldWordTool.BlockFont(ref _InputFieldStr))
        {
            Debug.Log("处理屏蔽字");
        }
        _sendBtn.enabled = false;
        if (_timeCountDownComponent == null)
        {
            CreationTimer();
        }

        _sendBtnMaskImage.fillAmount = 1;
        _isBeTimeUnlock = true;
        StartCountingTime(StaticData.configExcel.GetVertical().ChatSendCD);
        switch (_currType)
        {
            case 0:
                ChatTool.NotifyServerWorldChatMessage(_InputFieldStr, SendSuccess, SendFailed);
                break;
            case 1:
                if (_currPrivateChatIndex != 0)
                {
                    ChatTool.NotifyServerPrivateChatMessage(_currPrivateChatIndex, _InputFieldStr, PrivateSendSuccess, PrivateSendFailed);
                }
                else
                {
                    StaticData.CreateToastTips(StaticData.GetMultilingual(120261));
                }
                break;
            case 2:
                ChatTool.NotifyServerRoomChatMessage(_InputFieldStr, RoomChatSendSuccess, RoomChatSendFailed);
                break;
        }

    }
    /// <summary>
    /// 世界消息发送失败
    /// </summary>
    private void SendFailed()
    {

    }
    /// <summary>
    /// 世界消息发送成功
    /// </summary>
    private void SendSuccess()
    {
        _chatSlideListStateInspection.ResetData();
        ChatInfo chatInfo = ChatTool.GetPlayData(_InputFieldStr);
        ChatTool.EnqueuetWorldChannelMessages(chatInfo);
        ClearInputField();
    }
    /// <summary>
    /// 私聊消息发送失败
    /// </summary>
    private void PrivateSendFailed()
    {

    }
    /// <summary>
    /// 私聊消息发送成功
    /// </summary>
    private void PrivateSendSuccess()
    {
        _chatSlideListStateInspection.ResetData();
        ChatInfo chatInfo = ChatTool.GetPlayData(_InputFieldStr);
        ChatTool.ReceivePrivateChatMessage(_currPrivateChatIndex, chatInfo);
        ClearInputField();
    }
    /// <summary>
    /// 房间消息发送失败
    /// </summary>
    private void RoomChatSendFailed()
    {

    }
    /// <summary>
    /// 房间消息发送成功
    /// </summary>
    private void RoomChatSendSuccess()
    {
        _chatSlideListStateInspection.ResetData();
        ChatInfo chatInfo = ChatTool.GetPlayData(_InputFieldStr);
        ChatTool.EnqueuetRoomChannelMessages(chatInfo);
        ClearInputField();
    }
    /// <summary>
    /// 开关新消息提示按钮
    /// </summary>
    /// <param name="isOpen"></param>
    public void OpenNewMessageBtn(bool isOpen)
    {
        if (_newMessageBtn == null)
        {
            return;
        }
        _newMessageBtn.gameObject.SetActive(isOpen);
    }
    /// <summary>
    /// 点击有新消息按钮(直达底部)
    /// </summary>
    private void ClickNewMessageBtn()
    {
        _LoopScrollRect.verticalNormalizedPosition = 1;

        OpenNewMessageBtn(false);

    }
    /// <summary>
    /// 点击顶部按钮
    /// </summary>
    /// <param name="index"></param>
    public void ClickTopBtn(int index)
    {

        _currType = index;

        if (_currType != 1)
        {
            ChatTool.ClearBlankPrivateChat();
        }
        if (_currType != 0)
        {
            SetBarrageToggleOpen(false);
        }
        else
        {
            SetBarrageToggleOpen(true);
        }
        _chatSlideListStateInspection.ResetData();
        ClearInputField();
        RefreshChat();

    }
    /// <summary>
    /// 开关弹幕按钮
    /// </summary>
    /// <param name="isOpen"></param>
    void SetBarrageToggleOpen(bool isOpen)
    {
        _barrageToggle.gameObject.SetActive(isOpen);
    }
    /// <summary>
    /// 弹幕按钮点击监听
    /// </summary>
    /// <param name="arg0"></param>
    private void ClickBarrageToggle(bool arg0)
    {
        SetBarrageToggleTageTra(arg0);
    }
    /// <summary>
    /// 设置弹幕开关效果球位置
    /// </summary>
    /// <param name="isOpen"></param>
    void SetBarrageToggleTageTra(bool isOpen)
    {
        float toggX = _barrageToggleRect.rect.width / 2;
        float tageX = _barrageToggleTageTra.rect.width / 2;
        float x = 0;
        if (isOpen)
        {
            x = toggX - tageX;
        }
        else
        {
            x = -toggX + tageX;
        }
        _barrageToggleTageTra.localPosition = new Vector3(x, _barrageToggleTageTra.localPosition.y);
    }
    /// <summary>
    /// 点击私聊头像按钮
    /// </summary>
    /// <param name="uId"></param>
    public void ClickPrivateChat(long uId)
    {
        _currPrivateChatIndex = uId;
        _chatSlideListStateInspection.ResetData();
        RefreshPrivateChat();
        _chatTopBtnItemControllerDic[1].OpenUpdateLabelTra(ChatTool.IsNewMessagePrivateChat());
    }
    /// <summary>
    /// 点击私聊头像按钮
    /// </summary>
    /// <param name="uId"></param>
    public void OpenPrivateChat(long uId, int iconId, string iconUrl, string playName, int experience)
    {
        ChatTool.CreationNullPrivateChatMessage(uId, iconId, iconUrl, playName, experience);
        _currPrivateChatIndex = uId;
        _chatTopBtnItemControllerDic[1].SetSelect(true);
        if (_privateChatIconItemControllerDic.ContainsKey(uId))
        {
            _privateChatIconItemControllerDic[uId].SetSelcet(true);
        }
    }
    /// <summary>
    /// 关闭聊天面板
    /// </summary>
    private void ClickCloseBtn()
    {
        if (_timeCountDownComponent != null)
        {
            Destroy(_timeCountDownComponent.gameObject);
        }
        UniversalTool.CancelUIAnimTwo(GetComponent<CanvasGroup>(), _bgTra, () =>
        {
            UIComponent.HideUI(UIType.ChatPanel);
            _currPrivateChatIndex = 0;
            ChatTool.SetBulletScreenOnOff(_barrageToggle.isOn);
            //接收消息就存储
            ChatTool.PrivateChatSaveFile();
            //初始信息发送cd时记录离开时当前服务器时间戳
            if (_isBeTimeUnlock && ChatTool._isOverCd)
            {
                ChatTool._isOverCd = false;
                ChatTool._chatLeaveTime = TimeHelper.ServerTimeStampNow;
            }
            Debug.Log("关闭_________________________________");
        });


    }
    /// <summary>
    /// 更改列表
    /// </summary>
    /// <param name="coutn"></param>
    void ChangeList(int coutn)
    {

        if (_chatSlideListStateInspection._IsDrag)
        {
            _loopScrollRect.totalCount = coutn;
            return;
        }
        if (_loopScrollRect == null)
        {
            return;
        }
        _loopScrollRect.ClearCells();
        _loopScrollRect.totalCount = coutn;
        ChangeListSlideType(0);
    }
    void ClearList()
    {
        _loopScrollRect.ClearCells();
        _loopScrollRect.totalCount = 0;
        ChangeListSlideType(0);
    }
    /// <summary>
    /// 修改列表滑动状态
    /// </summary>
    /// <param name="type">0：始终显示最新数据 2:停留当前界面</param>
    public void ChangeListSlideType(int type)
    {
        //Debug.Log("更改列表填充状态");
        switch (type)
        {
            case 0:
                _loopScrollRect.RefillCellsFromEnd(0);
                break;
            case 1:
                _loopScrollRect.RefillCells();
                break;
        }
    }
    /// <summary>
    /// 刷新聊天界面
    /// </summary>
    void RefreshChat()
    {
        //Debug.Log("刷新界面");
        switch (_currType)
        {
            case 0:
                _topTwoTra.gameObject.SetActive(false);
                _itemBoxTra.sizeDelta = new Vector2(_itemBoxTra.sizeDelta.x, 1472);
                _itemBoxTra.localPosition = new Vector2(_itemBoxTra.localPosition.x, 69);
                ChangeList(ChatTool.GetWorkdNumber());
                break;
            case 1:
                _topTwoTra.gameObject.SetActive(true);
                _itemBoxTra.sizeDelta = new Vector2(_itemBoxTra.sizeDelta.x, 1291);
                _itemBoxTra.localPosition = new Vector2(_itemBoxTra.localPosition.x, -22);
                ClearList();
                CreatePrivateChatItem();
                break;
        }
    }
    /// <summary>
    /// 自己发送时调用
    /// </summary>
    void RefreshPrivateChat()
    {
        Debug.Log("刷新私聊界面");
        ChangeList(ChatTool.GetPrivateChatNumber(_currPrivateChatIndex));
    }
    /// <summary>
    /// 获取当前item聊天数据
    /// </summary>
    /// <param name="index"></param>
    public ChatInfo GetCurrChatItemData(int index)
    {
        switch (_currType)
        {
            case 0:
                return ChatTool.GetWorldData(index);
            case 1:
                return ChatTool.GetPrivateChatData(_currPrivateChatIndex, index);
            case 2:
                return ChatTool.GetRoomData(index);
        }
        return null;
    }
    /// <summary>
    /// 刷新世界聊天信息
    /// </summary>
    public void RefreshWorldChat()
    {
        if (_currType != 0)
        {
            _chatTopBtnItemControllerDic[0].OpenUpdateLabelTra(true);
            return;
        }
        ChangeList(ChatTool.GetWorkdNumber());
        if (_chatSlideListStateInspection._IsDrag)
        {
            OpenNewMessageBtn(true);
        }
        else
        {
            OpenNewMessageBtn(false);
        }
    }
    /// <summary>
    /// 刷新房间聊天
    /// </summary>
    public void RefreshRoomChat()
    {
        if (_currType != 2)
        {
            return;
        }
        ChangeList(ChatTool.GetRoomNumber());
        if (_chatSlideListStateInspection._IsDrag)
        {
            OpenNewMessageBtn(true);
        }
        else
        {
            OpenNewMessageBtn(false);
        }
    }
    /// <summary>
    /// 刷新私聊信息
    /// </summary>
    /// <param name="uId"></param>
    public void RefreshPrivateChat(long uId)
    {
        if (_currType != 1)
        {
            _chatTopBtnItemControllerDic[1].OpenUpdateLabelTra(true);
            return;
        }
        if (uId == _currPrivateChatIndex)
        {
            ChangeList(ChatTool.GetPrivateChatNumber(_currPrivateChatIndex));
            if (_chatSlideListStateInspection._IsDrag)
            {
                OpenNewMessageBtn(true);
            }
            else
            {
                OpenNewMessageBtn(false);
            }
        }

        if (_privateChatIconItemControllerDic.ContainsKey(uId))
        {
            _privateChatIconItemControllerDic[uId].RefreshDataShow();
        }
        PrivateChatBtnSort(uId);
        PrivateChatBtnUpdateLabel(uId);
    }
    /// <summary>
    /// 刷新私聊按钮排序
    /// </summary>
    /// <param name="uid"></param>
    public void PrivateChatBtnSort(long uid)
    {
        if (!_privateChatIconItemControllerDic.ContainsKey(uid))
        {
            CreatePrivateChatItem(ChatTool.FindMessagePrivateChat_UID(uid));
        }
        if (uid != _currPrivateChatIndex)
        {
            foreach (var item in _privateChatIconItemControllerDic)
            {
                if (item.Key == _currPrivateChatIndex)
                {
                    item.Value.transform.SetSiblingIndex(0);
                }
                else if (item.Key == uid)
                {
                    item.Value.transform.SetSiblingIndex(1);
                }
            }
        }
        else
        {
            foreach (var item in _privateChatIconItemControllerDic)
            {
                if (item.Key == _currPrivateChatIndex)
                {
                    item.Value.transform.SetSiblingIndex(0);
                }
            }
        }

    }
    /// <summary>
    /// 刷新私聊按钮标签
    /// </summary>
    /// <param name="uid"></param>
    public void PrivateChatBtnUpdateLabel(long uid)
    {
        if (uid != _currPrivateChatIndex)
        {
            _privateChatIconItemControllerDic[uid].NewMessage(true);
            bool isOpen = false;
            foreach (var item in _privateChatIconItemControllerDic)
            {
                if (!item.Value._Data.IsRead)
                {
                    isOpen = true;
                }
            }
            _chatTopBtnItemControllerDic[1].OpenUpdateLabelTra(isOpen);
        }

    }
    /// <summary>
    /// 强制刷新itembox区域
    /// </summary>
    public void CoerceItemBoxRefresh()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(_itemBoxContentRect);
        //Debug.Log("强制刷新itemBox");
    }
    /// <summary>
    /// 设置多语言显示
    /// </summary>
    void SetPanelMultilingual()
    {
        _barrageToggleRect.Find("Label").GetComponent<Text>().text = StaticData.GetMultilingual(120174);
        _barrageToggleRect.Find("OpenText").GetComponent<Text>().text = StaticData.GetMultilingual(120175);
        _barrageToggleRect.Find("CloseText").GetComponent<Text>().text = StaticData.GetMultilingual(120176);
        _newMessageBtnText.text = StaticData.GetMultilingual(120177);
        _bottomInputField.transform.Find("Placeholder").GetComponent<Text>().text = StaticData.GetMultilingual(120178);
    }
    #endregion
}
