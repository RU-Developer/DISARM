using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

/**
 * 맵에 있는 아이템에 넣을 컴포넌트  
 */ 
public class MapItem : MonoBehaviour
{
    [SerializeField]
    private string _itemId;

    [SerializeField]
    private string _id;

    GameObject _player;
    private float _eventDistance;
    private Data.ItemDetail _detail;
    private Data.MapItem _item;

    private bool _updated;

    private void Start()
    {
        _player = Managers.Game.GetPlayer();
        Managers.Game.AddPlayerChangedAction(ChangePlayer);
        Managers.Input.AddKeyAction(OnGet);
        _eventDistance = 2.0f;
        _updated = false;
        SetMapItem();
    }

    public void SetMapItem()
    {
        if (string.IsNullOrEmpty(_id) || string.IsNullOrEmpty(_itemId))
            return;

        // 아이템 정보가 없는 경우. 아이템 정보는 json에 잘 입력해 놔야 합니다.
        Data.ItemDetail detail;
        Managers.Data.ItemDetailDict.TryGetValue(_itemId, out detail);
        if (detail == null)
        {
            Debug.Log($"{name} 아이템의 정보가 없습니다. itemId를 확인해주세요.");
            return;
        }
        _detail = detail;

        Data.MapItem saved = Managers.Data.MapItemDict.GetValueOrDefault(_id, null);
        // 처음 생성된 아이템의 경우 추가
        if (Managers.Game.TEST && saved == null)
        {
            saved = new Data.MapItem();
            Managers.Data.MapItemDict[_id] = saved;
            saved.id = _id;
            saved.itemId = _itemId;
            saved.consume = false;
        }

        // 아이템 위치 변경 적용
        saved.scene = Managers.Scene.GetCurrentSceneName();
        saved.x = transform.position.x;
        saved.y = transform.position.y;

        if (saved.consume)
        {
            Managers.Resource.Destroy(gameObject);
            return;
        }

        _item = saved;
        _updated = true;
    }

    private void Update()
    {
        if (!_updated)
            return;

        Managers.Data.Init();
        if (Managers.Game.TEST)
            Managers.Data.SaveJson<Data.MapItem>(false);
        GetComponent<SpriteRenderer>().sprite = Managers.Resource.Load<Sprite>(Managers.Data.IconDict[_detail.icon].path);

        _updated = false;

        if (_item.consume)
            Managers.Resource.Destroy(gameObject);
    }

    /**
     * 아이템 획득시 팝업을 띄우고 아이템 효능 적용
     * 아이템 효능은 json을 확인하여 함수가 있으면 해당 함수명을 토대로 실행
     */
    private void OnGet()
    {
        if (Input.GetKeyDown(KeyCode.Return) == false || _detail == null || _item == null || _player == null)
            return;

        float distance = Vector2.Distance(new Vector2(_player.transform.position.x, _player.transform.position.y),
                new Vector2(transform.position.x, transform.position.y));

        if (distance > _eventDistance)
            return;

        // 아이템은 한번 먹으면 사라질 예정. 사라지기 전에 리소스 해제
        _item.consume = true;
        Managers.Sound.Play("item_get");

        Managers.Game.RemovePlayerChangedAction(ChangePlayer);
        Managers.Input.RemoveKeyAction(OnGet);

        Managers.UI.ShowPopupUI<UI_MapItem>().SetInfo(_detail.name, _detail.description, _detail.icon);
        Managers.Pause.Pause();

        MapItemFunctions.Invoke(_detail.id); //아이템 게임 오브젝트 이름(id)와 동일한 함수 호출

        //아이템 팝업 활성화 후 삭제
        Managers.Resource.Destroy(gameObject);
    }

    private void ChangePlayer()
    {
        _player = Managers.Game.GetPlayer();
    }
}
