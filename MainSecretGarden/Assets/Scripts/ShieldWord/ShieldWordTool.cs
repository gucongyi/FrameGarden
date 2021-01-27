using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 屏蔽词
/// </summary>
public static class ShieldWordTool
{
    private static Dictionary<string, int> _blockFonts = new Dictionary<string, int>();

    /// <summary>
    /// 初始化屏蔽词库
    /// </summary>
    public static void InitBlockFonts(string path = null, string text = null)
    {
        if (_blockFonts.Count > 0)
            return;
        if (string.IsNullOrEmpty(text)) 
        {
            if (string.IsNullOrEmpty(path))
            {
                //path = Application.streamingAssetsPath + @"\BlockThesaurus.txt";
                path = Application.dataPath  + @"\BlockThesaurus.txt";
            }
            //string text = System.IO.File.ReadAllText(@"C:\Users\Public\TestFolder\WriteText.txt");
            text = System.IO.File.ReadAllText(@path);
        }
        //Debug.Log("初始化屏蔽词库 text:"+ text);
        string[] arr1 = text.Split('、'); // 以'、'字符对字符串进行分割，返回字符串数组
        for (int i = 0; i < arr1.Length; i++)
        {
            if (!_blockFonts.ContainsKey(arr1[i]))
                _blockFonts.Add(arr1[i], i);

            //if (arr1.Length - 1 == i) 
            //{
            //    Debug.Log(string.Format("初始化屏蔽词库 arr1[{0}]:{1}", i, arr1[i])); 
            //}
        }
    }

    /// <summary>
    /// 屏蔽字库
    /// </summary>
    /// <param name="desc"></param>
    public static bool BlockFont(ref string desc)
    {
        //是否需要屏蔽
        bool isBlock = false;
        //1.获取屏蔽字库
        //Dictionary<string, int> blockFonts = new Dictionary<string, int>();
        if (_blockFonts.Count <= 0)
            InitBlockFonts();
        //2.拆解字符串
        List<string> descList = new List<string>();
        DisassembleTheString(desc, ref descList);
        //3.字符串排序 倒序 长的在前 短的在后
        descList.Sort((d1, d2) => d1.Length > d2.Length ? -1 : 1);
        //4.遍历字符串列表
        foreach (var item in descList)
        {
            //5.查询是否为屏蔽字/句子
            if (_blockFonts.ContainsKey(item))
            {
                int cout = item.Length;
                string replace = null;
                while (cout > 0)
                {
                    cout--;
                    replace = replace + "*";
                }
                //6.替换为同等长度的“*”
                desc = desc.Replace(item, replace);
                isBlock = true;
            }
        }
        return isBlock;
    }
    /// <summary>
    /// 拆解字符串 字符串字符的全集
    /// </summary>
    /// <returns></returns>
    public static void DisassembleTheString(string desc, ref List<string> descList)
    {
        if (desc.Length > 1)
        {
            string str = desc.Substring(0, desc.Length - 1);
            DisassembleTheString(str, ref descList);
        }
        for (int i = 0; i < desc.Length; i++)
        {
            descList.Add(desc.Substring(desc.Length - (1 + i)));
        }
    }
}
