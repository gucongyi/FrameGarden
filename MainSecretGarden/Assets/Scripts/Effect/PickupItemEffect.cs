using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 拾取物品效果
/// </summary>
public class PickupItemEffect : MonoBehaviour
{

    #region 变量

    public AnimationCurve curve;

    private Transform _traIcon;
    private Image _icon;
    private Text _num;

    //private float _moveTime = 0.9f;//
    private float _moveSpeed = 1050f;///
    private float _itemEffectTime = 0.6f;

    private string _effectKey = "PickupItemEffectIcon";

    #endregion

    #region 函数

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
        if (_traIcon != null)
            return;

        _traIcon = transform.Find("Icon");
        _icon = _traIcon.GetComponent<Image>();
        _num = _icon.transform.Find("Num").GetComponent<Text>();
        _num.gameObject.SetActive(false);

    }

    /// <summary>
    /// 初始化值
    /// </summary>
    /// <param name="sprite">物品图片</param>
    /// <param name="scale">尺寸</param>
    /// <param name="loc">屏幕位置</param>
    /// <param name="targetPos">目标位置</param>
    /// <param name="itemID">物品id</param>
    /// <param name="isEvent">是否为事件</param>
    public void InitValue(Sprite sprite, float scale, Vector3 loc, Vector3 targetPos, int itemID = 0, bool isEvent = false)
    {
        _effectKey = _effectKey + "_"+sprite.texture.name;

        Debug.Log("初始化值 _effectKey = "+ _effectKey);
        Init();

        //1.获取UI Camera 如果只有一个相机 就是MainCamera = Camera.main
        //Camera uiCamera = Camera.main;//GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        //将from转换到屏幕坐标
        //Vector2 V2fromInScreen = RectTransformUtility.WorldToScreenPoint(uiCamera, from.transform.position);
        //将屏幕坐标转换到at的局部坐标中
        Vector2 V2InAt;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, loc, Camera.main, out V2InAt);

        _traIcon.localPosition = V2InAt;
        _traIcon.localScale = new Vector3(scale, scale, 1.0f);
        _icon.color = new Color(_icon.color.r, _icon.color.g, _icon.color.b, 1.0f);
        _icon.sprite = sprite;
        _icon.SetNativeSize();

        if (itemID == StaticData.configExcel.GetVertical().GoldGoodsId || itemID == StaticData.configExcel.GetVertical().JewelGoodsId)
        {
            if (!isEvent && targetPos != Vector3.zero)
            {
                SatrtMoveTargetEffect(targetPos);
            }
            else 
            {
                GoodsEffect(itemID, targetPos);
            }
        }
        else
        {
            StartEffect();
        }

    }

    System.Action<int> PickupEffectComplete;
    private int _itemID;

    public void InitValue(Sprite sprite, float scale, Vector3 loc, int itemID, int itemNum, System.Action<int> effectComplete) 
    {
        PickupEffectComplete = effectComplete;
        _itemID = itemID;
        Init();

        //将屏幕坐标转换到at的局部坐标中
        Vector2 V2InAt;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, loc, Camera.main, out V2InAt);

        _traIcon.localPosition = V2InAt;
        _traIcon.localScale = new Vector3(scale, scale, 1.0f);
        _icon.color = new Color(_icon.color.r, _icon.color.g, _icon.color.b, 1.0f);
        _icon.sprite = sprite;
        _icon.SetNativeSize();

        _num.text = itemNum.ToString();
        _num.gameObject.SetActive(true);

        Vector3 targetPos = GetTargetPos(itemID);

        StartCurveMove(targetPos, _traIcon, _icon);
    }

    /// <summary>
    /// 开始执行效果 物品效果 头顶向上移动消失
    /// </summary>
    private void StartEffect()
    {
        DOTween.To(() => _traIcon.localPosition, r => _traIcon.localPosition = r, new Vector3(_traIcon.localPosition.x, _traIcon.localPosition.y + 240f, 0f), _itemEffectTime).SetEase(Ease.InQuint);
        DOTween.To(() => _icon.color, al => _icon.color = al, new Color(_icon.color.r, _icon.color.g, _icon.color.b, 0.0f), _itemEffectTime).SetEase(Ease.InQuint).onComplete = EffectComplete;
    }

    private void SatrtMoveTargetEffect(Vector3 targetPos) 
    {
        StartCurveMove(targetPos, _traIcon.transform, _icon);
    }


    /// <summary>
    /// 货币效果
    /// </summary>
    /// <param name="itemID"></param>
    private void GoodsEffect(int itemID, Vector3 endPos, int minrRandomNum = 5, int maxRandomNum = 9)
    {
        if (endPos == Vector3.zero)
            endPos = GetTargetPos(itemID);

        int goodNum = Random.Range(minrRandomNum, maxRandomNum);
        SpawnItemIcon(goodNum, endPos);

    }

    /// <summary>
    /// 随机生成后位置
    /// </summary>
    /// <returns></returns>
    private Vector3 RandomSpawnPos()
    {
        var xC = Random.Range(0, 2) == 0 ? -1 : 1;
        var yC = Random.Range(0, 2) == 0 ? -1 : 1;
        var randomX = Random.Range(Screen.width / 10, Screen.width / 8) * xC;
        var randomY = Random.Range(Screen.height / 10, Screen.height / 8) * yC;
        return _traIcon.localPosition + new Vector3(randomX, randomY, 0f);
    }

    /// <summary>
    /// 生成ItemIcon
    /// </summary>
    /// <param name="num"></param>
    /// <param name="endPos"></param>
    private async void SpawnItemIcon(int num, Vector3 endPos)
    {

        //var obj = Instantiate(_traIcon.gameObject, _traIcon.parent);
        var obj = ObjectPoolManager.Instance.CreatObject(_effectKey, _traIcon.gameObject, _traIcon.parent);
        obj.GetComponent<Image>().color = Color.white;
        obj.transform.localPosition = _traIcon.localPosition;
        obj.transform.localScale = _traIcon.localScale;

        var targetPos = RandomSpawnPos();

        DOTween.To(() => obj.transform.localPosition, localPos => obj.transform.localPosition = localPos, targetPos, 0.2f);
        await UniTask.Delay(100);
        num--;
        if (num > 0)
        {
            SpawnItemIcon(num, endPos);
        }
        else
        {
            StartCurveMove(endPos, _traIcon, _icon);
        }
        await UniTask.Delay(200);
        StartCurveMove(endPos, obj.transform, obj.GetComponent<Image>());
    }

    /// <summary>
    /// 曲线运动
    /// </summary>
    private async void StartCurveMove(Vector3 endPos, Transform traIcon, Image icon)
    {

        #region 方法 1
        ////圆的参数方程            x = a + r cosθ y = b + r sinθ（θ∈ [0，2π) ） (a, b) 为圆心坐标，r 为圆半径，θ 为参数，(x, y) 为经过点的坐标
        ////椭圆的参数方程          x= a cosθ　 y=b sinθ（θ∈[0，2π）） a为长半轴长 b为短半轴长 θ为参数
        ////双曲线的参数方程        x= a secθ （正割） y=b tanθ a为实半轴长 b为虚半轴长 θ为参数    secθ （正割）即1/cosθ 

        //var endPos = GetTargetPos();

        //Vector3[] path1 = new Vector3[3];
        //path1[0] = _traIcon.localPosition;//起始点
        //path1[1] = GetMiddlePos(_traIcon.localPosition, endPos);//中间点
        //path1[2] = endPos;//终点

        //Debug.Log("曲线运动 startPos = "+ path1[0]);
        //Debug.Log("曲线运动 GetMiddlePos = " + path1[1]);
        //Debug.Log("曲线运动 endPos = " + path1[2]);

        //var tweenPath = _traIcon.transform.DOLocalPath(path1, 1.8f, PathType.CatmullRom);//.SetEase(Ease.InQuint);
        //tweenPath.onComplete = () =>
        //{
        //    Debug.Log("曲线运动 完成 StartCurveMove ");
        //};
        #endregion

        #region 方法2

        //将屏幕坐标转换到transform的局部坐标中
        Vector2 V2InAt;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, endPos, Camera.main, out V2InAt);

        endPos = V2InAt;
        var r = (endPos - traIcon.localPosition).magnitude;
        var moveTime = r / _moveSpeed;

        //方向值 
        int directionValue = 1;
        if (traIcon.localPosition.x < _traIcon.localPosition.x)
        {
            directionValue = -1;
        }
        float timeAdd = 0f;
        DOTween.To(() => traIcon.localPosition,
            (pos) =>
            {
                timeAdd += Time.deltaTime;
                var x = curve.Evaluate(timeAdd / moveTime);

                traIcon.localPosition = pos + r * new Vector2(x, 0.0f) * directionValue;

            }, endPos, moveTime).SetEase(Ease.Linear);
        var delayTime = (int)(moveTime * 1000 - 200);
        if (delayTime > 0)
            await UniTask.Delay(delayTime);
        DOTween.To(() => icon.color, color => icon.color = color, new Color(icon.color.r, icon.color.g, icon.color.b, 0.0f), 0.2f);
        await UniTask.Delay(200);
        if (traIcon != _traIcon)
        {
            ObjectPoolManager.Instance.RecycleObject(_effectKey, traIcon.gameObject);
        }
        else
        {
            EffectComplete();
        }
        #endregion
    }

    /// <summary>
    /// 获取目标点
    /// </summary>
    /// <returns></returns>
    private Vector3 GetTargetPos(int itemID)
    {
        Vector3 endPos = Vector3.zero;

        bool isFindIcon = true;
        GameObject[] goods;
        if (itemID == StaticData.configExcel.GetVertical().GoldGoodsId)
        {
            goods = GameObject.FindGameObjectsWithTag("Gold");
        }
        else if (itemID == StaticData.configExcel.GetVertical().JewelGoodsId)
        {
            goods = GameObject.FindGameObjectsWithTag("Diamond");

        }
        else if (itemID == StaticData.configExcel.GetVertical().StageRelation[0])//水滴
        {
            goods = GameObject.FindGameObjectsWithTag("Water");

        }
        else if (itemID == StaticData.configExcel.GetVertical().ExpId) //
        {
            isFindIcon = false;
            goods = GameObject.FindGameObjectsWithTag("IconExp");
        }
        else
        {
            isFindIcon = false;
            goods = GameObject.FindGameObjectsWithTag("WarehouseIcon");
        }
        if (isFindIcon)
        {
            foreach (var item in goods)
            {
                if (item.activeInHierarchy)
                {
                    endPos = item.transform.Find("Icon").transform.position;
                }
            }
        }
        else 
        {
            if (goods.Length > 0)
                endPos = goods[0].transform.position;
        }


        return RectTransformUtility.WorldToScreenPoint(Camera.main, endPos);
    }


    /// <summary>
    /// 效果完成
    /// </summary>
    private void EffectComplete()
    {
        Debug.Log("EffectComplete 拾取物品效果 效果完成");
        //gameObject.SetActive(false);
        //Destroy(this);
        //UIComponent.HideUI(UIType.PickupItem);
        ObjectPoolManager.Instance.RecycleObject(UIType.PickupItem, gameObject);
        PickupEffectComplete?.Invoke(_itemID);
        PickupEffectComplete = null;
    }
    #endregion


}
