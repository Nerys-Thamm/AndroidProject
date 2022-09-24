using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
        public Ability ability;
        public int unlockRequirement;
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
    [SerializeField] private List<AbilityUnlock> _abilityUnlocks;
    [SerializeField] private List<StatUnlock> _statUnlocks;


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

}
