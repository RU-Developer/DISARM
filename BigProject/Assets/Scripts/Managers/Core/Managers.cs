using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 모든 메니저들을 관리하는 클래스
 */
public class Managers : MonoBehaviour
{
    static Managers s_instance;
    public static Managers Instance { get { Init(); return s_instance; } }

    #region Contents

    CameraManager _camera = new CameraManager();
    GameManager _game = new GameManager();
    ItemManager _item = new ItemManager();
    SaveManager _save = new SaveManager();
    SkillManager _skill = new SkillManager();
    WarpManager _warp = new WarpManager();

    public static CameraManager Camera { get { return Instance._camera; } }
    public static GameManager Game { get { return Instance._game; } }
    public static ItemManager Item { get { return Instance._item; } }
    public static SaveManager Save { get { return Instance._save; } }
    public static WarpManager Warp { get { return Instance._warp; } }
    public static SkillManager Skill { get { return Instance._skill; } }

    #endregion
    #region Core

    DataManager _data = new DataManager();
    InputManager _input = new InputManager();
    NetworkManager _network;
    PauseManager _pause = new PauseManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    SceneManagerEx _scene = new SceneManagerEx();
    SoundManager _sound = new SoundManager();
    UIManager _ui = new UIManager();

    public static DataManager Data { get { return Instance._data; } }
    public static InputManager Input { get { return Instance._input; } }
    public static NetworkManager Network { get { return Instance._network; } }
    public static PauseManager Pause { get { return Instance._pause; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static SceneManagerEx Scene { get { return Instance._scene; } }
    public static SoundManager Sound { get { return Instance._sound; } }
    public static UIManager UI { get { return Instance._ui; } }

    #endregion

    void Start()
    {
        Init();
    }

    void Update()
    {
        _input.OnUpdate();
    }

    public static void Init()
    {
        if (s_instance != null)
            return;

        GameObject managers = GameObject.Find("@Managers");

        if (managers == null)
        {
            managers = new GameObject { name = "@Managers" };
            managers.AddComponent<Managers>();
            managers.AddComponent<NetworkManager>();
        }
        else
        {
            managers.GetOrAddComponent<Managers>();
            managers.GetOrAddComponent<NetworkManager>();
        }

        DontDestroyOnLoad(managers);
        s_instance = managers.GetComponent<Managers>();
        s_instance._network = managers.GetComponent<NetworkManager>();
        s_instance._data.Init();
        s_instance._sound.Init();
        s_instance._pool.Init();

        s_instance._skill.Init();
        s_instance._game.Init();
    }

    /**
     * 화면 전환된 후 실행
     */
    public static void Load()
    {
        s_instance._game.Init();
    }

    /**
     * 화면 전환되기 전 실행
     */
    public static void Clear()
    {
        Game.Clear();

        Camera.Clear();
        Input.Clear();
        Pause.Clear();
        Sound.Clear();
        UI.Clear();
        Scene.Clear();
        Pool.Clear();
    }
}
