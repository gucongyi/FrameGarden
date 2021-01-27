using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using LitJson;
using System.Text;

public class GoogleProtobufEditor : UnityEditor.Editor
{
    private const string path = "/Scripts/Protocal/";
    private const string batFileName = "buildCSharp.bat";
    private const string jsonName = "proto.json";
    private static string jsonDestPath = Application.dataPath + path + "Relation";

    [MenuItem("数据/一键生成C#协议")]
    public static void GenerateProtobuf()
    {
        UpdateProtoFromSvn();
        CopyProto();
        GoogleProtobuf();
        CopyProtoRelationJson();
        GenerateProtoManagerAndRelationByProtoJson();
    }
    public static void GoogleProtobuf()
    {
        var p1 = Application.dataPath + path + batFileName;
        Debug.Log(p1);
        System.Diagnostics.Process newProcess = System.Diagnostics.Process.Start(p1);
        newProcess.WaitForExit();
    }
    static void UpdateProtoFromSvn()
    {
        string paths = GetProtoSvnPath();
        string cmd = @"/c svn checkout " + paths;
        System.Diagnostics.Process newProcess = System.Diagnostics.Process.Start("TortoiseProc.exe", @"/command:update /path:" + paths + @" /notempfile /closeonend:1");// System.Diagnostics.Process.Start("CMD.exe",cmd); 
        newProcess.WaitForExit();
    }
    static string GetProtoSvnPath()
    {
        string paths = Application.dataPath + "/../../svnProtoPath.txt";//Assets/的上两层
        if (File.Exists(paths))
        {
            paths = File.ReadAllText(paths);
        }
        return paths;
    }
    static void DelectDir(string srcPath)
    {
        try
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo)            //判断是否文件夹
                {
                    DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                    subdir.Delete(true);          //删除子目录和文件
                }
                else
                {
                    File.Delete(i.FullName);      //删除指定文件
                }
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }
    static void CopyProto()//拷贝到项目文件夹下
    {
        string paths = GetProtoSvnPath();
        string destPath = Application.dataPath + path + "protocs";
        if (Directory.Exists(destPath))
        {
            DelectDir(destPath);
        }
        CopyProto(paths, destPath);
    }

    static void CopyProtoRelationJson()
    {
        string paths = GetProtoSvnPath()+ "/Relation";
        
        if (Directory.Exists(jsonDestPath))
        {
            DelectDir(jsonDestPath);
        }
        File.Copy(paths+"/"+ jsonName, jsonDestPath + "/" + jsonName, true);
    }

    [SerializeField]
    public class JsonProto
    {
        public string title;
        public int op;
        public string request;
        public string response;
    }
    [SerializeField]
    public class ListJsonProto
    {
        public List<JsonProto> listJsonProto;
    }
    static void GenerateProtoManagerAndRelationByProtoJson()
    {
        string pathJsonFile = jsonDestPath + "/" + jsonName;
        string jsonContent=File.ReadAllText(pathJsonFile);
        jsonContent= "{\"listJsonProto\":" + jsonContent + "}";
        ListJsonProto listJsonProto=JsonMapper.ToObject<ListJsonProto>(jsonContent);
        
        string contentProtoManager = @"using System;

namespace Game.Protocal
{
    public class ProtocalManager
    {

        private static ProtocalManager _instance;
        private ProtocalManager() { }

        public static ProtocalManager Instance()
        {
            if (_instance == null)
            {
                _instance = new ProtocalManager();
            }
            return _instance;
        }

        {0}
    }
}




";
        StringBuilder allSendMsg = new StringBuilder();
       
        for (int i = 0; i < listJsonProto.listJsonProto.Count; i++)
        {
            if (listJsonProto.listJsonProto[i].request== "IMessage")
            {
                //推送的，不放在发送里边
                continue;
            }
            string sendTemplate = @"public void Send{0}({0} {1}, Action<{2}> Response{2}CallBack, Action<ErrorInfo> errorCallBack, bool isShowDefaultTip=true)
        {
            OpCodeType opCodeType = ListOPRelation.GetOpCodeTypeByRequest <{0}> ();
            ProtoSendMethod.BusinessRequest<{2}>({1}, opCodeType, Response{2}CallBack, errorCallBack, isShowDefaultTip);
        }";
            sendTemplate = sendTemplate.Replace("{0}",$"{listJsonProto.listJsonProto[i].request}");
            sendTemplate = sendTemplate.Replace("{1}", $"{listJsonProto.listJsonProto[i].request.ToLower()}");
            sendTemplate = sendTemplate.Replace("{2}", $"{listJsonProto.listJsonProto[i].response}");
            sendTemplate += "\n\t\t";
            allSendMsg.Append(sendTemplate);
        }
        contentProtoManager = contentProtoManager.Replace("{0}", allSendMsg.ToString());
        string p = Application.dataPath + path + "Relation/ProtocalManager.cs";

        using (FileStream fs = new FileStream(p, FileMode.Create, FileAccess.Write))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(contentProtoManager);
            }
        }


        string contentOpRelation = @"using Google.Protobuf;
