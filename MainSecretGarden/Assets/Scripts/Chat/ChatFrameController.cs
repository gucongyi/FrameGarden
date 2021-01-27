using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 界面聊天框控制器
/// </summary>
public class ChatFrameController : MonoBehaviour
{
    #region 字段
    /// <summary>
    /// 消息item 克隆母体
    /// </summary>
    RectTransform _showTextItemParent;
    /// <summary>
    /// 消息框
    /// </summary>
    RectTransform _frameRect;
    /// <summary>
    /// 消息框点击按钮
    /// </summary>
    Button _frameBtn;
    /// <summary>
    /// 消息item生成位置
    /// </summary>
    RectTransform _showTextItemBornAtRect;
    /// <summary>
    /// icon图标box
    /// </summary>
    RectTransform _iconBoxRect;
    /// <summary>
    /// icon图标
    /// </summary>
    RectTransform _iconRect;
    /// <summary>
    /// 展示过的数据
    /// </summary>
    List<ChatInfo> _oldDatas = new List<ChatInfo>();
    /// <summary>
    /// 所有聊天信息
    /// </summary>
    List<ChatInfo> _allChatDatas = new List<ChatInfo>();
    /// <summary>
    /// 所有世界聊天
    /// </summary>
    List<ChatInfo> _allWorldDatas = new List<ChatInfo>();
    /// <summary>
    /// 所有私聊
    /// </summary>
    List<ChatInfo> _allPrivateChats = new List<ChatInfo>();
    /// <summary>
    /// item滚动
    /// </summary>
    CancellationTokenSource _itemRollTokenSource;
    /// <summary>
    /// 关闭的聊天信息显示item
    /// </summary>
    List<RectTransform> _hidShowDataTexts = new List<RectTransform>();
    /// <summary>
    /// 正在使用的聊天信息item
    /// </summary>
    List<RectTransform> _showDataTexts = new List<RectTransform>();
    /// <summary>
    /// 对话框展开关闭速度
    /// </summary>
    float _unfoldSpeed = 10f;
    /// <summary>
    /// 是否已经展开
    /// </summary>
    bool _isUnfold;

