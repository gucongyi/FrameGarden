using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabTextControl : MonoBehaviour
{

    public static TabTextControl _instance;

    public Transform TabSeed;
    public Transform TabItem;
    public Transform TabOrnament;
    public Transform seedRed;
    public Transform itemRed;

    private Text tabLaterSeedText;
    private Text tabLaterItemText;
    private Text tabLaterOrnamentText;

    private void Awake()
    {
        tabLaterSeedText = TabSeed.Find("Checkmark/Text").GetComponent<Text>();
        tabLaterItemText = TabItem.Find("Checkmark/Text").GetComponent<Text>();
        tabLaterOrnamentText = TabOrnament.Find("Checkmark/Text").GetComponent<Text>();

        _instance = this;
    }

     

    public void ChangSeedText(bool isbool)
    {
        if (tabLaterSeedText == null)
            return;

        tabLaterSeedText.gameObject.SetActive(true);
        seedRed.gameObject.SetActive(false);
        tabLaterItemText.gameObject.SetActive(false);
        tabLaterOrnamentText.gameObject.SetActive(false);
    }
    
    public void ChangeItemText(bool isbool)
    {
        if (tabLaterItemText == null)
            return;

        tabLaterSeedText.gameObject.SetActive(false);
        tabLaterItemText.gameObject.SetActive(true);
        itemRed.gameObject.SetActive(false);
        tabLaterOrnamentText.gameObject.SetActive(false);
    }

    public void ChangeOrnamentText(bool isbool)
    {
        if (tabLaterOrnamentText == null)
            return;

        tabLaterSeedText.gameObject.SetActive(false);
        tabLaterItemText.gameObject.SetActive(false);
        tabLaterOrnamentText.gameObject.SetActive(true);
    }
}
