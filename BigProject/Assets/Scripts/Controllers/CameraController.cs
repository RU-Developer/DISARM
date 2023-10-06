using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    CinemachineVirtualCamera _camera;

    void Start()
    {
        _camera = gameObject.GetComponent<CinemachineVirtualCamera>();
        GameObject player = Managers.Game.GetPlayer();
        if (player.IsValid())
        {
            _camera.Follow = player.transform;
            _camera.LookAt = player.transform;
        }

        Managers.Game.AddPlayerChangedAction(() =>
        {
            gameObject.GetComponent<CinemachineVirtualCamera>().Follow = Managers.Game.GetPlayer().transform;
        });
    }
}
