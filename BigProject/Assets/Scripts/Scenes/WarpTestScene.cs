using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/**
 * 메인 Scene 입니다.
 */
public class WarpTestScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.WarpTestScene;

        Managers.Game.GamePlaySceneInitialize();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Managers.Scene.LoadScene(Define.Scene.ResearchScene);
    }

    public override void Clear()
    {
        
    }
}
