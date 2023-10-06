using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inven_Item : UI_Base
{
    enum GameObjects
    {
        ItemIcon,
        ItemNameText,
        EventHandler
    }

    string _name;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        GetObject((int)GameObjects.ItemNameText).GetComponent<Text>().text = _name;

        GetObject((int)GameObjects.EventHandler)
            .BindEvent((PointerEventData) => { Debug.Log($"Item Clicked {_name}"); });
    }

    public void SetInfo(string name)
    {
        _name = name;
    }
}
