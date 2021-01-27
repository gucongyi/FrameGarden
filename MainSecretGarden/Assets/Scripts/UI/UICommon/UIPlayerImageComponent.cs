using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerImageComponent : MonoBehaviour
{
    public Image ImageIcon;
    public Text TextLevel;
    public Button buttonPlayerImage;
    public Text TextName;
    #region Exp
    public Image imageExpSlider;
    public Text textExp;
    public Image imageExp;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        buttonPlayerImage.onClick.RemoveAllListeners();
        buttonPlayerImage.onClick.AddListener(OnButtonPlayerImageClick);
        TextName.text = StaticData.playerInfoData.userInfo.Name;
        if (Root2dSceneManager._instance != null && Root2dSceneManager._instance.isFriendManor)
        {
            buttonPlayerImage.onClick.RemoveAllListeners();
            TextName.text = StaticData.curFriendStealInfo.nickname;
        }
        LoadPlayerIcon();
    }

    public void SetInitInfo()
    {
        Start();
    }
    /// <summary>
    /// 加载玩家头像
    /// </summary>
    private async void LoadPlayerIcon()
    {
        var roleID = StaticData.playerInfoData.userInfo.Image;
        var path = StaticData.configExcel.GetPlayerAvatarByID(roleID);
        ImageIcon.sprite = await ABManager.GetAssetAsync<Sprite>(path.Icon);
        if (Root2dSceneManager._instance != null && Root2dSceneManager._instance.isFriendManor)
        {
            if (string.IsNullOrEmpty(StaticData.playerInfoData.userInfo.ImageAddress))
            {
                ImageIcon.sprite = ChatTool.GetIcon(StaticData.curFriendStealInfo.headIcon);
            }
            else
            {
                ImageIcon.sprite = await ChatTool.GetIcon(StaticData.playerInfoData.userInfo.ImageAddress);
            }
        }
    }

    private void OnButtonPlayerImageClick()
    {
        //todo 
        UIComponent.CreateUI(UIType.UIPersonalInformation);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLevel();
        UpdateCurrExp();
    }

    private void UpdateCurrExp()
    {
        StaticData.PlayerLevelAndCurrExp currPlayerExpInfo = StaticData.GetPlayerLevelAndCurrExp();
        float currExpRadio = (float)currPlayerExpInfo.currLevelHaveExp / currPlayerExpInfo.currLevelNeed;
        imageExpSlider.fillAmount = currExpRadio;
        textExp.text = $"{currPlayerExpInfo.currLevelHaveExp}/{currPlayerExpInfo.currLevelNeed}";

        if (Root2dSceneManager._instance != null && Root2dSceneManager._instance.isFriendManor)
        {
            StaticData.PlayerLevelAndCurrExp currcurFriendStealInfoExpInfo = StaticData.curFriendStealInfo.playerLevelAndCurrExp;
            currExpRadio = (float)currcurFriendStealInfoExpInfo.currLevelHaveExp / currcurFriendStealInfoExpInfo.currLevelNeed;
            imageExpSlider.fillAmount = currExpRadio;
            textExp.text = $"{currcurFriendStealInfoExpInfo.currLevelHaveExp}/{currcurFriendStealInfoExpInfo.currLevelNeed}";
        }

    }

    private void UpdateLevel()
    {
        TextLevel.text = $"{StaticData.GetPlayerLevelAndCurrExp().level}";
        if (Root2dSceneManager._instance != null && Root2dSceneManager._instance.isFriendManor)
        {
            TextLevel.text = $"{StaticData.curFriendStealInfo.playerLevelAndCurrExp.level}";
        }

    }
}
