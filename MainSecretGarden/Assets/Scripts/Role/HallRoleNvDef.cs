using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;
using Company.Cfg;
using Cysharp.Threading.Tasks;

/// <summary>
/// 主页默认女主控制类
/// </summary>
public class HallRoleNvDef : Live2DRoleControllerBase
{

    #region 变量

    /// <summary>
    /// 随机动画列表 点击动画
    /// </summary>
    [SerializeField]
    protected List<PlayerAnimType> _randomAnimTouchHairList;
    [SerializeField]
    protected List<PlayerAnimType> _randomAnimTouchClothesList;
    [SerializeField]
    protected List<PlayerAnimType> _randomAnimTouchBottomsList;

    private CubismModel _model;
    private CubismRenderController _renderController;

    /// <summary>
    /// 需要隐藏的部位
    /// </summary>
    private List<string> _needHideParts = new List<string>();
    /// <summary>
    /// 需要显示的部位
    /// </summary>
    private List<string> _needShowParts = new List<string>();

    #endregion

    private void Awake()
    {

    }

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        _model = _curPlayerAnim.gameObject.FindCubismModel();
        _renderController = _model.gameObject.GetComponent<CubismRenderController>();
    }



    private void Update()
    {
        UpdateRole();
    }

    protected override void UpdateRole()
    {
        base.UpdateRole();
    }


    private void LateUpdate()
    {
        UpdateRoleDressUpParts();
    }

    #region 控制点击角色播放动画

    /// <summary>
    /// 通知播放动画 1.头部 2.身体3.腿部
    /// </summary>
    /// <param name="type"></param>
    protected override void NotifyPlayAnim(HitLocation type)
    {
        if (_isPlayingAnim)
            return;

        List<PlayerAnimType> randomAnim = new List<PlayerAnimType>();
        switch (type) 
        {
            case HitLocation.Loc_Hair: randomAnim.AddRange(_randomAnimTouchHairList); break;
            case HitLocation.Loc_Clothes: randomAnim.AddRange(_randomAnimTouchClothesList); break;
            case HitLocation.Loc_Bottoms: randomAnim.AddRange(_randomAnimTouchBottomsList); break;
            default: randomAnim.AddRange(_randomAnimTouchHairList); break;
        }

        int index = 0;
        if (randomAnim.Count > 1)
            index = Random.Range(0, randomAnim.Count);

        PlayRoleAnim(PlayerAnimDirection.Down, randomAnim[index], true);
    }

    #endregion

    #region 换装
    //Live2D换装思路 
    //将live2D模型固定一套基础样式（默认裸妆/只你内衣）然后按照部位划分 每个部位单独对应一张贴图 客户端更换贴图实现换装
    //需求
    //1.live2D模型给到时所有的部位都要显示出来
    //2.部位 对应 id/名称
    //处理
    //1.部位的显示 控制方式2种 都需要在 LateUpdate() 中进行更新
    //（1）.控制 CubismModel->Parameters 需要客户端测试参数对应位置
    //（2）.控制 CubismModel->Parts->Id 与live2D模型给到的部位id一致 （当前选用的方式）
    //2.通过CubismRenderController 获取所有的 CubismRenderer 然后对比 CubismRenderer->MainTexture.name //贴图名称 
    //3.对比贴图名称判断是否为需要更新的贴图 然后更换贴图 
    //完成换装



    /// <summary>
    /// 通知换装
    /// </summary>
    /// <param name="costume"></param>
    /// <param name="isTakeOff">是否为脱下时装</param>
    public async void NotifyDressUp(CostumeDefine costume, bool isTakeOff = false)
    {
        if (isTakeOff) 
        {
            //await UniTask.WaitForEndOfFrame();
            //更新换装部位
            UpdateParts(costume.ShowParts, null);
            return;
        }
        //换装贴图
        UpdateTexture(costume.UseTexture);
        await UniTask.WaitForEndOfFrame();//UniTask.Delay(200);
        //更新换装部位
        UpdateParts(costume.HideParts, costume.ShowParts);

    }

    /// <summary>
    /// 初始化换装
    /// </summary>
    public async void InitDressUp(List<int> costumeIDs) 
    {
        Debug.Log("InitDressUp 初始化换装!");

        _needHideParts.Clear();
        _needShowParts.Clear();

        List<string> hideParts = new List<string>();
        List<string> showParts = new List<string>();

        //影藏全部部位 需要后续添加
        hideParts.Add("xiezi");
        hideParts.Add("tfaqian");
        hideParts.Add("tfaqianchang");
        hideParts.Add("tfahou");
        hideParts.Add("nabao");
        hideParts.Add("nabao2");
        hideParts.Add("bao2");
        hideParts.Add("bao");
        hideParts.Add("nabao3");
        hideParts.Add("nabao4");
        hideParts.Add("bao3");
        hideParts.Add("bao4");
        hideParts.Add("maozi");
        hideParts.Add("toushi2");
        hideParts.Add("weijin");
        hideParts.Add("dayi");
        hideParts.Add("erhuan");
        hideParts.Add("xiangquan");
        hideParts.Add("xianglian");
        hideParts.Add("shoushi");
        hideParts.Add("wazi");
        hideParts.Add("kuzi");
        hideParts.Add("lianyiqun");
        hideParts.Add("lianyiqun2");
        hideParts.Add("xiayi");
        hideParts.Add("xiayi2");
        hideParts.Add("xiajinshenqun");
        hideParts.Add("xiajinshenqun2");
        hideParts.Add("shangyi");
        hideParts.Add("toushi1");
        hideParts.Add("toushi");
        hideParts.Add("tfagaomawei");
        hideParts.Add("tfadimawei");
        hideParts.Add("tfahoumawei");
        //hideParts.Add("youshou");
        //hideParts.Add("zuoshou");

        List<CostumeTextureInfo> useTexture = new List<CostumeTextureInfo>();

        CostumeDefine costume = null;
        for (int i = 0; i < costumeIDs.Count; i++)
        {
            costume = StaticData.configExcel.GetCostumeByCostumeId(costumeIDs[i]);

            useTexture.AddRange(costume.UseTexture);
            for (int j = 0; j < costume.HideParts.Count; j++)
            {
                if (!hideParts.Contains(costume.HideParts[j])) 
                {
                    hideParts.Add(costume.HideParts[j]);
                }
            }
            for (int j = 0; j < costume.ShowParts.Count; j++)
            {
                //将显示的从影藏中移除
                if (hideParts.Contains(costume.ShowParts[j]))
                {
                    hideParts.Remove(costume.ShowParts[j]);
                }
                if (!showParts.Contains(costume.ShowParts[j]))
                {
                    showParts.Add(costume.ShowParts[j]);
                }
            }
        }

        //更新换装部位
        UpdateParts(hideParts, showParts);
        await UniTask.WaitForEndOfFrame();//UniTask.Delay(300);
        //换装贴图
        UpdateTexture(useTexture, true);
        
    }

    /// <summary>
    /// 更新角色换装部位  LateUpdate() 调用
    /// </summary>
    private void UpdateRoleDressUpParts() 
    {
        
        for (int i = 0; i < _model.Parts.Length; i++)
        {
            if (_needHideParts.Contains(_model.Parts[i].Id))//隐藏
            {
                _model.Parts[i].Opacity = 0f;
            }
            else if (_needShowParts.Contains(_model.Parts[i].Id))//显示
            {
                _model.Parts[i].Opacity = 1f;
            }
        }
    }

    /// <summary>
    /// 部位更新
    /// </summary>
    /// <param name="hideParts"></param>
    /// <param name="showParts"></param>
    private void UpdateParts(List<string> hideParts, List<string> showParts) 
    {
        if (hideParts != null) 
        {
            for (int i = 0; i < hideParts.Count; i++)
            {
                if (_needShowParts.Contains(hideParts[i]))
                {
                    _needShowParts.Remove(hideParts[i]);
                }
                if (!_needHideParts.Contains(hideParts[i]))
                {
                    _needHideParts.Add(hideParts[i]);
                }
            }
        }

        if (showParts != null)
        {
            for (int i = 0; i < showParts.Count; i++)
            {
                if (_needHideParts.Contains(showParts[i]))
                {
                    _needHideParts.Remove(showParts[i]);
                }
                if (!_needShowParts.Contains(showParts[i]))
                {
                    _needShowParts.Add(showParts[i]);
                }
            }
        }
        else 
        {
            //卸载
            //只有隐藏没有显示 注意手部是否需要显示
            HandDetection();
        }   
    }

    /// <summary>
    /// 手部检测
    /// </summary>
    private void HandDetection() 
    {
        List<string> handParts = new List<string>();
        handParts.Add("nabao");
        handParts.Add("nabao2");
        handParts.Add("nabao3");
        handParts.Add("nabao4");
        handParts.Add("youshou");
        handParts.Add("zuoshou");
        for (int i = 0; i < _needShowParts.Count; i++)
        {
            if (handParts.Contains(_needShowParts[i])) 
            {
                return;
            }
        }
        //
        if (_needHideParts.Contains("youshou"))
        {
            _needHideParts.Remove("youshou");
        }
        if (_needHideParts.Contains("zuoshou"))
        {
            _needHideParts.Remove("zuoshou");
        }
        _needShowParts.Add("youshou");
        _needShowParts.Add("zuoshou");

        List<CostumeTextureInfo> useTexture = new List<CostumeTextureInfo>();
        CostumeTextureInfo textInfo = new CostumeTextureInfo();
        textInfo.DefTextureName = "Hand_Def_00";
        textInfo.UseTextureName = "Hand_Def_00";
        useTexture.Add(textInfo);
        //换装贴图
        UpdateTexture(useTexture);
    }



    /// <summary>
    /// 更新贴图
    /// </summary>
    /// <param name="useTexture"></param>
    private void UpdateTexture(List<CostumeTextureInfo> useTexture, bool isInit = false) 
    {
        if (_renderController.Renderers == null)
            return;

        for (int i = 0; i < _renderController.Renderers.Length; i++)
        {
            UpdateRenderer(_renderController.Renderers[i], useTexture, isInit);
        }
    }

    /// <summary>
    /// 换装更新Renderer 里面的 MainTexture/贴图
    /// 资源可以自动释放无需程序员操作
    /// </summary> 
    /// <param name="cubismRender"></param>
    private async void UpdateRenderer(CubismRenderer cubismRender, List<CostumeTextureInfo> UseTexture, bool isInit = false)
    {
        var defTextureName = string.Empty;
        for (int i = 0; i < UseTexture.Count; i++)
        {
            defTextureName = cubismRender.MainTexture.name;
            if (defTextureName == UseTexture[i].DefTextureName)
            {
                //初始化 .ToLower()
                if (isInit && defTextureName == UseTexture[i].UseTextureName) 
                {
                    continue;
                }
                cubismRender.MainTexture = await ABManager.GetAssetAsync<Texture2D>(UseTexture[i].UseTextureName);
                cubismRender.MainTexture.name = defTextureName;
            }
        }
    }

    #endregion

    #region 测试使用
    private void PlayAnim() 
    {
        //_curPlayerAnim.PlayerAnimation.Play("anim001"); 
    }

    #endregion

}


