using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGoStealComponent : MonoBehaviour
{
    SeedGrowComponent seedGrowComponent;
    public long SoildId;
    public Button ButtonSteal;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (seedGrowComponent == null)
        {
            return;
        }
        if (seedGrowComponent.tileComponent == null)
        {
            return;
        }
        List<UIWorldHandleManager.StealClass> listStealClass = StaticData.GetUIWorldHandleComponent().listStealClass;
        var StealClass = listStealClass.Find(x => x.SolidId == seedGrowComponent.tileComponent.SoilId);
        if (StealClass != null && StealClass.isSteal==false)
        {
            StaticData.DestroySafe(this);
            return;
        }
        if (seedGrowComponent.tileComponent.CropGoodId == 0)
        {
            StaticData.DestroySafe(this);
            return;
        }
        SoildId = seedGrowComponent.tileComponent.SoilId;
        gameObject.GetComponent<RectTransform>().anchoredPosition = StaticData.ManorWorldPointToUICameraAnchorPos(seedGrowComponent.tileComponent.TransPointGainHandle.transform.position);
    }

    internal void SetSeedGrowComponent(SeedGrowComponent seedGrowComponent)
    {
        this.seedGrowComponent = seedGrowComponent;
        //偷取
        ButtonSteal.onClick.RemoveAllListeners();
        ButtonSteal.onClick.AddListener(()=> {
            StaticData.GetUIWorldHandleComponent().OnButtonStealClick(seedGrowComponent);
        });
    }
}
