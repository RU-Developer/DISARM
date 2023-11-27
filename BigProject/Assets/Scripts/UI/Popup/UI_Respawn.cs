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
        Bind<GameObject>(typeof(GameObjects));
        BindEvent(GetObject((int)GameObjects.ButtonEventHandler), evt => Close());
    }

    private void Update()
    {
        if (Managers.Input.GetInputDown(Define.InputType.Ok))
            Close();
    }

    private void Close()
    {
        Managers.Pause.Play();
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
