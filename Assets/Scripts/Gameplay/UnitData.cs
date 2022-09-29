using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "GameData/Unit/Create Unit Data", order = 0)]
public class UnitData : ScriptableObject
{
    [Header("Unit Info")]
    public string unitName;

    [Header("Unit Attributes")]
    public Attributes attributes = new Attributes();
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
        /// Attribute Get Methods
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

    public void Damage(int dmg)
    {
        _HP -= dmg;
        if (_HP < 0) _HP = 0;
    }
    public void Heal(int heal)
    {
        _HP += heal;
        if (_HP > attributes.HP(_level)) _HP = attributes.HP(_level);
    }
    public bool UseMP(int mp)
    {
        if (_MP >= mp)
        {
            _MP -= mp;
            return true;
        }
        return false;
    }
    public void RestoreMP(int mp)
    {
        _MP += mp;
        if (_MP > attributes.MP(_level)) _MP = attributes.MP(_level);
    }

    [Header("Unit Level")]
    [SerializeField] private int _level = 1;
    [SerializeField] private int _currXP = 0;
    public int Level { get { return _level; } }
    public int CurrXP { get { return _currXP; } }
    public bool AddXP(int xp)
    {

        if (_currXP + xp >= NextLevelXP(_level))
        {
            _level++;
            _currXP += xp;
            return true;
        }
        else
        {
            _currXP += xp;
            return false;
        }
    }

    public void XPTestFunc(int xp)
    {
        Debug.Log(AddXP(xp) ? "Level Up!" : "No Level Up");
    }

    [Header("Global Experience Settings")]
    public static float XPCurve = 1.5f;
    public static int firstLevelXP = 3;
    public static int maxLevel = 100;
    public static int NextLevelXP(int level)
    {
        return Mathf.FloorToInt(firstLevelXP * Mathf.Pow(XPCurve, level - 1));
    }

    [Header("Unit Skills")]
    [SerializeField] private List<SkillData> _skills = new List<SkillData>();

    public List<SkillData> Skills { get { return _skills; } }

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
    public UnitData(UnitData baseUnit, int level)
    {
        unitName = baseUnit.unitName;
        attributes = baseUnit.attributes;
        _level = level;
        _HP = attributes.HP(_level);
        _MP = attributes.MP(_level);
        _skills = new List<SkillData>(baseUnit._skills);
    }

}
