using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 条漫播放器
/// </summary>
public class CaricaturePlayerController : MonoBehaviour
{
    #region 字段
    /// <summary>
    /// 所有组件集合
    /// </summary>
    [SerializeField]
    List<CaricaturePlayerModule> _datas = new List<CaricaturePlayerModule>();
    /// <summary>
    /// 当前展示条漫
    /// </summary>
    List<CaricaturePlayerModule> _currShowCaricatures = new List<CaricaturePlayerModule>();
    /// <summary>
    /// 关闭的itme
    /// </summary>
    List<RectTransform> _hideItemRect = new List<RectTransform>();
    /// <summary>
    /// 自身的ui组件
    /// </summary>
    RectTransform _thisRect;
    /// <summary>
    /// 条漫组件实列母体
    /// </summary>
    RectTransform _caricatureItem;
    /// <summary>
    /// 播放结束回调
    /// </summary>
    Action _endAction;
    /// <summary>
    /// 播放步骤监听
    /// </summary>
    Action<int> _playMonitor;
    /// <summary>
    /// 背景点击按钮
    /// </summary>
    Button _bgBtn;
    /// <summary>
    /// ui控制组件
    /// </summary>
    CanvasGroup _canvasGroup;
    CaricaturePlayerModule _currShowItem;
    /// <summary>
    /// 点击计数
    /// </summary>
    int _clickIndex = 0;
    /// <summary>
    /// 第一屏是否展示完毕
    /// </summary>
    bool _theFirstScreen = false;
    /// <summary>
    /// 是否启用淡出效果
    /// </summary>
    [SerializeField]
    bool _isFadeOut = true;
    #endregion
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// 初始化播放器
    /// </summary>
    /// <param name="endAction"></param>
    /// <param name="playMonitor"></param>
    public void Initial(Action endAction, Action<int> playMonitor)
    {
        _thisRect = GetComponent<RectTransform>();
        _caricatureItem = transform.Find("CaricatureItem").GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _bgBtn = GetComponent<Button>();
        _bgBtn.onClick.RemoveAllListeners();
        _bgBtn.onClick.AddListener(ClickBtn);
        OpenClickBtn(false);
        _endAction = endAction;
        _playMonitor = playMonitor;
        ClickBtn();
    }
    /// <summary>
    /// 创建图片item
    /// </summary>
    /// <returns></returns>
    RectTransform CreationIamge()
    {
        RectTransform imageRect = GameObject.Instantiate(_caricatureItem.gameObject, transform).GetComponent<RectTransform>();
        return imageRect;
    }
    public RectTransform GetShowRect()
    {
        RectTransform imteRect = null;
        if (_hideItemRect.Count > 0)
        {
            imteRect = _hideItemRect[0];
            _hideItemRect.RemoveAt(0);
        }
        else
        {
            imteRect = CreationIamge();
        }
        return imteRect;
    }

    public void HideItem(CaricaturePlayerModule item)
    {
        if (item._CaricaturePlayerSpecialModuleBasics != null)
        {
            Destroy(item._ThisRect.gameObject);
            return;
        }
        if (_hideItemRect.Contains(item._ThisRect))
        {
            return;
        }
        if (_datas.IndexOf(item) == 0)
        {
            _theFirstScreen = true;
        }
        _hideItemRect.Add(item._ThisRect);
        item.Dispose();
        item._ThisRect.gameObject.SetActive(false);
        item._ThisRect = null;
    }
    /// <summary>
    /// 展示条漫
    /// </summary>
    /// <param name="index"></param>
    public async void Show(CaricaturePlayerModule currItem)
    {
        int index = 0;
        if (currItem != null)
        {
            index = _datas.IndexOf(currItem);
            index = index + 1;
        }
        if (index < _datas.Count)
        {
            List<CaricaturePlayerModule> items = GetShowItems(index);
            for (int i = 0; i < items.Count; i++)
            {
                CaricaturePlayerModule item = items[i];
                if (item._CaricaturePlayerSpecialModuleBasics != null)
                {
                    item.InitialSpecialModule(OpenClickBtn, () => { OpenClickBtn(true); }, transform);
                }
                else
                {
                    if (item == _datas[_datas.Count - 1])
                    {
                        item.Initial(OpenClickBtn, () => { ClickBtn(); });
                    }
                    else
                    {
                        item.Initial(OpenClickBtn, () => { OpenClickBtn(true); });
                    }

                }

            }
            MoveItems(_currShowCaricatures, items);
        }
        else
        {
            Debug.Log("条漫播放完毕");
            await Close();
        }
    }
    #region 废弃
    /// <summary>
    /// 展示条漫
    /// </summary>
    /// <param name = "index" ></ param >
    //public async void Show(int index)
    //{

