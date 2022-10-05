using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Loads a battle.
/// </summary>
public class BattleLoader : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public GameObject noSquadWarning;
    /// <summary>
    ///  Loads the battle scene.
    /// </summary>
    public void LoadBattle()
    {
        SaveSerialisation.Instance.Load(); // Load the save data
        if (SaveSerialisation.Instance.PartyMonsterA == null && SaveSerialisation.Instance.PartyMonsterB == null && SaveSerialisation.Instance.PartyMonsterC == null)
        {
            // If there is no squad, show the warning
            noSquadWarning.SetActive(true);
        }
        else
        {
            // If there is a squad, load the battle scene
            sceneLoader.LoadScene("BattleScene");
        }
    }
}
