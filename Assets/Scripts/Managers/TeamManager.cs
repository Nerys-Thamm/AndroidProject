using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{

    public UnitData[] playerUnits = { null, null, null };

    // Start is called before the first frame update
    void Start()
    {
        SaveSerialisation.Instance.Load();
        playerUnits[0] = SaveSerialisation.Instance.PartyMonsterA;
        playerUnits[1] = SaveSerialisation.Instance.PartyMonsterB;
        playerUnits[2] = SaveSerialisation.Instance.PartyMonsterC;
    }

    public void SaveMonsters()
    {
        SaveSerialisation.Instance.PartyMonsterA = playerUnits[0];
        SaveSerialisation.Instance.PartyMonsterB = playerUnits[1];
        SaveSerialisation.Instance.PartyMonsterC = playerUnits[2];
        SaveSerialisation.Instance.Save();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
