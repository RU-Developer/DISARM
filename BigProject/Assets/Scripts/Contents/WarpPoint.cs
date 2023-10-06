using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpPoint : Despawnable
{
    GameObject _player;

    [SerializeField]
    public string _warpId;
    [SerializeField]
    public string _targetPointId;

    private float _eventDistance;
    private Data.WarpPoint _warpPoint;
    public Data.WarpPoint Point { get { return _warpPoint; } private set { _warpPoint = value; } }

    private bool _updated;

    void Start()
    {
        _player = Managers.Game.GetPlayer();
        Managers.Game.AddPlayerChangedAction(ChangePlayer);
        Managers.Input.AddKeyAction(Event);
        _eventDistance = 5.0f;
        _updated = false;

        SetWarpPoint();
    }

    /**
     * 워프 포인트 변경 가능.
     */
    public void SetWarpPoint()
    {
        if (string.IsNullOrEmpty(_warpId) || string.IsNullOrEmpty(_targetPointId))
            return;

        Dictionary<string, Data.WarpPoint> warpDict = Managers.Data.WarpPointDict;
        Data.WarpPoint saved = new Data.WarpPoint();
        saved.id = _warpId;
        saved.scene = Managers.Scene.GetCurrentSceneName();
        Managers.Data.WarpPointDict[_warpId] = saved;

        saved.x = transform.position.x;
        saved.y = transform.position.y;
        saved.link = _targetPointId;

        _warpPoint = saved;

        Debug.Log($"{warpDict[_warpId].scene}");

        _updated = true;
    }

    private void Update()
    {
        if (!_updated)
            return;

        Managers.Data.Init();
        Managers.Data.SaveJson<Data.WarpPoint>(false);
        _updated = false;
    }

    private void Event()
    {
        if (Input.GetKeyDown(KeyCode.Return) == false || _warpPoint == null || _player == null)
            return;

        float distance = Vector2.Distance(new Vector2(_player.transform.position.x, _player.transform.position.y),
                new Vector2(transform.position.x, transform.position.y));

        if (distance > _eventDistance)
            return;

        Managers.Warp.SetWarpPoint(this);

        UI_Dialog dialog = Managers.UI.ShowSceneUI<UI_Dialog>();

        Data.DialogScript script = new Data.DialogScript();
        script.script = new Data.Script();
        script.name = "워프 시스템";
        script.script.content = "워프하시겠습니까?";
        script.selections = new List<Data.Script>();
        Data.Script warp = new Data.Script();
        warp.content = "워프하기";
        warp.link = "//Warp";
        
        Data.Script cancel = new Data.Script();
        cancel.content = "워프취소";
        cancel.link = "//WarpCancel";

        script.selections.Add(warp);
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
        Managers.Input.RemoveKeyAction(Event);
    }
}
