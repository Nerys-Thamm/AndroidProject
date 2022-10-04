using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StorageMonsterSelect : MonoBehaviour
{
    public GameObject cellPrefab;
    public Transform cellParent;
    public GameObject listView;
    public List<UnitData> monsters;


    // Start is called before the first frame update
    void Start()
    {

    }

    public void GetSelection(System.Action<UnitData> callback)
    {
        //Enable list
        listView.SetActive(true);

        //Clear list
        foreach (Transform child in cellParent)
        {
            Destroy(child.gameObject);
        }

        //Populate list
        SaveSerialisation.Instance.Load();
        monsters = SaveSerialisation.Instance.PlayerCreatureStorage;
        GameObject noneCell = Instantiate(cellPrefab, cellParent);
        noneCell.GetComponentInChildren<TMP_Text>().text = "None";
        noneCell.GetComponent<Button>().onClick.AddListener(() => { callback(null); listView.SetActive(false); });
        foreach (UnitData monster in monsters)
        {
            GameObject cell = Instantiate(cellPrefab, cellParent);
            cell.GetComponent<Button>().onClick.AddListener(() => { callback(monster); listView.SetActive(false); });
            cell.GetComponentInChildren<TMP_Text>().text = monster.unitName;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
