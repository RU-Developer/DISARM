using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * 상점에서 구매한 아이템들을 모듈로 장착할 수 있는 화면입니다.
 */
public class UI_Game_Menu_Module : UI_Scene
{
    private GameObject _player;
    private GameObject _inMemoryItemPanel;
    private GameObject _outMemoryItemPanel;

    private string _id;
    private string _itemName;
    private string _description;
    private string _imageSource;
    private int _memory = 0;
    private bool _equipped;

    enum Images
    {
        ItemLargeIcon
    }

    enum GameObjects
    {
        CollectionMenuHandler,
        InMemoryItems,
        OutMemoryItems,
        EquipButtonEventHandler
    }

    enum Texts
    {
        MemoryTotalText,
        ItemNameText,
        ItemDescriptionText,
        ItemMemoryText,
        EquipButtonText
    }

    private void QuitMenu()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        Managers.UI.CloseSceneUI<UI_Game_Menu_Module>();
        Managers.UI.ShowSceneUI<UI_Game>();
        Managers.Pause.Play();
    }

    public override void Init()
    {
        base.Init();
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));

        Managers.Input.AddUIKeyAction(QuitMenu);

        GetObject((int)GameObjects.CollectionMenuHandler).BindEvent(evt =>
        {
            Managers.UI.CloseSceneUI<UI_Game_Menu_Module>();
            Managers.UI.ShowSceneUI<UI_Game_Menu_Collection>();
        });

        GetObject((int)GameObjects.EquipButtonEventHandler).BindEvent(evt =>
        {
            // 장비 장착 및 해제
            if (string.IsNullOrEmpty(_id))
                return;

            bool success = false;

            if (_equipped)
                success = Managers.Game.GetPlayer().GetComponent<PlayerStatus>().UpdateMemory(-_memory);
            else
                success = Managers.Game.GetPlayer().GetComponent<PlayerStatus>().UpdateMemory(_memory);

            if (!success)
                return;

            Managers.Data.CollectionDict[_id].equipped = !_equipped;
            Refresh();
        });

        _player = Managers.Game.GetPlayer();

        _inMemoryItemPanel = GetObject((int)GameObjects.InMemoryItems);
        _outMemoryItemPanel = GetObject((int)GameObjects.OutMemoryItems);

        Refresh();
    }

    private void Refresh()
    {
        PlayerStatus status = _player.GetComponent<PlayerStatus>();
        GetText((int)Texts.MemoryTotalText).text = $"Memory Total: {status.Memory} / {status.MaxMemory}GB";

        foreach (Transform child in _inMemoryItemPanel.transform)
            Managers.Resource.Destroy(child.gameObject);

        foreach (Transform child in _outMemoryItemPanel.transform)
            Managers.Resource.Destroy(child.gameObject);

        //TODO: id만 넘기고, id를 통해서 아이템 정보를 DataManager에서 가져오는 방식으로 수정 가능할 듯.
        bool first = true;
        foreach (Data.Collection item in Managers.Data.CollectionDict.Values)
        {
            UI_Collection_Item itemUI;

            if (item.equipped)
            {
                itemUI = Managers.UI.MakeSubItem<UI_Collection_Item>(_inMemoryItemPanel.transform);
            }
            else
            {
                itemUI = Managers.UI.MakeSubItem<UI_Collection_Item>(_outMemoryItemPanel.transform);
            }
            
            itemUI.SetInfo(this, item.id, item.imageSrc, item.name, item.description, item.memory, item.equipped);

            if (first)
            {
                first = false;
                itemUI.OnClick();
            }
        }
    }

    public void SetInfo(string id, string imageSource, string itemName, string description, int memory, bool equipped)
    {
        _id = id;
        _imageSource = imageSource;
        _itemName = itemName;
        _description = description;
        _memory = memory;
        _equipped = equipped;

        GetText((int)Texts.ItemNameText).text = itemName;
        GetImage((int)Images.ItemLargeIcon).sprite = Managers.Resource.Load<Sprite>(imageSource);
        GetText((int)Texts.ItemDescriptionText).text = description;
        GetText((int)Texts.ItemMemoryText).text = $"Memory: {memory}GB";
        if (equipped)
            GetText((int)Texts.EquipButtonText).text = "장비 해제";
        else
            GetText((int)Texts.EquipButtonText).text = "장비 장착";
    }

    public override void Clear()
    {
        base.Clear();
        Managers.Input.RemoveUIKeyAction(QuitMenu);
    }
}
