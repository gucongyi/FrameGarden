using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemGainComponent : MonoBehaviour
{

    SeedGrowComponent seedGrowComponent;
    public long SoildId;
    public Button ButtonGain;
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
        ButtonGain.onClick.RemoveAllListeners();
        ButtonGain.onClick.AddListener(()=> {
            StaticData.GetUIWorldHandleComponent().OnButtonGainClick(seedGrowComponent);
        });
    }
}
