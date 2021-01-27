using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System;

/// <summary>
/// Scroll View 滑动居中,居中放大
/// </summary>
public class UICenterOnChildController : MonoBehaviour, IEndDragHandler, IDragHandler, IBeginDragHandler
{

    public enum ScrollDir
    {
        Horizontal, //横向
        Vertical    //竖向
    }

    #region 变量

    public ScrollDir Dir = ScrollDir.Horizontal;

    [Header("对齐中点速度/时间")]
    public float ToCenterTime = 0.5f;
    [Header("中心点放大倍数")]
    public float CenterScale = 1f;
    [Header("非中心点放大倍数")]
    public float UnCenterScale = 0.6f;


    private ScrollRect _scrollView;
    private Transform _content;
    private RectTransform _content_recttsf;
    private List<float> _childrenPos = new List<float>();
	private float _targetPos;

	/// <summary>
	/// 当前中心child索引
	/// </summary>
	private int _curCenterChildIndex = -1;

    /// <summary>
    /// 当前中心ChildItem
    /// </summary>
    public GameObject CurCenterChildItem
    {
        get
        {
            GameObject centerChild = null;
            if (_content != null && _curCenterChildIndex >= 0 && _curCenterChildIndex < _content.childCount)
            {
                centerChild = _content.GetChild(_curCenterChildIndex).gameObject;
            }
            return centerChild;
        }

    }
    #endregion

    #region 方法

    #region Scroll View 滑动居中,居中放大
    /// <summary>
	/// 根据拖动来改变每一个子物体的缩放
	/// </summary>
	public void SetCellScale()
    {
        GameObject centerChild = null;
        for (int i = 0; i < _content.childCount; i++)
        {
            centerChild = _content.GetChild(i).gameObject;
            if (i == _curCenterChildIndex)
                centerChild.transform.localScale = CenterScale * Vector3.one;
            else
                centerChild.transform.localScale = UnCenterScale * Vector3.one;
        }
		//通知当前选择的Center下标
		CenteredIndex?.Invoke(_curCenterChildIndex);
	}
	private void Init()
	{
		_scrollView = GetComponent<ScrollRect>();
		if (_scrollView == null)
		{
			Debug.LogError("ScrollRect is null");
			return;
		}
		_content = _scrollView.content;

		LayoutGroup layoutGroup = null;
		layoutGroup = _content.GetComponent<LayoutGroup>();
		_content_recttsf = _content.GetComponent<RectTransform>();
		if (layoutGroup == null)
		{
			Debug.LogError("LayoutGroup component is null");
		}
		_scrollView.movementType = ScrollRect.MovementType.Unrestricted;
		float spacing = 0f;
		//根据dir计算坐标，Horizontal：存x，Vertical：存y
		switch (Dir)
		{
			case ScrollDir.Horizontal:
				if (layoutGroup is HorizontalLayoutGroup)
				{
					float childPosX = _scrollView.GetComponent<RectTransform>().rect.width * 0.5f - GetChildItemWidth(0) * 0.5f;
					spacing = (layoutGroup as HorizontalLayoutGroup).spacing;//间距
					_childrenPos.Add(childPosX);
					for (int i = 1; i < _content.childCount; i++)
					{
						childPosX -= GetChildItemWidth(i) * 0.5f + GetChildItemWidth(i - 1) * 0.5f + spacing;
						_childrenPos.Add(childPosX);
					}
				}
				else if (layoutGroup is GridLayoutGroup)
				{
					GridLayoutGroup grid = layoutGroup as GridLayoutGroup;
					float childPosX = _scrollView.GetComponent<RectTransform>().rect.width * 0.5f - grid.cellSize.x * 0.5f;
					_childrenPos.Add(childPosX);
					for (int i = 0; i < _content.childCount - 1; i++)
					{
						childPosX -= grid.cellSize.x + grid.spacing.x;
						_childrenPos.Add(childPosX);
					}
				}
				else
				{
					Debug.LogError("Horizontal ScrollView is using VerticalLayoutGroup");
				}
				break;
			case ScrollDir.Vertical:
				if (layoutGroup is VerticalLayoutGroup)
				{
					float childPosY = -_scrollView.GetComponent<RectTransform>().rect.height * 0.5f + GetChildItemHeight(0) * 0.5f;
					spacing = (layoutGroup as VerticalLayoutGroup).spacing;
					_childrenPos.Add(childPosY);
					for (int i = 1; i < _content.childCount; i++)
					{
						childPosY += GetChildItemHeight(i) * 0.5f + GetChildItemHeight(i - 1) * 0.5f + spacing;
						_childrenPos.Add(childPosY);
					}
				}
				else if (layoutGroup is GridLayoutGroup)
				{
					GridLayoutGroup grid = layoutGroup as GridLayoutGroup;
					float childPosY = -_scrollView.GetComponent<RectTransform>().rect.height * 0.5f + grid.cellSize.y * 0.5f;
					_childrenPos.Add(childPosY);
					for (int i = 1; i < _content.childCount; i++)
					{
						childPosY += grid.cellSize.y + grid.spacing.y;
						_childrenPos.Add(childPosY);
					}
				}
				else
				{
					Debug.LogError("Vertical ScrollView is using HorizontalLayoutGroup");
				}
				break;
		}
	}

