using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;


/**
 * 게임 내부 요소들에 대해 전체적으로 관리하는 매니저
 */
public class GameManager
{
    GameObject _player;
    bool _IsEnter = false;
    Vector3 _playerSpawnPos;

    public class MonsterInit
    {
        public string monsterName;
        public Vector3 startPos;
        public GameObject go;
    }

    Dictionary<string, Dictionary<string, HashSet<MonsterInit>>> _monsterDict;
    public List<GameObject> paths;

    public Vector3 MapStartPosition { get; private set; }
    public Vector3 MapSize { get; private set; }

    public void SetPlayerSpawnPos(Vector3 pos)
    {
        _IsEnter = true;
        _playerSpawnPos = pos;
    }

    Action PlayerChangedAction = null;

    public bool TEST { get; } = true; // 현재 화면에서 데이터를 읽어서 json파일에 반영시켜주기 : true

    public void Init()
    {
        if (paths != null)
            return;

        paths = new List<GameObject>();
        _monsterDict = new Dictionary<string, Dictionary<string, HashSet<MonsterInit>>>();

        string scene = Managers.Scene.GetCurrentSceneName();

        List<string> roomIds = Managers.Data.MapDict[scene].roomIds;
        foreach (string roomId in roomIds)
        {
            Debug.Log($"dictionary roomId: {roomId}");
            _monsterDict[roomId] = new Dictionary<string, HashSet<MonsterInit>>();
            foreach (string id in Managers.Data.MonsterStatusDict.Keys)
                _monsterDict[roomId][id] = new HashSet<MonsterInit>();
        }
    }

    public Vector3 DefaultPosition { get; set; }

