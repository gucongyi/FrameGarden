using System;
using System.Net;
using UnityEngine;
using UnityEngine.UI;


public class UISceneLoadingComponent : MonoBehaviour
{
    #region UI Component
    public Image FgProgress;
    public Text TextProgress;
    #endregion
    private float realProgress;
    private float frameProgress;

    private float _targetMovePos = 0; //原始位置
    private float _moveLength = 0; //需要移动的距离

    public void Start()
    {
        if (_moveLength == 0)
        {
            _targetMovePos = FgProgress.transform.localPosition.x;
            var rect = (RectTransform)FgProgress.transform;
            _moveLength = rect.sizeDelta.x;
        }

        Vector3 pos = FgProgress.transform.localPosition;
        pos.x = _targetMovePos - _moveLength;
        FgProgress.transform.localPosition = pos;

        Reset();
    }

    public void Reset()
    {
        realProgress = 0f;
        frameProgress = 0f;
        //设置UI
        //FgProgress.fillAmount = 0;
        TextProgress.text = $"0%";
    }

    public void OnRealProgress(float realProgress)
    {
        this.realProgress = realProgress;
        SetSliderValue();
    }

    public void OnFrameProgress(float frameProgress)
    {
        this.frameProgress = frameProgress;
        SetSliderValue();
    }

    public void SetSliderValue()
    {
        if (_moveLength == 0) 
        {
            return;
        }

        float currProgress = Mathf.Min(realProgress, frameProgress);
        //FgProgress.fillAmount = currProgress / 100f;

        float f = currProgress / 100f;
        Vector3 pos = FgProgress.transform.localPosition;
        pos.x = (_targetMovePos - _moveLength) + _moveLength * f;
        FgProgress.transform.localPosition = pos;

        if (currProgress < 10)
        {
            TextProgress.text = $"{String.Format("{0:0}", currProgress)}%";
        }
        else if (currProgress < 100)
        {
            TextProgress.text = $"{String.Format("{0:00}", currProgress)}%";
        }
        else
        {
            TextProgress.text = $"100%";
        }
        
    }

    public void Update()
    {

    }

    private void OnEnable()
    {
        SpawnAnimEffect();
    }

    private void OnDisable()
    {
        HideAnimEffect();
    }

    private GameObject _animEffect;
    public async void SpawnAnimEffect()
    {

        Transform parent = UIRoot.instance.GetUIRootCanvas().transform.parent;
        string perfabName = "LoadingEffect";
        var obj = await ABManager.GetAssetAsync<GameObject>(perfabName);
        _animEffect = Instantiate(obj, parent);
    }

    private void HideAnimEffect()
    {
        _animEffect.SetActive(false);
        Destroy(_animEffect);
        _animEffect = null;
    }

}
