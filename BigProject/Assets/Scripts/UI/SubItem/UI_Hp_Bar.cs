using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Hp_Bar : UI_Base
{
    Image _slider;

    public override void Init()
    {
        // 자식 GameObject에서 bar 찾아서 가져오기
        _slider = gameObject.FindChild<Image>("bar");
    }

    /**
     * 1 ~ 100 사이의 값을 받아 HP바 슬라이더를 조절합니다.
     */
    public void SetValue(int ratio)
    {
        Init();
        _slider.rectTransform.localScale = new Vector3(ratio / 100.0f, 1, 1);
    }
}
