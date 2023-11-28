using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * 대화나 선택지가 등장하는 창입니다.
 * 대화를 선택지에 따라 다음 대화를 찾아서 출력합니다.
 */
public class UI_Dialog : UI_Scene
{
    enum GameObjects
    {
        SelectPanel
    }

    enum Texts
    {
        Comment
    }

    private Data.DialogScript _currentScript;

    private Text _comment;
    private VerticalLayoutGroup _panel;
    private int _select;
    private List<Text> _selections = new List<Text>();

    private bool _isDialogEnded = false;

    public override void Init()
    {
        if (_comment != null)
            return;

        base.Init();
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        _comment = GetText((int)Texts.Comment);
        _panel = GetObject((int)GameObjects.SelectPanel).GetOrAddComponent<VerticalLayoutGroup>();
        _select = 0;

        foreach (Transform child in _panel.transform)
            Managers.Resource.Destroy(child.gameObject);
    }

    private void Update()
    {
        if (_isDialogEnded)
            return;

        if (Managers.Input.GetInputDown(Define.InputType.Ok))
            ShowNextComment();

        Select();
    }

    /**
     * 처음 대화를 설정할 때 사용합니다.
     */
    public void InitComment(Data.DialogScript script)
    {
        Init();
        AddComment(script);
        ShowComment();
    }

    /**
     * 다음 대화 설정
     */
    private void AddComment(Data.DialogScript script)
    {
        if (script == null || script.script == null || string.IsNullOrEmpty(script.script.content))
        {
            _currentScript = null;
            return;
        }

        _currentScript = script;
    }

    /**
     * 선택지 조작
     */
    private void Select()
    {
        if (_selections == null || _selections.Count == 0 || _select < 0 || _select > _selections.Count || _selections[_select] == null)
            return;

        if (Managers.Input.GetInputDown(Define.InputType.Up))
        {
            _selections[_select].color = new Color(0, 0, 0);

            if (_select - 1 < 0)
                _select = _selections.Count - 1;
            else
                _select--;

            _selections[_select].color = new Color(0, 1, 0);
        }
        else if (Managers.Input.GetInputDown(Define.InputType.Down))
        {
            _selections[_select].color = new Color(0, 0, 0);

            if (_select + 1 >= _selections.Count)
                _select = 0;
            else
                _select++;

            _selections[_select].color = new Color(0, 1, 0);
        }
    }

    /**
     * 대화 및 선택지 출력 
     */
    private void ShowComment()
    {
        // 다음 스크립트 없으면 다이얼로그 종료
        if (_currentScript == null)
        {
            _isDialogEnded = true;
            Managers.UI.CloseSceneUI<UI_Dialog>();
            Managers.Pause.Play();
            return;
        }

        // 스크립트 설정
        if (string.IsNullOrEmpty(_currentScript.script.content))
            _currentScript.script.content = "";

        if (_currentScript.name == null)
            _comment.text = $"{_currentScript.script.content}";
        else
            _comment.text = $"{_currentScript.name}: {_currentScript.script.content}";

        // 선택지 설정
        if (_currentScript.selections != null && _currentScript.selections.Count != 0)
        {
            _selections.Clear();

            foreach (Data.Script select in _currentScript.selections)
            {
                Text selection = Managers.UI.MakeSubItem<UI_Dialog_Selection>(_panel.transform)
                    .gameObject.GetOrAddComponent<Text>();

                selection.text = select.content;
                _selections.Add(selection);
            }

            // 선택지 선택된 이펙트
            _select = 0;
            _selections[_select].color = new Color(0, 1, 0);
        }
    }

    /**
     * 선택지나 스크립트 데이터를 참조해서 다음 스크립트를 불러오고, 출력
     */
    private void ShowNextComment()
    {
        // 다음 스크립트 셋팅
        string next = null;

        if (_currentScript.selections == null || _currentScript.selections.Count == 0)
            next = _currentScript.script.link;
        else
            next = _currentScript.selections[_select].link;

        //TODO: 다이얼로그에서 마지막에 특정 함수를 호출하는 경우 지정. link의 시작에 //가 들어갈 경우 그 뒤는 함수명
        if (string.IsNullOrEmpty(next) == false && next.StartsWith("//"))
        {
            _isDialogEnded = true;
            Managers.UI.CloseSceneUI<UI_Dialog>();
            Managers.Pause.Play();
            DialogFunctions.Invoke($"{next.Substring(2)}");
            return;
        }

        if (string.IsNullOrEmpty(next))
        {
            _isDialogEnded = true;
            AddComment(null);
            ShowComment();
            return;
        }

        AddComment(Managers.Data.DialogScriptDict[next]);

        // 선택지 제거
        foreach (Transform child in _panel.transform)
            Managers.Resource.Destroy(child.gameObject);

        ShowComment();
    }

    /**
     * 화면 전환 시 또는 대화가 끝났을 시 등록된 이벤트 삭제
     */
    public override void Clear()
    {
        base.Clear();
    }
}