using System.Collections.Generic;

namespace Game.Protocal
{
    //三者1:1:1关系
    public class OPRelation
    {
        public OpCodeType codeType;
        public System.Type typeRequest;
        public System.Type typeResponse;
    }
    public class ListOPRelation
    {
        public static List<OPRelation> listOpRelation = new List<OPRelation>()
        {
            {0}
        };

        public static OpCodeType GetOpCodeTypeByRequest<T>() where T:IMessage
        {
            System.Type typeRequest = typeof(T);
            OPRelation opRelation=listOpRelation.Find((obj) => obj.typeRequest == typeRequest);
            return opRelation.codeType;
        }

        public static IMessage GetRealMessageByOpCodeType(OpCodeType codeType, byte[] data)
        {
            {1}
            return null;
        }
    }
}";

        StringBuilder allOPRealtion = new StringBuilder();
        StringBuilder allOPUnSer = new StringBuilder();

        for (int i = 0; i < listJsonProto.listJsonProto.Count; i++)
        {
            string template = @"new OPRelation(){codeType=(OpCodeType){0},typeRequest=typeof({1}),typeResponse=typeof({2})}";
            template = template.Replace("{0}", $"{listJsonProto.listJsonProto[i].op}");
            template = template.Replace("{1}", $"{listJsonProto.listJsonProto[i].request}");
            template = template.Replace("{2}", $"{listJsonProto.listJsonProto[i].response}");
            template += ",\n\t\t\t";
            allOPRealtion.Append(template);


            string templateUnSer = @"if ((int)codeType == {0})
            {
                return ProtoSerAndUnSer.UnSerialize<{1}>(data);
            }";
            templateUnSer = templateUnSer.Replace("{0}", $"{listJsonProto.listJsonProto[i].op}");
            templateUnSer = templateUnSer.Replace("{1}", $"{listJsonProto.listJsonProto[i].response}");
            templateUnSer += "\n\t\t\t";
            allOPUnSer.Append(templateUnSer);

        }

        
        contentOpRelation = contentOpRelation.Replace("{0}", allOPRealtion.ToString());
        contentOpRelation = contentOpRelation.Replace("{1}", allOPUnSer.ToString());
        string pOpRelation = Application.dataPath + path + "Relation/OPRelation.cs";

        using (FileStream fs = new FileStream(pOpRelation, FileMode.Create, FileAccess.Write))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(contentOpRelation);
            }
        }

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    static void CopyProto(string sourcePath, string targetPath)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(sourcePath);
        CopyProto(dirInfo, targetPath);
    }
    static void CopyProto(DirectoryInfo dirInfo, string targetPath)
    {
        foreach (var item in dirInfo.GetDirectories())
        {
            CopyProto(item, targetPath);
        }
        foreach (var item in dirInfo.GetFiles())
        {
            if (item.Extension.Trim() == ".proto")
            {
                File.Copy(item.FullName, targetPath + "/" + item.Name, true);
            }
        }
    }

}
