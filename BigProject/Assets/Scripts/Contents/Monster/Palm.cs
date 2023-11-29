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

    private void UpdateAnimation()
    {
        // 모든 애니메이션은 방향에 따라 해당 애니메이션을 뒤집어서 수행해야 할 것
        switch (_state)
        {
            case PalmState.Idle:
                // 목표가 범위에 들어왔으면 이동으로 범위 들어온 애니메이션
                // 아니면 뭐.. 해당 방에 들어왔을때 활성화 될테니.. 활성화 되기만 해도 바로 작동해도 좋고
                // 목표가 범위에 들어오지 않았다면 기본 Idle 애니메이션
                break;
            case PalmState.Move:
                // 방향에 따라 뒤집어서 이동 애니메이션
                break;
            case PalmState.Attack:
                // 일반 공격 애니메이션
                // Skill이 있다면 각각 다른 것들 추가해야 할 것
                break;
        }

        // 만약 페이즈가 갈리면서 완전히 다른 행동 양식이 보여진다면
        // 페이즈 마다의 필드를 따로 추가하여 다른 행동 양식으로 분기해도 좋을 듯
    }

    private void SetState(PalmState state)
    {
        if (_state == state)
            return;

        _state = state;
        UpdateAnimation();
    }

    private void Update()
    {
        if (Managers.Pause.IsPause)
            return;

        switch (_state)
        {
            case PalmState.Idle:
                Idle();
                break;
            case PalmState.Move:
                Move();
                break;
            case PalmState.Attack:
                Attack();
                break;
        }
    }

    private void Idle()
    {
        // Idle에서는 Move나 Attack으로 이동 가능
        // 범위 안에 Player가 들어왔다면 Attack
        // 없다면 다시 Move
    }

    private void Move()
    {
        // deltaTime 적용해서 움직이기
        // 플레이어 위치 가져와서 해당 위치 방향으로 스피드 만큼 움직이기
        // 움직임이 없는 보스라면 좀 다를 듯
        // 한번 움직이고 나면 Idle로 이동
    }

    private void Attack()
    {
        // 애니메이션 끝났으면 피격 판정 넣고 Idle State로 이동
    }

    public override void Die()
    {
        Debug.Log("dead");
        base.Die();
        this.gameObject.SetActive(false);
    }
}
