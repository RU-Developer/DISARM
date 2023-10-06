using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Game_Hp : UI_Base
{
    enum GameObjects
    {
        HpText,
        HpBarPanel
    }

    enum Texts
    {
        HpNumber
    }

    PlayerStatus _status;
    GameObject _panel;

    int _currentBarCount = 0;
    Stack<UI_Hp_Bar> _hpBars = new Stack<UI_Hp_Bar>();

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Bind<Text>(typeof(Texts));
        _status = Managers.Game.GetPlayer().GetOrAddComponent<PlayerStatus>();
        _panel = GetObject((int)GameObjects.HpBarPanel);

        Managers.Game.AddPlayerChangedAction(PlayerChangedAction);

        foreach (Transform child in _panel.transform)
            Managers.Resource.Destroy(child.gameObject);
    }

    private void PlayerChangedAction()
    {
        _status = Managers.Game.GetPlayer().GetOrAddComponent<PlayerStatus>();

        foreach (Transform child in _panel.transform)
            Managers.Resource.Destroy(child.gameObject);

        _hpBars.Clear();
        _currentBarCount = 0;
    }

    void Update()
    {
        int ratio = _status.Hp % 100;
        int barCount = _status.Hp / 100;

        if (ratio != 0)
            barCount++;

        int showHp = 0;

        if (_status.Hp == 0)
            showHp = 0;
        else if (ratio == 0)
        {
            ratio = 100;
            showHp = 100;
        }
        else
            showHp = ratio;

        SetHpRatio(ratio, barCount);
        GetText((int)Texts.HpNumber).text = $"{showHp}";
    }

    /**
     * HP 수치를 UI에 표기하는 기능입니다. 
     */ 
    public void SetHpRatio(int ratio, int barCount)
    {
        _currentBarCount = _hpBars.Count;
        // 기존과 hp bar 갯수가 달라졌으면 변경
        if (barCount != _currentBarCount)
        {
            int additional = barCount - _currentBarCount;
            if (additional > 0)
            {
                // 회복을 취했을 경우 hp가 늘어났으므로 다음 칸 만들기 전에 먼저 100만들기
                if (_hpBars.Count != 0)
                    _hpBars.Peek().SetValue(100);
                // 추가적으로 생성
                for (int i = 0; i < additional; i++)
                    _hpBars.Push(Managers.UI.MakeSubItem<UI_Hp_Bar>(_panel.transform));
            }
            else
                // 넘치는 양 제거
                for (int i = additional; i < 0; i++)
                    Managers.Resource.Destroy(_hpBars.Pop().gameObject);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_panel.transform as RectTransform);
            _currentBarCount = barCount;
        }

        //마지막 요소에 slider 조절
        if (_hpBars.Count != 0)
            _hpBars.Peek().SetValue(ratio);
    }

    public override void Clear()
    {
        Managers.Game.RemovePlayerChangedAction(PlayerChangedAction);
    }
}
