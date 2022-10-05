using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  UnitData is a data class that holds all the information about a unit.
/// </summary>
[CreateAssetMenu(fileName = "UnitData", menuName = "GameData/Unit/Create Unit Data", order = 0)]
public class UnitData : ScriptableObject
{
    [Header("Unit Info")]
    public string unitName; /// > The name of the unit.
    public string unitSpecies; /// > The species of the unit.
    [SerializeField] string _unitID = ""; /// > The ID of the unit.
    public string UnitID { get { if (_unitID == "") _unitID = System.Guid.NewGuid().ToString(); return _unitID; } private set { _unitID = value; } } /// > The ID of the unit.

    [Header("Unit Attributes")]
    public Attributes attributes = new Attributes(); /// > The attributes of the unit.

    [System.Serializable]
    /// <summary>
    ///  The Attributes of the unit, defined by their base amounts and stat growth.
    /// </summary>
    public struct Attributes
    {
        /// <summary>
        ///  Base Attributes
        /// </summary>
        [Header("Base Attributes")]
        [SerializeField] private int _HP_Base;
        [SerializeField] private int _MP_Base;
        [SerializeField] private int _STR_Base;
        [SerializeField] private int _DEF_Base;
        [SerializeField] private int _AGI_Base;
        [SerializeField] private int _INT_Base;

        /// <summary>
        ///  Attribute Growth
        /// </summary>
        [Header("Attribute Growth")]
        [SerializeField] private float _HP_Growth;
        [SerializeField] private float _MP_Growth;
        [SerializeField] private float _STR_Growth;
        [SerializeField] private float _DEF_Growth;
        [SerializeField] private float _AGI_Growth;
        [SerializeField] private float _INT_Growth;

        public float HP_Growth() { return _HP_Growth; }

        /// <summary>
        /// Attribute Get Methods (with level)
        /// </summary>
        public int HP(int lvl) { return Mathf.FloorToInt(_HP_Base + ((_HP_Growth) * (lvl - 1))); }
        public int MP(int lvl) { return Mathf.FloorToInt(_MP_Base + ((_MP_Growth) * (lvl - 1))); }
        public int STR(int lvl) { return Mathf.FloorToInt(_STR_Base + ((_STR_Growth) * (lvl - 1))); }
        public int DEF(int lvl) { return Mathf.FloorToInt(_DEF_Base + ((_DEF_Growth) * (lvl - 1))); }
        public int AGI(int lvl) { return Mathf.FloorToInt(_AGI_Base + ((_AGI_Growth) * (lvl - 1))); }
        public int INT(int lvl) { return Mathf.FloorToInt(_INT_Base + ((_INT_Growth) * (lvl - 1))); }

        public static Attributes Create(int hp, int mp, int str, int def, int agi, int intel, float hpGrowth, float mpGrowth, float strGrowth, float defGrowth, float agiGrowth, float intGrowth)
        {
            Attributes a = new()
            {
                _HP_Base = hp,
                _MP_Base = mp,
                _STR_Base = str,
                _DEF_Base = def,
                _AGI_Base = agi,
                _INT_Base = intel,
                _HP_Growth = hpGrowth,
                _MP_Growth = mpGrowth,
                _STR_Growth = strGrowth,
                _DEF_Growth = defGrowth,
                _AGI_Growth = agiGrowth,
                _INT_Growth = intGrowth
            };
            return a;
        }

        public static Attributes Fuse(Attributes parentA, int parentALVL, Attributes parentB, int parentBLVL, Attributes childBase)
        {
            Attributes a = new()
            {
                _HP_Base = (parentA.HP(parentALVL) + parentB.HP(parentBLVL)) / 4,
                _MP_Base = (parentA.MP(parentALVL) + parentB.MP(parentBLVL)) / 4,
                _STR_Base = (parentA.STR(parentALVL) + parentB.STR(parentBLVL)) / 4,
                _DEF_Base = (parentA.DEF(parentALVL) + parentB.DEF(parentBLVL)) / 4,
                _AGI_Base = (parentA.AGI(parentALVL) + parentB.AGI(parentBLVL)) / 4,
                _INT_Base = (parentA.INT(parentALVL) + parentB.INT(parentBLVL)) / 4,
                _HP_Growth = ((parentA._HP_Growth + parentB._HP_Growth) / 4) + childBase._HP_Growth,
                _MP_Growth = ((parentA._MP_Growth + parentB._MP_Growth) / 4) + childBase._MP_Growth,
                _STR_Growth = ((parentA._STR_Growth + parentB._STR_Growth) / 4) + childBase._STR_Growth,
                _DEF_Growth = ((parentA._DEF_Growth + parentB._DEF_Growth) / 4) + childBase._DEF_Growth,
                _AGI_Growth = ((parentA._AGI_Growth + parentB._AGI_Growth) / 4) + childBase._AGI_Growth,
                _INT_Growth = ((parentA._INT_Growth + parentB._INT_Growth) / 4) + childBase._INT_Growth
            };
            return a;
        }
    }
    [Header("Unit Stats")]
    [SerializeField] private int _HP;
    [SerializeField] private int _MP;
    public int HP { get { return _HP; } }
    public int MP { get { return _MP; } }