    //    if (index < _datas.Count)
    //    {
    //        List<CaricaturePlayerModule> items = GetShowItems(index);
    //        for (int i = 0; i < items.Count; i++)
    //        {
    //            CaricaturePlayerModule item = items[i];
    //            if (item._CaricaturePlayerSpecialModuleBasics != null)
    //            {
    //                item.InitialSpecialModule(() => { OpenClickBtn(true); }, transform);
    //            }
    //            else
    //            {
    //                item.Initial(OpenIsBeingSpeak, CreationIamge(), () => { OpenClickBtn(true); }, () => { OpenClickBtn(true); }, null);
    //            }
    //        }
    //        _currShowItem = items[0];
    //        _currShowCaricatures.AddRange(items);
    //        RefreshInitialPoint(items);

    //        CaricaturePlayerModule caricaturePlayerModule = _datas[index];
    //        if (caricaturePlayerModule._CaricaturePlayerSpecialModuleBasics != null)
    //        {
    //            caricaturePlayerModule.InitialSpecialModule(() => { OpenClickBtn(true); }, transform);
    //        }
    //        else
    //        {
    //            caricaturePlayerModule.Initial(OpenIsBeingSpeak, CreationIamge(), () => { OpenClickBtn(true); }, () => { OpenClickBtn(true); }, null);
    //        }
    //        _currShowItem = caricaturePlayerModule;
    //        _currShowCaricatures.Add(caricaturePlayerModule);
    //        RefreshInitialPoint(caricaturePlayerModule);
    //    }
    //    else
    //    {
    //        Debug.Log("条漫播放完毕");
    //        await Close();
    //    }
    //}

    ///// <summary>
    ///// 整体移动
    ///// </summary>
    ///// <param name="tages"></param>
    ///// <param name="currItem"></param>
    ///// <returns></returns>
    //public async UniTask OverallMove(List<CaricaturePlayerModule> tages, CaricaturePlayerModule currItem)
    //{
    //    Vector3 tagePoint = Vector3.zero;
    //    float speed = 0.8f;
    //    int index = 0;
    //    for (int i = tages.Count - 1; i >= 0; i--)
    //    {
    //        CaricaturePlayerModule item = tages[i];
    //        if (i != tages.Count - 1)
    //        {
    //            speed = 1;
    //            ChapterTool.MoveUi(item._ThisRect, tagePoint, speed, 0.1f, null, () => { index = index + 1; /*Debug.Log("移动完毕");*/ });
    //        }
    //        else
    //        {
    //            if (item._Hierarchy == -1)
    //            {
    //                int siblingIndex = tages[i - 1]._ThisRect.childCount - 1;
    //                if (siblingIndex < 0)
    //                {
    //                    siblingIndex = 0;
    //                }
    //                item._ThisRect.SetSiblingIndex(siblingIndex);
    //            }

    //            tagePoint = GetMoveOrBornTagePoint(item, true);
    //            if (item._IsMove)
    //            {
    //                ChapterTool.MoveUi(item._ThisRect, tagePoint, speed, 0.1f, null, () => { index = index + 1; /*Debug.Log("移动完毕"); */});
    //            }
    //            else
    //            {
    //                item._ThisRect.localPosition = tagePoint;
    //            }
    //        }
    //        float newY = tagePoint.y + item._Interval + (item._ThisRect.rect.height / 2);
    //        float newX = 0;
    //        if (i - 1 >= 0)
    //        {
    //            newY += tages[i - 1]._ThisRect.rect.height / 2;
    //            newX = tages[i - 1]._ThisRect.localPosition.x;
    //        }
    //        tagePoint = new Vector3(newX, newY, 0);

    //    }
    //    await UniTask.WaitUntil(() => index == tages.Count);
    //    _clickIndex++;
    //    if (!_isBeingSpeak)
    //    {
    //        OpenClickBtn(true);
    //    }
    //    _currShowItem.MoveEnd();
    //    if (_currShowItem._Dialogues == null || _currShowItem._Dialogues.Count <= 0)
    //    {
    //        //await UniTask.Delay(TimeSpan.FromMilliseconds(10));
    //        ClickBtn();
    //    }
    //    else
    //    {
    //        ClickBtn();
    //    }
    //}
    ///// <summary>
    ///// 设置图片初始位置
    ///// </summary>
    ///// <param name="currItem"></param>
    //async void RefreshInitialPoint(CaricaturePlayerModule currItem)
    //{
    //    await currItem.Show();
    //    OpenClickBtn(false);
    //    int currIndex = _currShowCaricatures.IndexOf(currItem);

