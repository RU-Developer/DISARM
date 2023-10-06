using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Game_Menu : UI_Scene
{
    enum GameObjects
    {
        WeaponMenuHandler,
        MapMenuHandler,
        GameMenuResumeHandler,
        GameMenuOptionsHandler,
        GameMenuMainHandler
    }

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        BindEvent(GetObject((int)GameObjects.WeaponMenuHandler),
            evt =>
            {
                Managers.UI.CloseSceneUI<UI_Game_Menu>();
                Managers.UI.ShowSceneUI<UI_Game_Menu_Weapon>();
            });

        BindEvent(GetObject((int)GameObjects.MapMenuHandler),
            evt =>
            {
                Managers.UI.CloseSceneUI<UI_Game_Menu>();
                Managers.UI.ShowSceneUI<UI_Game_Menu_Map>();
            });

        BindEvent(GetObject((int)GameObjects.GameMenuResumeHandler),
            evt =>
            {
                ResumeGame();
            });

        BindEvent(GetObject((int)GameObjects.GameMenuOptionsHandler),
            evt => Debug.Log("Options menu clicked"));

        BindEvent(GetObject((int)GameObjects.GameMenuMainHandler),
            evt => Debug.Log("Go to Main Lobby menu clicked"));

        Managers.Input.AddUIKeyAction(OnKeyBoard);
    }

    private void OnKeyBoard()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        ResumeGame();
    }

    private void ResumeGame()
    {
        Managers.UI.CloseSceneUI<UI_Game_Menu>();
        Managers.UI.ShowSceneUI<UI_Game>();
        Managers.Pause.Play();
    }

    public override void Clear()
    {
        base.Clear();
        Managers.Input.RemoveUIKeyAction(OnKeyBoard);
    }
}
