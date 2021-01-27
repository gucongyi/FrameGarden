using Company.Cfg;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
/// <summary>
/// 章节工具类
/// </summary>
public static class ChapterTool
{
    /// <summary>
    /// 条漫组件移动方向
    /// </summary>
    public enum CaricatureModuleDirectionType
    {
        /// <summary>
        /// 向上移动
        /// </summary>
        Up,
        /// <summary>
        /// 向左
        /// </summary>
        Left,
        /// <summary>
        /// 向右
        /// </summary>
        Right
    }
    /// <summary>
    /// 加载章节管理器
    /// </summary>
    /// <param name="index"></param>
    public static async void LoadChapterManager(int index)
    {

        if (ChapterModuleManager._Instance != null)
        {
            UIComponent.RemoveUI($"ChapterManager{ChapterModuleManager._Instance._ChapterIndex - StaticData.configExcel.Section[0].SectionId}");
        }
        GameObject obj = await UIComponent.CreateUIAsync($"ChapterManager{index}");
        ChapterModuleManager._Instance = obj.GetComponent<ChapterModuleManager>();
        await ChapterModuleManager._Instance.Initial();
    }
    /// <summary>
    /// 展示当前章节
    /// </summary>
    public static async UniTask ShowChapterManager()
    {
        if (ChapterModuleManager._Instance != null)
        {
            await ChapterModuleManager._Instance.Show();
        }
    }
    /// <summary>
    /// 移动ui
    /// </summary>
    /// <param name="uiRect"></param>
    /// <param name="tagePoint"></param>
    /// <param name="speed"></param>
    /// <param name="endAction"></param>
    /// <returns></returns>
    public static async UniTask MoveUi(RectTransform uiRect, Vector2 tagePoint, float speed, float plusSpeed = 0.1f, CancellationTokenSource cancellationTokenSource = null, Action endAction = null)
    {
        float speedValue = speed;
        while (uiRect != null && Vector2.Distance(uiRect.localPosition, tagePoint) > 1)
        {

            uiRect.localPosition = Vector3.Lerp(uiRect.localPosition, tagePoint, Time.smoothDeltaTime * speedValue);
            speedValue = speedValue + plusSpeed;
            if (cancellationTokenSource == null)
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(10));
            }
            else
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(10), cancellationToken: cancellationTokenSource.Token);
            }

        }
        endAction?.Invoke();
    }
    /// <summary>
    /// 淡入淡出
    /// </summary>
    /// <param name="uiRect"></param>
    /// <param name="tagePoint"></param>
    /// <param name="speed"></param>
    /// <param name="endAction"></param>
    /// <returns></returns>
    public static async UniTask FadeInFadeOut(CanvasGroup uiCanvasGroup, float tageValue, float speed, CancellationTokenSource cancellationTokenSource = null, Action endAction = null, float addSpeed = 0.02f)
    {
        bool plusOrMinus = uiCanvasGroup.alpha < tageValue;

        if (plusOrMinus)
        {
            while (uiCanvasGroup.alpha < tageValue)
            {
                addSpeed += Time.deltaTime;
                uiCanvasGroup.alpha = uiCanvasGroup.alpha + speed + addSpeed;
                if (cancellationTokenSource == null)
                {
                    await UniTask.Delay(TimeSpan.FromMilliseconds(100));
                }
                else
                {
                    await UniTask.Delay(TimeSpan.FromMilliseconds(100), cancellationToken: cancellationTokenSource.Token);
                }
            }
        }
        else
        {
            while (uiCanvasGroup.alpha > tageValue)
            {
                addSpeed += Time.deltaTime;
                uiCanvasGroup.alpha = uiCanvasGroup.alpha - speed - addSpeed;

                if (cancellationTokenSource == null)
                {
                    await UniTask.Delay(TimeSpan.FromMilliseconds(100));
                }
                else
                {
                    await UniTask.Delay(TimeSpan.FromMilliseconds(100), cancellationToken: cancellationTokenSource.Token);
                }
            }

        }

        endAction?.Invoke();
    }
    /// <summary>
    /// 修改ui尺寸
    /// </summary>
    /// <param name="uiRect"></param>
    /// <param name="tageScale"></param>
    /// <param name="speed"></param>
    /// <param name="endAction"></param>
    /// <returns></returns>
    public static async UniTask ChangeUiScale(RectTransform uiRect, Vector3 tageScale, float speed, float refreshRate = 10, CancellationTokenSource cancellationTokenSource = null, Action endAction = null)
    {
        while (uiRect != null && Vector2.Distance(uiRect.localScale, tageScale) > 0.01f)
        {
            uiRect.localScale = Vector2.MoveTowards(uiRect.localScale, tageScale, speed);

            if (cancellationTokenSource == null)
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(refreshRate));
            }
            else
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(refreshRate), cancellationToken: cancellationTokenSource.Token);
            }

        }
        Debug.Log("尺寸修改完毕");
        endAction?.Invoke();
        //endAction = null;
    }
    public static async UniTask ChangeUiScale(List<RectTransform> uiRects, Vector3 tageScale, float speed, float refreshRate = 10, CancellationTokenSource cancellationTokenSource = null, Action endAction = null)
    {
        while (uiRects[0] != null && Vector2.Distance(uiRects[0].localScale, tageScale) > 0.01f)
        {
            for (int i = 0; i < uiRects.Count; i++)
            {
                uiRects[i].localScale = Vector2.MoveTowards(uiRects[i].localScale, tageScale, speed);

            }

            if (cancellationTokenSource == null)
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(refreshRate));
            }
            else
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(refreshRate), cancellationToken: cancellationTokenSource.Token);
            }

        }
        Debug.Log("尺寸修改完毕");
        endAction?.Invoke();
        //endAction = null;
    }
    /// <summary>
    /// 修改ui尺寸
    /// </summary>
    /// <param name="uiRect"></param>
    /// <param name="tageSize"></param>
    /// <param name="speed"></param>
    /// <param name="endAction"></param>
    /// <returns></returns>
    public static async UniTask ChangeUiSize(RectTransform uiRect, Vector3 tageSize, float speed, float refreshRate = 10, CancellationTokenSource cancellationTokenSource = null, Action endAction = null)
    {

        while (Vector2.Distance(uiRect.sizeDelta, tageSize) > 0.01f)
        {
            uiRect.sizeDelta = Vector2.MoveTowards(uiRect.sizeDelta, tageSize, speed);
            if (cancellationTokenSource == null)
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(refreshRate));
            }
            else
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(refreshRate), cancellationToken: cancellationTokenSource.Token);
            }
        }
        endAction?.Invoke();

    }
    /// <summary>
    /// 打印机
    /// </summary>
    /// <param name="tageText"></param>
    /// <param name="showStr"></param>
    /// <param name="endAction"></param>
    /// <returns></returns>
    public static async UniTask PrinTing(Text tageText, string showStr, CancellationTokenSource cancellationTokenSource = null, Action endAction = null)
    {
        char[] charArray = showStr.ToCharArray();
        tageText.text = "";
        int index = 0;
        while (index < charArray.Length)
        {
            tageText.text += charArray[index];
            index = index + 1;
            if (cancellationTokenSource == null)
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(70));
            }
            else
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(70), cancellationToken: cancellationTokenSource.Token);
            }
        }
        endAction?.Invoke();
    }
    /// <summary>
    /// 闪烁
    /// 2019/10/24 huangjiangdong
    /// </summary>
    /// <param name="tageCanvasGroups"></param>
    /// <param name="refreshRate"></param>
    /// <param name="speed"></param>
    /// <param name="index">闪烁次数 默认没有次数限制</param>
    /// <param name="endAction">结束回调</param>
    /// <returns></returns>
    public static async UniTask Glint(List<CanvasGroup> tageCanvasGroups, float refreshRate, float speed, int index = 0, Action endAction = null)
    {
        int glintIndex = 0;

        while (tageCanvasGroups != null && tageCanvasGroups.Count > 0)
        {
            CanvasGroup tage = tageCanvasGroups[0];
            if (tage.alpha > 0)
            {
                while (tage.alpha > 0)
                {
                    for (int i = 0; i < tageCanvasGroups.Count; i++)
                    {
                        tageCanvasGroups[i].alpha = tageCanvasGroups[i].alpha - speed;
                    }
                    await UniTask.Delay(TimeSpan.FromMilliseconds(refreshRate));
                }

            }
            else
            {
                while (tage.alpha < 1)
                {
                    for (int i = 0; i < tageCanvasGroups.Count; i++)
                    {
                        tageCanvasGroups[i].alpha = tageCanvasGroups[i].alpha + speed;
                    }
                    await UniTask.Delay(TimeSpan.FromMilliseconds(refreshRate));
                }
            }
            glintIndex++;
            await UniTask.Delay(TimeSpan.FromMilliseconds(refreshRate));
            if (index != 0)
            {
                if (glintIndex / 2 == index)
                {
                    //Debug.Log("闪烁完毕！");
                    endAction?.Invoke();
                    break;
                }
            }
        }

    }
    /// <summary>
    /// 获取文本
    /// </summary>
    /// <returns></returns>
    public static SingleDialogueData GetDialogue(int id)
    {
        SingleDialogueData dialogueData = new SingleDialogueData(id);
        return dialogueData;
    }
    /// <summary>
    /// 获取章节功能文字（角色名，章节名，功能文字）
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string GetChapterFunctionString(int id)
    {
        ChapterFunctionTextDefine data = StaticData.configExcel.GetChapterFunctionTextByID(id);
        string str = "";
        if (data == null)
        {
            return str;
        }
        switch (StaticData.linguisticType)
        {
            case LinguisticType.Simplified:
                str = data.SimplifiedChinese;
                break;
            case LinguisticType.Complex:
                str = data.TraditionalChinese;
                break;
            case LinguisticType.English:
                str = data.English;
                break;
            default:
                str = data.SimplifiedChinese;
                break;
        }
        str = string.Format(str, StaticData.playerInfoData.userInfo.Name);
        string strTwo = SetLineFeed(str, data.StringNumber);
        return strTwo;
    }
    /// <summary>
    /// 获取章节内容文字
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static ChapterDialogueTextDefine GetChapterData(int id)
    {
        ChapterDialogueTextDefine data = StaticData.configExcel.GetChapterDialogueTextByID(id);
        return data;
    }
    /// <summary>
    /// 获取剧情对话文本内容
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string GetDialogueString(ChapterDialogueTextDefine data)
    {
        string str = "";
        Debug.Log(data.ID);
        switch (StaticData.linguisticType)
        {
            case LinguisticType.Simplified:
                str = data.SimplifiedChinese;
                break;
            case LinguisticType.Complex:
                str = data.TraditionalChinese;
                break;
            case LinguisticType.English:
                str = data.English;
                break;
            default:
                str = data.SimplifiedChinese;
                break;
        }
        str = string.Format(str, StaticData.playerInfoData.userInfo.Name);
        string strTwo = SetLineFeed(str, data.StringNumber);
        if (data.IsImage)
        {
            strTwo = str;
        }
        return strTwo;
    }
    /// <summary>
    /// 设置换行符
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string SetLineFeed(string str, int maxNumber)
    {
        char[] charArray = str.ToCharArray();

        if (maxNumber == -1)
        {
            maxNumber = charArray.Length;
        }
        else if (maxNumber == 0)
        {
            maxNumber = StaticData.configExcel.GetVertical().ChapterStringNumber;
        }

        int index = 1;
        string strTwo = "";
        for (int i = 0; i < charArray.Length; i++)
        {
            char ca = charArray[i];
            if (i < charArray.Length - 1 && index >= maxNumber)
            {
                strTwo = strTwo + ca + "\n";
                index = 1;
            }
            else
            {
                strTwo = strTwo + ca;
                index++;
            }

        }
        return strTwo;
    }
    /// <summary>
    /// 获取剧情对话文本内容
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string GetDialogueString(int dataId)
    {
        ChapterDialogueTextDefine data = GetChapterData(dataId);
        string str = "";
        if (data != null)
        {
            str = GetDialogueString(data);
        }

        return str;
    }
    /// <summary>
    /// 获取章节人物头像
    /// </summary>
    /// <param name="id">人物id</param>
    /// <returns></returns>
    public static async Task<Sprite> GetChapterRoleIconAsync(int id)
    {
        Sprite icon = null;
        ChapterFunctionTextDefine data = StaticData.configExcel.GetChapterFunctionTextByID(id);
        if (data != null)
        {
            icon = await ABManager.GetAssetAsync<Sprite>(data.IconImage);
        }

        return icon;
    }
    /// <summary>
    /// 获取章节对话图片
    /// </summary>
    /// <param name="id">人物id</param>
    /// <returns></returns>
    public static async Task<Sprite> GetChapterShowImageAsync(int id)
    {
        Sprite icon = null;
        ChapterDialogueTextDefine data = GetChapterData(id);
        if (data != null)
        {
            icon = await ABManager.GetAssetAsync<Sprite>(GetDialogueString(data));
        }
        return icon;
    }
    /// <summary>
    /// 开启微信聊天
    /// </summary>
    /// <param name="starId"></param>
    /// <param name="protagonistId"></param>
    /// <param name="anotherId"></param>
    /// <param name="endAction"></param>
    public static async UniTask OpenDialogueBoxWeChat(int starId, int protagonistId, int anotherId, Action endAction)
    {
        GameObject obj = await UIComponent.CreateUIAsync(UIType.DialogueBox_WeChat);
        DialogueBoxWeChatComponent dialogueBoxWeChatComponent = obj.GetComponent<DialogueBoxWeChatComponent>();
        dialogueBoxWeChatComponent.Initial(endAction);
        dialogueBoxWeChatComponent.SetInitialData(starId, protagonistId, anotherId);
    }
}
/// <summary>
/// 章节模块数据
/// </summary>
[Serializable]
public class ChapterModule
{
    #region 字段
    /// <summary>
    /// 模块实列名字
    /// </summary>
    [SerializeField]
    string _chapterModuleName;
    /// <summary>
    /// 是否预加载
    /// </summary>
    [SerializeField]
    bool _isPrestrain = false;
    /// <summary>
    /// 下一步下标
    /// </summary>
    [SerializeField]
    int _nextStepIndex;
    /// <summary>
    /// 下一步 回调
    /// </summary>
    Action _nextStepAction;
    /// <summary>
    /// 模块对应实列
    /// </summary>
    ChapterControllerBasics _chapterControllerBasics;
    #endregion
    #region 属性
    /// <summary>
    /// 是否预加载
    /// </summary>
    public bool _IsPrestrain { get { return _isPrestrain; } }
    /// <summary>
    /// 下一步下标
    /// </summary>
    public int _NextStepIndex { get { return _nextStepIndex; } }
    #endregion
    #region 函数
    /// <summary>
    /// 初始
    /// </summary>
    public void Initial(Action nextStepAction)
    {
        _nextStepAction = nextStepAction;
    }
    /// <summary>
    /// 创建模块实列
    /// </summary>
    public async UniTask Creation()
    {
        var prefabricate = await ABManager.GetAssetAsync<GameObject>(_chapterModuleName);
        GameObject obj = GameObject.Instantiate(prefabricate, ChapterModuleManager._Instance.transform);
        _chapterControllerBasics = obj.GetComponent<ChapterControllerBasics>();
        _chapterControllerBasics._nextCallback = _nextStepAction;
        _chapterControllerBasics._setNextIndex = SetNextIndex;
        _chapterControllerBasics.transform.SetAsFirstSibling();
    }
    /// <summary>
    /// 设置下一步跳转id
    /// </summary>
    /// <param name="index"></param>
    private void SetNextIndex(int index)
    {
        _nextStepIndex = index;
    }

