using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 花瓶or花束item
/// </summary>
public class DecorativeFlowerGame_VaseItem : DecorativeFlowerGame_Item
{
    Button btn;

    protected override void Start()
    {
        base.Start();
        this.btn = GetComponent<Button>();
        this.btn.onClick.RemoveAllListeners();
        this.btn.onClick.AddListener(OnClickVaseItem);
    }

    void OnClickVaseItem()
    {
        parent.ChangeVase(image.sprite);
    }

}