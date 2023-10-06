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

    bool _pressed = false; //현재 마우스가 눌러지고 있는 상태인지 여부
    float _pressedTime = 0;

    public void OnUpdate()
    {
        //일시정지시
        if (Managers.Pause.IsPause)
        {
            //UI 키보드 이벤트는 일시정지된 상태에서도 진행
            if (Input.anyKey && UIKeyAction != null)
                UIKeyAction.Invoke();
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
                if (!_pressed)
                {
                    MouseAction.Invoke(Define.MouseEvent.PointerDown);
                    _pressedTime = Time.time;
                }
                MouseAction.Invoke(Define.MouseEvent.Press);
                _pressed = true;
            }
            else
            {
                if (_pressed) //클릭을 땠을 때
                {
                    if (Time.time < _pressedTime + 0.2f) //누른 기간이 0.2초 이내인 경우
                        MouseAction.Invoke(Define.MouseEvent.Click);
                    MouseAction.Invoke(Define.MouseEvent.PointerUp);
                }
                _pressed = false;
                _pressedTime = 0;
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
     * 화면 전환 등에 따라 이벤트 등록 전부해제
     */
    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
        UIKeyAction = null;
    }
}
