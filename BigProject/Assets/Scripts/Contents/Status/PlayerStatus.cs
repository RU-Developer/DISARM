using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : Status
{
    private int _money;
    private int _memory;
    private int _maxMemory;
    private string _scene;
    private float _x;
    private float _y;
    private string _weapon;
    private bool isInvincible;

    public int Money { get { return _money; } protected set { _money = value; } }
    public int Memory { get { return _memory; } protected set { _memory = value; } }
    public int MaxMemory { get { return _maxMemory; } protected set { _maxMemory = value; } }
    public string Weapon { get { return _weapon; } protected set { _weapon = value; } }

    private Action _weaponChangedAction;

    protected override void Init()
    {
        if (Hp != 0)
            return;

        base.Init();
        Data.PlayerStatus status = Managers.Data.PlayerStatusDict["saved"];
        Hp = status.hp;
        MaxHp = status.maxHp;
        Attack = status.attack;
        _money = status.money;
        _memory = status.memory;
        _maxMemory = status.maxMemory;
        _weapon = status.weapon;

        _scene = status.scene;
        _x = status.x;
        _y = status.y;
        Debug.Log("PlayerStatus Init");
        GetComponent<PlayerGun>().Init();
    }

    public override void OnDead(Status attacker = null)
    {
        if (attacker == null)
            Debug.Log($"RGB쨩이 죽었습니다.");
        else
            Debug.Log($"RGB쨩이 {attacker.name}에게 죽었습니다.");
        Managers.Sound.Play("die");

        Managers.UI.ShowPopupUI<UI_Respawn>(); // Respawn 팝업을 닫을 때 이후 진행하도록 변경
        Managers.Pause.Pause();
    }

    public override void OnKill(Status deadTarget)
    {
        Debug.Log($"RGB쨩이 {deadTarget.name}을(를) 처치했습니다.");
    }

    public override void OnDamaged(Status attacker, Vector2 knockback = default, int damage = -1)
    {
        if (!isInvincible)
        {
            StartCoroutine(Invincible());
            base.OnDamaged(attacker, knockback, damage);
        }
        
    }

    public IEnumerator Invincible()
    {
        isInvincible = true;
        
        yield return new WaitForSeconds(0.5f);
        isInvincible = false;
    }

    public bool UpdateMaxHp(int amount)
    {
        if (MaxHp + amount <= 0)
            return false;

        MaxHp += amount;
        Managers.Data.PlayerStatusDict["saved"].maxHp = MaxHp;

        return true;
    }

    public void UpdateHp(int amount)
    {
        if (Hp + amount > MaxHp)
            Hp = MaxHp;
        else if (Hp + amount <= 0)
            OnDead();
        else
            Hp += amount;
    }

    public void FullRecover()
    {
        Hp = MaxHp;
    }

    public bool UpdateMoney(int amount)
    {
        if (amount < 0 && -amount > _money)
            return false;

        _money += amount;
        Managers.Data.PlayerStatusDict["saved"].money = _money;
        return true;
    }

    public bool UpdateMemory(int amount)
    {
        if (amount < 0 && -amount > _memory || amount > 0 && _memory + amount > _maxMemory)
            return false;

        _memory += amount;
        Managers.Data.PlayerStatusDict["saved"].memory = _memory;
        return true;
    }

    public void SavePosition()
    {
        _scene = Managers.Scene.GetCurrentSceneName();
        _x = transform.position.x;
        _y = transform.position.y;
    }

    public void SaveStatus()
    {
        Init();
        Data.PlayerStatus status = Managers.Data.PlayerStatusDict["saved"];
        status.hp = Hp;
        status.maxHp = MaxHp;
        status.attack = Attack;
        status.money = _money;
        status.memory = _memory;
        status.maxMemory = _maxMemory;
        status.weapon = _weapon;
        status.scene = _scene;
        status.x = _x;
        status.y = _y;
    }

    public void MoveToSavedPosition()
    {
        Init();
        Debug.Log($"saved Point = {_scene}:{_x},{_y}");
        if (!_scene.Equals(Managers.Scene.GetCurrentSceneName()))
        {
            Managers.Scene.LoadScene(_scene);
            return;
        }

        transform.position = new Vector3(_x, _y);
    }

    /**
     * enum을 사용한 변경
     */
    public void ChangeWeapon(Define.Weapon weapon)
    {
        ChangeWeapon(System.Enum.GetName(typeof(Define.Weapon), weapon));
    }

    /**
     * 착용중인 무기 변경
     */
    public void ChangeWeapon(string weapon)
    {
        _weapon = weapon;
        Managers.Data.PlayerStatusDict["saved"].weapon = _weapon;
        if (_weaponChangedAction != null)
            _weaponChangedAction.Invoke();
    }

    /**
     * 무기 해제
     */
    public void UnarmWeapon()
    {
        _weapon = "None";
        Managers.Data.PlayerStatusDict["saved"].weapon = _weapon;
        if (_weaponChangedAction != null)
            _weaponChangedAction.Invoke();
    }

    /**
     * 무기 교체 이벤트 구독하기
     */
    public void AddWeaponChangedAction(Action action)
    {
        _weaponChangedAction -= action;
        _weaponChangedAction += action;
    }

    /**
     * 무기 교체 이벤트 구독 해제하기
     */
    public void RemoveWeaponChangedAction(Action action)
    {
        _weaponChangedAction -= action;
    }

    /**
     * 플레이어가 Despawn될 때 같이 처리해야할 로직
     */
    public override void Despawn()
    {
        _weaponChangedAction = null;
    }
}
