using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 循环列表 横向
/// </summary>
public class CycleSlidingList : MonoBehaviour, IEndDragHandler, IDragHandler, IBeginDragHandler
{
    private class ShowItemInfo
    {
        public int DefIndex;
        public GameObject ShowItem;
        public GameObject ClickBut;
        public GameObject NameBG;
    }

    #region 变量

    [Header("对齐中点速度/时间")]
    public float ToCenterTime = 0.5f;
    [Header("中心点放大倍数")]
    public float CenterScale = 1f;
    [Header("非中心点放缩放值")]
    public float UnCenterScale = 0.8f;


    [SerializeField]
    private GameObject[] _DefList;

    private List<ShowItemInfo> _showItems = new List<ShowItemInfo>();

    private List<ShowItemInfo> _hideItems = new List<ShowItemInfo>();
    /// <summary>
    /// 中心点
    /// </summary>
    private float _centerPoint = 0f;
    //间距
    private float _itemSpacing = 0f;
    //item宽
    private float _itemWidth = 0f;

    private float _showMinX = 0f;
    private float _showMaxX = 0f;


    #endregion


    #region 方法

    private Action OnClickThrowCallback;
    private Action<int> SelectedThrowCallback;

    private void Awake()
    {
        //Init();
    }

    private void Update()
    {

    }

