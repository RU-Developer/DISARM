using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Cool_Time_Slider : UI_Base
{
    private Slider _slider;
    private string _skillName;

    public override void Init()
    {
        if (string.IsNullOrEmpty(_skillName))
            return;

        _slider = GetComponent<Slider>();
    }

    public void SetSkill(string skillName)
    {
        _skillName = skillName;
    }

    private void Update()
    {
        if (_slider == null)
        {
            Init();
            return;
        }

        float maxCoolTime = Managers.Data.CoolTimeDict[_skillName].coolTime;
        float remain = Managers.Skill.RemainCoolTime(_skillName);

        _slider.value = remain / maxCoolTime;
    }
}
