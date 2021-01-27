using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 心动时刻主界面
/// </summary>
public class ImpulesMainView : MonoBehaviour
{
    public GameObject NpcHeadItem;//男主item
    List<NPCItem> NpcItemList = new List<NPCItem>();

    Button Back_btn;

    ImpulesNPCView impulesNPCView;
    float moveAnimaTime = 0.3f;
    float showValue = 0f;
    float closeValue = -1242f;

    public UIImpulseComponent UIImpulseComponent;
    public ScrollRect scrollRect;

    void FindComponent()
    {
        //mainView
        Back_btn = transform.Find("Top/Back_btn").GetComponent<Button>();
        Back_btn.onClick.AddListener(ExitImpules);
    }
    public void Init(ImpulesNPCView impulesNPCView)
    {
        this.impulesNPCView = impulesNPCView;
        FindComponent();
    }
    //打开界面 调用一次
    public void OpenView(List<Favorable> favorableList)
    {
        for (int i = 0; i < favorableList.Count; i++)
        {
            int index = i;
            GameObject NpcItem = GameObject.Instantiate(NpcHeadItem);
            var npcitem = NpcItem.GetComponent<NPCItem>();
            //setParent
            ChapterHelper.SetParent(npcitem.gameObject, scrollRect.content);

            favorableList = ImpulseHelper.ImpulseSort(favorableList);

            //set
            bool isUnlock = false;
            var roleInfo = StaticData.configExcel.GetHallRoleByID(favorableList[i].NPCId);
            if (StaticData.playerInfoData.userInfo.SectionId >= roleInfo.UnlockChapter)
                isUnlock = true;
            //根据男主id 随机一句话
            NPCDialogue nPCDialogue = UIImpulseComponent.npcId.Find((f) =>
            {
                return f.NPCID == favorableList[i].NPCId;
            });
            int ind = Random.Range(0, nPCDialogue.dialogue.Count);
            //for (int j = 0; j < UIImpulseComponent.npcId.Count; j++)
            //{
            //    if (UIImpulseComponent.npcId[j].NPCID == favorableList[i].NPCId)
            //    {
            //        Random.Range(0,)
            //    }
            //}
            npcitem.Set(favorableList[i].NPCId, favorableList[i].headPortrait, favorableList[i].NPC2PlayerValue, favorableList[i].Player2NPCValue, favorableList[i].cardiacValue, ImpulseHelper.GetNPCDialogue(nPCDialogue.dialogue[ind]), isUnlock);
            npcitem.dateBtn.onClick.AddListener(() => OnClick(favorableList[index]));
            NpcItemList.Add(npcitem);
        }
    }

    void OnClick(Favorable favorable)
    {
        //打开页面
        ViewSwitchAnima();
        impulesNPCView.OpenView(favorable);

        Debug.Log($"男主id:{favorable.NPCId},对玩家的好感：{favorable.NPC2PlayerValue},玩家对男主的好感：{favorable.Player2NPCValue}");
    }

    //页面切换动画
    void ViewSwitchAnima()
    {
        this.transform.DOLocalMoveX(closeValue, moveAnimaTime);
    }
    //显示界面
    public void ShowView(float moveAnimaTime = 0.3f)
    {
        Reshf(StaticData.playerInfoData.favorableData);//刷新页面
        this.transform.DOLocalMoveX(showValue, moveAnimaTime);
    }
    //刷新
    void Reshf(List<Favorable> favorableList)
    {
        //排序
        favorableList = ImpulseHelper.ImpulseSort(favorableList);
        for (int i = 0; i < favorableList.Count; i++)
        {//直接全部重新刷新
            int index = i;
            bool isUnlock = false;
            var roleInfo = StaticData.configExcel.GetHallRoleByID(favorableList[i].NPCId);
            if (StaticData.playerInfoData.userInfo.SectionId >= roleInfo.UnlockChapter)
                isUnlock = true;
            //根据男主id 随机一句话
            //根据男主id 随机一句话
            NPCDialogue nPCDialogue = UIImpulseComponent.npcId.Find((f) =>
            {
                return f.NPCID == favorableList[i].NPCId;
            });
            int ind = Random.Range(0, nPCDialogue.dialogue.Count);
            NpcItemList[i].Set(favorableList[i].NPCId, favorableList[i].headPortrait, favorableList[i].NPC2PlayerValue, favorableList[i].Player2NPCValue, favorableList[i].cardiacValue, ImpulseHelper.GetNPCDialogue(nPCDialogue.dialogue[ind]), isUnlock);
            NpcItemList[i].dateBtn.onClick.AddListener(() => OnClick(favorableList[index]));
        }

    }

    void ExitImpules()
    {
        UIComponent.RemoveUI(UIType.UIImpulse);
        //UIComponent.HideUI(UIType.UIImpulse);
    }


}
