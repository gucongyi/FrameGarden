using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWaitComponent : MonoBehaviour
{
    public Image imageFg;
    public GameObject goRootNotMask;
    int deltaEulerAngles = 45;
    int currEulerZ = 0;
    float currWaitTime;
    // Start is called before the first frame update
    async void Start()
    {
        while (true)
        {
            if (imageFg == null)
            {
                return;
            }
            imageFg.transform.localEulerAngles = new Vector3(imageFg.transform.localEulerAngles.x, imageFg.transform.localEulerAngles.y, currEulerZ);
            await UniTask.Delay(100);
            currEulerZ += deltaEulerAngles;
        }
    }
    private void Awake()
    {
        currWaitTime = 0f;
        goRootNotMask.SetActive(false);
    }
    private void Update()
    {
        currWaitTime += Time.unscaledDeltaTime;
        if (currWaitTime > 2f)
        {
            goRootNotMask.SetActive(true);
        }
        else
        {
            goRootNotMask.SetActive(false);
        }
    }
}
