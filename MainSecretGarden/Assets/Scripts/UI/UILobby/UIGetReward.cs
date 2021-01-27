using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGetReward : MonoBehaviour
{
    [Header("图片icon")]
    private Sprite sprite;//精灵图片
    [Header("目标点")]
    private Vector3 endPoint;//目的点 屏幕坐标
    //[Header("特效")]
    //public ParticleSystem addPartic;//增加特效

    private long lastNumber;//之前的数量
    //设置属性
    public void Play(string rewardSpriteName, Vector3 startScreenPoint, Vector3 endScreenPoint, long lastNumber, long changeNumber)
    {
        this.endPoint = endScreenPoint;
        this.sprite = ABManager.GetAsset<Sprite>(rewardSpriteName);
        this.lastNumber = lastNumber;
        PlayAnim(startScreenPoint, changeNumber);
    }

    public void PlayAnim(Vector3 startScreenPoint, long changeNumber)
    {
        //Vector3 startPos = LocalPositonToLocalPosition(startGO, transform.parent.gameObject);
        transform.localPosition = startScreenPoint;

        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Image>().sprite = sprite;
        }      //总量-变化量
               //lastGold = PlayerDataManage._instance.GetGoldNumber() - changeNumber;
        GetComponent<Animator>().Play("GetGoldDimonds");
    }

    /// <summary>
    /// 外部animator关键帧动画
    /// </summary>
    public void UnitMoveAnim()
    {
        transform.DOLocalMove(endPoint, 0.3f).OnComplete(() =>
        {
            Destroy(this.gameObject);
            //addPartic.transform.localScale = new Vector3(1, 1, 1);
            //for (int i = 0; i < addPartic.transform.childCount; i++)//播放特效
            //{
            //    addPartic.transform.GetChild(i).localScale = new Vector3(1, 1, 1);
            //}
            //addPartic.Play();

            //金币总数的大小动画
            //LayerControler._instence.goldButton.transform.DOScale(1.2f, 0.1f).OnComplete(() =>
            //{
            //LayerControler._instence.goldButton.transform.DOScale(1.0f, 0.1f);
            //});

            //金币总数值改变动画
            //DOTween.To(() => lastGold, x =>
            //{
            //    LayerControler._instence.goldNumberText.text = ToolControler.UnitConversion(x);
            //}, PlayerDataManage._instance.GetGoldNumber(), 0.5f).OnComplete(() =>
            //{
            //    LayerControler._instence.goldNumberText.text = ToolControler.UnitConversion(PlayerDataManage._instance.GetGoldNumber());
            //});
        });
    }

    /// <summary>
    /// 对象坐标转换
    /// </summary>
    Vector3 LocalPositonToLocalPosition(GameObject LocalObj, GameObject targetObj)
    {
        Transform _parent = LocalObj.transform.parent;
        if (_parent != null)
        {
            Vector3 _wordSpace = _parent.TransformPoint(LocalObj.transform.localPosition);
            return targetObj.transform.InverseTransformPoint(_wordSpace);
        }
        return LocalObj.transform.position;
    }
}
