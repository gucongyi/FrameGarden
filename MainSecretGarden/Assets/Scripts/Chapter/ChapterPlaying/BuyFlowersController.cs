using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyFlowersController : MonoBehaviour
{
    Image progressBar;
    /// <summary>
    /// 已选的花
    /// </summary>
    List<Image> AllOption = new List<Image>();
    /// <summary>
    /// 心
    /// </summary>
    [SerializeField]
    List<Image> starImages = new List<Image>();
    /// <summary>
    /// 花数量
    /// </summary>
    [SerializeField]
    List<Button> flowerBtns = new List<Button>();

    /// <summary>
    /// 结束回调
    /// </summary>
    Action endAction;

    /// <summary>
    /// 结账按钮
    /// </summary>
    Button endBtn;

    // Start is called before the first frame update
    void Start()
    {
        Initial(null);
    }

    // Update is called once per frame
    void Update()
    {

    }


    /// <summary>
    /// 初始化组件
    /// </summary>
    /// <param name="endAction"></param>
    public void Initial(Action endAction)
    {
        progressBar = transform.Find("progressBar").GetComponent<Image>();
        endBtn = transform.Find("EndBtn").GetComponent<Button>();

        this.endAction = endAction;
        RegisterEvent();
    }
    /// <summary>
    /// 注册事件
    /// </summary>
    void RegisterEvent()
    {
        //结账按钮
        endBtn.onClick.RemoveAllListeners();
        endBtn.onClick.AddListener(ClickEndBtn);
        //花按钮
        for (int i = 0; i < flowerBtns.Count; i++)
        {
            int index = i;
            flowerBtns[index].onClick.RemoveAllListeners();
            flowerBtns[index].onClick.AddListener(() =>
            {
                if (starImages[index].gameObject.activeSelf)
                {
                    starImages[index].gameObject.SetActive(false);
                    AllOption.Remove(starImages[index]);
                }
                else
                {
                    starImages[index].gameObject.SetActive(true);
                    AllOption.Add(starImages[index]);
                }
                UpdataProgressBar();
            });
        }
    }
    /// <summary>
    /// 更新进度条
    /// </summary>
    void UpdataProgressBar()
    {
        float percent = AllOption.Count / 5.0f;
        if (percent >= 1f)
        {
            percent = 1;
            endBtn.gameObject.SetActive(true);
        }
        else
        {
            endBtn.gameObject.SetActive(false);
        }
        progressBar.fillAmount = percent;
    }

    /// <summary>
    /// 点击结账按钮
    /// </summary>
    void ClickEndBtn()
    {

        endAction?.Invoke();
    }


}
