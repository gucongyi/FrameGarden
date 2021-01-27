using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 聊天弹幕item控制器
/// </summary>
public class ChatBulletScreenItemController : MonoBehaviour
{
    #region 字段
    ChatBulletScreenController _chatBulletScreenController;
    RectTransform _thisRect;
    Text _showText;
    ChatInfo _showStr;
    bool _isInitial = false;
    #endregion
    #region 属性
    public ChatInfo _ShowStr { get { return _showStr; } }
    #endregion
    #region 函数
    private void Awake()
    {
        if (!_isInitial)
        {
            Initial();
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Initial()
    {
        _chatBulletScreenController = transform.parent.parent.GetComponent<ChatBulletScreenController>();
        _thisRect = GetComponent<RectTransform>();
        _showText = GetComponent<Text>();
        _isInitial = true;
    }
    /// <summary>
    /// 展示弹幕
    /// </summary>
    /// <param name="str"></param>
    public void ShowData(ChatInfo str, float speed)
    {

        if (!_isInitial)
        {
            Initial();
        }
        ResetItemData();
        _showStr = str;
        _showText.text = _showStr._message;
        gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_thisRect);
        ResetPoint();
        StartCoroutine(Move(GetEndPoint(), speed));
    }
    /// <summary>
    /// 重置数据
    /// </summary>
    void ResetItemData()
    {
        _showStr = null;
        _showText.text = "";
    }
    /// <summary>
    /// 重置位置
    /// </summary>
    void ResetPoint()
    {
        RectTransform parent = transform.parent.GetComponent<RectTransform>();
        float maxHeight = (parent.rect.height / 2) - _thisRect.rect.height / 2;
        float minHeight = -(parent.rect.height / 2 - _thisRect.rect.height / 2);
        float x = Screen.width / 2 + _thisRect.rect.width / 2;
        _thisRect.localPosition = new Vector3(x, Random.Range(minHeight, maxHeight));
    }
    /// <summary>
    /// 获取终点
    /// </summary>
    /// <returns></returns>
    Vector3 GetEndPoint()
    {
        RectTransform parent = transform.parent.GetComponent<RectTransform>();
        float x = -(Screen.width / 2 + _thisRect.rect.width / 2);
        return new Vector3(x, _thisRect.localPosition.y);
    }

    IEnumerator Move(Vector3 endVector, float speed)
    {
        while (Vector3.Distance(_thisRect.localPosition, endVector) > 1)
        {
            _thisRect.localPosition = Vector3.MoveTowards(_thisRect.localPosition, endVector, Time.deltaTime * speed);
            yield return new WaitForSeconds(0.01f);
        }
        _chatBulletScreenController.HidItem(this);
        yield return null;
    }
    #endregion
}
