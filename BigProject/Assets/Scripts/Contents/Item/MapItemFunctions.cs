using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/**
 * MapItem의 id(prefab이름과도 동일하게)에 맞춰서 함수명을 지으면 자동으로 아이템 획득시 호출됩니다.
 */
public class MapItemFunctions
{
    public static void Invoke(string func)
    {
        typeof(MapItemFunctions)
            .GetMethod(func, BindingFlags.Static | BindingFlags.NonPublic)
            .Invoke(null, null);
    }

    #region Upgrade

    private static void IncreaseHp()
    {
        PlayerStatus status = Managers.Game.GetPlayer().GetComponent<PlayerStatus>();
        status.UpdateMaxHp(100);
        status.UpdateHp(100);
    }

    #endregion

    #region Weapons

    private static void DartGun()
    {
        Managers.Data.WeaponDict["DartGun"].own = true;
        Managers.Game.GetPlayer()?.GetComponent<PlayerStatus>().ChangeWeapon("DartGun");
    }

    #endregion

    #region FirmWare

    private static void Dive()
    {
    }

    private static void Parry()
    {
    }

    private static void GrabLedge()
    {
    }

    private static void WallJump()
    {
    }

    private static void Twins()
    {
    }

    #endregion
}
