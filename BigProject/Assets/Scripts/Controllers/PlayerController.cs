using UnityEngine;
using System.Collections;

public class PlayerController : BaseController
{
    //점프,벽점프,구르기 힘
    private float jumpForce;
    //좌-우 키 누름 정도 / 이동 속도
    private float horizontal;
    private float xspeed;
    //초기 gravityScale과 collider size
    private Vector3 initSize;
    public float initGravity { private set; get; }
    //점프 한 횟수
    private int jumpCount = 0;
    //머리 오브젝트 각도
    private float headAngle = 0f;

    //레이어마스크
    public LayerMask groundMask;
    private LayerMask slopeMask, platformMask;
    //파티클
    private ParticleSystem groundParticle;

    //컴포넌트
    private Rigidbody2D rigid;
    private GameObject head;
    private Animator animator;
    private Collider2D coll;
    private PlayerGun playerGun;
    private GameObject leftArm, rightArm;

    //절벽 위치 확인
    private Transform ledgeCheck;

    //플레이어 고정 변수
    [HideInInspector] public bool isFix;


    //여러 기능과 관련된 조건 : 외부에서 현재 dive중인지 파악을 위해 사용함
    [HideInInspector]public bool isLedge, isWallFront, isWallRight, isWallLeft, isWallUp, isSlope, canGrabLedge = true, canClimbLedge,isHang=false,
        isWallJump = false, isRoll = false, fixFlip = false;
    public bool isGrounded{ private set; get; }

//절벽 잡기 시 위치 정보를 저장
[Header("Ledge info")]
    [SerializeField] Vector2 offset1;
    [SerializeField] Vector2 offset2;
    Vector2 climbBegunPosition;
    Vector2 climbOverPosition;

    public override void Init()
    {
        base.Init();
        isFix = false;

        //LayerMask설정
        platformMask = LayerMask.GetMask("Platform");
        slopeMask = LayerMask.GetMask("Slope");

        //점프,벽 점프,구르기 크기
        jumpForce = 14;
        isGrounded = false;
        isWallJump = false;

        //절벽 확인하는 gameObject저장
        ledgeCheck = gameObject.FindChild<Transform>("ledgeCheck");

        //변수에 컴포넌트 할당
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        playerGun = GetComponent<PlayerGun>();
        groundParticle = this.gameObject.FindChild("run_particle").GetComponent<ParticleSystem>();

        head = this.gameObject.FindChild("head");
        leftArm = this.gameObject.FindChild("leftArm");
        rightArm = this.gameObject.FindChild("rightArm");

        //원래 gravityScale과 collider size를 저장
        initGravity = rigid.gravityScale;
        initSize = ((BoxCollider2D)coll).size;

        WorldObjectType = Define.WorldObject.Player;
    }
    void Update()
    {
        if (Managers.Pause.IsPause)
            return;

        //조건 달성시 발동하는 이벤트
        CheckForLedge();
        Particle();
        Slope();

        //키보드 이벤트
        Move();
        Jump();
        WallJump();
        Hold();

        //마우스 이벤트 + 키보드 이벤트
        Roll();
        Flip();
    }

    void Hold()
    {
        if (canClimbLedge || isRoll || isSlope || isFix) return;

        if (animator.GetBool("isHang") && animator.GetBool("isFall"))
        {
            playerGun.canShoot = false;
            leftArm.SetActive(false);
            rightArm.SetActive(false);
            head.SetActive(false);
        }
        if (isGrounded || !animator.GetBool("isHang") || animator.GetBool("isJump"))
        {
            playerGun.canShoot = true;
            leftArm.SetActive(true);
            rightArm.SetActive(true);
            head.SetActive(true);
        }
    }
    void Look()
    {
        //플레이어의 위,아래를 봤을 때 특정 animation을 동작시키는 코드
        headAngle = (90 - (float)Managers.Input.GunAngle)*Mathf.Sign((int)Managers.Input.CurrentMoveDir);

        if (headAngle < -20)
            headAngle = -20;
        if (headAngle>20)
            headAngle = 20;
        
        head.GetComponent<Transform>().rotation = Quaternion.AngleAxis(headAngle, Vector3.forward * Time.deltaTime);
    }

    void Particle()
    {
        //GroundParticle 메소드가 활성화 됐는지에 따라서
        //파티클을 lifetime으로 활성화, 비활성화 시킨다.
        if (IsInvoking("GroundParticle"))
        {
            groundParticle.startLifetime = 0.5f;
        }
        else
        {
            groundParticle.startLifetime = 0;
        }
    }

