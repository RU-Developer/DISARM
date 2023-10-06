using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 게임 플레이 일시정지를 관리하는 클래스
 */ 
public class PauseManager
{
    private bool _pause;
    private Action<bool> PauseEvent = null;

    public bool IsPause { get { return _pause; } }

    public void Pause()
    {
        Time.timeScale = 0f;
        _pause = true;
    }

    public void Play()
    {
        Time.timeScale = 1f;
        _pause = false;
    }

    public void AddPauseEvent(Action<bool> pauseHandler)
    {
        PauseEvent -= pauseHandler;
        PauseEvent += pauseHandler;
    }

    public void RemovePauseEvent(Action<bool> pauseHandler)
    {
        PauseEvent -= pauseHandler;
    }

    public void Clear()
    {
        PauseEvent = null;
    }
}
