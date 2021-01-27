using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuDieController : MonoBehaviour
{
    #region 字段
    [SerializeField]
    Transform _teXiaoTra;
    [SerializeField]
    CubismModel _bubismModel;
    [SerializeField]
    ParticleSystem _particleSystem;
    public CubismRenderer cubismRenderer;


    #endregion
    #region 函数
    private void Awake()
    {


    }
    // Start is called before the first frame update
    void Start()
    {

    }

    private void LateUpdate()
    {
        MoveSpecialEffects();
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void MoveSpecialEffects()
    {
        if (cubismRenderer!=null)
        {
            var mesh = cubismRenderer.Mesh;
            _teXiaoTra.localPosition = mesh.bounds.center;
        }
    }
    #endregion
}
