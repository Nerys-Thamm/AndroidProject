using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using System;

[CreateAssetMenu(fileName = "New Skill", menuName = "GameData/Combat/Create Skill")]
public class SkillData : ScriptableObject
{
    public enum StatType
    {
        HP,
        MP,
        STR,
        DEF,
        AGI,
        INT
    }
    [System.Serializable]
    public struct AbilityUnlock
    {
        [SerializeField] public Ability ability;
        public int unlockRequirement;
    }
    [System.Serializable]
    public struct SerializedAbilityUnlock
    {
        [SerializeField] public Ability.SerializedAbility ability;
        public int unlockRequirement;

        public SerializedAbilityUnlock(AbilityUnlock abilityUnlock)
        {
            ability = new Ability.SerializedAbility(abilityUnlock.ability);
            unlockRequirement = abilityUnlock.unlockRequirement;
        }

        public AbilityUnlock Deserialize()
        {
            return new AbilityUnlock
            {
                ability = ability.GetAbility(),
                unlockRequirement = unlockRequirement
            };
        }
    }
    [System.Serializable]
    public struct StatUnlock
    {
        public int statPoints;
        public int unlockRequirement;
        public StatType statType;
    }

    [Header("Skill Info")]
    [SerializeField] private string _name;
    [SerializeField] private string _description;
    public string Name { get { return _name; } }
    public string Description { get { return _description; } }


    [Header("Skill Tree")]
    [SerializeField] private int _skillPoints;
    public int SkillPoints { get { return _skillPoints; } }
    [SerializeField] private List<AbilityUnlock> _abilityUnlocks;
    [SerializeField] private List<StatUnlock> _statUnlocks;
    public void InvestPoints(int points)
    {
        _skillPoints += points;
    }
    public void ResetPoints()
    {
        _skillPoints = 0;
    }


    [System.Serializable]
    public struct UnlockInfo
    {
        public int unlockRequirement;
        public string unlockName;
        public string unlockDescription;
        public bool isUnlocked;
    }

    public List<UnlockInfo> GetUnlockInfo()
    {
        List<UnlockInfo> unlocks = new List<UnlockInfo>();
        foreach (AbilityUnlock a in _abilityUnlocks)
        {

            unlocks.Add(new UnlockInfo() { unlockRequirement = a.unlockRequirement, unlockName = a.ability.Name, isUnlocked = (a.unlockRequirement <= _skillPoints), unlockDescription = a.ability.Description });

        }
        foreach (StatUnlock s in _statUnlocks)
        {
            unlocks.Add(new UnlockInfo() { unlockRequirement = s.unlockRequirement, unlockName = s.statType.ToString() + "+" + s.statPoints.ToString(), isUnlocked = (s.unlockRequirement <= _skillPoints) });
        }
        unlocks.Sort((x, y) => x.unlockRequirement.CompareTo(y.unlockRequirement));
        return unlocks;
    }

    public List<Ability> GetUnlockedAbilities()
    {
        List<Ability> abilities = new List<Ability>();
        foreach (AbilityUnlock a in _abilityUnlocks)
        {
            if (a.unlockRequirement <= _skillPoints)
            {
                abilities.Add(a.ability);
            }
        }
        return abilities;
    }

    [System.Serializable]
    public class SerializedSkillData
    {
        public string name;
        public string description;
        public int skillPoints;
        public List<SerializedAbilityUnlock> abilityUnlocks;
        public List<StatUnlock> statUnlocks;

        public SerializedSkillData(SkillData skillData)
        {
            name = skillData._name;
            description = skillData._description;
            skillPoints = skillData._skillPoints;
            statUnlocks = skillData._statUnlocks;
            abilityUnlocks = new List<SerializedAbilityUnlock>();
            foreach (AbilityUnlock a in skillData._abilityUnlocks)
            {
                abilityUnlocks.Add(new SerializedAbilityUnlock(a));
            }
        }

        public SkillData GetSkillData()
        {
            SkillData skillData = ScriptableObject.CreateInstance<SkillData>();
            skillData._name = name;
            skillData._description = description;
            skillData._skillPoints = skillPoints;
            skillData._statUnlocks = statUnlocks;
            skillData._abilityUnlocks = new List<AbilityUnlock>();
            foreach (SerializedAbilityUnlock a in abilityUnlocks)
            {
                skillData._abilityUnlocks.Add(a.Deserialize());
            }
            return skillData;
        }
    }

}
