using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chapter2_module1 : ChapterControllerBasics
{
    #region 字段
    /// <summary>
    /// 背景点击按钮
    /// </summary>
    Button _bgClickBtn;
    /// <summary>
    /// 点击计数
    /// </summary>
    int _clickIndex = 0;
    /// <summary>
    /// 第一场景
    /// </summary>
    RectTransform _oneRect;
    /// <summary>
    /// 手机点击按钮
    /// </summary>
    Button _phoneBtn;
    /// <summary>
    /// 条漫1
    /// </summary>
    CaricaturePlayerController _caricaturePlayerControllerOne;
    /// <summary>
    /// 购物
    /// </summary>
    ShoppingController _shoppingController;
    /// <summary>
    /// 除草
    /// </summary>
    WeedingController _weedingController;
    /// <summary>
    /// 角色动作切换
    /// </summary>
    [SerializeField]
    List<Sprite> _roleS = new List<Sprite>();
    /// <summary>
    /// 角色对象
    /// </summary>
    Image _roleImage;
    #endregion
    #region 函数
    // Start is called before the first frame update
    void Start()
    {
        //Initial();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Initial()
    {
        _bgClickBtn = GetComponent<Button>();
        _bgClickBtn.onClick.RemoveAllListeners();
        _bgClickBtn.onClick.AddListener(OnClickBg);
        //_roleImage = transform.Find("Role").GetComponent<Image>();
        _oneRect = transform.Find("One").GetComponent<RectTransform>();
        _phoneBtn = _oneRect.Find("PhoneBtn").GetComponent<Button>();
        _caricaturePlayerControllerOne = transform.Find("CaricaturePlayer").GetComponent<CaricaturePlayerController>();
        _shoppingController = transform.Find("Shopping").GetComponent<ShoppingController>();

        _weedingController = transform.Find("Weeding").GetComponent<WeedingController>();

        _phoneBtn.onClick.RemoveAllListeners();
        _phoneBtn.onClick.AddListener(OnClickPhoneBtn);

        OnClickBg();
        base.Initial();
    }
    /// <summary>
    /// 手机点击
    /// </summary>
    private async void OnClickPhoneBtn()
    {
        await ChapterTool.OpenDialogueBoxWeChat(13000001, 10000001, 10000003, () =>
        {
            Debug.Log("微信聊天完毕！");
            _oneRect.gameObject.SetActive(false);
            //backBtn.gameObject.SetActive(false);//聊天完成 隐藏返回按钮
            _caricaturePlayerControllerOne.Initial(() =>
            {
                _caricaturePlayerControllerOne.gameObject.SetActive(false);
                _shoppingController.Initial(() =>
                {
                    Debug.Log("购物结束");
                    _weedingController.Initial(() =>
                    {
                        Debug.Log("除草结束");
                        NextStep();
                    });
                });
                //NextStep();
            }, null);
        });
    }
    /// <summary>
    /// 开关点击按钮
    /// </summary>
    /// <param name="isOpen"></param>
    void OpenClickBtn(bool isOpen)
    {
        _bgClickBtn.enabled = isOpen;
    }
    /// <summary>
    /// 点击背景
    /// </summary>
    private void OnClickBg()
    {
        OpenClickBtn(false);
        StepCut(_clickIndex);
        _clickIndex++;
    }
    /// <summary>
    /// 步骤切换
    /// </summary>
    /// <param name="index"></param>
    public async void StepCut(int index)
    {
        switch (index)
        {
            case 0:
                await ChapterTool.MoveUi(_oneRect, new Vector2(1320.1f, 0), 0.1f, 0.1f, null, () => { _phoneBtn.enabled = true; _phoneBtn.gameObject.SetActive(true); });
                break;
        }
    }
    #endregion
}
