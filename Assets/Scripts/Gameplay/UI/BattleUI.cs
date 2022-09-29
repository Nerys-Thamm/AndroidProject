using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private BattleManager _battleManager;

    [Header("Selection")]
    int selectedUnitIndex = 0;
    bool selectingUnit = false;
    BattleManager.BattleAction.ActionType selectedAction = BattleManager.BattleAction.ActionType.Attack;

    [Header("Action Creation")]
    int selectedUnit = 0;
    int selectedTarget = 0;
    Ability selectedAbility = null;
    BattleManager.BattleAction[] _actions = new BattleManager.BattleAction[3];
    public BattleManager.BattleAction[] Actions { get { return _actions; } }
    public void ClearActions() { _actions = new BattleManager.BattleAction[3]; }

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

    [SerializeField] private GameObject _unitSelectIndicator1, _unitSelectIndicator2, _unitSelectIndicator3;
    private void SelectUnit(int index)
    {
        selectedUnitIndex = index;
        selectingUnit = true;
        _unitSelectIndicator1.SetActive(index == 0);
        _unitSelectIndicator2.SetActive(index == 1);
        _unitSelectIndicator3.SetActive(index == 2);
    }
    private void DisableSelection()
    {
        selectingUnit = false;
        _unitSelectIndicator1.SetActive(false);
        _unitSelectIndicator2.SetActive(false);
        _unitSelectIndicator3.SetActive(false);
    }


    [Header("Enemy UI")]
    [SerializeField] private GameObject _enemyNamePanel;
    [SerializeField] private TMP_Text _enemyNameTextA, _enemyNameTextB, _enemyNameTextC;

    [Header("Player UI")]
    [SerializeField] private GameObject _allyNamePanel;
    [SerializeField] private GameObject _unitSelectMenu, _unitInfoMenu, _actionMenu, _abilityMenu, _mainMenu;
    [SerializeField] private Button _unitSelectButtonA, _unitSelectButtonB, _unitSelectButtonC;
    [SerializeField] private TMP_Text _allyNameTextA, _allyNameTextB, _allyNameTextC;
    [SerializeField] private GameObject _unitPanelPrefab;

    [Header("Action Menu")]
    [SerializeField] private Button _attackButton, _abilityButton, _defendButton;

    [Header("Ability Menu")]
    [SerializeField] private AbilityList _abilityList;

    [Header("Misc")]
    [SerializeField] private Animator _frontCamAnimator;

    public void SetEnemyNames(string[] names)
    {
        _enemyNameTextA.text = names[0];
        _enemyNameTextB.text = names[1];
        _enemyNameTextC.text = names[2];
    }
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

    public void SetAllyNames(string[] names)
    {
        _allyNameTextA.text = names[0];
        _allyNameTextB.text = names[1];
        _allyNameTextC.text = names[2];
    }

    public void CreateUnitInfoPanel(UnitStats unit)
    {
        GameObject panel = Instantiate(_unitPanelPrefab, _unitInfoMenu.transform);
        panel.GetComponent<UnitPanel>().unitStats = unit;
    }

    enum MenuState
    {
        Main,
        UnitSelect,
        Action,
        Ability,
        TargetSelect
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
            case MenuState.UnitSelect:
                SelectUnit(unitIndex);
                _unitSelectMenu.SetActive(false);
                selectedUnit = unitIndex;
                _actionMenu.SetActive(true);
                _actionMenu.GetComponent<ActionMenu>().SetCurrentActionText(GetCurrentActionText(unitIndex));
                _menuState = MenuState.Action;
                break;
            case MenuState.TargetSelect:
                _abilityMenu.SetActive(false);
                _mainMenu.SetActive(true);
                selectedTarget = unitIndex;
                bool targetAllies = (selectedAbility && (selectedAbility.Type == Ability.AbilityType.Heal || selectedAbility.Type == Ability.AbilityType.Buff));
                _actions[selectedUnit] = new BattleManager.BattleAction
                {
                    unitIndex = selectedUnit,
                    targetIndex = selectedTarget,
                    actionType = selectedAction,
                    ability = selectedAbility,
                    unit = _battleManager.battleField.playerCells[selectedUnit].unitStats,
                    target = targetAllies ? _battleManager.battleField.playerCells[selectedTarget].unitStats : _battleManager.battleField.enemyCells[selectedTarget].unitStats
                };
                DisableSelection();
                selectedAbility = null;
                _unitSelectMenu.SetActive(false);
                _menuState = MenuState.Main;
                break;
        }
    }
    private void SetUnitSelectButtonsInteractable(bool A, bool B, bool C)
    {
        _unitSelectButtonA.interactable = A;
        _unitSelectButtonB.interactable = B;
        _unitSelectButtonC.interactable = C;
    }

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
        _menuState = MenuState.Action;
        _unitSelectMenu.SetActive(false);
        _actionMenu.SetActive(true);
        _abilityMenu.SetActive(false);
    }

    void OnAttackBtnClick()
    {
        DisableSelection();
        _actionMenu.SetActive(false);
        selectedAction = BattleManager.BattleAction.ActionType.Attack;
        SelectEnemyMenu();
    }

    void OnAbilityBtnClick()
    {
        _actionMenu.SetActive(false);
        _abilityMenu.SetActive(true);
        _unitSelectMenu.SetActive(false);
        selectedAction = BattleManager.BattleAction.ActionType.Ability;
        _abilityList.PopulateList(_battleManager.battleField.playerCells[selectedUnit].unitStats);
        _menuState = MenuState.Ability;
    }

    void OnDefendBtnClick()
    {
        _actions[selectedUnit] = new BattleManager.BattleAction
        {
            unitIndex = selectedUnit,
            targetIndex = 0,
            actionType = BattleManager.BattleAction.ActionType.Defend,
            ability = null,
            unit = _battleManager.battleField.playerCells[selectedUnit].unitStats,
            target = _battleManager.battleField.playerCells[selectedUnit].unitStats
        };
        DisableSelection();
        _actionMenu.SetActive(false);
        _mainMenu.SetActive(true);
        _menuState = MenuState.Main;
    }


    //----------------------------------------------------------------------

    //----------------------------------------------------------------------
    // Ability Menu
    //
    public void SelectAbilityMenu()
    {
        _menuState = MenuState.Ability;
        selectedAction = BattleManager.BattleAction.ActionType.Ability;
        _unitSelectMenu.SetActive(false);
        _actionMenu.SetActive(false);
        _abilityMenu.SetActive(true);
    }

    void RefreshAbilityMenu()
    {
        _abilityMenu.SetActive(false);
        _abilityMenu.SetActive(true);
    }

    public void SelectAbility(Ability ability)
    {
        selectedAbility = ability;
    }

    public void ConfirmAbility()
    {
        _abilityMenu.SetActive(false);
        _unitSelectMenu.SetActive(true);
        if (selectedAbility.Type == Ability.AbilityType.Heal || selectedAbility.Type == Ability.AbilityType.Buff)
        {
            SelectAllyMenu();
        }
        else
        {
            SelectEnemyMenu();
        }
    }


    //----------------------------------------------------------------------

    private MenuState _menuState = MenuState.Main;

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
