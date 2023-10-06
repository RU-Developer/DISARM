using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MapItem : UI_Popup
{
    enum GameObjects
    {
        ButtonEventHandler
    }

    enum Texts
    {
        ItemName,
        ItemDescription
    }

    enum Images
    {
        ItemIcon
    }

    private string _name;
    private string _description;
    private string _icon;

    public override void Init()
    {
        base.Init();
        Managers.Input.AddUIKeyAction(OnKeyBoard);
        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
        BindEvent(GetObject((int)GameObjects.ButtonEventHandler), evt => Close());

        GetText((int)Texts.ItemName).text = _name;
        GetText((int)Texts.ItemDescription).text = _description;
        GetImage((int)Images.ItemIcon).sprite = Managers.Resource.Load<Sprite>(Managers.Data.IconDict[_icon].path);
    }

    private void OnKeyBoard()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            Close();
    }

    private void Close()
    {
        Managers.Pause.Play();
        Managers.Input.RemoveUIKeyAction(OnKeyBoard);
        ClosePopupUI();
    }

    public void SetInfo(string name, string description, string icon)
    {
        _name = name;
        _description = description;
        _icon = icon;
    }
}
