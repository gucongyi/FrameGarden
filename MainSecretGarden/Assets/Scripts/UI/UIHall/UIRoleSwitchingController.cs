using Company.Cfg;
using Cysharp.Threading.Tasks;
using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 角色切换界面
/// </summary>
public class UIRoleSwitchingController : MonoBehaviour
{

    #region 变量

    private static UIRoleSwitchingController _instance;

    private LoopVerticalScrollRect _loopVertical;
    private Transform _loopVerticalBG;
    private Button _butReturn;
    private Button _butTakePictures;

    private Button _butWear;

    private GameObject _top;
    private GameObject _bottom;
    private GameObject _middle;

    private Transform _transFirstType;
    private Button _butHairstyle;       //头发
    private Button _butJacket;          //上装
    private Button _butOvercoat;        //外套
    private Button _butUnderClothes;    //下装
    private Button _butDress;           //连衣裙
    private Button _butSocks;           //袜子
    private Button _butShoes;           //鞋子
    private Button _butOrnament;        //饰品

    private Transform _transSecondType;
    private Button _butScarf;           //围巾
    private Button _butNecklace;        //项链
    private Button _butEarrings;        //耳环
    private Button _butBracelet;        //手镯
    private Button _butHat;             //帽子
    private Button _butGlove;           //手套
    private Button _butHandbag;         //手包


    /// <summary>
    /// 当前角色使用的时装
    /// </summary>
    private List<int> _curUsedFashions = new List<int>();

    /// <summary>
    /// 当前冲突类型
    /// </summary>
    private List<CostumeType> _curConflictTypes = new List<CostumeType>();


    /// <summary>
    /// 清除时装选中效果
    /// </summary>
    private Action ClearFashionSelectedEffect;
    /// <summary>
    /// 清除时装冲突效果
    /// </summary>
    private Action ClearFashionConflictEffect;

    /// <summary>
    /// 默认显示类型
    /// </summary>
    private List<CostumeType> _defShowTypes = new List<CostumeType>();

    /// <summary>
    /// 角色时装表 全部的时装 当前类型全部的时装
    /// </summary>
    private Dictionary<int, List<CostumeDefine>> _roleFashionDic = new Dictionary<int, List<CostumeDefine>>();


    /// <summary>
    /// 是否为点击界面button
    /// </summary>
    private bool _isNotMouseButtonUp = false;

    private bool _isShowFirstType = false;

    private bool isInitComplete = false;
    #endregion

    #region 属性

    /// <summary>
    /// 角色切换界面
    /// </summary>
    public static UIRoleSwitchingController Instance { get { return _instance; } }
    #endregion


