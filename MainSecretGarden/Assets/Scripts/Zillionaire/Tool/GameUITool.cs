using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 游戏界面帮助类
/// </summary>
public static class GameUITool
{
    /// <summary>
    /// 获取物品的保存提示时间的key 用于 PlayerPrefs
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
    public static string GetItemSaveTipsTimeKey(int itemID) 
    {
        return "Day" + itemID;
    }

}
