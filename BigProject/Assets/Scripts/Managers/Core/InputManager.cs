using UnityEngine;
using System;
using UnityEngine.EventSystems;

/**
 * UI이벤트가 아닌 마우스나 키보드의 직접 입력이 발생하는 경우에 대해
 * 이벤트를 받아서 전달하는 클래스입니다.
 * UI를 컨트롤 하는 키보드 이벤트도 여기서 작업 합니다.
 */
public class InputManager
{
    // InputMap
    private bool[] _inputDownMap = new bool[(int)Define.InputType.Length];
    private bool[] _inputMap = new bool[(int)Define.InputType.Length];
    private bool[] _inputUpMap = new bool[(int)Define.InputType.Length];

    private Action KeyAction = null;
    private Action<Define.MouseEvent> MouseAction = null;
    private Action UIKeyAction = null;

    // 현재 마우스나 조이스틱 각도
    public double CurrentAngle { get; private set; } = 0;

    public double GunAngle { get; private set; } = 0;
    // 현재 이동 방향
    public Define.InputDir CurrentMoveDir { get; private set; } = Define.InputDir.Right;

    /**
     * Update 한번당 한번씩 초기화됨
     */
    public bool GetInputDown(Define.InputType inputType)
    {
        return _inputDownMap[(int)inputType];
    }

    /**
     * 계속 눌리는 상태인지
     */
    public bool GetInput(Define.InputType inputType)
    {
        return _inputMap[(int)inputType];
    }

    /**
     * 땠을 때 한번 측정함
     */
    public bool GetInputUp(Define.InputType inputType)
    {
        return _inputUpMap[(int)inputType];
    }



    private GameObject _player;

    private bool _leftMousePressed = false; //현재 마우스가 눌러지고 있는 상태인지 여부
    private float _leftMousePressedTime = 0;

    private bool _joyStickPressed = false;



    // JoyStick 값을 위가 0인 360도 값으로 반환
    // 조이스틱의 x, y값을 각도로 변환하는 함수
    double JoystickToAngle(int x, int y)
    {
        // x, y값이 500 ~ 550 사이면 조이스틱이 움직이지 않은 것으로 판단하고 이전 값을 반환
        if (x >= 500 && x <= 550 && y >= 500 && y <= 550)
        {
            _joyStickPressed = true;
            return CurrentAngle;
        }

        // x, y값을 double로 형변환하고, Math.Atan2 함수를 이용하여 라디안 값을 구함
        double radian = Math.Atan2(512 - (double)y, (double)x - 512);

        // 라디안 값을 각도로 변환하고, 0 ~ 360 범위로 맞춤
        CurrentAngle = radian * 180 / Math.PI;
        if (CurrentAngle < 0)
        {
            CurrentAngle += 360;
        }

        // 각도를 반시계 방향으로 90도 회전시킴
        CurrentAngle = (CurrentAngle + 270) % 360;

        // TODO: GunAngle 넣어줘야 함
        return CurrentAngle;
    }

