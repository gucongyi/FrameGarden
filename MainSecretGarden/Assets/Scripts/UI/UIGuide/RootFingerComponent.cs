using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootFingerComponent : MonoBehaviour
{
    public GameObject goRightFinger;
    public GameObject goLeftFinger;
    public void SetFinger(bool isRight)
    {
        goRightFinger.SetActive(false);
        goLeftFinger.SetActive(false);
        if (isRight)
        {
            goRightFinger.SetActive(true);
        }
        else
        {
            goLeftFinger.SetActive(true);
        }
    }
}
