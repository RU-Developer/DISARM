using UnityEngine;
using System.Collections;
using System;
using UnityEngine.EventSystems;

/**
 * UI이벤트가 아닌 마우스나 키보드의 직접 입력이 발생하는 경우에 대해
 * 이벤트를 받아서 전달하는 클래스입니다.
 * UI를 컨트롤 하는 키보드 이벤트도 여기서 작업 합니다.
 */
public class InputManager
{
    private Action KeyAction = null;
    private Action<Define.MouseEvent> MouseAction = null;
    private Action UIKeyAction = null;

    private Action<Define.InputType> InputAction = null;

    // 이전 마우스나 조이스틱 각도
    public double LastAngle { get; private set; } = 0;
    // 현재 마우스나 조이스틱 각도
    public double CurrentAngle { get; private set; } = 0;
    // 현재 조준 방향
    public Define.InputDir CurrentAimDir { get; private set; } = Define.InputDir.Right;
    // 현재 이동 방향
    public Define.InputDir CurrentMoveDir { get; private set; } = Define.InputDir.Right;
    // 조준 방향과 이동 방향은 Pause되지 않은 상태일 때만 변함

    private GameObject _player;

    private bool _leftMousePressed = false; //현재 마우스가 눌러지고 있는 상태인지 여부
    private float _leftMousePressedTime = 0;

    private bool _rightMousePressed = false;
    private float _rightMousePressedTime = 0;

    private bool _joyStickPressed = false;

    // JoyStick 값을 위가 0인 360도 값으로 반환
    // 조이스틱의 x, y값을 각도로 변환하는 함수
    double JoystickToAngle(int x, int y)
    {
        LastAngle = CurrentAngle;
        // x, y값이 500 ~ 550 사이면 조이스틱이 움직이지 않은 것으로 판단하고 이전 값을 반환
        if (x >= 500 && x <= 550 && y >= 500 && y <= 550)
        {
            _joyStickPressed = true;
            return CurrentAngle;
        }

        // x, y값을 double로 형변환하고, Math.Atan2 함수를 이용하여 라디안 값을 구함
        double radian = Math.Atan2((double)y - 512, (double)x - 512);

        // 라디안 값을 각도로 변환하고, 0 ~ 360 범위로 맞춤
        CurrentAngle = radian * 180 / Math.PI;
        if (CurrentAngle < 0)
        {
            CurrentAngle += 360;
        }

        // 각도를 반시계 방향으로 90도 회전시킴
        CurrentAngle = (CurrentAngle + 270) % 360;
        return CurrentAngle;
    }

