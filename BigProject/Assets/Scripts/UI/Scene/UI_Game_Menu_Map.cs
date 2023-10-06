using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Game_Menu_Map : UI_Scene
{
    enum GameObjects
    {
        MenuMenuHandler,
        GameMap
    }

    enum Images
    {
        LockIcon,
        TemporaryLockIcon,
        OpenIcon,
        BossIcon,
        LatchIcon,
        SaveIcon,
        TeleportIcon,
        FirmWareIcon,
        UpgradeHpIcon,
        WeaponIcon
    }

    private void QuitMenu()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        Managers.UI.CloseSceneUI<UI_Game_Menu_Map>();
        Managers.UI.ShowSceneUI<UI_Game>();
        Managers.Pause.Play();
    }

    public override void Init()
    {
        base.Init();
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));
        BindEvent(GetObject((int)GameObjects.MenuMenuHandler),
            evt =>
            {
                Managers.UI.CloseSceneUI<UI_Game_Menu_Map>();
                Managers.UI.ShowSceneUI<UI_Game_Menu>();
            });

        Managers.Input.AddUIKeyAction(QuitMenu);

        if (!Managers.Data.MapItemDict["Twins"].consume)
            return;

        // 아이콘 설명쪽의 아이콘 설정
        GetImage((int)Images.LockIcon).sprite =
            Managers.Resource.Load<Sprite>(Managers.Data.IconDict["path_lock"].path);
        GetImage((int)Images.TemporaryLockIcon).sprite =
            Managers.Resource.Load<Sprite>(Managers.Data.IconDict["path_templock"].path);
        GetImage((int)Images.OpenIcon).sprite =
            Managers.Resource.Load<Sprite>(Managers.Data.IconDict["path_open"].path);

        GetImage((int)Images.BossIcon).sprite =
            Managers.Resource.Load<Sprite>(Managers.Data.IconDict["boss"].path);
        GetImage((int)Images.LatchIcon).sprite =
            Managers.Resource.Load<Sprite>(Managers.Data.IconDict["latch"].path);
        GetImage((int)Images.SaveIcon).sprite =
            Managers.Resource.Load<Sprite>(Managers.Data.IconDict["save"].path);
        GetImage((int)Images.TeleportIcon).sprite =
            Managers.Resource.Load<Sprite>(Managers.Data.IconDict["teleport"].path);

        GetImage((int)Images.FirmWareIcon).sprite =
            Managers.Resource.Load<Sprite>(Managers.Data.IconDict["firmWare"].path);
        GetImage((int)Images.UpgradeHpIcon).sprite =
            Managers.Resource.Load<Sprite>(Managers.Data.IconDict["upgradeHp"].path);
        GetImage((int)Images.WeaponIcon).sprite =
            Managers.Resource.Load<Sprite>(Managers.Data.IconDict["weapon"].path);

        // 미니맵
        GameObject gameMap = GetObject((int)GameObjects.GameMap);

        // 미니맵의 최소 최대 좌표. 영역을 가운데 정렬하기 위해 사용
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        // 맵 데이터로부터 아이콘 데이터를 불러와서 설정
        Dictionary<string, List<Sprite>> mapIconDict = new Dictionary<string, List<Sprite>>();
        string scene = Managers.Scene.GetCurrentSceneName();
        Data.MapData mapData = Managers.Data.MapDict[scene];
        foreach (string roomId in mapData.roomIds)
        {
            // 방문한적 없는 방이면 넘기기
            if (Managers.Data.RoomDict[roomId].visited == false)
                continue;

            List<Sprite> current = new List<Sprite>();
            mapIconDict.Add(roomId, current);

            // 아이콘 경로에 해당하는 스프라이트 넣어주기
            Data.RoomIconData roomIconData = Managers.Data.RoomIconDict.GetValueOrDefault(roomId, null);
            if (roomIconData == null)
                continue;

            if (roomIconData.boss)
                current.Add(Managers.Resource.Load<Sprite>(Managers.Data.IconDict["boss"].path));

            if (roomIconData.latch)
                current.Add(Managers.Resource.Load<Sprite>(Managers.Data.IconDict["latch"].path));

            if (roomIconData.save)
                current.Add(Managers.Resource.Load<Sprite>(Managers.Data.IconDict["save"].path));

            if (roomIconData.teleport)
                current.Add(Managers.Resource.Load<Sprite>(Managers.Data.IconDict["teleport"].path));

            if (roomIconData.upgradeHp)
                current.Add(Managers.Resource.Load<Sprite>(Managers.Data.IconDict["upgradeHp"].path));

            if (roomIconData.firmWare)
                current.Add(Managers.Resource.Load<Sprite>(Managers.Data.IconDict["firmWare"].path));

            if (roomIconData.weapon)
                current.Add(Managers.Resource.Load<Sprite>(Managers.Data.IconDict["weapon"].path));
        }

        //section 을 통해 미니맵 생성
        foreach (CameraSection section in Managers.Camera.Sections)
        {
            // 해당 방에 방문한적 없거나 데이터 없으면 정보 출력 안해줌
            if (mapIconDict.GetValueOrDefault($"{scene}_{section.name}", null) == null)
                continue;

            Transform map = gameMap.transform;
            GameObject area = new GameObject() { name = section.name };
            area.AddComponent<RectTransform>();
            RectTransform areaRect = area.transform as RectTransform;

            areaRect.SetParent(section.transform.parent);

            // 데이터 파일에 있는 대로 아이콘 설정
            List<Sprite> iconImages = mapIconDict[$"{scene}_{section.name}"];

            int iconCount = iconImages.Count;
            float iconSize = 50; // 아이콘 크기
            float totalWidth = iconCount * iconSize;
            float startX = -totalWidth / 2 + iconSize / 2;

            for (int i = 0; i < iconCount; i++)
            {
                Sprite iconImage = iconImages[i];
                GameObject icon = new GameObject() { name = $"{section.name}_icon{i}" };
                icon.AddComponent<RectTransform>();
                RectTransform iconRect = icon.transform as RectTransform;
                iconRect.SetParent(area.transform);

                Image image = icon.AddComponent<Image>();
                image.sprite = iconImage;

                // 아이콘 크기 지정
                iconRect.sizeDelta = new Vector2(iconSize, iconSize);

                // 아이콘 이미지의 위치 지정
                float x = startX + i * iconSize;
                float y = 0;
                iconRect.anchoredPosition = new Vector2(x, y);
            }

            // 영역 색상 지정
            Image areaImage = area.AddComponent<Image>();
            // 현재 위치 표시
            if (section.name.Equals(Managers.Camera.LiveCamera.name))
                areaImage.sprite = Managers.Resource.Load<Sprite>(Managers.Data.IconDict["current_room"].path);
            else
                areaImage.sprite = Managers.Resource.Load<Sprite>(Managers.Data.IconDict[$"{scene}_Room"].path);

            areaImage.type = Image.Type.Sliced;

            // 테두리 색상 지정
            Outline outline = area.AddComponent<Outline>();
            outline.effectColor = Color.black;

            // 영역 사이즈 10배로 변경
            areaRect.sizeDelta = new Vector2(
                section.transform.localScale.x * 10,
                section.transform.localScale.y * 10);
            areaRect.SetParent(map);

            // 영역 포지션 10배로 조정
            areaRect.localPosition = new Vector3(
                section.transform.localPosition.x * 10,
                section.transform.localPosition.y * 10,
                section.transform.localPosition.z);
            

            // 최대 최소 좌표 갱신
            Vector3[] corners = new Vector3[4];
            areaRect.GetWorldCorners(corners);

            foreach (Vector3 corner in corners)
            {
                Vector3 localCorner = gameMap.transform.InverseTransformPoint(corner);
                minX = Mathf.Min(minX, localCorner.x);
                maxX = Mathf.Max(maxX, localCorner.x);
                minY = Mathf.Min(minY, localCorner.y);
                maxY = Mathf.Max(maxY, localCorner.y);
            }
        }

        // 미니맵에 통로 이미지 추가
        foreach (GameObject path in Managers.Game.paths)
        {
            // 통로 데이터가 없으면 넘김
            Data.PathData current = Managers.Data.PathDict.GetValueOrDefault($"{scene}_{path.name}", null);
            if (current == null)
                continue;

            // 경로가 연결된 두 방에 모두 방문한적 없거나 데이터 없으면 정보 출력 안해줌
            if (mapIconDict.GetValueOrDefault(current.roomId1, null) == null &&
                mapIconDict.GetValueOrDefault(current.roomId2, null) == null)
                continue;

            Transform map = gameMap.transform;
            GameObject area = new GameObject() { name = path.name };
            area.AddComponent<RectTransform>();
            RectTransform areaRect = area.transform as RectTransform;

            areaRect.SetParent(path.transform.parent);

            // 통로 색상 지정
            Image areaImage = area.AddComponent<Image>();
            areaImage.sprite = Managers.Resource.Load<Sprite>(Managers.Data.IconDict[$"path_{current.type}"].path);

            // 영역 사이즈 10배로 변경
            areaRect.sizeDelta = new Vector2(
                path.transform.localScale.x * 10,
                path.transform.localScale.y * 10);
            areaRect.SetParent(map);

            // 영역 포지션 10배로 조정
            areaRect.localPosition = new Vector3(
                path.transform.localPosition.x * 10,
                path.transform.localPosition.y * 10,
                path.transform.localPosition.z);
        }

        // 미니맵의 중앙 좌표 계산
        Vector2 mapCenter = new Vector2((minX + maxX) / 2, (minY + maxY) / 2);
        // gameMap 중앙 좌표
        RectTransform gameMapRect = gameMap.GetComponent<RectTransform>();
        Vector2 gameMapCenter = new Vector2(
            (gameMapRect.rect.xMin + gameMapRect.rect.xMax) / 2,
            (gameMapRect.rect.yMin + gameMapRect.rect.yMax) / 2);

        // 둘의 차이나는 좌표 gameMapCenter가 더 왼쪽이면 오른쪽으로, 위면 아래로
        Vector2 difference = new Vector2(gameMapCenter.x - mapCenter.x, gameMapCenter.y - mapCenter.y);

        // 맵의 중앙 좌표를 바탕으로 가운데 정렬
        foreach (Transform child in gameMap.transform)
        {
            RectTransform childRect = child.GetComponent<RectTransform>();
            if (childRect != null)
            {
                childRect.localPosition = new Vector3(
                child.localPosition.x + difference.x,
                child.localPosition.y + difference.y,
                child.localPosition.z);
            }
        }
    }

    public override void Clear()
    {
        base.Clear();
        Managers.Input.RemoveUIKeyAction(QuitMenu);
    }
}
