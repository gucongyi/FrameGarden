using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Chapter1_MoveBG : MonoBehaviour
{
    public GameObject left_tree;
    public GameObject right_tree;

    float left_X = -80;
    float right_X = 500;

    Vector2 left_Anchor;// = new Vector2(-1207, -1189);
    Vector2 right_Anchor;// = new Vector2(2064f, -1179);

    public void Start()
    {
        left_Anchor = (left_tree.transform as RectTransform).anchoredPosition;
        right_Anchor = (right_tree.transform as RectTransform).anchoredPosition;
        //延迟1秒调用 每2秒循环一次
        InvokeRepeating("InstanceTree", 0f, 15f);
    }

    void InstanceTree()
    {
        GameObject go_left = GameObject.Instantiate(left_tree);
        go_left.SetActive(true);
        ChapterHelper.SetParent(go_left, this.transform);
        (go_left.transform as RectTransform).anchoredPosition = left_Anchor;
        go_left.transform.localScale = new Vector3(1.5f, 1.5f, 0);

        GameObject go_right = GameObject.Instantiate(right_tree);
        go_right.SetActive(true);
        ChapterHelper.SetParent(go_right, this.transform);
        (go_right.transform as RectTransform).anchoredPosition = right_Anchor;
        go_right.transform.localScale = new Vector3(1.5f, 1.5f, 0);

        go_left.transform.DOLocalMoveX(left_X, 30f);
        go_left.transform.DOScale(0f, 30f).OnComplete(() => Destroy(go_left));

        go_right.transform.DOLocalMoveX(right_X, 30f);
        go_right.transform.DOScale(0f, 30f).OnComplete(() => Destroy(go_right));


    }
}
