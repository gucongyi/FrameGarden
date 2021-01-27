using System;
using Game.Protocal;
using Google.Protobuf;
using UnityEngine;

public class ProtoSendMethod
{
    //业务层协议
    public static void BusinessRequest<toc>(IMessage sendMsg, OpCodeType opCodeType,Action<toc> onSuccess,
            Action<ErrorInfo> onFail, bool isShowDefaultTip=true) where toc:IMessage<toc>
    {
        WaitManager.BeginRotate();
        WebSocketComponent._instance.SendMsg(sendMsg, opCodeType,
            (res) =>
            {
                toc tocRes = (toc)res;
                onSuccess(tocRes);
            },
            async (error) =>
            {
                StaticData.DebugGreen(sendMsg.GetType().ToString() + " webErrorCode:" + error.webErrorCode + " ErrorMessage:" + error.ErrorMessage);
                if (isShowDefaultTip)
                {
                    var defineErrorCode=StaticData.configExcel.ServerErrorCode.Find(x => x.errorCode == (Company.Cfg.WebErrorCode)error.webErrorCode);
                    if (defineErrorCode.isShowTips)
                    {
                        StaticData.CreateToastTips($"{defineErrorCode.SimplifiedChinese}");
                    }
                    
                }
                else
                {
                    onFail(error);
                }
            });
    }
}
