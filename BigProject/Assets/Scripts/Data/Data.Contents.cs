using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DataManager;

namespace Data //단순 데이터를 가져오기 위한 클래스로 메인 컨텐츠들과의 이름이 충돌될 수 있어 미리 namespace 적용
{
    #region PlayerStatus
    /**
     * 게임 내 데이터 예시입니다.
     * Json 파일에 있는 변수명과 동일하게 필드명을 작성해야 합니다.
     * Serializable을 붙여줘야 Json파일로 변환 및 Json에서 해당 클래스 객체로 변환이 가능합니다.
     * 필드는 public 접근제한자나 [SerializeField] 어트리뷰트를 붙여서 사용하면 됩니다.
     */
    [Serializable]
    public class PlayerStatus
    {
        public string id;
        public int hp;
        public int maxHp;
        public int attack;
        public int money;
        public int memory;
        public int maxMemory;
        public string weapon;
        public string scene;
        public float x;
        public float y;
    }

    /**
     * ILoader를 구현하여 Dictionary를 생성할 수 있게 작성합니다.
     * Json의 {} 중괄호는 클래스 객체, [] 대괄호는 List 로 매핑됩니다.
     */
    [Serializable]
    public class PlayerStatusLoader : ILoader<string, PlayerStatus>
    {
        /**
         * Json 파일의 playerStats에 있는 리스트를 담습니다.
         */
        public List<PlayerStatus> playerStats = new List<PlayerStatus>();

        /**
         * List를 Dictionary로 변환합니다.
         */
        public Dictionary<string, PlayerStatus> MakeDict()
        {
            Dictionary<string, PlayerStatus> dict = new Dictionary<string, PlayerStatus>();

            foreach (PlayerStatus status in playerStats)
                dict.Add(status.id, status);

            playerStats.Clear();

            return dict;
        }

        /**
         * 저장할 때 사용할 방법입니다.
         */
        public void LoadFromDict()
        {
            playerStats = new List<PlayerStatus>();
            foreach (PlayerStatus status in Managers.Data.PlayerStatusDict.Values)
                playerStats.Add(status);
        }
    }
    #endregion

    #region MonsterStatus

    [Serializable]
    public class MonsterStatus
    {
        public string id;
        public int maxHp;
        public int attack;
    }

    [Serializable]
    public class MonsterStatusLoader : ILoader<string, MonsterStatus>
    {
        public List<MonsterStatus> monsterStats = new List<MonsterStatus>();

        public void LoadFromDict()
        {
            monsterStats = new List<MonsterStatus>();
            foreach (MonsterStatus status in Managers.Data.MonsterStatusDict.Values)
                monsterStats.Add(status);
        }

        public Dictionary<string, MonsterStatus> MakeDict()
        {
            Dictionary<string, MonsterStatus> dict = new Dictionary<string, MonsterStatus>();

            foreach (MonsterStatus status in monsterStats)
                dict.Add(status.id, status);

            monsterStats.Clear();

            return dict;
        }
    }

    #endregion

    #region Non Damageable Environment Status

    [Serializable]
    public class NonDamageableEnvStatus
    {
        public string id;
        public int attack;
    }

    [Serializable]
    public class NonDamageableEnvStatusLoader : ILoader<string, NonDamageableEnvStatus>
    {
        public List<NonDamageableEnvStatus> envStats = new List<NonDamageableEnvStatus>();

        public void LoadFromDict()
        {
            envStats = new List<NonDamageableEnvStatus>();
            foreach (NonDamageableEnvStatus status in Managers.Data.NonDamageableEnvStatusDict.Values)
                envStats.Add(status);
        }

        public Dictionary<string, NonDamageableEnvStatus> MakeDict()
        {
            Dictionary<string, NonDamageableEnvStatus> dict = new Dictionary<string, NonDamageableEnvStatus>();

            foreach (NonDamageableEnvStatus status in envStats)
                dict.Add(status.id, status);

            envStats.Clear();

            return dict;
        }
    }

    #endregion

    #region Dialog Scripts

    [Serializable]
    public class Script
    {
        public string content;
        public string link;
    }

    [Serializable]
    public class DialogScript
    {
        public string id;
        public string name;
        public Script script;
        public List<Script> selections;
    }

    [Serializable]
    public class DialogScriptLoader : ILoader<string, DialogScript>
    {
        public List<DialogScript> scripts = new List<DialogScript>();

        public void LoadFromDict()
        {
            scripts = new List<DialogScript>();
            foreach (DialogScript script in Managers.Data.DialogScriptDict.Values)
                scripts.Add(script);
        }

        public Dictionary<string, DialogScript> MakeDict()
        {
            Dictionary<string, DialogScript> dict = new Dictionary<string, DialogScript>();

            foreach (DialogScript script in scripts)
                dict.Add(script.id, script);

            scripts.Clear();

            return dict;
        }
    }

    #endregion

    #region Weapon Data

    [Serializable]
    public class WeaponData
    {
        public string id;
        public string description;
        public string name;
        public int attack;
        public bool own;
    }

    [Serializable]
    public class WeaponLoader : ILoader<string, WeaponData>
    {
        public List<WeaponData> weapons = new List<WeaponData>();

