using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// THe level select menu.
/// </summary>
public class LevelSelect : MonoBehaviour
{
    public GameObject levelButtonPrefab; // The prefab for the level buttons
    public Transform levelButtonParent; // The parent for the level buttons
    public int levelInterval = 2; // The interval between levels
    public int levelCount = 20; // The number of levels
    public BattleLoader battleLoader; // The battle loader

    /// <summary>
    ///  Updates the level select menu.
    /// </summary>
    public void UpdateLevelButtons()
    {
        // Clear the level buttons
        foreach (Transform child in levelButtonParent)
        {
            Destroy(child.gameObject);
        }

        // Create the level buttons
        for (int i = 0; i < levelCount; i++)
        {
            GameObject levelButton = Instantiate(levelButtonPrefab, levelButtonParent);
            int level = i + 1;
            levelButton.GetComponent<Button>().onClick.AddListener(() => { PlayerPrefs.SetInt("CurrLevel", level); battleLoader.LoadBattle(); });
            levelButton.GetComponent<Button>().interactable = (PlayerPrefs.GetInt("HighestCompletedLevel", 0) >= i);
            levelButton.GetComponentInChildren<TMP_Text>().text = "Level " + (i + 1);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("LevelInterval", levelInterval);
    }

    void OnEnable()
    {
        UpdateLevelButtons();
    }

}