    /**
     * Game을 플레이 하기 위한 Scene에서 자동적으로 작동하는 초기화 작업 
     */
    public void GamePlaySceneInitialize()
    {
        Init();
        Managers.Pause.Pause();
        string scene = Managers.Scene.GetCurrentSceneName();

        // 플레이어 셋팅
        GameObject player = Managers.Scene.CurrentScene.gameObject.FindChild<PlayerController>().gameObject;
        DefaultPosition = player.transform.position;

        Despawn(player);
        // 리스폰과 스폰, 워프 분리
        if (Managers.Warp.HasWarpPoint())
        {
            Warped();
        }
        else if (_IsEnter)
        {
            EnterSceneSpawn();
        }
        else // 저장된 위치로 이동
        {
            // 개발중일 때 디버깅일시 저장된 위치에서 부활하거나 시작하지 않고, 화면에 넣어 놓은 위치에서 시작
            if (!TEST)
                Respawn();
            else
            {
                SetPlayerSpawnPos(DefaultPosition);
                EnterSceneSpawn();
            }
        }

        // 몬스터 셋팅
        List<MonsterController> monsters =
            Managers.Scene.CurrentScene.gameObject.FindAllChild<MonsterController>(null, true);

        
        // 밑의 시네머신 Section쪽에서 한번에 진행할 예정


        // 시네머신 셋팅 및 미니맵 셋팅
        // 타일맵에서 읽어와서 section, path를 생성
        GameObject tilemap = GameObject.Find("Grid");
        Grid grid = tilemap.GetComponent<Grid>();

        // 타일맵 색션 이름을 Section이라고 시작만 하면 됨
        List<Tilemap> tilemapSections = new List<Tilemap>();
        // 타일맵 통로 이름을 Path로 시작하면 됨
        List<Tilemap> tilemapPaths = new List<Tilemap>();
        foreach (Tilemap map in tilemap.FindAllChild<Tilemap>())
        {
            if (map.name.StartsWith("Section"))
            {
                tilemapSections.Add(map);
                continue;
            }

            if (map.name.StartsWith("Path"))
                tilemapPaths.Add(map);
        }

        Vector3 cellSize = grid.cellSize;

        // 타일맵 사이즈만큼 Section생성
        foreach (Tilemap section in tilemapSections)
        {
            BoundsInt bounds = section.cellBounds;
            bool first = true;
            Vector3Int firstCell = new Vector3Int();
            Vector3Int lastCell = new Vector3Int();

            foreach (Vector3Int cell in bounds.allPositionsWithin)
            {
                // 스프라이트가 그려진 것을 기준으로 넓이 구하기
                if (section.HasTile(cell))
                {
                    if (first)
                    {
                        // 첫번째 셀 위치 구하기
                        firstCell = cell;
                        first = false;
                    }
                    // 마지막 셀 위치 구하기
                    lastCell = cell;
                }
            }

            Vector3 firstPos = section.CellToWorld(firstCell);
            Vector3 lastPos = section.CellToWorld(lastCell) + cellSize;

            float width = lastPos.x - firstPos.x;
            float height = lastPos.y - firstPos.y;
            Vector3 actualSize = new Vector3(width, height, 0);


            // 몬스터 셋팅
            foreach (MonsterController monster in monsters)
            {
                // 이 Section의 범위에 속하면
                Vector3 pos = monster.transform.position;
                if (pos.x < firstPos.x || pos.y < firstPos.y || pos.x > lastPos.x || pos.y > lastPos.y)
                    continue;

                MonsterInit init = new MonsterInit();
                init.monsterName = monster.name;
                init.startPos = pos;
                init.go = monster.gameObject;
                _monsterDict[$"{scene}_{section.name}"][monster.name].Add(init);
            }


            GameObject cameraSection = Managers.Resource.Instantiate("Camera/Section");
            cameraSection.transform.position = firstPos + (actualSize / 2);
            cameraSection.transform.localScale = actualSize;

            cameraSection.transform.parent = Managers.Scene.CurrentScene.transform;

            cameraSection.name = section.name;
            Managers.Resource.Destroy(section.gameObject);

            Managers.Camera.MakeCamera(cameraSection.GetComponent<CameraSection>());
        }

        // 타일맵 사이즈만큼 Path 생성
        foreach (Tilemap path in tilemapPaths)
        {
            BoundsInt bounds = path.cellBounds;
            bool first = true;
            Vector3Int firstCell = new Vector3Int();
            Vector3Int lastCell = new Vector3Int();

            foreach (Vector3Int cell in bounds.allPositionsWithin)
            {
                // 스프라이트가 그려진 것을 기준으로 넓이 구하기
                if (path.HasTile(cell))
                {
                    if (first)
                    {
                        // 첫번째 셀 위치 구하기
                        firstCell = cell;
                        first = false;
                    }
                    // 마지막 셀 위치 구하기
                    lastCell = cell;
                }
            }

            Vector3 firstPos = path.CellToWorld(firstCell);
            Vector3 lastPos = path.CellToWorld(lastCell) + cellSize;

            float width = lastPos.x - firstPos.x;
            float height = lastPos.y - firstPos.y;
            Vector3 actualSize = new Vector3(width, height, 0);

            GameObject mapPath = Managers.Resource.Instantiate("Map/Path");
            mapPath.transform.position = firstPos + (actualSize / 2);
            mapPath.transform.localScale = actualSize;

            mapPath.transform.parent = Managers.Scene.CurrentScene.transform;

            mapPath.name = path.name;
            Managers.Resource.Destroy(path.gameObject);
            paths.Add(mapPath);
        }

        // UI 셋팅
        Managers.UI.ShowSceneUI<UI_Game>();

        Managers.Pause.Play();
    }

    /**
     * 플레이어 위치 변경시나 참조가 변경되었을 경우 전파
     */
    public void AddPlayerChangedAction(Action playerChangedHandler)
    {
        PlayerChangedAction -= playerChangedHandler;
        PlayerChangedAction += playerChangedHandler;
    }

    public void RemovePlayerChangedAction(Action playerChangedHandler)
    {
        PlayerChangedAction -= playerChangedHandler;
    }

    /**
     * 현재 게임 Scene에 있는 Player GameObject 반환
     */
    public GameObject GetPlayer()
    {
        return _player;
    }

