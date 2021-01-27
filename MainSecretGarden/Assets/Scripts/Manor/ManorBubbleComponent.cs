using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManorBubbleComponent : MonoBehaviour
{
    bool isBeginAnimByTime;
    float currAnimTimeBegin;
    bool isBubbleShow;
    float currBubbleShowTime;
    public static ManorBubbleComponent _instance;
    public enum StateIdle
    {
        Idle0,
        Idle1,
        Idle2,
        Idle3
    }
    public StateIdle stateIdle;
    public TileComponent personTileComponent;
    public Animator animatorIdle;
    public Transform transPersonPointBubble;
    [HideInInspector]
    public ManorDialogueRightBubble personBubble;
    List<int> TipsRandom = new List<int>();//随机提示
    List<int> TipsPertinence = new List<int>();//针对性提示，可以重复
    int seedRandomTips;//随机种子
    System.Random randomTips;
    int maxRandom;
    private void Awake()
    {
        _instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        TipsRandom.AddRange(StaticData.configExcel.GetVertical().ManorRoleBubbleRandom);
        maxRandom = TipsRandom.Count;
        seedRandomTips = (int)TimeHelper.ClientNow();
        randomTips = new System.Random(seedRandomTips);
        currAnimTimeBegin = 0f;
        currBubbleShowTime = 0f;
        isBubbleShow = false;
        isBeginAnimByTime = true;
    }

    public void AddTipsPertinence(int idLocalize)
    {
        TipsPertinence.Add(idLocalize);
    }
    public void RemoveTipsPertinenceOne(int idLocalize)
    {
        int pos = -1;
        for (int i = 0; i < TipsPertinence.Count; i++)
        {
            if (TipsPertinence[i] == idLocalize)
            {
                pos = i;
                break;
            }
        }
        if (pos != -1)
        {
            TipsPertinence.RemoveAt(pos);
        }
    }
    public void RemoveTipsPertinenceAll(int idLocalize)
    {
        if (!TipsPertinence.Contains(idLocalize))
        {
            return;
        }
        TipsPertinence.RemoveAll((x) => x == idLocalize);
    }

    public void ResetBeginAnimByTime()//重新计时
    {
        isBeginAnimByTime = true;
    }
    public void BreakBeginAnimByTime()//打断计时
    {
        isBeginAnimByTime = false;
        CloseBubble();
    }

    // Update is called once per frame
    void Update()
    {
        if (personBubble == null)
        {
            return;
        }
        Vector2 localPoint = StaticData.ManorWorldPointToUICameraAnchorPos(transPersonPointBubble.position);
        personBubble.GetComponent<RectTransform>().anchoredPosition = localPoint;
        if (isBeginAnimByTime)
        {
            currAnimTimeBegin += Time.unscaledDeltaTime;
        }
        //设置对话状态
        if (currAnimTimeBegin >= StaticData.configExcel.GetVertical().ManorTimePlayAnimTimeInterval)
        {
            currAnimTimeBegin = 0f;
            //开始播放
            personBubble.gameObject.SetActive(true);
            //设置内容
            string content = string.Empty;
            //先取针对性的
            int idLocal = -1;
            if (TipsPertinence.Count > 0)
            {
                idLocal = TipsPertinence[TipsPertinence.Count - 1];
                content = StaticData.GetMultilingual(idLocal);
            }
            if (string.IsNullOrEmpty(content))
            {
                //随机
                int randomValue = randomTips.Next(0, maxRandom);
                idLocal = TipsRandom[randomValue];
                content = StaticData.GetMultilingual(idLocal);
            }
            personBubble.SetContent(content, scaleRadio: 1f);
            //播放角色动作
            stateIdle = (StateIdle)randomTips.Next(1, 3 + 1);//1-5
            animatorIdle.Play(stateIdle.ToString());
            isBubbleShow = true;
        }

        if (isBubbleShow)
        {
            currBubbleShowTime += Time.unscaledDeltaTime;
            if (currBubbleShowTime >= StaticData.configExcel.GetVertical().ManorTimeShowBubble)
            {
                CloseBubble();
            }
        }

    }

    

    private void CloseBubble()
    {
        currBubbleShowTime = 0f;
        isBubbleShow = false;
        //关闭气泡
        personBubble.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        if (_instance != null)
        {
            _instance = null;
        }
    }
}
