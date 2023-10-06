using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 워프를 이용할 때 워프 매니저 이용
 */
public class WarpManager
{
    private Data.WarpPoint _temp;
    private Data.WarpPoint _warpPoint;

    public Vector3 WarpTargetPoint { get; private set; }

    /**
     * 다른 장면으로 워프하게 될 경우 GameManager가 워프되어 왔는지 확인하기 위한 함수
     */
    public bool HasWarpPoint()
    {
        return _warpPoint != null;
    }

    /**
     * 워프 포인트를 지정하기 위함. 워프 되고 나면 null을 넣어줘서 워프되지않게 한다.
     */
    public void SetWarpPoint(WarpPoint warp)
    {
        if (warp == null)
        {
            _temp = null;
            _warpPoint = null;
        }
        else
            _temp = warp.Point;
    }

    /**
     * 워프 포인트로 워프 진행. 다른 장면간에 워프는 GameManager를 통해 진행
     */
    public void Warp()
    {
        if (_temp == null || !_temp.scene.Equals(Managers.Scene.GetCurrentSceneName()))
            return;

        _warpPoint = _temp;

        Data.WarpPoint targetPoint = Managers.Data.WarpPointDict[_warpPoint.link];

        Debug.Log("[" + _warpPoint.scene + "] " + _warpPoint.id + "에서 [" +
             targetPoint.scene + "] " + targetPoint.id + "로 워프했습니다.");

        WarpTargetPoint = new Vector3(targetPoint.x, targetPoint.y);

        // 워프 대상 포인트가 현재 scene과 같다면 그냥 이동
        if (_warpPoint.scene.Equals(targetPoint.scene))
        {
            GameObject player = Managers.Game.GetPlayer();
            if (player != null)
                player.transform.position = WarpTargetPoint;
            _warpPoint = null;
            return;
        }
        // 다르면 장면전환까지 진행
        else
        {
            Managers.Scene.LoadScene(targetPoint.scene);
            return;
            // GameManager에서 Spawn할 때 WarpPoint 존재하면 그쪽으로 보내고 WarpPoint를 비워줌
        }
    }
}
