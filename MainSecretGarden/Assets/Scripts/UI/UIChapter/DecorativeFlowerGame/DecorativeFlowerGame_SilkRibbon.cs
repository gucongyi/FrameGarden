using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 花绸带
/// </summary>
public class DecorativeFlowerGame_SilkRibbon : DecorativeFlowerGame_Item
{
    Button btn;

    protected override void Start()
    {
        base.Start();
        this.btn = GetComponent<Button>();
        this.btn.onClick.RemoveAllListeners();
        this.btn.onClick.AddListener(OnClickSilkRibbonItem);
    }

    void OnClickSilkRibbonItem()
    {
        parent.ChangeSilkRibbon(image.sprite);
    }
}
