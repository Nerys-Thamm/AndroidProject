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

    [Header("Misc")]
    [SerializeField] private Animator _frontCamAnimator;

    public void SetEnemyNames(string[] names)
    {
        _enemyNameTextA.text = names[0];
        _enemyNameTextB.text = names[1];
        _enemyNameTextC.text = names[2];
    }

    enum MenuState
    {
        Main,
        UnitSelect,
        Action,
        Ability,
        TargetSelect
    }

    private void HandleUnitSelectionButtonPress(int unitIndex)
    {
        SelectUnit(unitIndex);
        switch (_menuState)
        {
            case MenuState.UnitSelect:
                _unitSelectMenu.SetActive(false);
                _actionMenu.SetActive(true);
                _menuState = MenuState.Action;
                break;
            case MenuState.TargetSelect:
                _abilityMenu.SetActive(false);
                _mainMenu.SetActive(true);
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
        _unitSelectMenu.SetActive(true);
        _unitInfoMenu.SetActive(false);
        _actionMenu.SetActive(false);
        _abilityMenu.SetActive(false);
    }

    public void SelectAllyMenu()
    {
        if (_menuState == MenuState.Main)
        {
            _menuState = MenuState.UnitSelect;
        }
        else
        {
            _menuState = MenuState.TargetSelect;
        }
        SetUnitSelectButtonsInteractable(_battleManager.CanSelectUnit(0, false), _battleManager.CanSelectUnit(1, false), _battleManager.CanSelectUnit(2, false));
        _frontCamAnimator.SetBool("ShowingAllies", true);
        _enemyNamePanel.SetActive(false);
        _allyNamePanel.SetActive(true);
        _unitSelectMenu.SetActive(true);
        _unitInfoMenu.SetActive(true);
        _actionMenu.SetActive(false);
        _abilityMenu.SetActive(false);
    }

    public void SelectActionMenu()
    {
        _menuState = MenuState.Action;
        _unitSelectMenu.SetActive(false);
        _unitInfoMenu.SetActive(false);
        _actionMenu.SetActive(true);
        _abilityMenu.SetActive(false);
    }

    public void SelectAbilityMenu()
    {
        _menuState = MenuState.Ability;
        _unitSelectMenu.SetActive(false);
        _unitInfoMenu.SetActive(false);
        _actionMenu.SetActive(false);
        _abilityMenu.SetActive(true);
    }

    private MenuState _menuState = MenuState.Main;

    // Start is called before the first frame update
    void Start()
    {
        _unitSelectButtonA.onClick.AddListener(() => HandleUnitSelectionButtonPress(0));
        _unitSelectButtonB.onClick.AddListener(() => HandleUnitSelectionButtonPress(1));
        _unitSelectButtonC.onClick.AddListener(() => HandleUnitSelectionButtonPress(2));
    }

    // Update is called once per frame
    void Update()
    {

    }

}
