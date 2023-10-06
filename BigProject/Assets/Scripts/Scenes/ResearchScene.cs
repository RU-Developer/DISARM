using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/**
 * 메인 Scene 입니다.
 */
public class ResearchScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.ResearchScene;

        Managers.Game.GamePlaySceneInitialize();

        Managers.Scene.CurrentScene.gameObject.FindChild<DialogEvent>("dialogTest")
            .SetScript("1");
        Managers.Sound.Play("researchRoom", Define.Sound.Bgm);
    }

    private void Update()
    {
        
    }

    public override void Clear()
    {
        
    }
}
