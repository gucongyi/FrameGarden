using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 分享界面控制
/// </summary>
public class UIShareController : MonoBehaviour
{

    private Image _image;

    private Button _butClose;
    private Button _butSave;
    private Button _butShare;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Init()
    {
        if (_image != null)
            return;

        _image = transform.Find("Panel/Image").GetComponent<Image>();
        _butClose = transform.Find("Panel/But_Close").GetComponent<Button>();
        _butSave = transform.Find("Panel/But_Save").GetComponent<Button>();
        _butShare = transform.Find("Panel/But_Share").GetComponent<Button>();

        _butClose.onClick.RemoveAllListeners();
        _butClose.onClick.AddListener(OnClickClose);

        _butSave.onClick.RemoveAllListeners();
        _butSave.onClick.AddListener(OnClickSave);

        _butShare.onClick.RemoveAllListeners();
        _butShare.onClick.AddListener(OnClickShare);
    }

    /// <summary>
    /// 界面初始化值
    /// </summary>
    public void InitValue( Sprite image ) 
    {
        Init();

        Debug.Log("image.texture.width = "+ image.texture.width);
        float f = 1242f / (float)image.texture.width;
        f /= 2.0f;
        _image.sprite = image;
        _image.SetNativeSize();
        Vector3 targetScale = Vector3.one;
        targetScale.x *= f;
        targetScale.y *= f;
        _image.transform.localScale = targetScale;
    }

    /// <summary>
    /// 关闭界面
    /// </summary>
    private void OnClickClose() 
    {
        StaticData.CloseUIShare();
    }

    /// <summary>
    /// 保存截图
    /// </summary>
    private void OnClickSave()
    {

    }

    /// <summary>
    /// 分享截图
    /// </summary>
    private void OnClickShare()
    {

    }
}
