using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 여러가지 게임 내에서 정의할 데이터들입니다.
 */
public class Define
{
    public enum InputType
    {
        // 마우스나 조이스틱의 각도를 받는 타입 플레이어 기준 위부터 0 ~ 360. 조이스틱은 위부터 360도

        Left, Right, Up, Down, // UI 포커스 조작 타입: 키보드는 WASD, 조이스틱으로 각도 확인해서 조작
        Ok, // UI 확인 버튼 Enter Button3
        Cancel, // UI 취소 버튼 Escape Button2
        Menu, // 메뉴 열고 닫는 버튼 Escape Button5
        Attack, // 공격 버튼 Mouse Right Button4
        Jump, // 점프 버튼 Spacebar Button3
        Skill1, Skill2, // Skill1: Left Shift Button1, Skill2: Mouse Left Button2

        Length // Input Type 갯수
    }

    public enum InputDir
    {
        Left=-1, 
        Right=1, 
        Up=2, 
        Down=-2,
    }

    public enum Weapon
    {
        None,
        DartGun,
    }

    /**
     * GameManager에서 관리하는 모든 GameObject들의 종류입니다.
     * BaseController를 상속받으면 필드로 어떤 타입인지 구별할 수 있습니다.
     */
    public enum WorldObject
    {
        Unknown, //기본값

        Player,
        Monster,
    }

    /**
     * 게임의 Layer 정보를 기록해둔 enum입니다.
     */
    public enum Layer
    {
        Player = 3,
        Platform = 6,
        Confiner = 7,
        Spike = 8,
        Dart = 9,
    }

    /**
     * Scene 이름 정보입니다.
     */
    public enum Scene
    {
        Unknown, //기본값

        ResearchScene,
        ForestScene,
        WarpTestScene, //Warp Test
    }

    /**
     * 들려줄 소리의 종류입니다.
     * 각 소리의 종류마다 AudioSource가 하나씩 배정됩니다.
     */
    public enum Sound
    {
        Bgm, //배경음악. 계속 반복해서 들림
        Effect, //한번 들려주는 소리. 효과음

        MaxCount //enum 타입의 저장된 갯수를 파악하기 위한 마지막 요소
    }

    /**
     * UI를 마우스로 조작할 때 발생하는 이벤트들입니다.
     */
    public enum UIEvent
    {
        Click,
        Drag,
    }

    /**
     * UI가 아닌 인게임 마우스 조작입니다.
     */
    public enum MouseEvent
    {
        Press, //마우스를 누르고 있는 상태
        PointerDown, //마우스를 눌렀을 때 발생
        PointerUp, //마우스를 땠을 때 발생
        Click, //마우스를 빠르게(0.2초) 눌렀다 땠을 때 발생
    }
}
