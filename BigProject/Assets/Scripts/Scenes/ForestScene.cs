using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.ForestScene;

        Managers.Game.GamePlaySceneInitialize();
        Managers.Sound.Play("forestRoom", Define.Sound.Bgm);
    }

    void Update()
    {
        
    }

    public override void Clear()
    {
        
    }
}
