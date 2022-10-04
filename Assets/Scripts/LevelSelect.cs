using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelect : MonoBehaviour
{
    public GameObject levelButtonPrefab;
    public Transform levelButtonParent;
    public int levelInterval = 2;
    public int levelCount = 20;
    public BattleLoader battleLoader;

    public void UpdateLevelButtons()
    {
        foreach (Transform child in levelButtonParent)
        {
            Destroy(child.gameObject);
        }

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

    // Update is called once per frame
    void Update()
    {

    }
}