    //    currItem.SetPoint(GetMoveOrBornTagePoint(currItem, false));

    //    if (currIndex == 0)
    //    {
    //        if (currItem._IsMove)
    //        {
    //            await ChapterTool.MoveUi(currItem._ThisRect, GetMoveOrBornTagePoint(currItem, true), 1.5f, 0.3f, null, () =>
    //            {
    //                //Debug.Log("移动完毕");
    //                if (!_isBeingSpeak)
    //                {
    //                    OpenClickBtn(true);
    //                }
    //                _clickIndex++;
    //                _currShowItem.MoveEnd();
    //                ClickBtn();
    //            });

    //        }
    //        else
    //        {
    //            currItem._ThisRect.localPosition = GetMoveOrBornTagePoint(currItem, true);
    //        }

    //    }
    //    else
    //    {
    //        await OverallMove(_currShowCaricatures, currItem);
    //    }
    //    _playMonitor?.Invoke(_clickIndex);
    //}
    ///// <summary>
    ///// 设置图片初始位置
    ///// </summary>
    ///// <param name="currItem"></param>
    //async void RefreshInitialPoint(List<CaricaturePlayerModule> currItems)
    //{
    //    OpenClickBtn(false);
    //    for (int i = 0; i < currItems.Count; i++)
    //    {
    //        await currItems[i].Show();
    //        currItems[i].SetPoint(GetMoveOrBornTagePoint(currItems[i], false));
    //    }
    //    int currIndex = _currShowCaricatures.IndexOf(currItems[0]);
    //    if (currIndex == 0)
    //    {
    //        if (currItems[0]._IsMove)
    //        {
    //            await ChapterTool.MoveUi(currItems[0]._ThisRect, GetMoveOrBornTagePoint(currItems[0], true), 1.5f, 0.3f, null, () =>
    //            {
    //                //Debug.Log("移动完毕");
    //                if (!_isBeingSpeak)
    //                {
    //                    OpenClickBtn(true);
    //                }
    //                _clickIndex++;
    //                _currShowItem.MoveEnd();
    //                ClickBtn();
    //            });

    //        }
    //        else
    //        {
    //            currItems[0]._ThisRect.localPosition = GetMoveOrBornTagePoint(currItems[0], true);
    //        }

    //    }
    //    else
    //    {
    //        await OverallMove(_currShowCaricatures);
    //    }
    //    _playMonitor?.Invoke(_clickIndex);
    //}
    ///// <summary>
    ///// 整体移动
    ///// </summary>
    ///// <param name="tages"></param>
    ///// <param name="currItem"></param>
    ///// <returns></returns>
    //public async UniTask OverallMove(List<CaricaturePlayerModule> tages)
    //{
    //    Vector3 tagePoint = Vector3.zero;
    //    float speed = 0.8f;
    //    int index = 0;
    //    for (int i = tages.Count - 1; i >= 0; i--)
    //    {
    //        CaricaturePlayerModule item = tages[i];
    //        if (i != tages.Count - 1)
    //        {
    //            speed = 1;
    //            ChapterTool.MoveUi(item._ThisRect, tagePoint, speed, 0.1f, null, () => { index = index + 1; /*Debug.Log("移动完毕");*/ });
    //        }
    //        else
    //        {
    //            if (item._Hierarchy == -1)
    //            {
    //                int siblingIndex = tages[i - 1]._ThisRect.childCount - 1;
    //                if (siblingIndex < 0)
    //                {
    //                    siblingIndex = 0;
    //                }
    //                item._ThisRect.SetSiblingIndex(siblingIndex);
    //            }

    //            tagePoint = GetMoveOrBornTagePoint(item, true);
    //            if (item._IsMove)
    //            {
    //                ChapterTool.MoveUi(item._ThisRect, tagePoint, speed, 0.1f, null, () => { index = index + 1; /*Debug.Log("移动完毕"); */});
    //            }
    //            else
    //            {
    //                item._ThisRect.localPosition = tagePoint;
    //            }
    //        }
    //        float newY = tagePoint.y + item._Interval + (item._ThisRect.rect.height / 2);
    //        float newX = 0;
    //        if (i - 1 >= 0)
    //        {
    //            newY += tages[i - 1]._ThisRect.rect.height / 2;
    //            newX = tages[i - 1]._ThisRect.localPosition.x;
    //        }
    //        tagePoint = new Vector3(newX, newY, 0);

