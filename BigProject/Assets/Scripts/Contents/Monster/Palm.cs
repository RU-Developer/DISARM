using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Palm : MonsterController
{
    // 이 상태에 따라 정해진 행동들 중 하나를 수행함
    enum PalmState 
    {
        Idle,
        Move,
        Attack,
    }

    private PalmState _state = PalmState.Idle;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override void Init()
    {
        base.Init();
        SetState(PalmState.Move);
        SetState(PalmState.Idle);
    }

    private void SetState(PalmState state)
    {
        if (_state == state)
            return;

        _state = state;
    }

    private void Update()
    {
        if (Managers.Pause.IsPause)
            return;

        switch (_state)
        {
            case PalmState.Idle:
                break;
            case PalmState.Move:
                break;
            case PalmState.Attack:
                break;
        }
    }

    public override void Die()
    {
        Debug.Log("dead");
        base.Die();
        this.gameObject.SetActive(false);
    }
}
