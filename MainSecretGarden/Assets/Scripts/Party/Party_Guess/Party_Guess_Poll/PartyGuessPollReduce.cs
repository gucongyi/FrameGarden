using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyGuessPollReduce : MonoBehaviour
{
    private Text pollText;
    private float number;

    // Start is called before the first frame update
    void Start()
    {
        pollText = transform.Find("number").GetComponent<Text>();
    }

    public void Reduce()
    {
        number = int.Parse(pollText.text);

        if (number <= 0) 
        {
            Debug.Log("票数为零，不能再减");
            return;
        } 

        number--;

        pollText.text = number.ToString();
    }
}
