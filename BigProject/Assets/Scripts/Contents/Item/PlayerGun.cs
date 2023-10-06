using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerGun : Despawnable
{
    //�� �κ� ������Ʈ
    public GameObject leftArm, rightArm;
    //���콺 ��ġ
    Vector2 mouse;
    //���� ������ ������ �ð�(�ߵ� ���۽ú���)
    private float attackTime = 0;
    //�÷��̾� ����
    private float dir = 0;
    //���� ���ݽ��� �� ����
    private float meleeAngle = 0;
    //defend����
    private bool isDefend = false;
    //���� ����(��=-1,�Ʒ�=1)
    private float isAngleDown = 0;
    //���� ���� ������Ʈ
    private GameObject attack;
    //�� ����
    public float angle { private set; get; }
    //���� ��� ��������
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

        dir = GetComponent<PlayerController>().dir;

        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if(dir>0)
            angle = Mathf.Atan2(mouse.y - transform.position.y,
            Mathf.Abs(mouse.x - transform.position.x) * dir) * Mathf.Rad2Deg;
        else if (dir < 0)
            angle = 180-Mathf.Atan2(transform.position.y - mouse.y,
            Mathf.Abs(mouse.x - transform.position.x) * dir) * Mathf.Rad2Deg;




        if (angle<=0 && angle >= -180)
        {
            isAngleDown = 1;
        }
        else
        {
            isAngleDown = -1;
        }
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
            Attack(0.1f);
            Managers.Sound.Play("player_parry");
            if (attack != null)
            {
                attack.GetComponent<MeleeAttack>().dir = 0;
            }
        }

        if (attackTime > 0 && Managers.Skill.CanUseSkill("parry"))
        {
            
            if (dir > 0 || Mathf.Sign(angle) == Mathf.Sign(meleeAngle))
            {
                if ((angle - meleeAngle) * dir > 45)
                {
                    Attack(0.2f);
                    Managers.Sound.Play("player_parry2");
                    if (attack != null)
                    {
                        attack.GetComponent<Animator>().SetBool("isUpward", true);
                        attack.GetComponent<MeleeAttack>().dir = 1;
                        Managers.Skill.UseSkill("parry");
                    }
                    
                }
                else if ((angle - meleeAngle) * dir < -45)
                {
                    Managers.Sound.Play("player_parry2");
                    Attack(0.2f);
                    if(attack != null)
                    {
                        attack.GetComponent<Animator>().SetBool("isDownward", true);
                        attack.GetComponent<MeleeAttack>().dir = -1;
                        Managers.Skill.UseSkill("parry");
                    }
                }
            }
            else
            {
                if (angle > 0)
                {
                    Attack(0.2f);
                    if (attack != null)
                    {
                        Managers.Sound.Play("player_parry2");
                        attack.GetComponent<Animator>().SetBool("isUpward", true);
                        attack.GetComponent<MeleeAttack>().dir = 1;
                        Managers.Skill.UseSkill("parry");
                    }
                }
                else
                {
                    Attack(0.2f);
                    if (attack != null)
                    {
                        Managers.Sound.Play("player_parry2");
                        attack.GetComponent<Animator>().SetBool("isDownward", true);
                        attack.GetComponent<MeleeAttack>().dir = -1;
                        Managers.Skill.UseSkill("parry");
                    }
                }
            }
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

    private void Attack(float remain)
    {
        if (attack != null && Managers.Skill.CanUseSkill("parry"))
        {
            Managers.Resource.Destroy(attack);
        }
        if (canShoot) 
            attack = Managers.Resource.Instantiate("attack");
            canShoot = false;
        if(attack != null)
        {
            attack.GetComponent<MeleeAttack>().playerDir = dir;
            attack.transform.position = new Vector2(transform.position.x + 0.4f * dir, transform.position.y -0.1f - 0.4f*isAngleDown);
            attack.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            Managers.Resource.Destroy(attack, remain);
            
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
                    dart.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
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
