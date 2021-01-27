using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationCalcFontSize : MonoBehaviour
{
    public Text text;
    public int defaultSize;
    int lenStringText;
    float height;
    float width;
    // Start is called before the first frame update
    void Start()
    {
        RectTransform rectTrans = text.GetComponent<RectTransform>();
        width = rectTrans.sizeDelta.x;
        height = rectTrans.sizeDelta.y;
        lenStringText = text.text.Length;
        CalcFontSize(lenStringText, text);
        
    }

    void CalcFontSize(int totalCount,Text text)
    {
        int sizeFont = 0;
        //w=font*col
        //h=font*line+(line-1)*16   16为测出的行间距
        //totalCount=line*col  totalCount总的字符数
        //font^2+16font-16 -hw/t=0;
        float delta = Mathf.Pow(16, 2) - 4 * (-height * width) / totalCount;
        // x=(-b+-sqrt(b^2-4ac))/2a
        var floatFontSize = (-16 + Mathf.Sqrt(delta)) / 2;
        sizeFont = Mathf.FloorToInt(floatFontSize);
        //算出行数
        int col = Mathf.CeilToInt(width / sizeFont);
        int line = Mathf.FloorToInt(totalCount / col);
        if (line > 3)
        {
            text.lineSpacing = 1+(line-3)*0.1f;
        }
        Debug.LogError($"fontSize:{sizeFont}");
        if (sizeFont <= defaultSize)
        {
            text.fontSize = sizeFont;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