    /// <summary>
    /// 展示模块
    /// </summary>
    public async UniTask Show()
    {
        if (_chapterControllerBasics == null)
        {
            await Creation();
        }
        _chapterControllerBasics.Initial();
    }
    /// <summary>
    /// 重置
    /// </summary>
    public void ResetModule()
    {
        _chapterControllerBasics.ResetModule();
    }
    /// <summary>
    /// 卸载
    /// </summary>
    public void Dispose()
    {
        if (_chapterControllerBasics == null)
        {
            return;
        }
        _chapterControllerBasics.Dispose();
    }
    /// <summary>
    /// 下一步
    /// </summary>
    public void NextStep()
    {

    }
    /// <summary>
    /// 设置下一步对象下标
    /// </summary>
    /// <param name="index"></param>
    public void SetNextStepIndex(int index)
    {
        _nextStepIndex = index;
    }
    #endregion
}
/// <summary>
/// 对话数据
/// </summary>
public class SingleDialogueData
{
    #region 字段
    /// <summary>
    /// 配置数据
    /// </summary>
    ChapterDialogueTextDefine _data;
    /// <summary>
    /// 功能文本数据
    /// </summary>
    ChapterFunctionTextDefine _functionData;
    /// <summary>
    /// 是否是结束
    /// </summary>
    bool _isEnd = false;
    /// <summary>
    /// 是否是分支
    /// </summary>
    bool _isBranch = false;
    /// <summary>
    /// 是否为功能文本
    /// </summary>
    bool _isFunction = false;
    #endregion
    #region 属性
    /// <summary>
    /// 配置数据
    /// </summary>
    public ChapterDialogueTextDefine _Data { get { return _data; } }
    /// <summary>
    /// 功能文本数据
    /// </summary>
    public ChapterFunctionTextDefine _FunctionData { get { return _functionData; } }
    /// <summary>
    /// 是否是结束
    /// </summary>
    public bool _IsEnd { get { return _isEnd; } }
    /// <summary>
    /// 是否是分支
    /// </summary>
    public bool _IsBranch { get { return _isBranch; } }
    #endregion
    #region 函数

