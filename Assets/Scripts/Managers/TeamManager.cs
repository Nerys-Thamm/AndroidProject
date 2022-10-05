using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Manages the player's team.
/// </summary>
public class TeamManager : MonoBehaviour
{

    public UnitData[] playerUnits = { null, null, null }; // The players units
    public UnitData[] starterUnits = { null, null, null }; // The starter units

    // Start is called before the first frame update
    void Start()
    {
        SaveSerialisation.Instance.Load(); // Load the save data
        playerUnits[0] = SaveSerialisation.Instance.PartyMonsterA; // Set the players units to the save data
        playerUnits[1] = SaveSerialisation.Instance.PartyMonsterB; // Set the players units to the save data
        playerUnits[2] = SaveSerialisation.Instance.PartyMonsterC; // Set the players units to the save data

        /// If the player has no units, and none in storage, give them the starter units
        if ((playerUnits[0] == null && playerUnits[1] == null && playerUnits[2] == null) && SaveSerialisation.Instance.PlayerCreatureStorage.Count == 0)
        {
            //No monsters in party, load starter monsters
            playerUnits[0] = starterUnits[0];
            playerUnits[1] = starterUnits[1];
            playerUnits[2] = starterUnits[2];
            SaveSerialisation.Instance.PartyMonsterA = starterUnits[0];
            SaveSerialisation.Instance.PartyMonsterB = starterUnits[1];
            SaveSerialisation.Instance.PartyMonsterC = starterUnits[2];
            SaveSerialisation.Instance.Save(); // Save the data
        }
    }

    /// <summary>
    ///  Sets the players units to the save data.
    /// </summary>
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
