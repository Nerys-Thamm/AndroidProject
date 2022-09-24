using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleManager : MonoBehaviour
{
    private static class Utilities
    {
        public static int CalcPhysDamage(UnitStats attacker, UnitStats defender)
        {
            int dmg = Mathf.FloorToInt(attacker.STR * 0.5f - defender.DEF * 0.25f);
            dmg = dmg + Random.Range(-dmg / 8, dmg / 8);
            return dmg;
        }
    }


    /// <summary>
    ///  Managers and UI Controllers
    /// </summary>
    public BattleField battleField;
    public TeamManager teamManager;
    public EnemyData enemyData;
    public BattleUI battleUI;

    public struct BattleAction
    {
        public UnitStats unit;
        public UnitStats target;
        public int targetIndex;
        public int unitIndex;
        public bool enemy;
        public Ability ability;
        enum ActionType
        {
            Attack,
            Ability,
            Defend
        }
        ActionType actionType;
    }

    List<BattleAction> roundActions = new List<BattleAction>();


    // Start is called before the first frame update
    void Start()
    {
        battleField.OnReady += InitialiseBattle;
    }

    // Update is called once per frame
    void Update()
    {

    }

    ///
    /// Helper Functions for UI
    /// 

    public bool CanSelectUnit(int index, bool enemy)
    {
        if (enemy)
        {
            return battleField.enemyCells[index].unitStats != null && battleField.enemyCells[index].unitStats.CurrHP > 0;
        }
        else
        {
            return battleField.playerCells[index].unitStats != null && battleField.playerCells[index].unitStats.CurrHP > 0;
        }
    }


    /// <summary>
    /// Battle Sequence
    /// </summary>

    void PrepareRound()
    {
        roundActions.Sort((a, b) => a.unit.AGI.CompareTo(b.unit.AGI));
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

        //Set the Enemy Names
        string[] enemyNames = { "", "", "" };
        for (int i = 0; i < battleField.enemyCells.Length; i++)
        {
            if (battleField.enemyCells[i].unit != null)
            {
                enemyNames[i] = battleField.enemyCells[i].unitStats.Name;
            }
        }
        //Add Letters to duplicate names
        if (enemyNames[0] == enemyNames[1])
        {
            if (enemyNames[0] == enemyNames[2])
            {
                enemyNames[0] = enemyNames[0] + " A";
                enemyNames[1] = enemyNames[1] + " B";
                enemyNames[2] = enemyNames[2] + " C";
            }
            else
            {
                enemyNames[0] = enemyNames[0] + " A";
                enemyNames[1] = enemyNames[1] + " B";
            }
        }
        else if (enemyNames[1] == enemyNames[2] && enemyNames[1] != "")
        {
            enemyNames[1] = enemyNames[1] + " A";
            enemyNames[2] = enemyNames[2] + " B";
        }
        else if (enemyNames[0] == enemyNames[2] && enemyNames[2] != "")
        {
            enemyNames[0] = enemyNames[0] + " A";
            enemyNames[2] = enemyNames[2] + " B";
        }
        battleUI.SetEnemyNames(enemyNames);

    }
}
