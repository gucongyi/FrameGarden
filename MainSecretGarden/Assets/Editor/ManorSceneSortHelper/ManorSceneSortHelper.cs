using Generate;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Generate
{
    [SerializeField]
    public class SCManorStruct
    {
        // 种植地id
        public long SoilId;
        // 地类型
        public Game.Protocal.ManorScene SoilType;
        // 种植道具id
        public long CropGoodId;
        // 当前产量
        public int Yield;
        // 总产量
        public int TotalYield;
        // 状态
        public int SoilStatus;
        // x轴信息
        public float Xaxle;
        // y轴信息
        public float Yaxle;
        // 成熟时间
        public long AdultnessTime;
        //属于哪个大地块
        public int ParcelDivision;
    }
    [SerializeField]
    public class ManorInitInfo
    {
        public List<SCManorStruct> listManorTree = new List<SCManorStruct>();
    }
}

public class ManorSceneSortHelper : Editor
{
   [MenuItem("ManorHelper/GenerateSorting On Region")]
   public static void GenerateSortingOnManorScene()
    {
        GameObject rootSorting = GameObject.Find("Root2DScene/ScrollSceneRoot/AllManorPlant");
        var tilesCom = rootSorting.GetComponentsInChildren<TileComponent>(true);
        List<TileComponent> listChildActive = new List<TileComponent>();
        for (int i = 0; i < tilesCom.Length; i++)
        {
            listChildActive.Add(tilesCom[i]);
        }
        //给显示的排序
        listChildActive.Sort((a, b) =>
        {
            //用Y坐标来排序
            float yA = a.transform.position.y;
            float yB = b.transform.position.y;
            float xA = a.transform.position.x;
            float xB = b.transform.position.x;
            float delta = 0.003f;
            if (yA > (yB + delta))
            {
                return -1;
            }
            else if (yA < (yB + delta))
            {
                return 1;//y降序
            }
            else
            {
                if (xA > xB)
                {
                    return 1;//x 升序
                }
                else if (xA < xB)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
        });

        //设置层级
        for (int i = 0; i < listChildActive.Count; i++)
        {
            var tileCom = listChildActive[i];
            if (tileCom.RenderPlantList.Count > 0)
            {//地块上植物
                for (int len = 0; len < tileCom.RenderPlantList.Count; len++)
                {
                    tileCom.RenderPlantList[len].sortingOrder = 20 + 1 * i + len;
                    //地砖统一19
                    if (tileCom.name.Contains("Decorate_dizhuan"))
                    {
                        tileCom.RenderPlantList[len].sortingOrder = 19;
                    }
                }
            }
            if (tileCom.RenderTile != null)
            {//地块上植物
                tileCom.RenderTile.sortingOrder = 11;
            }
        }
        //生成json给服务器
        GenerateServerInfo(listChildActive);

        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    private static void GenerateServerInfo(List<TileComponent> listChildActive)
    {
        List<Generate.SCManorStruct> listManorTree = new List<SCManorStruct>();
        for (int i = 0; i < listChildActive.Count; i++)
        {
            if (listChildActive[i].typeTile == Company.Cfg.TypeManorDecorate.Tile
                || listChildActive[i].typeTile == Company.Cfg.TypeManorDecorate.Decorate
                /*|| listChildActive[i].typeTile == Company.Cfg.TypeManorDecorate.GiftBox*/
                || listChildActive[i].typeTile == Company.Cfg.TypeManorDecorate.DogHouse)
            {
                SCManorStruct csManorStruct = new SCManorStruct();
                csManorStruct.SoilType = (Game.Protocal.ManorScene)listChildActive[i].typeTile;
                csManorStruct.CropGoodId = listChildActive[i].CropGoodId;
                csManorStruct.Xaxle = listChildActive[i].transform.localPosition.x;
                csManorStruct.Yaxle = listChildActive[i].transform.localPosition.y;
                csManorStruct.ParcelDivision = listChildActive[i].regionId;
                listManorTree.Add(csManorStruct);
                listChildActive[i].gameObject.SetActive(false);
            }
        }
        //增加NPC
        TileComponent TileNpc = GameObject.Find("Root2DScene/FemaleManor").GetComponent<TileComponent>();
        SCManorStruct csManorStructNpc = new SCManorStruct();
        csManorStructNpc.SoilType = Game.Protocal.ManorScene.Npc;
        csManorStructNpc.Xaxle = TileNpc.transform.localPosition.x;
        csManorStructNpc.Yaxle = TileNpc.transform.localPosition.y;
        listManorTree.Add(csManorStructNpc);

        string jsonInitTreesInScenes = LitJson.JsonMapper.ToJson(listManorTree);
        StaticData.isShowSelfLog = true;
        StaticData.DebugGreen($"====jsonInitTreesInScenes:{jsonInitTreesInScenes}====");
    }
}
