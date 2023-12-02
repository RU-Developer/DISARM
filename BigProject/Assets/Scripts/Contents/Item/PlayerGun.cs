using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerGun : Despawnable
{
    //�� �κ� ������Ʈ
    private GameObject leftArm, rightArm;
    //���� ������ ������ �ð�(�ߵ� ���۽ú���)
    private float attackTime = 0;
    //�ֱ� ���� ����
    private float lastAngle = 0;
    //���� ��� ��������
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

        // UI�� ���콺 �����Ͱ� ��ġ�� ������ UI �̺�Ʈ �켱
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
        // �и� ���� �߿��� ����
        if (Managers.Input.GetInputDown(Define.InputType.Skill2) || !canShoot)
            return;

        // �߻� ��ư ����
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

                    //�Ʒ��� ���� & �������� ���� �� ��¦ �������� ȿ��
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