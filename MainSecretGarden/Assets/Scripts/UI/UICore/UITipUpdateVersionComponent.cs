using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITipUpdateVersionComponent : MonoBehaviour
{
    public Button ButtonOk;

    private float _timer;
    /// <summary>
    /// 等待关闭时间
    /// </summary>
    private float _waitTime = 6;//s
    // Start is called before the first frame update
    void Start()
    {
        _timer = 0;
        ButtonOk.onClick.RemoveAllListeners();
        ButtonOk.onClick.AddListener(OnButtonOkClick);
    }

    void Update()
    {
        //if (_timer >= _waitTime)
        //{
        //    OnButtonOkClick();
        //    _timer = 0f;
        //}
        //else 
        //{
        //    _timer += Time.deltaTime;
        //}
    }

    private void OnButtonOkClick()
    {
        StaticData.QuitApplication();
        Destroy(gameObject);
    }

    // Update is called once per frame
   
}
