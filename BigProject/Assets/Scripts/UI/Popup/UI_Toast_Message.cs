using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Toast_Message : UI_Popup
{
    enum Texts
    {
        Message
    }

    private GameObject _player;
    private float _eventDistance;
    private ToastMessageEvent _go;

    private string _text;
    private Vector3 _position;

    private void ChangePlayer()
    {
        _player = Managers.Game.GetPlayer();
    }

    public override void Init()
    {
        if (_player != null)
            return;

        base.Init();
        Bind<Text>(typeof(Texts));
        GetText((int)Texts.Message).text = _text;

        _eventDistance = 2.0f;
        _player = Managers.Game.GetPlayer();
        Managers.Game.AddPlayerChangedAction(ChangePlayer);
    }

    private void Update()
    {
        Init();
        // 플레이어가 일정 거리 이상 떨어지면 메시지 삭제
        float distance = Vector2.Distance(new Vector2(_player.transform.position.x, _player.transform.position.y),
                new Vector2(_position.x, _position.y));

        if (distance > _eventDistance)
        {
            ClosePopupUI();
            _go.isOn = false;
        }
    }

    public void SetInfo(string text, Vector3 position, ToastMessageEvent go)
    {
        _text = text;
        _position = position;
        _go = go;
    }

    public override void Clear()
    {
        base.Clear();
        Managers.Game.RemovePlayerChangedAction(ChangePlayer);
    }
}
