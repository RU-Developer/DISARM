using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerGun : Despawnable
{
    //팔 부분 오브젝트
    private GameObject leftArm, rightArm;
    //마우스 위치
    Vector2 mouse;
    //근접 공격이 가능한 시각(발동 시작시부터)
    private float attackTime = 0;
    //플레이어 방향
    private float dir = 0;
    //근접 공격시의 팔 각도
    private float meleeAngle = 0;
    //defend상태
    private bool isDefend = false;
    //팔의 방향(위=-1,아래=1)
    private float isAngleDown = 0;
    //근접 공격 컴포넌트
    private MeleeAttack melee;
    //팔 각도
    public float angle { private set; get; }
    public float gunAngle { private set; get; }
    //총이 사용 가능한지
    [HideInInspector] public bool canShoot;
    
    private string weapon;
    private PlayerStatus status;

    public void Init()
    {
        if (status != null)
            return;

        canShoot = true;
        Managers.Input.AddKeyAction(OnKeyBoard);
        Managers.Input.AddMouseAction(OnMouseClick);

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

    private void OnKeyBoard()
    {
        Fire();
    }

    private void OnMouseClick(Define.MouseEvent evt)
    {
        Fire();
    }

    void Update()
    {
        Melee();

        dir = (int)Managers.Input.CurrentMoveDir;

        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        angle = (90 - (float)Managers.Input.GunAngle) * (int)Managers.Input.CurrentMoveDir;
    }

    private void FixedUpdate()
    {
        leftArm.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward * Time.deltaTime);
        rightArm.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward * Time.deltaTime);
    }
    private void Melee()
    {
        if (!Managers.Data.MapItemDict["Parry"].consume)
            return;

        if (Input.GetMouseButton(0) && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        attackTime -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && Managers.Skill.CanUseSkill("parry") && attackTime < 0)
        {
            meleeAngle = angle;
            attackTime = 0.5f;
            leftArm.GetComponent<SpriteRenderer>().color = new Color(0, 150, 255);
            rightArm.GetComponent<SpriteRenderer>().color = new Color(0, 150, 255);
        }

        if (attackTime> 0 && !isDefend && Managers.Skill.CanUseSkill("parry"))
        {
            isDefend = true;
            Managers.Sound.Play("player_parry");
        }

        if (Managers.Skill.CanUseSkill("parry"))
        {
            canShoot = true;
        }

        if (attackTime < 0)
        {
            if(isDefend&& Managers.Skill.CanUseSkill("parry"))
            {
                isDefend = false;
                Managers.Skill.UseSkill("parry");
            }
            leftArm.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
            rightArm.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
        }

    }

    private void Fire()
    {
        if (Input.GetMouseButtonDown(0))
            return;

        if (Input.GetMouseButtonDown(1) && canShoot)
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

                    Managers.Sound.Play("dart_fire");
                    GameObject dart = Managers.Resource.Instantiate("dart");
                    dart.transform.position = new Vector2(transform.position.x, transform.position.y);

                    dart.transform.rotation = Quaternion.AngleAxis(-(float)Managers.Input.GunAngle*(int)Managers.Input.CurrentMoveDir, Vector3.forward);
                    dart.GetComponent<Dart>().status = status;
                    break;
            }
        }
    }

    public override void Despawn()
    {
        if (status != null)
            status.RemoveWeaponChangedAction(OnWeaponChanged);
        Managers.Input.RemoveKeyAction(OnKeyBoard);
        Managers.Input.RemoveMouseAction(OnMouseClick);
    }
}