using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipsTileComponent : MonoBehaviour
{
    public Transform selfParent;
    public GameObject goCanPlace;
    public GameObject goNotAllow;
    public GameObject goUpAnim;

    //全局一份
    public static TipsTileComponent _instance;

    private void Awake()
    {
        _instance = this;
    }
    public void CloseAll()
    {
        goCanPlace.SetActive(false);
        goNotAllow.SetActive(false);
        transform.parent = selfParent;
    }
    public async UniTask PlayUpAnim(System.Threading.CancellationToken token)
    {
        goUpAnim.SetActive(true);
        await UniTask.Delay(600,cancellationToken: token);
        goUpAnim.SetActive(false);
    }
    internal void CloseUpAnim()
    {
        goUpAnim.SetActive(false);
    }
    public void SetUpAnimPos(Transform transParent)
    {
        transform.parent = transParent;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
    }
    public void SetCanPlace(Transform transParent, bool isCanPlace, TileComponent tileComponent)
    {
        transform.parent = transParent;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        goCanPlace.SetActive(isCanPlace);
        goNotAllow.SetActive(!isCanPlace);
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
