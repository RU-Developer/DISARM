using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * DialogEvent를 할 수 있게 만들고 싶은 게임 오브젝트에 붙이는 컴포넌트입니다.
 * 반드시 gameObject를 없애기 전에 Despawn을 호출해야 합니다.
 * 해결책으로는 BaseController를 컴포넌트를 추가하고, 
 * gameObject 삭제시에는 GameManager를 통해서 Despawn시키는 방법이 있습니다.
 */
public class DialogEvent : Despawnable
{
    GameObject _player;

    private float _eventDistance;
    private Data.DialogScript _script;

    void Start()
    {
        _player = Managers.Game.GetPlayer();
        Managers.Game.AddPlayerChangedAction(ChangePlayer);
        Managers.Input.AddKeyAction(Event);
        _eventDistance = 5.0f;
    }

    /**
     * SetScript를 통해 설정된 스크립트가 UI_Dialog를 통해 출력됩니다.
     */ 
    public void SetScript(string id)
    {
        _script = Managers.Data.DialogScriptDict[id];
    }

    private void Event()
    {
        if (Input.GetKeyDown(KeyCode.Return) == false || _script == null || _player == null)
            return;

        float distance = Vector2.Distance(new Vector2(_player.transform.position.x, _player.transform.position.y),
                new Vector2(transform.position.x, transform.position.y));

        if (distance > _eventDistance)
            return;
        
        UI_Dialog dialog = Managers.UI.ShowSceneUI<UI_Dialog>();
        dialog.InitComment(_script);
        Managers.Pause.Pause();
    }

    private void ChangePlayer()
    {
        _player = Managers.Game.GetPlayer();
    }

    public override void Despawn()
    {
        Managers.Game.RemovePlayerChangedAction(ChangePlayer);
        Managers.Input.RemoveKeyAction(Event);
    }
}
