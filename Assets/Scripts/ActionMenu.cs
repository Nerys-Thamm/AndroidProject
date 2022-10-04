using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionMenu : MonoBehaviour
{
    [SerializeField] BattleUI _battleUI;
    [SerializeField] private TMP_Text currentActionText;


    public void SetCurrentActionText(string text)
    {
        currentActionText.text = "Current Action: " + text;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
