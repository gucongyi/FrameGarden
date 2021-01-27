using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectManorRightUp : MonoBehaviour
{
    public RectTransform[] transArray;
    public float animTime = 1f;
    //记录原始位置
    List<Vector3> posTransAnim=new List<Vector3>();
    //收取按钮
    public Button ButtonUp;
    //展开按钮
    public Button ButtonDown;
    // Start is called before the first frame update
    void Start()
    {
        SetButtonDownOrUp(true);
        posTransAnim.Clear();
        for (int i = 0; i < transArray.Length; i++)
        {
            posTransAnim.Add(transArray[i].anchoredPosition);
        }
        ButtonUp.onClick.RemoveAllListeners();
        ButtonUp.onClick.AddListener(OnButtonUpClick);
        ButtonDown.onClick.RemoveAllListeners();
        ButtonDown.onClick.AddListener(OnButtonDownClick);
    }

   

    private void SetButtonDownOrUp(bool isUpOpen)
    {
        ButtonUp.gameObject.SetActive(isUpOpen);
        ButtonDown.gameObject.SetActive(!isUpOpen);
    }

    private async void OnButtonUpClick()
    {
        ButtonUp.gameObject.SetActive(false);
        for (int i = 0; i < transArray.Length; i++)
        {
            PlayAnimUp(i);
        }
        await UniTask.Delay((int)animTime * 1000+100);
        ButtonDown.gameObject.SetActive(true);
    }

    private void PlayAnimUp(int i)
    {
        DOTween.To(() => transArray[i].anchoredPosition, (pos) =>
        {
            transArray[i].anchoredPosition = pos;
        },
                    ButtonDown.GetComponent<RectTransform>().anchoredPosition, animTime);
        DOTween.To(() => transArray[i].localScale, (localScale) => transArray[i].localScale = localScale, Vector3.zero, animTime);
    }

    private async void OnButtonDownClick()
    {
        ButtonDown.gameObject.SetActive(false);
        for (int i = 0; i < transArray.Length; i++)
        {
            PlayAnimDown(i);
        }
        await UniTask.Delay((int)animTime * 1000+100);
        ButtonUp.gameObject.SetActive(true);
    }

    private void PlayAnimDown(int i)
    {
        DOTween.To(() => transArray[i].anchoredPosition, (Vector2 pos) => transArray[i].anchoredPosition = pos, posTransAnim[i], animTime);
        DOTween.To(() => transArray[i].localScale, (localScale) => transArray[i].localScale = localScale, Vector3.one, animTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