    void GroundParticle()
    {

    }
    void CheckForLedge()
    {
        if (!Managers.Data.MapItemDict["GrabLedge"].consume)
            return;

        if (isWallJump || isFix) return;

        //절벽을 잡을 수 있고, 마우스 각도가 캐릭터 위를 향할 때 작동
        //처음 위치와 끝 위치를 설정한다
        if (isLedge && canGrabLedge
            &&(((int)Managers.Input.CurrentMoveDir>0&&headAngle==20)
            || ((int)Managers.Input.CurrentMoveDir<0 && headAngle==-20)))
        {
            canGrabLedge = false;
            Vector2 ledgePosition = new Vector2(ledgeCheck.transform.position.x + (0.5f * Mathf.Sign((int)Managers.Input.CurrentMoveDir)), ledgeCheck.transform.position.y);
            fixFlip = true;
            climbBegunPosition = ledgePosition + new Vector2(offset1.x * Mathf.Sign((int)Managers.Input.CurrentMoveDir), offset1.y);
            climbOverPosition = ledgePosition + new Vector2(offset2.x * Mathf.Sign((int)Managers.Input.CurrentMoveDir), offset2.y);
            canClimbLedge = true;
        }
        //animation이 하나로 만들어져 있기 때문에 팔 부분은 비활성화 시켜 주고
        //기능들은 사용 불가능하게 만들어주고
        //올라가기 전 플레이어 위치를 고정시킨다.
        if (canClimbLedge)
        {
            rigid.velocity = Vector2.zero;
            fixFlip = true;
            transform.position = climbBegunPosition;

            leftArm.SetActive(false);
            rightArm.SetActive(false);
            head.SetActive(false);

            playerGun.canShoot = false;
            animator.speed = 1;
            animator.SetBool("isClimb", true);
        }
    }

    public void LedgeClimbOver()
    {
        //절벽에 올라가야 하는 상황이므로, 기본 설정들을 다시 원래대로 돌리고 끝 위치로 이동시킨다.
        canClimbLedge = false;
        fixFlip = false;
        animator.SetBool("isClimb", false);
        playerGun.canShoot = true;
        leftArm.SetActive(true);
        rightArm.SetActive(true);
        head.SetActive(true);

        Invoke("AllowLedgeGrab", 0.1f);
        transform.position = climbOverPosition;
    }

    //animation에서 직접 호출하는 부분, 절벽 잡기 모션이 끝났음을 알려 준다.
    public void AllowLedgeGrab() => canGrabLedge = true;
    void Move()
    {
        //move

        if (isWallJump || canClimbLedge || isRoll || isSlope || isFix ||  fixFlip)
            return;

        //horizontal값을 이동 관련 변수로 사용
        //땅에 있을 때와 공중에 있을 때 이동속도가 다름.
        /*
        horizontal = Input.GetAxisRaw("Horizontal");
        */
        horizontal = (Managers.Input.GetInput(Define.InputType.Right)||
            Managers.Input.GetInput(Define.InputType.Left)) ?1:0;

        if (Mathf.Abs(horizontal) > 0)
            xspeed = Mathf.Sign((int)Managers.Input.CurrentMoveDir) * 0.08f;
        else
            xspeed = 0;
            
        animator.SetFloat("isRun", Mathf.Abs(horizontal));

        //땅에서 이동할 때 GroundParticle 작동
        if (isGrounded && Input.GetAxis("Horizontal") != 0)
        {
            Invoke("GroundParticle", 0.1f);
        }

    }
    void Slope()
    {
        if (canClimbLedge || isFix || isWallJump || !isGrounded) return;
        //경사로를 밟았을 때 그 방향으로 미끄러지고 점프만 할 수 있음.
        //gravityScale을 올리면 방향을 따로 지정할 필요 없으므로 그렇게 쓴다.
        if (isSlope && Managers.Input.GetInputDown(Define.InputType.Jump))
        {
            fixFlip = true;
            rigid.gravityScale = 10f;
            if (Mathf.Abs(rigid.velocity.x) < 0.3f)
            {
                rigid.AddForce(new Vector2(0.2f * Mathf.Sign((int)Managers.Input.CurrentMoveDir), 0), ForceMode2D.Impulse);
            }
        }
        else
        {
            //경사로 벗어났을 때 코드, gravityScale을 원래대로 돌림
            fixFlip = false;
            rigid.gravityScale = initGravity;
        }
    }