    //    }
    //    await UniTask.WaitUntil(() => index == tages.Count);
    //    _clickIndex = _currShowCaricatures.IndexOf(tages[tages.Count - 1]);
    //    _clickIndex++;
    //    if (!_isBeingSpeak)
    //    {
    //        OpenClickBtn(true);
    //    }
    //    _currShowItem.MoveEnd();
    //    if (_currShowItem._Dialogues == null || _currShowItem._Dialogues.Count <= 0)
    //    {
    //        //await UniTask.Delay(TimeSpan.FromMilliseconds(10));
    //        ClickBtn();
    //    }
    //    else
    //    {
    //        ClickBtn();
    //    }
    //}

    ///// <summary>
    ///// 获取出生点或者移动目标点
    ///// </summary>
    ///// <param name="data"></param>
    ///// <param name="isMoveOrBorn"></param>
    ///// <returns></returns>
    //Vector3 GetMoveOrBornTagePoint(CaricaturePlayerModule data, bool isMoveOrBorn)
    //{
    //    float maxHeight = _thisRect.rect.height;
    //    float maxWide = _thisRect.rect.width;
    //    float itemHeight = data._ThisRect.rect.height;
    //    float itemWide = data._ThisRect.rect.width;
    //    float tageY = 0;
    //    float tageX = 0;
    //    Vector3 tagePoint = Vector3.zero;
    //    Vector3 moveTagePoint = Vector3.zero;
    //    switch (data._DirectionType)
    //    {
    //        case ChapterTool.CaricatureModuleDirectionType.Up:
    //            tageY = (maxHeight / 2) + (itemHeight / 2);
    //            tagePoint = new Vector3(tageX, -tageY);
    //            break;
    //        case ChapterTool.CaricatureModuleDirectionType.Left:
    //            tageX = (maxWide / 2) + (itemWide / 2);
    //            tagePoint = new Vector3(tageX, tageY);
    //            moveTagePoint = new Vector3((maxWide / 2) - (itemWide / 2), 0);
    //            break;
    //        case ChapterTool.CaricatureModuleDirectionType.Right:
    //            tageX = (maxWide / 2) + (itemWide / 2);
    //            tagePoint = new Vector3(-tageX, tageY);
    //            moveTagePoint = new Vector3((-(maxWide / 2)) + (itemWide / 2), 0);
    //            break;
    //    }
    //    if (isMoveOrBorn)
    //    {
    //        moveTagePoint = new Vector3(moveTagePoint.x + data._SkewingValue.x, moveTagePoint.y + data._SkewingValue.y, moveTagePoint.z + data._SkewingValue.z);
    //        return moveTagePoint;
    //    }
    //    else
    //    {
    //        tagePoint = new Vector3(tagePoint.x + data._SkewingValue.x, tagePoint.y + data._SkewingValue.y, tagePoint.z + data._SkewingValue.z);
    //        return tagePoint;
    //    }

