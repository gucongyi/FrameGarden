using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoolItemFriendStealInfo : MonoBehaviour, InterfaceScrollCell
{
    private FriendStealInfo curFriendStealInfo = new FriendStealInfo();

    public Text TextNickname;
    public GameObject handTag;
    public Text TextLevelNum;
    public Button ButtonEnterManor;
    public Image imageIcon;
    private void OnEnterManor() 
    {
        UIManorComponent uiManorComponent = UIComponent.GetComponentHaveExist<UIManorComponent>(UIType.UIManor);
        StaticData.curFriendStealInfo = curFriendStealInfo;
        uiManorComponent.OnButtonEnterFriendManor(curFriendStealInfo.uid);
        if (StaticData.isOpenGuide && GuideCanvasComponent._instance != null && GuideCanvasComponent._instance.isCurrStepGuiding)
        {
            GuideCanvasComponent._instance.SetLittleStepFinish();
        }
    }
    public async void ScrollCellIndex(int idx)
    {
        ButtonEnterManor.onClick.RemoveAllListeners();
        ButtonEnterManor.onClick.AddListener(OnEnterManor);
        curFriendStealInfo = StaticData.playerInfoData.listFriendStealInfo[idx];
        //头像
        imageIcon.sprite=ChatTool.GetIcon(curFriendStealInfo.headIcon);
        TextNickname.text = curFriendStealInfo.nickname;
        TextLevelNum.text = curFriendStealInfo.level.ToString();
        handTag.gameObject.SetActive(curFriendStealInfo.isSteal);
    }
}
