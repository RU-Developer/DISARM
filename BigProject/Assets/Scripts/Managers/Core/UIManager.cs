using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * UI를 관리하는 클래스입니다.
 */
public class UIManager
{
    int _order = 10; //1 ~ 9번은 기본 UI가 필요할 때를 대비해 예약. UI 순서

    HashSet<UI_Popup> _popupUIs = new HashSet<UI_Popup>();
    Dictionary<string, UI_Scene> _sceneUIs = new Dictionary<string, UI_Scene>();

    //UI GameObject들을 정리해서 넣어놓을 최상위 객체
    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");

            if (root == null)
                root = new GameObject { name = "@UI_Root" };

            return root;
        }
    }

    /**
     * UI가 그려질 Canvas를 설정하는 함수
     */
    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go);
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.overrideSorting = true;
        CanvasScaler scaler = Util.GetOrAddComponent<CanvasScaler>(go);
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        if (sort)
            canvas.sortingOrder = _order++;
        else
            canvas.sortingOrder = 0;
    }

    /**
     * Scene UI를 생성
     */
    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        UI_Scene sceneUI = null;
        _sceneUIs.TryGetValue(name, out sceneUI);

        if (sceneUI != null)
            Managers.Resource.Destroy(sceneUI.gameObject);

        GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
        sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUIs.Add(name, sceneUI);

        go.transform.SetParent(Root.transform);

        return sceneUI as T;
    }

    /**
     * SubItem UI를 생성
     */
    public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{name}");

        if (parent != null)
            go.transform.SetParent(parent);

        go.transform.localScale = Vector3.one;

        return Util.GetOrAddComponent<T>(go);
    }

    /**
     * Popup UI를 생성
     */
    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
        T popup = Util.GetOrAddComponent<T>(go);
        _popupUIs.Add(popup);

        go.transform.SetParent(Root.transform);

        return popup;
    }

    /**
     * Popup UI를 확인하며 닫는 함수
     */
    public void ClosePopupUI(UI_Popup popup)
    {
        if (_popupUIs.Count == 0)
            return;

        _popupUIs.Remove(popup);

        popup.Clear();
        if (popup.gameObject != null)
            Managers.Resource.Destroy(popup.gameObject);
        popup = null;

        _order--;
    }

    /**
     * 모든 Popup UI를 닫는 함수
     */
    public void CloseAllPopupUI()
    {
        foreach (UI_Popup popup in _popupUIs)
            Managers.Resource.Destroy(popup.gameObject);

        _popupUIs.Clear();
    }

    /**
     * Scene UI를 닫는 함수
     */
    public void CloseSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (_sceneUIs.Count == 0)
            return;

        if (string.IsNullOrEmpty(name))
            name = typeof(T).Name;

        UI_Scene scene = null;
        _sceneUIs.TryGetValue(name, out scene);

        if (scene != null)    
            _sceneUIs.Remove(name);

        scene.Clear();
        Managers.Resource.Destroy(scene.gameObject);
        scene = null;
    }

    public void CloseSceneUI(string name)
    {
        CloseSceneUI<UI_Scene>(name);
    }

    public void CloseSceneUI(UI_Scene scene)
    {
        if (_sceneUIs.Count == 0 || scene == null)
            return;

        if (_sceneUIs.ContainsKey(scene.name))
            _sceneUIs.Remove(scene.name);

        Managers.Resource.Destroy(scene.gameObject);
    }

    /**
     * 모든 Scene UI를 닫는 함수
     */
    public void CloseAllSceneUI()
    {
        foreach (UI_Scene scene in _sceneUIs.Values)
        {
            scene.Clear();
            Managers.Resource.Destroy(scene.gameObject);
        }
        _sceneUIs.Clear();
    }

    /**
     * 장면 전환 등에 사용될 리소스 초기화
     */
    public void Clear()
    {
        CloseAllPopupUI();
        CloseAllSceneUI();
    }
}
