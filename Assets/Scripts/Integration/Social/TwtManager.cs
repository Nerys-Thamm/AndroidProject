using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
///  Manages twitter integration.
/// </summary>
public class TwtManager : MonoBehaviour
{
    [SerializeField] Button tweetButton; // Button to tweet
    // Start is called before the first frame update
    void Start()
    {
        tweetButton.onClick.AddListener(Share);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    ///  Shares the game on twitter.
    /// </summary>
    public void Share()
    {
        string twtAddr = "http://twitter.com/intent/tweet";
        string msg = "Try out this cool game!";
        string descParam = "Battle Monsters";
        string urlParam = "https://play.google.com/store/apps/details?id=com.VoyagerSoftworks.BattleMonsters";

        Application.OpenURL(twtAddr + "?text=" + WWW.EscapeURL(msg) + "&url=" + WWW.EscapeURL(urlParam) + "&hashtags=" + WWW.EscapeURL(descParam));
    }
}