        public void LoadFromDict()
        {
            weapons = new List<WeaponData>();
            foreach (WeaponData weapon in Managers.Data.WeaponDict.Values)
                weapons.Add(weapon);
        }

        public Dictionary<string, WeaponData> MakeDict()
        {
            Dictionary<string, WeaponData> dict = new Dictionary<string, WeaponData>();
            foreach (WeaponData weapon in weapons)
                dict.Add(weapon.id, weapon);

            weapons.Clear();

            return dict;
        }
    }

    #endregion

    #region MapItem
    [Serializable]
    public class ItemDetail
    {
        public string id;
        public string name;
        public string description;
        public string icon;
    }

    [Serializable]
    public class ItemDetailLoader : ILoader<string, ItemDetail>
    {
        public List<ItemDetail> itemDetails = new List<ItemDetail>();

        public void LoadFromDict()
        {
            itemDetails = new List<ItemDetail>();
            foreach (ItemDetail item in Managers.Data.ItemDetailDict.Values)
                itemDetails.Add(item);
        }

        public Dictionary<string, ItemDetail> MakeDict()
        {
            Dictionary<string, ItemDetail> dict = new Dictionary<string, ItemDetail>();

            foreach (ItemDetail item in itemDetails)
                dict.Add(item.id, item);

            itemDetails.Clear();

            return dict;
        }
    }


    [Serializable]
    public class MapItem
    {
        public string id;
        public string itemId;
        public string scene;
        public float x;
        public float y;
        public bool consume;
    }

    [Serializable]
    public class MapItemLoader : ILoader<string, MapItem>
    {
        public List<MapItem> mapItems = new List<MapItem>();

        public void LoadFromDict()
        {
            mapItems = new List<MapItem>();
            foreach (MapItem item in Managers.Data.MapItemDict.Values)
                mapItems.Add(item);
        }

        public Dictionary<string, MapItem> MakeDict()
        {
            Dictionary<string, MapItem> dict = new Dictionary<string, MapItem>();

            foreach (MapItem item in mapItems)
                dict.Add(item.id, item);

            mapItems.Clear();

            return dict;
        }
    }

    #endregion

    #region ShopItem

    [Serializable]
    public class ShopItem
    {
        public string id;
        public string name;
        public string description;
        public int cost;
        public int memory;
        public string imageSrc;
    }

    [Serializable]
    public class ShopItemLoader : ILoader<string, ShopItem>
    {
        public List<ShopItem> shopItems = new List<ShopItem>();

        public void LoadFromDict()
        {
            shopItems = new List<ShopItem>();
            foreach (ShopItem item in Managers.Data.ShopItemDict.Values)
                shopItems.Add(item);
        }

        public Dictionary<string, ShopItem> MakeDict()
        {
            Dictionary<string, ShopItem> dict = new Dictionary<string, ShopItem>();

            foreach (ShopItem item in shopItems)
                dict.Add(item.id, item);

            shopItems.Clear();

            return dict;
        }
    }

    #endregion

    #region Collection

    [Serializable]
    public class Collection
    {
        public string id;
        public string name;
        public int memory;
        public string description;
        public string imageSrc;
        public bool equipped;
    }

    [Serializable]
    public class CollectionLoader : ILoader<string, Collection>
    {
        public List<Collection> collections = new List<Collection>();

        public void LoadFromDict()
        {
            collections = new List<Collection>();
            foreach (Collection collection in Managers.Data.CollectionDict.Values)
                collections.Add(collection);
        }

        public Dictionary<string, Collection> MakeDict()
        {
            Dictionary<string, Collection> dict = new Dictionary<string, Collection>();

            foreach (Collection collection in collections)
                dict.Add(collection.id, collection);

            collections.Clear();

            return dict;
        }
    }

    #endregion

    #region WarpPoint

    [Serializable]
    public class WarpPoint
    {
        public string id;
        public string scene;
        public string link;
        public float x;
        public float y;
    }

    [Serializable]
    public class WarpPointLoader : ILoader<string, WarpPoint>
    {
        public List<WarpPoint> warpPoints = new List<WarpPoint>();

        public void LoadFromDict()
        {
            warpPoints = new List<WarpPoint>();
            foreach (WarpPoint warp in Managers.Data.WarpPointDict.Values)
                warpPoints.Add(warp);
        }

        public Dictionary<string, WarpPoint> MakeDict()
        {
            Dictionary<string, WarpPoint> dict = new Dictionary<string, WarpPoint>();

            foreach (WarpPoint warpPoint in warpPoints)
                dict.Add(warpPoint.id, warpPoint);

            warpPoints.Clear();

            return dict;
        }
    }

    #endregion

    #region Map 미니맵을 위한 데이터

    [Serializable]
    public class MapData
    {
        public string scene;
        /**
         * sceneName_sectionNo
         */
        public List<string> roomIds;

        /**
         * sceneName_pathNo
         */
        public List<string> pathIds;
    }

    [Serializable]
    public class MapLoader : ILoader<string, MapData>
    {
        public List<MapData> maps = new List<MapData>();

