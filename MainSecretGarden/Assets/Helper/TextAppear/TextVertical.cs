using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TextVertical : MonoBehaviour
{
    Text TextOrigin;
    [Header("一列的字符数量")]
    public int rowCount;
    int colunmnCount;
    string textContent;
    StringBuilder tempText = new StringBuilder();
    char[] originTextCharArray;
    public enum TypeVertical
    {
        LeftToRight,
        RightToLeft
    }
    public TypeVertical typeVertical;
    private void Awake()
    {
        TextOrigin = GetComponent<Text>();
    }
    // Start is called before the first frame update
    void Start()
    {
        SetTextVertical();
    }

    public void SetTextVertical()
    {
        textContent = TextOrigin.text;
        originTextCharArray=textContent.ToCharArray();
        int textLength = originTextCharArray.Length;
        if (rowCount <= 0)
        {
            return;
        }
        if (textLength<=0)
        {
            return;
        }
        colunmnCount = Mathf.CeilToInt((float)textLength / rowCount);
        char[,] afterTwoArray = new char[colunmnCount,rowCount];
        tempText.Clear();
        //收集数据
        for (int i = 0; i < colunmnCount; i++)
        {
            for (int j = 0; j < rowCount; j++)
            {
                int ret = i * rowCount + j;
                if (ret < textLength)
                {
                    afterTwoArray[i, j] = originTextCharArray[ret];
                }
            }
        }
        switch (typeVertical)
        {
            case TypeVertical.LeftToRight:
                //转下行遍历
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < colunmnCount; j++)
                    {
                        tempText.Append(afterTwoArray[j, i]);
                        if (j < colunmnCount - 1)
                        {
                            tempText.Append(" ");
                        }
                    }
                    tempText.Append("\n");
                }
                break;
            case TypeVertical.RightToLeft:
                //转下行遍历
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = colunmnCount-1; j >=0; j--)
                    {
                        tempText.Append(afterTwoArray[j, i]);
                        if (j!=0)
                        {
                            tempText.Append(" ");
                        }
                    }
                    tempText.Append("\n");
                }
                break;
        }
        tempText.Replace('\0', ' ');
        TextOrigin.text = tempText.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
