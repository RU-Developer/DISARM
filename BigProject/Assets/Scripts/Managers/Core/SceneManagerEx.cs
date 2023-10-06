using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Scene 전환시 관리해줄 매니저 클래스
 */
public class SceneManagerEx
{
    /**
     * 현재 Scene에 대한 BaseScene 
     */
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    /**
     * Scene 전환
     */
    public void LoadScene(Define.Scene type)
    {
        LoadScene(GetSceneName(type));
    }

    /**
     * 문자열 이름으로 Scene 전환
     */
    public void LoadScene(string scene)
    {
        Managers.Clear();
        Debug.Log($"{CurrentScene} -> {scene}");
        SceneManager.LoadScene(scene);
    }

    /**
     * Enum으로 정의한 Scene들을 문자열로 변환
     */
    private string GetSceneName(Define.Scene type)
    {
        return System.Enum.GetName(typeof(Define.Scene), type);
    }

    /**
     * 현재 Scene의 이름을 문자열로 반환
     */
    public string GetCurrentSceneName()
    {
        return GetSceneName(CurrentScene.SceneType);
    }

    /**
     * 다른 화면으로 넘어가기 전에 리소스 해제
     */
    public void Clear()
    {
        CurrentScene.Clear();
    }
}
