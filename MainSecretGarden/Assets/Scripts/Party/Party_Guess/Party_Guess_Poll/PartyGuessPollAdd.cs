using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyGuessPollAdd : MonoBehaviour
{
    private Text pollText;
    private float number;

    void Start()
    {
        pollText=transform.Find("number").GetComponent<Text>();
    }

    public void Add()
    {
        if (number >=UIPartyGuessController.Instance._hasPolls) return;

        number = int.Parse(pollText.text);

        number++;

        pollText.text = number.ToString();
    }

}
