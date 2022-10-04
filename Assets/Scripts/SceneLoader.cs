using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadScene(string sceneName)
    {
        if (PlayerPrefs.GetInt("AdsDisabled", 0) == 0 && FindObjectOfType<AdvertisementManager>() != null)
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