    public SingleDialogueData()
    {

    }
    public SingleDialogueData(int id, bool isFunction = false)
    {
        _isFunction = isFunction;
        if (isFunction)
        {
            GetFunctionData(id);
        }
        else
        {
            GetData(id);
        }

        if (_data == null)
        {
            _isEnd = true;
            return;
        }
        _isEnd = _data.NextDialogId == 0 && (_data.DialogIdList == null || _data.DialogIdList.Count == 0);
        _isBranch = _data.DialogIdList != null && _data.DialogIdList.Count > 0;
    }
    /// <summary>
    /// 获取配置数据
    /// </summary>
    /// <param name="id"></param>
    void GetData(int id)
    {
        _data = StaticData.configExcel.GetChapterDialogueTextByID(id);
    }
    /// <summary>
    /// 获取配置数据
    /// </summary>
    /// <param name="id"></param>
    void GetFunctionData(int id)
    {
        _functionData = StaticData.configExcel.GetChapterFunctionTextByID(id);
    }
    #endregion
}
/// <summary>
/// 多人对话
/// </summary>
public class MultiDialogueData
{
    #region 字段
    /// <summary>
    /// 对话数据字典
    /// </summary>
    Dictionary<int, Dictionary<int, ChapterDialogueTextDefine>> _dataDic = new Dictionary<int, Dictionary<int, ChapterDialogueTextDefine>>();
    /// <summary>
    /// 等待获取数据自字典
    /// </summary>
    List<int> _waitGetIds = new List<int>();

    #endregion
    #region 属性
    public Dictionary<int, Dictionary<int, ChapterDialogueTextDefine>> _DataDic { get { return _dataDic; } }

