using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager
{
    private Dictionary<string, float> lastUsedTimes = new Dictionary<string, float>();
    private Dictionary<string, float> currentCoolTime = new Dictionary<string, float>();

    public void Init()
    {
        foreach (Data.CoolTime coolTime in Managers.Data.CoolTimeDict.Values)
        {
            string skillName = coolTime.id;
            lastUsedTimes[skillName] = -Mathf.Infinity;
            currentCoolTime[skillName] = 0;
        }
    }

    /**
     * 스킬 사용 성공 여부 반환
     * 쿨타임 입력 안하면 기본 쿨타임 적용
     */
    public bool UseSkill(string skillName)
    {
        return UseSkill(skillName, Managers.Data.CoolTimeDict[skillName].coolTime);
    }

    /**
     * 쿨타임을 적용하여 스킬 사용 성공 여부 반환
     */
    public bool UseSkill(string skillName, float coolTime)
    {
        if (Time.time < (lastUsedTimes[skillName] + coolTime))
            return false;

        // 스킬 사용
        lastUsedTimes[skillName] = Time.time;
        currentCoolTime[skillName] = coolTime;
        return true;
    }

    /**
     * 남은 쿨타임을 반환
     */
    public float RemainCoolTime(string skillName)
    {
        float result = lastUsedTimes[skillName] + currentCoolTime[skillName] - Time.time;
        return result > 0 ? result : 0;
    }

    /**
     * 스킬 사용 가능 여부 반환
     */
    public bool CanUseSkill(string skillName)
    {
        float time = lastUsedTimes[skillName] + currentCoolTime[skillName] - Time.time;
        return time > 0 ? false : true;
    }
}