    private void LateUpdate()
    {
        //if (isDrag)
            
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="curShowItem"></param>
    private void ShowItemInfoAddListener(ref ShowItemInfo curShowItem) 
    {
        curShowItem.ClickBut = curShowItem.ShowItem.transform.Find("ButBG").gameObject;
        curShowItem.ClickBut.GetComponent<Button>().onClick.RemoveAllListeners();
        curShowItem.ClickBut.GetComponent<Button>().onClick.AddListener(OnClickThrow);
    }

    private void Init()
    {
        _showMaxX = (transform as RectTransform).sizeDelta.x / 2f;
        _showMinX = _showMaxX * -1;
        _itemWidth = (_DefList[0].transform as RectTransform).sizeDelta.x;
        _itemSpacing = Mathf.Abs(_DefList[0].transform.localPosition.x - _DefList[1].transform.localPosition.x) - _itemWidth;

        //中心点向外扩散显示item
        float showItemMinX = 0;
        float showItemMaxX = 0;
        int index = _DefList.Length / 2;
        int addIndex = 1;
        //中心点
        _DefList[index].transform.localPosition = Vector3.zero;
        _DefList[index].gameObject.SetActive(true);

        ShowItemInfo curShowItem = new ShowItemInfo();
        curShowItem.DefIndex = index;
        curShowItem.ShowItem = _DefList[index];
        ShowItemInfoAddListener(ref curShowItem);
        curShowItem.NameBG = curShowItem.ShowItem.transform.Find("NameBG").gameObject;
        _showItems.Add(curShowItem);

        showItemMaxX = _itemWidth / 2f;
        showItemMinX = showItemMaxX * -1;
        while (showItemMinX > _showMinX || showItemMaxX < _showMaxX)
        {
            GameObject objLeft = null;
            List<ShowItemInfo> showItems = new List<ShowItemInfo>();
            //左
            index = _DefList.Length / 2 - addIndex;
            if (index >= 0)
            {
                objLeft = _DefList[index];

            }
            else
            {
                index = index % _DefList.Length;
                index = _DefList.Length + index;
                objLeft = Instantiate(_DefList[index], transform);
            }

            Vector3 targetPos = Vector3.zero;
            targetPos.x = (_itemWidth + _itemSpacing) * addIndex * -1;
            objLeft.transform.localPosition = targetPos;
            objLeft.transform.localScale = Vector2.one * UnCenterScale;
            objLeft.SetActive(true);

            ShowItemInfo curShowItemLeft = new ShowItemInfo();
            curShowItemLeft.DefIndex = index;
            curShowItemLeft.ShowItem = _DefList[index];
            ShowItemInfoAddListener(ref curShowItemLeft);
            curShowItemLeft.NameBG = curShowItemLeft.ShowItem.transform.Find("NameBG").gameObject;
            _showItems.Add(curShowItemLeft);


            showItemMinX = objLeft.transform.localPosition.x - _itemWidth / 2f; ;
            //右
            GameObject objRight = null;
            index = _DefList.Length / 2 + addIndex;
            if (index < _DefList.Length)
            {
                objRight = _DefList[index];

            }
            else
            {
                index = index % _DefList.Length;
                objRight = Instantiate(_DefList[index], transform);
            }

            targetPos.x = (_itemWidth + _itemSpacing) * addIndex;
            objRight.transform.localPosition = targetPos;
            showItemMaxX = objRight.transform.localPosition.x + _itemWidth / 2f;
            objRight.transform.localScale = Vector2.one * UnCenterScale;
            objRight.SetActive(true);

            ShowItemInfo curShowItemRight = new ShowItemInfo();
            curShowItemRight.DefIndex = index;
            curShowItemRight.ShowItem = _DefList[index];
            ShowItemInfoAddListener(ref curShowItemRight);
            curShowItemRight.NameBG = curShowItemRight.ShowItem.transform.Find("NameBG").gameObject;
            _showItems.Add(curShowItemRight);
            addIndex += 1;

        }

        int curCenterChildIndex = -1;
        float curCenterChildPos = 0;
        FindClosestChildPos(out curCenterChildIndex, out curCenterChildPos);
        SetCellScale(curCenterChildIndex);
    }

    /// <summary>
    /// 是否为列表默认item
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    private bool DefListContains(GameObject obj) 
    {
        for (int i = 0; i < _DefList.Length; i++)
        {
            if (_DefList[i].Equals(obj))
                return true;
        }
        return false;
    }

    public void RestItemPos() 
    {

        List<GameObject> waitDestroy = new List<GameObject>();

        _showItems.Clear();
        _hideItems.Clear();
        for (int i = transform.childCount - 1; i >= 0; i--)
        {

            if (!DefListContains(transform.GetChild(i).gameObject)) 
            {
                transform.GetChild(i).gameObject.SetActive(false);
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        Init();
    }

    public void InitValue(Action onClickThrowCallback, Action<int> selectedThrowCallback) 
    {
        //
        OnClickThrowCallback = onClickThrowCallback;
        SelectedThrowCallback = selectedThrowCallback;

        RestItemPos();
    }

    /// <summary>
    /// 设置物体的位置
    /// </summary>
    /// <param name="index"> </param>
    private void UpdatePosition(float moveX)
    {
        Vector3 move = Vector3.zero;
        move.x = moveX;
        for (int i = 0; i < _showItems.Count; i++)
        {
            _showItems[i].ShowItem.transform.localPosition += move;
        }
    }

    /// <summary>
    /// 是否需要添加item
    /// </summary>
    /// <returns></returns>
    private bool IsNeedAddItem(float targetX, bool isLeft = true) 
    {
        for (int i = 0; i < _showItems.Count; i++)
        {
            if (isLeft)
            {
                if (targetX > _showItems[i].ShowItem.transform.localPosition.x)
                    return false;
            }
            else 
            {
                if (targetX < _showItems[i].ShowItem.transform.localPosition.x)
                    return false;
            }
            
        }

        return true;
    }

    /// <summary>
    /// 循环检测
    /// </summary>
    private void LoopDetection() 
    {
        for (int i = 0; i < _showItems.Count; i++)
        {
            float leftX = 0;
            float rightX = 0;

            if (Mathf.Abs(_showItems[i].ShowItem.transform.localPosition.x - _showMinX) < _itemWidth)
            {
                leftX = _showItems[i].ShowItem.transform.localPosition.x - _itemWidth / 2f;
                rightX = _showItems[i].ShowItem.transform.localPosition.x + _itemWidth / 2f + _itemSpacing;
                if (leftX > _showMinX)
                {
                    //需要在左边添加一个item
                    //左边是否已经添加
                    if (IsNeedAddItem(leftX, true)) 
                    {
                        AddItem(_showItems[i], true);
                    }
                    Debug.Log("需要在左边添加一个item11");
                    return;
                }
                else if (rightX < _showMinX)
                {
                    //隐藏最左边的item
                    _showItems[i].ShowItem.SetActive(false);
                    _hideItems.Add(_showItems[i]);
                    _showItems.RemoveAt(i);
                    continue;
                }
            }else if (Mathf.Abs(_showItems[i].ShowItem.transform.localPosition.x - _showMaxX) < _itemWidth) 
            {
                leftX = _showItems[i].ShowItem.transform.localPosition.x - _itemWidth / 2f + _itemSpacing;
                rightX = _showItems[i].ShowItem.transform.localPosition.x + _itemWidth / 2f;
                if (leftX > _showMaxX)
                {
                    //隐藏最右边的item
                    _showItems[i].ShowItem.SetActive(false);
                    _hideItems.Add(_showItems[i]);
                    _showItems.RemoveAt(i);
                    continue;
                }
                else if (rightX < _showMaxX)
                {
                    //需要在右边添加一个item
                    //右边是否已经添加
                    if (IsNeedAddItem(rightX, false))
                    {
                        AddItem(_showItems[i], false);
                    }
                    Debug.Log("需要在右边添加一个item22");
                    return;
                }
            }
        }
    }

    /// <summary>
    /// 添加item
    /// </summary>
    /// <param name="item"></param>
    /// <param name="isLeft"></param>
    private void AddItem(ShowItemInfo item, bool isLeft) 
    {
        int targetIndex = item.DefIndex;
        if (isLeft)
        {
            targetIndex -= 1;
            if (targetIndex < 0)
            {
                targetIndex = targetIndex % _DefList.Length;
                targetIndex = _DefList.Length + targetIndex;
            }
        }
        else 
        {
            targetIndex += 1;
            if (targetIndex >= _DefList.Length)
            {
                targetIndex = targetIndex % _DefList.Length;
            }
        }


        ShowItemInfo showItemInfo = new ShowItemInfo();
        for (int i = 0; i < _hideItems.Count; i++)
        {
            if (_hideItems[i].DefIndex == targetIndex) 
            {
                showItemInfo = _hideItems[i];
                _hideItems.RemoveAt(i);
                break;
            }
        }
        //重新生成一个
        if (showItemInfo.ShowItem == null) 
        {
            showItemInfo.DefIndex = targetIndex;
            showItemInfo.ShowItem = Instantiate(_DefList[targetIndex], transform);
            ShowItemInfoAddListener(ref showItemInfo);
            showItemInfo.NameBG = showItemInfo.ShowItem.transform.Find("NameBG").gameObject;
        }

        Vector3 targetPos = item.ShowItem.transform.localPosition;
        if (isLeft)
        {
            targetPos.x -= (_itemWidth + _itemSpacing);
        }
        else 
        {
            targetPos.x += (_itemWidth + _itemSpacing);
            
        }
        _showItems.Add(showItemInfo);
        showItemInfo.ShowItem.transform.localPosition = targetPos;
        showItemInfo.ShowItem.SetActive(true);
    }

    /// <summary>
	/// 根据拖动来改变每一个子物体的缩放
	/// </summary>
	public void SetCellScale(int curCenterChildIndex)
    {
        for (int i = 0; i < _showItems.Count; i++)
        {
            if (i == curCenterChildIndex)
            {
                _showItems[i].ShowItem.transform.localScale = CenterScale * Vector3.one;
                _showItems[i].ClickBut.SetActive(true);
                _showItems[i].NameBG.SetActive(true);
            }
            else 
            {
                _showItems[i].ShowItem.transform.localScale = UnCenterScale * Vector3.one;
                _showItems[i].ClickBut.SetActive(false);
                _showItems[i].NameBG.SetActive(false);
            }
                
        }

        SelectedThrowCallback?.Invoke(_showItems[curCenterChildIndex].DefIndex);
    }

    /// <summary>
    /// 查询最近的子类的位置
    /// </summary>
    /// <param name="currentPos"> 当前的位置 </param>
    /// <param name="curCenterChildIndex"></param>
    /// <returns></returns>
    private void FindClosestChildPos(out int curCenterChildIndex, out float curCenterChildPos)
    {
        curCenterChildPos = 0;
        //当前居中的子类下标
        curCenterChildIndex = -1;

        //正无穷大的表示形式
        float distance = Mathf.Infinity;

        for (int i = 0; i < _showItems.Count; i++)
        {
            float p = _showItems[i].ShowItem.transform.localPosition.x;
            float d = Mathf.Abs(p - _centerPoint);
            if (d < distance)
            {
                distance = d;
                curCenterChildPos = _showItems[i].ShowItem.transform.localPosition.x;
                curCenterChildIndex = i;
            }
            else
                break;
        }
    }

    private float _moveStartPositionX = 0;
    public void OnBeginDrag(PointerEventData eventData)
    {
        _moveStartPositionX = eventData.position.x;
        for (int i = 0; i < _showItems.Count; i++)
        {
            _showItems[i].ShowItem.transform.DOKill();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //这里会一直调用 实时更新中心点的下标 并作出缩放改变 所以如果需求是拖动结束的时候 这里不调用FindClosestChildPos
        float moveX = eventData.position.x - _moveStartPositionX;

        moveX = Mathf.Clamp(moveX, -60, 60);
        _moveStartPositionX = eventData.position.x;
        UpdatePosition(moveX);
        int curCenterChildIndex = -1;
        float curCenterChildPos = 0;
        FindClosestChildPos(out curCenterChildIndex, out curCenterChildPos);
        SetCellScale(curCenterChildIndex);
        LoopDetection();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        int curCenterChildIndex = -1;
        float curCenterChildPos = 0;
        FindClosestChildPos(out curCenterChildIndex, out curCenterChildPos);
        float targetX = 0;
        for (int i = 0; i < _showItems.Count; i++)
        {
            targetX = _showItems[i].ShowItem.transform.localPosition.x - curCenterChildPos;
            _showItems[i].ShowItem.transform.DOLocalMoveX(targetX, ToCenterTime);
        }
        SetCellScale(curCenterChildIndex);
    }


    /// <summary>
    /// 点击骰子
    /// </summary>
    private void OnClickThrow() 
    {
        OnClickThrowCallback?.Invoke();
    }

    #endregion
}