    /// <summary>
    ///  Deals damage to the unit.
    /// </summary>
    /// <param name="dmg"></param>
    public void Damage(int dmg)
    {
        _HP -= dmg;
        if (_HP < 0) _HP = 0;
    }

    /// <summary>
    ///  Heals the unit.
    /// </summary>
    /// <param name="heal"></param>
    public void Heal(int heal)
    {
        _HP += heal;
        if (_HP > attributes.HP(_level)) _HP = attributes.HP(_level);
    }

    /// <summary>
    ///  Uses MP.
    /// </summary>
    /// <param name="mp"></param>
    /// <returns></returns>
    public bool UseMP(int mp)
    {
        if (_MP >= mp)
        {
            _MP -= mp;
            return true;
        }
        return false;
    }

    /// <summary>
    ///  Restores MP.
    /// </summary>
    /// <param name="mp"></param>
    public void RestoreMP(int mp)
    {
        _MP += mp;
        if (_MP > attributes.MP(_level)) _MP = attributes.MP(_level);
    }

    [Header("Unit Level")]
    [SerializeField] private int _level = 1; // Level of the unit
    [SerializeField] private int _currXP = 0; // Current XP of the unit
    public int Level { get { return _level; } } // Level of the unit
    public int CurrXP { get { return _currXP; } } // Current XP of the unit

    /// <summary>
    ///  Adds XP to the unit.
    /// </summary>
    /// <param name="xp"></param>
    /// <returns></returns>
    public bool AddXP(int xp)
    {

        if (_currXP + xp >= NextLevelXP(_level))
        {
            _level++;
            _skillPoints += SPgain(_level);
            _currXP += xp;
            while (_currXP >= NextLevelXP(_level))
            {
                _level++;
                _skillPoints += SPgain(_level);
            }
            return true;
        }
        else
        {
            _currXP += xp;
            return false;
        }
    }

    /// <summary>
    ///  Test function to add XP to the unit.
    /// </summary>
    /// <param name="xp"></param>
    public void XPTestFunc(int xp)
    {
        Debug.Log(AddXP(xp) ? "Level Up!" : "No Level Up");
    }


    [Header("Global Experience Settings")]
    public static float XPCurve = 1.5f; // The curve of the XP gain
    public static int firstLevelXP = 3; // The XP needed to reach level 2
    public static int maxLevel = 100; // The maximum level of the unit

    /// <summary>
    ///  Returns the XP needed to reach the next level.
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static int NextLevelXP(int level)
    {
        return Mathf.FloorToInt(firstLevelXP * Mathf.Pow(XPCurve, level - 1));
    }

    /// <summary>
    ///  Returns the SP points gained apon reaching the next level.
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public static int SPgain(int level)
    {
        if (level % 2 != 0)
        {
            return 0;
        }

        // Lvl 1 - 12 : 3sp
        // Lvl 13 - 21 : 4sp
        // Lvl 22 - 30 : 5sp
        // Lvl 31 - 39 : 6sp
        // Lvl 40 - 42 : 5sp
        // Lvl 43 - 45 : 4sp
        // Lvl 46 - 48 : 3sp
        // Lvl 49+ : 2sp

        if (level <= 12)
        {
            return 3;
        }
        else if (level <= 21)
        {
            return 4;
        }
        else if (level <= 30)
        {
            return 5;
        }
        else if (level <= 39)
        {
            return 6;
        }
        else if (level <= 42)
        {
            return 5;
        }
        else if (level <= 45)
        {
            return 4;
        }
        else if (level <= 48)
        {
            return 3;
        }
        else
        {
            return 2;
        }
    }


