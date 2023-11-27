using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Game_Menu_Status : UI_Scene
{
    enum GameObjects
    {
        CollectionMenuHandler,
        MenuHandler,
    }

    private void Update()
    {
        if (Managers.Input.GetInputDown(Define.InputType.Menu))
        {
            Managers.UI.CloseSceneUI<UI_Game_Menu_Status>();
            Managers.UI.ShowSceneUI<UI_Game>();
            Managers.Pause.Play();
        }
    }

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        BindEvent(GetObject((int)GameObjects.CollectionMenuHandler),
            evt =>
            {
                Managers.UI.CloseSceneUI<UI_Game_Menu_Status>();
                Managers.UI.ShowSceneUI<UI_Game_Menu_Collection>();
            });
        BindEvent(GetObject((int)GameObjects.MenuHandler),
            evt =>
            {
                Managers.UI.CloseSceneUI<UI_Game_Menu_Status>();
                Managers.UI.ShowSceneUI<UI_Game_Menu>();
            });
    }

    public override void Clear()
    {
        base.Clear();
    }
}