    #region 函数

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //点击按钮
        if (Input.GetMouseButtonUp(0) && isInitComplete) 
        {
            if (_isNotMouseButtonUp)
            {
                _isNotMouseButtonUp = false;
                return;
            }
            else 
            {
                if (!_isShowFirstType) 
                {
                    //进入第一类型
                    ShowFirstTypeHideOther();
                }
            }
            
        }
    }

    private void Init()
    {
        if (_loopVertical != null)
            return;

        _top = transform.Find("Top").gameObject;
        _bottom = transform.Find("Bottom").gameObject;
        _middle = transform.Find("Middle").gameObject;

        _loopVerticalBG = transform.Find("Middle/ListBG");
        _loopVertical = transform.Find("Middle/ListBG/Loop Vertical Scroll Rect").GetComponent<LoopVerticalScrollRect>();
        _butReturn = transform.Find("Top/But_Return").GetComponent<Button>();
        _butTakePictures = transform.Find("Top/But_TakePictures").GetComponent<Button>();

        _butWear = transform.Find("Bottom/But_Wear/").GetComponent<Button>();


        _butWear.onClick.RemoveAllListeners();
        _butWear.onClick.AddListener(OnClickSaveFashions);

        _butReturn.onClick.RemoveAllListeners();
        _butReturn.onClick.AddListener(OnClickReturn);

        _butTakePictures.onClick.RemoveAllListeners();
        _butTakePictures.onClick.AddListener(OnClickTakePictures);


        _transFirstType = transform.Find("Middle/FirstType");
        _butHairstyle = transform.Find("Middle/FirstType/HairstyleBG").GetComponent<Button>();
        _butJacket = transform.Find("Middle/FirstType/JacketBG").GetComponent<Button>();
        _butOvercoat = transform.Find("Middle/FirstType/OvercoatBG").GetComponent<Button>();
        _butUnderClothes = transform.Find("Middle/FirstType/UnderClothesBG").GetComponent<Button>();
        _butDress = transform.Find("Middle/FirstType/DressBG").GetComponent<Button>();
        _butSocks = transform.Find("Middle/FirstType/SocksBG").GetComponent<Button>();
        _butShoes = transform.Find("Middle/FirstType/ShoesBG").GetComponent<Button>();
        _butOrnament = transform.Find("Middle/FirstType/OrnamentBG").GetComponent<Button>();

        _butHairstyle.onClick.RemoveAllListeners();
        _butHairstyle.onClick.AddListener(OnClickTypeHairstyle);
        _butJacket.onClick.RemoveAllListeners();
        _butJacket.onClick.AddListener(OnClickTypeJacket);
        _butOvercoat.onClick.RemoveAllListeners();
        _butOvercoat.onClick.AddListener(OnClickTypeOvercoat);
        _butUnderClothes.onClick.RemoveAllListeners();
        _butUnderClothes.onClick.AddListener(OnClickTypeUnderClothes);
        _butDress.onClick.RemoveAllListeners();
        _butDress.onClick.AddListener(OnClickTypeDress);
        _butSocks.onClick.RemoveAllListeners();
        _butSocks.onClick.AddListener(OnClickTypeSocks);
        _butShoes.onClick.RemoveAllListeners();
        _butShoes.onClick.AddListener(OnClickTypeShoes);
        _butOrnament.onClick.RemoveAllListeners();
        _butOrnament.onClick.AddListener(OnClickTypeOrnament);

        _transSecondType = transform.Find("Middle/ListBG/SecondType");
        _butScarf = transform.Find("Middle/ListBG/SecondType/ScarfBG").GetComponent<Button>();
        _butNecklace = transform.Find("Middle/ListBG/SecondType/NecklaceBG").GetComponent<Button>();
        _butEarrings = transform.Find("Middle/ListBG/SecondType/EarringsBG").GetComponent<Button>();
        _butBracelet = transform.Find("Middle/ListBG/SecondType/BraceletBG").GetComponent<Button>();
        _butHat = transform.Find("Middle/ListBG/SecondType/HatBG").GetComponent<Button>();
        _butGlove = transform.Find("Middle/ListBG/SecondType/GloveBG").GetComponent<Button>();
        _butHandbag = transform.Find("Middle/ListBG/SecondType/HandbagBG").GetComponent<Button>();

        _butScarf.onClick.RemoveAllListeners();
        _butScarf.onClick.AddListener(OnClickTypeScarf);
        _butNecklace.onClick.RemoveAllListeners();
        _butNecklace.onClick.AddListener(OnClickTypeNecklace);
        _butEarrings.onClick.RemoveAllListeners();
        _butEarrings.onClick.AddListener(OnClickTypeEarrings);
        _butBracelet.onClick.RemoveAllListeners();
        _butBracelet.onClick.AddListener(OnClickTypeBracelet);
        _butHat.onClick.RemoveAllListeners();
        _butHat.onClick.AddListener(OnClickTypeHat);
        _butGlove.onClick.RemoveAllListeners();
        _butGlove.onClick.AddListener(OnClickTypeGlove);
        _butHandbag.onClick.RemoveAllListeners();
        _butHandbag.onClick.AddListener(OnClickTypeHandbag);
    }

    #region 界面操作

    private void OnClickTypeScarf()
    {
        _isNotMouseButtonUp = true;
        UpdateLoopRoleFashionShow(CostumeType.Scarf);
    }

    private void OnClickTypeNecklace()
    {
        _isNotMouseButtonUp = true;
        UpdateLoopRoleFashionShow(CostumeType.Necklace);
    }

    private void OnClickTypeEarrings()
    {
        _isNotMouseButtonUp = true;
        UpdateLoopRoleFashionShow(CostumeType.Earrings);
    }

    private void OnClickTypeBracelet()
    {
        _isNotMouseButtonUp = true;
        UpdateLoopRoleFashionShow(CostumeType.Bracelet);
    }

    private void OnClickTypeHat()
    {
        _isNotMouseButtonUp = true;

        List<CostumeType> types = new List<CostumeType>();
        types.Add(CostumeType.TopHat);
        types.Add(CostumeType.LeftHat);
        types.Add(CostumeType.RightHat);
        types.Add(CostumeType.LongHat);

        UpdateLoopRoleFashionShow(CostumeType.None, types);
    }

    private void OnClickTypeGlove()
    {
        _isNotMouseButtonUp = true;
        UpdateLoopRoleFashionShow(CostumeType.Glove);
    }

    private void OnClickTypeHandbag()
    {
        _isNotMouseButtonUp = true;
        UpdateLoopRoleFashionShow(CostumeType.Handbag);
    }


    /// <summary>
    /// 点击类型 头发
    /// </summary>
    private void OnClickTypeHairstyle() 
    {
        _isNotMouseButtonUp = true;
        OnClickFirstType(CostumeType.Hairstyle);
    }

    private void OnClickTypeJacket()
    {
        _isNotMouseButtonUp = true;
        OnClickFirstType(CostumeType.Jacket);
    }

    private void OnClickTypeOvercoat()
    {
        _isNotMouseButtonUp = true;
        OnClickFirstType(CostumeType.Overcoat);
    }

    private void OnClickTypeUnderClothes()
    {
        _isNotMouseButtonUp = true;
        OnClickFirstType(CostumeType.UnderClothes);
    }

    private void OnClickTypeDress()
    {
        _isNotMouseButtonUp = true;
        OnClickFirstType(CostumeType.Dress);
    }

    private void OnClickTypeSocks()
    {
        _isNotMouseButtonUp = true;
        OnClickFirstType(CostumeType.Socks);
    }

    private void OnClickTypeShoes()
    {
        _isNotMouseButtonUp = true;
        OnClickFirstType(CostumeType.Shoes);
    }

    /// <summary>
    /// 第一类型点击
    /// </summary>
    /// <param name="type"></param>
    private async void OnClickFirstType(CostumeType type) 
    {
        FirstTypeHideAnim();
        UpdateLoopRoleFashionShow(type);
        await UniTask.Delay(460);
        LoopVerticalBGShowAnim();
    }

    /// <summary>
    /// 点击类型 饰品
    /// </summary>
    private async void OnClickTypeOrnament()
    {
        _isNotMouseButtonUp = true;

        //影藏第一类型
        FirstTypeHideAnim();
        await UniTask.Delay(460);
        _transSecondType.gameObject.SetActive(true);
        //_transFirstType.gameObject.SetActive(false);
        //显示第二类型
        LoopVerticalBGShowAnim();
        UpdateLoopRoleFashionShow(CostumeType.Scarf);
    }

    /// <summary>
    /// 显示第一类型影藏其他的
    /// </summary>
    private async void ShowFirstTypeHideOther() 
    {
        LoopVerticalBGHideAnim();
        await UniTask.Delay(460);
        _transSecondType.gameObject.SetActive(false);
        FirstTypeShowAnim();
    }


    /// <summary>
    /// 更新循环列表显示类型
    /// </summary>
    /// <param name="type"></param>
    private void UpdateLoopRoleFashionShow(CostumeType type, List<CostumeType> showTyps = null) 
    {
        _defShowTypes.Clear();
        if (showTyps != null) 
        {
            _defShowTypes.AddRange(showTyps);
        }else if (type != CostumeType.None)
        {
            _defShowTypes.Add(type);
        }
        if (_defShowTypes.Count > 0)
        {
            UpdateRoleFashionShow();
        }
    }

    private Vector3 _firstTypeDefPos = Vector3.zero;
    private Vector3 _loopVerticalBGDefPos = Vector3.zero;

    private void InitShow() 
    {
        _transSecondType.gameObject.SetActive(false);
        //_transFirstType.gameObject.SetActive(true);

        
        var topDefPos = _top.transform.localPosition;
        var startPos = topDefPos;
        startPos.x -= 460f;
        _top.transform.localPosition = startPos;
        _top.transform.DOLocalMoveX(topDefPos.x, 0.6f);

        var bottomDefPos = _bottom.transform.localPosition;
        startPos = bottomDefPos;
        startPos.x -= 460f;
        _bottom.transform.localPosition = startPos;
        _bottom.transform.DOLocalMoveX(bottomDefPos.x, 0.6f);

        //_loopVerticalBG
        _firstTypeDefPos = _transFirstType.localPosition;
        startPos = _firstTypeDefPos;
        startPos.x += 460f;
        _transFirstType.localPosition = startPos;
        FirstTypeShowAnim();

        _loopVerticalBGDefPos = _loopVerticalBG.localPosition;
        startPos = _loopVerticalBGDefPos;
        startPos.x += 460f;
        _loopVerticalBG.localPosition = startPos;
    }

    private void FirstTypeShowAnim() 
    {
        _isShowFirstType = true;
        _transFirstType.DOLocalMoveX(_firstTypeDefPos.x, 0.6f);
    }

    private void FirstTypeHideAnim()
    {
        _isShowFirstType = false;
        _transFirstType.DOLocalMoveX(_firstTypeDefPos.x + 460f, 0.6f);
    }

    private void LoopVerticalBGShowAnim()
    {
        _loopVerticalBG.DOLocalMoveX(_loopVerticalBGDefPos.x, 0.6f);
    }

    private void LoopVerticalBGHideAnim()
    {
        _loopVerticalBG.DOLocalMoveX(_loopVerticalBGDefPos.x + 460f, 0.6f);
    }
    #endregion

    public async void InitValue() 
    {
        Init();
        InitShow();
        //初始化角色使用的时装
        _curUsedFashions.AddRange(HallRoleManager.Instance.CurSelectedRoleChoicesID);
        UpdateConflictTypes();
        //
        await UniTask.WaitForEndOfFrame();
        HallRoleManager.Instance.UpdateRoleToSwitching();
        isInitComplete = true;
    }

    /// <summary>
    /// 更新冲突类型
    /// </summary>
    private void UpdateConflictTypes() 
    {
        _curConflictTypes.Clear();
        CostumeDefine costumeDefine = null;
        for (int i = 0; i < _curUsedFashions.Count; i++)
        {
            costumeDefine = StaticData.configExcel.GetCostumeByCostumeId(_curUsedFashions[i]);
            for (int j = 0; j < costumeDefine.ConflictTypes.Count; j++)
            {
                if (!_curConflictTypes.Contains(costumeDefine.ConflictTypes[j])) 
                {
                    _curConflictTypes.Add(costumeDefine.ConflictTypes[j]);
                }
            }
            
        }
        
    }

    #region 界面更新

    /// <summary>
    /// 更新角色时装显示
    /// </summary>
    private void UpdateRoleFashionShow() 
    {
        _roleFashionDic.Clear();

        var fashions = HallRoleManager.Instance.GetCurRoleAllFashions();
        List<CostumeDefine> defTypeFashions = new List<CostumeDefine>();
        for (int i = 0; i < fashions.Count; i++)
        {
            if (_defShowTypes.Contains(fashions[i].Type))
            {
                defTypeFashions.Add(fashions[i]);
            }
        }
        _roleFashionDic.Add(HallRoleManager.Instance.CurSelectedRoleID, defTypeFashions);

        UpdateLoopLength(_roleFashionDic[HallRoleManager.Instance.CurSelectedRoleID].Count);
    }

    /// <summary>
    /// 更新循环列表长度且重新刷新
    /// </summary>
    /// <param name="length"></param>
    private void UpdateLoopLength(int length) 
    {
        _loopVertical.totalCount = length;
        _loopVertical.RefillCells();
    }

    #endregion

    #region 界面操作


    /// <summary>
    /// 点击退出
    /// </summary>
    private void OnClickReturn() 
    {
        _isNotMouseButtonUp = true;

        //角色时装
        HallRoleManager.Instance.CurRole.InitDressUp(HallRoleManager.Instance.CurSelectedRoleChoicesID);
        HallRoleManager.Instance.UpdateRoleToHall();

        StaticData.HideUIRoleSwitching();
    }

    /// <summary>
    /// 点击拍照
    /// </summary>
    private void OnClickTakePictures() 
    {
        _isNotMouseButtonUp = true;

        Debug.Log("OnClickTakePictures 点击拍照!");
        //
        StartCoroutine(TakePictures());
    }

    /// <summary>
    /// 拍照
    /// </summary>
    /// <returns></returns>
    private IEnumerator TakePictures() 
    {
        //隐藏界面
        //gameObject.SetActive(false);
        _top.SetActive(false);
        _bottom.SetActive(false);
        _middle.SetActive(false);
        //隐藏聊天相关
        ChatTool.CloseChat();
        yield return UniversalTool.StaticWaitForEndOfFrame;
        //截屏
        Texture2D texture2D = UniversalTool.CaptureScreen();
        Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0, 0));
        //
        _top.SetActive(true);
        _bottom.SetActive(true);
        _middle.SetActive(true);
        //打开聊天
        ShowChatMini();
        //打开分享界面
        StaticData.OpenUIShare(sprite);
    }

    /// <summary>
    /// 打开聊天小按钮
    /// </summary>
    private async void ShowChatMini() 
    {
        await UIComponent.CreateUIAsync(UIType.ChatMini);
    }

    /// <summary>
    /// 时装是否需要保存
    /// </summary>
    /// <returns></returns>
    private bool IsNeedSaveFashions() 
    {
        if (_curUsedFashions.Count != HallRoleManager.Instance.CurSelectedRoleChoicesID.Count) 
        {
            return true;
        }
        for (int i = 0; i < _curUsedFashions.Count; i++)
        {
            if (!HallRoleManager.Instance.CurSelectedRoleChoicesID.Contains(_curUsedFashions[i])) 
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    ///保存时装->穿戴
    /// </summary>
    private void OnClickSaveFashions() 
    {
        _isNotMouseButtonUp = true;

        if (!IsNeedSaveFashions())
        {
            //没有需要保存的时装
            return;
        }

        Debug.Log("向服务器发起穿戴请求!");
        HallRoleManager.Instance.NotifyServerUseRoleAndFashion(HallRoleManager.Instance.CurSelectedRoleID, _curUsedFashions, UseRoleAndFashionServerCallback);
    }

    /// <summary>
    /// 使用角色和服装->穿戴 服务器返回是否成功
    /// </summary>
    /// <param name="isSuccess"></param>
    private void UseRoleAndFashionServerCallback(bool isSuccess) 
    {
        Debug.Log("向服务器发起穿戴请求,服务器返回!");
        //服务器返回 成功
        if (isSuccess)
        {
            //更新本地数据
            StaticData.UpdateUsedRoleAndFashion(HallRoleManager.Instance.CurSelectedRoleID, _curUsedFashions);
            //
            HallRoleManager.Instance.UpdateCurSelectedRoleChoicesID();
        }
        else //失败
        {
            Debug.Log("UseRoleAndFashionServerCallback 穿戴失败！");
        }
    }
    #endregion

    #region 外部调用函数

    /// <summary>
    /// 获取时装信息
    /// </summary>
    /// <param name="index"></param>
    /// <param name="name"></param>
    /// <param name="isUnlock"></param>
    /// <param name="icon"></param>
    public async void GetFashionInfo(int index, Action<string, bool, Sprite, bool, bool> loadFunc, Action clearSelected, Action clearConflict) 
    {
        Debug.Log("获取时装信息 index = "+ index);

        string name = null;
        bool isUnlock = true;
        Sprite icon = null;
        bool isSelected = false;
        bool isConflict = false;

        if (_roleFashionDic.Count <= 0)
            InitValue();

        if (index >= _roleFashionDic[HallRoleManager.Instance.CurSelectedRoleID].Count) 
        {
            Debug.LogError("获取时装信息,数据下标越界！ index = " + index);
            return;
        }

        //国际化时装名称
        name = StaticData.GetMultilingual(_roleFashionDic[HallRoleManager.Instance.CurSelectedRoleID][index].MultilingualNameID);

        //isUnlock = HallRoleManager.Instance.GetRoleFashionIsUnlock(HallRoleManager.Instance.CurSelectedRoleID, _roleFashionDic[HallRoleManager.Instance.CurSelectedRoleID][index].CostumeId);
        string path = _roleFashionDic[HallRoleManager.Instance.CurSelectedRoleID][index].Icon;
        if (!string.IsNullOrEmpty(path))
            icon = await ABManager.GetAssetAsync<Sprite>(path);

        //是否已经选中
        if (_curUsedFashions.Contains(_roleFashionDic[HallRoleManager.Instance.CurSelectedRoleID][index].CostumeId)) 
        {
            isSelected = true;
        }

        //是否冲突
        if (_curConflictTypes.Contains(_roleFashionDic[HallRoleManager.Instance.CurSelectedRoleID][index].Type)) 
        {
            isConflict = true;
        }

        ClearFashionSelectedEffect -= clearSelected;
        ClearFashionSelectedEffect += clearSelected;

        ClearFashionConflictEffect -= clearConflict;
        ClearFashionConflictEffect += clearConflict;

        //
        loadFunc?.Invoke(name, isUnlock, icon, isSelected, isConflict);
    }

    /// <summary>
    /// 获取使用的冲突的时装和同类型的时装
    /// </summary>
    /// <param name="conflictType"></param>
    /// <returns></returns>
    private void GetUsedConflictFashion(CostumeType sameType, List<CostumeType> conflictTypes, ref List<int> conflictTypeIDs) 
    {
        CostumeDefine costumeDefine = null;
        for (int i = 0; i < _curUsedFashions.Count; i++)
        {
            costumeDefine = StaticData.configExcel.GetCostumeByCostumeId(_curUsedFashions[i]);
            if (costumeDefine.Type == sameType) 
            {
                conflictTypeIDs.Add(_curUsedFashions[i]);
                continue;
            }
            if (conflictTypes.Contains(costumeDefine.Type)) 
            {
                conflictTypeIDs.Add(_curUsedFashions[i]);
                continue;
            }
        }
    }

    /// <summary>
    /// 选中时装
    /// </summary>
    /// <param name="index"></param>
    /// <param name="isSelected 是否选中"></param>
    public void SelectedFashion(int index, ref bool isSelected) 
    {

        _isNotMouseButtonUp = true;
        if (index >= _roleFashionDic[HallRoleManager.Instance.CurSelectedRoleID].Count)
            return;

        //选中的角色时装id 
        int slectedFashionID = _roleFashionDic[HallRoleManager.Instance.CurSelectedRoleID][index].CostumeId;

        //获取角色时装数据
        var fashionInfo = _roleFashionDic[HallRoleManager.Instance.CurSelectedRoleID][index];

        //是否为已经穿戴的时装
        if (_curUsedFashions.Contains(slectedFashionID)) 
        {
            //发型不可以取下
            if (fashionInfo.Type == CostumeType.Hairstyle)
            {
                isSelected = true;
                return;
            }
            else 
            {
                //取消选择时装
                isSelected = false;
                //脱下时装
                TakeOffTheFashion(fashionInfo);
                return;
            }
        }

        //穿戴时装
        WearFashion(fashionInfo);
    }

    /// <summary>
    /// 脱下时装
    /// </summary>
    private void TakeOffTheFashion(CostumeDefine fashionInfo) 
    {
        //
        if (_curUsedFashions.Contains(fashionInfo.CostumeId))
        {
            //换装
            HallRoleManager.Instance.CurRole.NotifyDressUp(fashionInfo, true);
            //
            _curUsedFashions.Remove(fashionInfo.CostumeId);
            //更新冲突类型
            UpdateConflictTypes();
        }
    }

    /// <summary>
    /// 穿戴时装
    /// </summary>
    /// <param name="fashionInfo"></param>
    private void WearFashion(CostumeDefine fashionInfo) 
    {
        //
        ClearFashionSelectedEffect?.Invoke();
        ClearFashionConflictEffect?.Invoke();

        //获取需要移除的时装
        //冲突的时装id 和 相同类型的时装id
        List<int> conflictTypeIDs = new List<int>();

        //1.获取冲突类型 获取冲突时装id
        GetUsedConflictFashion(fashionInfo.Type, fashionInfo.ConflictTypes, ref conflictTypeIDs);
        CostumeDefine costumeDefine = null;
        //2.移除冲突时装id 和 相同类型的时装id
        for (int i = 0; i < conflictTypeIDs.Count; i++)
        {
            if (_curUsedFashions.Contains(conflictTypeIDs[i]))
            {
                //
                costumeDefine = StaticData.configExcel.GetCostumeByCostumeId(conflictTypeIDs[i]);
                //脱下时装
                TakeOffTheFashion(costumeDefine);
            }
        }

        //换装
        HallRoleManager.Instance.CurRole.NotifyDressUp(fashionInfo);

        //更新当前角色使用时装数据
        //添加新的时装id
        if (!_curUsedFashions.Contains(fashionInfo.CostumeId))
        {
            _curUsedFashions.Add(fashionInfo.CostumeId);
        }
        //更新冲突类型
        UpdateConflictTypes();
    }

    #endregion

    #endregion
}
