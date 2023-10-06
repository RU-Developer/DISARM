using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Respawn : UI_Popup
{
    enum GameObjects
    {
        ButtonEventHandler
    }

    public override void Init()
    {
        base.Init();
        Managers.Input.AddUIKeyAction(OnKeyBoard);
        Bind<GameObject>(typeof(GameObjects));
        BindEvent(GetObject((int)GameObjects.ButtonEventHandler), evt => Close());
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

        Managers.Game.Despawn(Managers.Game.GetPlayer());
        if (!Managers.Game.TEST)
            Managers.Game.Respawn();
        else
        {
            Managers.Game.SetPlayerSpawnPos(Managers.Game.DefaultPosition);
            Managers.Game.EnterSceneSpawn();
        }
    }
}
