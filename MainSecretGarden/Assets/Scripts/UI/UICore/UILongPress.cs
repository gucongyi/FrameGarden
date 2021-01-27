using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UILongPress : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    public float longTime = 0.1f;
    //触发长按后的回调
    public Action LongPressAction;
    float currPressTime;
    bool isPressing;
    bool isTriggerLongPress;
    public void OnPointerDown(PointerEventData eventData)
    {
        currPressTime = longTime;
        isPressing = true;
        isTriggerLongPress = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressing = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPressing&&!isTriggerLongPress)
        {
            currPressTime -= Time.unscaledDeltaTime;
            if (currPressTime<=0f)
            {
                LongPressAction?.Invoke();
                isTriggerLongPress = true;
                //StaticData.DebugGreen($"触发长按！");
            }
        }
    }
}
