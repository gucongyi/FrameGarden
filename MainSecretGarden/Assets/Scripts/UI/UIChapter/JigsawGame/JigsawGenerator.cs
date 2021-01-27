using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 拼图块生成器（切割、分散等一系列操作）
/// </summary>
public class JigsawGenerator : MonoBehaviour
{
    //所有的拼图块
    private List<JigsawTile> _allJigsawTiles = new List<JigsawTile>();
    public List<JigsawTile> AllJigsawTiles
    {
        get
        {
            return _allJigsawTiles;
        }
    }
    //是否过关
    public bool isReachAStandard = false;
    //当前操作的拼图块
    private JigsawTile _curJigsawTile;
    //生成的图块对象
    public GameObject tileItem;
    //显示的图片名称
    public string _spriteName;

    Canvas _canvas;
    public GameObject tileParent;

    private void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
    }

    Vector2 lastPos = Vector2.zero;
    List<JigsawTile> tempList = new List<JigsawTile>();
    //遍历已经合成的图块
    void Recursive(JigsawTile tree)
    {
        if (!tree.visited && tree.tilesTree.Count > 0)
        {
            tree.visited = true;
            foreach (var item in tree.tilesTree)
            {
                Recursive(item);
            }
        }
        if (!tempList.Contains(tree))
            tempList.Add(tree);
    }
    /// <summary>
    /// 初始图的遍历记录
    /// </summary>
    void InitTile()
    {
        foreach (var item in _allJigsawTiles)
        {
            item.visited = false;
        }
    }

    public void FinishTileMove()
    {
        tempList.Clear();
        Recursive(_curJigsawTile);
        InitTile();
    }

    /// <summary>
    /// 切割
    /// </summary>
    public void Split(int rc)
    {
        int count = 1;
        for (int r = 0; r < rc; r++)
        {
            for (int c = 0; c < rc; c++)
            {
                GameObject go = Instantiate(tileItem);
                go.name = $"tile{r}{c}";

                JigsawTile tile = go.GetComponent<JigsawTile>();
                tile.id = count++;
                tile.DragEvent = () => { _curJigsawTile = tile; lastPos = tile._thisRect.anchoredPosition; };//拖拽事件
                tile.AllMove = FinishTileMove;
                _allJigsawTiles.Add(tile);
                go.transform.SetParent(this.transform, false);
            }
        }
    }

    /// <summary>
    /// 洗牌-给每个小图块都分配合理的边界
    /// </summary>
    public void Shuffle(int rc)
    {
        float tileSize = (this.transform as RectTransform).rect.width / rc;

        for (int r = 0; r < rc; r++)
        {
            for (int c = 0; c < rc; c++)
            {
                char up = GetMod(rc, r, c, r + 1, c, Direction.down);//获取相邻上方图块的下边缘值
                char right = GetMod(rc, r, c, r, c + 1, Direction.left);
                char down = GetMod(rc, r, c, r - 1, c, Direction.up);
                char left = GetMod(rc, r, c, r, c - 1, Direction.right);

                string modName = $"{up}{right}{down}{left}";
                _allJigsawTiles[r * rc + c].SetMask(modName, tileSize, _spriteName);

                //对应的正确位置
                Vector2 perfect = new Vector2(c * tileSize + tileSize / 2, r * tileSize + tileSize / 2);
                //所有图块一开始的位置给玩家看整图（之后再打乱）
                _allJigsawTiles[r * rc + c].SetPosition(perfect);
                //前置做好了最后在替换父物体
                _allJigsawTiles[r * rc + c].SetTileParent();
                //提前设置所有图块的相对位置
                InitPosList(rc, r * rc + c + 1, tileSize);
            }
        }
    }
    //相对位置列表
    List<TileInitPos> posList = new List<TileInitPos>();
    public void InitPosList(int rc, int id, float tileSize)
    {
        TileInitPos p = new TileInitPos();
        p.id = id;//1-16
        for (int r = 0; r < rc; r++)
        {
            for (int c = 0; c < rc; c++)
            {
                Dictionary<int, Vector2> dic = new Dictionary<int, Vector2>();
                switch (id)
                {
                    case 1://1行
                        dic.Add(r * rc + c + 1, new Vector2(c * tileSize, r * tileSize));
                        break;
                    case 2:
                        dic.Add(r * rc + c + 1, new Vector2(c * tileSize - tileSize, r * tileSize));
                        break;
                    case 3:
                        dic.Add(r * rc + c + 1, new Vector2(c * tileSize - tileSize * 2, r * tileSize));
                        break;
                    case 4:
                        dic.Add(r * rc + c + 1, new Vector2(c * tileSize - tileSize * 3, r * tileSize));
                        break;
                    case 5://2行
                        dic.Add(r * rc + c + 1, new Vector2(c * tileSize, r * tileSize - tileSize));
                        break;
                    case 6:
                        dic.Add(r * rc + c + 1, new Vector2(c * tileSize - tileSize, r * tileSize - tileSize));
                        break;
                    case 7:
                        dic.Add(r * rc + c + 1, new Vector2(c * tileSize - tileSize * 2, r * tileSize - tileSize));
                        break;
                    case 8:
                        dic.Add(r * rc + c + 1, new Vector2(c * tileSize - tileSize * 3, r * tileSize - tileSize));
                        break;
                    case 9://3行
                        dic.Add(r * rc + c + 1, new Vector2(c * tileSize, r * tileSize - tileSize * 2));
                        break;
                    case 10:
                        dic.Add(r * rc + c + 1, new Vector2(c * tileSize - tileSize, r * tileSize - tileSize * 2));
                        break;
                    case 11:
                        dic.Add(r * rc + c + 1, new Vector2(c * tileSize - tileSize * 2, r * tileSize - tileSize * 2));
                        break;
                    case 12:
                        dic.Add(r * rc + c + 1, new Vector2(c * tileSize - tileSize * 3, r * tileSize - tileSize * 2));
                        break;
                    case 13://4
                        dic.Add(r * rc + c + 1, new Vector2(c * tileSize, r * tileSize - tileSize * 3));
                        break;
                    case 14:
                        dic.Add(r * rc + c + 1, new Vector2(c * tileSize - tileSize, r * tileSize - tileSize * 3));
                        break;
                    case 15:
                        dic.Add(r * rc + c + 1, new Vector2(c * tileSize - tileSize * 2, r * tileSize - tileSize * 3));
                        break;
                    case 16:
                        dic.Add(r * rc + c + 1, new Vector2(c * tileSize - tileSize * 3, r * tileSize - tileSize * 3));
                        break;
                }
                p.pos.Add(dic);
            }
        }
        posList.Add(p);
    }

    /// <summary>
    /// 分配对应正确的边界mask
    /// </summary>
    /// <param name="rc">图块被分为的几行几列</param>
    /// <param name="r">要生成的是哪个图块</param>
    /// <param name="c">要生成的是哪个图块</param>
    /// <param name="side_r2">相邻图块</param>
    /// <param name="side_c2">相邻图块</param>
    /// <param name="direction">当前图块位于相邻图块的方向</param>
    char GetMod(int rc, int r, int c, int side_r2, int side_c2, Direction direction)
    {
        if (side_r2 < 0 || side_r2 >= rc || side_c2 < 0 || side_c2 >= rc)
            return 'f';
        //将所有相邻图块赋给当前图块
        switch (direction)
        {
            case Direction.up:
                _allJigsawTiles[r * rc + c].adjacentTile.downTileid = _allJigsawTiles[side_r2 * rc + side_c2].id;
                break;
            case Direction.right:
                _allJigsawTiles[r * rc + c].adjacentTile.leftTileid = _allJigsawTiles[side_r2 * rc + side_c2].id;
                break;
            case Direction.down:
                _allJigsawTiles[r * rc + c].adjacentTile.upTileid = _allJigsawTiles[side_r2 * rc + side_c2].id;
                break;
            case Direction.left:
                _allJigsawTiles[r * rc + c].adjacentTile.rightTileid = _allJigsawTiles[side_r2 * rc + side_c2].id;
                break;
        }

        //左下角是(0,0)
        string maskName = _allJigsawTiles[side_r2 * rc + side_c2].GetMaskName();

        //如果相邻的图块还没设置过边界，就给(r,c)随机设置一个边界
        //如果相邻的图块已经设置了边界，就给(r,c)设置成相反的
        if (string.IsNullOrEmpty(maskName))
        {
            return JigsawGameHelper.Random();
        }
        else
        {
            int index = JigsawGameHelper.Direction2Index(direction);
            char ch = maskName[index];
            ch = JigsawGameHelper.Reversal(ch);
            return ch;
        }
    }

    //清除所有拼图块
    public void Clear()
    {
        for (int i = 0; i < _allJigsawTiles.Count; ++i)
        {
            Destroy(_allJigsawTiles[i].gameObject);
        }
        _allJigsawTiles.Clear();
    }

    //分散图块 
    public void Disperse()
    {//之后用协程
        StartCoroutine(Deal());
    }
    IEnumerator Deal()
    {
        for (int i = 0; i < _allJigsawTiles.Count; i++)
        {
            yield return new WaitForSeconds(0.05f);
            Vector2 v2 = new Vector2(Random.Range(4, 996), Random.Range(-172, 1500));
            while (_allJigsawTiles[i]._thisRect.anchoredPosition != v2)
            {
                _allJigsawTiles[i]._thisRect.anchoredPosition = Vector2.MoveTowards(_allJigsawTiles[i]._thisRect.anchoredPosition, v2, 100f);
                yield return null;
            }

        }
    }


    /// <summary>
    /// 获取tile2当对于tile1的位置
    /// </summary>
    /// <param name="tile1ID">tile1的id</param>
    /// <param name="tile2ID">tile2的id</param>
    /// <returns>返回tile2当对于tile1的位置</returns>
    public Vector2 GetTilePos(int tile1ID, int tile2ID)
    {
        foreach (var item in posList)
        {
            if (item.id == tile1ID)
            {
                foreach (var temp in item.pos)
                {
                    if (temp.ContainsKey(tile2ID))
                    {
                        return temp[tile2ID];
                    }
                }
            }
        }
        return Vector2.zero;
    }

    /// <summary>
    /// oh is good
    /// </summary>
    /// <param name="tile1">thisTile</param>
    /// <param name="tile2">item</param>
    public void Joint(JigsawTile tile1, JigsawTile tile2)
    {
        if (tile2.compound)
        {
            tile1.compound = true;
            tile1.transform.SetParent(tile2.parentTile.transform);
            tile1.parentTile = tile2.parentTile;
            if (!tile1.parentTile.curentJigsawTile.Contains(tile1))
                tile1.parentTile.curentJigsawTile.Add(tile1);
            tile1.SetPosition(tile2.GetPosition() + GetTilePos(tile2.id, tile1.id));
            tile1.parentTile.IsReachAStandard?.Invoke();
            return;
        }

        //赋值相对位置
        tile1.compound = true;
        tile2.compound = true;
        GameObject parent = Instance();
        tile1.parentTile = parent.GetComponent<JigsawTileParent>();
        tile2.parentTile = tile1.parentTile;
        if (!tile1.parentTile.curentJigsawTile.Contains(tile1))
            tile1.parentTile.curentJigsawTile.Add(tile1);
        if (!tile1.parentTile.curentJigsawTile.Contains(tile2))
            tile1.parentTile.curentJigsawTile.Add(tile2);
        //Set
        (parent.transform as RectTransform).anchoredPosition = tile1._thisRect.anchoredPosition;
        (parent.transform as RectTransform).sizeDelta = tile1._thisRect.sizeDelta;
        //setParent
        tile1.transform.SetParent(parent.transform);
        tile2.transform.SetParent(parent.transform);
        tile2.SetPosition(tile1.GetPosition() + GetTilePos(tile1.id, tile2.id));
        tile2.parentTile.IsReachAStandard?.Invoke();
    }

    GameObject Instance()
    {
        GameObject parent = Instantiate(tileParent);
        //Set
        parent.name = "tileParent";
        parent.transform.SetParent(transform);
        parent.transform.localScale = Vector3.one;
        return parent;
    }
}
//存放所有点及其他对应点的位置
public class TileInitPos
{
    public int id;
    public List<Dictionary<int, Vector2>> pos = new List<Dictionary<int, Vector2>>();
}
//方向
public enum Direction
{
    up,
    right,
    down,
    left
}
//帮助方法
public class JigsawGameHelper
{
    /// <summary>
    /// 根据旁边图块的边缘 颠倒 成对应边缘图块
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static char Reversal(char c)
    {
        switch (c)
        {
            case 'f': return 'f';
            case 'v': return 'a';
            case 'a': return 'v';
            default: return 'o';
        }
    }

    /// <summary>
    /// 随机边
    /// </summary>
    public static char Random()
    {
        int rand = UnityEngine.Random.Range(0, 2);
        switch (rand)
        {
            case 0: return 'a';
            case 1: return 'v';
            default: return 'o';
        }
    }

    /// <summary>
    /// 方向转为下标
    /// </summary>
    public static int Direction2Index(Direction direction)
    {//上右下左  0123
        switch (direction)
        {
            case Direction.up:
                return 0;
            case Direction.right:
                return 1;
            case Direction.down:
                return 2;
            case Direction.left:
                return 3;
            default: return -1;
        }
    }

    /// <summary>
    /// 顺时针旋转90度后的 上右下左的样子
    /// avva->aavv
    /// </summary>
    static public string ClockwiseRotation90(string mode)
    {
        return $"{mode[3]}{mode[0]}{mode[1]}{mode[2]}";
    }
}
