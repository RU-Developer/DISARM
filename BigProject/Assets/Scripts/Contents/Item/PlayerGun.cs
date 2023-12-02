using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerGun : Despawnable
{
    //팔 부분 오브젝트
    private GameObject leftArm, rightArm;
    //근접 공격이 가능한 시각(발동 시작시부터)
    private float attackTime = 0;
    //최근 팔의 각도
    private float lastAngle = 0;
    //총이 사용 가능한지
    [HideInInspector] public bool canShoot;
    private bool canParry;

    private string weapon;
    private PlayerStatus status;

    public void Init()
    {
        if (status != null)
            return;

        canShoot = true;
        canParry = true;

        leftArm = gameObject.FindChild("leftArm");
        rightArm = gameObject.FindChild("rightArm");

        status = GetComponent<PlayerStatus>();
        weapon = status.Weapon;
        status.AddWeaponChangedAction(OnWeaponChanged);
        Debug.Log($"PlayerGun Init {weapon}");
    }

    private void OnWeaponChanged()
    {
        weapon = status.Weapon;
    }

    void Update()
    {
        Melee();
        Fire();
    }

    private void FixedUpdate()
    {
        leftArm.transform.rotation = 
            Quaternion.AngleAxis((90 - (float)Managers.Input.GunAngle) * Mathf.Sign((int)Managers.Input.CurrentMoveDir), 
            Vector3.forward * Time.deltaTime);
        rightArm.transform.rotation = 
            Quaternion.AngleAxis((90 - (float)Managers.Input.GunAngle) * Mathf.Sign((int)Managers.Input.CurrentMoveDir), 
            Vector3.forward * Time.deltaTime);
    }
    private void Melee()
    {
        if (Managers.Data.MapItemDict["Parry"].consume == false)
            return;

        // UI에 마우스 포인터가 위치해 있으면 UI 이벤트 우선
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        attackTime -= Time.deltaTime;

        if (Managers.Input.GetInputDown(Define.InputType.Skill2) && Managers.Skill.CanUseSkill("parry") && attackTime < 0 && canParry)
        {
            canParry = false;
            attackTime = 0.5f;
            lastAngle = (90 - (float)Managers.Input.GunAngle);
            leftArm.GetComponent<MeleeAttack>().canParry = true;
            leftArm.FindChild("Trail").GetComponent<TrailRenderer>().time = 1;
            leftArm.GetComponent<SpriteRenderer>().color = new Color(0, 150, 255);
            rightArm.GetComponent<SpriteRenderer>().color = new Color(0, 150, 255);
        }
        if (attackTime > 0)
        {
            if(lastAngle+10 < 90 - (float)Managers.Input.GunAngle)
            {
                leftArm.GetComponent<MeleeAttack>().dir = 1;
            }
            else if(lastAngle-10 > 90 - (float)Managers.Input.GunAngle)
            {
                leftArm.GetComponent<MeleeAttack>().dir = -1;
            }
            else
            {
                leftArm.GetComponent<MeleeAttack>().dir = 0;
            }
        }

        if (attackTime < 0 && !canParry)
        {
            canParry = true;
            Managers.Skill.UseSkill("parry");
            leftArm.GetComponent<MeleeAttack>().dir = 0;
            leftArm.GetComponent<MeleeAttack>().canParry = false;
            leftArm.FindChild("Trail").GetComponent<TrailRenderer>().time = 0;
            leftArm.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            rightArm.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
        }

    }

    private void Fire()
    {
        // 패링 시전 중에는 금지
        if (Managers.Input.GetInputDown(Define.InputType.Skill2) || !canShoot)
            return;

        // 발사 버튼 눌림
        if (Managers.Input.GetInputDown(Define.InputType.Attack))
        {
            switch (weapon)
            {
                case "None":
                    break;

                case "DartGun":
                    Debug.Log($"dartNum:{Dart.dartNum}, dartLimit:{Dart.dartLimit}");
                    if (Dart.dartNum >= Dart.dartLimit)
                        break;

                    Dart.dartNum++;
                    Debug.Log($"dartNum = {Dart.dartNum}");

                    //아래로 조준 & 떨어지고 있을 때 살짝 떠오르는 효과
                    if ((float)Managers.Input.GunAngle >135 && !GetComponent<PlayerController>().isGrounded)
                    {
                        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0);
                        GetComponent<Rigidbody2D>().AddForce(Vector2.up * 4f, ForceMode2D.Impulse);
                    }

                    Managers.Sound.Play("dart_fire");
                    GameObject dart = Managers.Resource.Instantiate("dart");
                    dart.transform.position = new Vector2(leftArm.transform.position.x, leftArm.transform.position.y);

                    dart.transform.rotation = Quaternion.AngleAxis(-(float)Managers.Input.GunAngle*Mathf.Sign((int)Managers.Input.CurrentMoveDir), Vector3.forward);
                    dart.GetComponent<Dart>().status = status;
                    break;
            }
        }
    }

    public override void Despawn()
    {
        if (status != null)
            status.RemoveWeaponChangedAction(OnWeaponChanged);
    }
}