    #endregion
    #region 函数
    public MultiDialogueData(int id)
    {
        _waitGetIds.Clear();
        GetData(id);
    }
    public void GetAllData(int id)
    {
        GetData(id);
    }
    public void GetData(int id)
    {
        ChapterDialogueTextDefine data = StaticData.configExcel.GetChapterDialogueTextByID(id);
        if (_dataDic.ContainsKey(data.NameID))
        {
            if (!_dataDic[data.NameID].ContainsKey(data.ID))
            {
                _dataDic[data.NameID].Add(data.ID, data);
            }
            else
            {
                _dataDic[data.NameID][data.ID] = data;
            }
        }
        else
        {
            Dictionary<int, ChapterDialogueTextDefine> keyValuePairs = new Dictionary<int, ChapterDialogueTextDefine>();
            keyValuePairs.Add(data.ID, data);
            _dataDic.Add(data.NameID, keyValuePairs);
        }

        for (int i = 0; i < data.DialogIdList.Count; i++)
        {
            ChapterDialogueTextDefine dataBranch = StaticData.configExcel.GetChapterDialogueTextByID(data.DialogIdList[i]);
            GetData(dataBranch.ID);
        }

        if (data.NextDialogId != 0)
        {
            GetData(data.NextDialogId);
        }

    }
    #endregion
}
/// <summary>
/// 多人对话人物角色对象
/// </summary>
[Serializable]
public class DialogueBoxBubbleRole
{
    #region  字段
    /// <summary>
    /// 人物id
    /// </summary>
    [SerializeField]
    int _roleId;
    /// <summary>
    /// 人物对应实列
    /// </summary>
    [SerializeField]
    RectTransform _roleRect;
    /// <summary>
    /// 背景框
    /// </summary>
    RectTransform _boxBgRect;
    /// <summary>
    /// 气泡形背景框
    /// </summary>
    RectTransform _showBubbleBox;
    /// <summary>
    /// 气泡形
    /// </summary>
    RectTransform _bubbleRect;
    /// <summary>
    /// 气泡形文字展示text
    /// </summary>
    Text _showBubbleText;

    /// <summary>
    /// 方形背景框
    /// </summary>
    RectTransform _showTetragonumBox;
    /// <summary>
    /// 三角形
    /// </summary>
    RectTransform _triangleRect;
    /// <summary>
    /// 方形文字展示
    /// </summary>
    Text _showTetragonumText;
    /// <summary>
    /// 方形图片展示
    /// </summary>
    Image _showImage;
    /// <summary>
    /// 数据字典
    /// </summary>
    Dictionary<int, ChapterDialogueTextDefine> _dataDic = new Dictionary<int, ChapterDialogueTextDefine>();

    bool _isClose = true;
    #endregion
    #region 属性
    /// <summary>
    /// 人物id
    /// </summary>
    public int _RoleId { get { return _roleId; } }
    /// <summary>
    /// 人物对应实列
    /// </summary>
    public RectTransform _RoleRect { get { return _roleRect; } }

    public bool _IsClose { get { return _isClose; } }
    #endregion
    #region 函数
    /// <summary>
    /// 初始化组件
    /// </summary>
    /// <param name="dataDic"></param>
    public void Initial(Dictionary<int, ChapterDialogueTextDefine> dataDic)
    {
        _boxBgRect = _roleRect.Find("BoxBg").GetComponent<RectTransform>();
        _showBubbleBox = _boxBgRect.Find("BubbleBox").GetComponent<RectTransform>();
        _bubbleRect = _boxBgRect.Find("Bubble").GetComponent<RectTransform>();
        _showBubbleText = _showBubbleBox.Find("Text").GetComponent<Text>();
        GameObject imageObj = _roleRect.parent.Find("Image").gameObject;
        _showImage = _roleRect.parent.Find("Image").GetComponent<Image>();
        _showImage = GameObject.Instantiate(imageObj, _roleRect).GetComponent<Image>();
        _showTetragonumBox = _boxBgRect.Find("TetragonumBox").GetComponent<RectTransform>();
        _triangleRect = _boxBgRect.Find("Triangle").GetComponent<RectTransform>();
        _showTetragonumText = _showTetragonumBox.Find("Text").GetComponent<Text>();
        _dataDic = dataDic;
    }
    /// <summary>
    /// 展示对话
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async UniTask Show(int id)
    {
        if (_dataDic.ContainsKey(id))
        {
            VerticalLayoutGroup verticalLayoutGroup = SetBoxType(_dataDic[id].BoxType, _dataDic[id].BoxAdditional);
            if (_dataDic[id].IsImage)
            {
                verticalLayoutGroup.childControlWidth = false;
                verticalLayoutGroup.childControlHeight = false;
                Image showImage = GetImage(_dataDic[id].BoxType);
                if (showImage != null)
                {
                    showImage.gameObject.SetActive(true);
                    showImage.sprite = await ChapterTool.GetChapterShowImageAsync(_dataDic[id].ID);
                    showImage.SetNativeSize();
                    showImage.transform.localScale = Vector3.one;
                }

            }
            else
            {
                verticalLayoutGroup.childControlWidth = true;
                verticalLayoutGroup.childControlHeight = true;
                Text shwoText = GetText(_dataDic[id].BoxType);
                if (shwoText != null)
                {
                    shwoText.gameObject.SetActive(true);
                    shwoText.text = ChapterTool.GetDialogueString(_dataDic[id]);
                }

            }
            _boxBgRect.gameObject.SetActive(true);

            RectTransform tageBox = verticalLayoutGroup.transform.GetComponent<RectTransform>();

            RefreshGroup(_dataDic[id].BoxType);
            if (_isClose)
            {
                tageBox.localScale = Vector3.zero;
                List<RectTransform> tages = new List<RectTransform>();

                if (_bubbleRect.gameObject.activeSelf)
                {
                    _bubbleRect.localScale = Vector3.zero;
                    tages.Add(_bubbleRect);
                }
                else if (_triangleRect.gameObject.activeSelf)
                {
                    _triangleRect.localScale = Vector3.zero;
                    tages.Add(_triangleRect);
                }
                tages.Add(tageBox);
                await ChapterTool.ChangeUiScale(tages, Vector3.one, 0.1f, 5);
                _isClose = false;
            }
        }
    }
    /// <summary>
    /// 强制刷新布局组件
    /// </summary>
    public void RefreshGroup(DialogueType dialogueType)
    {
        switch (dialogueType)
        {
            case DialogueType.Normal:
                LayoutRebuilder.ForceRebuildLayoutImmediate(_showTetragonumBox);
                _boxBgRect.sizeDelta = _showTetragonumBox.sizeDelta;
                break;
            case DialogueType.Fantasy:
                LayoutRebuilder.ForceRebuildLayoutImmediate(_showBubbleBox);
                _boxBgRect.sizeDelta = _showBubbleBox.sizeDelta;
                break;
        }
    }
    /// <summary>
    /// 获取展示的text
    /// </summary>
    /// <param name="dialogueType"></param>
    public Text GetText(DialogueType dialogueType)
    {
        Text showText = null;
        switch (dialogueType)
        {
            case DialogueType.Normal:
                showText = _showTetragonumText;
                break;
            case DialogueType.Fantasy:
                showText = _showBubbleText;
                break;
        }
        return showText;
    }
    /// <summary>
    /// 获取展示的Iamge
    /// </summary>
    /// <param name="dialogueType"></param>
    public Image GetImage(DialogueType dialogueType)
    {

        switch (dialogueType)
        {
            case DialogueType.Normal:
                _showImage.transform.SetParent(_showTetragonumBox);
                break;
            case DialogueType.Fantasy:
                _showImage.transform.SetParent(_showBubbleBox);
                break;
        }
        return _showImage;
    }
    /// <summary>
    /// 设置对话框样式
    /// </summary>
    /// <param name="dialogueType"></param>
    public VerticalLayoutGroup SetBoxType(DialogueType dialogueType, bool isHaveCorner)
    {
        VerticalLayoutGroup verticalLayoutGroup = null;
        _triangleRect.gameObject.SetActive(false);
        _bubbleRect.gameObject.SetActive(false);
        _showImage.gameObject.SetActive(false);
        _showTetragonumText.gameObject.SetActive(false);
        _showBubbleText.gameObject.SetActive(false);
        switch (dialogueType)
        {
            case DialogueType.Normal:
                _showTetragonumBox.gameObject.SetActive(true);
                _showBubbleBox.gameObject.SetActive(false);
                _triangleRect.gameObject.SetActive(isHaveCorner);
                verticalLayoutGroup = _showTetragonumBox.GetComponent<VerticalLayoutGroup>();
                break;
            case DialogueType.Fantasy:
                _showTetragonumBox.gameObject.SetActive(false);
                _showBubbleBox.gameObject.SetActive(true);
                _bubbleRect.gameObject.SetActive(isHaveCorner);
                verticalLayoutGroup = _showBubbleBox.GetComponent<VerticalLayoutGroup>();
                break;
        }
        return verticalLayoutGroup;
    }
    /// <summary>
    /// 关闭人物对话框
    /// </summary>
    /// <returns></returns>
    public async UniTask CloseShowText()
    {
        if (_boxBgRect.gameObject.activeSelf)
        {
            RectTransform tageBox = null;
            if (_showBubbleBox.gameObject.activeSelf)
            {
                tageBox = _showBubbleBox;
            }
            else if (_showTetragonumBox.gameObject.activeSelf)
            {
                tageBox = _showTetragonumBox;
            }

            List<RectTransform> tages = new List<RectTransform>();

            if (_bubbleRect.gameObject.activeSelf)
            {
                //ChapterTool.ChangeUiScale(_bubbleRect, Vector3.zero, 0.1f, 1);
                tages.Add(_bubbleRect);
            }
            else if (_triangleRect.gameObject.activeSelf)
            {
                //ChapterTool.ChangeUiScale(_triangleRect, Vector3.zero, 0.1f, 1);
                tages.Add(_triangleRect);
            }
            tages.Add(tageBox);

            await ChapterTool.ChangeUiScale(tages, Vector3.zero, 0.1f, 5, new CancellationTokenSource(), () =>
            {
                if (_boxBgRect != null)
                {
                    _boxBgRect.gameObject.SetActive(false);
                }
            });
            _isClose = true;
        }
    }
    /// <summary>
    /// 设置人物对象位置
    /// </summary>
    /// <param name="vector3"></param>
    public void SetRolePoint(Vector3 vector3)
    {
        _roleRect.localPosition = vector3;
    }
    #endregion
}
/// <summary>
/// 文字按钮
/// </summary>
public class DialogueBoxWeChatTextBtnItem
{
    #region 字段
    /// <summary>
    /// 按钮实列
    /// </summary>
    RectTransform _tra;
    /// <summary>
    /// 点击按钮
    /// </summary>
    Button _clickBtn;
    /// <summary>
    /// 文字展示
    /// </summary>
    Text _showText;
    /// <summary>
    /// 按钮数据
    /// </summary>
    ChapterDialogueTextDefine _data;
    /// <summary>
    /// 按钮点击回调
    /// </summary>
    Action<ChapterDialogueTextDefine> _clickBtnAction;
    #endregion
    #region 函数
    /// <summary>
    /// 初始化按钮
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="data"></param>
    /// <param name="clickBtnAction"></param>
    public void Initial(RectTransform rect, ChapterDialogueTextDefine data, Action<ChapterDialogueTextDefine> clickBtnAction)
    {
        _tra = rect;
        _clickBtn = _tra.GetComponent<Button>();
        _showText = _tra.Find("Text").GetComponent<Text>();
        _data = data;
        _clickBtnAction = clickBtnAction;
        _clickBtn.onClick.RemoveAllListeners();
        _clickBtn.onClick.AddListener(ClickBtn);
        ShowText();
    }
    /// <summary>
    /// 展示数据
    /// </summary>
    public void ShowText()

