using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// The skill menu for a unit.
/// </summary>
public class UnitSkillsMenu : MonoBehaviour
{
    // References
    public SquadSelectMenu squadSelectMenu;


    // Prefabs and list transforms
    public GameObject skillCellPrefab;
    public Transform skillCellParent;
    public GameObject skillUnlockPrefab;
    public Transform skillUnlockParent;

    // UI elements
    public TMP_Text skillName;
    public TMP_Text skillSP;
    public TMP_InputField assignSPinput;
    public Button assignSPbutton;

    // Current unit
    UnitData unit;

    // Current skill
    SkillData selectedSkill = null;

    /// <summary>
    ///  CHeck if the player has entered a valid SP amount.
    /// </summary>
    /// <param name="input"></param>
    void ValidateSPInput(string input)
    {
        int sp;
        if (int.TryParse(input, out sp))
        {
            if (sp > 0 && sp <= unit.SkillPoints)
            {
                assignSPbutton.interactable = true;
            }
            else
            {
                assignSPbutton.interactable = false;
            }
        }
        else
        {
            assignSPbutton.interactable = false;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        unit = squadSelectMenu.GetSelectedUnit();
        selectedSkill = unit.Skills[0];
        assignSPinput.onValueChanged.AddListener(ValidateSPInput);
        assignSPbutton.onClick.AddListener(AssignSkillPoints);
        PopulateSkills();
        UpdateSkillInfo();
    }

    void OnEnable()
    {
        unit = squadSelectMenu.GetSelectedUnit();
        selectedSkill = unit.Skills[0];
        PopulateSkills();
        UpdateSkillInfo();
    }

    /// <summary>
    /// Assign the skill points to the skill.
    /// </summary>
    void AssignSkillPoints()
    {
        int sp;
        if (int.TryParse(assignSPinput.text, out sp))
        {
            if (sp > 0 && sp <= unit.SkillPoints)
            {
                unit.InvestSkillPoints(unit.Skills.IndexOf(selectedSkill), sp);
                UpdateSkillInfo();
                squadSelectMenu.UpdateInfoMenu();
                squadSelectMenu.SaveMonsters();
            }
        }
    }

    /// <summary>
    /// Populate the skills list
    /// </summary>
    void PopulateSkills()
    {
        // Clear list
        foreach (Transform child in skillCellParent)
        {
            Destroy(child.gameObject);
        }

        // Populate list
        foreach (SkillData skill in unit.Skills)
        {
            GameObject cell = Instantiate(skillCellPrefab, skillCellParent);
            cell.GetComponentInChildren<TMP_Text>().text = skill.Name;
            cell.GetComponent<Button>().onClick.AddListener(() => { selectedSkill = skill; UpdateSkillInfo(); });
        }
    }

    /// <summary>
    /// Update the skill info 
    /// </summary>
    void UpdateSkillInfo()
    {
        foreach (Transform child in skillUnlockParent)
        {
            Destroy(child.gameObject);
        }
        if (selectedSkill == null)
        {
            skillName.text = "None";
            skillSP.text = "0";
            assignSPinput.text = "";
            assignSPinput.interactable = false;
            assignSPbutton.interactable = false;
            return;
        }
        else
        {
            skillName.text = selectedSkill.Name;
            skillSP.text = selectedSkill.SkillPoints.ToString();
            assignSPinput.text = "";
            assignSPinput.interactable = true;
            assignSPbutton.interactable = false;
        }

        foreach (SkillData.UnlockInfo unlock in selectedSkill.GetUnlockInfo())
        {
            GameObject unlockCell = Instantiate(skillUnlockPrefab, skillUnlockParent);
            unlockCell.GetComponentInChildren<SkillUnlockUI>().SetContent(unlock.unlockName, unlock.unlockRequirement, unlock.isUnlocked);
        }
    }

}
