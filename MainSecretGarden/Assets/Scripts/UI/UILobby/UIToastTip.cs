using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToastTip : MonoBehaviour
{
    public Text tipText;
    float currTime = 0f;
    const float showTime = 3f;

    public void Init(string content)
    {
        tipText.text = content;
        currTime = 0f;
    }
    private void Update()
    {
        currTime += Time.unscaledDeltaTime;
        if (currTime > showTime)
        {
            currTime = 0f;
            Destroy(gameObject);
        }
    }
}
