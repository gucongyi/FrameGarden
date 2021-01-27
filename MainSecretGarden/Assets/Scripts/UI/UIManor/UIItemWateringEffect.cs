using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemWateringEffect : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 5f);
    }
}
