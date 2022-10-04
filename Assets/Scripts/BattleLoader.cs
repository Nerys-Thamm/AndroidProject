using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLoader : MonoBehaviour
{
    public SceneLoader sceneLoader;
    public GameObject noSquadWarning;
    public void LoadBattle()
    {
        SaveSerialisation.Instance.Load();
        if (SaveSerialisation.Instance.PartyMonsterA == null && SaveSerialisation.Instance.PartyMonsterB == null && SaveSerialisation.Instance.PartyMonsterC == null)
        {
            noSquadWarning.SetActive(true);
        }
        else
        {
            sceneLoader.LoadScene("BattleScene");
        }
    }
}
