using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 大富翁普通事件效果
/// </summary>
public class ZillionaireOrdinaryEventEffectController : MonoBehaviour
{

    #region 变量

    /// <summary>
    /// 事件效果信息
    /// </summary>
    [SerializeField]
    protected List<ZillionaireEventEffect> zillionaireEventEffects = new List<ZillionaireEventEffect>();

    protected ZillionaireEventEffect _curEffect = null;

    #endregion

    #region 方法

    protected Action CloseUICallback;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void InitValue(int eventID, Action closeUICallback = null) 
    {
        Debug.Log("播放事件提示 131");
        CloseUICallback = closeUICallback;
        if (_curEffect != null)
        {
            _curEffect.Pannel.SetActive(false);
            _curEffect.Icon.SetActive(false);
        }

        ZillionaireEventEffect eventEffect = zillionaireEventEffects.Find(effect => effect.EventID == eventID);
        if (eventEffect == null) 
        {
            Debug.Log("播放事件提示 132");
            CloseUI();
            return;
        }
        _curEffect = eventEffect;
        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
        _curEffect.Pannel.SetActive(true);
        _curEffect.Icon.SetActive(true);
        Debug.Log("播放事件提示 133");
        PlayEffect();
    }

    protected virtual void PlayEffect() 
    {
        transform.DOScale(Vector3.one, 0.3f).OnComplete( ()=> WaitNext());
    }

    protected virtual async void WaitNext() 
    {
        //单位
        await UniTask.Delay(800);
        transform.DOScale(Vector3.zero, 0.12f).OnComplete(() => CloseUI());
        //DOTween.KillAll();
    }


    protected virtual void CloseUI() 
    {
        CloseUICallback?.Invoke();
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }

    public void DestroySelf()
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    #endregion
}

[Serializable]
public class ZillionaireEventEffect
{
    public int EventID;
    public GameObject Pannel;
    public GameObject Icon;
}
