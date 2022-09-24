using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleManager : MonoBehaviour
{
    public BattleField battleField;
    public TeamManager teamManager;
    public EnemyData enemyData;

    // Start is called before the first frame update
    void Start()
    {
        battleField.OnReady += InitialiseBattle;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitialiseBattle()
    {
        // Spawn the player units
        for (int i = 0; i < teamManager.playerUnits.Length; i++)
        {
            battleField.TrySpawnUnit(teamManager.playerUnits[i], ref battleField.playerCells);
        }

        // Decide how many enemies to spawn between 1 and 3
        int enemyCount = Random.Range(1, 4);
        for (int i = 0; i < enemyCount; i++)
        {
            // Decide which enemy to spawn
            int enemyIndex = Random.Range(0, enemyData.units.Count);
            battleField.TrySpawnUnit(enemyData.units[enemyIndex], ref battleField.enemyCells);
        }

        // Apply animation offsets
        for (int i = 0; i < battleField.playerCells.Length; i++)
        {
            if (battleField.playerCells[i].unit != null)
            {
                battleField.playerCells[i].unit.GetComponent<Animator>().SetFloat("Offset", i / 3f);
            }
        }

        for (int i = 0; i < battleField.enemyCells.Length; i++)
        {
            if (battleField.enemyCells[i].unit != null)
            {
                battleField.enemyCells[i].unit.GetComponent<Animator>().SetFloat("Offset", i / 3f);
            }
        }

    }
}