    //}
    #endregion
    /// <summary>
    /// 获取需要展示的item
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    List<CaricaturePlayerModule> GetShowItems(int index)
    {
        List<CaricaturePlayerModule> showItems = new List<CaricaturePlayerModule>();
        if (index == 0)
        {
            showItems.Add(_datas[index]);
        }
        else
        {
            if (index < _datas.Count)
            {
                CaricaturePlayerModule currItem = _datas[index];
                showItems.Add(currItem);
                int indexValue = index;
                while (indexValue < _datas.Count && (currItem._Dialogues == null || currItem._Dialogues.Count <= 0) && (currItem._CaricaturePlayerSpecialModuleBasics == null || !currItem._CaricaturePlayerSpecialModuleBasics._isOperation))
                {
                    indexValue++;
                    if (indexValue < _datas.Count)
                    {
                        currItem = _datas[indexValue];
                        showItems.Add(currItem);
                    }
                    else
                    {
                        return showItems;
                    }

                }
            }
        }

        return showItems;

    }
    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="datas"></param>
    async void MoveItems(List<CaricaturePlayerModule> oldItems, List<CaricaturePlayerModule> datas)
    {

        List<CaricaturePlayerModule> oldItemList = new List<CaricaturePlayerModule>();
        oldItemList.AddRange(oldItems);
        List<CaricaturePlayerModule> newItemList = new List<CaricaturePlayerModule>();
        newItemList.AddRange(datas);
        List<CaricaturePlayerModule> moveItems = new List<CaricaturePlayerModule>();

        List<int> hidItems = new List<int>();
        int initialItemIndex = 0;
        for (int i = 0; i < datas.Count; i++)
        {
            CaricaturePlayerModule initialItem = datas[i];
            if (initialItemIndex >= 1)
            {
                break;
            }
            else
            {
                if (initialItem._CaricaturePlayerSpecialModuleBasics == null)
                {
                    await initialItem.Show(GetShowRect());
                }
                else
                {
                    initialItem.Show();
                }
                //设置初始位置
                CaricaturePlayerModule oldItem = null;
                if (oldItemList != null && oldItemList.Count > 0)
                {
                    oldItem = oldItemList[oldItemList.Count - 1];
                }
                initialItem._ThisRect.localPosition = GetBornTagePoint(initialItem, oldItem);
                moveItems.Add(initialItem);
                newItemList.Remove(initialItem);
                initialItemIndex++;
                if (initialItem._CaricaturePlayerSpecialModuleBasics != null)
                {
                    Debug.Log("初始");
                }
            }
        }
        //条漫第一格是否移动完毕
        bool isOneItem = false;
        //将旧item加入移动队列
        if (oldItemList != null && oldItemList.Count > 0)
        {
            float minY = _thisRect.rect.height / 2;
            minY = -(minY) + moveItems[0]._ThisRect.rect.height / 2;

            if (_theFirstScreen)
            {
                List<CaricaturePlayerModule> newMoveItems = new List<CaricaturePlayerModule>();
                newMoveItems.AddRange(oldItemList);
                newMoveItems.AddRange(moveItems);
                oldItemList.Clear();
                moveItems.Clear();
                moveItems.AddRange(newMoveItems);
            }

            isOneItem = false;
        }
        else
        {
            isOneItem = true;
        }
        bool _isMove = true;
        //获取初始移动终点
        List<Vector3> tagePoints = new List<Vector3>();
        if (!_theFirstScreen && oldItemList != null && oldItemList.Count > 0)
        {
            tagePoints = GetMoveEndPoint(moveItems, null, oldItemList[oldItemList.Count - 1], isOneItem);
        }
        else
        {
            tagePoints = GetMoveEndPoint(moveItems, null, null, isOneItem);
        }
        while (_isMove)
        {
            hidItems.Clear();
            for (int i = 0; i < moveItems.Count; i++)
            {
                CaricaturePlayerModule moveItem = moveItems[i];

                //修改item层级
                if (i != 0)
                {
                    CaricaturePlayerModule lastMonveItem = moveItems[i - 1];
                    int siblingIndex = lastMonveItem._ThisRect.GetSiblingIndex();
                    switch (moveItem._Hierarchy)
                    {
                        case 0:
                        case 1:
                            siblingIndex = siblingIndex + 1;
                            break;
                        case -1:
                            siblingIndex = siblingIndex - 1;
                            break;
                    }
                    moveItem._ThisRect.SetSiblingIndex(siblingIndex);
                }
                Vector3 tagePoint = tagePoints[i];
                float speed = 5;
                //左右插入速度更改
                if (moveItem._DirectionType != ChapterTool.CaricatureModuleDirectionType.Up && Mathf.Abs(moveItem._ThisRect.localPosition.x - tagePoint.x) > 1)
                {
                    speed = 20;
                }
                else
                {
                    speed = 18;
                }
                moveItem._ThisRect.localPosition = Vector3.MoveTowards(moveItem._ThisRect.localPosition, tagePoint, speed);
                //判断是否超出屏幕
                float maxY = (_thisRect.rect.height / 2) + moveItem._ThisRect.rect.height / 2;
                if (moveItem._ThisRect.localPosition.y >= maxY)
                {
                    hidItems.Add(i);
                }
            }
            //关闭超出屏幕的item
            for (int i = 0; i < hidItems.Count; i++)
            {
                HideItem(moveItems[hidItems[i]]);
                moveItems.RemoveAt(hidItems[i]);
                tagePoints.RemoveAt(hidItems[i]);
            }

            //将旧item加入移动队列
            if (oldItemList != null && oldItemList.Count > 0)
            {
                CaricaturePlayerModule oldLastItem = oldItemList[oldItemList.Count - 1];
                CaricaturePlayerModule moveOneItem = moveItems[0];

                float tageY = oldLastItem._ThisRect.localPosition.y - oldLastItem._ThisRect.rect.height / 2 - moveOneItem._ThisRect.rect.height / 2 - moveOneItem._Interval;
                Vector3 tagePoint = new Vector3(moveOneItem._ThisRect.localPosition.x, tageY, moveOneItem._ThisRect.localPosition.z);
                Debug.Log("_____________________________" + tagePoint);
                Debug.Log("_____________________________" + _theFirstScreen);
                Debug.Log("_____________________________" + Vector3.Distance(tagePoint, moveOneItem._ThisRect.localPosition));

                float minY = _thisRect.rect.height / 2;
                minY = -(minY) + moveOneItem._ThisRect.rect.height / 2;
                Debug.Log("_____________________________" + (moveOneItem._ThisRect.localPosition.y < minY));
                if (/*!_theFirstScreen &&*/ /*moveOneItem._ThisRect.localPosition.y< tagePoint.y*/Vector3.Distance(tagePoint, moveOneItem._ThisRect.localPosition) < 10 && moveOneItem._ThisRect.localPosition.y < minY)
                {
                    List<CaricaturePlayerModule> newMoveItems = new List<CaricaturePlayerModule>();
                    newMoveItems.AddRange(oldItemList);
                    newMoveItems.AddRange(moveItems);
                    oldItemList.Clear();
                    moveItems.Clear();
                    moveItems.AddRange(newMoveItems);
                    if (newItemList == null || newItemList.Count <= 0)
                    {
                        tagePoints = GetMoveEndPoint(moveItems);
                    }
                }
                else
                {
                    if (newItemList == null || newItemList.Count <= 0)
                    {
                        tagePoints = GetMoveEndPoint(moveItems, null, oldLastItem);
                    }
                }
            }
            //将下一个item加入移动队列
            if (newItemList != null && newItemList.Count > 0)
            {
                CaricaturePlayerModule newFirstItem = newItemList[0];
                if (newFirstItem._CaricaturePlayerSpecialModuleBasics == null)
                {
                    //提前生成下一个item
                    if (!newFirstItem._IsShow)
                    {
                        newFirstItem.Show(GetShowRect());
                    }
                }
                else
                {
                    newFirstItem.Show();
                }

                newFirstItem._ThisRect.localPosition = GetBornTagePoint(newFirstItem);
                CaricaturePlayerModule moveLastItem = moveItems[moveItems.Count - 1];
                float yValue = (moveLastItem._ThisRect.localPosition.y - moveLastItem._ThisRect.rect.height / 2) - newFirstItem._Interval - newFirstItem._ThisRect.rect.height / 2;
                if (newFirstItem._ThisRect.localPosition.y <= yValue)
                {
                    moveItems.Add(newFirstItem);
                    newItemList.Remove(newFirstItem);
                }
                //更新移动目标终点
                if (newItemList != null && newItemList.Count > 0)
                {
                    tagePoints = GetMoveEndPoint(moveItems, newItemList[0]);
                }
                else
                {
                    tagePoints = GetMoveEndPoint(moveItems);
                }
            }
            else
            {
                if ((newItemList == null || newItemList.Count <= 0) && Vector3.Distance(moveItems[moveItems.Count - 1]._ThisRect.localPosition, tagePoints[tagePoints.Count - 1]) <= 1)
                {
                    _isMove = false;
                }
            }
            await UniTask.Delay(TimeSpan.FromMilliseconds(10));
        }
        _currShowCaricatures.Clear();
        if (oldItemList != null && oldItemList.Count > 0)
        {
            _currShowCaricatures.AddRange(oldItemList);
        }
        _currShowCaricatures.AddRange(moveItems);
        _currShowItem = _currShowCaricatures[_currShowCaricatures.Count - 1];
        if (_currShowItem != null)
        {
            _currShowItem.MoveEnd();
            if ((_currShowItem._Dialogues == null || _currShowItem._Dialogues.Count <= 0) && isOneItem)
            {
                ClickBtn();
            }

        }
        Debug.Log("移动完毕");
    }
    /// <summary>
    /// 获取移动的目标点
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    List<Vector3> GetMoveEndPoint(List<CaricaturePlayerModule> items, CaricaturePlayerModule aboutInsertionItem = null, CaricaturePlayerModule oldItem = null, bool isOneItem = false)
    {
        List<Vector3> tagePoints = new List<Vector3>();
        for (int i = items.Count - 1; i >= 0; i--)
        {
            CaricaturePlayerModule item = items[i];
            float maxHeight = (_thisRect.rect.height / 2) + item._ThisRect.rect.height / 2;
            float maxWide = 0;
            float y = 0;
            float x = 0;
            if (item._DirectionType == ChapterTool.CaricatureModuleDirectionType.Up)
            {
                x = item._ThisRect.localPosition.x;
            }
            else
            {
                switch (item._DirectionType)
                {
                    case ChapterTool.CaricatureModuleDirectionType.Left:
                        maxWide = (_thisRect.rect.width / 2) - item._ThisRect.rect.width / 2;
                        x = maxWide + item._SkewingValue.x;
                        break;
                    case ChapterTool.CaricatureModuleDirectionType.Right:
                        maxWide = -(_thisRect.rect.width / 2) + item._ThisRect.rect.width / 2;
                        x = maxWide + item._SkewingValue.x;
                        break;
                }
            }


            if (tagePoints.Count <= 0)
            {
                if (aboutInsertionItem == null)
                {
                    float bottomY = _thisRect.rect.height / 2;
                    if (oldItem == null)
                    {
                        if (isOneItem)
                        {
                            bottomY = (bottomY) - item._ThisRect.rect.height / 2;

                        }
                        else
                        {
                            bottomY = -(bottomY) + item._ThisRect.rect.height / 2;
                        }
                    }
                    else
                    {
                        bottomY = oldItem._ThisRect.localPosition.y - oldItem._ThisRect.rect.height / 2 - item._ThisRect.rect.height / 2 - item._Interval;
                    }

                    tagePoints.Add(new Vector3(x, bottomY));
                }
                else
                {
                    float bottomY = _thisRect.rect.height / 2;
                    if (aboutInsertionItem._DirectionType == ChapterTool.CaricatureModuleDirectionType.Up)
                    {
                        if (item._DirectionType != ChapterTool.CaricatureModuleDirectionType.Up)
                        {

                            switch (item._DirectionType)
                            {
                                case ChapterTool.CaricatureModuleDirectionType.Left:
                                    maxWide = (_thisRect.rect.width / 2) - item._ThisRect.rect.width / 2;
                                    x = maxWide + item._SkewingValue.x;
                                    if (Mathf.Abs(item._ThisRect.localPosition.x - x) > 1)
                                    {
                                        bottomY = -(bottomY) + item._ThisRect.rect.height / 2;
                                    }
                                    else
                                    {
                                        bottomY = -(bottomY) + item._ThisRect.rect.height / 2 + aboutInsertionItem._Interval;
                                    }
                                    break;
                                case ChapterTool.CaricatureModuleDirectionType.Right:
                                    maxWide = -(_thisRect.rect.width / 2) + item._ThisRect.rect.width / 2;
                                    x = maxWide + item._SkewingValue.x;
                                    if (Mathf.Abs(item._ThisRect.localPosition.x - x) > 1)
                                    {
                                        bottomY = -(bottomY) + item._ThisRect.rect.height / 2;
                                    }
                                    else
                                    {
                                        bottomY = -(bottomY) + item._ThisRect.rect.height / 2 + aboutInsertionItem._Interval;
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            bottomY = -(bottomY) + item._ThisRect.rect.height / 2 + aboutInsertionItem._Interval;
                        }

                    }
                    else
                    {
                        if (!aboutInsertionItem._IsShow)
                        {
                            aboutInsertionItem.Show(GetShowRect());
                            aboutInsertionItem._ThisRect.localPosition = GetBornTagePoint(aboutInsertionItem);
                        }
                        bottomY = aboutInsertionItem._ThisRect.localPosition.y + item._ThisRect.rect.height / 2 + aboutInsertionItem._Interval + aboutInsertionItem._ThisRect.rect.height / 2;
                    }
                    tagePoints.Add(new Vector3(x, bottomY));
                }
            }
            else
            {
                Vector3 lastPoint = tagePoints[tagePoints.Count - 1];
                Vector3 tagePoint = Vector3.zero;
                y = lastPoint.y + items[i + 1]._ThisRect.rect.height / 2 + items[i + 1]._Interval + item._ThisRect.rect.height / 2 + item._SkewingValue.y;
                if (y > maxHeight)
                {
                    y = maxHeight;
                }

                switch (item._DirectionType)
                {
                    case ChapterTool.CaricatureModuleDirectionType.Left:
                        maxWide = (_thisRect.rect.width / 2) - item._ThisRect.rect.width / 2;
                        x = maxWide + item._SkewingValue.x;
                        if (Mathf.Abs(item._ThisRect.localPosition.x - x) > 1/* item._ThisRect.localPosition.x != x*/)
                        {
                            y = item._ThisRect.localPosition.y;
                        }
                        break;
                    case ChapterTool.CaricatureModuleDirectionType.Right:
                        maxWide = -(_thisRect.rect.width / 2) + item._ThisRect.rect.width / 2;
                        x = maxWide + item._SkewingValue.x;
                        if (Mathf.Abs(item._ThisRect.localPosition.x - x) > 1)
                        {
                            y = item._ThisRect.localPosition.y;
                        }
                        break;
                }
                tagePoint = new Vector3(x, y);
                tagePoints.Add(tagePoint);
            }
        }

        tagePoints.Reverse();
        CaricaturePlayerModule lastItem = items[items.Count - 1];
        Vector3 lastItemTagePoint = tagePoints[tagePoints.Count - 1];

        float minY = _thisRect.rect.height / 2;
        minY = -(minY) + lastItem._ThisRect.rect.height / 2;

        if (minY >= lastItemTagePoint.y)
        {
            _theFirstScreen = true;
        }
        return tagePoints;
    }
    /// <summary>
    /// 获取出生点或者移动目标点
    /// </summary>
    /// <param name="Item"></param>
    /// <param name="isMoveOrBorn"></param>
    /// <returns></returns>
    Vector3 GetBornTagePoint(CaricaturePlayerModule data, CaricaturePlayerModule OldItem = null)
    {
        float maxHeight = -(_thisRect.rect.height / 2);
        float maxWide = _thisRect.rect.width;
        float itemHeight = data._ThisRect.rect.height;
        float itemWide = data._ThisRect.rect.width;

        //if (data._CaricaturePlayerSpecialModuleBasics != null)
        //{
        //    Debug.Log("特殊模块");
        //}
        float tageY = 0;
        float tageX = 0;
        Vector3 tagePoint = Vector3.zero;
        switch (data._DirectionType)
        {
            case ChapterTool.CaricatureModuleDirectionType.Up:
                tageY = (maxHeight) - (itemHeight / 2);
                //Debug.LogError("y轴：" + tageY);
                tagePoint = new Vector3(tageX + data._SkewingValue.x, tageY);
                break;
            case ChapterTool.CaricatureModuleDirectionType.Left:
                tageY = (maxHeight) + (itemHeight / 2) + data._SkewingValue.y;
                tageX = (maxWide / 2) + (itemWide / 2);
                tagePoint = new Vector3(tageX, tageY);
                break;
            case ChapterTool.CaricatureModuleDirectionType.Right:
                tageY = (maxHeight) + (itemHeight / 2) + data._SkewingValue.y;
                tageX = (maxWide / 2) + (itemWide / 2);
                tagePoint = new Vector3(-tageX, tageY);
                break;
        }
        //tagePoint = new Vector3(tagePoint.x + data._SkewingValue.x, tagePoint.y + data._SkewingValue.y, tagePoint.z + data._SkewingValue.z);
        if (OldItem != null)
        {
            float mindY = -(_thisRect.rect.height / 2);

            float oldValue = OldItem._ThisRect.localPosition.y - OldItem._ThisRect.rect.height / 2;

            if (oldValue < mindY)
            {
                float exceedValue = mindY - oldValue;
                tagePoint = new Vector3(tagePoint.x, tagePoint.y - data._Interval - exceedValue, tagePoint.z);
            }
            else
            {
                tagePoint = new Vector3(tagePoint.x, tagePoint.y - data._Interval, tagePoint.z);
            }

        }
        return tagePoint;
    }
    /// <summary>
    /// 点击背景
    /// </summary>
    public void ClickBtn()
    {
        OpenClickBtn(false);
        if (_currShowItem == null || !_currShowItem._IsShowIn)
        {
            //Debug.Log("播放下一张图片");
            Show(_currShowItem);
        }
        else
        {
            _currShowItem.ClickBtn();
            //Debug.Log("播放图片对话");
        }
    }
    /// <summary>
    /// 开关背景按钮
    /// </summary>
    /// <param name="isOpen"></param>
    public void OpenClickBtn(bool isOpen)
    {
        //Debug.Log("开关按钮点击" + isOpen);
        _bgBtn.enabled = isOpen;
    }
    /// <summary>
    /// 关闭
    /// </summary>
    public async UniTask Close()
    {
        if (_isFadeOut)
        {
            await ChapterTool.FadeInFadeOut(_canvasGroup, 0, 0.1f, null, () =>
            {
                _endAction?.Invoke();
            });
        }
        else
        {
            _endAction?.Invoke();
        }

    }
}