    {
        _tra.gameObject.SetActive(true);
        _showText.text = ChapterTool.GetDialogueString(_data);

        LayoutRebuilder.ForceRebuildLayoutImmediate(_tra);
    }
    /// <summary>
    /// 点击按钮
    /// </summary>
    public void ClickBtn()
    {
        _clickBtnAction?.Invoke(_data);
    }
    /// <summary>
    /// 卸载按钮
    /// </summary>
    public void Dispose()
    {
        _data = null;
        GameObject.Destroy(_tra.gameObject);
    }
    #endregion

}
/// <summary>
/// 图片按钮
/// </summary>
public class DialogueBoxWeChatImageBtnItem
{
    #region 字段
    /// <summary>
    /// 按钮实列
    /// </summary>
    RectTransform _tra;
    /// <summary>
    /// 点击按钮
    /// </summary>
    Button _clickBtn;
    /// <summary>
    /// 按钮数据
    /// </summary>
    ChapterDialogueTextDefine _data;
    /// <summary>
    /// 展示图片
    /// </summary>
    Image _showImage;
    /// <summary>
    /// 按钮点击回调
    /// </summary>
    Action<ChapterDialogueTextDefine> _clickBtnAction;
    #endregion
    #region 函数
    /// <summary>
    /// 初始化按钮
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="data"></param>
    /// <param name="clickBtnAction"></param>
    public void Initial(RectTransform rect, ChapterDialogueTextDefine data, Action<ChapterDialogueTextDefine> clickBtnAction)
    {
        _tra = rect;
        _clickBtn = _tra.GetComponent<Button>();
        _showImage = _tra.GetComponent<Image>();

        _data = data;
        _clickBtnAction = clickBtnAction;
        _clickBtn.onClick.RemoveAllListeners();
        _clickBtn.onClick.AddListener(ClickBtn);
        ShowText();
    }
    /// <summary>
    /// 展示数据
    /// </summary>
    public async void ShowText()
    {
        _tra.gameObject.SetActive(true);
        _showImage.sprite = await ChapterTool.GetChapterShowImageAsync(_data.ID);
        _showImage.SetNativeSize();
        await UniTask.DelayFrame(1);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_tra);
    }
    /// <summary>
    /// 点击按钮
    /// </summary>
    public void ClickBtn()
    {
        _clickBtnAction?.Invoke(_data);
    }
    /// <summary>
    /// 卸载按钮
    /// </summary>
    public void Dispose()
    {
        _data = null;
        GameObject.Destroy(_tra.gameObject);
    }
    #endregion

}
/// <summary>
/// 聊天信息
/// </summary>
public class DialogueBoxWeChatMessageItem
{
    #region 字段
    /// <summary>
    /// item 对象实列
    /// </summary>
    RectTransform _tra;
    /// <summary>
    /// 大盒子排序组件
    /// </summary>
    HorizontalLayoutGroup _traGroup;
    /// <summary>
    /// 头像盒子
    /// </summary>
    RectTransform _avatarRect;
    /// <summary>
    /// 头像按钮
    /// </summary>
    Button _iconBtn;
    /// <summary>
    /// 头像
    /// </summary>
    Image _icon;
    /// <summary>
    /// 文本盒子
    /// </summary>
    RectTransform _contentRect;
    /// <summary>
    /// 文本背景
    /// </summary>
    RectTransform _bgRect;
    /// <summary>
    /// 文本背景图片
    /// </summary>
    Image _bgImage;
    /// <summary>
    /// 文本
    /// </summary>
    RectTransform _showTextRect;
    /// <summary>
    /// 文本显示
    /// </summary>
    Text _showText;
    Button _imageBtn;
    LayoutElement _showLayouElement;
    /// <summary>
    /// 文本展示
    /// </summary>
    Image _showImag;
    /// <summary>
    /// item数据
    /// </summary>
    ChapterDialogueTextDefine _data;
    /// <summary>
    /// 头像点击回调
    /// </summary>
    Action<ChapterDialogueTextDefine> _clickIconAction;
    Action<ChapterDialogueTextDefine> _imageClickAction;
    /// <summary>
    /// 是否是主角
    /// </summary>
    bool _isProtagonist = false;
    #endregion
    #region 函数

