using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFertilizerEffectComponent : MonoBehaviour
{
    public Image IconFertilizer;
    public async void ShowInfo(string iconName)
    {
        IconFertilizer.sprite = await ABManager.GetAssetAsync<Sprite>(iconName);
    }
}
