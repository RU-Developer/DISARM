using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

/**
 * 게임의 기본적인 수치 등의 데이터를 관리하는 클래스입니다.
 * ex) level 1 : hp 1, attack 1; level 2 : hp2, attack 2;
 * 기본적으로 Json 형태의 데이터를 읽어들여 Dictionary 형태로 메모리에 들고있어
 * 장면이 전환되도 계속 메모리에 유지되는 데이터입니다.
 */ 
public class DataManager
{
    /**
     * 이런식으로 데이터 하나당 필드 하나씩 관리하게 됩니다.
     * 필드가 너무 많아지게 되면, 하나의 Dictionary로 다시 묶어버리게 될 수도 있습니다.
     */ 
    public Dictionary<string, Data.PlayerStatus> PlayerStatusDict { get; private set; }
    public Dictionary<string, Data.MonsterStatus> MonsterStatusDict { get; private set; }
    public Dictionary<string, Data.NonDamageableEnvStatus> NonDamageableEnvStatusDict { get; private set; }
    public Dictionary<string, Data.DialogScript> DialogScriptDict { get; private set; }
    public Dictionary<string, Data.ItemDetail> ItemDetailDict { get; private set; }
    public Dictionary<string, Data.MapItem> MapItemDict { get; private set; }
    public Dictionary<string, Data.ShopItem> ShopItemDict { get; private set; }
    public Dictionary<string, Data.Collection> CollectionDict { get; private set; }
    public Dictionary<string, Data.WarpPoint> WarpPointDict { get; private set; }
    public Dictionary<string, Data.MapData> MapDict { get; private set; }
    public Dictionary<string, Data.RoomData> RoomDict { get; private set; }
    public Dictionary<string, Data.PathData> PathDict { get; private set; }
    public Dictionary<string, Data.IconData> IconDict { get; private set; }
    public Dictionary<string, Data.WeaponData> WeaponDict { get; private set; }
    //public Dictionary<string, Data.BattleMenuData> BattleMenuDict { get; private set; }
    public Dictionary<string, Data.RoomIconData> RoomIconDict { get; private set; }
    public Dictionary<string, Data.CoolTime> CoolTimeDict { get; private set; }

    private Dictionary<string, object> _loaders = new Dictionary<string, object>();

    /**
     * ILoader 인터페이스를 구현하여 MakeDict 함수를 호출하면 Json 데이터가 Dictionary타입으로 변환됩니다.
     */ 
    public interface ILoader<Key, Value>
    {
        Dictionary<Key, Value> MakeDict();
        void LoadFromDict();
    }

    /**
     * 이곳에서 Json을 Dictionary로 변경하여 필드에 초기화 시켜주는 작업을 진행합니다.
     */ 
    public void Init()
    {
        if (PlayerStatusDict != null)
            return;

        PlayerStatusDict = JsonToDictionary<Data.PlayerStatusLoader, string, Data.PlayerStatus>();
        MonsterStatusDict = JsonToDictionary<Data.MonsterStatusLoader, string, Data.MonsterStatus>();
        NonDamageableEnvStatusDict =
            JsonToDictionary<Data.NonDamageableEnvStatusLoader, string, Data.NonDamageableEnvStatus>();
        DialogScriptDict = JsonToDictionary<Data.DialogScriptLoader, string, Data.DialogScript>();
        ItemDetailDict = JsonToDictionary<Data.ItemDetailLoader, string, Data.ItemDetail>();
        MapItemDict = JsonToDictionary<Data.MapItemLoader, string, Data.MapItem>();
        ShopItemDict = JsonToDictionary<Data.ShopItemLoader, string, Data.ShopItem>();
        CollectionDict = JsonToDictionary<Data.CollectionLoader, string, Data.Collection>();
        WarpPointDict = JsonToDictionary<Data.WarpPointLoader, string, Data.WarpPoint>();
        MapDict = JsonToDictionary<Data.MapLoader, string, Data.MapData>();
        RoomDict = JsonToDictionary<Data.RoomLoader, string, Data.RoomData>();
        PathDict = JsonToDictionary<Data.PathLoader, string, Data.PathData>();
        IconDict = JsonToDictionary<Data.IconLoader, string, Data.IconData>();
        WeaponDict = JsonToDictionary<Data.WeaponLoader, string, Data.WeaponData>();
        //BattleMenuDict = JsonToDictionary<Data.BattleMenuLoader, string, Data.BattleMenuData>();
        RoomIconDict = JsonToDictionary<Data.RoomIconLoader, string, Data.RoomIconData>();
        CoolTimeDict = JsonToDictionary<Data.CoolTimeLoader, string, Data.CoolTime>();
    }

    /**
     * Json을 Dictionary로 변경하는 함수
     */
    Dictionary<Key, Value> JsonToDictionary<Loader, Key, Value>(string path = null) where Loader : ILoader<Key, Value>
    {
        if (string.IsNullOrEmpty(path))
            path = typeof(Value).Name;

        return LoadJson<Loader, Key, Value>(path).MakeDict();
    }

    /**
     * Json 파일을 Resources/Data/ 폴더 안에서 찾아 ILoader를 구현한 클래스 타입으로 변환해줍니다.
     */
    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        TextAsset data = Managers.Resource.Load<TextAsset>($"Data/{path}");
        Loader loader = JsonUtility.FromJson<Loader>(data.text);
        _loaders.Add(path, loader);
        return loader;
    }

    /**
     * 메모리에 들고 있던 데이터를 전부 Json파일로 저장해줍니다.
     */
    public void SaveJson(bool test = true)
    {
        foreach (string path in _loaders.Keys)
            SaveJson(path, test);
    }

    /**
     * 특정 경로의 데이터를 Json파일로 저장해줍니다.
     */
    public void SaveJson(string path, bool test = true)
    {
        // 딕셔너리 추가된 변경된 사항까지해서 전부 _loader에 반영
        object loader = _loaders[path];
        Type loaderType = loader.GetType();
        MethodInfo loadFromDictMethod = loaderType.GetMethod("LoadFromDict");
        loadFromDictMethod.Invoke(loader, null);

        string json = JsonUtility.ToJson(loader, true);
        string filename = path;

        if (test)
        {
            filename = "test/" + filename;
            Directory.CreateDirectory($"{Application.dataPath}/Resources/Data/test");
        }

        // 덮어쓰기
        FileStream fileStream = new FileStream($"{Application.dataPath}/Resources/Data/{filename}.json", FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(json);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    /**
     * 경로를 오타 없이 입력하기 위해 제네릭으로 넘길 수 있게 제공
     * test가 아닌 경우 덮어씌움
     */
    public void SaveJson<T>(bool test = true)
    {
        SaveJson(typeof(T).Name, test);
    }
}