    public void Initial(RectTransform tra, bool isProtagonist, ChapterDialogueTextDefine data, Action<ChapterDialogueTextDefine> clickIconAction, Action<ChapterDialogueTextDefine> imageClick)
    {

        _tra = tra;
        _traGroup = _tra.GetComponent<HorizontalLayoutGroup>();
        _avatarRect = _tra.Find("Avatar").GetComponent<RectTransform>();
        _iconBtn = _avatarRect.GetComponent<Button>();
        _icon = _iconBtn.transform.Find("Icon").GetComponent<Image>();
        _contentRect = _tra.Find("Content").GetComponent<RectTransform>();
        _bgRect = _contentRect.Find("Bg").GetComponent<RectTransform>();
        _bgImage = _bgRect.GetComponent<Image>();
        _showTextRect = _bgRect.Find("ShowText").GetComponent<RectTransform>();
        _showText = _bgRect.Find("ShowText").GetComponent<Text>();
        _showLayouElement = _showTextRect.GetComponent<LayoutElement>();
        _showImag = _bgRect.Find("Image").GetComponent<Image>();
        _imageBtn = _showImag.GetComponent<Button>();
        _data = data;
        _isProtagonist = isProtagonist;
        _clickIconAction = clickIconAction;
        _imageClickAction = imageClick;
        _iconBtn.onClick.RemoveAllListeners();
        _iconBtn.onClick.AddListener(ClickIcon);
        _imageBtn.onClick.RemoveAllListeners();
        _imageBtn.onClick.AddListener(ClickImage);
        ShowData();
    }
    /// <summary>
    /// 点击头像
    /// </summary>
    private void ClickIcon()
    {
        Debug.Log("点击聊天头像");
    }
    /// <summary>
    /// 点击图片
    /// </summary>
    public void ClickImage()
    {
        _imageClickAction?.Invoke(_data);
    }
    /// <summary>
    /// 展示数据
    /// </summary>
    public async void ShowData()
    {
        if (_data != null)
        {
            _tra.gameObject.SetActive(true);
            _icon.sprite = await ChapterTool.GetChapterRoleIconAsync(_data.NameID);
            if (_data.IsImage)
            {
                _showTextRect.gameObject.SetActive(false);
                _showImag.gameObject.SetActive(true);
                _showImag.sprite = await ABManager.GetAssetAsync<Sprite>(ChapterTool.GetDialogueString(_data.ID));
                _showImag.SetNativeSize();

                _imageBtn.enabled = _data.IsClick;


            }
            else
            {
                _showImag.gameObject.SetActive(false);
                _showTextRect.gameObject.SetActive(true);
                _showText.text = ChapterTool.GetDialogueString(_data.ID);
            }

            Sprite bgSprite = null;
            if (_isProtagonist)
            {
                bgSprite = await ABManager.GetAssetAsync<Sprite>("liaotian_yuansu3");
                _traGroup.childAlignment = TextAnchor.UpperRight;
                _contentRect.transform.SetAsFirstSibling();
            }
            else
            {
                bgSprite = await ABManager.GetAssetAsync<Sprite>("liaotian_yuansu4");
                _traGroup.childAlignment = TextAnchor.UpperLeft;
                _avatarRect.transform.SetAsFirstSibling();
            }

            _bgImage.sprite = bgSprite;

            LayoutRebuilder.ForceRebuildLayoutImmediate(_showTextRect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_bgRect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_avatarRect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_tra);
            if (!_data.IsImage)
            {

                if (_showTextRect.rect.width > 700)
                {
                    _showLayouElement.enabled = true;
                }
                else
                {
                    _showLayouElement.enabled = false;
                }

            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_showTextRect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_bgRect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_avatarRect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_tra);

        }
    }

    #endregion

}
/// <summary>
/// 条漫组件
/// </summary>
[Serializable]
public class CaricaturePlayerModule
{
    #region 字段
    /// <summary>
    /// 图片资源名字
    /// </summary>
    [SerializeField]
    string _imageName;
    /// <summary>
    /// 条漫图片资源
    /// </summary>
    [SerializeField]
    Sprite _imageSprite;
    /// <summary>
    /// 特殊条漫
    /// </summary>
    [SerializeField]
    CaricaturePlayerSpecialModuleBasics _caricaturePlayerSpecialModuleBasics;
    /// <summary>
    /// 当前展示特殊模块
    /// </summary>
    CaricaturePlayerSpecialModuleBasics _currModule;
    /// <summary>
    /// 层级
    /// </summary>
    [SerializeField]
    int _hierarchy = 1;
    /// <summary>
    /// 间距
    /// </summary>
    [SerializeField]
    float _interval;
    /// <summary>
    /// 初始位置偏移量
    /// </summary>
    [SerializeField]
    Vector3 _skewingValue;
    /// <summary>
    /// 加尺寸
    /// </summary>
    [SerializeField]
    Vector3 _addSize;
    /// <summary>
    /// 条漫组件移动方向
    /// </summary>
    [SerializeField]
    ChapterTool.CaricatureModuleDirectionType _directionType;
    /// <summary>
    /// 是否展示移动效果
    /// </summary>
    [SerializeField]
    bool _isMove = true;
    /// <summary>
    /// 是否自动播放
    /// </summary>
    [SerializeField]
    bool _autoplay = false;
    /// <summary>
    /// 对话集合
    /// </summary>
    [SerializeField]
    List<DialogueBoxCartoonCpmponent> _dialogues = new List<DialogueBoxCartoonCpmponent>();
    /// <summary>
    /// 展示时的回调
    /// </summary>
    Action _showEndAcrion;