    /**
     * 워프를 타고 왔을 때
     */
    public GameObject Warped(string path = "Player", Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(path, parent);
        _player = go;

        if (Managers.Warp.WarpTargetPoint != null)
            _player.transform.position = Managers.Warp.WarpTargetPoint;
        
        Managers.Warp.SetWarpPoint(null);

        // 플레이어 변경 전파
        if (PlayerChangedAction != null)
            PlayerChangedAction.Invoke();

        return go;
    }

    /**
     * 플레이어가 죽고 부활했을 때
     */
    public GameObject Respawn(string path = "Player", Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(path, parent);

        go.GetComponent<BaseController>().OnSpawn();
        _player = go;

        _player.GetComponent<PlayerStatus>().MoveToSavedPosition();

        if (PlayerChangedAction != null)
            PlayerChangedAction.Invoke();

        return go;
    }

    /**
     * Monster, Player 등 BaseController 상속하는 GameObject들 생성 및 관리
     */
    public GameObject EnterSceneSpawn(string path = "Player", Transform parent = null)
    {
        GameObject go = Managers.Resource.Instantiate(path, parent);

        Managers.Resource.Destroy(_player);
        _player = go;
                
        _player.transform.position = _playerSpawnPos;
        _IsEnter = false;
        _player.GetComponent<PlayerStatus>().SaveStatus();
                

        if (PlayerChangedAction != null)
            PlayerChangedAction.Invoke();

        return go;
    }

    /**
     * 해당 섹션의 모든 몬스터를 Respawn 시킵니다.
     * 생성하고 일단은 비활성화 시킵니다.
     */
    public void MonsterRespawn(string section)
    {
        string scene = Managers.Scene.GetCurrentSceneName();

        foreach (HashSet<MonsterInit> monsters in _monsterDict[$"{scene}_{section}"].Values)
        {
            foreach (MonsterInit monster in monsters)
            {
                Despawn(monster.go);
                GameObject mon = Managers.Resource.Instantiate(monster.monsterName);
                mon.transform.position = monster.startPos;
                monster.go = mon;
                mon.SetActive(false);
            }
        }
    }

    /**
     * 해당 섹션의 모든 몬스터를 활성화 시킵니다.
     */
    public void MonsterActive(string section)
    {
        string scene = Managers.Scene.GetCurrentSceneName();
        foreach (HashSet<MonsterInit> monsters in _monsterDict[$"{scene}_{section}"].Values)
            foreach (MonsterInit monster in monsters)
                monster.go.SetActive(true);
    }

    /**
     * 생성된 GameObject 제거 및 관리
     */
    public void Despawn(GameObject go)
    {
        if (go == null)
            return;

        Define.WorldObject type = GetWorldObjectType(go);

        if (type != Define.WorldObject.Unknown)
            go.GetComponent<BaseController>().OnDeSpawn();

        switch (type)
        {
            case Define.WorldObject.Player:                
                if (_player == go)
                    _player = null;
                break;
        }
        Managers.Resource.Destroy(go);
    }

    /**
     * BaseController에 기반한 WorldObject 타입 반환
     */
    public Define.WorldObject GetWorldObjectType(GameObject go)
    {
        BaseController bc = go.GetComponent<BaseController>();
        if (bc == null)
            return Define.WorldObject.Unknown;

        return bc.WorldObjectType;
    }

    /**
     * 장면 전환시 리소스 정리
     */
    public void Clear()
    {
        PlayerChangedAction = null;

        // 장면 전환시 플레이어 현재 상태. 스텟 저장
        if (_player != null)
        {
            _player.GetComponent<PlayerStatus>().SaveStatus();
            Managers.Resource.Destroy(_player);
            _player = null;
        }

        // 몬스터 초기화 정보 제거
        foreach (Dictionary<string, HashSet<MonsterInit>> monsters in _monsterDict.Values)
        {
            foreach (HashSet<MonsterInit> mons in monsters.Values)
            {
                foreach (MonsterInit mon in mons)
                    Managers.Resource.Destroy(mon.go);
                mons.Clear();
            }
            monsters.Clear();
        }
        _monsterDict.Clear();
        _monsterDict = null;

        paths.Clear();
        paths = null;
    }
}
