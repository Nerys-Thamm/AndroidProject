using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LootboxMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public Button summonButton;
    public Button coinPurchaseButton;
    public TMP_Text coinText;
    public MicrotransactionManager microtransactionManager;
    public SpeciesData speciesData;
    public GameObject summonResultPopup;
    public TMP_Text summonResultText;

    void OnEnable()
    {
        coinText.text = "Coins: " + PlayerPrefs.GetInt("Currency", 0).ToString();
        summonButton.interactable = (PlayerPrefs.GetInt("Currency", 0) >= 200);
        summonButton.onClick.AddListener(Summon);
        coinPurchaseButton.onClick.AddListener(() => { microtransactionManager.BuyCurrency(); mainMenu.SetActive(true); gameObject.SetActive(false); });
    }

    void OnDisable()
    {
        summonButton.onClick.RemoveAllListeners();
        coinPurchaseButton.onClick.RemoveAllListeners();
    }

    public void Summon()
    {
        PlayerPrefs.SetInt("Currency", PlayerPrefs.GetInt("Currency", 0) - 200);
        coinText.text = "Coins: " + PlayerPrefs.GetInt("Currency", 0).ToString();
        summonButton.interactable = (PlayerPrefs.GetInt("Currency", 0) >= 200);
        SaveSerialisation.Instance.Load();
        List<UnitData> playerStorage = SaveSerialisation.Instance.PlayerCreatureStorage;
        playerStorage.Add(new UnitData(speciesData.speciesList[Random.Range(0, speciesData.speciesList.Count)].baseUnit, 1));
        SaveSerialisation.Instance.PlayerCreatureStorage = playerStorage;
        SaveSerialisation.Instance.Save();
        summonResultPopup.SetActive(true);
        summonResultText.text = "You summoned a " + speciesData.speciesList[Random.Range(0, speciesData.speciesList.Count)].name + "!";
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
