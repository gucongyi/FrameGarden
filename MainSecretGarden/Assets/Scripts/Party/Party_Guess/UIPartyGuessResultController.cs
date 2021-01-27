using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPartyGuessResultController : MonoBehaviour
{
    /// <summary>
    /// 单例字段
    /// </summary>
    public static UIPartyGuessResultController instance;

    private void Awake()
    {
        instance = this;
    }


    #region 变量
    private Button _butReturn;
    public Text guessResult;
    #endregion


    void Start()
    {
        Init();
    }
    //测试
    private void Update()
    {
        guessResult.text = PartyManager._instance.test;
    }

    private void Init()
    {
        _butReturn = transform.Find("Return").GetComponent<Button>();
        _butReturn.onClick.RemoveAllListeners();
        _butReturn.onClick.AddListener(StaticData.CloseGuessResultUI);

        guessResult = transform.Find("Result/Text").GetComponent<Text>();
    }
}
