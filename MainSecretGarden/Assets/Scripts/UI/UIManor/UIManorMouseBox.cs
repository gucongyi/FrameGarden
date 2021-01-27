using System;
using System.Collections;
using System.Collections.Generic;
using Game.Protocal;
using UnityEngine;
using UnityEngine.UI;

public class UIManorMouseBox : MonoBehaviour
{
    public Image iconBox;
    public Button ButtonGetOne;
    public Button ButtonGetDouble;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal async void SetInfo(CSWareHouseStruct awardInfo, MouseManorManager mouseManorManager)
    {
        var itemConfig = StaticData.configExcel.GetGameItemByID(awardInfo.GoodId);
        iconBox.sprite = await ABManager.GetAssetAsync<Sprite>(itemConfig.Icon);
        ButtonGetOne.onClick.RemoveAllListeners();
        ButtonGetOne.onClick.AddListener(mouseManorManager.OnSingleReceiveClick);
        ButtonGetDouble.onClick.RemoveAllListeners();
        ButtonGetDouble.onClick.AddListener(mouseManorManager.OnDoubleReceiveClick);
    }
}
