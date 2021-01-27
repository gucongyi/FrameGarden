using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantSeedDragComponent : MonoBehaviour
{
    public bool isDrag;
    Vector2 offsetPos;
    UIPlantItem uiPlantItem;
    public void BeginDrag(UIPlantItem uiPlantItem)
    {
        uiPlantItem.ResetDragNeedSeedCount();
        Root2dSceneManager._instance.isTileDrag = true;
        isDrag = true;
        this.uiPlantItem = uiPlantItem;
        SetDragImagePos(true);
    }


    // Update is called once per frame
    void Update()
    {
        if (isDrag && Input.GetMouseButton(0))//鼠标按住处理,拖拽过程中
        {
            SetDragImagePos(false);
            uiPlantItem.HandlePlant();
        }
        if (isDrag&&Input.GetMouseButtonUp(0))
        {
            uiPlantItem.OnDragUp();
        }
    }

    private void SetDragImagePos(bool isSetOriginPos)
    {
        //获取鼠标位置
        Vector2 mousePos = Input.mousePosition;
        
        transform.GetComponent<RectTransform>().anchoredPosition = StaticData.ScreenPointToUICameraAnchorPos(mousePos);
    }
}
