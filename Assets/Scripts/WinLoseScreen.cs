using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WinLoseScreen : MonoBehaviour
{
    public TMP_Text winLoseText;
    public TMP_Text rewardsText;
    public Button menuButton;

    public string winText, loseText;
    string rewardsString = "";

    public void Show(bool win)
    {
        winLoseText.text = win ? winText : loseText;
        //rewardsText.text = "Rewards: " + GameManager.Instance.rewards;
        //menuButton.onClick.AddListener(() => GameManager.Instance.LoadMenu());
    }

    public void AddReward(string reward)
    {
        rewardsString += reward + "\n";
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        rewardsText.text = rewardsString;
    }
}
