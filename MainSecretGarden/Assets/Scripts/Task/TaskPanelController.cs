using Company.Cfg;
using Cysharp.Threading.Tasks;
using Game.Protocal;
using Google.Protobuf.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 任务面板控制器
/// </summary>
public class TaskPanelController : MonoBehaviour
{
    #region 字段
    /// <summary>
    /// 背景
    /// </summary>
    RectTransform _thisBgRect;
    /// <summary>
    /// 顶部栏
    /// </summary>
    RectTransform _topRect;
    /// <summary>
    /// 每日任务勾选组件
    /// </summary>
    Toggle _dailyTaskToggle;
    /// <summary>
    /// 每日任务勾选组件text
    /// </summary>
    Text _dailyTaskToggleText;
    /// <summary>
    /// 新手任务勾选组件
    /// </summary>
    Toggle _tyroTaskToggle;
    /// <summary>
    /// 新手任务勾选组件text
    /// </summary>
    Text _tyroTaskToggleText;
    /// <summary>
    /// 任务item克隆母体
    /// </summary>
    RectTransform _taskItem;
    /// <summary>
    /// 每日任务内容面板
    /// </summary>
    RectTransform _dailyTaskBoxRect;
    /// <summary>
    /// 每日任务内容面板中部
    /// </summary>
    RectTransform _dailyTaskBoxMiddleRect;
    /// <summary>
    /// 每日任务内容面板滑动组件
    /// </summary>
    ScrollRect _dilyTaskBoxMiddleScrollRect;
    /// <summary>
    /// 每日任务内容面板底部
    /// </summary>
    RectTransform _dailyTaskBoxBottomRect;
    /// <summary>
    /// 每日任务内容面板一键领取按钮
    /// </summary>
    Button _aKeyToGetBtn;
    /// <summary>
    /// 每日任务内容面板一键领取按钮text
    /// </summary>
    Text _aKeyToGetBtnText;
    /// <summary>
    /// 新手任务内容面板
    /// </summary>
    RectTransform _tyroTaskBoxRect;
    /// <summary>
    /// 新手任务内容面板中部
    /// </summary>
    RectTransform _tyroTaskBoxMiddleRect;
    /// <summary>
    /// 新手内容面板提示
    /// </summary>
    RectTransform _showTipBoxRect;
    /// <summary>
    /// 新手内容面板提示Text
    /// </summary>
    Text _showTipBoxText;
    /// <summary>
    /// 新手内容面板滑动组件
    /// </summary>
    ScrollRectAuxiliary _tyroTaskBoxMiddleScrollRect;
    /// <summary>
    /// 新手任务向左滑动按钮
    /// </summary>
    Button _tyroTaskBoxSlideToTheLeftBtn;
    /// <summary>
    /// 新手任务向右滑动按钮
    /// </summary>
    Button _tyroTaskBoxSlideToTheRightBtn;
    /// <summary>
    /// 新手任务内容面板底部
    /// </summary>
    RectTransform _tyroTaskBoxBottomRect;
    /// <summary>
    /// 视频播放box
    /// </summary>
    RectTransform _videoBoxRect;
    /// <summary>
    /// 播放视频提示语
    /// </summary>
    Text _videoBoxTipText;
    /// <summary>
    /// 视频播放器
    /// </summary>
    RectTransform _videRect;
    /// <summary>
    /// 退出按钮
    /// </summary>
    Button _closeBtn;
    /// <summary>
    /// 前往按钮图片精灵
    /// </summary>
    [SerializeField]
    Sprite _leaveForBtnSprite;
    /// <summary>
    /// 领取按钮图片精灵
    /// </summary>
    [SerializeField]
    Sprite _getBtnSprite;
    /// <summary>
    /// 已领取按钮图片精灵
    /// </summary>
    [SerializeField]
    Sprite _alreadyReceivedBtnSprite;
    /// <summary>
    /// 所有任务数据以id储存
    /// </summary>
    Dictionary<int, TaskDefine> _allDataDic = new Dictionary<int, TaskDefine>();
    /// <summary>
    /// 所有任务数据根据类型划分
    /// </summary>
    Dictionary<Company.Cfg.TaskType, List<TaskDefine>> _allTypeDataDic = new Dictionary<Company.Cfg.TaskType, List<TaskDefine>>();
    /// <summary>
    /// 已拥有的视频
    /// </summary>
    Dictionary<string, GameObject> _haveVideoDic = new Dictionary<string, GameObject>();
    /// <summary>
    /// 服务器储存数据
    /// </summary>
    List<SCGetTaskInfoStruct> _sCGetTaskInfoStructs = new List<SCGetTaskInfoStruct>();
    /// <summary>
    /// 服务器储存数据字典
    /// </summary>
    Dictionary<int, SCGetTaskInfoStruct> _sCGetTaskInfoStructDic = new Dictionary<int, SCGetTaskInfoStruct>();
    /// <summary>
    /// 当前分页标记
    /// </summary>
    int _currPagingIndex = -1;
    /// <summary>
    /// 当前展示item集合
    /// </summary>
    List<TaskPanelItem> _currShowItems = new List<TaskPanelItem>();
    /// <summary>
    /// 当前正在展示的item字典
    /// </summary>
    Dictionary<int, TaskPanelItem> _currShowItemDic = new Dictionary<int, TaskPanelItem>();
    /// <summary>
    /// 是否已经初始化
    /// </summary>
    bool _isInitial = false;
    /// <summary>
    /// 是否播放开始动画效果
    /// </summary>
    bool _isPlayAnimation = false;
    #endregion
    #region 函数
    // Start is called before the first frame update
    void Start()
    {

    }
    private void Awake()
    {
        if (!_isInitial)
        {
            Initial();
        }
        if (_thisBgRect != null)
            UniversalTool.ReadyUIAnimTwo(GetComponent<CanvasGroup>(), _thisBgRect);

        _isPlayAnimation = true;
    }
    private void OnDisable()
    {
        if (_thisBgRect != null)
            UniversalTool.ReadyUIAnimTwo(GetComponent<CanvasGroup>(), _thisBgRect);
        _isPlayAnimation = true;
    }
    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// 初始化组件
    /// </summary>
    public void Initial()
    {
        /// <summary>
        /// 背景
        /// </summary>
        _thisBgRect = transform.Find("Bg").GetComponent<RectTransform>();
        /// <summary>
        /// 顶部栏
        /// </summary>
        _topRect = _thisBgRect.Find("Top").GetComponent<RectTransform>();
        /// <summary>
        /// 每日任务勾选组件
        /// </summary>
        _dailyTaskToggle = _topRect.Find("DailyTaskToggle").GetComponent<Toggle>();
        /// <summary>
        /// 每日任务勾选组件text
        /// </summary>
        _dailyTaskToggleText = _dailyTaskToggle.transform.Find("Label").GetComponent<Text>();
        /// <summary>
        /// 新手任务勾选组件
        /// </summary>
        _tyroTaskToggle = _topRect.Find("TyroTaskToggle").GetComponent<Toggle>();
        /// <summary>
        /// 新手任务勾选组件text
        /// </summary>
        _tyroTaskToggleText = _tyroTaskToggle.transform.Find("Label").GetComponent<Text>();
        /// <summary>
        /// 任务item克隆母体
        /// </summary>
        _taskItem = _thisBgRect.Find("TaskItem").GetComponent<RectTransform>();
        /// <summary>
        /// 每日任务内容面板
        /// </summary>
        _dailyTaskBoxRect = _thisBgRect.Find("DailyTaskBox").GetComponent<RectTransform>();
        /// <summary>
        /// 每日任务内容面板中部
        /// </summary>
        _dailyTaskBoxMiddleRect = _dailyTaskBoxRect.Find("Middle").GetComponent<RectTransform>();
        /// <summary>
        /// 每日任务内容面板滑动组件
        /// </summary>
        _dilyTaskBoxMiddleScrollRect = _dailyTaskBoxMiddleRect.Find("Scroll View").GetComponent<ScrollRect>();
        /// <summary>
        /// 每日任务内容面板底部
        /// </summary>
        _dailyTaskBoxBottomRect = _dailyTaskBoxRect.Find("Bottom").GetComponent<RectTransform>();
        /// <summary>
        /// 每日任务内容面板一键领取按钮
        /// </summary>
        _aKeyToGetBtn = _dailyTaskBoxBottomRect.Find("AKeyToGetBtn").GetComponent<Button>();
        /// <summary>
        /// 每日任务内容面板一键领取按钮text
        /// </summary>
        _aKeyToGetBtnText = _aKeyToGetBtn.transform.Find("Text").GetComponent<Text>();
        /// <summary>
        /// 新手任务内容面板
        /// </summary>
        _tyroTaskBoxRect = _thisBgRect.Find("TyroTaskBox").GetComponent<RectTransform>();
        /// <summary>
        /// 新手任务内容面板中部
        /// </summary>
        _tyroTaskBoxMiddleRect = _tyroTaskBoxRect.Find("Middle").GetComponent<RectTransform>();
        /// <summary>
        /// 新手内容面板提示
        /// </summary>
        _showTipBoxRect = _tyroTaskBoxMiddleRect.Find("ShowTipBox").GetComponent<RectTransform>();
        /// <summary>
        /// 新手内容面板提示Text
        /// </summary>
        _showTipBoxText = _showTipBoxRect.Find("Text").GetComponent<Text>();
        /// <summary>
        /// 新手内容面板滑动组件
        /// </summary>
        _tyroTaskBoxMiddleScrollRect = _tyroTaskBoxMiddleRect.Find("Scroll View").GetComponent<ScrollRectAuxiliary>();
        /// <summary>
        /// 新手任务向左滑动按钮
        /// </summary>
        _tyroTaskBoxSlideToTheLeftBtn = _tyroTaskBoxMiddleScrollRect.transform.Find("Left").GetComponent<Button>();
        /// <summary>
        /// 新手任务向右滑动按钮
        /// </summary>
        _tyroTaskBoxSlideToTheRightBtn = _tyroTaskBoxMiddleScrollRect.transform.Find("Right").GetComponent<Button>();
        /// <summary>
        /// 新手任务内容面板底部
        /// </summary>
        _tyroTaskBoxBottomRect = _tyroTaskBoxRect.Find("Bottom").GetComponent<RectTransform>();
        /// <summary>
        /// 视频播放box
        /// </summary>
        _videoBoxRect = _tyroTaskBoxBottomRect.Find("VideoBox").GetComponent<RectTransform>();
        /// <summary>
        /// 播放视频提示语
        /// </summary>
        _videoBoxTipText = _videoBoxRect.Find("TipText").GetComponent<Text>();
        /// <summary>
        /// 视频播放器
        /// </summary>
        _videRect = _videoBoxRect.Find("Video").GetComponent<RectTransform>();

        _closeBtn = _thisBgRect.Find("CloseButton").GetComponent<Button>();
        _closeBtn.onClick.RemoveAllListeners();
        _closeBtn.onClick.AddListener(ClickClose);

        _dailyTaskToggle.onValueChanged.RemoveAllListeners();
        _dailyTaskToggle.onValueChanged.AddListener(ClickDailyTaskToggle);
        _tyroTaskToggle.onValueChanged.RemoveAllListeners();
        _tyroTaskToggle.onValueChanged.AddListener(ClickTryoTaskToggle);

        _tyroTaskBoxSlideToTheLeftBtn.onClick.RemoveAllListeners();
        _tyroTaskBoxSlideToTheLeftBtn.onClick.AddListener(() => { CliCkTyroTaskBoxSlideToTheRightBtn(0); });
        _tyroTaskBoxSlideToTheRightBtn.onClick.RemoveAllListeners();
        _tyroTaskBoxSlideToTheRightBtn.onClick.AddListener(() => { CliCkTyroTaskBoxSlideToTheRightBtn(1); });

        _tyroTaskBoxMiddleScrollRect._endDragAction = TyroTaskScrillRectEndDrag;
        _tyroTaskBoxMiddleScrollRect._isVoluntarilyLocation = true;
        _tyroTaskBoxMiddleScrollRect._startMoveLocationAction = () => { OpenTyroTaskBoxSlideToTheRightBtn(false); };
        _tyroTaskBoxMiddleScrollRect._endMoveLocationAction = () =>
        {
            OpenTyroTaskBoxSlideToTheRightBtn(true);
            TyroTaskBoxSlideEndDrag();
            PlayVideo();
        };

        _aKeyToGetBtn.onClick.RemoveAllListeners();
        _aKeyToGetBtn.onClick.AddListener(ClickAKeyToGetBtn);
        _isInitial = true;
    }
    /// <summary>
    /// 展示数据
    /// </summary>
    public void Show()
    {
        if (!_isInitial)
        {
            Initial();
        }

        //获取服务器数据
        GetServersData();
        //获取本地配置数据
        GetAllData();

    }
    /// <summary>
    /// 新手任务滑动栏拖拽结束
    /// </summary>
    private void TyroTaskScrillRectEndDrag()
    {
        Debug.Log("新手任务滑动栏拖拽结束");
    }
    /// <summary>
    /// 点击关闭
    /// </summary>
    public void ClickClose()
    {
        UniversalTool.CancelUIAnimTwo(GetComponent<CanvasGroup>(), _thisBgRect, () =>
        {
            ClearItemAndData();
            UIComponent.HideUI(UIType.TaskPanel);
            //TaskPanelTool.UpdateTaskTag();
            RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Task);
        });
    }

    public void Skip(Action endAction)
    {
        UniversalTool.CancelUIAnimTwo(GetComponent<CanvasGroup>(), _thisBgRect, () =>
        {
            ClearItemAndData();
            UIComponent.HideUI(UIType.TaskPanel);
            RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.Task);
            endAction?.Invoke();
        });
    }
    /// <summary>
    /// 开关新手任务左右滑动按钮点击
    /// </summary>
    /// <param name="isOpen"></param>
    public void OpenTyroTaskBoxSlideToTheRightBtn(bool isOpen)
    {
        _tyroTaskBoxSlideToTheLeftBtn.enabled = isOpen;
        _tyroTaskBoxSlideToTheRightBtn.enabled = isOpen;
    }
    /// <summary>
    /// 移动结束后判断是否开启新手任务滑动栏左右开关
    /// </summary>
    public void TyroTaskBoxSlideEndDrag()
    {
        _tyroTaskBoxSlideToTheLeftBtn.gameObject.SetActive(true);
        _tyroTaskBoxSlideToTheRightBtn.gameObject.SetActive(true);
        if (_tyroTaskBoxMiddleScrollRect.horizontalScrollbar.value <= 0)
        {
            _tyroTaskBoxSlideToTheLeftBtn.gameObject.SetActive(false);
        }

        if (_tyroTaskBoxMiddleScrollRect.horizontalScrollbar.value >= 0.95f)
        {
            _tyroTaskBoxSlideToTheRightBtn.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 新手任务滑动栏左右按钮点击
    /// </summary>
    /// <param name="type"></param>
    public void CliCkTyroTaskBoxSlideToTheRightBtn(int type)
    {
        int itemIndex = 0;
        _tyroTaskBoxMiddleScrollRect.CurrBeCentralItem(out itemIndex);
        //是否时有效点击
        bool isValid = false;
        switch (type)
        {
            case 0:
                if (itemIndex != 0)
                {
                    itemIndex = itemIndex - 1;
                    isValid = true;
                }
                break;
            case 1:
                if (itemIndex < _tyroTaskBoxMiddleScrollRect.content.childCount - 1)
                {
                    itemIndex = itemIndex + 1;
                    isValid = true;
                }
                break;
        }
        if (isValid)
        {
            _tyroTaskBoxMiddleScrollRect.LocationItem(itemIndex);
        }

    }
    /// <summary>
    /// 获取所有任务数据
    /// </summary>
    public void GetAllData()
    {
        _allDataDic = TaskPanelTool.GetAllData();
        //划分任务类型
        foreach (var item in _allDataDic)
        {
            if (_allTypeDataDic.ContainsKey(item.Value.TaskType))
            {
                _allTypeDataDic[item.Value.TaskType].Add(item.Value);
            }
            else
            {
                List<TaskDefine> taskDefines = new List<TaskDefine>();
                taskDefines.Add(item.Value);
                _allTypeDataDic.Add(item.Value.TaskType, taskDefines);
            }
        }
    }
    /// <summary>
    /// 获取服务器数据
    /// </summary>
    public void GetServersData()
    {
        TaskPanelTool.GetServersData(GetServersDataAction);
    }
    /// <summary>
    /// 获取服务器数据回调
    /// </summary>
    /// <param name="isSucceed"></param>
    /// <param name="datas"></param>
    public async void GetServersDataAction(bool isSucceed, List<SCGetTaskInfoStruct> datas)
    {
        if (isSucceed)
        {
            _sCGetTaskInfoStructDic.Clear();
            _sCGetTaskInfoStructs.Clear();
            _sCGetTaskInfoStructs.AddRange(datas);
            for (int i = 0; i < _sCGetTaskInfoStructs.Count; i++)
            {
                SCGetTaskInfoStruct data = _sCGetTaskInfoStructs[i];
                if (!_sCGetTaskInfoStructDic.ContainsKey(data.TaskID))
                {
                    _sCGetTaskInfoStructDic.Add(data.TaskID, data);
                }
            }
        }
        else
        {
            Debug.Log("获取服务器任务数据失败");
        }

        //刷新面板
        RefreshCurrItemS();

        await UniTask.Delay(TimeSpan.FromMilliseconds(100));
        //初始判断是否开启新手任务面板左右按钮是否打开
        TyroTaskBoxSlideEndDrag();

        if (_currPagingIndex == -1)
        {
            if (!StaticData.IsOpenFunction(10014, false))
            {
                ClickTryoTaskToggle(true);
                //PlayVideo(); 注释掉多余调用
            }
            else
            {
                ClickDailyTaskToggle(true);
            }

        }
        else
        {
            switch (_currPagingIndex)
            {
                case 0:
                    ClickDailyTaskToggle(true);
                    break;
                case 1:
                    ClickTryoTaskToggle(true);
                    //PlayVideo();
                    break;
            }
        }

    }
    /// <summary>
    /// 创建item实列
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public Transform CreationItem(Transform parent)
    {
        Transform item = Instantiate(_taskItem, parent);
        return item;
    }
    /// <summary>
    /// 点击新手任务勾选组件
    /// </summary>
    /// <param name="arg0"></param>
    private void ClickTryoTaskToggle(bool arg0)
    {
        if (arg0)
        {
            SelectToggle(1);
            ChageTopBtn(1);
        }
    }
    /// <summary>
    /// 点击每日任务勾选组件
    /// </summary>
    /// <param name="arg0"></param>
    private void ClickDailyTaskToggle(bool arg0)
    {
        if (arg0)
        {

            //功能是否开启
            if (!StaticData.IsOpenFunction(10014))
            {
                _tyroTaskToggle.isOn = true;
                _dailyTaskToggle.isOn = false;
                ChageTopBtn(1);
                return;
            }
            else
            {
                if (!_dailyTaskToggle.isOn)
                {
                    _dailyTaskToggle.isOn = true;
                }
                SelectToggle(0);
                ChageTopBtn(0);
            }
        }
    }
    /// <summary>
    /// 更改顶部勾选按钮样式
    /// </summary>
    /// <param name="type"></param>
    void ChageTopBtn(int type)
    {
        switch (type)
        {
            case 0:
                _tyroTaskToggleText.color = new Color32(138, 160, 239, 255);
                _dailyTaskToggleText.color = new Color32(255, 255, 255, 255);
                break;
            case 1:
                _tyroTaskToggleText.color = new Color32(255, 255, 255, 255);
                _dailyTaskToggleText.color = new Color32(138, 160, 239, 255);
                break;
        }
    }
    /// <summary>
    /// 分页选择
    /// </summary>
    /// <param name="index">0：每日任务  1：新手任务</param>
    public void SelectToggle(int index)
    {
        if (index != 1)
        {
            if (_isPlayAnimation)
            {
                _isPlayAnimation = false;
                UniversalTool.StartUIAnimTwo(GetComponent<CanvasGroup>(), _thisBgRect);
            }

        }

        //if (_currPagingIndex == index)
        //{
        //    return;
        //}
        _currPagingIndex = index;
        List<TaskDefine> taskDefines = new List<TaskDefine>();
        Transform parent = null;
        switch (index)
        {
            case 0:
                _dailyTaskBoxRect.gameObject.SetActive(true);
                _tyroTaskBoxRect.gameObject.SetActive(false);
                parent = _dilyTaskBoxMiddleScrollRect.content;
                if (_allTypeDataDic.ContainsKey(Company.Cfg.TaskType.DailyTask))
                {
                    taskDefines = _allTypeDataDic[Company.Cfg.TaskType.DailyTask];
                }
                break;
            case 1:
                _dailyTaskBoxRect.gameObject.SetActive(false);
                _tyroTaskBoxRect.gameObject.SetActive(true);
                parent = _tyroTaskBoxMiddleScrollRect.content;
                if (_allTypeDataDic.ContainsKey(Company.Cfg.TaskType.GuidanceTask))
                {
                    taskDefines = _allTypeDataDic[Company.Cfg.TaskType.GuidanceTask];
                }
                break;
        }
        DisposeAllCurrShowItem();
        taskDefines = DataSort(taskDefines);

        for (int i = 0; i < taskDefines.Count; i++)
        {
            RectTransform itemTra = CreationItem(parent).GetComponent<RectTransform>();
            TaskDefine data = taskDefines[i];
            TaskPanelItem taskPanelItem = itemTra.GetComponent<TaskPanelItem>();
            taskPanelItem.Initial(data, this);
            _currShowItems.Add(taskPanelItem);
            if (!_currShowItemDic.ContainsKey(data.TaskID))
            {
                _currShowItemDic.Add(data.TaskID, taskPanelItem);
            }
        }
        if (index == 0)
        {
            bool isOpen = IsOpenAKeyToGet();
            _aKeyToGetBtn.enabled = isOpen;
            Image btnIamge = _aKeyToGetBtn.transform.GetComponent<Image>();
            if (isOpen)
            {
                btnIamge.sprite = GetBtnSprite(1);
            }
            else
            {
                btnIamge.sprite = GetBtnSprite(2);
            }
        }

        if (index == 1)
        {
            _tyroTaskBoxSlideToTheLeftBtn.gameObject.SetActive(false);
            _tyroTaskBoxSlideToTheRightBtn.gameObject.SetActive(true);
            PlayVideo();
        }
        else
        {
            if (_isPlayAnimation)
            {
                _isPlayAnimation = false;
                UniversalTool.StartUIAnimTwo(GetComponent<CanvasGroup>(), _thisBgRect);
            }

        }

    }
    /// <summary>
    /// 判断一键领按钮状态
    /// </summary>
    /// <returns></returns>
    public bool IsOpenAKeyToGet()
    {
        bool isOpen = false;
        for (int i = 0; i < _currShowItems.Count; i++)
        {
            if (IsCanGetAward(_currShowItems[i]._ShowData))
            {
                isOpen = true;
            }
        }
        return isOpen;
    }
    /// <summary>
    /// 一键领取
    /// </summary>
    private void ClickAKeyToGetBtn()
    {
        List<TaskDefine> taskDefines = new List<TaskDefine>();
        taskDefines.AddRange(_allTypeDataDic[Company.Cfg.TaskType.DailyTask]);

        List<TaskDefine> getDatas = new List<TaskDefine>();
        for (int i = 0; i < taskDefines.Count; i++)
        {
            TaskDefine data = taskDefines[i];
            if (IsCanGetAward(data))
            {
                getDatas.Add(data);
            }
        }

        if (getDatas != null && getDatas.Count > 0)
        {
            TaskPanelTool.GetTaskAward(getDatas, true, GetAwardAction);
        }
    }
    /// <summary>
    /// 一键领取回调
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    private void GetAwardAction(bool arg1, SCGetTaskAward arg2)
    {
        if (arg1)
        {
            Debug.Log("一键领取成功");
            Debug.Log(arg2);
            GetAward(arg2);
        }
        else
        {
            Debug.Log("一键领取失败");
        }
    }
    /// <summary>
    /// 清理当前展示的所有item
    /// </summary>
    public void DisposeAllCurrShowItem()
    {
        for (int i = 0; i < _currShowItems.Count; i++)
        {
            _currShowItems[i].Dispose();
        }
        _currShowItems.Clear();
        _currShowItemDic.Clear();
    }
    /// <summary>
    /// 获取按钮样式
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Sprite GetBtnSprite(int type)
    {
        Sprite sprite = null;
        switch (type)
        {
            case 0:
                sprite = _leaveForBtnSprite;
                break;
            case 1:
                sprite = _getBtnSprite;
                break;
            case 2:
                sprite = _alreadyReceivedBtnSprite;
                break;
        }

        return sprite;
    }
    /// <summary>
    /// 播放当前新手任务视频
    /// </summary>
    public async void PlayVideo()
    {
        //return;
        Debug.Log("播放动画");
        int itemIndex = 0;
        RectTransform item = _tyroTaskBoxMiddleScrollRect.CurrBeCentralItem(out itemIndex);

        if (_currShowItems == null || _currShowItems.Count <= 0)
        {
            OpenAllVideo(false);
            return;
        }

        TaskDefine data = item.GetComponent<TaskPanelItem>()._ShowData;// _allTypeDataDic[Company.Cfg.TaskType.GuidanceTask][itemIndex];
        string videoName = data.VideoName;
        if (!string.IsNullOrEmpty(videoName))
        {
            if (_haveVideoDic.ContainsKey(videoName) && _haveVideoDic[videoName].activeSelf == true)
            {
                if (_isPlayAnimation)
                {
                    _isPlayAnimation = false;
                    UniversalTool.StartUIAnimTwo(GetComponent<CanvasGroup>(), _thisBgRect);
                }
                return;
            }
            OpenAllVideo(false);
            if (_haveVideoDic.ContainsKey(videoName))
            {
                _haveVideoDic[videoName].SetActive(true);
            }
            else
            {
                //WaitManager.BeginRotate();
                //Debug.Log("+++++++++++++++++++++++++++++加载预制体：" + Time.realtimeSinceStartup);
                GameObject videoPrefabObj = await ABManager.GetAssetAsync<GameObject>(videoName);
                //Debug.Log("++++++++++++++++++++++++++++++++++++++加载预制体结束：" + Time.realtimeSinceStartup);
                //WaitManager.EndRotate();
                GameObject videoObj = GameObject.Instantiate(videoPrefabObj, _videRect);
                //Debug.Log("++++++++++++++++++++++++++++++++++++++++实例化预制体结束：" + Time.realtimeSinceStartup);
                videoObj.transform.localPosition = Vector3.zero;

                _haveVideoDic.Add(videoName, videoObj);
            }
        }
        else
        {
            OpenAllVideo(false);
        }
        if (_isPlayAnimation)
        {
            _isPlayAnimation = false;
            UniversalTool.StartUIAnimTwo(GetComponent<CanvasGroup>(), _thisBgRect);
        }

    }
    /// <summary>
    /// 开关所哟新手任务视频
    /// </summary>
    /// <param name="isOpen"></param>
    public void OpenAllVideo(bool isOpen)
    {
        foreach (var item in _haveVideoDic)
        {
            item.Value.SetActive(isOpen);
        }
    }
    /// <summary>
    /// 获取当前任务完成次数
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public int GetTaskFinishNumber(TaskDefine data)
    {
        int number = 0;
        if (_sCGetTaskInfoStructDic.ContainsKey(data.TaskID))
        {
            number = _sCGetTaskInfoStructDic[data.TaskID].Schedule;
        }
        return number;
    }
    /// <summary>
    /// 获取当前任务是否已经领取
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool IsAlreadyGetAward(TaskDefine data)
    {
        bool isCanGet = false;
        if (_sCGetTaskInfoStructDic.ContainsKey(data.TaskID))
        {
            isCanGet = _sCGetTaskInfoStructDic[data.TaskID].IsGet;
        }
        return isCanGet;
    }
    /// <summary>
    /// 是否可以领取奖励
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool IsCanGetAward(TaskDefine data)
    {
        bool isCanGet = false;
        int currNumber = GetTaskFinishNumber(data);
        if (currNumber >= data.FinishNum && IsAlreadyGetAward(data))
        {
            isCanGet = true;
        }
        return isCanGet;
    }
    /// <summary>
    /// 数据排序
    /// </summary>
    /// <param name="datas"></param>
    /// <returns></returns>
    public List<TaskDefine> DataSort(List<TaskDefine> datas)
    {
        List<TaskDefine> taskDefines = new List<TaskDefine>();
        List<TaskDefine> taskDefinesTwo = new List<TaskDefine>();
        List<TaskDefine> taskDefinesTherr = new List<TaskDefine>();
        for (int i = 0; i < datas.Count; i++)
        {
            TaskDefine data = datas[i];
            if (IsCanGetAward(data))
            {
                taskDefines.Add(data);
            }
            else if (GetTaskFinishNumber(data) >= data.FinishNum)
            {
                taskDefinesTherr.Add(data);
            }
            else
            {
                taskDefinesTwo.Add(data);
            }
        }
        taskDefines.AddRange(taskDefinesTwo);
        taskDefines.AddRange(taskDefinesTherr);
        return taskDefines;
    }
    /// <summary>
    /// 领取奖励
    /// </summary>
    /// <param name="taskIds"></param>
    /// <param name="awardData"></param>
    public void GetAward(SCGetTaskAward awardData)
    {
        Dictionary<int, SCGetTaskInfoStruct> dataDic = new Dictionary<int, SCGetTaskInfoStruct>();
        foreach (var item in _sCGetTaskInfoStructDic)
        {
            dataDic.Add(item.Key, item.Value);
        }
        if (awardData == null)
        {
            return;
        }
        for (int i = 0; i < awardData.TaskIDList.Count; i++)
        {
            int id = awardData.TaskIDList[i];
            if (_allDataDic.ContainsKey(id))
            {
                if (dataDic.ContainsKey(id))
                {
                    dataDic[id].IsGet = false;
                    dataDic[id].Schedule = _allDataDic[id].FinishNum;
                }
                else
                {
                    SCGetTaskInfoStruct sCGetTaskInfoStruct = new SCGetTaskInfoStruct();
                    sCGetTaskInfoStruct.TaskID = id;
                    sCGetTaskInfoStruct.IsGet = false;
                    sCGetTaskInfoStruct.Schedule = _allDataDic[id].FinishNum;
                    switch (_allDataDic[id].TaskType)
                    {
                        case Company.Cfg.TaskType.None:
                            sCGetTaskInfoStruct.Type = Game.Protocal.TaskType.None;
                            break;
                        case Company.Cfg.TaskType.GuidanceTask:
                            sCGetTaskInfoStruct.Type = Game.Protocal.TaskType.GuidanceTaskType;
                            break;
                        case Company.Cfg.TaskType.DailyTask:
                            sCGetTaskInfoStruct.Type = Game.Protocal.TaskType.DailyTaskType;
                            break;
                    }

                    dataDic.Add(id, sCGetTaskInfoStruct);
                }

            }
        }
        AwardBePutInStorage(awardData.AwardInfo);
        List<SCGetTaskInfoStruct> sCGetTaskInfoStructs = new List<SCGetTaskInfoStruct>();
        foreach (var item in dataDic)
        {
            sCGetTaskInfoStructs.Add(item.Value);
        }
        GetServersDataAction(true, sCGetTaskInfoStructs);
    }
    /// <summary>
    /// 奖励入库
    /// </summary>
    /// <param name="awards"></param>
    public void AwardBePutInStorage(RepeatedField<CSGoodStruct> awards)
    {
        for (int i = 0; i < awards.Count; i++)
        {
            StaticData.UpdateWareHouseItem(awards[i].GoodId, awards[i].GoodNum);
        }
    }
    /// <summary>
    /// 刷新面板
    /// </summary>
    public void RefreshCurrItemS()
    {
        List<TaskDefine> taskDefines = new List<TaskDefine>();
        Transform parent = null;
        switch (_currPagingIndex)
        {
            case 0:
                if (_allTypeDataDic.ContainsKey(Company.Cfg.TaskType.DailyTask))
                {
                    taskDefines = _allTypeDataDic[Company.Cfg.TaskType.DailyTask];
                }
                break;
            case 1:
                if (_allTypeDataDic.ContainsKey(Company.Cfg.TaskType.GuidanceTask))
                {
                    taskDefines = _allTypeDataDic[Company.Cfg.TaskType.GuidanceTask];
                }
                break;
        }
        //数据排序
        taskDefines = DataSort(taskDefines);
        List<TaskPanelItem> items = new List<TaskPanelItem>();
        //重新排序item
        for (int i = 0; i < taskDefines.Count; i++)
        {
            int taskId = taskDefines[i].TaskID;
            if (_currShowItemDic.ContainsKey(taskId))
            {
                _currShowItemDic[taskId].SetHierarchy(i);
                _currShowItemDic[taskId].ShowData();
            }
            TaskPanelItem item = GetCurrItem(taskDefines[i]);
            if (item != null)
            {
                items.Add(item);
            }
        }
        _currShowItems.Clear();
        _currShowItems.AddRange(items);

        //重新设置一键领取
        if (_currPagingIndex == 0)
        {
            bool isOpen = IsOpenAKeyToGet();
            _aKeyToGetBtn.enabled = isOpen;
            Image btnIamge = _aKeyToGetBtn.transform.GetComponent<Image>();
            if (isOpen)
            {
                btnIamge.sprite = GetBtnSprite(1);
            }
            else
            {
                btnIamge.sprite = GetBtnSprite(2);
            }
        }
    }

    public TaskPanelItem GetCurrItem(TaskDefine taskDefine)
    {
        TaskPanelItem taskPanelItem = null;
        for (int i = 0; i < _currShowItems.Count; i++)
        {
            if (_currShowItems[i]._ShowData.TaskID == taskDefine.TaskID)
            {
                taskPanelItem = _currShowItems[i];
            }
        }
        return taskPanelItem;
    }
    public void ClearItemAndData()
    {
        _sCGetTaskInfoStructDic.Clear();
        _sCGetTaskInfoStructs.Clear();
        _allTypeDataDic.Clear();
        _allDataDic.Clear();
        DisposeAllCurrShowItem();
        _currPagingIndex = -1;
    }
    #endregion
}
