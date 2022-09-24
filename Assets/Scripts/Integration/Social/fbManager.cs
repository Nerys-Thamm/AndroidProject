using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;

public class fbManager : MonoBehaviour
{
    [SerializeField] Button _loginButton, _shareButton;

    void Awake()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            FB.ActivateApp();
        }

        _loginButton.onClick.AddListener(Login);
        _shareButton.onClick.AddListener(Share);
    }

    private void InitCallback()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void Share()
    {
        if (!FB.IsLoggedIn)
        {
            FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email" }, AuthCallback);
        }
        else
        {
            FB.ShareLink(new System.Uri("https://www.facebook.com/"), "Check out this cool game!", "This game is awesome!", new System.Uri("https://i.imgur.com/zkYlB.jpg"), ShareCallback);
        }
    }

    public void Login()
    {
        if (!FB.IsLoggedIn)
        {
            FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email" }, AuthCallback);
        }
        else
        {
            Debug.Log("Already logged in");
        }
    }

    private void AuthCallback(ILoginResult result)
    {
        if (!result.Cancelled)
        {
            Debug.Log("Logged in");
        }
        else
        {
            Debug.Log("Login cancelled");
        }
    }

    private void ShareCallback(IShareResult result)
    {
        if (result.Cancelled || !string.IsNullOrEmpty(result.Error))
        {
            Debug.Log("ShareLink error: " + result.Error);
        }
        else if (!string.IsNullOrEmpty(result.PostId))
        {
            Debug.Log(result.PostId);
        }
        else
        {
            Debug.Log("Share success!");
        }
    }
}
