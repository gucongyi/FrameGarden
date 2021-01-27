using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter1_MoveBG_3_1 : MonoBehaviour
{
    public Transform parent;

    public GameObject bgTree;//背景树
    public GameObject diban;//地板
    public GameObject fbg2CaoCong;//背景草丛

    public GameObject fbgCaoCong;//前景草丛



    float bg_X = -3296f;
    float fbg_X = -2259f;

    Vector2 treePos = new Vector2(1968f, 441.15f);
    Vector2 dibanPos = new Vector2(2220f, -846f);
    Vector2 fbg2Pos = new Vector2(1860f, 754f);
    Vector2 fbgPos = new Vector2(2550f, 0f);

    public void Start()
    {
        this.transform.DOLocalMoveX(-1590, 3600f);
        var t1 = bgTree.transform.DOLocalMoveX(bg_X, 9f);
        var t2 = diban.transform.DOLocalMoveX(bg_X, 12f);
        var t3 = fbg2CaoCong.transform.DOLocalMoveX(bg_X, 12f);
        var t4 = fbgCaoCong.transform.DOLocalMoveX(fbg_X, 7f);
        t1.SetEase(Ease.Linear);
        t2.SetEase(Ease.Linear);
        t3.SetEase(Ease.Linear);
        t4.SetEase(Ease.Linear);

        //left_Anchor = (left_tree.transform as RectTransform).anchoredPosition;
        //right_Anchor = (right_tree.transform as RectTransform).anchoredPosition;
        //延迟1秒调用 每2秒循环一次
        InvokeRepeating("InstanceTree", 0f, 8f);
        InvokeRepeating("InstanceDiban", 0f, 6f);
        InvokeRepeating("InstanceBbg", 0f, 5f);
        InvokeRepeating("InstanceFbg", 0f, 5f);
    }

    void InstanceTree()
    {
        GameObject go_tree = GameObject.Instantiate(bgTree);
        go_tree.SetActive(true);
        ChapterHelper.SetParent(go_tree, this.transform.parent);
        go_tree.transform.SetSiblingIndex(1);
        go_tree.transform.localPosition = treePos;

        Tween tween = go_tree.transform.DOLocalMoveX(bg_X, 18f).OnComplete(() => { Destroy(go_tree); });
        tween.SetEase(Ease.Linear);
    }
    //地板
    void InstanceDiban()
    {
        GameObject go_diban = GameObject.Instantiate(diban);
        go_diban.SetActive(true);
        ChapterHelper.SetParent(go_diban, parent);

        go_diban.transform.transform.localPosition = dibanPos;

        Tween tween = go_diban.transform.DOLocalMoveX(fbg_X, 15f).OnComplete(() => { Destroy(go_diban); });
        tween.SetEase(Ease.Linear);
    }

    //背景草丛
    void InstanceBbg()
    {
        GameObject go_bbg = GameObject.Instantiate(fbg2CaoCong);
        go_bbg.SetActive(true);
        ChapterHelper.SetParent(go_bbg, parent);

        go_bbg.transform.transform.localPosition = fbg2Pos;

        Tween tween = go_bbg.transform.DOLocalMoveX(fbg_X, 15f).OnComplete(() => { Destroy(go_bbg); });
        tween.SetEase(Ease.Linear);
    }

    //前景草丛
    void InstanceFbg()
    {
        GameObject go_fbg = GameObject.Instantiate(fbgCaoCong);
        go_fbg.SetActive(true);
        ChapterHelper.SetParent(go_fbg, this.transform.parent);

        go_fbg.transform.transform.localPosition = fbgPos;

        Tween tween = go_fbg.transform.DOLocalMoveX(fbg_X, 12f).OnComplete(() => { Destroy(go_fbg); });
        tween.SetEase(Ease.Linear);
    }
}
