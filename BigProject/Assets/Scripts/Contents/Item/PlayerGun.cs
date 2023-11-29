using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerGun : Despawnable
{
    //�� �κ� ������Ʈ
    private GameObject leftArm, rightArm;
    //���� ������ ������ �ð�(�ߵ� ���۽ú���)
    private float attackTime = 0;
    //defend����
    private bool isDefend = false;
    //���� ����(��=-1,�Ʒ�=1)
    private float isAngleDown = 0;
    //�ֱ� ���� ����
    private float lastAngle = 0;
    //���� ��� ��������
    [HideInInspector] public bool canShoot;
    
    private string weapon;
    private PlayerStatus status;

    public void Init()
    {
        if (status != null)
            return;

        canShoot = true;

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

        if (Managers.Input.GetInputDown(Define.InputType.Skill2) && Managers.Skill.CanUseSkill("parry") && attackTime < 0)
        {
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
        // �и� ���� �߿��� ����
        if (Managers.Input.GetInputDown(Define.InputType.Skill2))
            return;

        // �߻� ��ư ����
        if (Managers.Input.GetInputDown(Define.InputType.Attack) && canShoot)
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