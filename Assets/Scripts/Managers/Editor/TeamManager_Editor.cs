using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TeamManager))]
public class TeamManager_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TeamManager teamManager = (TeamManager)target;
        if (GUILayout.Button("Save"))
        {
            SaveSerialisation.Instance.Save();
            SaveSerialisation.Instance.PartyMonsterA = teamManager.playerUnits[0];
            SaveSerialisation.Instance.PartyMonsterB = teamManager.playerUnits[1];
            SaveSerialisation.Instance.PartyMonsterC = teamManager.playerUnits[2];
            SaveSerialisation.Instance.Save();
        }
        if (GUILayout.Button("Load"))
        {
            SaveSerialisation.Instance.Load();
            teamManager.playerUnits[0] = SaveSerialisation.Instance.PartyMonsterA;
            teamManager.playerUnits[1] = SaveSerialisation.Instance.PartyMonsterB;
            teamManager.playerUnits[2] = SaveSerialisation.Instance.PartyMonsterC;
        }
        if (GUILayout.Button("Delete"))
        {
            SaveSerialisation.Instance.DeleteSave();
        }

    }
}
