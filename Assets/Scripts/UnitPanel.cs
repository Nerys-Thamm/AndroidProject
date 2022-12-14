using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// A panel showing details of a unit.
/// </summary>
public class UnitPanel : MonoBehaviour
{
    public UnitStats unitStats;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text hpText;
    [SerializeField] TMP_Text mpText;
    [SerializeField] Image hpBar;
    [SerializeField] Image mpBar;


    // Update is called once per frame
    void Update()
    {
        if (unitStats != null)
        {
            nameText.text = unitStats.Name;
            hpText.text = "HP: " + unitStats.CurrHP + "/" + unitStats.MaxHP;
            mpText.text = "MP: " + unitStats.CurrMP + "/" + unitStats.MaxMP;
            hpBar.fillAmount = (float)unitStats.CurrHP / unitStats.MaxHP;
            mpBar.fillAmount = (float)unitStats.CurrMP / unitStats.MaxMP;
        }
    }
}
