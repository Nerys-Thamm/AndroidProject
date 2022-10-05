using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
///  Manages the lootbox menu.
/// </summary>
public class LootboxMenu : MonoBehaviour
{
    public GameObject mainMenu; // The main menu
    public Button summonButton; // The summon button
    public Button coinPurchaseButton; // The coin purchase button
    public TMP_Text coinText; // The coin text
    public MicrotransactionManager microtransactionManager; // The microtransaction manager
    public SpeciesData speciesData; // The species data
    public GameObject summonResultPopup; // The summon result popup
    public TMP_Text summonResultText; // The summon result text

    void OnEnable()
    {
        coinText.text = "Coins: " + PlayerPrefs.GetInt("Currency", 0).ToString(); // Set the coin text to the players currency
        summonButton.interactable = (PlayerPrefs.GetInt("Currency", 0) >= 200); // Set the summon button interactable to whether the player has enough coins
        summonButton.onClick.AddListener(Summon); // Add a listener to the summon button
        coinPurchaseButton.onClick.AddListener(() => { microtransactionManager.BuyCurrency(); mainMenu.SetActive(true); gameObject.SetActive(false); });
    }

    void OnDisable()
    {
        summonButton.onClick.RemoveAllListeners(); // Remove all listeners from the summon button
        coinPurchaseButton.onClick.RemoveAllListeners(); // Remove all listeners from the coin purchase button
    }

    /// <summary>
    ///  Summons a creature.
    /// </summary>
    public void Summon()
    {
        PlayerPrefs.SetInt("Currency", PlayerPrefs.GetInt("Currency", 0) - 200); // Remove 200 coins from the players currency
        coinText.text = "Coins: " + PlayerPrefs.GetInt("Currency", 0).ToString();   // Set the coin text to the players currency
        summonButton.interactable = (PlayerPrefs.GetInt("Currency", 0) >= 200); // Set the summon button interactable to whether the player has enough coins
        SaveSerialisation.Instance.Load(); // Load the save data
        List<UnitData> playerStorage = SaveSerialisation.Instance.PlayerCreatureStorage; // Get the players creature storage
        playerStorage.Add(new UnitData(speciesData.speciesList[Random.Range(0, speciesData.speciesList.Count)].baseUnit, 1)); // Add a random creature to the players creature storage
        SaveSerialisation.Instance.PlayerCreatureStorage = playerStorage; // Set the players creature storage to the new creature storage
        SaveSerialisation.Instance.Save(); // Save the data
        summonResultPopup.SetActive(true); // Set the summon result popup active
        summonResultText.text = "You summoned a " + speciesData.speciesList[Random.Range(0, speciesData.speciesList.Count)].name + "!"; // Set the summon result text to the name of the creature
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
