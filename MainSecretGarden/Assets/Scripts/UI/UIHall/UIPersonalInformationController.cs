using Game.Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static StaticData;

/// <summary>
/// 个人信息界面控制器
/// </summary>
public class UIPersonalInformationController : MonoBehaviour
{

    #region 变量

    private Transform _bgTra;

    /// <summary>
    /// 去评价
    /// </summary>
    private Button _butToEvaluate;

    /// <summary>
    /// 头像
    /// </summary>
    private Image _playerIcon;
    /// <summary>
    /// 昵称
    /// </summary>
    private Text _playerName;
    /// <summary>
    /// 等级
    /// </summary>
    private Text _playerLv;

    /// <summary>
    /// 当前等级经验 99/999
    /// </summary>
    private Text _playerCurLvExp;
    /// <summary>
    /// 当前等级经验进度
    /// </summary>
    private Image _playerCurLvExpSchedule;

    /// <summary>
    /// 角色id
    /// </summary>
    private Text _playerID;

    /// <summary>
    /// 音乐
    /// </summary>
    private Toggle _togMusic;
    private GameObject _musicCheckmark;
    /// <summary>
    /// 音效
    /// </summary>
    private Toggle _togSound;
    private GameObject _soundCheckmark;
    /// <summary>
    /// 复制id
    /// </summary>
    private Button _butIDCopy;


    /// <summary>
    /// 切换账号
    /// </summary>
    private Button _butSwitchAccount;

    /// <summary>
    /// 注销账号
    /// </summary>
    private Button _butLogoutAccount;

    /// <summary>
    /// 关闭界面
    /// </summary>
    private Button _butClose;
    #endregion

    #region 属性
    #endregion

    #region 函数/方法

    void Awake()
    {
        _bgTra = transform.Find("BG");
    }

    // Start is called before the first frame update
    void Start()
    {
        //Init();
        UniversalTool.ReadyPopupAnim(_bgTra);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        InitValue();
        UniversalTool.StartPopupAnim(_bgTra, new Vector3(0.85f, 0.85f, 1.0f)); 
    }

    private void Init()
    {
        if (_playerIcon != null)
            return;

        _butToEvaluate = transform.Find("BG/PlayerBG/ToEvaluate").GetComponent<Button>();

        _playerIcon = transform.Find("BG/PlayerIcon/IconMask/Icon").GetComponent<Image>();
        _playerName = transform.Find("BG/PlayerIcon/Name").GetComponent<Text>();
        _playerLv = transform.Find("BG/PlayerIcon/Level").GetComponent<Text>();

        _playerCurLvExp = transform.Find("BG/LVBG/Exp").GetComponent<Text>();
        _playerCurLvExpSchedule = transform.Find("BG/LVBG/Image").GetComponent<Image>();

        _playerID = transform.Find("BG/ID/IDNum").GetComponent<Text>();

        _togMusic = transform.Find("BG/MusicBG/Toggle").GetComponent<Toggle>();
        _musicCheckmark = _togMusic.transform.Find("Background/Checkmark").gameObject;
        _togSound = transform.Find("BG/SoundEffect/Toggle").GetComponent<Toggle>();
        _soundCheckmark = _togSound.transform.Find("Background/Checkmark").gameObject;

        _butIDCopy = transform.Find("BG/ID/But_Copy").GetComponent<Button>();
        _butSwitchAccount = transform.Find("BG/SwitchAccount").GetComponent<Button>();
        _butLogoutAccount = transform.Find("BG/LogoutAccount").GetComponent<Button>();

        _butClose = transform.Find("BGBlack").GetComponent<Button>();


        _togMusic.onValueChanged.RemoveAllListeners();
        _togMusic.onValueChanged.AddListener(OnValueChangedMusic);

        _togSound.onValueChanged.RemoveAllListeners();
        _togSound.onValueChanged.AddListener(OnValueChangedSound);

        _butIDCopy.onClick.RemoveAllListeners();
        _butIDCopy.onClick.AddListener(OnClickCopyID);

        _butToEvaluate.onClick.RemoveAllListeners();
        _butToEvaluate.onClick.AddListener(OnClickToEvaluate);

        _butSwitchAccount.onClick.RemoveAllListeners();
        _butSwitchAccount.onClick.AddListener(OnClickSwitchAccount);

        _butLogoutAccount.onClick.RemoveAllListeners();
        _butLogoutAccount.onClick.AddListener(OnClickLogoutAccount);

        _butClose.onClick.RemoveAllListeners();
        _butClose.onClick.AddListener(OnClickCloseUI); ;

    }

