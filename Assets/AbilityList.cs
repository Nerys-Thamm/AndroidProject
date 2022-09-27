using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityList : MonoBehaviour
{
    [SerializeField] BattleUI _battleUI;
    [SerializeField] RectTransform listContent;
    [SerializeField] TMP_Text abilityDescription;
    [SerializeField] TMP_Text abilityName;
    [SerializeField] TMP_Text abilityCost;
    [SerializeField] GameObject abilityButtonPrefab;
    [SerializeField] Button selectButton;
    List<Button> buttons = new List<Button>();

    public void PopulateList(UnitStats unit)
    {
        float height = 0;
        foreach (Button button in buttons)
        {
            Destroy(button.gameObject);
        }
        buttons.Clear();
        List<Ability> abilities = unit.GetAbilities();

        foreach (Ability ability in abilities)
        {
            Button button = Instantiate(abilityButtonPrefab, listContent).GetComponent<Button>();
            button.GetComponentInChildren<TMP_Text>().text = ability.Name;
            button.onClick.AddListener(() => _battleUI.SelectAbility(ability));
            button.onClick.AddListener(() => UpdateDescriptionField(ability, unit));
            button.interactable = (ability.MPCost <= unit.CurrMP);
            buttons.Add(button);
            height += button.GetComponent<RectTransform>().rect.height;
        }
        listContent.sizeDelta = new Vector2(listContent.sizeDelta.x, height);
    }

    void UpdateDescriptionField(Ability ability, UnitStats unit)
    {
        abilityDescription.text = ability.Description;
        abilityName.text = ability.Name;
        abilityCost.text = ability.MPCost.ToString() + " / " + unit.CurrMP.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Select and confirm the ability on button press
        selectButton.onClick.AddListener(() => _battleUI.ConfirmAbility());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