    public void OnUpdate()
    {
        #region 조준 각도 설정
        // 컨트롤러를 사용하는 경우 마우스를 이용한 조작 불가
        if (Managers.Network.IsConnected)
            JoystickToAngle(Managers.Network.X, Managers.Network.Y);
        else // Mouse Screen과 Player의 위치를 통해 각도 구하기
        {
            _player = Managers.Game.GetPlayer();
            if (_player != null) // 플레이어가 null이 아닌지 확인
            {
                Transform playerTransform = _player.transform; // 플레이어의 transform 컴포넌트를 저장

                Vector3 mousePosition = Input.mousePosition; // 마우스의 스크린 상의 위치를 입력 받음
                                                             // 마우스의 스크린 상의 위치를 월드 좌표로 변환함. 
                Vector3 mouseWorldPosition = 
                    Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 
                        playerTransform.position.z)); // 여기서 z 좌표는 플레이어의 z 좌표와 동일하게 설정
                Vector3 playerToMouse = mouseWorldPosition - playerTransform.position; // 플레이어와 마우스 사이의 벡터를 구함

                // Mathf.Atan2 함수를 사용하여 플레이어와 마우스 사이의 벡터의 각도를 구함.
                // 이 함수는 -π에서 π까지의 범위에서 각도를 반환하므로,
                // 각도를 0에서 360까지의 범위로 변환하기 위해 180을 더하고 결과에 180을 곱함
                CurrentAngle = Mathf.Atan2(playerToMouse.x, playerToMouse.y) * Mathf.Rad2Deg;

                GunAngle = Mathf.Atan2(Mathf.Abs(playerToMouse.x), playerToMouse.y) * Mathf.Rad2Deg;
                if (CurrentAngle < 0) // 각도가 0보다 작으면 360을 더하여 0 ~ 360 범위로 맞춤
                    CurrentAngle += 360;
            }
        }
        #endregion

        #region 일시정지 상태일 경우(메뉴를 연 경우)
        //일시정지시
        if (Managers.Pause.IsPause)
        {
            //UI 키보드 이벤트는 일시정지된 상태에서도 진행
            if (Input.anyKey && UIKeyAction != null)
                UIKeyAction.Invoke();
            return;
        }
        #endregion

        #region 플레이중 키보드 조작
        //UI 키보드 이벤트
        if (Input.anyKey && UIKeyAction != null)
            UIKeyAction.Invoke();

        //키보드 이벤트
        if (Input.anyKey && KeyAction != null)
            KeyAction.Invoke();
        #endregion
        //UI 위에 마우스 있을 시: UI 마우스 이벤트는 따로 다룸. InputManager가 아님
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        #region 마우스 이벤트
        //마우스 이벤트
        if (MouseAction != null)
        {
            if (Input.GetMouseButton(0)) //좌클릭
            {
                if (!_leftMousePressed)
                {
                    MouseAction.Invoke(Define.MouseEvent.PointerDown);
                    _leftMousePressedTime = Time.time;
                }
                MouseAction.Invoke(Define.MouseEvent.Press);
                _leftMousePressed = true;
            }
            else
            {
                if (_leftMousePressed) //클릭을 땠을 때
                {
                    if (Time.time < _leftMousePressedTime + 0.2f) //누른 기간이 0.2초 이내인 경우
                        MouseAction.Invoke(Define.MouseEvent.Click);
                    MouseAction.Invoke(Define.MouseEvent.PointerUp);
                }
                _leftMousePressed = false;
                _leftMousePressedTime = 0;
            }
        }
        #endregion
    }

    /**
     * Update 끝나고 실행 되는거.
     * Input Map을 여기서 업데이트
     * Input Down에 대해서 처리할 것
     * Press, Up을 이용
     * Press는 계속 누르고 있는 것. 누르고 있는 동안 딱 한번만 처리할 것
     * Up은 땐 것. 때고 나야 다시 키보드 맵에 등록할 수 있게됨
     */
    public void LateUpdate()
    {
        // 매 Update마다 초기화
        for (int i = 0; i < _inputUpMap.Length; i++)
        {
            _inputDownMap[i] = false;
            _inputUpMap[i] = false;
        }

        #region 네트워크 연결 안되있을 때(키보드 마우스 조작)
        if (Managers.Network.IsConnected == false)
        {
            #region InputType Left
            if (Input.GetKey(KeyCode.A))
            {
                if (Managers.Pause.IsPause == false)
                    CurrentMoveDir = Define.InputDir.Left;
                // 안눌리고 있었으면
                if (_inputMap[(int)Define.InputType.Left] == false)
                {
                    _inputDownMap[(int)Define.InputType.Left] = true;
                    _inputMap[(int)Define.InputType.Left] = true;
                }
            }
            else
            {
                // 눌렀다가 때는 경우
                if (_inputMap[(int)Define.InputType.Left])
                {
                    _inputMap[(int)Define.InputType.Left] = false;
                    _inputUpMap[(int)Define.InputType.Left] = true;
                }
            }
            #endregion
            #region InputType Right
            if (Input.GetKey(KeyCode.D))
            {
                if (Managers.Pause.IsPause == false)
                    CurrentMoveDir = Define.InputDir.Right;
                // 안눌리고 있었으면
                if (_inputMap[(int)Define.InputType.Right] == false)
                {
                    _inputDownMap[(int)Define.InputType.Right] = true;
                    _inputMap[(int)Define.InputType.Right] = true;
                }
            }
            else
            {
                // 눌렀다가 때는 경우
                if (_inputMap[(int)Define.InputType.Right])
                {
                    _inputMap[(int)Define.InputType.Right] = false;
                    _inputUpMap[(int)Define.InputType.Right] = true;
                }
            }
            #endregion
            #region InputType Up
            if (Input.GetKey(KeyCode.W))
            {
                // 안눌리고 있었으면
                if (_inputMap[(int)Define.InputType.Up] == false)
                {
                    _inputDownMap[(int)Define.InputType.Up] = true;
                    _inputMap[(int)Define.InputType.Up] = true;
                }
            }
            else
            {
                // 눌렀다가 때는 경우
                if (_inputMap[(int)Define.InputType.Up])
                {
                    _inputMap[(int)Define.InputType.Up] = false;
                    _inputUpMap[(int)Define.InputType.Up] = true;
                }
            }
            #endregion
            #region InputType Down
            if (Input.GetKey(KeyCode.S))
            {
                // 안눌리고 있었으면
                if (_inputMap[(int)Define.InputType.Down] == false)
                {
                    _inputDownMap[(int)Define.InputType.Down] = true;
                    _inputMap[(int)Define.InputType.Down] = true;
                }
            }
            else
            {
                // 눌렀다가 때는 경우
                if (_inputMap[(int)Define.InputType.Down])
                {
                    _inputMap[(int)Define.InputType.Down] = false;
                    _inputUpMap[(int)Define.InputType.Down] = true;
                }
            }
            #endregion

            #region InputType Ok
            if (Input.GetKey(KeyCode.Return))
            {
                // 안눌리고 있었으면
                if (_inputMap[(int)Define.InputType.Ok] == false)
                {
                    _inputDownMap[(int)Define.InputType.Ok] = true;
                    _inputMap[(int)Define.InputType.Ok] = true;
                }
            }
            else
            {
                // 눌렀다가 때는 경우
                if (_inputMap[(int)Define.InputType.Ok])
                {
                    _inputMap[(int)Define.InputType.Ok] = false;
                    _inputUpMap[(int)Define.InputType.Ok] = true;
                }
            }
            #endregion
            #region InputType Cancel
            if (Input.GetKey(KeyCode.Escape))
            {
                // 안눌리고 있었으면
                if (_inputMap[(int)Define.InputType.Cancel] == false)
                {
                    _inputDownMap[(int)Define.InputType.Cancel] = true;
                    _inputMap[(int)Define.InputType.Cancel] = true;
                }
            }
            else
            {
                // 눌렀다가 때는 경우
                if (_inputMap[(int)Define.InputType.Cancel])
                {
                    _inputMap[(int)Define.InputType.Cancel] = false;
                    _inputUpMap[(int)Define.InputType.Cancel] = true;
                }
            }
            #endregion

            #region InputType Menu
            if (Input.GetKey(KeyCode.Escape))
            {
                // 안눌리고 있었으면
                if (_inputMap[(int)Define.InputType.Menu] == false)
                {
                    _inputDownMap[(int)Define.InputType.Menu] = true;
                    _inputMap[(int)Define.InputType.Menu] = true;
                }
            }
            else
            {
                // 눌렀다가 때는 경우
                if (_inputMap[(int)Define.InputType.Menu])
                {
                    _inputMap[(int)Define.InputType.Menu] = false;
                    _inputUpMap[(int)Define.InputType.Menu] = true;
                }
            }
            #endregion

            #region InputType Attack
            // UI 위가 아닌 곳에 마우스가 있을때
            if (EventSystem.current == null || EventSystem.current.IsPointerOverGameObject() == false)
            {
                if (Input.GetMouseButton(1)) // 우클릭
                {
                    // 안눌리고 있었으면
                    if (_inputMap[(int)Define.InputType.Attack] == false)
                    {
                        _inputDownMap[(int)Define.InputType.Attack] = true;
                        _inputMap[(int)Define.InputType.Attack] = true;
                    }
                }
                else
                {
                    // 눌렀다가 때는 경우
                    if (_inputMap[(int)Define.InputType.Attack])
                    {
                        _inputMap[(int)Define.InputType.Attack] = false;
                        _inputUpMap[(int)Define.InputType.Attack] = true;
                    }
                }
            } 
            #endregion
            #region InputType Jump
            if (Input.GetKey(KeyCode.Space))
            {
                // 안눌리고 있었으면
                if (_inputMap[(int)Define.InputType.Jump] == false)
                {
                    _inputDownMap[(int)Define.InputType.Jump] = true;
                    _inputMap[(int)Define.InputType.Jump] = true;
                }
            }
            else
            {
                // 눌렀다가 때는 경우
                if (_inputMap[(int)Define.InputType.Jump])
                {
                    _inputMap[(int)Define.InputType.Jump] = false;
                    _inputUpMap[(int)Define.InputType.Jump] = true;
                }
            }
            #endregion

            #region InputType Skill1
            if (Input.GetKey(KeyCode.LeftShift))
            {
                // 안눌리고 있었으면
                if (_inputMap[(int)Define.InputType.Skill1] == false)
                {
                    _inputDownMap[(int)Define.InputType.Skill1] = true;
                    _inputMap[(int)Define.InputType.Skill1] = true;
                }
            }
            else
            {
                // 눌렀다가 때는 경우
                if (_inputMap[(int)Define.InputType.Skill1])
                {
                    _inputMap[(int)Define.InputType.Skill1] = false;
                    _inputUpMap[(int)Define.InputType.Skill1] = true;
                }
            }
            #endregion
            #region InputType Skill2
            //UI가 아닌 곳 위에 마우스가 있을 때
            if (EventSystem.current == null || EventSystem.current.IsPointerOverGameObject() == false)
            {
                if (Input.GetMouseButton(0)) // 좌클릭
                {
                    // 안눌리고 있었으면
                    if (_inputMap[(int)Define.InputType.Skill2] == false)
                    {
                        _inputDownMap[(int)Define.InputType.Skill2] = true;
                        _inputMap[(int)Define.InputType.Skill2] = true;
                    }
                }
                else
                {
                    // 눌렀다가 때는 경우
                    if (_inputMap[(int)Define.InputType.Skill2])
                    {
                        _inputMap[(int)Define.InputType.Skill2] = false;
                        _inputUpMap[(int)Define.InputType.Skill2] = true;
                    }
                }
            }
            #endregion
        }
        #endregion
        #region 네트워크 연결이 되어있을 때(컨트롤러 조작)
        else
        {
            // 일시정지 상태일 경우
            if (Managers.Pause.IsPause)
            {
                #region InputType Up
                if (CurrentAngle >= 0 && CurrentAngle < 45 || CurrentAngle >= 315)
                {
                    // 안눌리고 있었으면
                    if (_inputMap[(int)Define.InputType.Up] == false)
                    {
                        _inputDownMap[(int)Define.InputType.Up] = true;
                        _inputMap[(int)Define.InputType.Up] = true;
                    }
                }
                else
                {
                    // 눌렀다가 때는 경우
                    if (_inputMap[(int)Define.InputType.Up])
                    {
                        _inputMap[(int)Define.InputType.Up] = false;
                        _inputUpMap[(int)Define.InputType.Up] = true;
                    }
                }
                #endregion
                #region InputType Right
                if (CurrentAngle >= 45 && CurrentAngle < 135 && _joyStickPressed)
                {
                    // 안눌리고 있었으면
                    if (_inputMap[(int)Define.InputType.Right] == false)
                    {
                        _inputDownMap[(int)Define.InputType.Right] = true;
                        _inputMap[(int)Define.InputType.Right] = true;
                    }
                }
                else
                {
                    // 눌렀다가 때는 경우
                    if (_inputMap[(int)Define.InputType.Right])
                    {
                        _inputMap[(int)Define.InputType.Right] = false;
                        _inputUpMap[(int)Define.InputType.Right] = true;
                    }
                }
                #endregion
                #region InputType Down
                if (CurrentAngle >= 135 && CurrentAngle < 225 && _joyStickPressed)
                {
                    // 안눌리고 있었으면
                    if (_inputMap[(int)Define.InputType.Down] == false)
                    {
                        _inputDownMap[(int)Define.InputType.Down] = true;
                        _inputMap[(int)Define.InputType.Down] = true;
                    }
                }
                else
                {
                    // 눌렀다가 때는 경우
                    if (_inputMap[(int)Define.InputType.Down])
                    {
                        _inputMap[(int)Define.InputType.Down] = false;
                        _inputUpMap[(int)Define.InputType.Down] = true;
                    }
                }
                #endregion
                #region InputType Left
                if (CurrentAngle >= 225 && CurrentAngle < 360 && _joyStickPressed)
                {
                    // 안눌리고 있었으면
                    if (_inputMap[(int)Define.InputType.Left] == false)
                    {
                        _inputDownMap[(int)Define.InputType.Left] = true;
                        _inputMap[(int)Define.InputType.Left] = true;
                    }
                }
                else
                {
                    // 눌렀다가 때는 경우
                    if (_inputMap[(int)Define.InputType.Left])
                    {
                        _inputMap[(int)Define.InputType.Left] = false;
                        _inputUpMap[(int)Define.InputType.Left] = true;
                    }
                }
                #endregion
            }
            else
            {
                #region InputType Left
                if (Managers.Network.Move == 0b00)
                {
                    CurrentMoveDir = Define.InputDir.Left;
                    // 안눌리고 있었으면
                    if (_inputMap[(int)Define.InputType.Left] == false)
                    {
                        _inputDownMap[(int)Define.InputType.Left] = true;
                        _inputMap[(int)Define.InputType.Left] = true;
                    }
                }
                else
                {
                    // 눌렀다가 때는 경우
                    if (_inputMap[(int)Define.InputType.Left])
                    {
                        _inputMap[(int)Define.InputType.Left] = false;
                        _inputUpMap[(int)Define.InputType.Left] = true;
                    }
                }
                #endregion
                #region InputType Right
                if (Managers.Network.Move == 0b01)
                {
                    CurrentMoveDir = Define.InputDir.Right;
                    // 안눌리고 있었으면
                    if (_inputMap[(int)Define.InputType.Right] == false)
                    {
                        _inputDownMap[(int)Define.InputType.Right] = true;
                        _inputMap[(int)Define.InputType.Right] = true;
                    }
                }
                else
                {
                    // 눌렀다가 때는 경우
                    if (_inputMap[(int)Define.InputType.Right])
                    {
                        _inputMap[(int)Define.InputType.Right] = false;
                        _inputUpMap[(int)Define.InputType.Right] = true;
                    }
                }
                #endregion
            }

            // 나머지는 Pause에 관계 없이 안겹치기 때문에 그냥 받음
            #region InputType Ok
            if (Managers.Network.Button3 != 0)
            {
                // 안눌리고 있었으면
                if (_inputMap[(int)Define.InputType.Ok] == false)
                {
                    _inputDownMap[(int)Define.InputType.Ok] = true;
                    _inputMap[(int)Define.InputType.Ok] = true;
                }
            }
            else
            {
                // 눌렀다가 때는 경우
                if (_inputMap[(int)Define.InputType.Ok])
                {
                    _inputMap[(int)Define.InputType.Ok] = false;
                    _inputUpMap[(int)Define.InputType.Ok] = true;
                }
            }
            #endregion
            #region InputType Cancel
            if (Managers.Network.Button2 != 0)
            {
                // 안눌리고 있었으면
                if (_inputMap[(int)Define.InputType.Cancel] == false)
                {
                    _inputDownMap[(int)Define.InputType.Cancel] = true;
                    _inputMap[(int)Define.InputType.Cancel] = true;
                }
            }
            else
            {
                // 눌렀다가 때는 경우
                if (_inputMap[(int)Define.InputType.Cancel])
                {
                    _inputMap[(int)Define.InputType.Cancel] = false;
                    _inputUpMap[(int)Define.InputType.Cancel] = true;
                }
            }
            #endregion

            #region InputType Menu
            if (Managers.Network.Button5 != 0)
            {
                // 안눌리고 있었으면
                if (_inputMap[(int)Define.InputType.Menu] == false)
                {
                    _inputDownMap[(int)Define.InputType.Menu] = true;
                    _inputMap[(int)Define.InputType.Menu] = true;
                }
            }
            else
            {
                // 눌렀다가 때는 경우
                if (_inputMap[(int)Define.InputType.Menu])
                {
                    _inputMap[(int)Define.InputType.Menu] = false;
                    _inputUpMap[(int)Define.InputType.Menu] = true;
                }
            }
            #endregion

            #region InputType Attack
            if (Managers.Network.Button4 != 0)
            {
                // 안눌리고 있었으면
                if (_inputMap[(int)Define.InputType.Attack] == false)
                {
                    _inputDownMap[(int)Define.InputType.Attack] = true;
                    _inputMap[(int)Define.InputType.Attack] = true;
                }
            }
            else
            {
                // 눌렀다가 때는 경우
                if (_inputMap[(int)Define.InputType.Attack])
                {
                    _inputMap[(int)Define.InputType.Attack] = false;
                    _inputUpMap[(int)Define.InputType.Attack] = true;
                }
            }
            #endregion
            #region InputType Jump
            if (Managers.Network.Button3 != 0)
            {
                // 안눌리고 있었으면
                if (_inputMap[(int)Define.InputType.Jump] == false)
                {
                    _inputDownMap[(int)Define.InputType.Jump] = true;
                    _inputMap[(int)Define.InputType.Jump] = true;
                }
            }
            else
            {
                // 눌렀다가 때는 경우
                if (_inputMap[(int)Define.InputType.Jump])
                {
                    _inputMap[(int)Define.InputType.Jump] = false;
                    _inputUpMap[(int)Define.InputType.Jump] = true;
                }
            }
            #endregion

            #region InputType Skill1
            if (Managers.Network.Button1 != 0)
            {
                // 안눌리고 있었으면
                if (_inputMap[(int)Define.InputType.Skill1] == false)
                {
                    _inputDownMap[(int)Define.InputType.Skill1] = true;
                    _inputMap[(int)Define.InputType.Skill1] = true;
                }
            }
            else
            {
                // 눌렀다가 때는 경우
                if (_inputMap[(int)Define.InputType.Skill1])
                {
                    _inputMap[(int)Define.InputType.Skill1] = false;
                    _inputUpMap[(int)Define.InputType.Skill1] = true;
                }
            }
            #endregion
            #region InputType Skill2
            if (Managers.Network.Button2 != 0)
            {
                // 안눌리고 있었으면
                if (_inputMap[(int)Define.InputType.Skill2] == false)
                {
                    _inputDownMap[(int)Define.InputType.Skill2] = true;
                    _inputMap[(int)Define.InputType.Skill2] = true;
                }
            }
            else
            {
                // 눌렀다가 때는 경우
                if (_inputMap[(int)Define.InputType.Skill2])
                {
                    _inputMap[(int)Define.InputType.Skill2] = false;
                    _inputUpMap[(int)Define.InputType.Skill2] = true;
                }
            }
            #endregion
        }
        #endregion
    }

    /**
     * 키보드 이벤트 핸들러 설정
     */
    public void AddKeyAction(Action keyEventHandler)
    {
        KeyAction -= keyEventHandler;
        KeyAction += keyEventHandler;
    }

    /**
     * 키보드 이벤트 핸들러 해제
     */
    public void RemoveKeyAction(Action keyEventHandler)
    {
        KeyAction -= keyEventHandler;
    }

    /**
     * 마우스 이벤트 핸들러 설정
     */
    public void AddMouseAction(Action<Define.MouseEvent> mouseEventHandler)
    {
        MouseAction -= mouseEventHandler;
        MouseAction += mouseEventHandler;
    }

    /**
     * 마우스 이벤트 핸들러 해제
     */
    public void RemoveMouseAction(Action<Define.MouseEvent> mouseEventHandler)
    {
        MouseAction -= mouseEventHandler;
    }

    /**
     * UI 조작 키보드 이벤트 핸들러 설정
     */
    public void AddUIKeyAction(Action uiEventHandler)
    {
        UIKeyAction -= uiEventHandler;
        UIKeyAction += uiEventHandler;
    }

    /**
     * UI 조작 키보드 이벤트 핸들러 해제
     */
    public void RemoveUIKeyAction(Action uiEventHandler)
    {
        UIKeyAction -= uiEventHandler;
    }

    /**
     * 화면 전환 등에 따라 이벤트 등록 전부해제
     */
    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
        UIKeyAction = null;
    }
}
