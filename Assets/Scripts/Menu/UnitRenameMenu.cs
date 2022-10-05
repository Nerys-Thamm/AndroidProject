using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// The menu for renaming monsters.
/// </summary>
public class UnitRenameMenu : MonoBehaviour
{
    public TMP_InputField unitName;
    public Button confirmButton;
    public Button cancelButton;

    /// <summary>
    ///  Rename a unit, and return the new name in a callback.
    /// </summary>
    /// <param name="currName"></param>
    /// <param name="callback"></param>
    public void RenameUnit(string currName, System.Action<string> callback)
    {
        unitName.text = currName;
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => { callback(unitName.text); gameObject.SetActive(false); });
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() => { gameObject.SetActive(false); });
    }

}
