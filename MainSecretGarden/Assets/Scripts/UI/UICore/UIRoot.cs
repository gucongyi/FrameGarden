using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoot : MonoBehaviour
{
    Canvas canvas;
    Canvas canvasTop;
    Canvas canvasBG;
    Canvas canvasGuide;
    Camera uiCamera;
    public GameObject GoBgInit;
    public static UIRoot instance;
    private Transform clickEffect;
    void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        canvasTop= GameObject.Find("CanvasTop").GetComponent<Canvas>();
        canvasBG = GameObject.Find("CanvasBG").GetComponent<Canvas>();
        canvasGuide= GameObject.Find("CanvasGuide").GetComponent<Canvas>();
        uiCamera = canvas.worldCamera;
        clickEffect = transform.Find("ClickEff");
        clickEffect.GetComponent<ParticleSystem>().Play(); //先执行一次点击特效
    }

    public Canvas GetUIRootCanvas()
    {
        return canvas;
    }
    public Canvas GetUIRootCanvasTop()
    {
        return canvasTop;
    }

    public Canvas GetUIRootCanvasBG()
    {
        return canvasBG;
    }

    public Canvas GetUIRootCanvasGuide()
    {
        return canvasGuide;
    }

    public Camera GetUIRootCamera()
    {
        return uiCamera;
    }

    public Transform GetClickEffect()
    {
        return clickEffect;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
