using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public struct AdjacentTileID
{
    public int upTileid;
    public int rightTileid;
    public int downTileid;
    public int leftTileid;
}

public class JigsawTile : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerDownHandler
{
    public int id;
    public GameObject maskGO;//一开始生成的maskgo
    private Image _mask;//拼图遮罩块
    private Image _jigsaw;//完整的大图

    public AdjacentTileID adjacentTile;//周围的图块id
    public AdjacentTileID tileTree;//已连接的图块id(有方向判断)
    public List<JigsawTile> tilesTree = new List<JigsawTile>();//已经贴合的图块

    [SerializeField] private string modeName;//所使用遮罩图的模式，上右下左依次排列，f平、v凹、a凸

    Canvas _canvas;
    public RectTransform _thisRect;
    RectTransform _bgRect;
    GameObject min_x;
    GameObject max_x;
    GameObject min_y;
    GameObject max_y;
    JigsawGenerator jg;

    //4点
    public RectTransform upPoint;
    public RectTransform rightPoint;
    public RectTransform downPoint;
    public RectTransform leftPoint;

    public Action DragEvent;
    public Action AllMove;

    public bool visited;//是否访问过（遍历用）
    public bool compound;//是否已经贴合
    float _adsorptionDistance = 225f;//吸附距离
    public JigsawTileParent parentTile;//吸附后父物体对象

