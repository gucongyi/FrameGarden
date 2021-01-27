using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 聊天弹幕控制器
/// </summary>
public class ChatBulletScreenController : MonoBehaviour
{
    #region 字段
    /// <summary>
    /// item 母体
    /// </summary>
    ChatBulletScreenItemController _bulletScreenItemController;
    /// <summary>
    /// item父物体
    /// </summary>
    RectTransform _boxRect;
    /// <summary>
    /// 关闭的item盒子
    /// </summary>
    RectTransform _hidItemBox;
    CanvasGroup _showCanvaGroup;
    /// <summary>
    /// 弹幕待推送数据
    /// </summary>
    List<ChatInfo> _waitPushDatas = new List<ChatInfo>();
    /// <summary>
    /// 弹幕正在推送数据
    /// </summary>
    List<ChatInfo> _pushDatas = new List<ChatInfo>();
    /// <summary>
    /// item集合
    /// </summary>
    List<ChatBulletScreenItemController> _chatBulletScreenItemControllers = new List<ChatBulletScreenItemController>();
    /// <summary>
    /// 关闭的imte集合
    /// </summary>
    List<ChatBulletScreenItemController> _hidChatBulletScreenItemControllers = new List<ChatBulletScreenItemController>();
    float _time = 0;
    /// <summary>
    /// 是否已经初始化
    /// </summary>
    bool _isInitial = false;
    #endregion
    #region 函数
    private void Awake()
    {
        if (!_isInitial)
        {
            Initial();
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        if (_time <= 0 && _waitPushDatas != null && _waitPushDatas.Count > 0&& _chatBulletScreenItemControllers.Count<100)
        {
            PushBulletScreen();
            Debug.Log(string.Format("Timer1 is up !!! time=${0}", Time.time));
            _time = Random.Range(1, 3);
        }
        _time = _time-Time.deltaTime;
    }
    /// <summary>
    /// 初始化组件
    /// </summary>
    void Initial()
    {
        _bulletScreenItemController = transform.Find("BulletScreenItem").GetComponent<ChatBulletScreenItemController>();
        _boxRect = transform.Find("Box").GetComponent<RectTransform>();
        _hidItemBox = transform.Find("HidItemBox").GetComponent<RectTransform>();
        ChatTool.EnrollTakePushDataAction(TakePushData);
        _showCanvaGroup = GetComponent<CanvasGroup>();
        _isInitial = true;
    }
    /// <summary>
    /// 接受需要推送的弹幕
    /// </summary>
    /// <param name="chatInfo"></param>
    public void TakePushData(ChatInfo chatInfo)
    {
        if (_showCanvaGroup.alpha < 1)
        {
            return;
        }
        if (_waitPushDatas.Count < 100)
        {
            _waitPushDatas.Add(chatInfo);
        }
        else
        {
            _waitPushDatas.Clear();
            _waitPushDatas.Add(chatInfo);
        }

    }
    /// <summary>
    /// 显示item
    /// </summary>
    /// <param name="str"></param>
    public void OpenItem(ChatInfo str)
    {

        ChatBulletScreenItemController item = GetHidItem();
        if (item != null)
        {
            item.ShowData(str, 100);
        }
        else
        {
            CreateItem(str, 100);
        }

    }
    /// <summary>
    /// 关闭item
    /// </summary>
    /// <param name="item"></param>
    public void HidItem(ChatBulletScreenItemController item)
    {
        if (_chatBulletScreenItemControllers.Contains(item))
        {
            _chatBulletScreenItemControllers.Remove(item);
            item.transform.SetParent(_hidItemBox);
            _hidChatBulletScreenItemControllers.Add(item);
            _pushDatas.Remove(item._ShowStr);
        }
    }
    /// <summary>
    /// 创建新的item
    /// </summary>
    /// <param name="str"></param>
    public void CreateItem(ChatInfo str, float speed)
    {
        GameObject item = GameObject.Instantiate(_bulletScreenItemController.gameObject, _boxRect);
        ChatBulletScreenItemController chatBulletScreenItemController = item.GetComponent<ChatBulletScreenItemController>();
        chatBulletScreenItemController.ShowData(str, speed);
        _chatBulletScreenItemControllers.Add(chatBulletScreenItemController);
    }
    /// <summary>
    /// 获取已关闭的item
    /// </summary>
    /// <returns></returns>
    public ChatBulletScreenItemController GetHidItem()
    {
        ChatBulletScreenItemController item = null;
        if (_hidChatBulletScreenItemControllers != null && _hidChatBulletScreenItemControllers.Count > 0)
        {
            item = _hidChatBulletScreenItemControllers[0];
            _hidChatBulletScreenItemControllers.Remove(item);
            item.transform.SetParent(_boxRect);
            _chatBulletScreenItemControllers.Add(item);

        }
        return item;
    }
    /// <summary>
    /// 推送弹幕
    /// </summary>
    /// <returns></returns>
    public void PushBulletScreen()
    {
        ChatInfo chatInfo = _waitPushDatas[0];
        _waitPushDatas.Remove(chatInfo);
        _pushDatas.Add(chatInfo);
        OpenItem(chatInfo);
    }
    public void CloseShowItem()
    {
        for (int i = 0; i < _chatBulletScreenItemControllers.Count; i++)
        {
            HidItem(_chatBulletScreenItemControllers[i]);
        }
    }
    private void OnEnable()
    {
        _waitPushDatas.Clear();
        CloseShowItem();
    }

    public void OpenShow(bool isOpen)
    {
        if (!_isInitial)
        {
            Initial();
        }
        if (isOpen)
        {
            _showCanvaGroup.alpha = 1;
        }
        else
        {
            _showCanvaGroup.alpha = 0;
        }

    }
    #endregion
}
