using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles loading of scenes
/// </summary>
public class SceneLoader : MonoBehaviour
{
    /// <summary>
    ///  Load a scene by name.
    /// </summary>
    /// <param name="sceneName"></param>
    public void LoadScene(string sceneName)
    {
        if (PlayerPrefs.GetInt("AdsDisabled", 0) == 0 && FindObjectOfType<AdvertisementManager>() != null) //If ads arent disabled, then show an ad before loading.
        {
            FindObjectOfType<AdvertisementManager>().OnAdComplete += () => UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
            FindObjectOfType<AdvertisementManager>().LoadAd();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}
