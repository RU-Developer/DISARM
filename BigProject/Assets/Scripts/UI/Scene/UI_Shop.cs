using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Shop : UI_Scene
{
    private string _id;
    private string _itemName;
    private string _description;
    private string _imageSource;
    private int _cost = 0;
    private int _memory = 0;
    private GameObject _itemPanel;

    private void QuitMenu()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        Managers.UI.CloseSceneUI<UI_Shop>();
        Managers.UI.ShowSceneUI<UI_Game>();
        Managers.Pause.Play();
    }

    enum GameObjects
    {
        PurchaseEventHandler,
        QuitEventHandler,
        ItemPanel
    }

    enum Images
    {
        ItemLargeIcon
    }

    enum Texts
    {
        ItemNameText,
        ItemDescriptionText,
        ItemCost,
        ItemMemoryText
    }

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));

        Managers.Input.AddUIKeyAction(QuitMenu);

        BindEvent(GetObject((int)GameObjects.PurchaseEventHandler), evt =>
        {
            if (string.IsNullOrEmpty(_id))
                return;

            bool purchase = Managers.Game.GetPlayer().GetOrAddComponent<PlayerStatus>().UpdateMoney(-_cost);
            if (purchase == false)
                return;

            Debug.Log($"{_itemName}을 {_cost}bit에 구입하였습니다.");
            Data.Collection collection = new Data.Collection();
            collection.id = _id;
            collection.name = _itemName;
            collection.description = _description;
            collection.imageSrc = _imageSource;
            collection.memory = _memory;
            collection.equipped = false;
            Managers.Data.CollectionDict.Add(_id, collection);
            Managers.Data.ShopItemDict.Remove(_id);
            Refresh();
        });

        BindEvent(GetObject((int)GameObjects.QuitEventHandler), evt =>
        {
            QuitMenu();
        });

        _itemPanel = GetObject((int)GameObjects.ItemPanel);

        Refresh();
    }

    private void Refresh()
    {
        SetInfo(null, null, null, null, 0, 0);
        foreach (Transform child in _itemPanel.transform)
            Managers.Resource.Destroy(child.gameObject);

        //TODO: id만 넘기고, id를 통해서 아이템 정보를 DataManager에서 가져오는 방식으로 수정 가능할 듯.
        bool first = true;
        foreach (Data.ShopItem item in Managers.Data.ShopItemDict.Values)
        {
            UI_Collection_Item itemUI = Managers.UI.MakeSubItem<UI_Collection_Item>(_itemPanel.transform);
            itemUI.SetInfo(this, item.id, item.imageSrc, item.name, item.description, item.cost, item.memory);

            if (first)
            {
                first = false;
                itemUI.OnClick();
            }
        }
    }

    public void SetInfo(string id, string itemName, string imageSource, string description, int cost, int memory)
    {
        _id = id;
        _cost = cost;
        _memory = memory;
        _itemName = itemName;
        _description = description;
        _imageSource = imageSource;

        GetText((int)Texts.ItemNameText).text = itemName;
        GetImage((int)Images.ItemLargeIcon).sprite = Managers.Resource.Load<Sprite>(imageSource);
        GetText((int)Texts.ItemDescriptionText).text = description;
        GetText((int)Texts.ItemCost).text = $"가격: {cost}bit";
        GetText((int)Texts.ItemMemoryText).text = $"Memory: {memory}GB";
    }

    public override void Clear()
    {
        base.Clear();
        Managers.Input.RemoveUIKeyAction(QuitMenu);
    }
}
