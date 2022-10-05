using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private BattleManager _battleManager; // Reference to the battle manager

    [Header("Selection")]
    int selectedUnitIndex = 0; // Index of the selected unit
    bool selectingUnit = false; // Whether the player is selecting a unit
    BattleManager.BattleAction.ActionType selectedAction = BattleManager.BattleAction.ActionType.Attack; // The selected action

    [Header("Action Creation")]
    int selectedUnit = 0; // Index of the selected unit
    int selectedTarget = 0; // Index of the selected target
    Ability selectedAbility = null; // The selected ability
    BattleManager.BattleAction[] _actions = new BattleManager.BattleAction[3]; // The actions to be performed
    public BattleManager.BattleAction[] Actions { get { return _actions; } } // The actions to be performed
    public void ClearActions() { _actions = new BattleManager.BattleAction[3]; } // Clears the actions

    /// <summary>
    ///  Gets a text representation of the action, for the UI.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public string GetCurrentActionText(int index)
    {
        if (_actions[index].unit != null)
        {
            string actionText = "";
            if (_actions[index].actionType == BattleManager.BattleAction.ActionType.Attack)
            {
                actionText = "Attack";
                actionText += " --> " + _actions[index].target.Name;
            }
            else if (_actions[index].actionType == BattleManager.BattleAction.ActionType.Ability)
            {
                actionText = _actions[index].ability.Name;
                actionText += " --> " + _actions[index].target.Name;
            }
            else if (_actions[index].actionType == BattleManager.BattleAction.ActionType.Defend)
            {
                actionText = "Defend";
            }
            return actionText;
        }
        else
        {
            return "None";
        }


    }


    [SerializeField] private GameObject _unitSelectIndicator1, _unitSelectIndicator2, _unitSelectIndicator3; // The unit select indicators

    /// <summary>
    ///  Selects the next unit.
    /// </summary>
    /// <param name="index"></param>
    private void SelectUnit(int index)
    {
        selectedUnitIndex = index;
        selectingUnit = true;
        _unitSelectIndicator1.SetActive(index == 0);
        _unitSelectIndicator2.SetActive(index == 1);
        _unitSelectIndicator3.SetActive(index == 2);
    }

    /// <summary>
    ///  Deselects all units.
    /// </summary>
    private void DisableSelection()
    {
        selectingUnit = false;
        _unitSelectIndicator1.SetActive(false);
        _unitSelectIndicator2.SetActive(false);
        _unitSelectIndicator3.SetActive(false);
    }


    [Header("Enemy UI")]
    [SerializeField] private GameObject _enemyNamePanel; // The enemy name panel
    [SerializeField] private TMP_Text _enemyNameTextA, _enemyNameTextB, _enemyNameTextC; // The enemy name text

    [Header("Player UI")]
    [SerializeField] private GameObject _allyNamePanel; // The ally name panel
    [SerializeField] private GameObject _unitSelectMenu, _unitInfoMenu, _actionMenu, _abilityMenu, _mainMenu; // The menus
    [SerializeField] private Button _unitSelectButtonA, _unitSelectButtonB, _unitSelectButtonC; // The unit select buttons
    [SerializeField] private TMP_Text _allyNameTextA, _allyNameTextB, _allyNameTextC; // The ally name text
    [SerializeField] private GameObject _unitPanelPrefab; // The unit panel prefab

    [Header("Action Menu")]
    [SerializeField] private Button _attackButton, _abilityButton, _defendButton; // The action buttons

    [Header("Ability Menu")]
    [SerializeField] private AbilityList _abilityList; // The ability list

    [Header("Misc")]
    [SerializeField] private Animator _frontCamAnimator; // The front camera animator

    /// <summary>
    ///  Sets the enemy names in the UI.
    /// </summary>
    /// <param name="names"></param>
    public void SetEnemyNames(string[] names)
    {
        _enemyNameTextA.text = names[0];
        _enemyNameTextB.text = names[1];
        _enemyNameTextC.text = names[2];
    }

    /// <summary>
    ///  Updates the UI to reflect dead enemies.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="enemy"></param>
    public void SetDead(int index, bool enemy)
    {
        if (enemy)
        {
            if (index == 0)
            {
                _enemyNameTextA.text += " (Dead)";
            }
            else if (index == 1)
            {
                _enemyNameTextB.text += " (Dead)";
            }
            else if (index == 2)
            {
                _enemyNameTextC.text += " (Dead)";
            }
        }
        else
        {
            if (index == 0)
            {
                _allyNameTextA.text += " (Dead)";
            }
            else if (index == 1)
            {
                _allyNameTextB.text += " (Dead)";
            }
            else if (index == 2)
            {
                _allyNameTextC.text += " (Dead)";
            }
        }

    }


    /// <summary>
    ///  Sets the ally names in the UI.
    /// </summary>
    /// <param name="names"></param>
    public void SetAllyNames(string[] names)
    {
        _allyNameTextA.text = names[0];
        _allyNameTextB.text = names[1];
        _allyNameTextC.text = names[2];
    }

    /// <summary>
    ///  Creates an InfoPanel for a unit.
    /// </summary>
    /// <param name="unit"></param>
    public void CreateUnitInfoPanel(UnitStats unit)
    {
        GameObject panel = Instantiate(_unitPanelPrefab, _unitInfoMenu.transform);
        panel.GetComponent<UnitPanel>().unitStats = unit;
    }

    /// <summary>
    ///  Enum for the current state of the UI.
    /// </summary>
    enum MenuState
    {
        Main, /// > The main menu
        UnitSelect, /// > The unit select menu
        Action, /// > The action menu
        Ability, /// > The ability menu
        TargetSelect /// > The target select menu
    }

    ///-------------------------------------------------------------------------------
    /// Main Menu
    /// 

    public void SetMainMenuActive(bool active)
    {
        _mainMenu.SetActive(active);
    }

    //--------------------------------------------------------------------------------
    // Unit Select Menu
    //
    private void HandleUnitSelectionButtonPress(int unitIndex)
    {

        switch (_menuState)
        {
            case MenuState.UnitSelect: // If we're in the unit select menu
                SelectUnit(unitIndex); // Select the unit
                _unitSelectMenu.SetActive(false); // Disable the unit select menu
                selectedUnit = unitIndex; // Set the selected unit
                SelectActionMenu(); // Select the action menu
                _actionMenu.GetComponent<ActionMenu>().SetCurrentActionText(GetCurrentActionText(unitIndex)); // Set the current action text
                _menuState = MenuState.Action; // Set the menu state to action
                break;
            case MenuState.TargetSelect: // If we're in the target select menu
                _abilityMenu.SetActive(false); // Disable the ability menu
                _mainMenu.SetActive(true); // Enable the main menu
                selectedTarget = unitIndex; // Set the selected target
                bool targetAllies = (selectedAbility && (selectedAbility.Type == Ability.AbilityType.Heal || selectedAbility.Type == Ability.AbilityType.Buff)); // Check if the ability targets allies
                _actions[selectedUnit] = new BattleManager.BattleAction
                {
                    unitIndex = selectedUnit, // Set the unit index
                    targetIndex = selectedTarget, // Set the target index
                    actionType = selectedAction, // Set the action type
                    ability = selectedAbility, // Set the ability
                    unit = _battleManager.battleField.playerCells[selectedUnit].unitStats, // Set the unit
                    target = targetAllies ? _battleManager.battleField.playerCells[selectedTarget].unitStats : _battleManager.battleField.enemyCells[selectedTarget].unitStats // Set the target
                };
                DisableSelection(); // Disable the selection
                selectedAbility = null; // Set the selected ability to null
                _unitSelectMenu.SetActive(false); // Disable the unit select menu
                _menuState = MenuState.Main; // Set the menu state to main
                break;
        }
    }

    /// <summary>
    ///  Sets the interactable state of the unit select buttons.
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <param name="C"></param>
    private void SetUnitSelectButtonsInteractable(bool A, bool B, bool C)
    {
        _unitSelectButtonA.interactable = A;
        _unitSelectButtonB.interactable = B;
        _unitSelectButtonC.interactable = C;
    }

    /// <summary>
    ///  Opens the enemy unit select menu.
    /// </summary>
    public void SelectEnemyMenu()
    {
        SetUnitSelectButtonsInteractable(_battleManager.CanSelectUnit(0, true), _battleManager.CanSelectUnit(1, true), _battleManager.CanSelectUnit(2, true));
        _menuState = MenuState.TargetSelect;
        _frontCamAnimator.SetBool("ShowingAllies", false);
        _enemyNamePanel.SetActive(true);
        _allyNamePanel.SetActive(false);
        _unitSelectMenu.SetActive(true);
        _actionMenu.SetActive(false);
        _abilityMenu.SetActive(false);
    }

    /// <summary>
    ///  Opens the ally unit select menu.
    /// </summary>
    public void SelectAllyMenu()
    {
        SetUnitSelectButtonsInteractable(_battleManager.CanSelectUnit(0, false), _battleManager.CanSelectUnit(1, false), _battleManager.CanSelectUnit(2, false));
        _menuState = MenuState.TargetSelect;
        _frontCamAnimator.SetBool("ShowingAllies", true);
        _allyNamePanel.SetActive(true);
        _enemyNamePanel.SetActive(false);
        _unitSelectMenu.SetActive(true);
        _actionMenu.SetActive(false);
        _abilityMenu.SetActive(false);
    }

    /// <summary>
    ///  Opens the unit select menu.
    /// </summary>
    public void SelectUnitMenu()
    {

        _menuState = MenuState.UnitSelect;

        SetUnitSelectButtonsInteractable(_battleManager.CanSelectUnit(0, false), _battleManager.CanSelectUnit(1, false), _battleManager.CanSelectUnit(2, false));
        _frontCamAnimator.SetBool("ShowingAllies", true);
        _enemyNamePanel.SetActive(false);
        _allyNamePanel.SetActive(true);
        _unitSelectMenu.SetActive(true);
        _unitInfoMenu.SetActive(true);
        _actionMenu.SetActive(false);
        _abilityMenu.SetActive(false);
    }
    //----------------------------------------------------------------------



    //----------------------------------------------------------------------
    // Action Menu
    //

    public void SelectActionMenu()
    {
        _menuState = MenuState.Action; // Set the menu state to action
        _unitSelectMenu.SetActive(false); // Disable the unit select menu
        _actionMenu.SetActive(true); // Enable the action menu
        _abilityMenu.SetActive(false); // Disable the ability menu
        //Check if unit has any abilities
        if (_battleManager.battleField.playerCells[selectedUnit].unitStats.GetAbilities().Count > 0)
        {
            _abilityButton.interactable = true;
        }
        else
        {
            _abilityButton.interactable = false;
        }
    }

    /// <summary>
    ///  Listens for the attack button press.
    /// </summary>
    void OnAttackBtnClick()
    {
        DisableSelection(); // Disable the selection
        _actionMenu.SetActive(false); // Disable the action menu
        selectedAction = BattleManager.BattleAction.ActionType.Attack; // Set the selected action to attack
        SelectEnemyMenu(); // Select the enemy menu
    }

    /// <summary>
    ///  Listens for the ability button press.
    /// </summary>
    void OnAbilityBtnClick()
    {
        _actionMenu.SetActive(false); // Disable the action menu
        _abilityMenu.SetActive(true); // Enable the ability menu
        _unitSelectMenu.SetActive(false); // Disable the unit select menu
        selectedAction = BattleManager.BattleAction.ActionType.Ability; // Set the selected action to ability
        _abilityList.PopulateList(_battleManager.battleField.playerCells[selectedUnit].unitStats); // Populate the ability list
        _menuState = MenuState.Ability; // Set the menu state to ability
    }

    /// <summary>
    ///  Listens for the defend button press.
    /// </summary>
    void OnDefendBtnClick()
    {
        _actions[selectedUnit] = new BattleManager.BattleAction
        {
            unitIndex = selectedUnit, // Set the unit index
            targetIndex = 0, // Set the target index
            actionType = BattleManager.BattleAction.ActionType.Defend, // Set the action type
            ability = null, // Set the ability
            unit = _battleManager.battleField.playerCells[selectedUnit].unitStats, // Set the unit
            target = _battleManager.battleField.playerCells[selectedUnit].unitStats // Set the target
        };
        DisableSelection(); // Disable the selection
        _actionMenu.SetActive(false); // Disable the action menu
        _mainMenu.SetActive(true); // Enable the main menu
        _menuState = MenuState.Main;    // Set the menu state to main
    }


    //----------------------------------------------------------------------

    //----------------------------------------------------------------------
    // Ability Menu
    //

    /// <summary>
    ///  Opens the ability menu.
    /// </summary>
    public void SelectAbilityMenu()
    {
        _menuState = MenuState.Ability; // Set the menu state to ability
        selectedAction = BattleManager.BattleAction.ActionType.Ability; // Set the selected action to ability
        _unitSelectMenu.SetActive(false); // Disable the unit select menu
        _actionMenu.SetActive(false); // Disable the action menu
        _abilityMenu.SetActive(true); // Enable the ability menu
    }

    /// <summary>
    ///  Refreshes the ability menu.
    /// </summary>
    void RefreshAbilityMenu()
    {
        _abilityMenu.SetActive(false);
        _abilityMenu.SetActive(true);
    }

    /// <summary>
    ///  Selects the ability.
    /// </summary>
    /// <param name="ability"></param>
    public void SelectAbility(Ability ability)
    {
        selectedAbility = ability;
    }

    /// <summary>
    ///  Confirms an ability selection.
    /// </summary>
    public void ConfirmAbility()
    {
        _abilityMenu.SetActive(false); // Disable the ability menu
        _unitSelectMenu.SetActive(true); // Enable the unit select menu
        if (selectedAbility.Type == Ability.AbilityType.Heal || selectedAbility.Type == Ability.AbilityType.Buff) // If the ability is a heal or buff
        {
            SelectAllyMenu(); // Select the ally menu
        }
        else // If the ability is not a heal or buff
        {
            SelectEnemyMenu(); // Select the enemy menu
        }
    }


    //----------------------------------------------------------------------

    private MenuState _menuState = MenuState.Main; // The current menu state

    // Start is called before the first frame update
    void Start()
    {
        //----------------------------------------------------------------------
        // Setup Button Listeners
        //

        // Unit Select Button Listeners
        _unitSelectButtonA.onClick.AddListener(() => HandleUnitSelectionButtonPress(0));
        _unitSelectButtonB.onClick.AddListener(() => HandleUnitSelectionButtonPress(1));
        _unitSelectButtonC.onClick.AddListener(() => HandleUnitSelectionButtonPress(2));

        // Action Menu Button Listeners
        _attackButton.onClick.AddListener(OnAttackBtnClick);
        _abilityButton.onClick.AddListener(OnAbilityBtnClick);
        _defendButton.onClick.AddListener(OnDefendBtnClick);

        //----------------------------------------------------------------------
    }

    // Update is called once per frame
    void Update()
    {

    }

}
