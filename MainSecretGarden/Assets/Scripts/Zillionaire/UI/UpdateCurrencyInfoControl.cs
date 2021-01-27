using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 更新货币信息制器
/// </summary>
public class UpdateCurrencyInfoControl : MonoBehaviour
{
    /// <summary>
    /// 货币类型 1.金币 2.钻石 3.水滴 4.紫星币
    /// </summary>
    [SerializeField]
    private int _currencyType = 1;

    /// <summary>
    /// 货币数量
    /// </summary>
    private Text _currencyNum;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNum();
    }

    private void Init() 
    {
        _currencyNum = transform.Find("Num").GetComponent<Text>();
        var transAdd = transform.Find("Add");
        if (transAdd != null&& _currencyType!=1)
        {
            transAdd.gameObject.SetActive(false);
        }
        //transform.Find("Add").GetComponent<Button>().onClick.AddListener(OnClickAdd);
        gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
        gameObject.GetComponent<Button>().onClick.AddListener(OnClickAdd);
        UpdateNum();
    }

    /// <summary>
    /// 获取货币数量
    /// </summary>
    /// <returns></returns>
    private string GetCurrencyNum() 
    {
        if (_currencyType == 2)
        {
            return StaticData.GetWareHouseDiamond().ToString();
        }
        else if (_currencyType == 3)
        {
            var waterInfo = StaticData.GetFertilizerCountByWhich(0);
            int waterCount = waterInfo.GoodNum;
            if (waterCount <= 0)
            {
                waterCount = 0;
            }
            return $"{waterCount}";
        }
        else if (_currencyType == 4)
        {
            return StaticData.GetWareHousePurpleGold().ToString();
        }
        
        return StaticData.GetWareHouseGold().ToString();
    }

    /// <summary>
    /// 更新货币数量
    /// </summary>
    private void UpdateNum() 
    {
        if (_currencyNum != null)
            _currencyNum.text = GetCurrencyNum();
    }

    /// <summary>
    /// 点击快捷按钮
    /// </summary>
    private void OnClickAdd() 
    {
        if (_currencyType != 1)
        {
            return;
        }
        Debug.Log("点击打开外部充值接口 货币类型："+ _currencyType);
        StaticData.OpenRechargeUI(_currencyType - 1);
    }
}
