using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCItem : MonoBehaviour
{
    public int NpcID;
    string headImage;
    public Text cardiacValue;
    public Text npc2PlayerValue;
    public Text player2NpcValue;
    public Text Dialogue;
    public Button dateBtn;//进入NPC界面按钮
    public Text btnText;

    public bool IsUnlock;//是否解锁

    private void Start()
    {
        btnText.text = "亲密互动";
    }

    public void Set(int NpcID, string headImage, int npc2PlayerValue, int player2NpcValue, int cardiacValue, string dialogue, bool isUnlock)
    {
        this.NpcID = NpcID;
        this.headImage = headImage;
        this.npc2PlayerValue.text = $"{npc2PlayerValue.ToString()}";
        this.player2NpcValue.text = $"{player2NpcValue.ToString()}";
        this.cardiacValue.text = cardiacValue.ToString();
        this.Dialogue.text = dialogue;
        this.IsUnlock = isUnlock;
        UpdateUnlock();
    }

    public async void UpdateUnlock()
    {
        if (IsUnlock)
        {
            transform.Find("Head/Unlock").GetComponent<Image>().enabled = false;
            dateBtn.interactable = true;
        }
        else
        {
            transform.Find("Head/Unlock").GetComponent<Image>().enabled = true;
            dateBtn.interactable = false;
        }
        transform.Find("Head/HeadImage").GetComponent<Image>().sprite = await ABManager.GetAssetAsync<Sprite>(this.headImage);
    }
}
