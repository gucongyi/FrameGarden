using Company.Cfg;
using Game.Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 任务面板item类
/// </summary>
public class TaskPanelItem : MonoBehaviour
{
    #region 字段
    /// <summary>
    /// 自身ui组件
    /// </summary>
    RectTransform _thisRect;
    /// <summary>
    /// 顶部
    /// </summary>
    RectTransform _topRect;
    /// <summary>
    /// 标题
    /// </summary>
    Text _showTitleText;
    /// <summary>
    /// 底部
    /// </summary>
    RectTransform _bottomRect;
    /// <summary>
    /// 奖励说明文字
    /// </summary>
    Text _showTipText;
    /// <summary>
    /// 底部box
    /// </summary>
    RectTransform _boxRect;
    /// <summary>
    /// 小图标滑动组件
    /// </summary>
    ScrollRect _scrollRect;
    /// <summary>
    /// 确认按钮rect
    /// </summary>
    RectTransform _verifyBtnRect;
    /// <summary>
    /// 确认按钮
    /// </summary>
    Button _verifyBtn;
    /// <summary>
    /// 确认按钮图片展示
    /// </summary>
    Image _verifyBtnImage;
    /// <summary>
    /// 确认按钮显示文字
    /// </summary>
    Text _verifyBtnNameText;
    /// <summary>
    /// 任务数量显示
    /// </summary>
    Text _verifyBtnNumberText;
    /// <summary>
    /// 小图标克隆母体
    /// </summary>
    RectTransform _taskMinItemRect;
    /// <summary>
    /// 展示的数据
    /// </summary>
    TaskDefine _showData;
    /// <summary>
    /// 当前展示的奖励item
    /// </summary>
    List<TaskPanelAwardItem> _showAwardItems = new List<TaskPanelAwardItem>();
    /// <summary>
    /// 是否已经初始化
    /// </summary>
    bool _isInitial = false;
    TaskPanelController _controller;
    #endregion
    #region 属性
    /// <summary>
    /// 展示的数据
    /// </summary>
    public TaskDefine _ShowData { get { return _showData; } }
    #endregion
    #region 函数
    /// <summary>
    /// 初始化item
    /// </summary>
    /// <param name="tra"></param>
    /// <param name="data"></param>
    public void Initial(TaskDefine data, TaskPanelController controller)
    {
        //展示数据
        _showData = data;
        //item对象实列
        _thisRect = GetComponent<RectTransform>();
        /// <summary>
        /// 顶部
        /// </summary>
        _topRect = _thisRect.Find("Top").GetComponent<RectTransform>();
        /// <summary>
        /// 标题
        /// </summary>
        _showTitleText = _topRect.Find("TitleBg/Title").GetComponent<Text>();
        /// <summary>
        /// 底部
        /// </summary>
        _bottomRect = _thisRect.Find("Bottom").GetComponent<RectTransform>();
        /// <summary>
        /// 奖励说明文字
        /// </summary>
        _showTipText = _topRect.Find("ShowTip").GetComponent<Text>();
        /// <summary>
        /// 底部box
        /// </summary>
        _boxRect = _bottomRect.Find("Box").GetComponent<RectTransform>();
        /// <summary>
        /// 小图标滑动组件
        /// </summary>
        _scrollRect = _boxRect.Find("Scroll View").GetComponent<ScrollRect>();
        /// <summary>
        /// 确认按钮rect
        /// </summary>
        _verifyBtnRect = _boxRect.Find("VerifyBtn").GetComponent<RectTransform>();
        _verifyBtnImage = _verifyBtnRect.GetComponent<Image>();
        /// <summary>
        /// 确认按钮
        /// </summary>
        _verifyBtn = _verifyBtnRect.GetComponent<Button>();
        /// <summary>
        /// 确认按钮显示文字
        /// </summary>
        _verifyBtnNameText = _verifyBtnRect.Find("Name").GetComponent<Text>();
        /// <summary>
        /// 任务数量显示
        /// </summary>
        _verifyBtnNumberText = _verifyBtnRect.Find("NumberBg/Number").GetComponent<Text>();
        /// <summary>
        /// 小图标克隆母体
        /// </summary>
        _taskMinItemRect = _boxRect.Find("TaskMinItem").GetComponent<RectTransform>();
        _controller = controller;
        ShowData();
        _isInitial = true;
    }
    /// <summary>
    /// 展示数据
    /// </summary>
    public void ShowData()
    {
        _showTitleText.text = StaticData.GetMultilingual(_showData.TaskName);
        string showTipStr = StaticData.GetMultilingual(_showData.TaskDetails);
        showTipStr = string.Format(showTipStr, _showData.FinishNum);
        _showTipText.text = showTipStr;
        int currIndex = _controller.GetTaskFinishNumber(_showData);
        int maxIndex = _showData.FinishNum;

        _verifyBtn.onClick.RemoveAllListeners();
        _verifyBtn.enabled = true;
        if (currIndex >= maxIndex)
        {
            //是否已领取
            bool isGet = _controller.IsAlreadyGetAward(_showData);
            if (isGet)
            {
                _verifyBtnImage.sprite = _controller.GetBtnSprite(1);
                _verifyBtnNameText.text = "领取奖励";
                _verifyBtn.onClick.AddListener(GetAward);
            }
            else
            {
                _verifyBtn.enabled = false;
                _verifyBtnImage.sprite = _controller.GetBtnSprite(2);
                _verifyBtnNameText.text = "已领取";

            }
            _verifyBtnNumberText.text = currIndex + "/" + maxIndex;
        }
        else
        {
            _verifyBtnNumberText.text = "<color=red>" + currIndex + "</color>" + "/" + maxIndex;
            _verifyBtnImage.sprite = _controller.GetBtnSprite(0);
            _verifyBtnNameText.text = "前往";
            _verifyBtn.onClick.AddListener(LeaveFor);
        }
        DisposeAllCurrShowItem();
        for (int i = 0; i < _showData.TaskAward.Count; i++)
        {
            GoodIDCount goodIDCount = _showData.TaskAward[i];
            CreationItem(goodIDCount);
        }
        if (_scrollRect.content.rect.width > _scrollRect.transform.GetComponent<RectTransform>().rect.width)
        {
            _scrollRect.enabled = true;
        }
        else
        {
            _scrollRect.enabled = false;
        }
        _thisRect.gameObject.SetActive(true);
    }
    /// <summary>
    /// 前往对应场景
    /// </summary>
    public void LeaveFor()
    {
        _controller.Skip(() => { TaskPanelTool.SkipTaskScene(_showData.SceneTag); });

    }
    /// <summary>
    /// 领取奖励
    /// </summary>
    public void GetAward()
    {
        List<TaskDefine> taskDefines = new List<TaskDefine>();
        taskDefines.Add(_showData);
        TaskPanelTool.GetTaskAward(taskDefines, false, GetAwardAction);
    }
    /// <summary>
    /// 领取奖励回调
    /// </summary>
    /// <param name="isSucceed"></param>
    void GetAwardAction(bool isSucceed, SCGetTaskAward data)
    {
        if (isSucceed)
        {
            Debug.Log("领取奖励成功");
            Debug.Log(data);
            _controller.GetAward(data);
        }
        else
        {
            Debug.Log("领取奖励失败");
        }
    }
    /// <summary>
    /// 创建item实列
    /// </summary>
    public void CreationItem(GoodIDCount data)
    {
        Transform item = GameObject.Instantiate(_taskMinItemRect, _scrollRect.content);
        TaskPanelAwardItem taskPanelAwardItem = new TaskPanelAwardItem();
        taskPanelAwardItem.Initial(item.GetComponent<RectTransform>(), data);
        _showAwardItems.Add(taskPanelAwardItem);
    }
    /// <summary>
    /// 清理当前展示的所有item
    /// </summary>
    public void DisposeAllCurrShowItem()
    {
        for (int i = 0; i < _showAwardItems.Count; i++)
        {
            _showAwardItems[i].Dispose();
        }
        _showAwardItems.Clear();
    }
    /// <summary>
    /// 卸载
    /// </summary>
    public void Dispose()
    {
        GameObject.Destroy(_thisRect.gameObject);
    }
    /// <summary>
    /// 设置item层级
    /// </summary>
    /// <param name="index"></param>
    public void SetHierarchy(int index)
    {
        _thisRect.SetSiblingIndex(index);
    }
    #endregion
}
