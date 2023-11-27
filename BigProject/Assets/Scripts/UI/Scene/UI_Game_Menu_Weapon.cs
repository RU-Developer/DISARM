using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Game_Menu_Weapon : UI_Scene
{
    private GameObject _player;
    private GameObject _weapons;

    private string _id;

    enum GameObjects
    {
        CollectionMenuHandler,
        MenuHandler,
        Weapons,
        ArmButtonHandler
    }

    enum Texts
    {
        WeaponNameText,
        WeaponDescriptionText,
        WeaponAttackText,
        ArmButtonText
    }

    enum Images
    {
        WeaponLargeIcon,
        WeaponSlotIcon
    }

    private void Update()
    {
        if (Managers.Input.GetInputDown(Define.InputType.Menu))
        {
            Managers.UI.CloseSceneUI<UI_Game_Menu_Weapon>();
            Managers.UI.ShowSceneUI<UI_Game>();
            Managers.Pause.Play();
        }
    }

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        BindEvent(GetObject((int)GameObjects.CollectionMenuHandler),
            evt =>
            {
                Managers.UI.CloseSceneUI<UI_Game_Menu_Weapon>();
                Managers.UI.ShowSceneUI<UI_Game_Menu_Collection>();
            });

        BindEvent(GetObject((int)GameObjects.MenuHandler),
            evt =>
            {
                Managers.UI.CloseSceneUI<UI_Game_Menu_Weapon>();
                Managers.UI.ShowSceneUI<UI_Game_Menu>();
            });

        BindEvent(GetObject((int)GameObjects.ArmButtonHandler),
            evt =>
            {
                if (string.IsNullOrEmpty(_id))
                    return;

                PlayerStatus status = _player.GetComponent<PlayerStatus>();
                if (status.Weapon.Equals(_id))
                    status.UnarmWeapon();
                else
                    status.ChangeWeapon(_id);

                Refresh();
            });

        _player = Managers.Game.GetPlayer();
        _weapons = GetObject((int)GameObjects.Weapons);

        Refresh();
    }

    private void Refresh()
    {
        PlayerStatus status = _player.GetComponent<PlayerStatus>();
        GetImage((int)Images.WeaponSlotIcon).sprite =
            Managers.Resource.Load<Sprite>(Managers.Data.IconDict[status.Weapon].path);

        foreach (Transform child in _weapons.transform)
            Managers.Resource.Destroy(child.gameObject);

        bool first = true;
        foreach (Data.WeaponData weapon in Managers.Data.WeaponDict.Values)
        {
            // 소유하지 않은 무기는 보여주지 않음
            if (!weapon.own)
                continue;

            UI_Collection_Item weaponUI = Managers.UI.MakeSubItem<UI_Collection_Item>(_weapons.transform);
            weaponUI.SetInfo(this, weapon.id);

            // 무기가 장착되지 않은 상태이면 무기 리스트에 있는 첫번째 무기 선택되게끔
            if (status.Weapon.Equals("None") && first)
            {
                weaponUI.OnClick();
                first = false;
            }
            // 장착중인 무기에 focus가 가도록
            else if (weapon.id.Equals(status.Weapon))
                weaponUI.OnClick();
        }
    }

    public void SetInfo(string weaponId)
    {
        _id = weaponId;
        Data.WeaponData weaponData = Managers.Data.WeaponDict[weaponId];
        GetText((int)Texts.WeaponNameText).text = weaponData.name;
        GetText((int)Texts.WeaponDescriptionText).text = weaponData.description;
        GetText((int)Texts.WeaponAttackText).text = $"Attack: {weaponData.attack}";
        GetImage((int)Images.WeaponLargeIcon).sprite =
            Managers.Resource.Load<Sprite>(Managers.Data.IconDict[weaponId].path);

        Debug.Log($"{_player.GetComponent<PlayerStatus>().Weapon} : {weaponId}");

        // 현재 장착중인 무기라면
        if (_player.GetComponent<PlayerStatus>().Weapon.Equals(weaponId))
            GetText((int)Texts.ArmButtonText).text = "장착 해제";
        else
            GetText((int)Texts.ArmButtonText).text = "장착 하기";
    }

    public override void Clear()
    {
        base.Clear();
    }
}
