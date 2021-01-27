using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManorZoomNotAffect : MonoBehaviour
{
    Camera cameraScene;
    float normalCameraSize;

    // Start is called before the first frame update
    void Start()
    {
        normalCameraSize = SceneZoom.minCameraSize / 2f;
        cameraScene = Root2dSceneManager._instance.worldCameraComponent.cameraWorld;
    }

    // Update is called once per frame
    void Update()
    {
        //orthographicSize变大，场景看起来变小，对应的UI变小了，所以要放大
        transform.localScale=Vector3.one* (cameraScene.orthographicSize / normalCameraSize);
    }
}
