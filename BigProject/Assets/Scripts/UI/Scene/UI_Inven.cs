using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * 인게임 실시간으로 열리는 인벤토리
 */
public class UI_Inven : UI_Scene
{
    enum GameObjects
    {
        GridPanel
    }

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        GameObject gridPanel = GetObject((int)GameObjects.GridPanel);

        foreach (Transform child in gridPanel.transform)
            Managers.Resource.Destroy(child.gameObject);

        //임시로 8개 아이템 목록 출력
        int count = 8;
        for (int i = 0; i < count; i++)
        {
            UI_Inven_Item invenItem = Managers.UI.MakeSubItem<UI_Inven_Item>(gridPanel.transform);
            invenItem.SetInfo($"Item Name {i}");
        }

        //동적으로 grid layout 및 셀 사이즈 조절
        //GridLayoutGroup grid = gridPanel.GetComponent<GridLayoutGroup>();
        //RectTransform rectTransform = gridPanel.GetComponent<RectTransform>();

        //float originWidth = rectTransform.rect.width;
        //float originHeight = rectTransform.rect.height;

        //int minColsInARow = 3;
        //int maxRow = 6;

        //int rows = Mathf.Clamp(Mathf.CeilToInt((float)count / minColsInARow), 1, maxRow + 1);
        //int cols = Mathf.CeilToInt((float)count / rows);

        //float spaceWidth = (grid.padding.left + grid.padding.right) + (grid.spacing.x * (cols - 1));
        //float spaceHeight = (grid.padding.top + grid.padding.bottom) + (grid.spacing.y * (rows - 1));

        //float maxWidth = originWidth - spaceWidth;
        //float maxHeight = originHeight - spaceHeight;

        //float size = Mathf.Min(maxWidth / cols, maxHeight / rows);

        //grid.cellSize = new Vector2(maxWidth / cols, maxHeight / rows);
        //grid.cellSize = new Vector2(size, size);
        //grid.cellSize = new Vector2(75, 75);
    }
}