    /// <summary>
    /// 是否初始化完毕
    /// </summary>
    bool _isInitial = false;
    #endregion
    #region 函数
    private void Awake()
    {
        Initial();
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    /// <summary>
    /// 初始化组件
    /// </summary>
    void Initial()
    {
        _showTextItemParent = transform.Find("ShowTextItem").GetComponent<RectTransform>();
        _frameRect = transform.Find("Frame").GetComponent<RectTransform>();
        _frameBtn = _frameRect.GetComponent<Button>();
        _showTextItemBornAtRect = _frameRect.Find("TextBox").GetComponent<RectTransform>();
        _iconBoxRect = _frameRect.Find("IconBox").GetComponent<RectTransform>();
        _iconRect = _iconBoxRect.Find("Icon").GetComponent<RectTransform>();

        _frameBtn.onClick.RemoveAllListeners();
        _frameBtn.onClick.AddListener(ClickFrame);

        ChatTool.EnrollChatFrameAction(ReceiveData);
        InitialFramePoint();
        _isInitial = true;
    }

    void InitialFramePoint()
    {
        float x = -_frameRect.rect.width;
        x = x + _iconBoxRect.rect.width;
        _frameRect.localPosition = new Vector3(x, _frameRect.localPosition.y);
    }
    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// 点击聊天框
    /// </summary>
    private async void ClickFrame()
    {
        await StaticData.OpenChatPanel();
    }
    /// <summary>
    /// 接收推送消息
    /// </summary>
    /// <param name="obj">聊天数据</param>
    /// /// <param name="isPrivate">是否是私聊信息</param>
    private async void ReceiveData(ChatInfo data, bool isPrivate)
    {
        if (!_isInitial)
        {
            Initial();
        }
        if (isPrivate)
        {
            _allPrivateChats.Add(data);
        }
        else
        {
            _allWorldDatas.Add(data);
        }
        _allChatDatas.Add(data);

        UnfoldFrame();

    }
    /// <summary>
    /// 展开聊天框
    /// </summary>
    async UniTask UnfoldFrame()
    {
        _isUnfold = false;
        while (!_isUnfold)
        {
            _frameRect.localPosition = Vector3.MoveTowards(_frameRect.localPosition, Vector3.zero, _unfoldSpeed);
            await UniTask.Delay(TimeSpan.FromMilliseconds(10));
            if (Vector3.Distance(_frameRect.localPosition, Vector3.zero) < 1)
            {
                _isUnfold = true;
            }
        }
        await ShowChatData();
    }

    /// <summary>
    /// 关闭聊天框
    /// </summary>
    async UniTask CloseFrame()
    {
        float x = -_frameRect.rect.width;
        x = x + _iconBoxRect.rect.width;
        Vector3 tageVector = new Vector3(x, _frameRect.localPosition.y);

        while (_isUnfold)
        {
            _frameRect.localPosition = Vector3.MoveTowards(_frameRect.localPosition, tageVector, _unfoldSpeed);
            await UniTask.Delay(TimeSpan.FromMilliseconds(10));
            if (Vector3.Distance(_frameRect.localPosition, tageVector) < 1)
            {
                _isUnfold = false;


                //清楚旧数据
                for (int i = 0; i < _oldDatas.Count; i++)
                {
                    ChatInfo data = _oldDatas[i];
                    _allChatDatas.Remove(data);
                    _allWorldDatas.Remove(data);
                    _allPrivateChats.Remove(data);
                }
                _oldDatas.Clear();
            }
        }



    }
    /// <summary>
    /// 展示信息
    /// </summary>
    /// <returns></returns>
    async UniTask ShowChatData()
    {

        if (_itemRollTokenSource != null)
        {
            _itemRollTokenSource.Cancel();
            _itemRollTokenSource = null;
        }
        ChatInfo data = _allChatDatas[_allChatDatas.Count - 1];
        if (!_oldDatas.Contains(data))
        {
            await CreationShowItem(data);
            _oldDatas.Add(data);
            await UniTask.Delay(TimeSpan.FromMilliseconds(10));
        }
    }
    /// <summary>
    /// item 滚动
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    async UniTask ItemRoll(RectTransform item)
    {
        float x = -(_showTextItemBornAtRect.rect.width / 2) - item.rect.width / 2;
        _itemRollTokenSource = new CancellationTokenSource();

        await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: _itemRollTokenSource.Token);
        while (Math.Abs(item.localPosition.x - x) > 1)
        {
            item.localPosition = Vector3.MoveTowards(item.localPosition, new Vector3(x, item.localPosition.y), 2);
            await UniTask.Delay(TimeSpan.FromMilliseconds(10), cancellationToken: _itemRollTokenSource.Token);
        }
        HideItem(item);
        await CloseFrame();
    }
    /// <summary>
    /// 显示信息向上移动
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    async UniTask ItemUp(RectTransform item)
    {
        float y = 0;
        int oldIndex = _showDataTexts.IndexOf(item) - 1;
        RectTransform oldItem = null;
        float oldY = 0;
        if (oldIndex >= 0)
        {
            oldItem = _showDataTexts[oldIndex];
            oldY = _showTextItemBornAtRect.rect.height / 2 + oldItem.rect.height / 2;
        }

        while (Math.Abs(item.localPosition.y - y) > 1)
        {
            if (oldItem != null)
            {
                oldItem.localPosition = Vector3.MoveTowards(oldItem.localPosition, new Vector3(oldItem.localPosition.x, oldY), 10);
            }
            item.localPosition = Vector3.MoveTowards(item.localPosition, new Vector3(item.localPosition.x, y), 10);
            await UniTask.Delay(TimeSpan.FromMilliseconds(10));
        }
        if (oldItem != null)
        {
            HideItem(oldItem);
        }
        await ItemRoll(item);
    }
    /// <summary>
    /// 创建item
    /// </summary>
    /// <param name="data"></param>
    async UniTask CreationShowItem(ChatInfo data)
    {
        RectTransform itemRect = GetItemRect();
        Text showText = itemRect.GetComponent<Text>();
        string prefix = "";
        if (_allPrivateChats.Contains(data))
        {
            prefix = StaticData.GetMultilingual(120318);
        }
        else if (_allWorldDatas.Contains(data))
        {
            prefix = StaticData.GetMultilingual(120317);
        }
        showText.text = prefix + data._message;
        itemRect.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(itemRect);
        await SetItemPoint(itemRect);
    }
    /// <summary>
    /// 设置item 位置
    /// </summary>
    /// <param name="item"></param>
    async UniTask SetItemPoint(RectTransform item)
    {
        float x = _showTextItemBornAtRect.rect.width / 2;
        x = -x + item.rect.width / 2;

        float y = 0;

        if (_showDataTexts != null && _showDataTexts.Count > 1)
        {
            y = -(_showTextItemBornAtRect.rect.height / 2) - (item.rect.height / 2);
            item.localPosition = new Vector3(x, y);
            await ItemUp(item);
        }
        else
        {
            item.localPosition = new Vector3(x, y);
            await ItemRoll(item);
        }


    }
    /// <summary>
    /// 获取item
    /// </summary>
    /// <returns></returns>
    RectTransform GetItemRect()
    {
        RectTransform item = null;
        if (_hidShowDataTexts != null && _hidShowDataTexts.Count > 0)
        {
            item = _hidShowDataTexts[0];
            _hidShowDataTexts.RemoveAt(0);
            _showDataTexts.Add(item);
        }
        else
        {
            item = Instantiate(_showTextItemParent, _showTextItemBornAtRect);
            _showDataTexts.Add(item);
        }
        return item;
    }
    /// <summary>
    /// 关闭item
    /// </summary>
    /// <param name="item"></param>
    void HideItem(RectTransform item)
    {
        item.gameObject.SetActive(false);
        _showDataTexts.Remove(item);
        _hidShowDataTexts.Add(item);
    }

    void MoveItem()
    {

    }
    #endregion
}
