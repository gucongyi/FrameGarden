using System;
using System.Collections.Generic;
using UnityEngine;

public class JigsawTileParent : MonoBehaviour
{
    // 组里拼合的图块
    public List<JigsawTile> curentJigsawTile = new List<JigsawTile>();

    // 需要判定和哪些可以连接的图块
    public List<JigsawTile> usableJigsawTile = new List<JigsawTile>();

    public RectTransform _thisRect;
    Canvas _canvas;
    JigsawGenerator jg;
    public Action IsReachAStandard;
    private void Start()
    {
        _thisRect = this.GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        jg = GetComponentInParent<JigsawGenerator>();
        IsReachAStandard = ReachAStandard; 
    }

    void FuzhiUsable()
    {
        usableJigsawTile.Clear();
        foreach (var item in curentJigsawTile)
        {
            JigsawTile tempTile = null;
            tempTile = item.GetJigsawTile(item.adjacentTile.upTileid);
            if (tempTile != null && !usableJigsawTile.Contains(tempTile))
                usableJigsawTile.Add(tempTile);
            tempTile = item.GetJigsawTile(item.adjacentTile.rightTileid);
            if (tempTile != null && !usableJigsawTile.Contains(tempTile))
                usableJigsawTile.Add(tempTile);
            tempTile = item.GetJigsawTile(item.adjacentTile.downTileid);
            if (tempTile != null && !usableJigsawTile.Contains(tempTile))
                usableJigsawTile.Add(tempTile);
            tempTile = item.GetJigsawTile(item.adjacentTile.leftTileid);
            if (tempTile != null && !usableJigsawTile.Contains(tempTile))
                usableJigsawTile.Add(tempTile);
        }
        foreach (var temp in curentJigsawTile)
        {
            if (usableJigsawTile.Contains(temp))
                usableJigsawTile.Remove(temp);
        }
    }

