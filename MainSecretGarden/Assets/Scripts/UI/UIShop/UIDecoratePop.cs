using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Company.Cfg;
using Game.Protocal;
using static UIPoolItemGood;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class UIDecoratePop : MonoBehaviour
{
    private StoreDefine curStoreDefine;
    public Transform content;
    public Button leftBtn;//金币购买按钮
    public Button rightBtn;//钻石购买按钮

    public Transform _rightBuyTF;//弹窗中的钻石按钮
    public Transform _leftBuyTF;//弹窗中的金币按钮

    public Button _RightReversalBtn;//向右旋转
    public Button _LeftReversalBtn;//向左旋转
    private int _ReversalState = 0;//旋转状态 0为物品正常状态 1为反转状态 
    private string  newIconName;//用于保存旋转后的图标；
    public static GameItemDefine itemDefine;//保存旋转物品详情

    private bool isRightBtnOrnament;//判断装饰界面是否从钻石入口进入的

    private bool isPromotion = false;
    private int todayBuyLimit;  //今日购买限制
    private string limitStr;
    private string iconName;
    private int popIconName;//用于弹窗的名字;
    private UIOrnament uIOrnament;//被点击的装饰物
    private TypeGameItem typeGameItem;//用于保存判断当前物品的类型;
    private ShopType shopType;

    public UIShopChoiceAmount uiShopChoiceAmount;//购买弹窗
    public Button downClickBtn;//向下点击按钮
    public bool isFirstUpdatePopInfo = false;//用于判断是否显示第一个装饰物
    private string initItemName;//用于保存默认即第一个物品的名字
    public bool isItemClick;//用于判断是否有装饰物被点击
    public string clickName;//用于保存被点击装饰物的名字


    private float contentY;
    private float cellY;
    private int cellNum;

    private void Start()
    {
        //Init();
        cellY = content.GetComponent<GridLayoutGroup>().cellSize.y + content.GetComponent<GridLayoutGroup>().spacing.y/2;
        //contentY = (content as RectTransform).rect.height;
        cellNum = content.childCount;

        _RightReversalBtn.onClick.RemoveAllListeners();
        _RightReversalBtn.onClick.AddListener(Reversal);
        _LeftReversalBtn.onClick.RemoveAllListeners();
        _LeftReversalBtn.onClick.AddListener(Reversal);

    }

    //更新显示第一个装饰物品
    private void FirstUpdatePopInfo()
    {
        content.GetChild(0).Find("Background").gameObject.SetActive(true);
        //更新数据
        itemDefine = content.GetChild(0).GetComponent<UIOrnament>().itemDefine;
        //确认点击状态
        content.GetChild(0).GetComponent<UIOrnament>().isClick = true;
        //更新图片
        transform.Find("Icon").GetComponent<Image>().sprite = content.GetChild(0).Find("Icon").GetComponent<Image>().sprite;
        //更新名字
        transform.Find("IconName").GetComponent<Text>().text = content.GetChild(0).Find("ItemName").GetComponent<Text>().text;
        initItemName= content.GetChild(0).Find("ItemName").GetComponent<Text>().text;
        //更新价格
        transform.Find("BuyBtn_gold/CurPrice").GetComponent<Text>().text = content.GetChild(0).GetComponent<UIOrnament>()._topPriceText;//金币价格
        transform.Find("BuyBtn_diamond/CurPrice").GetComponent<Text>().text = content.GetChild(0).GetComponent<UIOrnament>()._topPriceText2;//钻石价格
        transform.gameObject.SetActive(true);
        //判断是否有旋转
        IsHasReversal();

        isFirstUpdatePopInfo = true;
    }

    //列表滑动的时候
    public void LoopVerticalScrollOnChange(Vector2 vector)
    {
        if (content.GetChild(0).Find("Background").gameObject == null) return;
        for (int i = 1; i < content.childCount; i++)
        {
            content.GetChild(i).Find("Background").gameObject.SetActive(false);
        }
        //默认没点击装饰物
        if (content.GetChild(0).Find("ItemName").GetComponent<Text>().text == initItemName&&isItemClick==false)
        {
            content.GetChild(0).Find("Background").gameObject.SetActive(true);
        }
        //点击了装饰物
        for (int i = 0; i < content.childCount; i++)
        {
            if(content.GetChild(i).Find("ItemName").GetComponent<Text>().text == clickName)
            {
                content.GetChild(i).Find("Background").gameObject.SetActive(true);
            }
        }

    }

    public void Init()
    {
        leftBtn.onClick.RemoveAllListeners();
        leftBtn.onClick.AddListener(LeftBtnClick);
        rightBtn.onClick.RemoveAllListeners();
        rightBtn.onClick.AddListener(RightBtnClick);

        downClickBtn.onClick.RemoveAllListeners();
        //downClickBtn.onClick.AddListener(DownClick);

        isPromotion = content.GetChild(0).GetComponent<UIOrnament>().isPromotion;
        limitStr = content.GetChild(0).GetComponent<UIOrnament>().limitStr;
        iconName = content.GetChild(0).GetComponent<UIOrnament>().iconName;
        popIconName = content.GetChild(0).GetComponent<UIOrnament>().popIconName;
        uIOrnament = content.GetChild(0).GetComponent<UIOrnament>();
        typeGameItem = content.GetChild(0).GetComponent<UIOrnament>().typeGameItem;
        shopType = content.GetChild(0).GetComponent<UIOrnament>().shopType;
        curStoreDefine = content.GetChild(0).GetComponent<UIOrnament>().curStoreDefine;
        todayBuyLimit = content.GetChild(0).GetComponent<UIOrnament>().todayBuyLimit;

        FirstUpdatePopInfo();

    }

    private void LeftBtnClick()
    {

        _rightBuyTF.gameObject.SetActive(false);
        _leftBuyTF.gameObject.SetActive(true);

        isRightBtnOrnament = false;

        for (int i = 0; i < content.childCount; i++)
        {
            if (content.GetChild(i).GetComponent<UIOrnament>().isClick)
            {
                isPromotion = content.GetChild(i).GetComponent<UIOrnament>().isPromotion;
                todayBuyLimit = content.GetChild(i).GetComponent<UIOrnament>().todayBuyLimit;
                limitStr = content.GetChild(i).GetComponent<UIOrnament>().limitStr;
                iconName = content.GetChild(i).GetComponent<UIOrnament>().iconName;
                popIconName = content.GetChild(i).GetComponent<UIOrnament>().popIconName;
                uIOrnament = content.GetChild(i).GetComponent<UIOrnament>();
                typeGameItem = content.GetChild(i).GetComponent<UIOrnament>().typeGameItem;
                shopType = content.GetChild(i).GetComponent<UIOrnament>().shopType;
                curStoreDefine = content.GetChild(i).GetComponent<UIOrnament>().curStoreDefine;
            }
        }
        Debug.Log(iconName);
        OnBuyBtnClick();
    }

    private void RightBtnClick()
    {

        _rightBuyTF.gameObject.SetActive(true);
        _leftBuyTF.gameObject.SetActive(false);
        isRightBtnOrnament = true;

        for (int i = 0; i < content.childCount; i++)
        {
            if (content.GetChild(i).GetComponent<UIOrnament>().isClick)
            {
                isPromotion = content.GetChild(i).GetComponent<UIOrnament>().isPromotion;
                todayBuyLimit = content.GetChild(i).GetComponent<UIOrnament>().todayBuyLimit;
                limitStr = content.GetChild(i).GetComponent<UIOrnament>().limitStr;
                iconName = content.GetChild(i).GetComponent<UIOrnament>().iconName;
                popIconName = content.GetChild(i).GetComponent<UIOrnament>().popIconName;
                uIOrnament = content.GetChild(i).GetComponent<UIOrnament>();
                typeGameItem = content.GetChild(i).GetComponent<UIOrnament>().typeGameItem;
                shopType = content.GetChild(i).GetComponent<UIOrnament>().shopType;
                curStoreDefine = content.GetChild(i).GetComponent<UIOrnament>().curStoreDefine;
            }
        }
        OnBuyBtnClick();
    }

    /// <summary>
    /// 点击购买
    /// </summary>
    public void OnBuyBtnClick()
    {
        //if (todayBuyLimit < 1)
        //{
        //    StaticData.CreateToastTips(limitStr);
        //    return;
        //}
        uiShopChoiceAmount.gameObject.SetActive(true);
        uiShopChoiceAmount.InitShow(curStoreDefine, isPromotion, todayBuyLimit, limitStr, iconName, shopType, uIOrnament, popIconName, typeGameItem, isRightBtnOrnament);
    }


    //向下按钮事件
    public async void DownClick()
    {
        //for (int i = 0; i < content.childCount; i++)
        //{
        //    if (content.GetChild(i).GetComponent<UIOrnament>().isClick)
        //    {
        //        content.GetChild(i).GetComponent<UIOrnament>().isClick = false;
        //        content.GetChild(i).Find("Background").gameObject.SetActive(false);


        //        content.GetChild(i + 1).GetComponent<UIOrnament>().isClick = true;
        //        content.GetChild(i + 1).Find("Background").gameObject.SetActive(true);
        //        //更新图片
        //      transform.Find("Icon").GetComponent<Image>().sprite = content.GetChild(i + 1).Find("Icon").GetComponent<Image>().sprite;
        //        //更新名字
        //        transform.Find("IconName").GetComponent<Text>().text = content.GetChild(i + 1).Find("ItemName").GetComponent<Text>().text;
        //        //更新价格
        //        transform.Find("BuyBtn_gold/CurPrice").GetComponent<Text>().text = content.GetChild(i + 1).GetComponent<UIOrnament>()._topPriceText;//金币价格
        //        transform.Find("BuyBtn_diamond/CurPrice").GetComponent<Text>().text = content.GetChild(i + 1).GetComponent<UIOrnament>()._topPriceText2;//钻石价格

        //        return;
        //    }
        //}

        

        for (int i = 0; i < cellNum; i++)
        {
            content.DOLocalMoveY(content.localPosition.y + cellY, 0.2f);
           // giftList.RefillCells();
            //await UniTask.Delay(200);
        }
        //contentY = contentY + cellNum * cellY;

        //content.localPosition = new Vector3(0, contentY, 0);
    }

    //旋转事件
    private async void Reversal()
    {
        if (_ReversalState == 0)
        {
            transform.Find("Icon").GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(newIconName);
            _ReversalState = 1;
        }
        else
        {
            transform.Find("Icon").GetComponent<Image>().sprite= await ABManager.GetAssetAsync<Sprite>(itemDefine.Icon);
            _ReversalState = 0;
        }
    }

    //是否有旋转
    public void IsHasReversal()
    {
        var ornamentConfig = StaticData.configExcel.GetDecorateByDecorateId(itemDefine.ID);

        if (ornamentConfig.DecorateIcon.Count == 0)
        {
            _RightReversalBtn.gameObject.SetActive(false);
            _LeftReversalBtn.gameObject.SetActive(false);
            return;
        }

        if (ornamentConfig.DecorateIcon.Count!=1)//有旋转
        {
            _RightReversalBtn.gameObject.SetActive(true);
            _LeftReversalBtn.gameObject.SetActive(true);
            newIconName = ornamentConfig.DecorateIcon[1];

            //初始化旋转状态
            _ReversalState = 0;
        }
        else
        {
            _RightReversalBtn.gameObject.SetActive(false);
            _LeftReversalBtn.gameObject.SetActive(false);
        }
    }
}
