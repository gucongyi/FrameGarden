﻿using System.IO;
using UnityEditor;
using UnityEngine;

public static class BuildHelper
{
    private const string relativeDirPrefix = "../Release";

    public static string BuildFolder = "../Release/{0}/StreamingAssets/";

    [MenuItem("Tools/web资源服务器")]
    public static void OpenFileServer()
    {
        string currentDir = System.Environment.CurrentDirectory;
        string path = Path.Combine(currentDir, @"..\FileServer\");
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "FileServer.exe";
        process.StartInfo.WorkingDirectory = path;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
    }

    public static void Build(PlatformType type, BuildAssetBundleOptions buildAssetBundleOptions, BuildOptions buildOptions,bool isContainAB)
    {
        BuildTarget buildTarget = BuildTarget.StandaloneWindows;
        string exeName = "ET";
        switch (type)
        {
            case PlatformType.PC:
                buildTarget = BuildTarget.StandaloneWindows;
                exeName += ".exe";
                break;
            case PlatformType.Android:
                buildTarget = BuildTarget.Android;
                exeName += ".apk";
                break;
            case PlatformType.IOS:
                buildTarget = BuildTarget.iOS;
                break;
        }

        string fold = string.Format(BuildFolder, type);
        if (!Directory.Exists(fold))
        {
            Directory.CreateDirectory(fold);
        }

        Debug.Log("开始资源打包");
        BuildPipeline.BuildAssetBundles(fold, buildAssetBundleOptions, buildTarget);

        //CopyVideo(fold, "Assets/Videos/" + type);

        GenerateVersionInfo(fold);
        Debug.Log("完成资源打包");

        if (isContainAB)
        {
            FileHelper.CleanDirectory("Assets/StreamingAssets/");
            FileHelper.CopyDirectory(fold, "Assets/StreamingAssets/");
        }
    }

    static void CopyVideo(string dir, string sourceVideoDir)
    {
        if (Directory.Exists(sourceVideoDir))
        {
            foreach (var file in new DirectoryInfo(sourceVideoDir).GetFiles())
            {
                if (file.Extension.ToLower().Equals(".meta"))
                    continue;
                File.Copy(file.FullName, Path.Combine(dir, file.Name), true);
            }
        }
    }

    private static void GenerateVersionInfo(string dir)
    {
        VersionConfig versionProto = new VersionConfig();
        GenerateVersionProto(dir, versionProto, "");

        using (FileStream fileStream = new FileStream($"{dir}/Version.txt", FileMode.Create))
        {
            byte[] bytes = JsonHelper.ToJson(versionProto).ToByteArray();
            fileStream.Write(bytes, 0, bytes.Length);
        }
    }

    private static void GenerateVersionProto(string dir, VersionConfig versionProto, string relativePath)
    {
        foreach (string file in Directory.GetFiles(dir))
        {
            if (file.EndsWith(".manifest"))
            {
                continue;
            }
            string md5 = MD5Helper.FileMD5(file);
            FileInfo fi = new FileInfo(file);
            long size = fi.Length;
            string filePath = relativePath == "" ? fi.Name : $"{relativePath}/{fi.Name}";

            versionProto.FileInfoDict.Add(filePath, new FileVersionInfo
            {
                File = filePath,
                MD5 = md5,
                Size = size,
            });
        }

        foreach (string directory in Directory.GetDirectories(dir))
        {
            DirectoryInfo dinfo = new DirectoryInfo(directory);
            string rel = relativePath == "" ? dinfo.Name : $"{relativePath}/{dinfo.Name}";
            GenerateVersionProto($"{dir}/{dinfo.Name}", versionProto, rel);
        }
    }
}