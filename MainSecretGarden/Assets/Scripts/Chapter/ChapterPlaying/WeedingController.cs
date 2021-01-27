using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using Random = UnityEngine.Random;
/// <summary>
/// 章节玩法 除草
/// </summary>
public class WeedingController : MonoBehaviour
{
    #region 字段
    /// <summary>
    /// 开始对话
    /// </summary>
    [SerializeField]
    int _startDialogueId;
    /// <summary>
    /// 提示对话
    /// </summary>
    [SerializeField]
    int _tipDialogueId;
    /// <summary>
    /// ui整体控制组件
    /// </summary>
    CanvasGroup _thisCanvasGroup;
    /// <summary>
    /// 除草人物对象
    /// </summary>
    RectTransform _roleRect;
    /// <summary>
    /// 除草按钮rect
    /// </summary>
    RectTransform _weedingBtnRect;
    /// <summary>
    /// 除草按钮
    /// </summary>
    Button _weedingBtn;
    /// <summary>
    /// 清理完成退出按钮
    /// </summary>
    Button _outBtn;
    /// <summary>
    /// 除草奖励
    /// </summary>
    [SerializeField]
    List<Sprite> _awardItemSprites = new List<Sprite>();
    /// <summary>
    /// 垃圾桶
    /// </summary>
    [SerializeField]
    List<Transform> _trashCan = new List<Transform>();
    /// <summary>
    /// 杂草点击按钮
    /// </summary>
    [SerializeField]
    List<Button> _weedBtns = new List<Button>();
    /// <summary>
    /// 除草奖励item
    /// </summary>
    UIPanelDrag _awardItem;
    /// <summary>
    /// 生成的奖励
    /// </summary>
    Dictionary<Transform, List<UIPanelDrag>> _showAwardItemDic = new Dictionary<Transform, List<UIPanelDrag>>();
    /// <summary>
    /// 所有生成的奖励
    /// </summary>
    List<UIPanelDrag> _allAwards = new List<UIPanelDrag>();
    /// <summary>
    /// 当前处于的杂草
    /// </summary>
    [SerializeField]
    int _currWeedIndex;
    /// <summary>
    /// 是否淡出
    /// </summary>
    [SerializeField]
    bool _isFadeOut = true;
    /// <summary>
    /// 结束回调
    /// </summary>
    Action _endAction;
    /// <summary>
    /// 方形对话框
    /// </summary>
    DialogueBoxTetragonumComponent _tetragonumComponent;
    /// <summary>
    /// 角色动画控制器
    /// </summary>
    Animator roleWeedingAnimatorController;
    /// <summary>
    /// 是否在除草状态
    /// </summary>
    bool isWeeding = false;
    int weedCount = 0;//锄草次数，2次时生成引导
    bool onece = true;//第一次生成引导
    Action createGuidance;//创建引导
    ChapterGuidance chapterGuidance;//第一次除草的引导
    int awaitTime = 2400;//原速4300
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
    /// 初始化组件
    /// </summary>
    /// <param name="endAction"></param>
    public void Initial(Action endAction)
    {
        _endAction = endAction;
        _thisCanvasGroup = GetComponent<CanvasGroup>();
        _roleRect = transform.Find("Role").GetComponent<RectTransform>();
        _weedingBtnRect = transform.Find("WeedingBtn").GetComponent<RectTransform>();
        _weedingBtn = _weedingBtnRect.GetComponent<Button>();
        _awardItem = transform.Find("AwardItem").GetComponent<UIPanelDrag>();
        _outBtn = transform.Find("OutBtn").GetComponent<Button>();
        _tetragonumComponent = transform.Find("DialogueBox_Tetragonum").GetComponent<DialogueBoxTetragonumComponent>();
        _outBtn.onClick.RemoveAllListeners();
        _outBtn.onClick.AddListener(ClickOutBtn);
        _weedingBtn.onClick.RemoveAllListeners();
        _weedingBtn.onClick.AddListener(ClickWeedingBtn);
        _weedingBtn.gameObject.SetActive(false);//recompose
        InitialWeedBtn();
        _roleRect.localPosition = _weedBtns[_currWeedIndex].transform.localPosition;
        //显示人物对象
        _roleRect.gameObject.SetActive(true);
        roleWeedingAnimatorController = _roleRect.Find("ChuCao/chucao").GetComponent<Animator>();
        _outBtn.gameObject.SetActive(false);
        OpenTrashCan(true);
        if (_startDialogueId != 0)
        {
            _tetragonumComponent.SetStartDialogueId(_startDialogueId);
            _tetragonumComponent.Initial(() => { _tetragonumComponent.Close(); });
            _tetragonumComponent.gameObject.SetActive(true);
            _tetragonumComponent.Show();
        }
        else
        {
            _tetragonumComponent.gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// 初始化所有杂草点击按钮
    /// </summary>
    void InitialWeedBtn()
    {
        for (int i = 0; i < _weedBtns.Count; i++)
        {
            _weedBtns[i].onClick.RemoveAllListeners();
            Transform tageTra = _weedBtns[i].transform;
            _weedBtns[i].onClick.AddListener(() => { ClickWeedBtn(tageTra); });
        }
    }
    /// <summary>
    /// 开关所有杂草点击按钮
    /// </summary>
    /// <param name="isOpen"></param>
    public void OpenAllWeedBtn(bool isOpen)
    {
        for (int i = 0; i < _weedBtns.Count; i++)
        {
            _weedBtns[i].enabled = isOpen;
        }
    }
    /// <summary>
    /// 杂草按钮点击
    /// </summary>
    /// <param name="tage"></param>
    public async void ClickWeedBtn(Transform tage)
    {
        Vector3 targetPos = tage.transform.localPosition;
        var weedindex = _weedBtns.IndexOf(tage.GetComponent<Button>());
        if (weedindex == _currWeedIndex && weedCount != 0)
        {//点击当前除草区域不给予任何提示
            return;
        }
        if (isWeeding)
        {
            StaticData.CreateToastTips(ChapterTool.GetChapterFunctionString(10000541));//还没割完草
            return;
        }
        OpenAllWeedBtn(false);
        if (weedCount == 1)
        {
            createGuidance?.Invoke();
        }
        if (_roleRect.localPosition.x <= targetPos.x)
        {//向右
            if (weedindex == 1 || weedindex == 4 || weedindex == 7)
            {
                targetPos = new Vector3(targetPos.x - 65, targetPos.y, targetPos.z);//中间的草根据转向人物停止目标点修改
            }
            roleWeedingAnimatorController.SetBool("IsRight", true);
            roleWeedingAnimatorController.SetBool("IsRun", true);
        }
        else
        {//向左
            if (weedindex == 1 || weedindex == 4 || weedindex == 7)
            {
                targetPos = new Vector3(targetPos.x + 65, targetPos.y, targetPos.z);
            }
            roleWeedingAnimatorController.SetBool("IsLeft", true);
            roleWeedingAnimatorController.SetBool("IsRun", true);
        }
        _currWeedIndex = _weedBtns.IndexOf(tage.GetComponent<Button>());

        await ChapterTool.MoveUi(_roleRect, targetPos, 1, 0.5f, null, async () =>
        {
            OpenAllWeedBtn(true);
            //到达后播放待机动画
            roleWeedingAnimatorController.SetBool("IsLeft", false);
            roleWeedingAnimatorController.SetBool("IsRight", false);
            roleWeedingAnimatorController.SetBool("IsRun", false);
            //隐藏镰刀图片
            _weedBtns[weedindex].transform.Find("Image").gameObject.SetActive(false);
            //到达后自动播放锄草
            WeedingAnimaPlaying();
            //await UniTask.DelayFrame(1);
            //roleWeedingAnimatorController.SetBool("IsArrive", false);
            //除草按钮
            //if (_weedBtns[_currWeedIndex].gameObject.activeSelf)
            //{
            //    _weedingBtn.gameObject.SetActive(true);
            //}
            //else
            //{
            //    _weedingBtn.gameObject.SetActive(false);
            //}
        });
    }
    /// <summary>
    /// 点击除草按钮
    /// </summary>
    private void ClickWeedingBtn()
    {
        if (!_weedBtns[_currWeedIndex].gameObject.activeSelf)
        {
            return;
        }
        _weedingBtn.gameObject.SetActive(false);
        _weedingBtn.enabled = false;
        //播放live2D动画 TODO
        OpenAllWeedBtn(false);
        WeedingAnimaPlaying();
    }
    /// <summary>
    /// 播放除草动画中
    /// </summary>
    async void WeedingAnimaPlaying()
    {
        roleWeedingAnimatorController.SetBool("IsWeeding", true);
        isWeeding = true;
        await UniTask.Delay(awaitTime);//等待除草动画播放完
        weedCount++;
        roleWeedingAnimatorController.SetBool("IsWeeding", false);
        isWeeding = false;

        GameObject tageObj = _weedBtns[_currWeedIndex].gameObject;
        if (tageObj.activeSelf)
        {
            tageObj.SetActive(false);
            CreationAwardItem(tageObj.transform.localPosition);
            _roleRect.SetAsLastSibling();
            if (IsWeedingEnd())
            {
                _roleRect.gameObject.SetActive(false);
                _weedingBtn.gameObject.SetActive(false);
                _tetragonumComponent.SetStartDialogueId(_tipDialogueId);
                _tetragonumComponent.Initial(() => { _tetragonumComponent.Close(); });
                _tetragonumComponent.gameObject.SetActive(true);
                _tetragonumComponent.Show();
                OpenAwardDrag(true);
            }
        }

        _weedingBtn.enabled = true;
        OpenAllWeedBtn(true);
    }

    /// <summary>
    /// 是否除草完毕
    /// </summary>
    /// <returns></returns>
    bool IsWeedingEnd()
    {
        bool isEnd = true;
        for (int i = 0; i < _weedBtns.Count; i++)
        {
            GameObject weedObj = _weedBtns[i].gameObject;
            if (weedObj.activeSelf)
            {
                isEnd = false;
            }

        }
        return isEnd;
    }
    /// <summary>
    /// 开关垃圾桶
    /// </summary>
    /// <param name="isOpen"></param>
    public void OpenTrashCan(bool isOpen)
    {
        for (int i = 0; i < _trashCan.Count; i++)
        {
            _trashCan[i].gameObject.SetActive(isOpen);
        }
    }
    /// <summary>
    /// 创建除草奖励
    /// </summary>
    /// <param name="tagePoin"></param>
    void CreationAwardItem(Vector3 tagePoin)
    {
        GameObject obj = GameObject.Instantiate(_awardItem.gameObject, transform);
        UIPanelDrag item = obj.GetComponent<UIPanelDrag>();
        item.localPos = tagePoin;
        item.transform.localPosition = tagePoin;
        int index = Random.Range(0, _awardItemSprites.Count);
        Image awardIcon = item.GetComponent<Image>();
        awardIcon.sprite = _awardItemSprites[index];
        awardIcon.SetNativeSize();
        Transform trashCanTage = _trashCan[index];
        item.actionOnPointerUp = DragUp;
        if (_showAwardItemDic.ContainsKey(trashCanTage))
        {
            _showAwardItemDic[trashCanTage].Add(item);
        }
        else
        {
            List<UIPanelDrag> uIPanelDrags = new List<UIPanelDrag>();
            uIPanelDrags.Add(item);
            _showAwardItemDic.Add(trashCanTage, uIPanelDrags);
        }
        _allAwards.Add(item);
        item.gameObject.SetActive(true);
        //item.enabled = false;
        if (onece)
        {//创建引导
            createGuidance = () =>
            {
                var parfab = ABManager.GetAsset<GameObject>("ChapterGuidance");
                GameObject guidance = GameObject.Instantiate(parfab, transform);
                chapterGuidance = guidance.GetComponent<ChapterGuidance>();
                chapterGuidance.PlayGuidanceAnima(item.transform.localPosition, trashCanTage.transform.localPosition);
            };
            onece = false;
        }
    }

    private void DragUp(PointerEventData obj)
    {
        if (chapterGuidance != null)
        {
            GameObject.Destroy(chapterGuidance.gameObject);
        }
        UIPanelDrag item = obj.pointerPress.transform.GetComponent<UIPanelDrag>();
        if (item != null)
        {
            if (IsDragSucceed(item))
            {
                item.gameObject.SetActive(false);
                if (IsAllDragSuceed())
                {
                    Debug.Log("奖励收拾完毕");
                    _outBtn.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            Debug.Log("无效拖拽");
        }
    }
    /// <summary>
    /// 判断拖拽是否成功
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool IsDragSucceed(UIPanelDrag item)
    {
        Transform tageTra = GetTrashCan(item);
        bool isSucced = false;
        if (Vector3.Distance(item.transform.localPosition, tageTra.localPosition) < 300)
        {
            Debug.Log("拖拽成功");
            isSucced = true;
        }
        else
        {
            Debug.Log("拖拽失败");
            isSucced = false;
        }
        return isSucced;
    }
    /// <summary>
    /// 获取对应垃圾桶对象
    /// </summary>
    /// <param name="itemTage"></param>
    /// <returns></returns>
    public Transform GetTrashCan(UIPanelDrag itemTage)
    {
        Transform tageTra = null;
        foreach (var item in _showAwardItemDic)
        {
            if (item.Value.Contains(itemTage))
            {
                tageTra = item.Key;
            }
        }
        return tageTra;
    }
    /// <summary>
    /// 开关所有奖励拖拽
    /// </summary>
    /// <param name="isOpen"></param>
    public void OpenAwardDrag(bool isOpen)
    {
        for (int i = 0; i < _allAwards.Count; i++)
        {
            _allAwards[i].enabled = isOpen;
        }
    }
    /// <summary>
    /// 是否所有奖励收拾完毕
    /// </summary>
    /// <returns></returns>
    bool IsAllDragSuceed()
    {//如果还有草没除
        for (int i = 0; i < _weedBtns.Count; i++)//recompose
        {
            if (_weedBtns[i].gameObject.activeSelf)
            {
                return false;
            }
        }
        bool isAllDragSuceed = true;
        for (int i = 0; i < _allAwards.Count; i++)
        {
            if (_allAwards[i].gameObject.activeSelf)
            {
                isAllDragSuceed = false;
            }
        }
        return isAllDragSuceed;
    }
    /// <summary>
    /// 清理完成退出
    /// </summary>
    private async void ClickOutBtn()
    {
        if (_isFadeOut)
        {
            await ChapterTool.FadeInFadeOut(_thisCanvasGroup, 0, 0.1f, null, () =>
             {
                 gameObject.SetActive(false);
                 _endAction?.Invoke();
             });
        }
        else
        {
            gameObject.SetActive(false);
            _endAction?.Invoke();
        }

    }

    public void ShowRole()
    {
        //显示人物对象
        _roleRect.gameObject.SetActive(true);
    }
    #endregion
}
