using Company.Cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 购物玩法控制器
/// </summary>
public class ShoppingController : MonoBehaviour
{
    #region 字段
    /// <summary>
    /// 所有商品
    /// </summary>
    [SerializeField]
    List<ShoppingItem> _shoppingItemAlls = new List<ShoppingItem>();
    /// <summary>
    /// 已经购买的物品
    /// </summary>
    List<ShoppingItem> _haveItems = new List<ShoppingItem>();
    /// <summary>
    /// 任务目标清单
    /// </summary>
    [SerializeField]
    List<int> _inventorys = new List<int>();
    /// <summary>
    /// 人物初始需要说的话
    /// </summary>
    [SerializeField]
    int _initialDialogueId;
    /// <summary>
    /// 人物初始需要说的话(关闭清单后)
    /// </summary>
    [SerializeField]
    int _initialDialogueIdTwo;
    /// <summary>
    /// 购物结束id
    /// </summary>
    [SerializeField]
    int _endDialogueId;
    /// <summary>
    /// 错误提示
    /// </summary>
    [SerializeField]
    List<int> _mistakeTipId = new List<int>();
    /// <summary>
    /// 正确提示
    /// </summary>
    [SerializeField]
    List<int> _correctTipId = new List<int>();
    /// <summary>
    /// 清单item字典
    /// </summary>
    Dictionary<int, InventoryItem> _inventoryItemDic = new Dictionary<int, InventoryItem>();
    /// <summary>
    /// 是否说完初始对话
    /// </summary>
    bool _isShowInitialDialogueId = false;
    /// <summary>
    /// 商品盒子
    /// </summary>
    Transform _merchandiseBoxTra;
    /// <summary>
    /// 商品拖拽目标点
    /// </summary>
    Transform _tagePointTra;
    /// <summary>
    /// 清单
    /// </summary>
    RectTransform _inventoryBoxRect;
    /// <summary>
    /// 清单盒子按钮
    /// </summary>
    Button _inventoryBoxBtn;
    /// <summary>
    /// 大清单
    /// </summary>
    RectTransform _inventoryRect;
    /// <summary>
    /// 小清单按钮
    /// </summary>
    Button _inventoryMinBtn;
    /// <summary>
    /// 对话框
    /// </summary>
    DialogueBoxCartoonCpmponent _dialogueBox;
    /// <summary>
    /// 多人对话
    /// </summary>
    DialogueBoxBubbleComponent _dialogue;
    /// <summary>
    /// 清单滑动列表
    /// </summary>
    ScrollRect _inventoryScrollRect;
    /// <summary>
    /// 清单item母体
    /// </summary>
    RectTransform _inventoryItem;
    /// <summary>
    /// 购物结束回调
    /// </summary>
    Action _endAction;
    RectTransform _maskBoxRect;
    Button _maskBtn;
    Text _maskBtnText;

    CanvasGroup _thisCanvasGroup;
    bool _isBeingSpeak = false;
    #endregion
    #region 函数
    // Start is called before the first frame update
    void Start()
    {

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
        _merchandiseBoxTra = transform.Find("MerchandiseBox");
        _thisCanvasGroup = GetComponent<CanvasGroup>();
        _tagePointTra = _merchandiseBoxTra.Find("TagePoint");
        _dialogueBox = transform.Find("DialogueBox_Cartoon").GetComponent<DialogueBoxCartoonCpmponent>();
        _inventoryBoxRect = transform.Find("InventoryBox").GetComponent<RectTransform>();
        _inventoryBoxBtn = _inventoryBoxRect.Find("Mask").GetComponent<Button>();
        _inventoryRect = transform.Find("InventoryBox/Inventory").GetComponent<RectTransform>();
        _inventoryScrollRect = _inventoryRect.Find("Scroll View").GetComponent<ScrollRect>();
        _inventoryMinBtn = transform.Find("InventoryMinBtn").GetComponent<Button>();
        _inventoryItem = transform.Find("InventoryItem").GetComponent<RectTransform>();
        _inventoryMinBtn.onClick.RemoveAllListeners();
        _inventoryMinBtn.onClick.AddListener(ClickInventoryMinBtn);
        _inventoryBoxBtn.onClick.RemoveAllListeners();
        _inventoryBoxBtn.onClick.AddListener(ClickInventoryBox);
        _dialogue = transform.Find("DialogueBox_Bubble").GetComponent<DialogueBoxBubbleComponent>();
        _maskBoxRect = transform.Find("Mask").GetComponent<RectTransform>();
        _maskBtn = _maskBoxRect.Find("MaskBtn").GetComponent<Button>();
        _maskBtnText = _maskBtn.transform.Find("Text").GetComponent<Text>();
        _maskBtn.onClick.RemoveAllListeners();
        _maskBtn.onClick.AddListener(Close);
        _maskBtnText.text = ChapterTool.GetChapterFunctionString(10000523);
        _endAction = endAction;
        //_maskBoxRect.gameObject.SetActive(false);
        if (_initialDialogueId != 0)
        {
            _dialogue.SetStartDialogueId(_initialDialogueId);
            _dialogue.Initial(() =>
            {
                _dialogue.Close();
                OpenInventory(true);
            });
            _dialogue.Show();
            //_dialogue.gameObject.SetActive(true);
            //_dialogue.OpenClickBtn(true);

        }
        _haveItems.Clear();
        OpenInventory(false);
        InitialShoppingItemAll();
        InitialInventory();
    }

