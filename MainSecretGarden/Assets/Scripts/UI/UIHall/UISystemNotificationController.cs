using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 系统公告控制
/// </summary>
public class UISystemNotificationController : MonoBehaviour
{
    #region 变量
    private GameObject _top;

    private Transform _playerMes;
    private Text _playerDesc;

    private Transform _systemMes;
    private Text _systemDesc;
    /// <summary>
    /// 滚动次数
    /// </summary>
    private int _scrollsNum = 0;

    private bool _isShowMes = false;
    private bool _isNeedFindMes = false;

    private Transform _moveMes;

    #endregion

    #region 方法/函数
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isShowMes)
            FindNewMes();
    }

    private void Init()
    {
        _top = transform.Find("Top").gameObject;

        _playerMes = transform.Find("Top/BG/Mask/PlayerMes");
        _playerDesc = _playerMes.Find("Desc").GetComponent<Text>();
        _playerMes.gameObject.SetActive(false);

        _systemMes = transform.Find("Top/BG/Mask/SystemMes");
        _systemDesc = _systemMes.Find("Desc").GetComponent<Text>();
        _systemMes.gameObject.SetActive(false);

        _top.SetActive(false);
        _isNeedFindMes = true;
    }

    /// <summary>
    /// 查找下一个需要显示的消息
    /// </summary>
    private void FindNewMes()
    {
        if (!_isNeedFindMes)
            return;

        var mes = StaticData.DequeueSystemNotification();
        if (mes == null )
        {
            if (_scrollsNum > 0 && _scrollsNum < 3 && _moveMes != null)
            {
                ResetScrollsShow();
            }
            else 
            {
                _isShowMes = false;
                _top.SetActive(false);
            }

            return;
        }

        _scrollsNum = 0;
        _isShowMes = true;
        _top.SetActive(true);
        switch (mes.NoticeSource)
        {
            case Game.Protocal.PushNoticeSource.PlayerType:
                _playerDesc.text = mes.Desc;
                //ShowMesPlayer();
                _moveMes = _playerMes;
                break;
            case Game.Protocal.PushNoticeSource.SystemType:
                _systemDesc.text = mes.Desc;
                _moveMes = _systemMes;
                break;
            default:
                return;
                break;
        }
        ShowMesMove();
    }

    private void ShowMesMove()
    {
        _moveMes.localPosition = new Vector3(0, -180, 0);
        _moveMes.gameObject.SetActive(true);
        DOTween.To(() => _moveMes.localPosition, r => _moveMes.localPosition = r, new Vector3(0, 0, 0f), 1.2f).SetEase(Ease.Linear).onComplete = InitShowComplete;
    }

    private async void InitShowComplete()
    {
        //await UniTask.DelayFrame(2);
        await UniTask.Delay(2000);

        var targetX = -1.0f * ((_moveMes as RectTransform).sizeDelta.x + (_moveMes.parent as RectTransform).sizeDelta.x) / 2.0f;
        Debug.Log("targetX :"+ targetX + "(_moveMes as RectTransform).sizeDelta :"+ (_moveMes as RectTransform).sizeDelta + "(_moveMes.parent as RectTransform).sizeDelta.x:" + (_moveMes.parent as RectTransform).sizeDelta);
        DOTween.To(() => _moveMes.localPosition, r => _moveMes.localPosition = r, new Vector3(targetX, 0f, 0f), 3f).SetEase(Ease.Linear).onComplete = ScrollsShowComplete;
    }

    private void ResetScrollsShow() 
    {
        var targetX = -1.0f * ((_moveMes as RectTransform).sizeDelta.x + (_moveMes.parent as RectTransform).sizeDelta.x) / 2.0f;
        Debug.Log("targetX :" + targetX);
        _moveMes.localPosition = new Vector3(-1* targetX, 0);
        _moveMes.gameObject.SetActive(true);
        
        DOTween.To(() => _moveMes.localPosition, r => _moveMes.localPosition = r, new Vector3(targetX, 0f, 0f), 6f).SetEase(Ease.Linear).onComplete = ScrollsShowComplete;
    }

    private void ScrollsShowComplete() 
    {
        _scrollsNum += 1;
        _moveMes.gameObject.SetActive(false);
        FindNewMes();
    }

    #endregion

}
