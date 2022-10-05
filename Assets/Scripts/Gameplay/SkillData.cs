using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using System;

/// <summary>
///  Contains data about a skill.
/// </summary>
[CreateAssetMenu(fileName = "New Skill", menuName = "GameData/Combat/Create Skill")]
public class SkillData : ScriptableObject
{
    /// <summary>
    ///  The type of stat that this skill modifies.
    /// </summary>
    public enum StatType
    {
        HP, /// > Hit Points
        MP, /// > Magic Points
        STR, /// > Strength
        DEF, /// > Defense
        AGI, /// > Agility
        INT /// > Intelligence
    }

    /// <summary>
    ///  Unlockable Ability.
    /// </summary>
    [System.Serializable]
    public struct AbilityUnlock
    {
        [SerializeField] public Ability ability;
        public int unlockRequirement;
    }

    /// <summary>
    ///  Unlockable ability as a serialized class.
    /// </summary>
    [System.Serializable]
    public struct SerializedAbilityUnlock
    {
        [SerializeField] public Ability.SerializedAbility ability; // Serialized ability
        public int unlockRequirement; // SP requirement to unlock

        /// <summary>
        ///  Creates a serialized ability unlock from an ability unlock.
        /// </summary>
        /// <param name="abilityUnlock"></param>
        public SerializedAbilityUnlock(AbilityUnlock abilityUnlock)
        {
            ability = new Ability.SerializedAbility(abilityUnlock.ability);
            unlockRequirement = abilityUnlock.unlockRequirement;
        }

        /// <summary>
        ///   Creates an ability unlock from a serialized ability unlock.
        /// </summary>
        /// <returns></returns>
        public AbilityUnlock Deserialize()
        {
            return new AbilityUnlock
            {
                ability = ability.GetAbility(),
                unlockRequirement = unlockRequirement
            };
        }
    }

    /// <summary>
    ///  A stat boost unlock.
    /// </summary>
    [System.Serializable]
    public struct StatUnlock
    {
        public int statPoints; // The amount of points to boost the stat by
        public int unlockRequirement; // The SP requirement to unlock
        public StatType statType; // The type of stat to boost
    }

    [Header("Skill Info")]
    [SerializeField] private string _name; // The name of the skill
    [SerializeField] private string _description; // The description of the skill
    public string Name { get { return _name; } } // The name of the skill
    public string Description { get { return _description; } } // The description of the skill


    [Header("Skill Tree")]
    [SerializeField] private int _skillPoints; // The amount of skill points invested in this skill
    public int SkillPoints { get { return _skillPoints; } } // The amount of skill points invested in this skill
    [SerializeField] private List<AbilityUnlock> _abilityUnlocks; // The list of abilities that can be unlocked
    [SerializeField] private List<StatUnlock> _statUnlocks; // The list of stat boosts that can be unlocked

    /// <summary>
    ///  Invests skill points into this skill.
    /// </summary>
    /// <param name="points"></param>
    public void InvestPoints(int points)
    {
        _skillPoints += points;
    }

    /// <summary>
    ///  Resets the skill points invested in this skill.
    /// </summary>
    public void ResetPoints()
    {
        _skillPoints = 0;
    }

    /// <summary>
    /// Data struct storing info about the contents of a skill for use with UI.
    /// </summary>
    [System.Serializable]
    public struct UnlockInfo
    {
        public int unlockRequirement;
        public string unlockName;
        public string unlockDescription;
        public bool isUnlocked;
    }

    /// <summary>
    ///  Gets all of the info pertaining to the skill, for use with UI.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    ///  Gets a list of all currently unlocked abilities.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    ///  A serialized version of the skill data.
    /// </summary>
    [System.Serializable]
    public class SerializedSkillData
    {
        public string name;
        public string description;
        public int skillPoints;
        public List<SerializedAbilityUnlock> abilityUnlocks;
        public List<StatUnlock> statUnlocks;

        /// <summary>
        ///  Creates a serialized version of the skill data.
        /// </summary>
        /// <param name="skillData"></param>
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


        /// <summary>
        ///  Creates a skill data from a serialized version of the skill data.
        /// </summary>
        /// <returns></returns>
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
