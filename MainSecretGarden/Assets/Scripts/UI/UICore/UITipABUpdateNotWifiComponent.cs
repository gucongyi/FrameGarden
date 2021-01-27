using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITipABUpdateNotWifiComponent : MonoBehaviour
{
    public Button ButtonOk;
    public Button ButtonCancel;
    public Text TextShow;
    // Start is called before the first frame update
    void Start()
    {
        ButtonOk.onClick.RemoveAllListeners();
        ButtonOk.onClick.AddListener(OnButtonOkClick);
        ButtonCancel.onClick.RemoveAllListeners();
        ButtonCancel.onClick.AddListener(OnButtonCancelClick);
    }

    private void OnButtonCancelClick()
    {
        StaticData.isNotWifiDownloadClick = true;
        StaticData.isNotWifiDownload = false;
        Destroy(gameObject);
    }

    private void OnButtonOkClick()
    {
        StaticData.isNotWifiDownloadClick = true;
        StaticData.isNotWifiDownload = true;
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
