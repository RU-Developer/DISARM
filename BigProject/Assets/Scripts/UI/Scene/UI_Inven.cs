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
    }
}