    Action<bool> _openBtnAction;
    /// <summary>
    /// 自身ui组件
    /// </summary>
    RectTransform _thisRect;
    /// <summary>
    /// 图片展示
    /// </summary>
    Image _showImage;
    /// <summary>
    /// 是否展示完毕
    /// </summary>
    bool _isShowIn;
    /// <summary>
    /// 是否设置图片完毕
    /// </summary>
    bool _isShow = false;
    /// <summary>
    /// 条漫点击计数
    /// </summary>
    int _click = 0;
    #endregion
    #region 属性
    /// <summary>
    /// 自身ui组件
    /// </summary>
    public RectTransform _ThisRect { get { return _thisRect; } set { _thisRect = value; } }
    /// <summary>
    /// 层级
    /// </summary>
    public int _Hierarchy { get { return _hierarchy; } }
    /// <summary>
    /// 间距
    /// </summary>
    public float _Interval { get { return _interval; } }
    /// <summary>
    /// 初始位置偏移量
    /// </summary>
    public Vector3 _SkewingValue { get { return _skewingValue; } }
    /// <summary>
    /// 条漫组件移动方向
    /// </summary>
    public ChapterTool.CaricatureModuleDirectionType _DirectionType { get { return _directionType; } }
    /// <summary>
    /// 是否展示移动效果
    /// </summary>
    public bool _IsMove { get { return _isMove; } }
    /// <summary>
    /// 是否展示完毕
    /// </summary>
    public bool _IsShowIn { get { return _isShowIn; } }

    /// <summary>
    /// 是否自动播放
    /// </summary>
    public bool _Autoplay { get { return _autoplay; } }
    /// <summary>
    /// 特殊条漫
    /// </summary>
    public CaricaturePlayerSpecialModuleBasics _CaricaturePlayerSpecialModuleBasics { get { return _caricaturePlayerSpecialModuleBasics; } }
    /// <summary>
    /// 对话集合
    /// </summary>
    public List<DialogueBoxCartoonCpmponent> _Dialogues { get { return _dialogues; } }
    /// <summary>
    /// 是否设置图片完毕
    /// </summary>
    public bool _IsShow { get { return _isShow; } }
    #endregion
    #region 函数
    /// <summary>
    /// 初始化正常条漫
    /// </summary>
    /// <param name="openIsBeingSpeakAction"></param>
    /// <param name="imageRect"></param>
    /// <param name="showEndAction"></param>
    public void Initial(Action<bool> openBtnAction, Action showEndAction)
    {
        _openBtnAction = openBtnAction;
        _showEndAcrion = showEndAction;
    }
    /// <summary>
    /// 初始化特殊模块
    /// </summary>
    /// <param name="showEndAction"></param>
    public void InitialSpecialModule(Action<bool> openBtnAction, Action showEndAction, Transform par)
    {
        _showEndAcrion = showEndAction;
        if (_caricaturePlayerSpecialModuleBasics != null)
        {
            _isShowIn = true;
        }
        else
        {
            _isShowIn = false;
        }
        GameObject obj = GameObject.Instantiate(_caricaturePlayerSpecialModuleBasics.gameObject, par);
        _currModule = obj.GetComponent<CaricaturePlayerSpecialModuleBasics>();
        _currModule.SetEndAction(openBtnAction, () =>
        {
            _showEndAcrion?.Invoke();
            _isShowIn = false;
        });
        _currModule.Initial();
        _thisRect = _currModule.transform.GetComponent<RectTransform>();
        _thisRect.gameObject.SetActive(false);
    }
    /// <summary>
    /// 条漫模块移动完毕
    /// </summary>
    public void MoveEnd()
    {
        if (_caricaturePlayerSpecialModuleBasics != null)
        {
            _isShowIn = true;
            _currModule.MoveEnd();
        }
        else
        {
            if (_dialogues == null || _dialogues.Count <= 0)
            {
                _isShowIn = false;
            }
            else
            {
                _isShowIn = true;
            }
            ClickBtn();
        }
    }
    /// <summary>
    /// 展示条漫
    /// </summary>
    /// <returns></returns>
    public async UniTask Show(RectTransform rectTransform = null)
    {
        if (rectTransform != null)
        {
            _thisRect = rectTransform;
            _showImage = rectTransform.GetComponent<Image>();
            _showImage.raycastTarget = false;
            try
            {
                if (_imageSprite == null && _currModule == null)
                {
                    _showImage.sprite = await ABManager.GetAssetAsync<Sprite>(_imageName);
                    _showImage.SetNativeSize();
                }
                else
                {
                    _showImage.sprite = _imageSprite;
                    _showImage.SetNativeSize();
                }

                //_thisRect.sizeDelta = new Vector2(_thisRect.sizeDelta.x + _addSize.x, _thisRect.sizeDelta.y + _addSize.y);
                _thisRect.gameObject.SetActive(true);
            }
            catch (Exception er)
            {
                Debug.Log("找不到条漫资源：" + _imageName);
            }
        }
        else
        {
            _thisRect.gameObject.SetActive(true);
        }

        _isShow = true;
    }
    /// <summary>
    /// 设置条漫位置
    /// </summary>
    /// <param name="localPoint"></param>
    public void SetPoint(Vector3 localPoint)
    {
        _thisRect.localPosition = localPoint;
    }
    /// <summary>
    /// 条漫组件点击
    /// </summary>
    public async void ClickBtn()
    {
        if (_currModule != null)
        {
            _currModule.Click();
        }
        else
        {
            if (_click < _dialogues.Count)
            {
                if (_click - 1 >= 0)
                {
                    await _dialogues[_click - 1].CloseShowText();
                }
                _dialogues[_click].SetIsDefaultShow(false);
                _dialogues[_click].Initial(DialogueEnd, SpeakAefore, SpeakRear, DialogueBoxClose);
                Transform dualogueTra = _dialogues[_click].transform;

                dualogueTra.SetParent(_thisRect);

                dualogueTra.localPosition = new Vector3(dualogueTra.localPosition.x, dualogueTra.localPosition.y + _thisRect.localPosition.y);
                await _dialogues[_click].DefaultShow();
            }
            else
            {
                _showEndAcrion?.Invoke();
            }
            _click++;
            if (_click >= _dialogues.Count)
            {
                _isShowIn = false;
            }
        }
    }
    public void DialogueEnd()
    {

    }
    /// <summary>
    /// 说话前
    /// </summary>
    /// <param name="data"></param>
    public void SpeakAefore(ChapterDialogueTextDefine data)
    {

    }
    /// <summary>
    /// 说话后
    /// </summary>
    /// <param name="data"></param>
    public async void SpeakRear(ChapterDialogueTextDefine data)
    {
        _openBtnAction?.Invoke(true);
        if (_click < _dialogues.Count)
        {
            _openBtnAction?.Invoke(true);
        }
        else
        {
            _showEndAcrion?.Invoke();
        }
    }
    /// <summary>
    /// 对话框关闭后
    /// </summary>
    /// <param name="data"></param>
    public void DialogueBoxClose(ChapterDialogueTextDefine data)
    {

    }


