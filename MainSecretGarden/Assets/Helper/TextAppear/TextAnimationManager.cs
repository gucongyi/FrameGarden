using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TextAnimationManager : MonoBehaviour
{
    [SerializeField]
    public UnityEvent onAnimationEnd;
    const float constSpeed = 0.1f;
    public float letterPause;

    public string word;
    public Text text { get; set; }

    //是否已初始化
    private bool _isInitialized = false;

    //是否正在播放中
    public bool _IsPlaying { get; set; } = false;

    private void Awake()
    {
        Text comp = GetComponent<Text>();
        comp.text = "";
        letterPause = constSpeed;
    }

    private void OnDisable()
    {
        Stop();
    }

    private void Init()
    {
        if (!_isInitialized)
        {
            GetComponent<Text>().text = "";
            text = GetComponent<Text>();
            _isInitialized = true;
        }
    }

    public void Play()
    {
        Init();
        StartCoroutine(TypeText());
    }
    public void Speed6Play()
    {
        if (_IsPlaying)
        {
            letterPause = letterPause / 6f;
        }
    }
    public void ResetSpeedPlay()
    {
        letterPause = constSpeed;
    }
    public void Stop()
    {
        if (_IsPlaying)
        {
            _IsPlaying = false;
            StopCoroutine(TypeText());
            text.text = word;
        }
    }



    private IEnumerator TypeText()
    {
        _IsPlaying = true;
        text.text = "";
        foreach (char letter in word.ToCharArray())
        {
            if (_IsPlaying)
            {
                text.text += letter;
                yield return new WaitForSeconds(letterPause);
            }
        }
        _IsPlaying = false;
        onAnimationEnd?.Invoke();
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