    /// <summary>
    /// 值初始化
    /// </summary>
    private async void InitValue()
    {
        Init();
        string path = StaticData.configExcel.GetPlayerAvatarByID(StaticData.playerInfoData.userInfo.Image).Icon;
        _playerIcon.sprite = await ABManager.GetAssetAsync<Sprite>(path);
        if (!string.IsNullOrEmpty(StaticData.playerInfoData.userInfo.Name))
        {
            _playerName.text = StaticData.playerInfoData.userInfo.Name;
        }
        else
        {
            _playerName.text = StaticData.Uid.ToString();
        }

        ShowLevelInfo();
        _playerID.text = LocalizationDefineHelper.GetStringNameById(120038) + StaticData.Uid;

        //isOn = true 代表静音
        _togMusic.isOn = GameSoundPlayer.Instance.MusicMuted;
        _musicCheckmark.SetActive(_togMusic.isOn);
        _togSound.isOn = GameSoundPlayer.Instance.SoundEffectMuted;
        _soundCheckmark.SetActive(_togSound.isOn);
    }

    public void ShowLevelInfo()
    {
        PlayerLevelAndCurrExp level = StaticData.GetPlayerLevelAndCurrExp();
        if (level.currLevelNeed < 0)
            level.currLevelNeed = 1;
        if (level.currLevelHaveExp < 0)
            level.currLevelHaveExp = 0;
        _playerLv.text = /*LocalizationDefineHelper.GetStringNameById(120035) + */level.level.ToString();
        _playerCurLvExp.text = level.currLevelHaveExp + "/" + level.currLevelNeed;
        _playerCurLvExpSchedule.fillAmount = (float)level.currLevelHaveExp / (float)level.currLevelNeed;
    }


    /// <summary>
    /// 点击是否开启音乐 isOn = true 关
    /// </summary>
    /// <param name="isTrue"></param>
    private void OnValueChangedMusic(bool isTrue)
    {
        GameSoundPlayer.Instance.MuteMusic(_togMusic.isOn);
        _musicCheckmark.SetActive(_togMusic.isOn);
    }

    /// <summary>
    /// 点击是否开启音效 isTrue true 表示静音
    /// </summary>
    /// <param name="isTrue"></param>
    private void OnValueChangedSound(bool isTrue)
    {

        GameSoundPlayer.Instance.MuteSoundEffect(_togSound.isOn);
        _soundCheckmark.SetActive(_togSound.isOn);
    }

    /// <summary>
    /// 复制角色id
    /// </summary>
    private void OnClickCopyID() 
    {
        GUIUtility.systemCopyBuffer = StaticData.Uid.ToString(); //将文字复制到剪贴板API

        //已经复制到粘贴板
        string desc = LocalizationDefineHelper.GetStringNameById(120264);
        StaticData.CreateToastTips(desc);
    }

    /// <summary>
    /// 去评价
    /// </summary>
    private void OnClickToEvaluate() 
    {

        Debug.Log("去评价");
        Sdk.SdkFuc.GetEvaluationPath((path)=> { if (!string.IsNullOrEmpty(path)) 
            {
                Application.OpenURL(path);
            } } );
        //OnClickCloseUI();
    }

    /// <summary>
    /// 切换账号
    /// </summary>
    private void OnClickSwitchAccount()
    {

        Debug.Log("切换账号提示");
        string desc = LocalizationDefineHelper.GetStringNameById(120257);//"确定要切换账号吗？";

        StaticData.OpenCommonTips(desc, 120040, SwitchAccount);
    }

    private void SwitchAccount() 
    {
        if (WebSocketComponent._instance != null)
            WebSocketComponent._instance.QuitUILogin();
        OnClickCloseUI();
    }

    /// <summary>
    /// 注销账号
    /// </summary>
    private void OnClickLogoutAccount()
    {

        Debug.Log("注销账号提示");
        string desc = LocalizationDefineHelper.GetStringNameById(120258);//"永久注销，账号一旦注销，账号相关数据无法找回，是否注销账号。";

        StaticData.OpenCommonTips(desc, 120256, LogoutAccount);
    }

    private void LogoutAccount() 
    {
        StaticData.NotifyServerLogoutAccount(LogoutAccountCallback);
    }

    private void LogoutAccountCallback(bool isSuccess)
    {
        if (isSuccess && WebSocketComponent._instance != null)
        {
            WebSocketComponent._instance.QuitUILogin();
            OnClickCloseUI();
        }
    }


    /// <summary>
    /// 关闭界面
    /// </summary>
    private void OnClickCloseUI() 
    {

        UniversalTool.CancelPopAnim(_bgTra, CloseUI, new Vector3(0.85f, 0.85f, 1.0f));
        //UIComponent.HideUI(UIType.UIPersonalInformation);
    }

    private void CloseUI() 
    {
        UIComponent.HideUI(UIType.UIPersonalInformation);
    }

    #endregion


}