    void Flip()
    {
        if (fixFlip) return;

        //플레이어 스프라이트 좌우 반전
        //Mathf.Sign((int)Managers.Input.CurrentMoveDir)변수는 보고 있는 방향. 1이면 오른쪽, -1이면 왼쪽
        if (Mathf.Sign((int)Managers.Input.CurrentMoveDir) > 0 || (isSlope && rigid.velocity.x > 0))
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
        else if (Mathf.Sign((int)Managers.Input.CurrentMoveDir) < 0 || (isSlope && rigid.velocity.x < 0))
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }

    }

    void Jump()
    {
        if (isRoll || isFix || canClimbLedge || isWallJump) return;
        //점프 키를 누르고, jumpCount가 0이면 점프
        if (Managers.Input.GetInputDown(Define.InputType.Jump) && jumpCount == 0)
        {
            Debug.Log("jump");
            Managers.Sound.Play("player_jump");
            jumpCount += 1;
            rigid.velocity = new Vector2(rigid.velocity.x, 0);
            rigid.AddForce(Vector2.up * jumpForce * 0.9f, ForceMode2D.Impulse);
        }
        //점프 높이조절
        if (Managers.Input.GetInputUp(Define.InputType.Jump) && rigid.velocity.y > 0)
        {
            Debug.Log("jump velocity / 2");
            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * 0.5f);
        }


        if (isGrounded && rigid.velocity.y < -1f)
            Invoke("GroundParticle", 0.5f);

        //jumpCount를 초기화
        if (isGrounded)
            jumpCount = 0;
        else if (!isGrounded && rigid.velocity.y >= 1f)
            jumpCount = 1;
        else if (rigid.velocity.y <= -2f && jumpCount == 0)
            jumpCount = 1;

        
    }

    void WallJump()
    {
        if (!Managers.Data.MapItemDict["WallJump"].consume)
            return;

        if (isRoll) return;
        //플레이어 옆에 벽이 있고, 땅 위에 있으며 플레이어가 하강 중일 때 벽 점프
        //벽 점프시 어느 정도의 관성이 주어져서 한 벽만 탈 수는 없다.
        float scale = 0f;

        if (isWallRight)
            scale = -1f;
        if (isWallLeft)
            scale = 1f;


        if ((isWallRight || isWallLeft))
        {
            if (Managers.Input.GetInputDown(Define.InputType.Jump))
            {
                if (!isWallJump && !isGrounded && horizontal != 0)
                {
                    isWallJump = true;
                    rigid.velocity = Vector2.zero;
                    StartCoroutine(wallJumpX(scale* 0.1f, 0.35f));
                    rigid.velocity = new Vector2(rigid.velocity.x, jumpForce * 0.5f);

                    Managers.Sound.Play("player_jump");
                    Debug.Log("wall jump");
                }
            }

        }
    }


    public void Roll()
    {
        if (!Managers.Data.MapItemDict["Dive"].consume)
            return;

        if (canClimbLedge || isFix) return;

        //쿨다운이 없고, 왼쪽 shift를 누르면 구르기
        //플레이어의 크기가 작아지고, 앞으로 빠르게 이동한다.
        //animation이 하나이므로, 팔 부분은 비활성화한다
        //animation은 땅에 닿을 때 까지 진행되지 않는다.
        //그리고 땅에 닿을 때 까지 아무런 행동도 할 수 없다.
        if (Managers.Input.GetInputDown(Define.InputType.Skill1) && !isRoll && Managers.Skill.UseSkill("dive") && !isWallFront)
        {
            isRoll = true;
            fixFlip = true;
            canGrabLedge = false;
            playerGun.canShoot = false;

            Managers.Sound.Play("player_jump");
            ((BoxCollider2D)coll).size = new Vector3(0.5f, 0.5f, 1f);

            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * jumpForce * 0.4f, ForceMode2D.Impulse);
            StartCoroutine(rollX(Mathf.Sign((int)Managers.Input.CurrentMoveDir) * 0.15f, 0.6f));

            leftArm.SetActive(false);
            rightArm.SetActive(false);
            head.SetActive(false);
            
            animator.SetBool("isRoll", true);
            animator.speed = 0;
        }
        if (isWallFront && isRoll)
        {
            xspeed = 0;
            rigid.velocity = Vector2.zero;
            StartCoroutine(rollX(Mathf.Sign((int)Managers.Input.CurrentMoveDir) * -0.04f, 0.6f));
        }
        if (isRoll && isGrounded) animator.speed = 1;
    }
    public void RollUp()
    {
        //animation 끝부분에서 호출되는 메소드로 구르기가 끝난 것으로 간주
        //모든 상황을 원래대로 돌린다.
        //크기가 원래대로 돌아오는 부분이 있음
        animator.SetBool("isRoll", false);
        playerGun.canShoot = true;
        leftArm.SetActive(true);
        rightArm.SetActive(true);
        head.SetActive(true);
        ((BoxCollider2D)coll).size = initSize;
        isRoll = false;
        canGrabLedge = true;
        fixFlip = false;
    }

    IEnumerator wallJumpX(float forceX = 0f, float time = 0.1f)
    {
        //특정 상황에서 한 쪽으로 힘을 주기 위한 메소드
        fixFlip = true;
        transform.localScale = new Vector3(Mathf.Sign(forceX), transform.localScale.y, transform.localScale.z);

        Debug.Log("forceX=" + forceX + ": localScale=" + transform.localScale.x);
        xspeed = forceX;
        yield return new WaitForSeconds(time);
        fixFlip = false;
        isWallJump = false;
    }

    IEnumerator rollX(float forceX = 0f, float time = 0.1f)
    {
        //특정 상황에서 한 쪽으로 힘을 주기 위한 메소드
        fixFlip = true;
        transform.localScale = new Vector3(Mathf.Sign(forceX), transform.localScale.y, transform.localScale.z);

        Debug.Log("forceX=" + forceX + ": localScale=" + transform.localScale.x);
        xspeed = forceX;
        yield return new WaitForSeconds(time);
        fixFlip = false;
    }

    void FixedUpdate()
    {
        //몇몇 조건들을 위한 레이케스트가 있음.
        isGrounded = Physics2D.Raycast(new Vector2(transform.position.x - 0.2f, transform.position.y), Vector2.down, 0.6f, groundMask)
            || Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.down, 0.6f, groundMask)
            || Physics2D.Raycast(new Vector2(transform.position.x + 0.2f, transform.position.y), Vector2.down, 0.6f, groundMask);
        isLedge = !Physics2D.Raycast(ledgeCheck.position, Vector2.right*Mathf.Sign((int)Managers.Input.CurrentMoveDir), 0.5f, groundMask) && !isWallUp &&
            Physics2D.Raycast(new Vector2(ledgeCheck.position.x, ledgeCheck.position.y - 0.2f), Vector2.right * Mathf.Sign((int)Managers.Input.CurrentMoveDir), 0.5f, platformMask);
        isWallFront = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.1f), new Vector2(Mathf.Sign((int)Managers.Input.CurrentMoveDir), 0), 0.4f, platformMask);
        isWallRight = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.1f), Vector2.right, 0.8f, platformMask);
        isWallLeft = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.1f), Vector2.left, 0.8f, platformMask);
        isWallUp = Physics2D.Raycast(transform.position, Vector2.up, 0.8f, groundMask);
        isSlope = Physics2D.Raycast(transform.position, Vector2.down, 1f, slopeMask);

        //키보드 이벤트(스프라이트 떨림 현상 때문에 deltaTime을 쓰지 않고 여기에 정의함.)
        rigid.velocity = new Vector2(xspeed * 60, rigid.velocity.y);
        //애니메이션 관련 이벤트
        Look();

        //점프 animation
        if (!isGrounded)
        {
            if (rigid.velocity.y > 0.2f)
            {
                animator.SetBool("isFall", false);
                animator.SetBool("isJump", true);
            }
            else if (rigid.velocity.y < -0.2f)
            {
                animator.SetBool("isJump", false);
                animator.SetBool("isFall", true);
            }
        }
        else
        {
            animator.SetBool("isFall", false);
            animator.SetBool("isJump", false);
        }

        if(rigid.velocity.y<0)
        {
            if((isWallRight && Managers.Input.GetInput(Define.InputType.Right))||
                (isWallLeft && Managers.Input.GetInput(Define.InputType.Left)))
            {
                rigid.AddForce(Vector2.up * 0.4f, ForceMode2D.Impulse);
                animator.SetBool("isHang", true);
            }
            else
            {
                animator.SetBool("isHang", false);
            }
        }

    }

    public override void OnDeSpawn()
    {
        base.OnDeSpawn();
        //GetComponent<PlayerGun>().Despawn();
    }
}
