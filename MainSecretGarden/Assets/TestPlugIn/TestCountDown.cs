using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestCountDown : MonoBehaviour
{
    public Text text1;
    public Text text2;
    public Text text3;
    // Start is called before the first frame update
    void Start()
    {
        StaticData.CreateTimer(100f, true, (go) => { text1.gameObject.SetActive(false); }, 
            (timeCount) => 
            {
                text1.text = $"{timeCount}";
            },"Test");
        StaticData.CreateTimer(50f, true, (go) => { text2.gameObject.SetActive(false); },
            (timeCount) =>
            {
                text2.text = $"{timeCount}";
            }, "Test");
        StaticData.CreateTimer(20f, false, (go) => { text3.gameObject.SetActive(false); },
            (timeCount) =>
            {
                text3.text = $"{timeCount}";
            }, "Test");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
