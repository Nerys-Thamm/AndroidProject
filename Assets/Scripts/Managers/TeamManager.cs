using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{

    public UnitData[] playerUnits = { null, null, null };
    public UnitData[] starterUnits = { null, null, null };

    // Start is called before the first frame update
    void Start()
    {
        SaveSerialisation.Instance.Load();
        playerUnits[0] = SaveSerialisation.Instance.PartyMonsterA;
        playerUnits[1] = SaveSerialisation.Instance.PartyMonsterB;
        playerUnits[2] = SaveSerialisation.Instance.PartyMonsterC;

        if ((playerUnits[0] == null && playerUnits[1] == null && playerUnits[2] == null) && SaveSerialisation.Instance.PlayerCreatureStorage.Count == 0)
        {
            //No monsters in party, load starter monsters
            playerUnits[0] = starterUnits[0];
            playerUnits[1] = starterUnits[1];
            playerUnits[2] = starterUnits[2];
            SaveSerialisation.Instance.PartyMonsterA = starterUnits[0];
            SaveSerialisation.Instance.PartyMonsterB = starterUnits[1];
            SaveSerialisation.Instance.PartyMonsterC = starterUnits[2];
            SaveSerialisation.Instance.Save();
        }
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
