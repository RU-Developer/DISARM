using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Game_Menu_Collection : UI_Scene
{
    enum GameObjects
    {
        ModuleMenuHandler,
        WeaponMenuHandler,
        Items
    }

    enum Images
    {
        ItemLargeIcon
    }

    enum Texts
    {
        ItemNameText,
        ItemDescriptionText,
        ItemMemoryText
    }

    private GameObject _itemPanel;

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));

        BindEvent(GetObject((int)GameObjects.ModuleMenuHandler),
            evt =>
            {
                Managers.UI.CloseSceneUI<UI_Game_Menu_Collection>();
                Managers.UI.ShowSceneUI<UI_Game_Menu_Module>();
            });

        BindEvent(GetObject((int)GameObjects.WeaponMenuHandler),
            evt =>
            {
                Managers.UI.CloseSceneUI<UI_Game_Menu_Collection>();
                Managers.UI.ShowSceneUI<UI_Game_Menu_Weapon>();
            });

        _itemPanel = GetObject((int)GameObjects.Items);

        Refresh();
    }

    private void Update()
    {
        if (Managers.Input.GetInputDown(Define.InputType.Menu))
        {
            Managers.UI.CloseSceneUI<UI_Game_Menu_Collection>();
            Managers.UI.ShowSceneUI<UI_Game>();
            Managers.Pause.Play();
        }
    }

    private void Refresh()
    {
        foreach (Transform child in _itemPanel.transform)
            Managers.Resource.Destroy(child.gameObject);

        //TODO: id만 넘기고, id를 통해서 아이템 정보를 DataManager에서 가져오는 방식으로 수정 가능할 듯.
        bool first = true;
        foreach (Data.Collection item in Managers.Data.CollectionDict.Values)
        {
            UI_Collection_Item itemUI = Managers.UI.MakeSubItem<UI_Collection_Item>(_itemPanel.transform);
            itemUI.SetInfo(this, item.id, item.imageSrc, item.name, item.description, item.memory);

            if (first)
            {
                first = false;
                itemUI.OnClick();
            }
        }
    }

    public void SetInfo(string imageSource, string itemName, string description, int memory)
    {
        GetText((int)Texts.ItemNameText).text = itemName;
        GetImage((int)Images.ItemLargeIcon).sprite = Managers.Resource.Load<Sprite>(imageSource);
        GetText((int)Texts.ItemDescriptionText).text = description;
        GetText((int)Texts.ItemMemoryText).text = $"Memory: {memory}GB";
    }

    public override void Clear()
    {
        base.Clear();
    }
}