        public void LoadFromDict()
        {
            maps = new List<MapData>();
            foreach (MapData map in Managers.Data.MapDict.Values)
                maps.Add(map);
        }

        public Dictionary<string, MapData> MakeDict()
        {
            Dictionary<string, MapData> dict = new Dictionary<string, MapData>();

            foreach (MapData map in maps)
                dict.Add(map.scene, map);

            maps.Clear();

            return dict;
        }
    }

    [Serializable]
    public class RoomData
    {
        public string id;
        public string scene;
        public string name;
        public bool visited;
    }

    [Serializable]
    public class RoomLoader : ILoader<string, RoomData>
    {
        public List<RoomData> rooms = new List<RoomData>();

        public void LoadFromDict()
        {
            rooms = new List<RoomData>();

            foreach (RoomData room in Managers.Data.RoomDict.Values)
                rooms.Add(room);
        }

        public Dictionary<string, RoomData> MakeDict()
        {
            Dictionary<string, RoomData> dict = new Dictionary<string, RoomData>();

            foreach (RoomData room in rooms)
                dict.Add(room.id, room);

            rooms.Clear();

            return dict;
        }
    }

    [Serializable]
    public class RoomIconData
    {
        /**
         * sceneName_sectionNo
         */
        public string id;
        public bool boss;
        public bool teleport;
        public bool latch;
        public bool save;
        public bool upgradeHp;
        public bool firmWare;
        public bool weapon;
    }

    public class RoomIconLoader : ILoader<string, RoomIconData>
    {
        public List<RoomIconData> iconDatas = new List<RoomIconData>();

        public void LoadFromDict()
        {
            iconDatas = new List<RoomIconData>();
            foreach (RoomIconData icon in Managers.Data.RoomIconDict.Values)
                iconDatas.Add(icon);
        }

        public Dictionary<string, RoomIconData> MakeDict()
        {
            Dictionary<string, RoomIconData> dict = new Dictionary<string, RoomIconData>();
            foreach (RoomIconData icon in iconDatas)
                dict.Add(icon.id, icon);

            iconDatas.Clear();

            return dict;
        }
    }

    [Serializable]
    public class PathData
    {
        public string id;
        public string scene;
        public string roomId1;
        public string roomId2;
        /**
         * open, lock, templock
         */
        public string type;
    }

    [Serializable]
    public class PathLoader : ILoader<string, PathData>
    {
        public List <PathData> paths = new List<PathData>();

        public void LoadFromDict()
        {
            paths = new List<PathData>();
            foreach (PathData path in Managers.Data.PathDict.Values)
                paths.Add(path);
        }

        public Dictionary<string, PathData> MakeDict()
        {
            Dictionary<string, PathData> dict = new Dictionary<string, PathData>();
            foreach (PathData path in paths)
                dict.Add(path.id, path);

            paths.Clear();

            return dict;
        }
    }

    #endregion

    #region Icon 아이콘 리소스 경로 매핑

    [Serializable]
    public class IconData
    {
        public string id;
        public string path;
    }

    [Serializable]
    public class IconLoader : ILoader<string, IconData>
    {
        public List<IconData> icons = new List<IconData>();

        public void LoadFromDict()
        {
            icons = new List<IconData>();
            foreach (IconData icon in Managers.Data.IconDict.Values)
                icons.Add(icon);
        }

        public Dictionary<string, IconData> MakeDict()
        {
            Dictionary<string, IconData> dict = new Dictionary<string, IconData>();
            foreach (IconData icon in icons)
                dict.Add(icon.id, icon);

            icons.Clear();

            return dict;
        }
    }

    #endregion

    #region Battle Menu Data

    //[Serializable]
    //public class BattleMenuData
    //{
    //    public string id;
    //    public string data;
    //}

    //[Serializable]
    //public class BattleMenuLoader : ILoader<string, BattleMenuData>
    //{
    //    public List<BattleMenuData> datas = new List<BattleMenuData>();

    //    public void LoadFromDict()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Dictionary<string, BattleMenuData> MakeDict()
    //    {
    //        Dictionary<string, BattleMenuData> dict = new Dictionary<string, BattleMenuData>();
    //        foreach (BattleMenuData data in datas)
    //            dict.Add(data.id, data);

    //        datas.Clear();

    //        return dict;
    //    }
    //}

    #endregion

    #region Skill CoolTime

    [Serializable]
    public class CoolTime
    {
        public string id;
        public float coolTime;
    }

    [Serializable]
    public class CoolTimeLoader : ILoader<string, CoolTime>
    {
        public List<CoolTime> skills = new List<CoolTime>();

        public void LoadFromDict()
        {
            skills = new List<CoolTime>();
            foreach (CoolTime coolTime in Managers.Data.CoolTimeDict.Values)
                skills.Add(coolTime);
        }

        public Dictionary<string, CoolTime> MakeDict()
        {
            Dictionary<string, CoolTime> dict = new Dictionary<string, CoolTime>();
            foreach (CoolTime coolTime in skills)
                dict.Add(coolTime.id, coolTime);

            skills.Clear();

            return dict;
        }
    }

    #endregion
}
