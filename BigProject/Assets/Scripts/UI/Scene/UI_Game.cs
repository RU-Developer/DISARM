using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * 게임 진행 중에 보이는 정보들
 */
public class UI_Game : UI_Scene
{
    enum Images
    {
        WeaponIcon
    }

    enum GameObjects
    {
        UI_Game_Hp,
        DiveSkillCoolTimer,
        ParrySkillCoolTimer
    }

    public override void Init()
    {
        base.Init();
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));

        Image weapon = GetImage((int)Images.WeaponIcon);
        switch (Managers.Data.PlayerStatusDict["saved"].weapon)
        {
            case "None":
                break;

            case "DartGun":
                weapon.sprite = Managers.Resource.Load<Sprite>(Managers.Data.IconDict["DartGun"].path);
                break;
        }

        // dive parry 쿨타임 설정
        GetObject((int)GameObjects.DiveSkillCoolTimer).GetComponent<UI_Cool_Time_Slider>().SetSkill("dive");
        GetObject((int)GameObjects.ParrySkillCoolTimer).GetComponent<UI_Cool_Time_Slider>().SetSkill("parry");
    }

    private void Update()
    {
        if (Managers.Input.GetInputDown(Define.InputType.Menu) == false)
            return;

        if (Managers.Pause.IsPause)
            Managers.Pause.Play();
        else
        {
            Managers.Pause.Pause();
            Managers.UI.CloseSceneUI<UI_Game>();
            Managers.UI.ShowSceneUI<UI_Game_Menu>();
        }
    }

    public override void Clear()
    {
        base.Clear();
        GetObject((int)GameObjects.UI_Game_Hp)?
            .GetComponent<UI_Game_Hp>()?.Clear();
    }
}
