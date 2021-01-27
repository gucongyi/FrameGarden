using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManorRegionPay : MonoBehaviour
{
    public Button ButtonPay;
    public Button ButtonCancel;
    public Text TextRegion;
    [HideInInspector]
    public ManorRegionComponent manorRegionComponent;
    public LoopHorizontalScrollRect LoopHorizontalScrollRect;
    public Transform transAnim;
    // Start is called before the first frame update
    void Start()
    {
        ButtonPay.onClick.RemoveAllListeners();
        ButtonPay.onClick.AddListener(OnButtonPayClick);
        ButtonCancel.onClick.RemoveAllListeners();
        ButtonCancel.onClick.AddListener(OnButtonCancelClick);
    }
    private void OnEnable()
    {
        if (transAnim != null)
            UniversalTool.StartPopupAnim(transAnim);
    }
    private void OnButtonCancelClick()
    {
        if (transAnim != null)
            UniversalTool.CancelPopAnim(transAnim, () => {
                UIComponent.RemoveUI(UIType.UIManorRegionPay);
            });
    }

    private void OnButtonPayClick()
    {
        if (StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.isCurrStepGuiding)
        {
            if (GuideCanvasComponent._instance.CurrExecuteGuideLittleStepDefine.Id== 10013)
            {
                GuideCanvasComponent._instance.SetLittleStepFinish();
            }
        }
        manorRegionComponent.BeginPay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void SetManorRegionComponent(ManorRegionComponent manorRegionComponent)
    {
        this.manorRegionComponent = manorRegionComponent;
        TextRegion.text = $"{manorRegionComponent.regionId}";
        //道具判定
        var dealClass = StaticData.configExcel.GetAreaUnlockByID(manorRegionComponent.regionId).ConsumptionGood;
        LoopHorizontalScrollRect.totalCount = dealClass.Count;
        LoopHorizontalScrollRect.RefillCells();
    }
}
