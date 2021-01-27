using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 插花小游戏所有item的基类
/// </summary>
public class DecorativeFlowerGame_Item : MonoBehaviour
{
    protected Image image;
    protected DecorativeFlowerGameComponent parent;

    protected virtual void Start()
    {
        this.image = transform.Find("Icon").GetComponent<Image>();
        parent = GetComponentInParent<DecorativeFlowerGameComponent>();
    }

}
