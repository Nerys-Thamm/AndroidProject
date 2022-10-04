using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SquadSelectMenu : MonoBehaviour
{
    // References
    public GameObject infoMenu;
    public UnitInfoMenu unitInfoMenu;
    public StorageMonsterSelect storageMonsterSelect;

    //Squad UI;
    public Button SquadA;
    public Button SquadB;
    public Button SquadC;

    //Squad Member Menu UI
    public Button ChangeMonsterButton;
    public Button RenameMonsterButton;
    public Button SkillsMenuButton;

    //Rename Menu UI
    public GameObject RenameMenu;
    public UnitRenameMenu unitRenameMenu;

    //Squad Members
    public UnitData SquadAMember;
    public UnitData SquadBMember;
    public UnitData SquadCMember;

    // Menu variables
    int selectedUnit = 0;

    public UnitData GetSelectedUnit()
    {
        switch (selectedUnit)
        {
            case 0:
                return SquadAMember;
            case 1:
                return SquadBMember;
            case 2:
                return SquadCMember;
            default:
                return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SaveSerialisation.Instance.Load();
        SquadAMember = SaveSerialisation.Instance.PartyMonsterA;
        SquadBMember = SaveSerialisation.Instance.PartyMonsterB;
        SquadCMember = SaveSerialisation.Instance.PartyMonsterC;
        SquadA.GetComponentInChildren<Text>().text = SquadAMember?.unitName ?? "Empty";
        SquadB.GetComponentInChildren<Text>().text = SquadBMember?.unitName ?? "Empty";
        SquadC.GetComponentInChildren<Text>().text = SquadCMember?.unitName ?? "Empty";

        //Add Listeners to Buttons

        //Selecting Squad Members
        SquadA.onClick.AddListener(() => { selectedUnit = 0; UpdateInfoMenu(); });
        SquadB.onClick.AddListener(() => { selectedUnit = 1; UpdateInfoMenu(); });
        SquadC.onClick.AddListener(() => { selectedUnit = 2; UpdateInfoMenu(); });

        //Change Monster Button
        ChangeMonsterButton.onClick.AddListener(() => { storageMonsterSelect.GetSelection(SwapCurrentMonster); });

        //Rename Monster Button
        RenameMonsterButton.onClick.AddListener(() =>
        {
            RenameMenu.SetActive(true);
            string currName = "";
            switch (selectedUnit)
            {
                case 0:
                    currName = SquadAMember.unitName;
                    break;
                case 1:
                    currName = SquadBMember.unitName;
                    break;
                case 2:
                    currName = SquadCMember.unitName;
                    break;
            }
            unitRenameMenu.RenameUnit(currName, RenameCurrentMonster);
        });


        //Initialise Info Menu
        selectedUnit = 0;
        UpdateInfoMenu();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void RenameCurrentMonster(string newName)
    {
        switch (selectedUnit)
        {
            case 0:
                SquadAMember.unitName = newName;
                break;
            case 1:
                SquadBMember.unitName = newName;
                break;
            case 2:
                SquadCMember.unitName = newName;
                break;
        }
        UpdateInfoMenu();
        SaveSerialisation.Instance.SaveMonsters(SquadAMember, SquadBMember, SquadCMember);
        SaveSerialisation.Instance.Save();
    }

    public void SaveMonsters()
    {
        SaveSerialisation.Instance.SaveMonsters(SquadAMember, SquadBMember, SquadCMember);
        SaveSerialisation.Instance.Save();
    }

    void SwapCurrentMonster(UnitData newMonster)
    {
        UnitData oldMonster = null;
        switch (selectedUnit)
        {
            case 0:
                oldMonster = SquadAMember;
                SquadAMember = newMonster;
                SquadA.GetComponentInChildren<Text>().text = SquadAMember?.unitName ?? "Empty";
                break;
            case 1:
                oldMonster = SquadBMember;
                SquadBMember = newMonster;
                SquadB.GetComponentInChildren<Text>().text = SquadBMember?.unitName ?? "Empty";
                break;
            case 2:
                oldMonster = SquadCMember;
                SquadCMember = newMonster;
                SquadC.GetComponentInChildren<Text>().text = SquadCMember?.unitName ?? "Empty";
                break;
        }
        List<UnitData> storage = SaveSerialisation.Instance.PlayerCreatureStorage;
        if (oldMonster != null)
        {
            storage.Add(oldMonster);
        }
        if (newMonster != null)
        {
            storage.RemoveAll(x => x.UnitID == newMonster.UnitID);
        }
        SaveSerialisation.Instance.PlayerCreatureStorage = storage;
        SaveSerialisation.Instance.SaveMonsters(SquadAMember, SquadBMember, SquadCMember);
        SaveSerialisation.Instance.Save();
        UpdateInfoMenu();

    }

    public void UpdateInfoMenu()
    {
        SquadA.GetComponentInChildren<Text>().text = SquadAMember?.unitName ?? "Empty";
        SquadB.GetComponentInChildren<Text>().text = SquadBMember?.unitName ?? "Empty";
        SquadC.GetComponentInChildren<Text>().text = SquadCMember?.unitName ?? "Empty";
        switch (selectedUnit)
        {
            case 0:
                if (SquadAMember != null)
                {
                    unitInfoMenu.SetUnitInfo(SquadAMember);
                    infoMenu.SetActive(true);
                    RenameMonsterButton.interactable = true;
                    SkillsMenuButton.interactable = true;
                }
                else
                {
                    infoMenu.SetActive(false);
                    RenameMonsterButton.interactable = false;
                    SkillsMenuButton.interactable = false;
                }
                break;
            case 1:
                if (SquadBMember != null)
                {
                    unitInfoMenu.SetUnitInfo(SquadBMember);
                    infoMenu.SetActive(true);
                    RenameMonsterButton.interactable = true;
                    SkillsMenuButton.interactable = true;
                }
                else
                {
                    infoMenu.SetActive(false);
                    RenameMonsterButton.interactable = false;
                    SkillsMenuButton.interactable = false;

                }
                break;
            case 2:
                if (SquadCMember != null)
                {
                    unitInfoMenu.SetUnitInfo(SquadCMember);
                    infoMenu.SetActive(true);
                    RenameMonsterButton.interactable = true;
                    SkillsMenuButton.interactable = true;
                }
                else
                {
                    infoMenu.SetActive(false);
                    RenameMonsterButton.interactable = false;
                    SkillsMenuButton.interactable = false;
                }
                break;
        }

    }
}