	/// <summary>
	/// 获取子item宽度
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	private float GetChildItemWidth(int index)
	{
		return (_content.GetChild(index) as RectTransform).sizeDelta.x;
	}

	/// <summary>
	/// 获取子item高度
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	private float GetChildItemHeight(int index)
	{
		return (_content.GetChild(index) as RectTransform).sizeDelta.y;
	}

	#region 接口

	public void OnBeginDrag(PointerEventData eventData)
	{
		//停止所有动画
		_content.DOKill();
		_curCenterChildIndex = -1;
	}

	public void OnDrag(PointerEventData eventData)
	{
		//这里会一直调用 实时更新中心点的下标 并作出缩放改变 所以如果需求是拖动结束的时候 这里不调用FindClosestChildPos
		switch (Dir)
		{
			case ScrollDir.Horizontal:
				FindClosestChildPos(_content.localPosition.x, out _curCenterChildIndex, out _targetPos);
				break;
			case ScrollDir.Vertical:
				FindClosestChildPos(_content.localPosition.y, out _curCenterChildIndex, out _targetPos);
				break;
		}
		SetCellScale();
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		_scrollView.StopMovement();
		switch (Dir)
		{
			case ScrollDir.Horizontal:
				FindClosestChildPos(_content.localPosition.x, out _curCenterChildIndex, out _targetPos);
				_content.DOLocalMoveX(_targetPos, ToCenterTime);
				break;
			case ScrollDir.Vertical:
				FindClosestChildPos(_content.localPosition.y, out _curCenterChildIndex, out _targetPos);
				_content.DOLocalMoveY(_targetPos, ToCenterTime);
				break;
		}
		SetCellScale();
	}

	#endregion

	/// <summary>
	/// 查询最近的子类的位置
	/// </summary>
	/// <param name="currentPos"> 当前的位置 </param>
	/// <param name="curCenterChildIndex"></param>
	/// <returns></returns>
	private void FindClosestChildPos(float currentPos, out int curCenterChildIndex, out float curCenterChildPos)
	{
		curCenterChildPos = 0;
		//当前居中的子类下标
		curCenterChildIndex = -1;

		//正无穷大的表示形式
		float distance = Mathf.Infinity;

		for (int i = 0; i < _childrenPos.Count; i++)
		{
			float p = _childrenPos[i];
			float d = Mathf.Abs(p - currentPos);
			if (d < distance)
			{
				distance = d;
				curCenterChildPos = _childrenPos[i];
				curCenterChildIndex = i;
			}
			else
				break;
		}
	}


	#endregion

	/// <summary>
	/// 居中的item的下标
	/// </summary>
	private Action<int> CenteredIndex;

	private void Awake()
    {
		Init();
	}

    // Start is called before the first frame update
    void Start()
    {
		

	}

    // Update is called once per frame
    void Update()
    {

    }

	public void InitValue(Action<int> centeredIndex) 
	{
		CenteredIndex = centeredIndex;
		FindClosestChildPos(_content.localPosition.x, out _curCenterChildIndex, out _targetPos);
		SetCellScale();
		
		_curCenterChildIndex = -1;
		
	}

    #endregion


}