    public void Dispose()
    {

        if (_thisRect != null)
        {
            for (int i = _thisRect.childCount - 1; i >= 0; i--)
            {
                Transform chidTra = _thisRect.GetChild(i);
                GameObject.Destroy(chidTra.gameObject);
            }
        }
    }
    #endregion
}
/// <summary>
/// 购物商品item
/// </summary>
[Serializable]
public class ShoppingItem
{
    #region 字段
    /// <summary>
    /// 商品名字
    /// </summary>
    [SerializeField]
    int _commodityName;
    /// <summary>
    /// 商品图片资源名字
    /// </summary>
    [SerializeField]
    string _imageName;
    /// <summary>
    /// 商品图片精灵
    /// </summary>
    [SerializeField]
    Sprite _imageSprite;
    /// <summary>
    /// 商品实列对象
    /// </summary>
    [SerializeField]
    RectTransform _thisRect;
    /// <summary>
    /// 图片展示
    /// </summary>
    Image _showImage;
    /// <summary>
    /// 拖拽脚本
    /// </summary>
    UIPanelDrag _uIPanelDrag;
    /// <summary>
    /// 鼠标放开回调
    /// </summary>
    Action<ShoppingItem> _pointerUpAction;
    /// <summary>
    /// 鼠标按下回调
    /// </summary>
    Action<ShoppingItem> _pointerDown;
    /// <summary>
    /// 鼠标点击
    /// </summary>
    Action<ShoppingItem> _clickItemAction;
    /// <summary>
    /// 初始位置
    /// </summary>
    Vector3 _initialPoint;
    /// <summary>
    /// 移动目标点
    /// </summary>
    Vector3 _tagePoint;
    #endregion
    #region 属性
    /// <summary>
    /// 商品数据id
    /// </summary>
    public int _CommodityName { get { return _commodityName; } }
    /// <summary>
    /// 拖拽脚本
    /// </summary>
    public UIPanelDrag _UIPanelDrag { get { return _uIPanelDrag; } }
    /// <summary>
    /// 商品实列对象
    /// </summary>
    public RectTransform _ThisRect { get { return _thisRect; } }
    #endregion
    #region 函数
    /// <summary>
    /// 初始化商品
    /// </summary>
    /// <param name="tagePoint"></param>
    /// <param name="endAction"></param>
    public void Initial(Vector3 tagePoint, Action<ShoppingItem> pointerUpAction, Action<ShoppingItem> pointerDown, Action<ShoppingItem> clickItemAction)
    {
        _tagePoint = tagePoint;
        _initialPoint = _thisRect.localPosition;
        _pointerUpAction = pointerUpAction;
        _pointerDown = pointerDown;
        _clickItemAction = clickItemAction;
        _uIPanelDrag = _thisRect.GetComponent<UIPanelDrag>();
        _uIPanelDrag.actionOnClick = ClickItem;
        _uIPanelDrag.actionOnPointerUp = PointerUp;
        _uIPanelDrag.actionOnPointerDown = PointerDown;
        _uIPanelDrag.localPos = _initialPoint;
        ShowImage();
    }
    /// <summary>
    /// 鼠标放开
    /// </summary>
    /// <param name="obj"></param>
    private void PointerUp(PointerEventData obj)
    {
        if (Vector3.Distance(_tagePoint, _thisRect.localPosition) < 230)
        {
            _pointerUpAction?.Invoke(this);
        }
        else
        {
            ResetPoint();
        }
    }
    /// <summary>
    /// 鼠标按下
    /// </summary>
    /// <param name="obj"></param>
    private void PointerDown(PointerEventData obj)
    {
        _thisRect.SetAsLastSibling();
        _pointerDown?.Invoke(this);
    }
    /// <summary>
    /// 鼠标点击
    /// </summary>
    /// <param name="obj"></param>
    private void ClickItem(PointerEventData obj)
    {

        _clickItemAction?.Invoke(this);
    }
    /// <summary>
    /// 重置位置
    /// </summary>
    public void ResetPoint()
    {
        _thisRect.localPosition = _initialPoint;
    }
    /// <summary>
    /// 展示图片
    /// </summary>
    /// <returns></returns>
    public async UniTask ShowImage()
    {
        try
        {
            if (_imageSprite == null)
            {
                _showImage.sprite = await ABManager.GetAssetAsync<Sprite>(_imageName);
            }
            else
            {
                _showImage.sprite = _imageSprite;
            }

            _thisRect.gameObject.SetActive(true);
            _showImage.SetNativeSize();
        }
        catch (Exception er)
        {
            Debug.Log("找不到条漫资源：" + _imageName);
        }
    }
    /// <summary>
    /// 开关item
    /// </summary>
    /// <param name="isOpen"></param>
    public void OpenItem(bool isOpen)
    {
        _thisRect.gameObject.SetActive(isOpen);
    }
    #endregion
}
/// <summary>
/// 购物清单item
/// </summary>
public class InventoryItem
{
    #region 字段
    /// <summary>
    /// 对象实体
    /// </summary>
    RectTransform _thisRect;
    /// <summary>
    /// 商品名字展示
    /// </summary>
    Text _showText;
    /// <summary>
    /// 完成提示
    /// </summary>
    Transform _finishTipeTra;
    #endregion
    #region 函数
    /// <summary>
    /// 初始化item
    /// </summary>
    /// <param name="iteRect"></param>
    /// <param name="showName"></param>
    public void Initial(RectTransform iteRect, string showName)
    {
        _thisRect = iteRect;
        _showText = _thisRect.Find("ShowText").GetComponent<Text>();
        _finishTipeTra = _thisRect.Find("Line/FinishTipe");
        _showText.text = showName;
        LayoutRebuilder.ForceRebuildLayoutImmediate(_showText.transform.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(_thisRect);
        OpenFinishTipe(false);
    }
    /// <summary>
    /// 开关完成提示
    /// </summary>
    /// <param name="isOpen"></param>
    public void OpenFinishTipe(bool isOpen)
    {
        _finishTipeTra.gameObject.SetActive(isOpen);
    }

    public void Dispose()
    {
        GameObject.Destroy(_thisRect.gameObject);
    }
    #endregion
}
/// <summary>
/// 除草玩法杂草对象
/// </summary>
public class WeedItem
{
    #region 字段
    RectTransform _thisRect;

    //RectTransform _award
    #endregion
    #region 函数
    #endregion

}