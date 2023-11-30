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
        MeleeAttack,
        RangedAttack,
    }

    enum Phase
    {
        None,
        Phase1,
        Phase2,
        Phase3,
    }

    private Phase _phase = Phase.None;
    private PalmState _state = PalmState.Idle;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override void Init()
    {
        status = gameObject.GetComponent<MonsterStatus>();
        WorldObjectType = Define.WorldObject.Monster;

        SetState(PalmState.Move);
        SetState(PalmState.Idle);
        SetPhase(Phase.Phase1);
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
            case PalmState.MeleeAttack:
                // 일반 공격 애니메이션
                // Skill이 있다면 각각 다른 것들 추가해야 할 것
                break;
            case PalmState.RangedAttack:

                break;
        }

        // 만약 페이즈가 갈리면서 완전히 다른 행동 양식이 보여진다면
        // 페이즈 마다의 필드를 따로 추가하여 다른 행동 양식으로 분기해도 좋을 듯
        // 1페이즈에서는 근접, 2페이즈에서는 원거리 추가, 3페이즈에서는 속도 증가 정도의 차이가 있음
        // 기능이 많지 않아 bool 필드로 관리해도 될 정도 또는 페이즈 변경시 마다 위에 페이즈 변화하게 해도 되고
    }

    private void SetPhase(Phase phase)
    {
        if (_phase == phase)
            return;

        _phase = phase;

        // phase 변화시마다 뭐 이벤트 호출이나 speed 변경 등 하고 싶다면 여기서 수정
        switch (_phase)
        {
            case Phase.Phase1:
                break;
            case Phase.Phase2:
                break;
            case Phase.Phase3:
                break;
        }
    }

    // 맞을 때마다 이거 호출 될거임
    public override void OnHit()
    {
        base.OnHit();
        int hp = status.Hp;
        int maxHp = status.MaxHp;
        int attack = status.Attack;
        // 이 정보를 통해서 phase 변경을 해도 될 거.
        switch (_phase)
        {
            case Phase.Phase1:
                if ((float)hp / maxHp <= 0.8f)
                    SetPhase(Phase.Phase2);
                break;
            case Phase.Phase2:
                if ((float)hp / maxHp <= 0.5f)
                    SetPhase(Phase.Phase3);
                break;
            case Phase.Phase3:
                break;
        }


        // 이렇게 설정한 phase 정보를 바탕으로 아래의 함수들에서 _phase의 정보를 참고해서 작업하면 될 것
        // phase 변경시마다 뭔가 이벤트를 주고 싶다면 SetPhase 함수 수정
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
            case PalmState.MeleeAttack:
                MeleeAttack();
                break;
            case PalmState.RangedAttack:
                RangedAttack();
                break;
        }
    }

    private void Idle()
    {
        // deltatime하나 체크 해서, 일정 주기가 되지 않았으면 Idle상태 유지도 괜찮겠지?
        // Idle에서는 Move나 Attack 가능
        // 사정거리 범위 안에 Player가 들어왔다면 Attack
        // 스킬 들이 여럿 있겠지?
        // 그 스킬이 사용 가능하면 범위를 체크 하고, 그 중에서 사정거리 맞는거 바로 사용?
        // 쿨타임 없는 게 있으면 ... Attack... 일반공격? 사거리 안에 들면 그것도 가능
        // 없다면 다시 Move
    }

    private void Move()
    {
        // deltaTime 적용해서 움직이기
        // 플레이어 위치 가져와서 해당 위치 방향으로 스피드 만큼 움직이기
        // 움직임이 없는 보스라면 좀 다를 듯
        // 한번 움직이고 나면 Idle로 이동
    }

    private void MeleeAttack()
    {
        // 근접공격 애니메이션 끝났으면 피격 판정 넣고(범위에 아직 있다면) Idle State로 이동
        // 범위 판정은 삼각함수 외적을 이용하거나 충돌 되는 범위를 생성해서 아래의 원거리 공격과 동일하게 데미지 주는 방식
        // 삼각함수 외적인 경우 여기서 데미지 처리 로직을 작성할 수 있음
    }

    private void RangedAttack()
    {
        // 투사체 생성 애니메이션 실행. 끝났으면 뒤에꺼 실행
        // 투사체 생성(Managers.Resource.Instantiate)
        // 투사체의 타겟을 Managers.Game.GetPlayer().position으로 Init()을 통해 지정해주기
        // Palm 보스 몬스터의 MonsterStatus도 넘겨주면 좋을 것
        // 투사체는 Init이 호출되고 나면 해당 포인트로 이동하며 충돌시 플레이어에게 데미지를 줌.
        // 충돌시 MonsterStatus를 공격자로 잡고, PlayerStatus 맞는 사람으로 해서 OnDamaged 호출.
    }

    public override void Die()
    {
        Debug.Log("dead");
        base.Die();
        this.gameObject.SetActive(false);
    }
}
