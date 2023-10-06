using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/**
 * Section 사이를 Player가 지나갈 때 시네머신 카메라 전환 작업
 */ 
public class CameraSection : MonoBehaviour
{
    public GameObject virtualCam;

    private List<string> _paths;
    private PolygonCollider2D coll;

    private void Start()
    {
        coll = GetComponent<PolygonCollider2D>();
        _paths = new List<string>();
        List<string> pathIds = Managers.Data.MapDict[Managers.Scene.GetCurrentSceneName()].pathIds;
        string roomId = $"{Managers.Scene.GetCurrentSceneName()}_{name}";
        foreach (string pathId in pathIds)
        {
            Data.PathData currentPath = Managers.Data.PathDict[pathId];
            if (currentPath.roomId1.Equals(roomId))
                _paths.Add(pathId);
            else if (currentPath.roomId2.Equals(roomId))
                _paths.Add(pathId);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        UpdateCamera(collision);
        if (collision.CompareTag("Player") == false)
            return;

        Managers.Game.MonsterActive(name);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        UpdateCamera(collision);
        UpdateLock(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") == false)
            return;
        
        Managers.Game.MonsterRespawn(name);
    }

    private void UpdateLock(Collider2D collision)
    {
        if (collision.CompareTag("Player") == false)
            return;

        foreach (string pathId in _paths)
        {
            switch (Managers.Data.PathDict[pathId].type)
            {
                case "open":
                    coll.isTrigger = true;
                    break;
                case "templock":
                    coll.isTrigger = false;
                    break;
                case "lock":
                    coll.isTrigger = false;
                    break;
            }
        }
    }


    private void UpdateCamera(Collider2D collision)
    {
        if (collision.CompareTag("Player") == false)
            return;

        if (virtualCam.IsValid() == false || Managers.Camera.IsLiveCamera(virtualCam) || Managers.Camera.ManualMode)
            return;

        // 방을 방문한 것으로 업데이트. 이를 통해 해당 방의 정보 접근 가능
        Data.RoomData data = Managers.Data.RoomDict.GetValueOrDefault($"{Managers.Scene.GetCurrentSceneName()}_{name}", null);
        if (data != null)
            data.visited = true;
        // 카메라 전환
        Managers.Camera.ChangeCamera(virtualCam);
    }
}
