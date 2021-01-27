using System;
using System.Collections;
using System.Collections.Generic;
using Game.Protocal;
using UnityEngine;
using UnityEngine.UI;

public class UIManorUnlockRewardComponent : MonoBehaviour
{
    public Button ButtonGet;
    public LoopHorizontalScrollRect scrollReward;
    public Transform transAnim;
    ManorRegionComponent manorRegionComponent;
    List<CSWareHouseStruct> awardIds;
    SCUnlockArea regionUnLockDialogEndSucc;
    private void Awake()
    {
        ButtonGet.onClick.RemoveAllListeners();
        ButtonGet.onClick.AddListener(OnButtonGetClick);
    }

    private void OnButtonGetClick()
    {
        if (transAnim != null)
        {
            UniversalTool.CancelPopAnim(transAnim, () => {
                manorRegionComponent.OnWorkSheldRewardGet(awardIds, regionUnLockDialogEndSucc);
                UIComponent.RemoveUI(UIType.UIManorUnlockReward);
                //刷新装饰物
                if (UIComponent.IsHaveUI(UIType.UIManor))
                {
                    UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor).RefreshDecorateList();
                }
            });
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        if (transAnim != null)
            UniversalTool.StartPopupAnim(transAnim);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    internal void SetRewards(ManorRegionComponent manorRegionComponent, List<CSWareHouseStruct> awardIds, SCUnlockArea regionUnLockDialogEndSucc)
    {
        this.manorRegionComponent = manorRegionComponent;
        this.awardIds = awardIds;
        Root2dSceneManager._instance.awardIdsCurrManorRegion = awardIds;
        this.regionUnLockDialogEndSucc = regionUnLockDialogEndSucc;
        scrollReward.ClearCells();
        scrollReward.totalCount = awardIds.Count;
        scrollReward.RefillCells();
    }
}