    /// <summary>
    /// 检测吸附
    /// </summary>
    public void DetectionAdsorb()
    {
        FuzhiUsable();
        for (int i = 0; i < usableJigsawTile.Count; i++)
        {
            for (int j = 0; j < curentJigsawTile.Count; j++)
            {
                int upid = curentJigsawTile[j].adjacentTile.upTileid;
                int rightid= curentJigsawTile[j].adjacentTile.rightTileid;
                int downid = curentJigsawTile[j].adjacentTile.downTileid;
                int leftid = curentJigsawTile[j].adjacentTile.leftTileid;

                if (upid != 0 && upid == usableJigsawTile[i].id)
                {
                    Vector3 targetPos = _thisRect.InverseTransformPoint(usableJigsawTile[i].downPoint.position);
                    Vector3 localPos = _thisRect.InverseTransformPoint(curentJigsawTile[j].upPoint.position);
                    if ((localPos - targetPos).sqrMagnitude <= 225f)
                    {
                        if (usableJigsawTile[i].parentTile != null && usableJigsawTile[i].parentTile != this)
                        {//如果时合成后的图块
                            usableJigsawTile[i].parentTile.transform.SetParent(this.transform);
                            JigsawTileParent destory = usableJigsawTile[i].parentTile;
                            foreach (var item in usableJigsawTile[i].parentTile.curentJigsawTile)
                            {
                                item.transform.SetParent(this.transform);
                                if (!this.curentJigsawTile.Contains(item))
                                    this.curentJigsawTile.Add(item);
                                item.parentTile = this;
                                //setPos
                                item._thisRect.anchoredPosition = curentJigsawTile[j]._thisRect.anchoredPosition + jg.GetTilePos(curentJigsawTile[j].id, item.id);
                            }
                            Destroy(destory.gameObject);
                        }
                        else
                        {
                            usableJigsawTile[i].transform.SetParent(this.transform);
                            usableJigsawTile[i].parentTile = this;
                            usableJigsawTile[i].compound = true;
                            if (!this.curentJigsawTile.Contains(usableJigsawTile[i]))
                                this.curentJigsawTile.Add(usableJigsawTile[i]);
                            usableJigsawTile[i]._thisRect.anchoredPosition = new Vector2(curentJigsawTile[j]._thisRect.anchoredPosition.x, curentJigsawTile[j]._thisRect.anchoredPosition.y + curentJigsawTile[j]._thisRect.sizeDelta.y);
                        }
                        //set
                        curentJigsawTile[j].upPoint = null;
                        usableJigsawTile[i].downPoint = null;
                        curentJigsawTile[j].adjacentTile.upTileid = 0;
                        usableJigsawTile[i].adjacentTile.downTileid = 0;
                        curentJigsawTile[j].tileTree.upTileid = usableJigsawTile[i].id;
                        usableJigsawTile[i].tileTree.downTileid = curentJigsawTile[j].id;
                        if (!usableJigsawTile[i].tilesTree.Contains(curentJigsawTile[j]))
                            usableJigsawTile[i].tilesTree.Add(curentJigsawTile[j]);
                        if (!usableJigsawTile[i].tilesTree.Contains(usableJigsawTile[i]))
                            curentJigsawTile[j].tilesTree.Add(usableJigsawTile[i]);
                    }
                }
                else if (rightid != 0 && rightid == usableJigsawTile[i].id)
                {
                    Vector3 targetPos = _thisRect.InverseTransformPoint(usableJigsawTile[i].leftPoint.position);
                    Vector3 localPos = _thisRect.InverseTransformPoint(curentJigsawTile[j].rightPoint.position);
                    if ((localPos - targetPos).sqrMagnitude <= 225f)
                    {
                        if (usableJigsawTile[i].parentTile != null && usableJigsawTile[i].parentTile != this)
                        {//如果时合成后的图块
                            usableJigsawTile[i].parentTile.transform.SetParent(this.transform);
                            JigsawTileParent destory = usableJigsawTile[i].parentTile;
                            foreach (var item in usableJigsawTile[i].parentTile.curentJigsawTile)
                            {
                                item.transform.SetParent(this.transform);
                                if (!this.curentJigsawTile.Contains(item))
                                    this.curentJigsawTile.Add(item);
                                item.parentTile = this;
                                //setPos
                                item._thisRect.anchoredPosition = curentJigsawTile[j]._thisRect.anchoredPosition + jg.GetTilePos(curentJigsawTile[j].id, item.id);
                            }
                            Destroy(destory.gameObject);
                        }
                        else
                        {
                            usableJigsawTile[i].transform.SetParent(this.transform);
                            usableJigsawTile[i].parentTile = this;
                            usableJigsawTile[i].compound = true;
                            if (!this.curentJigsawTile.Contains(usableJigsawTile[i]))
                                this.curentJigsawTile.Add(usableJigsawTile[i]);
                            usableJigsawTile[i]._thisRect.anchoredPosition = new Vector2(curentJigsawTile[j]._thisRect.anchoredPosition.x + curentJigsawTile[j]._thisRect.sizeDelta.x, curentJigsawTile[j]._thisRect.anchoredPosition.y);
                        }
                        //set
                        curentJigsawTile[j].rightPoint = null;
                        usableJigsawTile[i].leftPoint = null;
                        curentJigsawTile[j].adjacentTile.rightTileid = 0;
                        usableJigsawTile[i].adjacentTile.leftTileid = 0;
                        curentJigsawTile[j].tileTree.rightTileid = usableJigsawTile[i].id;
                        usableJigsawTile[i].tileTree.leftTileid = curentJigsawTile[j].id;
                        if (!usableJigsawTile[i].tilesTree.Contains(curentJigsawTile[j]))
                            usableJigsawTile[i].tilesTree.Add(curentJigsawTile[j]);
                        if (!usableJigsawTile[i].tilesTree.Contains(usableJigsawTile[i]))
                            curentJigsawTile[j].tilesTree.Add(usableJigsawTile[i]);
                    }
                }
                else if (downid != 0 && downid == usableJigsawTile[i].id)
                {
                    Vector3 targetPos = _thisRect.InverseTransformPoint(usableJigsawTile[i].upPoint.position);
                    Vector3 localPos = _thisRect.InverseTransformPoint(curentJigsawTile[j].downPoint.position);
                    if ((localPos - targetPos).sqrMagnitude <= 225f)
                    {
                        if (usableJigsawTile[i].parentTile != null && usableJigsawTile[i].parentTile != this)
                        {//如果时合成后的图块
                            usableJigsawTile[i].parentTile.transform.SetParent(this.transform);
                            JigsawTileParent destory = usableJigsawTile[i].parentTile;
                            foreach (var item in usableJigsawTile[i].parentTile.curentJigsawTile)
                            {
                                item.transform.SetParent(this.transform);
                                if (!this.curentJigsawTile.Contains(item))
                                    this.curentJigsawTile.Add(item);
                                item.parentTile = this;
                                //setPos
                                item._thisRect.anchoredPosition = curentJigsawTile[j]._thisRect.anchoredPosition + jg.GetTilePos(curentJigsawTile[j].id, item.id);
                            }
                            Destroy(destory.gameObject);
                        }
                        else
                        {
                            usableJigsawTile[i].transform.SetParent(this.transform);
                            usableJigsawTile[i].parentTile = this;
                            usableJigsawTile[i].compound = true;
                            if (!this.curentJigsawTile.Contains(usableJigsawTile[i]))
                                this.curentJigsawTile.Add(usableJigsawTile[i]);
                            usableJigsawTile[i]._thisRect.anchoredPosition = new Vector2(curentJigsawTile[j]._thisRect.anchoredPosition.x, curentJigsawTile[j]._thisRect.anchoredPosition.y - curentJigsawTile[j]._thisRect.sizeDelta.y);
                        }
                        //set
                        curentJigsawTile[j].downPoint = null;
                        usableJigsawTile[i].upPoint = null;
                        curentJigsawTile[j].adjacentTile.downTileid = 0;
                        usableJigsawTile[i].adjacentTile.upTileid = 0;
                        curentJigsawTile[j].tileTree.downTileid = usableJigsawTile[i].id;
                        usableJigsawTile[i].tileTree.upTileid = curentJigsawTile[j].id;
                        if (!usableJigsawTile[i].tilesTree.Contains(curentJigsawTile[j]))
                            usableJigsawTile[i].tilesTree.Add(curentJigsawTile[j]);
                        if (!usableJigsawTile[i].tilesTree.Contains(usableJigsawTile[i]))
                            curentJigsawTile[j].tilesTree.Add(usableJigsawTile[i]);
                    }
                }
                else if (leftid != 0 && leftid == usableJigsawTile[i].id)
                {
                    Vector3 targetPos = _thisRect.InverseTransformPoint(usableJigsawTile[i].rightPoint.position);
                    Vector3 localPos = _thisRect.InverseTransformPoint(curentJigsawTile[j].leftPoint.position);
                    if ((localPos - targetPos).sqrMagnitude <= 225f)
                    {
                        if (usableJigsawTile[i].parentTile != null && usableJigsawTile[i].parentTile != this)
                        {//如果时合成后的图块
                            usableJigsawTile[i].parentTile.transform.SetParent(this.transform);
                            JigsawTileParent destory = usableJigsawTile[i].parentTile;
                            foreach (var item in usableJigsawTile[i].parentTile.curentJigsawTile)
                            {
                                item.transform.SetParent(this.transform);
                                if (!this.curentJigsawTile.Contains(item))
                                    this.curentJigsawTile.Add(item);
                                item.parentTile = this;
                                //setPos
                                item._thisRect.anchoredPosition = curentJigsawTile[j]._thisRect.anchoredPosition + jg.GetTilePos(curentJigsawTile[j].id, item.id);
                            }
                            Destroy(destory.gameObject);
                        }
                        else
                        {
                            usableJigsawTile[i].transform.SetParent(this.transform);
                            usableJigsawTile[i].parentTile = this;
                            usableJigsawTile[i].compound = true;
                            if (!this.curentJigsawTile.Contains(usableJigsawTile[i]))
                                this.curentJigsawTile.Add(usableJigsawTile[i]);
                            usableJigsawTile[i]._thisRect.anchoredPosition = new Vector2(curentJigsawTile[j]._thisRect.anchoredPosition.x - curentJigsawTile[j]._thisRect.sizeDelta.x, curentJigsawTile[j]._thisRect.anchoredPosition.y);
                        }
                        //set
                        curentJigsawTile[j].leftPoint = null;
                        usableJigsawTile[i].rightPoint = null;
                        curentJigsawTile[j].adjacentTile.leftTileid = 0;
                        usableJigsawTile[i].adjacentTile.rightTileid = 0;
                        curentJigsawTile[j].tileTree.leftTileid = usableJigsawTile[i].id;
                        usableJigsawTile[i].tileTree.rightTileid = curentJigsawTile[j].id;
                        if (!usableJigsawTile[i].tilesTree.Contains(curentJigsawTile[j]))
                            usableJigsawTile[i].tilesTree.Add(curentJigsawTile[j]);
                        if (!usableJigsawTile[i].tilesTree.Contains(usableJigsawTile[i]))
                            curentJigsawTile[j].tilesTree.Add(usableJigsawTile[i]);
                    }
                }
            }
        }
    }

    //ReachAStandard
    void ReachAStandard()
    {
        if (curentJigsawTile.Count >= 16)
        {
            jg.isReachAStandard = true;
        }
    }
}
