using Cysharp.Threading.Tasks;
using Game.Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static StaticData;

/// <summary>
/// 选择房间界面
/// </summary>
public class UIPartyChooseRoomController : MonoBehaviour
{
    #region 变量

    private Button _butQuicklyJoin;
    private Button _butJoin;
    private Button _butReturn;

    #endregion

    #region 属性
    #endregion

    #region 函数

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Init() 
    {
        _butQuicklyJoin = transform.Find("But_QuicklyJoin").GetComponent<Button>();
        _butJoin = transform.Find("But_Join").GetComponent<Button>();
        _butReturn = transform.Find("But_Return").GetComponent<Button>();

        _butQuicklyJoin.onClick.RemoveAllListeners();
        _butQuicklyJoin.onClick.AddListener(OnClickQuicklyJoin);

        _butJoin.onClick.RemoveAllListeners();
        _butJoin.onClick.AddListener(OnClickJoin);

        _butReturn.onClick.RemoveAllListeners();
        _butReturn.onClick.AddListener(OnClickReturn);
    }


    private List<int> _roomList = new List<int>();

    /// <summary>
    /// 初始化值
    /// </summary>
    /// <param name="roomList"></param>
    public void InitValue(SCRoomListInfo roomList) 
    {
        for (int i = 0; i < roomList.RoomInfo.Count; i++)
        {
            _roomList.Add(roomList.RoomInfo[i].RoomId);
        }
    }

    /// <summary>
    /// 点击快速加入按钮
    /// </summary>
    private void OnClickQuicklyJoin() 
    {
        PartyServerDockingManager.NotifyServerQuicklyJoinRoom(JoinRoomSuccess, JoinRoomFailed);
    }

    /// <summary>
    /// 点击加入按钮
    /// </summary>
    private void OnClickJoin()
    {
        int index = Random.Range(0, _roomList.Count);
        Debug.Log("点击加入按钮 roomid = "+ _roomList[index]);
        PartyServerDockingManager.NotifyServerEntranceRoom(_roomList[index], JoinRoomSuccess, JoinRoomFailed);  
    }

    /// <summary>
    /// 加入房间成功
    /// </summary>
    /// <param name="sCEntranceRoom"></param>
    private async void JoinRoomSuccess(SCEntranceRoom sCEntranceRoom, guessState guessState)
    {
        //获取这个阶段
        PartyGuessTime.Instance.guessState = guessState;
        //切换关卡
        await StaticData.EnterParty(sCEntranceRoom);

    }

    private void JoinRoomFailed() 
    {

    }

    /// <summary>
    /// 点击返回按钮
    /// </summary>
    private void OnClickReturn() 
    {
        //隐藏自己
        UIComponent.RemoveUI(UIType.UIPartyChooseRoom);
    }

    #endregion
}
