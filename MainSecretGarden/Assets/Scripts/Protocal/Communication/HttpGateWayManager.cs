using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Game.Protocal;
using Google.Protobuf;
using Company.Cfg;

public class HttpGateWayManager
{
    public static IEnumerator Post(string url, CSLinkInfo csLinkInfo,int timeout, Action<string,int,long> OnSucc,Action<WebErrorCode> OnError)
    {
        byte[] byteData = ProtoSerAndUnSer.Serialize(csLinkInfo);
        CSServerReq req = new CSServerReq { Data = ByteString.AttachBytes(byteData),OpCode = OpCodeType.GetConnectServer };
        byte[] sendData = ProtoSerAndUnSer.Serialize(req);
        WWWForm from = new WWWForm();

        UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST)
        {
            uploadHandler = new UploadHandlerRaw(sendData),
            downloadHandler = new DownloadHandlerBuffer(),
            timeout = timeout
        };
        request.SetRequestHeader("Content-Type", "multipart/form-data");
        yield return request.SendWebRequest();
        //拿到信息
        SCServerRes data = ProtoSerAndUnSer.UnSerialize<SCServerRes>(request.downloadHandler.data);
        if (data==null)
        {
            Debug.LogError("网关异常");
            OnError?.Invoke(new WebErrorCode());
            yield break;
        }
        if (data.Data != null && data.Code == (int)WebErrorCode.Success)
        {
            SCLinkInfo info = ProtoSerAndUnSer.UnSerialize<SCLinkInfo>(data.Data.ToByteArray());
            Debug.Log(info);
            if (info != null)
            {
                string ip = StaticData.IntToIp((long)info.Ip);
                OnSucc?.Invoke(ip,info.Port, info.Uid);
            }
        }
        else
        {
            OnError?.Invoke((WebErrorCode)data.Code);
        }
    }

    
}
