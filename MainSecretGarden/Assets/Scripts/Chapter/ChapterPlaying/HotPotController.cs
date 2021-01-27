using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 章节吃火锅玩法
/// </summary>
public class HotPotController : MonoBehaviour
{
    #region 字段
    /// <summary>
    /// 背景
    /// </summary>
    RectTransform _bgRect;
    /// <summary>
    /// 锅
    /// </summary>
    RectTransform _wokRect;
    /// <summary>
    /// 食物item
    /// </summary>
    [SerializeField]
    List<UIPanelDrag> _items = new List<UIPanelDrag>();
    /// <summary>
    /// 食物增加值
    /// </summary>
    [SerializeField]
    List<int> _foodEnergyValue = new List<int>();
    /// <summary>
    /// 滑动球速度
    /// </summary>
    [SerializeField]
    List<int> _speeds = new List<int>();
    /// <summary>
    /// 滑动球刷新率
    /// </summary>
    [SerializeField]
    int _refreshRate;
    /// <summary>
    /// 一共需要多少个能量块
    /// </summary>
    [SerializeField]
    int _tageEnergyValue;
    /// <summary>
    /// 增加好感的对象
    /// </summary>
    [SerializeField]
    int _addGoodFeelingRoleID;
    /// <summary>
    /// 饱食度进度条Box
    /// </summary>
    RectTransform _lifebarBoxRect;
    /// <summary>
    /// 饱食度进度条
    /// </summary>
    RectTransform _lifebarRect;
    /// <summary>
    /// 饱食度item母体
    /// </summary>
    Transform _lifebarItem;
    /// <summary>
    /// 饱食度标题
    /// </summary>
    Text _lifebarTitleText;
    /// <summary>
    /// 食物状态操作栏Box
    /// </summary>
    RectTransform _operationBoxRect;
    /// <summary>
    /// 食物状态操作条
    /// </summary>
    RectTransform _operationRect;
    /// <summary>
    /// 状态操作条小球
    /// </summary>
    RectTransform _globuleRect;
    /// <summary>
    /// 滑动条目标点
    /// </summary>
    RectTransform _operationTageRect;
    /// <summary>
    /// 食物状态说明box
    /// </summary>
    RectTransform _explainBoxRect;
    /// <summary>
    /// 食物状态标题
    /// </summary>
    Text _operationTitleText;
    /// <summary>
    /// 确认按钮
    /// </summary>
    Button _affirmBtn;
    /// <summary>
    /// 确认按钮文字显示
    /// </summary>
    Text _affirmBtnText;
    /// <summary>
    /// 结束回调
    /// </summary>
    Action _endAction;
    /// <summary>
    /// 拖拽完毕回调
    /// </summary>
    Action _dragAction;
    /// <summary>
    /// 拖动引导
    /// </summary>
    ChapterGuidance _chapterGuidance;
    /// <summary>
    /// 当前能量值
    /// </summary>
    int _currEnergyValue;
    /// <summary>
    /// 是否移动状态栏滑动球
    /// </summary>
    bool _isMoveGlobu = false;
    /// <summary>
    /// 是否显示下一步
    /// </summary>
    [SerializeField]
    bool _isOpenNextBtn = true;
    /// <summary>
    /// 是否已经初始化
    /// </summary>
    bool _isInitial = false;
    #endregion
    #region 函数
    private void Awake()
    {
        //if (!_isInitial)
        //{
        //    Initial(null, null);
        //}
    }
    // Start is called before the first frame update
    void Start()
    {
        //Initial(null, null);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Initial(Action endAction, Action dragAction)
    {
        _bgRect = transform.Find("Bg").GetComponent<RectTransform>();
        _wokRect = _bgRect.Find("Wok").GetComponent<RectTransform>();
        _lifebarBoxRect = _bgRect.Find("LifebarBox").GetComponent<RectTransform>();
        _lifebarItem = _lifebarBoxRect.Find("Energy");
        _lifebarRect = _lifebarBoxRect.Find("Lifebar").GetComponent<RectTransform>();
        _lifebarTitleText = _lifebarBoxRect.Find("Title").GetComponent<Text>();
        _operationBoxRect = _bgRect.Find("OperationBox").GetComponent<RectTransform>();
        _operationRect = _operationBoxRect.Find("Operation").GetComponent<RectTransform>();
        _globuleRect = _operationRect.Find("Globule").GetComponent<RectTransform>();
        _operationTageRect = _operationRect.Find("TagePoint").GetComponent<RectTransform>();
        _explainBoxRect = _operationBoxRect.Find("ExplainBox").GetComponent<RectTransform>();
        _operationTitleText = _operationBoxRect.Find("Title").GetComponent<Text>();
        _affirmBtn = transform.Find("AffirmBtn").GetComponent<Button>();
        _affirmBtnText = _affirmBtn.transform.Find("Text").GetComponent<Text>();
        _chapterGuidance = _bgRect.Find("ChapterGuidance").GetComponent<ChapterGuidance>();
        _chapterGuidance.gameObject.SetActive(true);
        _chapterGuidance.PlayGuidanceAnima(_chapterGuidance.transform.localPosition, _wokRect.localPosition);
        _affirmBtn.onClick.RemoveAllListeners();
        _affirmBtn.onClick.AddListener(OnClickAffirmBtn);
        InitialLifebar();
        InitialOperation();
        InitialFood();
        OpenAffirmBtn(false);
        _endAction = endAction;
        _dragAction = dragAction;

        _isInitial = true;
    }
    /// <summary>
    /// 初始化能量条
    /// </summary>
    public async void InitialLifebar()
    {
        _currEnergyValue = 0;
        for (int i = 0; i < _tageEnergyValue; i++)
        {
            Transform tageTra = Instantiate(_lifebarItem, _lifebarRect);
            tageTra.GetComponent<Image>().enabled = false;
            tageTra.gameObject.SetActive(true);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(_lifebarRect);
        await UniTask.Delay(TimeSpan.FromMilliseconds(10));
        LayoutRebuilder.ForceRebuildLayoutImmediate(_lifebarBoxRect);
    }
    /// <summary>
    /// 增加能量值
    /// </summary>
    /// <param name="index"></param>
    public void AddEnergyValue(int index)
    {
        int number = _currEnergyValue + index;
        if (number <= _tageEnergyValue)
        {
            if (number - 1 < _lifebarRect.childCount)
            {
                for (int i = _currEnergyValue; i < number; i++)
                {
                    Image image = _lifebarRect.GetChild(i).GetComponent<Image>();
                    if (!image.enabled)
                    {
                        image.enabled = true;
                    }
                }

                _currEnergyValue = number;
                if (_currEnergyValue >= _tageEnergyValue)
                {
                    Debug.Log("能量值已满");
                }
            }

        }
        else
        {
            Debug.Log("能量值已满");
        }
    }
    /// <summary>
    /// 初始化食物
    /// </summary>
    public void InitialFood()
    {
        for (int i = 0; i < _items.Count; i++)
        {
            UIPanelDrag item = _items[i];
            item.actionOnPointerDown = PointerDown;
            item.actionOnPointerUp = PointerUp;
            item.localPos = item.transform.localPosition;
        }
    }

    private void PointerDown(PointerEventData obj)
    {
        _chapterGuidance.gameObject.SetActive(false);
    }

    /// <summary>
    /// 鼠标放开
    /// </summary>
    /// <param name="obj"></param>
    private void PointerUp(PointerEventData obj)
    {
        UIPanelDrag item = obj.pointerDrag.transform.GetComponent<UIPanelDrag>();
        if (item != null && Vector3.Distance(item.transform.localPosition, _wokRect.localPosition) < 200)
        {
            item.gameObject.SetActive(false);
            OpenAffirmBtn(true);
            OpenOperation(true);
            int index = _items.IndexOf(item);

            MoveGlobu(_speeds[_currEnergyValue], _foodEnergyValue[_currEnergyValue], MoveEnd);
        }
    }
    /// <summary>
    /// 开关操作栏
    /// </summary>
    /// <param name="isOpen"></param>
    public void OpenOperation(bool isOpen)
    {
        _operationBoxRect.gameObject.SetActive(isOpen);
    }
    /// <summary>
    /// 初始化滑动条
    /// </summary>
    public void InitialOperation()
    {
        float minX = -(_operationRect.sizeDelta.x / 2);
        float tagex = minX + _globuleRect.sizeDelta.x + 5;
        _globuleRect.localPosition = new Vector3(tagex, _globuleRect.localPosition.y);
        OpenOperation(false);
    }
    /// <summary>
    /// 滑动球移动
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="endAction"></param>
    /// <returns></returns>
    public async UniTask MoveGlobu(int speed, int foodEnergyValue, Action<int, int> endAction)
    {

        float minX = -(_operationRect.sizeDelta.x / 2) + _globuleRect.sizeDelta.x + 5;
        float maxX = (_operationRect.sizeDelta.x / 2) - _globuleRect.sizeDelta.x + 5;
        _isMoveGlobu = true;
        while (_isMoveGlobu)
        {
            if (_globuleRect.localPosition.x < maxX)
            {
                while (_isMoveGlobu && _globuleRect.localPosition.x < maxX)
                {
                    _globuleRect.localPosition = new Vector3(_globuleRect.localPosition.x + speed, _globuleRect.localPosition.y);
                    await UniTask.Delay(TimeSpan.FromMilliseconds(_refreshRate));
                }

            }
            else if (_globuleRect.localPosition.x > minX)
            {
                while (_isMoveGlobu && _globuleRect.localPosition.x > minX)
                {
                    _globuleRect.localPosition = new Vector3(_globuleRect.localPosition.x - speed, _globuleRect.localPosition.y);
                    await UniTask.Delay(TimeSpan.FromMilliseconds(_refreshRate));
                }

            }
        }
        if (Vector3.Distance(_globuleRect.localPosition, _operationTageRect.localPosition) < (_operationTageRect.sizeDelta.x / 2) - (_globuleRect.sizeDelta.x / 2))
        {
            endAction?.Invoke(foodEnergyValue, 1);
        }
        else
        {
            endAction?.Invoke(foodEnergyValue, 0);
        }

    }

    public void MoveEnd(int foodEnergyValue, int type)
    {

        if (type == 1)
        {
            Debug.Log("增加好感度");
        }
        else
        {
            Debug.Log("不增加好感");
        }
        Debug.Log("移动结束");
        AddEnergyValue(foodEnergyValue);
        if (_currEnergyValue >= _tageEnergyValue)
        {
            Debug.Log("能量值已满");
            SetAffirmBtnShowText("下一步");
            _dragAction?.Invoke();
            if (_isOpenNextBtn)
            {
                OpenAffirmBtn(true);
            }
            else
            {
                OpenAffirmBtn(false);
            }
        }
        else
        {
            OpenAffirmBtn(false);
        }


        InitialOperation();
    }
    /// <summary>
    /// 开关确认按钮
    /// </summary>
    /// <param name="isOpen"></param>
    public void OpenAffirmBtn(bool isOpen)
    {
        _affirmBtn.gameObject.SetActive(isOpen);
    }
    public void SetAffirmBtnShowText(string showStr)
    {
        _affirmBtnText.text = showStr;
    }
    /// <summary>
    /// 点击确认
    /// </summary>
    public void OnClickAffirmBtn()
    {
        if (_currEnergyValue >= _tageEnergyValue)
        {
            Debug.Log("能量值已满");
            _endAction?.Invoke();
        }
        else
        {
            _isMoveGlobu = false;
        }

    }
    #endregion
}
