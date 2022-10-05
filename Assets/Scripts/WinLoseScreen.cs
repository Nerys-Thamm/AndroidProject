using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// UI Displaying the win and lose result, as well as rewards.
/// </summary>
public class WinLoseScreen : MonoBehaviour
{
    public TMP_Text winLoseText;
    public TMP_Text rewardsText;
    public Button menuButton;

    public string winText, loseText;
    string rewardsString = "";

    /// <summary>
    ///  Sets the win/lose text and rewards.
    /// </summary>
    /// <param name="win"></param>
    public void Show(bool win)
    {
        winLoseText.text = win ? winText : loseText;
        //rewardsText.text = "Rewards: " + GameManager.Instance.rewards;
        //menuButton.onClick.AddListener(() => GameManager.Instance.LoadMenu());
    }

    /// <summary>
    ///  Adds a reward to the rewards string.
    /// </summary>
    /// <param name="reward"></param>
    public void AddReward(string reward)
    {
        rewardsString += reward + "\n";
    }


    // Update is called once per frame
    void Update()
    {
        rewardsText.text = rewardsString;
    }
}