    void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
        _thisRect = GetComponent<RectTransform>();
        _bgRect = this.transform.parent.parent.Find("bg").transform as RectTransform;
        min_x = _bgRect.transform.Find("left").gameObject;
        max_x = _bgRect.transform.Find("right").gameObject;
        min_y = _bgRect.transform.Find("down").gameObject;
        max_y = _bgRect.transform.Find("up").gameObject;
        jg = GetComponentInParent<JigsawGenerator>();
    }

    void SetSize(float tileSize, string spriteName)
    {
        _mask = transform.Find("Mask").transform.GetComponent<Image>();
        _jigsaw = transform.Find("Mask/Image").transform.GetComponent<Image>();
        //设置mask大小
        (transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tileSize);
        (transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tileSize);
        float maskSize = 150 * tileSize / 100;
        (_mask.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maskSize);
        (_mask.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maskSize);
        //设置大图的大小
        float jigmapSize = (this.transform.parent as RectTransform).rect.width;
        (this._jigsaw.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, jigmapSize);
        (this._jigsaw.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, jigmapSize);
        //设置图片
        this._jigsaw.sprite = ABManager.GetAsset<Sprite>(spriteName);
    }

    //设置使用哪张mask图，用fva代表，上右下左4个字符
    public void SetMask(string maskmode, float tileSize, string spriteName)
    {
        modeName = maskmode;
        //创建mask
        var prefab = ABManager.GetAsset<GameObject>($"Mask-{modeName}");
        GameObject go = Instantiate(prefab);
        go.name = "Mask";
        go.transform.SetParent(this.transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        SetSize(tileSize, spriteName);//初始化图块的大小
    }

    public void SetTileParent()
    {
        this._jigsaw.transform.SetParent(this.transform.parent);
        this._jigsaw.transform.localPosition = Vector3.zero;
        this._jigsaw.transform.SetParent(this._mask.transform);
        this._jigsaw.transform.localEulerAngles = new Vector3(0, 0, -this._mask.transform.localEulerAngles.z);
    }

    public string GetMaskName()
    {
        return modeName;
    }

    public Vector2 GetPosition()
    {
        return this._thisRect.anchoredPosition;
    }

    public void SetPosition(Vector2 pos)
    {
        RectTransform re = transform as RectTransform;
        re.anchoredPosition = pos;
        //this._thisRect.anchoredPosition = pos;  ？？
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (compound)//已经组合的移动用父物体移动
        {
            MoveParent(eventData);
            return;
        }
        MoveSelf(eventData);
        Adsorb();
        AllMove?.Invoke();
    }

    //移动父物体
    void MoveParent(PointerEventData eventData)
    {
        Vector2 newPos = parentTile._thisRect.anchoredPosition + eventData.delta / _canvas.scaleFactor;
        Vector2 bgPos = jg.transform.InverseTransformPoint(_bgRect.transform.position);
        Vector2 leftPos = jg.transform.InverseTransformPoint(min_x.transform.position);
        Vector2 rightPos = jg.transform.InverseTransformPoint(max_x.transform.position);
        Vector2 downPos = jg.transform.InverseTransformPoint(min_y.transform.position);
        Vector2 upPos = jg.transform.InverseTransformPoint(max_y.transform.position);

        Vector2 offset = jg.transform.localPosition - new Vector3(-(jg.transform as RectTransform).sizeDelta.x / 2, -(jg.transform as RectTransform).sizeDelta.y / 2);//中心点-左下角点
        leftPos += offset;
        rightPos += offset;
        downPos += offset;
        upPos += offset;
        float x = Mathf.Clamp(newPos.x, leftPos.x + _thisRect.sizeDelta.x / 2, rightPos.x - _thisRect.sizeDelta.x / 2);
        float y = Mathf.Clamp(newPos.y, downPos.y + _thisRect.sizeDelta.y / 2, upPos.y - _thisRect.sizeDelta.y / 2);
        //把偏移量传给父物体
        parentTile._thisRect.anchoredPosition = new Vector2(x, y);

        parentTile.DetectionAdsorb();
    }
    //移动自身
    void MoveSelf(PointerEventData eventData)
    {
        Vector2 newPos = _thisRect.anchoredPosition + eventData.delta / _canvas.scaleFactor;
        Vector2 bgPos = jg.transform.InverseTransformPoint(_bgRect.transform.position);

        Vector2 leftPos = jg.transform.InverseTransformPoint(min_x.transform.position);
        Vector2 rightPos = jg.transform.InverseTransformPoint(max_x.transform.position);
        Vector2 downPos = jg.transform.InverseTransformPoint(min_y.transform.position);
        Vector2 upPos = jg.transform.InverseTransformPoint(max_y.transform.position);

        Vector2 offset = jg.transform.localPosition - new Vector3(-(jg.transform as RectTransform).sizeDelta.x / 2, -(jg.transform as RectTransform).sizeDelta.y / 2);//中心点-左下角点
        leftPos += offset;
        rightPos += offset;
        downPos += offset;
        upPos += offset;

        float x = Mathf.Clamp(newPos.x, leftPos.x + _thisRect.sizeDelta.x / 2, rightPos.x - _thisRect.sizeDelta.x / 2);
        float y = Mathf.Clamp(newPos.y, downPos.y + _thisRect.sizeDelta.y / 2, upPos.y - _thisRect.sizeDelta.y / 2);
        _thisRect.anchoredPosition = new Vector2(x, y);
    }

    [Obsolete("备用的")]
    void Set(JigsawTile tile1, JigsawTile tile2)
    {
        //合成准备
        if (tile2.compound)
        {
            tile1.compound = true;
            tile1.transform.SetParent(tile2.parentTile.transform);
            tile1.parentTile = tile2.parentTile;
            if (!tile1.parentTile.curentJigsawTile.Contains(tile1))
                parentTile.curentJigsawTile.Add(tile1);
            tile1.SetPosition(tile2.GetPosition() + jg.GetTilePos(tile2.id, tile1.id));
            return;
        }

        //赋值相对位置
        tile1.compound = true;
        tile2.compound = true;
        GameObject tileParent = Instantiate(jg.tileParent);
        tile1.parentTile = tileParent.GetComponent<JigsawTileParent>();
        tile2.parentTile = tile1.parentTile;
        if (!tile1.parentTile.curentJigsawTile.Contains(tile1))
            tile1.parentTile.curentJigsawTile.Add(tile1);
        if (!tile1.parentTile.curentJigsawTile.Contains(tile2))
            tile1.parentTile.curentJigsawTile.Add(tile2);
        //Set
        tileParent.name = "tileParent";
        tileParent.transform.SetParent(jg.transform);
        tileParent.transform.localScale = Vector3.one;
        (tileParent.transform as RectTransform).anchoredPosition = tile1._thisRect.anchoredPosition;
        (tileParent.transform as RectTransform).sizeDelta = tile1._thisRect.sizeDelta;
        //setParent
        tile1.transform.SetParent(tileParent.transform);
        tile2.transform.SetParent(tileParent.transform);
        tile2.SetPosition(tile1.GetPosition() + jg.GetTilePos(tile1.id, tile2.id));
    }

    //吸附
    void Adsorb()
    {
        foreach (var item in jg.AllJigsawTiles)
        {
            if (item.id == adjacentTile.upTileid)
            {
                Vector3 targetPos = jg.transform.InverseTransformPoint(item.downPoint.transform.position);
                Vector3 localPos = jg.transform.InverseTransformPoint(this.upPoint.transform.position);
                if ((localPos - targetPos).sqrMagnitude < _adsorptionDistance)
                {
                    JigsawTile tempTile = item;
                    //清除两个图块的互交
                    this.adjacentTile.upTileid = 0;
                    tempTile.adjacentTile.downTileid = 0;
                    //清除点
                    this.upPoint = null;
                    tempTile.downPoint = null;
                    //连接两图块id
                    this.tileTree.upTileid = tempTile.id;
                    tempTile.tileTree.downTileid = this.id;
                    //为连接的图块列表赋值
                    this.tilesTree.Add(GetJigsawTile(this.tileTree.upTileid));
                    tempTile.tilesTree.Add(GetJigsawTile(tempTile.tileTree.downTileid));
                    //合成准备
                    jg.Joint(this, tempTile);
                }
            }
            else if (item.id == adjacentTile.rightTileid)
            {
                Vector3 targetPos = jg.transform.InverseTransformPoint(item.leftPoint.transform.position);
                Vector3 localPos = jg.transform.InverseTransformPoint(this.rightPoint.transform.position);
                if ((localPos - targetPos).sqrMagnitude < _adsorptionDistance)
                {
                    JigsawTile tempTile = item;
                    //清除两个图块的互交
                    this.adjacentTile.rightTileid = 0;//使之只执行一次
                    tempTile.adjacentTile.leftTileid = 0;
                    //清除点
                    this.rightPoint = null;
                    tempTile.leftPoint = null;
                    //连接两图块id
                    this.tileTree.rightTileid = tempTile.id;
                    tempTile.tileTree.leftTileid = this.id;
                    //为连接的图块列表赋值
                    this.tilesTree.Add(GetJigsawTile(this.tileTree.rightTileid));
                    tempTile.tilesTree.Add(GetJigsawTile(tempTile.tileTree.leftTileid));
                    //合成准备
                    jg.Joint(this, tempTile);
                }
            }
            else if (item.id == adjacentTile.downTileid)
            {
                Vector3 targetPos = jg.transform.InverseTransformPoint(item.upPoint.transform.position);
                Vector3 localPos = jg.transform.InverseTransformPoint(this.downPoint.transform.position);
                if ((localPos - targetPos).sqrMagnitude < _adsorptionDistance)
                {
                    JigsawTile tempTile = item;
                    //清除两个图块的互交
                    this.adjacentTile.downTileid = 0;//使之只执行一次
                    tempTile.adjacentTile.upTileid = 0;
                    //清除点
                    this.downPoint = null;
                    tempTile.upPoint = null;
                    //连接两图块id
                    this.tileTree.downTileid = tempTile.id;
                    tempTile.tileTree.upTileid = this.id;
                    //为连接的图块列表赋值
                    this.tilesTree.Add(GetJigsawTile(this.tileTree.downTileid));
                    tempTile.tilesTree.Add(GetJigsawTile(tempTile.tileTree.upTileid));
                    //合成准备
                    jg.Joint(this, tempTile);
                }
            }
            else if (item.id == adjacentTile.leftTileid)
            {
                Vector3 targetPos = jg.transform.InverseTransformPoint(item.rightPoint.transform.position);
                Vector3 localPos = jg.transform.InverseTransformPoint(this.leftPoint.transform.position);
                if ((localPos - targetPos).sqrMagnitude < _adsorptionDistance)
                {
                    JigsawTile tempTile = item;
                    //清除两个图块的互交
                    this.adjacentTile.leftTileid = 0;//使之只执行一次
                    tempTile.adjacentTile.rightTileid = 0;
                    //清除点
                    this.leftPoint = null;
                    tempTile.rightPoint = null;
                    //连接两图块id
                    this.tileTree.leftTileid = tempTile.id;
                    tempTile.tileTree.rightTileid = this.id;
                    //为连接的图块列表赋值
                    this.tilesTree.Add(GetJigsawTile(this.tileTree.leftTileid));
                    tempTile.tilesTree.Add(GetJigsawTile(tempTile.tileTree.rightTileid));
                    //合成准备
                    jg.Joint(this, tempTile);
                }
            }
        }
    }



    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        _thisRect.SetAsLastSibling();
    }

    public void SetSortBastLayer()
    {
        _thisRect.SetAsLastSibling();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        DragEvent?.Invoke();
    }

    //根据id获取对应图块
    public JigsawTile GetJigsawTile(int tileid)
    {
        if (tileid <= 0 || tileid > 16) return null;
        foreach (var item in jg.AllJigsawTiles)
        {
            if (item.id == tileid)
                return item;
        }
        return null;
    }
}
