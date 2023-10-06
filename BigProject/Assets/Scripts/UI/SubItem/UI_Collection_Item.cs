using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/**
 * Collection과 Shop에서 판매하는 아이템에 사용됨.
 */
public class UI_Collection_Item : UI_Base
{
    UI_Game_Menu_Weapon _weapon;
    UI_Game_Menu_Module _module;
    UI_Game_Menu_Collection _menu;
    UI_Shop _shop;

    private string _id;
    private string _imageSource;
    private string _itemName;
    private string _description;
    private int _cost;
    private int _memory;
    private bool _equipped;

    enum Images
    {
        ItemIcon,
        ItemHandler
    }

    public override void Init()
    {
        if (GetImage((int)Images.ItemIcon) == null)
            Bind<Image>(typeof(Images));
    }

    /**
     * 무기 정보 저장
     */
    public void SetInfo(UI_Game_Menu_Weapon weapon, string weaponId)
    {
        Init();

        if (_weapon != null)
            return;

        _weapon = weapon;
        _id = weaponId;
        _imageSource = Managers.Data.IconDict[weaponId].path;

        BindEvent(GetImage((int)Images.ItemHandler).gameObject,
            evt => _weapon.SetInfo(weaponId));

        GetImage((int)Images.ItemIcon).sprite = Managers.Resource.Load<Sprite>(_imageSource);
    }

    public void SetInfo(UI_Game_Menu_Collection menu, string id, string imageSource, string itemName, string description, int memory)
    {
        Init();

        if (_menu != null)
            return;

        _menu = menu;

        _id = id;
        _imageSource = imageSource;
        _itemName = itemName;
        _description = description;
        _memory = memory;

        BindEvent(GetImage((int)Images.ItemHandler).gameObject,
            evt => _menu.SetInfo(_imageSource, _itemName, _description, _memory));

        GetImage((int)Images.ItemIcon).sprite = Managers.Resource.Load<Sprite>(_imageSource);
    }

    public void SetInfo(UI_Game_Menu_Module module, string id, string imageSource, string itemName, string description, int memory, bool equipped)
    {
        Init();

        if (_module != null)
            return;

        _module = module;
        _equipped = equipped;

        _id = id;
        _imageSource = imageSource;
        _itemName = itemName;
        _description = description;
        _memory = memory;

        BindEvent(GetImage((int)Images.ItemHandler).gameObject,
            evt => _module.SetInfo(_id, _imageSource, _itemName, _description, _memory, _equipped));

        GetImage((int)Images.ItemIcon).sprite = Managers.Resource.Load<Sprite>(_imageSource);
    }

    public void SetInfo(UI_Shop shop, string id, string imageSource, string itemName, string description, int cost, int memory)
    {
        Init();

        if (_shop != null)
            return;

        _shop = shop;
        _cost = cost;

        _id = id;
        _imageSource = imageSource;
        _itemName = itemName;
        _description = description;
        _memory = memory;

        BindEvent(GetImage((int)Images.ItemHandler).gameObject,
            evt => _shop.SetInfo(_id, _itemName, _imageSource, _description, _cost, _memory));

        GetImage((int)Images.ItemIcon).sprite = Managers.Resource.Load<Sprite>(_imageSource);
    }

    public void OnClick()
    {
        GetImage((int)Images.ItemHandler)
            .GetComponent<UI_EventHandler>()
            .OnPointerClick();
    }
}
