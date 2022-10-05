using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;


public class GooglePlay : MonoBehaviour
{
    bool authenticated = false;

    // Start is called before the first frame update
    void Start()
    {
        PlayGamesPlatform.Activate();
    }

    // Update is called once per frame
    void Update()
    {
        if (!authenticated)
        {
            Authenticate();
        }
    }

    /// <summary>
    ///  Authenticates the user.
    /// </summary>
    void Authenticate()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                authenticated = true;
            }
            else
            {
                authenticated = false;
            }
        });
    }

    /// <summary>
    /// Shows the achievements UI.
    /// </summary>
    public void ShowAchievements()
    {
        if (authenticated)
        {
            Social.ShowAchievementsUI();
        }
    }
}
