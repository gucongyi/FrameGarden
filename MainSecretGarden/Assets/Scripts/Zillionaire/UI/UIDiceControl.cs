using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 大富翁游戏骰子
/// </summary>
public class UIDiceControl : MonoBehaviour
{
    #region 字段
    /// <summary>
    /// 骰子展示image
    /// </summary>
    private Image _showImage;
    /// <summary>
    /// 骰子动画播放器
    /// </summary>
    Animator _animator;
    /// <summary>
    /// 骰子结果字典
    /// </summary>
    Dictionary<int, Sprite> _resultDic = new Dictionary<int, Sprite>();
    /// <summary>
    /// 是否可以投掷
    /// </summary>
    bool _isThrow = false;
    /// <summary>
    /// 投掷结果
    /// </summary>
    int _currResult = 0;
    /// <summary>
    /// 摇色子结束
    /// </summary>
    System.Action _endThowAction;
    #endregion
    #region 属性
    /// <summary>
    /// 是否可以投掷
    /// </summary>
    public bool _IsThrow { get { return _isThrow; } }

    private Sprite[] sprites = new Sprite[6];

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

    /// <summary>
    /// 骰子初始化
    /// </summary>
    /// <param name="tra"></param>
    /// <param name="keyValuePairs"></param>
    public void Initial()
    {
        InitSprite();

        _isThrow = true;
        _showImage = transform.GetComponent<Image>();
        _animator = transform.GetComponent<Animator>();
        InitialDiceResultDic(1);
        gameObject.SetActive(false);
    }

    private void InitSprite() 
    {
        string name = "";
        for (int i = 0; i < 6; i++)
        {
            name = "Image (" + (i + 1) + ")";
            sprites[i] = transform.Find(name).GetComponent<Image>().sprite;
        }
    }


    /// <summary>
    /// 初始化骰子结果字典
    /// </summary>
    private void InitialDiceResultDic(int index)
    {
        if (index > 6)
        {
            _isThrow = false;
            return;
        }
        //string mapNam = "touzi_" + index;
        //Sprite icon = await ABManager.GetAssetAsync<Sprite>(mapNam);
        _resultDic.Add(index, sprites[index - 1]);
        InitialDiceResultDic(index + 1);
    }


    /// <summary>
    /// 投掷骰子
    /// </summary>
    /// <param name="index"></param>
    public void ThrowDice(int index, System.Action endAction)
    {
        if (_isThrow || !ZillionairePlayerManager._instance.IsPlayerMoveEnd())
        {
            return;
        }
        _endThowAction = endAction;
        _isThrow = true;
        _currResult = index;

        
        int time = Random.Range(600, 1000);
        PosMove(time);
        transform.gameObject.SetActive(true);
        Throw(time);
    }

    /// <summary>
    /// 位置移动
    /// </summary>
    /// <param name="time"></param>
    private void PosMove(int time) 
    {
        this.transform.localPosition = new Vector3(0, 1200, 0);
        float timeSecond = (float)time / 1000f;

        DOTween.To(() => this.transform.position, r => this.transform.position = r, Vector3.zero, timeSecond).SetEase(Ease.OutBounce); 
    }

    /// <summary>
    /// 投掷
    /// </summary>
    /// <param name="time"></param>
    private async void Throw(int time)
    {

        _animator.enabled = true;

        await Task.Delay(time);

        _animator.enabled = false;

        ShowResult();
        await Task.Delay(900);
        gameObject.SetActive(false);
        _isThrow = false;
        _endThowAction?.Invoke();
    }


    /// <summary>
    /// 展示结果
    /// </summary>
    private void ShowResult()
    {
        _showImage.sprite = _resultDic[_currResult];
        _showImage.SetNativeSize();
    }


    /// <summary>
    /// 开关骰子
    /// </summary>
    /// <param name="isOpen"></param>
    public void Open(bool isOpen)
    {
        transform.gameObject.SetActive(isOpen);
    }

    /// <summary>
    /// 是否可以使用
    /// </summary>
    public void Disp()
    {
        Open(false);
        _isThrow = false;
        _currResult = 0;
    }
    #endregion
}
