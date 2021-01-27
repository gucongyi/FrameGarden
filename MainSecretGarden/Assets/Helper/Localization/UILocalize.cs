using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILocalize : MonoBehaviour
{
    public int idDefineLocalize;
    Image image;
    Text text;

    private void Awake()
    {
        image = GetComponent<Image>();
        SetLocalName();

    }

    private void SetLocalName()
    {
        if (image != null)
        {
            string imageName = LocalizationDefineHelper.GetImageNameById(idDefineLocalize);
            image.sprite = ABManager.GetAsset<Sprite>(imageName);
        }
        else
        {
            text = GetComponent<Text>();
            string textString = LocalizationDefineHelper.GetStringNameById(idDefineLocalize);
            text.text = textString;
        }
    }

    public void SetOtherLanguageId(int idDefineLocalize)
    {
        this.idDefineLocalize = idDefineLocalize;
        SetLocalName();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))//测试更新
        {
            SetOtherLanguageId(this.idDefineLocalize);
        }
    }
}
