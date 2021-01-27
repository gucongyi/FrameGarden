using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITipABUpdateProgressComponent : MonoBehaviour
{
    public BundleDownloadInfo DownLoadInfo;
    public Image ImageFgProgress;
    public Text textPercent;
    //public Text _loading;
    private long lastDownloaded;

    private float _targetMovePos = 0; //原始位置
    private float _moveLength = 0; //需要移动的距离

    // Start is called before the first frame update
    void Start()
    {
        if (_moveLength == 0) 
        {
            _targetMovePos = ImageFgProgress.transform.localPosition.x;
            var rect = (RectTransform)ImageFgProgress.transform;
            _moveLength = rect.sizeDelta.x;
        }

        Vector3 pos = ImageFgProgress.transform.localPosition;
        pos.x = _targetMovePos - _moveLength;
        ImageFgProgress.transform.localPosition = pos;

    }

    public void UpdateShow()
    {
        textPercent.text = $"{DownLoadInfo.Progress}%";
        float f = DownLoadInfo.Progress / 100f;

        Vector3 pos = ImageFgProgress.transform.localPosition;

        pos.x = (_targetMovePos - _moveLength) + _moveLength * f;
        ImageFgProgress.transform.localPosition = pos;
    }
    // Update is called once per frame
    void Update()
    {
        if (DownLoadInfo.IsEnd) return;
        if (DownLoadInfo == null) return;
        if (!DownLoadInfo.IsStart) return;
        UpdateShow();
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
