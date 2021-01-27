using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuidePanelAnimComponent : MonoBehaviour
{
    public Button ButtonNext;
    public float timeAfterOpenButton;
    string uiName;
    public async void SetInfo(string uiName)
    {
        if (ButtonNext==null)
        {
            return;
        }
        this.uiName = uiName;
        ButtonNext.onClick.RemoveAllListeners();
        ButtonNext.onClick.AddListener(OnButtonNextClick);
        await UniTask.Delay((int)(timeAfterOpenButton * 1000));
        if (ButtonNext == null)
        {
            return;
        }
        ButtonNext.gameObject.SetActive(true);
    }
    private void Awake()
    {
        ButtonNext.gameObject.SetActive(false);
    }

    private void OnButtonNextClick()
    {
        UIComponent.RemoveUI(uiName);
        GuideCanvasComponent._instance.SetLittleStepFinish();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
