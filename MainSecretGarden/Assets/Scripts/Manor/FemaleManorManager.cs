using Cysharp.Threading.Tasks;
using DG.Tweening;
using Live2D.Cubism.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class FemaleManorManager : MonoBehaviour
{
    public Animator anim;
    public CubismRenderController renderModelCtl;
    public float walkSpeed=5f;//每s多少米
    public int QuickReachCount = 5;
    public const float tileLength = 8f;
    bool isBeginMove;
    Vector3 currDirection;
    Vector3 finalPos;
    float totalDistance;
    float currWalkDistance;
    TileComponent currTileComponent;
    CancellationTokenSource ctsWaitTileReach;
    /*与x或y轴夹角
     * <=30度，是为y
     * >30度小于60度视为四个角
     * >60度是为x
     */
    public float angleIncluded = 15f;
    private void Awake()
    {
        //常量赋值
        walkSpeed = StaticData.configExcel.GetVertical().ManorModelSpeed;
        QuickReachCount = StaticData.configExcel.GetVertical().ManorQuickTileCount;
        angleIncluded = StaticData.configExcel.GetVertical().ManorModelAngleForwardAndBack;
    }
    public enum StateMove
    {
        Idle,
        Forward,
        Back,
        Left,
        Right,
        LeftTop,
        RightTop,
        LeftBottom,
        RightBottom
    }
    public void PlayAnim(Vector3 ScreenPoint)
    {
        var cameraWorld=Root2dSceneManager._instance.worldCameraComponent.GetComponent<Camera>();
        Vector3 worldPoint=cameraWorld.ScreenToWorldPoint(ScreenPoint);
        //临时点
        var TempCalcLocalPosGo = new GameObject("TempCalcLocalPos");
        TempCalcLocalPosGo.transform.parent = Root2dSceneManager._instance.transform;
        TempCalcLocalPosGo.transform.position = worldPoint;
        //以为有缩放，所以需要转一下
        Vector3 willReachLocalPos = TempCalcLocalPosGo.transform.localPosition;
        var finalPos = willReachLocalPos;
        //删除临时点
        Destroy(TempCalcLocalPosGo);
        //开始计算方向
        var deltaVecor = finalPos - transform.localPosition;
        if (Mathf.Abs(deltaVecor.y) < float.Epsilon && Mathf.Abs(deltaVecor.x) < float.Epsilon)
        {
            return;
        }
        currDirection = Vector3.Normalize(deltaVecor);
        totalDistance = Vector3.Distance(finalPos, transform.localPosition);
        if (totalDistance >= QuickReachCount * tileLength)
        {
            //瞬移
            isBeginMove = false;//之前走的就不继续了
            transform.localPosition = finalPos;
            anim.Play(StateMove.Idle.ToString(), 0);
            return;
        }
        currWalkDistance = 0f;
        if (ctsWaitTileReach != null)
        {
            ctsWaitTileReach.Cancel();
        }
        isBeginMove = true;
        this.finalPos = finalPos;
        PlayAnimStateByDirection(deltaVecor);
    }

    public async UniTask<TileComponent> PlayAnim(TileComponent tileComponent)
    {
        currTileComponent = tileComponent;
        var willReachLocalPos = tileComponent.transform.localPosition;
        //放在地块左边
        var finalPos = willReachLocalPos + new Vector3(-3.73f, -0.46f);
        var deltaVecor = finalPos - transform.localPosition;
        if (Mathf.Abs(deltaVecor.y) < float.Epsilon && Mathf.Abs(deltaVecor.x) < float.Epsilon)
        {
            return currTileComponent;
        }
        currDirection = Vector3.Normalize(deltaVecor);
        totalDistance = Vector3.Distance(finalPos, transform.localPosition);
        if (totalDistance >= QuickReachCount * tileLength)
        {
            //瞬移
            isBeginMove = false;//之前走的就不继续了
            transform.localPosition = finalPos;
            anim.Play(StateMove.Idle.ToString(), 0);
            return currTileComponent;
        }
        currWalkDistance = 0f;
        isBeginMove = true;
        this.finalPos = finalPos;
        PlayAnimStateByDirection(deltaVecor);
        ctsWaitTileReach = new CancellationTokenSource();
        await UniTask.WaitUntil(() => isBeginMove == false,cancellationToken:ctsWaitTileReach.Token);
        return currTileComponent;
    }

    private void PlayAnimStateByDirection(Vector3 deltaVecor)
    {
        StateMove stateMove = StateMove.Idle;
        float angleY = Vector2.Angle(deltaVecor, Vector2.up);//与y轴夹角
        float angleX = Vector2.Angle(deltaVecor, Vector2.right);//与x轴夹角
        //限制夹角在0-180
        if (angleY > 180)
        {
            angleY = 360 - angleY;
        }
        if (angleX > 180)
        {
            angleX = 360 - angleX;
        }
        if (angleY <= angleIncluded)
        {
            stateMove = StateMove.Back;
        }
        else if (angleY >= (180 - angleIncluded))
        {
            stateMove = StateMove.Forward;
        }
        else if (angleX <= angleIncluded)
        {
            stateMove = StateMove.Right;
        }
        else if (angleX >= (180 - angleIncluded))
        {
            stateMove = StateMove.Left;
        }
        else if (angleY > angleIncluded && angleY < (90 - angleIncluded))//左上或者右上
        {
            if (angleX > angleIncluded && angleX < (90 - angleIncluded))
            {
                stateMove = StateMove.RightTop;
            }
            else if (angleX > (90 + angleIncluded) && angleX < (180 - angleIncluded))
            {
                stateMove = StateMove.LeftTop;
            }
        }
        else if (angleY > (90 + angleIncluded) && angleY < (180 - angleIncluded))//左下或者右下
        {
            if (angleX > angleIncluded && angleX < (90 - angleIncluded))
            {
                stateMove = StateMove.RightBottom;
            }
            else if (angleX > (90 + angleIncluded) && angleX < (180 - angleIncluded))
            {
                stateMove = StateMove.LeftBottom;
            }
        }
        anim.Play(stateMove.ToString(), 0);
    }

    public void SetSortingOrder(int order)
    {
        renderModelCtl.SortingOrder = order;
    }
    private void Update()
    {
        var currFrameVec = currDirection * Time.deltaTime * walkSpeed;
        if (isBeginMove)
        {
            transform.localPosition += currFrameVec;
            currWalkDistance += Vector3.Magnitude(currFrameVec);
            if (currWalkDistance >= totalDistance)
            {
                transform.localPosition = finalPos;
                isBeginMove = false;//到达
                anim.Play(StateMove.Idle.ToString(), 0);
            }
            Root2dSceneManager._instance.UpdateSortLayer(true);
        }
    }
}
