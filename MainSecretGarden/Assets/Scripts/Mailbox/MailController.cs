using Game.Protocal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 信件
/// </summary>
public class MailController : MonoBehaviour
{
    #region 字段
    CanvasGroup _uiIntegralController;
    CanvasScaler _canvasScaler;
    Transform _topTra;
    /// <summary>
    /// 标题
    /// </summary>
    Text _titleText;
    /// <summary>
    /// 发件人
    /// </summary>
    Text _addressorText;
    /// <summary>
    /// 退出按钮
    /// </summary>
    Button _outBtn;
    /// <summary>
    /// 删除按钮
    /// </summary>
    Button _deleteBtn;
    /// <summary>
    /// 信件文字内容box
    /// </summary>
    RectTransform _contentRect;
    /// <summary>
    /// 信件文字内容滑动组件
    /// </summary>
    ScrollRect _contentScrollRect;
    /// <summary>
    /// 信件文字内容显示
    /// </summary>
    Text _contentText;
    /// <summary>
    /// 彩条
    /// </summary>
    Transform _colourBarTra;
    /// <summary>
    /// 附件标签
    /// </summary>
    Image _adjuncTlabelIamge;
    /// <summary>
    /// 附件标签文字
    /// </summary>
    Text _adjuncTlabelText;
    /// <summary>
    /// 附件box
    /// </summary>
    RectTransform _accessoryBoxRect;
    /// <summary>
    /// 附件滑动组件
    /// </summary>
    ScrollRect _accessoryBoxScrollRect;
    /// <summary>
    /// 底部栏
    /// </summary>
    RectTransform _bottomRect;
    /// <summary>
    /// 确定按钮
    /// </summary>
    Button _confirmBtn;
    /// <summary>
    /// 已领取标签
    /// </summary>
    Transform _getLabelTra;
    /// <summary>
    /// 确定按钮文字显示
    /// </summary>
    Text _confirmBtnText;
    /// <summary>
    /// 附件item克隆母体
    /// </summary>
    Transform _accessoryItem;
    /// <summary>
    /// 信件数据
    /// </summary>
    MailData _mailData;
    /// <summary>
    /// 附件信息
    /// </summary>
    List<MailAccessoryItem> _mailAccessoryItems = new List<MailAccessoryItem>();
    Transform _tageRight;
    Transform _tageLeft;
    /// <summary>
    /// 附件收集box
    /// </summary>
    Transform _gatherTra;
    /// <summary>
    /// 附件收集box上
    /// </summary>
    Transform _gatherTraUp;
    /// <summary>
    /// 附件手机box下
    /// </summary>
    Transform _gatherTraDown;
    float _animationSpeed = 8000f;
    /// <summary>
    /// 是否已经初始化
    /// </summary>
    bool _isInitial = false;
    #endregion
    #region 函数
    // Start is called before the first frame update
    void Start()
    {
        if (!_isInitial)
        {
            Initial();
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// 初始组件
    /// </summary>
    public void Initial()
    {
        _canvasScaler = UIRoot.instance.GetUIRootCanvasTop().transform.GetComponent<CanvasScaler>();
        _uiIntegralController = GetComponent<CanvasGroup>();
        _topTra = transform.Find("Top");
        _titleText = transform.Find("Top/Title").GetComponent<Text>();
        _addressorText = transform.Find("Top/Addressor").GetComponent<Text>();
        _deleteBtn = transform.Find("Top/Delete").GetComponent<Button>();
        _outBtn = transform.Find("OutBtn").GetComponent<Button>();

        _contentRect = transform.Find("Content").GetComponent<RectTransform>();
        _contentScrollRect = _contentRect.Find("Scroll View").GetComponent<ScrollRect>();
        _contentText = _contentScrollRect.content.Find("Text").GetComponent<Text>();
        _accessoryBoxRect = transform.Find("AccessoryBox").GetComponent<RectTransform>();
        _accessoryBoxScrollRect = _accessoryBoxRect.Find("Scroll View").GetComponent<ScrollRect>();
        _bottomRect = transform.Find("Bottom").GetComponent<RectTransform>();
        _confirmBtn = _bottomRect.Find("ConfirmBtn").GetComponent<Button>();
        _confirmBtnText = _confirmBtn.transform.Find("Text").GetComponent<Text>();
        _getLabelTra = _bottomRect.Find("GetLabel");
        _accessoryItem = transform.Find("AccessoryItem");

        _tageRight = _accessoryBoxRect.Find("TageRight");
        _tageLeft = _accessoryBoxRect.Find("TageLeft");
        _colourBarTra = transform.Find("ColourBar ");
        _adjuncTlabelIamge = _colourBarTra.Find("AdjuncTlabel").GetComponent<Image>();
        _adjuncTlabelText = _colourBarTra.Find("Text").GetComponent<Text>();
        _confirmBtnText.text = "领取附件";


        _gatherTra = transform.Find("Gather");
        _gatherTraUp = _gatherTra.Find("Up");
        _gatherTraDown = _gatherTra.Find("Down");

        _outBtn.onClick.RemoveListener(OnClickOutBtn);
        _outBtn.onClick.AddListener(OnClickOutBtn);

        _confirmBtn.onClick.RemoveListener(OnClickConfirmBtn);
        _confirmBtn.onClick.AddListener(OnClickConfirmBtn);

        _deleteBtn.onClick.RemoveListener(OnClickDeleteBtn);
        _deleteBtn.onClick.AddListener(OnClickDeleteBtn);

        SetPanelMultilingual();

        _isInitial = true;
    }
    /// <summary>
    /// 显示信件
    /// </summary>
    /// <param name="mailData"></param>
    public void ShowMail(MailData mailData, List<CSMailAccessory> cSMailAccessories)
    {
        _mailData = mailData;
        if (!_isInitial)
        {
            Initial();
        }
        _titleText.text = _mailData._mailName;
        //设置发件时间
        DateTime time = MailboxTool.GetDateTime(_mailData._timestamp);
        switch (_mailData._addressor)
        {
            case 1:
                _addressorText.text = "系统" + "   " + time.Year + "-" + time.Month + "-" + (time.Day - 7) + "   " + time.Hour + ":" + time.Minute;
                break;
            case 2:
                _addressorText.text = "管理员" + "   " + time.Year + "-" + time.Month + "-" + (time.Day - 7) + "   " + time.Hour + ":" + time.Minute;
                break;
        }
        _contentText.text = _mailData._mailContent;

        if (_mailData._isHaveAccessory)
        {
            _accessoryBoxRect.gameObject.SetActive(true);
            CreationAccessoryItem(cSMailAccessories);
            if (_mailData._type == MailState.ReadAlreadyState)
            {
                _getLabelTra.gameObject.SetActive(true);
                _confirmBtn.gameObject.SetActive(false);
            }
            else
            {
                _getLabelTra.gameObject.SetActive(false);
                _confirmBtn.gameObject.SetActive(true);
            }
            _colourBarTra.gameObject.SetActive(true);

        }
        else
        {

            _colourBarTra.gameObject.SetActive(false);
            _accessoryBoxRect.gameObject.SetActive(false);
            _getLabelTra.gameObject.SetActive(false);
            _confirmBtn.gameObject.SetActive(false);
        }
        _contentScrollRect.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(_contentRect.rect.width, _contentRect.rect.height);
        ResetPoint();
        StartAnimation();
    }
    /// <summary>
    /// 创建附件item
    /// </summary>
    /// <param name="cSMailAccessories"></param>
    void CreationAccessoryItem(List<CSMailAccessory> cSMailAccessories)
    {

        for (int i = 0; i < cSMailAccessories.Count; i++)
        {
            CSMailAccessory data = cSMailAccessories[i];
            GameObject itemObj = GameObject.Instantiate(_accessoryItem.gameObject, _accessoryBoxScrollRect.content);
            MailAccessoryItem mailAccessoryItem = new MailAccessoryItem();
            mailAccessoryItem.Initial(itemObj.transform);
            mailAccessoryItem.Show(itemObj.transform, data, _tageRight, _tageLeft, _mailData._type == MailState.ReadAlreadyState);
            _mailAccessoryItems.Add(mailAccessoryItem);
        }
    }
    /// <summary>
    /// 移动附件item
    /// </summary>
    /// <param name="items"></param>
    /// <param name="index"></param>
    void MoveItem(List<MailAccessoryItem> items, int index, Action endAction)
    {
        if (index >= items.Count)
        {
            Debug.Log("附件移动完毕");
            endAction?.Invoke();
            return;
        }

        StartCoroutine(items[index].MoveIcon(() =>
        {
            int newIndex = index + 1;
            MoveItem(items, newIndex, endAction);
        }));
    }
    /// 移动附件item
    /// </summary>
    /// <param name="items"></param>
    /// <param name="index"></param>
    void QuitMoveItem(List<MailAccessoryItem> items, int index, Action endAction)
    {
        if (index >= items.Count)
        {
            Debug.Log("附件移动完毕");
            endAction?.Invoke();
            return;
        }

        StartCoroutine(items[index].QuitMoveIcon(() =>
        {
            int newIndex = index + 1;
            QuitMoveItem(items, newIndex, endAction);
        }));
    }
    /// <summary>
    /// 清空附件
    /// </summary>
    void EmptyAccessoryItem()
    {
        for (int i = 0; i < _mailAccessoryItems.Count; i++)
        {
            _mailAccessoryItems[i].Destroy();
        }
        _mailAccessoryItems.Clear();
    }
    /// <summary>
    /// 刷新附件item显示
    /// </summary>
    void RefreshAccessoryItem()
    {
        for (int i = 0; i < _mailAccessoryItems.Count; i++)
        {
            _mailAccessoryItems[i].RefreshAccessoryItem(true);
        }
    }
    /// <summary>
    /// 点击确认
    /// </summary>
    private void OnClickConfirmBtn()
    {
        if (_mailData._isHaveAccessory)
        {
            if (_mailData._type != MailState.ReadAlreadyState)
            {
                MailboxTool.GetMailAccessoryInWareHouse(_mailData._mailID, GetMailAccessoryInWareHouseAction);
            }
        }
    }
    /// <summary>
    /// 点击删除
    /// </summary>
    private void OnClickDeleteBtn()
    {
        if (_mailData._isHaveAccessory)
        {

            if (_mailData._type == MailState.ReadAlreadyState)
            {
                //删除信件
                MailboxController.Instance.RefreshMailType(_mailData._mailID, MailState.DeleteState);
                Hide();
            }
            else
            {
                //打开提示弹窗
                StaticData.OpenCommonTips(StaticData.GetMultilingual(120259), 120016, () =>
                {   //删除信件
                    MailboxController.Instance.RefreshMailType(_mailData._mailID, MailState.DeleteState);
                    Hide();
                    Debug.Log("删除信件");
                }, () => { Debug.Log("取消删除！"); }
                , 120075);
            }
        }
        else
        {
            //删除信件
            MailboxController.Instance.RefreshMailType(_mailData._mailID, MailState.DeleteState);
            Hide();
        }

    }
    /// <summary>
    /// 领取附件回调
    /// </summary>
    void GetMailAccessoryInWareHouseAction(List<int> ids, List<CSMailAccessory> cSMails)
    {
        _mailData._type = MailState.ReadAlreadyState;
        _getLabelTra.gameObject.SetActive(true);
        _confirmBtn.gameObject.SetActive(false);
        //List<Transform> iconTras = new List<Transform>();
        //List<Transform> iconPas = new List<Transform>();
        for (int i = 0; i < _mailAccessoryItems.Count; i++)
        {
            //iconPas.Add(_mailAccessoryItems[i]._iconTwoTra.parent);
            //iconTras.Add(_mailAccessoryItems[i]._iconTwoTra);
            StaticData.UpdateWareHouseItem(_mailAccessoryItems[i]._Data.GoodsId, _mailAccessoryItems[i]._Data.GoodsNum);
        }
        //StaticData.CreateToastTips(StaticData.GetMultilingual(120260));

        //HarvestItemAnimation(iconTras, iconPas);
    }
    /// <summary>
    /// 点击退出
    /// </summary>
    private void OnClickOutBtn()
    {
        CloseAnimation();
    }

    void ResetDataType()
    {
        if (_mailData._isHaveAccessory)
        {
            if (_mailData._type != MailState.ReadAlreadyState)
            {
                MailboxController.Instance.RefreshMailType(_mailData._mailID, MailState.ReadUnAlreadyState);
            }
            else
            {
                MailboxController.Instance.RefreshMailType(_mailData._mailID, MailState.ReadAlreadyState);
            }
        }
        else
        {
            MailboxController.Instance.RefreshMailType(_mailData._mailID, MailState.ReadUnAlreadyState);
        }
    }
    void Hide()
    {

        UIComponent.HideUI(UIType.Mail);
        EmptyAccessoryItem();
    }
    /// <summary>
    /// 重置ui位置便于开始动画播放
    /// </summary>
    void ResetPoint()
    {
        _uiIntegralController.interactable = false;

        float x = (_canvasScaler.referenceResolution.x / 2);
        //_outBtn.transform.localPosition = new Vector3(x + (_outBtn.transform.GetComponent<RectTransform>().sizeDelta.x / 2), _outBtn.transform.localPosition.y);
        _topTra.localPosition = new Vector3(x + (_topTra.GetComponent<RectTransform>().sizeDelta.x / 2), _topTra.localPosition.y);
        _contentScrollRect.transform.localPosition = new Vector3(x + (_contentScrollRect.transform.GetComponent<RectTransform>().sizeDelta.x / 2), _contentScrollRect.transform.localPosition.y);
        _colourBarTra.localPosition = new Vector3(x + (_colourBarTra.GetComponent<RectTransform>().sizeDelta.x / 2), _colourBarTra.localPosition.y);
        _getLabelTra.localScale = Vector3.zero;
        _confirmBtn.transform.localScale = Vector3.zero;
    }
    /// <summary>
    /// 开始动画
    /// </summary>
    public void StartAnimation()
    {
        float x = (_canvasScaler.referenceResolution.x / 2);
        _outBtn.gameObject.SetActive(true);
        StartCoroutine(MailboxTool.MoveUI(_topTra.transform, new Vector3(-(x - _topTra.GetComponent<RectTransform>().sizeDelta.x / 2), _topTra.localPosition.y), () =>
        {
            StartCoroutine(MailboxTool.MoveUI(_contentScrollRect.transform, new Vector3(-(x - _contentScrollRect.GetComponent<RectTransform>().sizeDelta.x / 2), _contentScrollRect.transform.localPosition.y), () =>
            {

                if (_mailData._isHaveAccessory)
                {
                    StartCoroutine(MailboxTool.MoveUI(_colourBarTra.transform, new Vector3(-(x - _colourBarTra.GetComponent<RectTransform>().sizeDelta.x / 2), _colourBarTra.transform.localPosition.y), () =>
                    {
                        MoveItem(_mailAccessoryItems, 0, () =>
                        {
                            ChangBtnScale(Vector3.one, () => { _uiIntegralController.interactable = true; });
                        });
                    }, _animationSpeed));
                }
                else
                {
                    _uiIntegralController.interactable = true;
                }

            }, _animationSpeed));

        }, _animationSpeed));

    }
    /// <summary>
    /// 修改按钮尺寸
    /// </summary>
    /// <param name="tageScale"></param>
    /// <param name="endAction"></param>
    void ChangBtnScale(Vector3 tageScale, Action endAction)
    {
        StartCoroutine(MailboxTool.ChangScale(_getLabelTra.transform, tageScale, null, _animationSpeed));
        StartCoroutine(MailboxTool.ChangScale(_confirmBtn.transform, tageScale, () =>
        {
            endAction?.Invoke();
            Debug.Log("展开按钮完毕");
        }, 10f));
    }
    /// <summary>
    /// 关闭动画
    /// </summary>
    void CloseAnimation()
    {
        float x = (_canvasScaler.referenceResolution.x / 2);
        StartCoroutine(MailboxTool.MoveUI(_topTra.transform, new Vector3(-(x + _topTra.GetComponent<RectTransform>().sizeDelta.x / 2), _topTra.localPosition.y), () =>
        {
            StartCoroutine(MailboxTool.MoveUI(_contentScrollRect.transform, new Vector3(-(x + _contentScrollRect.GetComponent<RectTransform>().sizeDelta.x / 2), _contentScrollRect.transform.localPosition.y), () =>
            {

                if (_mailData._isHaveAccessory)
                {
                    StartCoroutine(MailboxTool.MoveUI(_colourBarTra.transform, new Vector3(-(x + _colourBarTra.GetComponent<RectTransform>().sizeDelta.x / 2), _colourBarTra.transform.localPosition.y), () =>
                    {
                        QuitMoveItem(_mailAccessoryItems, 0, () =>
                        {
                            ChangBtnScale(Vector3.zero, () =>
                            {
                                StopAllCoroutines();
                                ResetDataType();
                                Hide();
                            });
                        });


                    }, _animationSpeed));
                }
                else
                {
                    ResetDataType();
                    Hide();

                }

            }, _animationSpeed));

        }, _animationSpeed));
    }
    /// <summary>
    /// 领取动画
    /// </summary>
    /// <param name="tageTras"></param>
    /// <param name="oldParents"></param>
    void HarvestItemAnimation(List<Transform> tageTras, List<Transform> oldParents)
    {

        for (int i = 0; i < tageTras.Count; i++)
        {
            tageTras[i].SetParent(_gatherTra);
        }

        List<Vector3> tagePoints = new List<Vector3>();
        for (int i = 0; i < tageTras.Count; i++)
        {
            float x = UnityEngine.Random.Range(-200, 200);
            Vector3 vector3 = new Vector3(x, _gatherTraUp.localPosition.y);
            tagePoints.Add(vector3);
        }

        StartCoroutine(MailboxTool.MoveUIs(tageTras, tagePoints, () =>
        {
            GatherMoveItem(tageTras, 0, () =>
            {
                Debug.Log("领取完毕");
                for (int i = 0; i < tageTras.Count; i++)
                {
                    tageTras[i].SetParent(oldParents[i]);
                    tageTras[i].localPosition = Vector3.zero;
                }
            });

        }, 3000));
    }

    /// <summary>
    /// 移动附件item
    /// </summary>
    /// <param name="items"></param>
    /// <param name="index"></param>
    void GatherMoveItem(List<Transform> items, int index, Action endAction)
    {
        if (index >= items.Count)
        {
            Debug.Log("附件移动完毕");
            endAction?.Invoke();
            return;
        }

        StartCoroutine(MailboxTool.MoveUITwo(items[index], _gatherTraDown.localPosition, () =>
        {
            int indexTwo = index + 1;
            GatherMoveItem(items, indexTwo, endAction);
        }, 4000, 100));
    }

    /// <summary>
    /// 设置多语言显示
    /// </summary>
    void SetPanelMultilingual()
    {
        _adjuncTlabelText.text = StaticData.GetMultilingual(120165);
        _confirmBtnText.text = StaticData.GetMultilingual(120166);
    }
    #endregion
}
