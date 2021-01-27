using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ManorRedDotTool
{
    public static bool isOpenManorDecorateRedDot = false;
    public static bool isManorHaveGain = false;
    public static bool IsOpenManorDecorateRedDot()
    {
        bool isOpen = isOpenManorDecorateRedDot;
        return isOpen;
    }

    public static bool IsOpenManorRedDotInLobby()
    {
        bool isOpen = false;
        bool isTaskRedDotOpen = TaskPanelTool.IsOpenDot();
        bool isOrderRedDotOpen = StaticData.IsSubmintDeal();
        bool isWarehouseRedDotOpen = StaticData.IsWarehouseRedDotOpen();
        bool isShopRedDotOpen = ShopTool.IsOpenRedDot();
        bool isOpenManorDecorateRedDot = IsOpenManorDecorateRedDot();
        if (isTaskRedDotOpen
            || isOrderRedDotOpen
            || isWarehouseRedDotOpen
            || isShopRedDotOpen
            || isOpenManorDecorateRedDot
            || isManorHaveGain)
        {
            isOpen = true;
        }
        //收获判定

        return isOpen;
    }

}
