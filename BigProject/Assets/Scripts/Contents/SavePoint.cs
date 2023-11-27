using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : Despawnable
{
    GameObject _player;

    private float _eventDistance;

    void Start()
    {
        _player = Managers.Game.GetPlayer();
        Managers.Game.AddPlayerChangedAction(ChangePlayer);
        _eventDistance = 5.0f;
    }

    private void Update()
    {
        if (Managers.Input.GetInputDown(Define.InputType.Ok) == false || _player == null)
            return;

        float distance = Vector2.Distance(new Vector2(_player.transform.position.x, _player.transform.position.y),
                new Vector2(transform.position.x, transform.position.y));

        if (distance > _eventDistance)
            return;

        UI_Dialog dialog = Managers.UI.ShowSceneUI<UI_Dialog>();

        Data.DialogScript script = new Data.DialogScript();
        script.script = new Data.Script();
        script.name = "세이브 포인트";
        script.script.content = "저장하시겠습니까?";
        script.selections = new List<Data.Script>();
        Data.Script save = new Data.Script();
        save.content = "저장하기";
        save.link = "//Save";

        Data.Script cancel = new Data.Script();
        cancel.content = "취소";

        script.selections.Add(save);
        script.selections.Add(cancel);

        dialog.InitComment(script);
        Managers.Pause.Pause();
    }

    private void ChangePlayer()
    {
        _player = Managers.Game.GetPlayer();
    }

    public override void Despawn()
    {
        Managers.Game.RemovePlayerChangedAction(ChangePlayer);
    }
}
