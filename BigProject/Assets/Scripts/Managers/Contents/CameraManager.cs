using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/**
 * Cinemachine Virtual Camera들을 관리하는 클래스입니다
 */
public class CameraManager
{
    List<GameObject> _cameras = new List<GameObject>();
    GameObject _liveCamera = null;

    public List<CameraSection> Sections { get; private set; } = new List<CameraSection>();

    public GameObject LiveCamera { get { return _liveCamera; } }

    public bool ManualMode { get; private set; } = false;
    private GameObject liveCameraBeforeManual;

    public bool IsLiveCamera(GameObject camera)
    {
        return _liveCamera == camera;
    }

    /**
     * Cinemachine Virtual Camera들이 위치할 parent 객체
     */
    public Transform Root
    {
        get
        {
            GameObject root = GameObject.Find("@Camera_Root");

            if (root == null)
                root = new GameObject { name = "@Camera_Root" };

            return root.transform;
        }
    }

    public void InitialCameraSet(List<CameraSection> sections)
    {
        foreach (CameraSection section in sections)
            Managers.Camera.MakeCamera(section);
    }

    /**
     * Section(프리팹)을 지정해서 Camera 생성 가능
     */
    public GameObject MakeCamera(CameraSection section)
    {
        if (section == null)
            return null;

        Sections.Add(section);

        GameObject camera = Managers.Resource.Instantiate("Camera/VirtualCamera");
        camera.transform.parent = Root;

        //section 셋팅
        section.virtualCam = camera;
        //camera 셋팅
        camera.GetComponent<CinemachineConfiner>().m_BoundingShape2D = section.GetComponent<PolygonCollider2D>();
        camera.transform.position = section.transform.position;
        camera.name = section.name;

        _cameras.Add(camera);

        return camera;
    }

    /**
     * 화면을 해당 카메라 화면으로 전환
     */ 
    public void ChangeCamera(GameObject changeTo)
    {
        if (_liveCamera == changeTo)
            return;

        //우선순위 조절 방식
        changeTo.GetComponent<CinemachineVirtualCamera>()
            .MoveToTopOfPrioritySubqueue();

        _liveCamera = changeTo;
    }

    /**
     * 이벤트 등으로 카메라를 수동으로 조작하는 경우
     */
    public void ManualModeStart()
    {
        ManualMode = true;
        liveCameraBeforeManual = LiveCamera;
    }

    /**
     * 수동 조작 해제
     */
    public void ManualModeEnd()
    {
        ChangeCamera(liveCameraBeforeManual);
        liveCameraBeforeManual = null;
    }

    /**
     * 장면전환시 카메라 리소스 제거
     */ 
    public void Clear()
    {
        foreach (CameraSection section in Sections)
            Managers.Resource.Destroy(section.gameObject);
        Sections.Clear();

        foreach (GameObject camera in _cameras)
            Managers.Resource.Destroy(camera);
        _cameras.Clear();
    }
}
