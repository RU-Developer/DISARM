using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/**
 * Dialog를 통해 전달될 함수들을 정의해 놓은 곳입니다.
 * 이곳에 private static void 함수명()  이런식으로 함수를 선언하면
 * Dialog.json의 link에 //함수명 이라고 적은 부분을 통해 호출됩니다.
 */
public class DialogFunctions
{
    public static void Invoke(string func)
    {
        typeof(DialogFunctions)
            .GetMethod(func, BindingFlags.Static | BindingFlags.NonPublic)
            .Invoke(null, null);
    }

    /**
     * 상점창 열기
     */
    private static void OpenShop()
    {
        Managers.Pause.Pause();
        Managers.UI.CloseSceneUI<UI_Game>();
        Managers.UI.ShowSceneUI<UI_Shop>();
    }

    /**
     * 현재 워프 포인트 워프 기동
     */
    private static void Warp()
    {
        Managers.Warp.Warp();
    }

    /**
     * 워프 포인트 취소
     */
    private static void WarpCancel()
    {
        Managers.Warp.SetWarpPoint(null);
    }

    /**
     * 세이브 포인트 위치에서 저장
     */
    private static void Save()
    {
        Managers.Save.Save();
    }
}