    public void OnUpdate()
    {
        Debug.Log("Mouse Angle: " + CurrentAngle);

        // 컨트롤러를 사용하는 경우 마우스를 이용한 조작 불가
        if (Managers.Network.IsConnected)
            JoystickToAngle(Managers.Network.X, Managers.Network.Y);
        else // Mouse Screen과 Player의 위치를 통해 각도 구하기
        {
            _player = Managers.Game.GetPlayer();
            if (_player != null) // 플레이어가 null이 아닌지 확인
            {
                LastAngle = CurrentAngle; // 마지막 각도를 현재 각도로 설정
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
                if (CurrentAngle < 0) // 각도가 0보다 작으면 360을 더하여 0 ~ 360 범위로 맞춤
                {
                    CurrentAngle += 360;
                }
            }
        }

        //일시정지시
        if (Managers.Pause.IsPause)
        {
            //UI 키보드 이벤트는 일시정지된 상태에서도 진행
            if (Input.anyKey && UIKeyAction != null)
                UIKeyAction.Invoke();

            // UI 이벤트
            if (InputAction == null)
                return;

            if (Input.GetKey(KeyCode.Escape))
            {
                InputAction.Invoke(Define.InputType.Menu);
                InputAction.Invoke(Define.InputType.Cancel);
            }
            
            if (Input.GetKey(KeyCode.Return))
                InputAction.Invoke(Define.InputType.Check);

            if (Input.GetKey(KeyCode.W))
                InputAction.Invoke(Define.InputType.Up);
            if (Input.GetKey(KeyCode.D))
                InputAction.Invoke(Define.InputType.Right);
            if (Input.GetKey(KeyCode.S))
                InputAction.Invoke(Define.InputType.Down);
            if (Input.GetKey(KeyCode.A))
                InputAction.Invoke(Define.InputType.Left);


            if (Managers.Network.IsConnected)
            {
                if (Managers.Network.Button5 != 0)
                    InputAction.Invoke(Define.InputType.Menu);
                if (Managers.Network.Button3 != 0)
                    InputAction.Invoke(Define.InputType.Check);
                if (Managers.Network.Button2 != 0)
                    InputAction.Invoke(Define.InputType.Cancel);

                if ((CurrentAngle >= 0 && CurrentAngle < 45 || CurrentAngle >= 315) && _joyStickPressed)
                {
                    InputAction.Invoke(Define.InputType.Up);
                    _joyStickPressed = false;
                }
                else if (CurrentAngle >= 45 && CurrentAngle < 135 && _joyStickPressed)
                {
                    InputAction.Invoke(Define.InputType.Right);
                    _joyStickPressed = false;
                }
                else if (CurrentAngle >= 135 && CurrentAngle < 225 && _joyStickPressed)
                {
                    InputAction.Invoke(Define.InputType.Down);
                    _joyStickPressed = false;
                }
                else if (CurrentAngle >= 225 && CurrentAngle < 360 && _joyStickPressed)
                {
                    InputAction.Invoke(Define.InputType.Left);
                    _joyStickPressed = false;
                }
            }

            return;
        }

        //UI 키보드 이벤트
        if (Input.anyKey && UIKeyAction != null)
            UIKeyAction.Invoke();

        //키보드 이벤트
        if (Input.anyKey && KeyAction != null)
            KeyAction.Invoke();

        //UI 위에 마우스 있을 시
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

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


            if (InputAction == null)
                return;

            if (Input.GetKey(KeyCode.Escape))
            {
                InputAction.Invoke(Define.InputType.Menu);
                InputAction.Invoke(Define.InputType.Cancel);
            }

            if (Input.GetKey(KeyCode.Return))
                InputAction.Invoke(Define.InputType.Check);

            if (Input.GetMouseButton(1))
            {
                if (!_rightMousePressed)
                {
                    _rightMousePressedTime = Time.time;
                }
                _rightMousePressed = true;
            }
            else
            {
                if (_rightMousePressed) //클릭을 땠을 때
                {
                    if (Time.time < _rightMousePressedTime + 0.2f) //누른 기간이 0.2초 이내인 경우
                        InputAction.Invoke(Define.InputType.Attack);
                }
                _rightMousePressed = false;
                _rightMousePressedTime = 0;
            }

            if (Input.GetKey(KeyCode.Space))
                InputAction.Invoke(Define.InputType.Jump);

            if (Input.GetKey(KeyCode.LeftShift))
                InputAction.Invoke(Define.InputType.Skill1);

            if (Input.GetMouseButton(0))
            {
                if (!_leftMousePressed)
                {
                    _leftMousePressedTime = Time.time;
                }
                _leftMousePressed = true;
            }
            else
            {
                if (_leftMousePressed) //클릭을 땠을 때
                {
                    if (Time.time < _leftMousePressedTime + 0.2f) //누른 기간이 0.2초 이내인 경우
                        InputAction.Invoke(Define.InputType.Skill2);
                }
                _leftMousePressed = false;
                _leftMousePressedTime = 0;
            }

            if (Input.GetKey(KeyCode.W))
            {
                InputAction.Invoke(Define.InputType.Up);
            }
            if (Input.GetKey(KeyCode.D))
            {
                CurrentMoveDir = Define.InputDir.Right;
                InputAction.Invoke(Define.InputType.Right);
            }
            if (Input.GetKey(KeyCode.S))
            {
                InputAction.Invoke(Define.InputType.Down);
            }
            if (Input.GetKey(KeyCode.A))
            {
                CurrentMoveDir = Define.InputDir.Left;
                InputAction.Invoke(Define.InputType.Left);
            }

            if (Managers.Network.IsConnected)
            {
                if (Managers.Network.Button5 != 0)
                    InputAction.Invoke(Define.InputType.Menu);
                if (Managers.Network.Button3 != 0)
                    InputAction.Invoke(Define.InputType.Check);
                if (Managers.Network.Button2 != 0)
                    InputAction.Invoke(Define.InputType.Cancel);
                if (Managers.Network.Button4 != 0)
                    InputAction.Invoke(Define.InputType.Attack);
                if (Managers.Network.Button3 != 0)
                    InputAction.Invoke(Define.InputType.Jump);
                if (Managers.Network.Button1 != 0)
                    InputAction.Invoke(Define.InputType.Skill1);
                if (Managers.Network.Button2 != 0)
                    InputAction.Invoke(Define.InputType.Skill2);

                CurrentMoveDir = Managers.Network.Move == 0b10 ? CurrentMoveDir :
                    Managers.Network.Move == 0b00 ? Define.InputDir.Left : Define.InputDir.Right;

                if ((CurrentAngle >= 0 && CurrentAngle < 45 || CurrentAngle >= 315) && _joyStickPressed)
                {
                    CurrentAimDir = Define.InputDir.Up;
                    InputAction.Invoke(Define.InputType.Up);
                    _joyStickPressed = false;
                }
                else if (CurrentAngle >= 45 && CurrentAngle < 135 && _joyStickPressed)
                {
                    CurrentAimDir = Define.InputDir.Right;
                    InputAction.Invoke(Define.InputType.Right);
                    _joyStickPressed = false;
                }
                else if (CurrentAngle >= 135 && CurrentAngle < 225 && _joyStickPressed)
                {
                    CurrentAimDir = Define.InputDir.Down;
                    InputAction.Invoke(Define.InputType.Down);
                    _joyStickPressed = false;
                }
                else if (CurrentAngle >= 225 && CurrentAngle < 360 && _joyStickPressed)
                {
                    CurrentAimDir = Define.InputDir.Left;
                    InputAction.Invoke(Define.InputType.Left);
                    _joyStickPressed = false;
                }
            }
        }
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
     * Input 조작 통합 이벤트 핸들러 등록
     */
    public void AddInputAction(Action<Define.InputType> inputEventHandler)
    {
        InputAction -= inputEventHandler;
        InputAction += inputEventHandler;
    }

    /**
     * Input 조작 통합 이벤트 핸들러 등록 해제
     */
    public void RemoveInputAction(Action<Define.InputType> inputEventHandler)
    {
        InputAction -= inputEventHandler;
    }

    /**
     * 화면 전환 등에 따라 이벤트 등록 전부해제
     */
    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
        UIKeyAction = null;
        InputAction = null;
    }
}
