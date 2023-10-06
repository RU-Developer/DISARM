using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToastMessageEvent : MonoBehaviour
{
    public string message;

    public bool isOn = false;

    private float _eventDistance = 2.0f;

    private void Update()
    {
        if (isOn)
            return;

        GameObject player = Managers.Game.GetPlayer();

        float distance = Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.y),
                new Vector2(transform.position.x, transform.position.y));

        if (distance > _eventDistance)
            return;

        isOn = true;
        Managers.UI.ShowPopupUI<UI_Toast_Message>().SetInfo(message, transform.position, this);
    }
}
