using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UI_Game_Money : UI_Base
{
    private Text _money;

    enum Texts
    {
        MoneyNumber
    }

    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        _money = GetText((int)Texts.MoneyNumber);
    }

    private void Update()
    {
        if (_money != null)
            _money.text = $"{Managers.Data.PlayerStatusDict["saved"].money}bit";
    }
}
