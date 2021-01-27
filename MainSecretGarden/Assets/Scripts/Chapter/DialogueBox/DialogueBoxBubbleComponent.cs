using System;
using System.Collections;
using System.Collections.Generic;
using Company.Cfg;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 气泡形对话框
/// </summary>
public class DialogueBoxBubbleComponent : DialogueBoxBasics
{
    #region 字段

    /// <summary>
    /// 多人对话数据
    /// </summary>
    MultiDialogueData _multiDialogueData;
    /// <summary>
    /// 对话角色对象
    /// </summary>
    [SerializeField]
    List<DialogueBoxBubbleRole> _roles = new List<DialogueBoxBubbleRole>();
    /// <summary>
    /// 对话角色字典
    /// </summary>
    Dictionary<int, DialogueBoxBubbleRole> _roleDic = new Dictionary<int, DialogueBoxBubbleRole>();

    ChapterDialogueTextDefine _previousRole;

    bool _isMomentCloseOrOpen = false;
    /// <summary>
    /// 说话前是否等待
    /// </summary>
    bool _roleSpeakAeforeAwait = false;
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
    /// 设置初始id
    /// </summary>
    /// <param name="id"></param>
    public virtual void StartId(int id)
    {
        _startDialogueId = id;
    }
    /// <summary>
    /// 初始化组件
    /// </summary>
    /// <param name="endAction"></param>
    /// <param name="roleSpeakAeforeAction"></param>
    /// <param name="roleSpeakRearAction"></param>
    /// <param name="roleDialogueBoxCloseAction"></param>
    public virtual void Initial(Action endAction, Action<ChapterDialogueTextDefine> roleSpeakAeforeAction = null, Action<ChapterDialogueTextDefine> roleSpeakRearAction = null, Action<ChapterDialogueTextDefine> roleDialogueBoxCloseAction = null, Action<ChapterDialogueTextDefine> roleBoxCloseAeforeAction = null)
    {
        base.Initial(endAction, roleSpeakAeforeAction, roleSpeakRearAction, roleDialogueBoxCloseAction, roleBoxCloseAeforeAction);


        _bgButton = GetComponent<Button>();

        _bgButton.onClick.RemoveAllListeners();
        _bgButton.onClick.AddListener(() => { ClickBtn(); });
        _multiDialogueData = new MultiDialogueData(_dialogueData._Data.ID);

        for (int i = 0; i < _roles.Count; i++)
        {
            if (!_roleDic.ContainsKey(_roles[i]._RoleId))
            {
                _roleDic.Add(_roles[i]._RoleId, _roles[i]);
            }
        }
        foreach (var item in _multiDialogueData._DataDic)
        {
            if (_roleDic.ContainsKey(item.Key))
            {
                _roleDic[item.Key].Initial(item.Value);
            }
        }
        Debug.Log("对话初始化完毕");
    }
    /// <summary>
    /// 开始默认展示
    /// </summary>
    public override async void Show()
    {
        base.Show();
        gameObject.SetActive(true);
        ClickBtn();
    }
    /// <summary>
    /// 展示对话
    /// </summary>
    /// <param name="data"></param>
    public override async UniTask Show(ChapterDialogueTextDefine data)
    {
        await base.Show(data);

        if (_roleDic.ContainsKey(data.NameID))
        {
            _roleBoxCloseAeforeAction?.Invoke(_dialogueData._Data);

            if (_previousRole != null)
            {
                Debug.Log("关闭旧对话框：" + _previousRole.ID);
                await CloseShowText(_previousRole.NameID);
            }
            //if (_previousRole != null)
            //{
            //    Debug.Log("关闭旧对话框完毕：" + _previousRole.ID);
            //}

            await CloseAllShowText(data.NameID);
            //Debug.Log("关闭旧对话");
            await UniTask.WaitUntil(() => IsAllClose(data.NameID));

            //Debug.Log("关闭旧对话完毕");
            _roleSpeakAeforeAction?.Invoke(data);
            await UniTask.WaitUntil(() => !_roleSpeakAeforeAwait);

            await _roleDic[data.NameID].Show(data.ID);
            _roleSpeakRearAction?.Invoke(_dialogueData._Data);
            //await UniTask.Delay(TimeSpan.FromMilliseconds(800)); //取消说话间隔

            if (_dialogueData._Data.NextDialogId == 0)
            {
                _isEnd = true;
            }
            else
            {
                _previousRole = _dialogueData._Data;
                _dialogueData = new SingleDialogueData(_dialogueData._Data.NextDialogId);
            }
            if (_isMomentCloseOrOpen)
            {
                return;
            }
            OpenClickBtn(true);
        }
    }
    /// <summary>
    /// 关闭所有对话框
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async UniTask CloseAllShowText(int id)
    {
        foreach (var item in _roleDic)
        {
            if (id != item.Key)
            {
                item.Value.CloseShowText();
            }
        }
    }
    /// <summary>
    /// 关闭所有对话框
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async UniTask CloseAllShowText()
    {
        foreach (var item in _roleDic)
        {
            item.Value.CloseShowText();
        }
    }
    /// <summary>
    /// 关闭某个对话框
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async UniTask CloseShowText(int id)
    {
        foreach (var item in _roleDic)
        {
            if (item.Key == id)
            {
                if (item.Key != _dialogueData._Data.NameID)
                {
                    await item.Value.CloseShowText();
                    _roleDialogueBoxCloseAction?.Invoke(_dialogueData._Data);
                }

            }
        }
    }
    /// <summary>
    /// 对话框是否已经全部关闭
    /// </summary>
    /// <returns></returns>
    public bool IsAllClose(int roleId)
    {
        bool isClose = true;
        foreach (var item in _roleDic)
        {
            if (!item.Value._IsClose && item.Value._RoleId != roleId)
            {
                isClose = false;
            }
        }

        return isClose;

    }
    /// <summary>
    /// 强制关闭某个对话框
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async UniTask CompelCloseShowText(int id)
    {
        foreach (var item in _roleDic)
        {
            if (item.Key == id)
            {
                await item.Value.CloseShowText();
            }
        }
    }
    /// <summary>
    /// 点击
    /// </summary>
    public override async UniTask ClickBtn()
    {
        base.ClickBtn();
        OpenClickBtn(false);
        if (_isEnd)
        {
            _previousRole = null;
            _endAction?.Invoke();
        }
        else
        {
            await Show(_dialogueData._Data);
        }
    }
    /// <summary>
    /// 设置是否等待说话前回调
    /// </summary>
    /// <param name="isAwait"></param>
    public void SetRoleSpeakAeforeAwait(bool isAwait)
    {
        _roleSpeakAeforeAwait = isAwait;
    }
    /// <summary>
    /// 暂时开关对话
    /// </summary>
    /// <param name="isOpen"></param>
    public void MomentCloseOrOpen(bool isOpen)
    {
        _isMomentCloseOrOpen = isOpen;
    }
    public override async void Close()
    {
        await CloseAllShowText();
        base.Close();
    }
    /// <summary>
    /// 设置角色位置
    /// </summary>
    /// <param name="vector3"></param>
    /// <param name="id"></param>
    public void SetRolePoint(Vector3 vector3, int id)
    {
        if (_roleDic.ContainsKey(id))
        {
            _roleDic[id].SetRolePoint(vector3);
        }
    }
    /// <summary>
    /// 获取角色对象
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public DialogueBoxBubbleRole GetRoleData(int id)
    {
        DialogueBoxBubbleRole data = null;
        if (_roleDic.ContainsKey(id))
        {
            data = _roleDic[id];
        }
        return data;
    }
    #endregion

}
