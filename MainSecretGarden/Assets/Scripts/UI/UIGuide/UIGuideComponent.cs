using System;
using System.Collections;
using System.Collections.Generic;
using Company.Cfg;
using UnityEngine;
using UnityEngine.UI;

public class UIGuideComponent : MonoBehaviour
{
    public GameObject GoUIGuide;
    GuideLittleStepDefine guideLittleStepDefine;
    GameObject transOrigin;
    bool isFollow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transOrigin == null)
        {
            return;
        }
        if (guideLittleStepDefine!=null&& isFollow)
        {
            SetFingerPos();//跟随
            if (guideLittleStepDefine.Id == 10003&& GuideCanvasComponent._instance.isLastChild(UIComponent.GetExistUI(UIType.UIManor).transform))//作物面板关闭，ui也关闭
            {
                GoUIGuide.SetActive(transOrigin.activeInHierarchy&& GuideCanvasComponent._instance.isCurrStepGuiding);
            }
        }
    }

    private void SetFingerPos()
    {
        if (transOrigin == null)
        {
            return;
        }
        Vector2 AnchorPos = Vector2.zero;
        if (!guideLittleStepDefine.is3dCamera)
        {
            AnchorPos = StaticData.RectTransWorldPointToUICameraAnchorPos(transOrigin.transform.position);
        }
        else
        {
            AnchorPos = StaticData.ManorWorldPointToUICameraAnchorPos(transOrigin.transform.position);
        }
        GoUIGuide.transform.GetComponent<RectTransform>().anchoredPosition = AnchorPos + new Vector2(guideLittleStepDefine.offset.X, guideLittleStepDefine.offset.Y);
    }

    internal void SetUIInfo(GuideLittleStepDefine guideLittleStepDefine,GameObject transOrigin)
    {
        this.guideLittleStepDefine = guideLittleStepDefine;
        this.transOrigin = transOrigin;
        SetFingerPos();
        GoUIGuide.GetComponent<RootFingerComponent>().SetFinger(guideLittleStepDefine.isRightFinger);
        isFollow = true;
        if (guideLittleStepDefine.Id== 10019)//大富翁骰子界面不跟随
        {
            isFollow = false;
        }
    }
}