    [Header("Unit Skills")]
    [SerializeField] private List<SkillData> _skills = new List<SkillData>(); // List of skills the unit has
    [SerializeField] private int _skillPoints = 0; // Unassigned Skill points the unit has
    public List<SkillData> Skills { get { return _skills; } } // List of skills the unit has
    public int SkillPoints { get { return _skillPoints; } } // Unassigned Skill points the unit has

    /// <summary>
    ///  Invests unassigned skill points into the given skill.
    /// </summary>
    /// <param name="skillIndex"></param>
    /// <param name="points"></param>
    /// <returns></returns>
    public bool InvestSkillPoints(int skillIndex, int points)
    {
        if (points <= _skillPoints)
        {
            _skills[skillIndex].InvestPoints(points);
            _skillPoints -= points;
            return true;
        }
        return false;
    }

    /// <summary>
    ///  Gets all the unlocked abilities of the unit.
    /// </summary>
    /// <returns></returns>
    public List<Ability> UnlockedAbilities()
    {
        List<Ability> abilities = new List<Ability>();
        foreach (SkillData skill in _skills)
        {
            abilities.AddRange(skill.GetUnlockedAbilities());
        }
        return abilities;
    }

    ///Constructor
    public UnitData(UnitData from, int level)
    {
        unitName = from.unitName;
        unitSpecies = from.unitSpecies;
        UnitID = System.Guid.NewGuid().ToString();
        attributes = from.attributes;
        _level = level;
        _currXP = NextLevelXP(_level - 1);
        _HP = attributes.HP(_level);
        _MP = attributes.MP(_level);
        _skills = new List<SkillData>(from._skills);
        _skillPoints = 0;
        foreach (SkillData skill in _skills)
        {
            skill.ResetPoints();
        }
    }


    /// <summary>
    ///  Returns a copy of the unit data.
    /// </summary>
    /// <param name="baseUnit"></param>
    /// <returns></returns>
    public static UnitData CreateFromBase(UnitData baseUnit)
    {
        return new UnitData(baseUnit, 1);
    }

    public UnitData()
    {
        UnitID = System.Guid.NewGuid().ToString();
    }

    /// <summary>
    ///  A serializable class for storing the data of a unit.
    /// </summary>
    [System.Serializable]
    public class SerializedUnitData
    {
        public string unitName; // Name of the unit
        public string unitSpecies; // Species of the unit
        public string unitID; // ID of the unit
        public Attributes attributes; // Attributes of the unit
        public int level; // Level of the unit
        public int currXP; // Current XP of the unit
        public int currHP; // Current HP of the unit
        public int currMP;  // Current MP of the unit
        public int skillPoints; // Unassigned Skill points the unit has
        public List<SkillData.SerializedSkillData> skills = new List<SkillData.SerializedSkillData>(); // List of skills the unit has

        /// <summary>
        ///  Returns a serialized version of the unit data.
        /// </summary>
        /// <param name="unit"></param>
        public SerializedUnitData(UnitData unit)
        {
            unitName = unit.unitName;
            unitSpecies = unit.unitSpecies;
            unitID = unit.UnitID;
            attributes = unit.attributes;
            level = unit._level;
            currXP = unit._currXP;
            currHP = unit._HP;
            currMP = unit._MP;
            skillPoints = unit._skillPoints;

            foreach (SkillData skill in unit._skills)
            {
                skills.Add(new SkillData.SerializedSkillData(skill));
            }
        }

        /// <summary>
        ///  Returns a deserialized version of the unit data.
        /// </summary>
        /// <returns></returns>
        public UnitData Deserialise()
        {
            UnitData unit = new UnitData();
            unit.unitName = unitName;
            unit.unitSpecies = unitSpecies;
            unit.UnitID = unitID;
            unit.attributes = attributes;
            unit._level = level;
            unit._currXP = currXP;
            unit._HP = currHP;
            unit._MP = currMP;
            unit._skillPoints = skillPoints;
            foreach (SkillData.SerializedSkillData skill in skills)
            {
                unit._skills.Add(skill.GetSkillData());
            }
            return unit;
        }
    }



}
