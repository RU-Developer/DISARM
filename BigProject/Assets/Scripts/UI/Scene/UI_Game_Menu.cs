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
        GameMenuMainHandler,
        GameMenuQuitHandler,
    }

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        BindEvent(GetObject((int)GameObjects.WeaponMenuHandler),
            evt => WeaponMenu());

        BindEvent(GetObject((int)GameObjects.MapMenuHandler),
            evt => MapMenu());

        BindEvent(GetObject((int)GameObjects.GameMenuResumeHandler),
            evt => ResumeGame());

        BindEvent(GetObject((int)GameObjects.GameMenuOptionsHandler),
            evt => Options());

        BindEvent(GetObject((int)GameObjects.GameMenuMainHandler),
            evt => GoToMainLobby());

        BindEvent(GetObject((int)GameObjects.GameMenuQuitHandler),
            evt =>
            {
            #if UNITY_EDITOR
                // Application.Quit() does not work in the editor so
                // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
            });
    }

    private void Update()
    {
        if (Managers.Input.GetInputDown(Define.InputType.Menu))
            ResumeGame();
    }

    private void ResumeGame()
    {
        Managers.UI.CloseSceneUI<UI_Game_Menu>();
        Managers.UI.ShowSceneUI<UI_Game>();
        Managers.Pause.Play();
    }

    private void WeaponMenu()
    {
        Managers.UI.CloseSceneUI<UI_Game_Menu>();
        Managers.UI.ShowSceneUI<UI_Game_Menu_Weapon>();
    }

    private void MapMenu()
    {
        Managers.UI.CloseSceneUI<UI_Game_Menu>();
        Managers.UI.ShowSceneUI<UI_Game_Menu_Map>();
    }

    private void Options()
    {
        Debug.Log("Options menu clicked");
    }

    private void GoToMainLobby()
    {
        Debug.Log("Go to Main Lobby menu clicked");
    }

    public override void Clear()
    {
        base.Clear();
    }
}