    private async void Close()
    {
        await ChapterTool.FadeInFadeOut(_thisCanvasGroup, 0, 0.1f, null, () =>
         {
             gameObject.SetActive(false);
             _endAction?.Invoke();
         });
    }


    /// <summary>
    /// 初始化所有商品
    /// </summary>
    public void InitialShoppingItemAll()
    {
        for (int i = 0; i < _shoppingItemAlls.Count; i++)
        {
            _shoppingItemAlls[i].Initial(_tagePointTra.localPosition, PointerUpItem, PointerDownItem, ClickItem);
        }
    }
    /// <summary>
    /// 初始化清单
    /// </summary>
    public void InitialInventory()
    {
        foreach (var item in _inventoryItemDic)
        {
            item.Value.Dispose();
        }
        _inventoryItemDic.Clear();
        for (int i = 0; i < _inventorys.Count; i++)
        {
            ShoppingItem data = _shoppingItemAlls[_inventorys[i]];
            string showStr = "     " + (i + 1) + "." + ChapterTool.GetChapterFunctionString(data._CommodityName);
            InventoryItem item = CreationInventoryItem(showStr);
            if (!_inventoryItemDic.ContainsKey(_inventorys[i]))
            {
                _inventoryItemDic.Add(_inventorys[i], item);
            }
        }
    }
    /// <summary>
    /// 创建清单item
    /// </summary>
    /// <param name="showName"></param>
    /// <returns></returns>
    public InventoryItem CreationInventoryItem(string showName)
    {
        GameObject obj = GameObject.Instantiate(_inventoryItem.gameObject, _inventoryScrollRect.content);
        InventoryItem item = new InventoryItem();
        obj.gameObject.SetActive(true);
        item.Initial(obj.GetComponent<RectTransform>(), showName);
        return item;
    }
    /// <summary>
    /// 获取item下标
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int GetItemIdex(ShoppingItem item)
    {
        int index = -1;
        if (_shoppingItemAlls.Contains(item))
        {
            index = _shoppingItemAlls.IndexOf(item);
        }
        return index;
    }
    /// <summary>
    /// 判断是否是正确商品
    /// </summary>
    /// <param name="index"></param>
    /// <param name="correctIndex"></param>
    /// <returns></returns>
    public bool IsCorrectCommodity(int index, out int correctIndex)
    {
        bool isCorrect = false;
        if (_inventorys.Contains(index))
        {
            correctIndex = _inventorys.IndexOf(index);
            isCorrect = true;
        }
        else
        {
            correctIndex = -1;
        }
        return isCorrect;
    }
    /// <summary>
    /// 点击某个商品
    /// </summary>
    /// <param name="item"></param>
    public void ClickItem(ShoppingItem item)
    {

    }
    /// <summary>
    /// 某个商品按下鼠标
    /// </summary>
    /// <param name="item"></param>
    public void PointerDownItem(ShoppingItem item)
    {

    }
    /// <summary>
    /// 某个商品放开鼠标
    /// </summary>
    /// <param name="item"></param>
    public void PointerUpItem(ShoppingItem item)
    {
        Debug.Log(item._CommodityName + "到达目标");
        int index = 0;
        if (IsCorrectCommodity(GetItemIdex(item), out index))
        {
            if (_inventoryItemDic.ContainsKey(_inventorys[index]))
            {
                _inventoryItemDic[_inventorys[index]].OpenFinishTipe(true);
                _inventorys.RemoveAt(index);
                //item.OpenItem(false);
                item._ThisRect.localPosition = new Vector3(item._ThisRect.localPosition.x, _tagePointTra.localPosition.y);
                item._ThisRect.localScale = new Vector3(0.5f, 0.5f, 0.5f);

                item._UIPanelDrag.localPos = item._ThisRect.localPosition;
                _haveItems.Add(item);
                if (_inventorys == null || _inventorys.Count <= 0)
                {
                    Debug.Log("买完东西");
                    if (_endDialogueId != 0)
                    {
                        _dialogue.SetStartDialogueId(_endDialogueId);
                        _dialogue.Initial(() =>
                        {
                            _dialogue.Close();
                            _maskBoxRect.gameObject.SetActive(true);
                        });
                        _dialogue.Show();
                        //_dialogue.gameObject.SetActive(true);
                        //_dialogue.OpenClickBtn(true);
                    }
                    else
                    {
                        _maskBoxRect.gameObject.SetActive(true);
                    }
                    for (int i = 0; i < _haveItems.Count; i++)
                    {
                        _haveItems[i].OpenItem(true);
                    }
                }
            }
            else
            {
                Debug.Log("清单没有配置此商品");
            }
            Debug.Log("正确商品");
            int correctIndex = UnityEngine.Random.Range(0, _correctTipId.Count);
            ShowDialogue(_correctTipId[correctIndex], Spak);
        }
        else
        {
            Debug.Log("错误商品");
            int correctIndex = UnityEngine.Random.Range(0, _mistakeTipId.Count);
            ShowDialogue(_mistakeTipId[correctIndex], Spak);
        }
        _isBeingSpeak = true;
    }
    /// <summary>
    /// 展示对话
    /// </summary>
    /// <param name="dialogueId"></param>
    public void ShowDialogue(int dialogueId, Action<ChapterFunctionTextDefine> endAction)
    {
        if (!_dialogueBox._IsInitial)
        {
            _dialogueBox.SetIsFunction(true);
            _dialogueBox.SetIsDefaultShow(false);
            _dialogueBox.Initial(null);
        }
        _dialogueBox.ResetFunctionData(dialogueId, endAction);
    }
    /// <summary>
    /// 点击清单按钮
    /// </summary>
    private void ClickInventoryMinBtn()
    {
        OpenInventory(true);
    }
    /// <summary>
    /// 点击清单遮罩
    /// </summary>
    private void ClickInventoryBox()
    {
        if (!_isShowInitialDialogueId && _initialDialogueIdTwo != 0)
        {

            _dialogue.SetStartDialogueId(_initialDialogueIdTwo);
            _dialogue.Initial(() =>
            {
                _dialogue.Close();
                _isShowInitialDialogueId = true;
            });
            _dialogue.Show();
            //_dialogue.gameObject.SetActive(true);
            //_dialogue.OpenClickBtn(true);
        }
        OpenInventory(false);
    }
    /// <summary>
    /// 开关清单
    /// </summary>
    /// <param name="isOpen"></param>
    public void OpenInventory(bool isOpen)
    {
        if (isOpen)
        {
            _inventoryBoxRect.gameObject.SetActive(true);
            _inventoryMinBtn.gameObject.SetActive(false);
        }
        else
        {
            _inventoryBoxRect.gameObject.SetActive(false);
            _inventoryMinBtn.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// 说话后回调
    /// </summary>
    /// <param name="data"></param>
    public void Spak(ChapterFunctionTextDefine data)
    {
        _isBeingSpeak = false;
        Invoke("CloseDialogue", 3);
    }
    /// <summary>
    /// 关闭对话框
    /// </summary>
    public void CloseDialogue()
    {
        if (_isBeingSpeak)
        {
            return;
        }
        _dialogueBox.Close();
    }
    #endregion
}
