using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TwtManager : MonoBehaviour
{
    [SerializeField] Button tweetButton;
    // Start is called before the first frame update
    void Start()
    {
        tweetButton.onClick.AddListener(Share);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Share()
    {
        string twtAddr = "http://twitter.com/intent/tweet";
        string msg = "Try out this cool game!";
        string descParam = "TestyGame";
        string urlParam = "https://play.google.com/store/apps/details?id=com.VoyagerSoftworks.AndroidProject";

        Application.OpenURL(twtAddr + "?text=" + WWW.EscapeURL(msg) + "&url=" + WWW.EscapeURL(urlParam) + "&hashtags=" + WWW.EscapeURL(descParam));
    }
}
