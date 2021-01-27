using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 初始化屏蔽词库
/// </summary>
public class InitShieldWord : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Init() 
    {
        //ShieldWordTool.InitBlockFonts();
        //TextAsset textAsset = await ABManager.GetAssetAsync<TextAsset>("blockthesaurus");
        TextAsset textAsset = Resources.Load<TextAsset>("ShieldWord/BlockThesaurus");
        string text = textAsset.text;
        ShieldWordTool.InitBlockFonts(null, text);
    }
}
