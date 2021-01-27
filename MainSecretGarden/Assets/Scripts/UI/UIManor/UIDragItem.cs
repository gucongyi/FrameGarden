using Company.Cfg;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//创建装饰物
public class UIDragItem : MonoBehaviour,InterfaceScrollCell
{
    public UIPanelDrag uiPanelDrag;
    public GameObject goRootTab;

    public Image ImageDecorate;
    public Text textNum;
    public Text textDes;
    List<DecorateDefine> listDecorateNotTile=new List<DecorateDefine>();
    List<Game.Protocal.CSWareHouseStruct> listDecorateMyHave;
    DecorateDefine currDecorateDefine;
    string NameModel;
    private int haveNum = 0;

    private void Awake()
    {
        uiPanelDrag.actionFromUpDrag = OnDragOutFromUp;
        uiPanelDrag.actionOnClick = OnClickUIDecorate;

        textNum.gameObject.SetActive(false);
    }
    private void OnClickUIDecorate(PointerEventData eventData)
    {
        //更新庄园装扮红点
        ManorRedDotTool.isOpenManorDecorateRedDot = false;
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.ManorDecorate);
        StaticData.CreateToastTips($"长按拖拽装饰物到场景中！");
    }

    public void ScrollCellIndex(int idx)
    {
        //获取配置文件
        listDecorateMyHave = StaticData.GetWareHouseDecorate();
        listDecorateNotTile.Clear();
        for (int i = 0; i < listDecorateMyHave.Count; i++)
        {
            listDecorateNotTile.Add(StaticData.configExcel.GetDecorateByDecorateId(listDecorateMyHave[i].GoodId));
        }

        var gameItemDefine= StaticData.configExcel.GetGameItemByID(listDecorateNotTile[idx].idGameItem);
        StaticData.DebugGreen($"listDecorateNotTile[idx].idGameItem:{listDecorateNotTile[idx].idGameItem}");
        ImageDecorate.sprite = ABManager.GetAsset<Sprite>(gameItemDefine.Icon);
        NameModel = listDecorateNotTile[idx].Model[0];
        textDes.text = listDecorateNotTile[idx].notes;
        currDecorateDefine = listDecorateNotTile[idx];
        haveNum=StaticData.GetWareHouseItem(gameItemDefine.ID).GoodNum;
        if (haveNum>0)
        {
            textNum.gameObject.SetActive(true);
            textNum.text = $"{haveNum}";
        }
    }

    

    private async void OnDragOutFromUp(PointerEventData eventData)
    {
        //更新庄园装扮红点
        ManorRedDotTool.isOpenManorDecorateRedDot = false;
        RedDotManager.UpdateRedDot(RedDotManager.RedDotKey.ManorDecorate);
        //默认装饰物是第一个下标0的模型
        var goDecorate = await Root2dSceneManager._instance.GenerateDecorate(NameModel, currDecorateDefine.DecorateId);
        goDecorate.GetComponent<TileComponent>().Init(goRootTab,currDecorateDefine.DecorateType);
        //设置回去,下次打开的时候看到已经复位
        uiPanelDrag.OnEndDrag(eventData);
        goRootTab.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
