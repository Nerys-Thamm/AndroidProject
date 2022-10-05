using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  This class is used to store the stats of a unit.
/// </summary>
public class UnitStats : MonoBehaviour
{
    //Data
    [SerializeField] private UnitData _unitData;

    /// <summary>
    ///  Set the unit data.
    /// </summary>
    /// <param name="unitData"></param>
    public void SetUnitData(UnitData unitData)
    {
        _unitData = unitData;
    }

    /// <summary>
    /// Set the Unit Data to a new Unit Data of a specified level.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="level"></param>
    public void SetNewUnitData(UnitData data, int level)
    {
        _unitData = new UnitData(data, level);
    }

    public string Name { get { return _unitData.unitName; } } /// > The name of the unit.

    //Stats
    public int MaxHP { get { return _unitData.attributes.HP(_unitData.Level); } } /// > The maximum HP of the unit.
    public int CurrHP { get { return _unitData.HP; } } /// > The current HP of the unit.
    public int MaxMP { get { return _unitData.attributes.MP(_unitData.Level); } } /// > The maximum MP of the unit.
    public int CurrMP { get { return _unitData.MP; } }  /// > The current MP of the unit.
    public int STR { get { return Mathf.CeilToInt(_unitData.attributes.STR(_unitData.Level) * _buffModTable[_buffModSTR + 2]); } } /// > The STR of the unit, modified by buffs.
    public int DEF { get { return Mathf.CeilToInt(_unitData.attributes.DEF(_unitData.Level) * _buffModTable[_buffModDEF + 2]); } } /// > The DEF of the unit, modified by buffs.
    public int AGI { get { return Mathf.CeilToInt(_unitData.attributes.AGI(_unitData.Level) * _buffModTable[_buffModAGI + 2]); } } /// > The AGI of the unit, modified by buffs.
    public int INT { get { return Mathf.CeilToInt(_unitData.attributes.INT(_unitData.Level) * _buffModTable[_buffModINT + 2]); } } /// > The INT of the unit, modified by buffs.

    public int Level { get { return _unitData.Level; } } /// > The level of the unit. <summary> 

                                                         /// <summary>
                                                         /// Add XP to the unit.
                                                         /// </summary>
                                                         /// <param name="xp"></param>
                                                         /// <returns></returns>
    public bool AddXP(int xp)
    {
        return _unitData.AddXP(xp);
    }



    //Stat Modifiers (from buffs/debuffs) (Range -2 to 2)
    [Header("Stat Modifiers")]
    [SerializeField, Range(-2, 2)] private int _buffModSTR = 0; /// > The STR modifier.
    [SerializeField, Range(-2, 2)] private int _buffModDEF = 0; /// > The DEF modifier.
    [SerializeField, Range(-2, 2)] private int _buffModAGI = 0; /// > The AGI modifier.
    [SerializeField, Range(-2, 2)] private int _buffModINT = 0; /// > The INT modifier.

    private float[] _buffModTable = new float[] { 0.6f, 0.9f, 1f, 1.1f, 1.4f }; /// > The table used to calculate the stat modifiers.



    //Combat Methods

    /// <summary>
    ///  Deal damage to the unit.
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(int damage)
    {
        _unitData.Damage(damage);
    }

    /// <summary>
    ///  Heal the unit.
    /// </summary>
    /// <param name="heal"></param>
    public void Heal(int heal)
    {
        _unitData.Heal(heal);
    }

    /// <summary>
    ///  Use MP.
    /// </summary>
    /// <param name="mp"></param>
    /// <returns></returns>
    public bool UseMP(int mp)
    {
        return _unitData.UseMP(mp);
    }

    /// <summary>
    ///  Restore MP.
    /// </summary>
    /// <param name="mp"></param>
    public void RestoreMP(int mp)
    {
        _unitData.RestoreMP(mp);
    }

    /// <summary>
    ///  Apply a buff to the unit.
    /// </summary>
    /// <param name="effect"></param>
    public void ApplyBuff(Ability.EffectMask effect)
    {
        if ((effect & Ability.EffectMask.STR) != 0)
        {
            _buffModSTR++;
            if (_buffModSTR > 2) _buffModSTR = 2;
        }
        if ((effect & Ability.EffectMask.DEF) != 0)
        {
            _buffModDEF++;
            if (_buffModDEF > 2) _buffModDEF = 2;
        }
        if ((effect & Ability.EffectMask.AGI) != 0)
        {
            _buffModAGI++;
            if (_buffModAGI > 2) _buffModAGI = 2;
        }
        if ((effect & Ability.EffectMask.INT) != 0)
        {
            _buffModINT++;
            if (_buffModINT > 2) _buffModINT = 2;
        }
    }

    /// <summary>
    ///  Apply a debuff to the unit.
    /// </summary>
    /// <param name="effect"></param>
    public void ApplyDebuff(Ability.EffectMask effect)
    {
        if ((effect & Ability.EffectMask.STR) != 0)
        {
            _buffModSTR--;
            if (_buffModSTR < -2) _buffModSTR = -2;
        }
        if ((effect & Ability.EffectMask.DEF) != 0)
        {
            _buffModDEF--;
            if (_buffModDEF < -2) _buffModDEF = -2;
        }
        if ((effect & Ability.EffectMask.AGI) != 0)
        {
            _buffModAGI--;
            if (_buffModAGI < -2) _buffModAGI = -2;
        }
        if ((effect & Ability.EffectMask.INT) != 0)
        {
            _buffModINT--;
            if (_buffModINT < -2) _buffModINT = -2;
        }
    }

    /// <summary>
    ///  Get a list of all unlocked abilities.
    /// </summary>
    /// <returns></returns>
    public List<Ability> GetAbilities()
    {
        return _unitData.UnlockedAbilities();
    }
}
