using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitInfoMenu : MonoBehaviour
{
    [Header("Text Fields")]
    public TMP_Text unitName;
    public TMP_Text unitSpecies;
    public TMP_Text unitLVL;
    public TMP_Text unitEXP;
    [Header("Unit Stats")]
    public TMP_Text unitHealth;
    public TMP_Text unitMP;
    [Header("Unit Attributes")]
    public TMP_Text unitStr;
    public TMP_Text unitInt;
    public TMP_Text unitAgi;
    public TMP_Text unitDef;
    [Header("Unit Skills")]
    public TMP_Text unitSkills;
    public TMP_Text unitUnassignedSP;


    public void SetUnitInfo(UnitData unit)
    {
        unitName.text = unit.unitName;
        unitSpecies.text = unit.unitSpecies;
        unitLVL.text = "LVL: " + unit.Level.ToString();
        unitHealth.text = "HP: " + unit.attributes.HP(unit.Level).ToString();
        unitMP.text = "MP: " + unit.attributes.MP(unit.Level).ToString();
        unitStr.text = "STR: " + unit.attributes.STR(unit.Level).ToString();
        unitInt.text = "INT: " + unit.attributes.INT(unit.Level).ToString();
        unitAgi.text = "AGI: " + unit.attributes.AGI(unit.Level).ToString();
        unitDef.text = "DEF: " + unit.attributes.DEF(unit.Level).ToString();
        unitUnassignedSP.text = "Unassigned SP: " + unit.SkillPoints.ToString();
        unitSkills.text = "Skills: \n";
        foreach (SkillData skill in unit.Skills)
        {
            unitSkills.text += skill.Name + "\n";
        }
        unitEXP.text = "EXP: " + unit.CurrXP.ToString() + "/" + UnitData.NextLevelXP(unit.Level).ToString();

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
