using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitRenameMenu : MonoBehaviour
{
    public TMP_InputField unitName;
    public Button confirmButton;
    public Button cancelButton;

    public void RenameUnit(string currName, System.Action<string> callback)
    {
        unitName.text = currName;
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => { callback(unitName.text); gameObject.SetActive(false); });
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() => { gameObject.SetActive(false); });
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
