using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
///  The monster select menu from the players monster storage.
/// </summary>
public class StorageMonsterSelect : MonoBehaviour
{
    public GameObject cellPrefab; // The cell prefab
    public Transform cellParent; // The cell parent
    public GameObject listView; // The list view
    public List<UnitData> monsters; // The monsters


    // Start is called before the first frame update
    void Start()
    {

    }

    /// <summary>
    ///  Get a unit from the menu, and return it in a callback.
    /// </summary>
    /// <param name="callback"></param>
    public void GetSelection(System.Action<UnitData> callback)
    {
        //Enable list
        listView.SetActive(true);

        //Clear list
        foreach (Transform child in cellParent)
        {
            Destroy(child.gameObject);
        }

        //Populate list
        SaveSerialisation.Instance.Load(); // Load the save
        monsters = SaveSerialisation.Instance.PlayerCreatureStorage; // Get the players creature storage
        GameObject noneCell = Instantiate(cellPrefab, cellParent); // Instantiate the cell prefab
        noneCell.GetComponentInChildren<TMP_Text>().text = "None"; // Set the text to none
        noneCell.GetComponent<Button>().onClick.AddListener(() => { callback(null); listView.SetActive(false); }); // Add a listener to the button
        foreach (UnitData monster in monsters)
        {
            GameObject cell = Instantiate(cellPrefab, cellParent); // Instantiate the cell prefab
            cell.GetComponent<Button>().onClick.AddListener(() => { callback(monster); listView.SetActive(false); }); // Add a listener to the button
            cell.GetComponentInChildren<TMP_Text>().text = monster.unitName; // Set the text to the name of the unit
        }
    }

}
