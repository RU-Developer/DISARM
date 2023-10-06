using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager
{
    public void Save()
    {
        GameObject player = Managers.Game.GetPlayer();
        if (player != null)
        {
            PlayerStatus status = player.GetComponent<PlayerStatus>();
            status.FullRecover();
            status.SavePosition();
            status.SaveStatus();
        }

        Managers.Data.SaveJson(false);
    }
}
