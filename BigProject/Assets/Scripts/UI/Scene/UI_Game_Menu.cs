using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private GameObjects _state = GameObjects.GameMenuResumeHandler;

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
            evt => Quit());

        SetState(GameObjects.GameMenuResumeHandler);
    }

    private void SetState(GameObjects state)
    {
        GetObject((int)_state).GetComponent<Image>().color = new Color32(255, 255, 255, 0);
        _state = state;
        GetObject((int)_state).GetComponent<Image>().color = new Color32(255, 255, 255, 20);
    }

    private void Update()
    {
        if (Managers.Pause.IsPause == false)
            return;

        if (Managers.Input.GetInputDown(Define.InputType.Menu))
            ResumeGame();

        switch (_state)
        {
            case GameObjects.GameMenuResumeHandler:
                if (Managers.Input.GetInputDown(Define.InputType.Ok))
                    ResumeGame();
                else if (Managers.Input.GetInputDown(Define.InputType.Up))
                    SetState(GameObjects.GameMenuQuitHandler);
                else if (Managers.Input.GetInputDown(Define.InputType.Down))
                    SetState(GameObjects.GameMenuOptionsHandler);
                else if (Managers.Input.GetInputDown(Define.InputType.Left))
                    WeaponMenu();
                else if (Managers.Input.GetInputDown(Define.InputType.Right))
                    MapMenu();
                break;
            case GameObjects.GameMenuOptionsHandler:
                if (Managers.Input.GetInputDown(Define.InputType.Ok))
                    Options();
                else if (Managers.Input.GetInputDown(Define.InputType.Up))
                    SetState(GameObjects.GameMenuResumeHandler);
                else if (Managers.Input.GetInputDown(Define.InputType.Down))
                    SetState(GameObjects.GameMenuMainHandler);
                else if (Managers.Input.GetInputDown(Define.InputType.Left))
                    WeaponMenu();
                else if (Managers.Input.GetInputDown(Define.InputType.Right))
                    MapMenu();
                break;
            case GameObjects.GameMenuMainHandler:
                if (Managers.Input.GetInputDown(Define.InputType.Ok))
                    GoToMainLobby();
                else if (Managers.Input.GetInputDown(Define.InputType.Up))
                    SetState(GameObjects.GameMenuOptionsHandler);
                else if (Managers.Input.GetInputDown(Define.InputType.Down))
                    SetState(GameObjects.GameMenuQuitHandler);
                else if (Managers.Input.GetInputDown(Define.InputType.Left))
                    WeaponMenu();
                else if (Managers.Input.GetInputDown(Define.InputType.Right))
                    MapMenu();
                break;
            case GameObjects.GameMenuQuitHandler:
                if (Managers.Input.GetInputDown(Define.InputType.Ok))
                    Quit();
                else if (Managers.Input.GetInputDown(Define.InputType.Up))
                    SetState(GameObjects.GameMenuMainHandler);
                else if (Managers.Input.GetInputDown(Define.InputType.Down))
                    SetState(GameObjects.GameMenuResumeHandler);
                else if (Managers.Input.GetInputDown(Define.InputType.Left))
                    WeaponMenu();
                else if (Managers.Input.GetInputDown(Define.InputType.Right))
                    MapMenu();
                break;
        }
    }

    private void Quit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                 Application.Quit();
        #endif
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
