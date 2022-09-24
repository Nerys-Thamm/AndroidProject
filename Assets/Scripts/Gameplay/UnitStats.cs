using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    //Data
    [SerializeField] private UnitData _unitData;

    public void SetUnitData(UnitData unitData)
    {
        _unitData = unitData;
    }

    //Stats
    public int MaxHP { get { return _unitData.attributes.HP(_unitData.Level); } }
    public int CurrHP { get { return _unitData.HP; } }
    public int MaxMP { get { return _unitData.attributes.MP(_unitData.Level); } }
    public int CurrMP { get { return _unitData.MP; } }
    public int STR { get { return Mathf.CeilToInt(_unitData.attributes.STR(_unitData.Level) * _buffModTable[_buffModSTR + 2]); } }
    public int DEF { get { return Mathf.CeilToInt(_unitData.attributes.DEF(_unitData.Level) * _buffModTable[_buffModDEF + 2]); } }
    public int AGI { get { return Mathf.CeilToInt(_unitData.attributes.AGI(_unitData.Level) * _buffModTable[_buffModAGI + 2]); } }
    public int INT { get { return Mathf.CeilToInt(_unitData.attributes.INT(_unitData.Level) * _buffModTable[_buffModINT + 2]); } }

    public int Level { get { return _unitData.Level; } }

    //Stat Modifiers (from equipment) (TODO)

    //Stat Modifiers (from buffs/debuffs) (Range -2 to 2)
    [Header("Stat Modifiers")]
    [SerializeField, Range(-2, 2)] private int _buffModSTR = 0;
    [SerializeField, Range(-2, 2)] private int _buffModDEF = 0;
    [SerializeField, Range(-2, 2)] private int _buffModAGI = 0;
    [SerializeField, Range(-2, 2)] private int _buffModINT = 0;

    private float[] _buffModTable = new float[] { 0.6f, 0.9f, 1f, 1.1f, 1.4f };



    //Combat Methods
    public void TakeDamage(int damage)
    {
        _unitData.Damage(damage);
    }

    public void Heal(int heal)
    {
        _unitData.Heal(heal);
    }

    public bool UseMP(int mp)
    {
        return _unitData.UseMP(mp);
    }
    public void RestoreMP(int mp)
    {
        _unitData.RestoreMP(mp);
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
