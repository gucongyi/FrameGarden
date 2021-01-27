using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManorUnLockRegionDialogComponent : MonoBehaviour
{
    public Button buttonMask;
    #region left
    public GameObject goLeft;
    public Text TextLeftName;
    public TextAnimationManager TextContentLeft;
    public Image imageLeftPerson;
    #endregion
    #region Right
    public GameObject goRight;
    public Text TextRightName;
    public TextAnimationManager TextContentRight;
    public Image imageRightPerson;
    #endregion
    public Button buttonSkip;
    
    
    
    ManorRegionComponent manorRegionComponent;
    int currDialogId;
    bool isCurrEnd;
    private void Awake()
    {
        buttonMask.onClick.RemoveAllListeners();
        buttonMask.onClick.AddListener(OnButtonDialogClick);
        buttonSkip.onClick.RemoveAllListeners();
        buttonSkip.onClick.AddListener(OnButtonSkipClick);
        TextContentLeft.onAnimationEnd.RemoveAllListeners();
        TextContentLeft.onAnimationEnd.AddListener(OnAnimEnd);
        TextContentRight.onAnimationEnd.RemoveAllListeners();
        TextContentRight.onAnimationEnd.AddListener(OnAnimEnd);
    }

    private void OnAnimEnd()
    {
        isCurrEnd = true;
    }

    private void OnButtonSkipClick()
    {
        EndDilog();
    }

    private async void EndDilog()
    {
        UIComponent.RemoveUI(UIType.UIManorUnLockRegionDialog);
        if (manorRegionComponent != null)
        {
            await manorRegionComponent.EndDialog();
        }
        else
        {
            LocalInfoData.localManorInfoData.isFirstAvgDialogFinish = true;
            JsonHelper.SaveDataToPersist<LocalManorInfoData>(LocalInfoData.localManorInfoData);
        }
        //AVG动画结束 播放庄园动画
        await StaticData.PlayManorEnterAnim();
    }

    private void OnButtonDialogClick()
    {
        int pos = StaticData.configExcel.GetManorRegionWordsByDialogId(currDialogId).Pos;
        
        if (!isCurrEnd)//这句话没结束加速
        {
            if (pos == 1)
            {
                TextContentLeft.Speed6Play();
            }
            else if (pos == 2)
            {
                TextContentRight.Speed6Play();
            }
            
        }
        else//这句话结束了，播放下一句
        {
            TextContentLeft.ResetSpeedPlay();
            TextContentRight.ResetSpeedPlay();
            var wordDefine = StaticData.configExcel.GetManorRegionWordsByDialogId(currDialogId);
            if (wordDefine.isEnd)//对话段结束
            {
                EndDilog();
            }
            else//播放下一句
            {
                currDialogId = wordDefine.NextDialogId;
                PlayDialog();
            }
            
        }
        
    }
    void SetLeftGo(string content)
    {
        goLeft.SetActive(true);
        goRight.SetActive(false);
        var dataDefine=StaticData.configExcel.GetManorRegionWordsByDialogId(currDialogId);
        string NameText= StaticData.configExcel.GetChapterFunctionTextByID(dataDefine.Name).SimplifiedChinese;
        if (dataDefine.Name == 10000007)
        {
            NameText = StaticData.playerInfoData.userInfo.Name;
        }
        NameText = string.Format(NameText, StaticData.playerInfoData.userInfo.Name);
        TextLeftName.text = NameText;
        imageLeftPerson.sprite = ABManager.GetAsset<Sprite>(dataDefine.HalfPhoto);
        TextContentLeft.word = $"{content}";
        TextContentLeft.Play();
    }
    void SetRightGo(string content)
    {
        goLeft.SetActive(false);
        goRight.SetActive(true);
        var dataDefine = StaticData.configExcel.GetManorRegionWordsByDialogId(currDialogId);
        string NameText = StaticData.configExcel.GetChapterFunctionTextByID(dataDefine.Name).SimplifiedChinese;
        if (dataDefine.Name == 10000007)
        {
            NameText = StaticData.playerInfoData.userInfo.Name;
        }
        NameText = string.Format(NameText, StaticData.playerInfoData.userInfo.Name);
        TextRightName.text = NameText;
        imageRightPerson.sprite= ABManager.GetAsset<Sprite>(dataDefine.HalfPhoto); 
        TextContentRight.word = $"{content}";
        TextContentRight.Play();
    }

    internal void SetRegionComponent(ManorRegionComponent manorRegionComponent)
    {
        this.manorRegionComponent = manorRegionComponent;
        currDialogId = StaticData.configExcel.ManorRegionDialog.Find(x => x.regionId == manorRegionComponent.regionId).BeginDialogId;
        PlayDialog();
    }

    public void BeginFirstAVGDialog()
    {
        this.manorRegionComponent = null;
        currDialogId = 12001;
        PlayDialog();
    }

    private void PlayDialog()
    {
        isCurrEnd = false;
        int pos = StaticData.configExcel.GetManorRegionWordsByDialogId(currDialogId).Pos;
        int idLocalize = StaticData.configExcel.GetManorRegionWordsByDialogId(currDialogId).RegionWords;
        string content = StaticData.GetMultilingual(idLocalize);
        if (pos == 1)
        {
            SetLeftGo(content);
        }
        else if (pos == 2)
        {
            SetRightGo(content);
        }
    }
}
