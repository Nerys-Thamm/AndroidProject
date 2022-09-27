using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public static int CalcMagicDamage(UnitStats attacker, UnitStats defender)
        {
            int dmg = Mathf.FloorToInt(attacker.INT * 0.5f - defender.DEF * 0.15f);
            dmg = dmg + Random.Range(-dmg / 8, dmg / 8);
            return dmg;
        }

        public static int CalcHeal(UnitStats healer)
        {
            int heal = Mathf.FloorToInt(healer.INT * 0.5f);
            heal = heal + Random.Range(-heal / 8, heal / 8);
            return heal;
        }
    }


    /// <summary>
    ///  Managers and UI Controllers
    /// </summary>
    public BattleField battleField;
    public TeamManager teamManager;
    public BattleDirector battleDirector;
    public EnemyData enemyData;
    public BattleUI battleUI;

    /// <summary>
    ///  Effects and Sounds
    /// </summary>

    public GameObject damageEffect;
    public GameObject healEffect;
    public AudioController musicController, sfxController;


    public struct BattleAction
    {
        public UnitStats unit;
        public UnitStats target;
        public int targetIndex;
        public int unitIndex;
        public bool enemy;
        public Ability ability;
        public enum ActionType
        {
            Attack,
            Ability,
            Defend
        }
        public ActionType actionType;
    }



    List<BattleAction> roundActions = new List<BattleAction>();
    BattleAction currentAction;

    bool currentlyBattling = false;
    bool win = false;
    bool readyForNextAction = false;

    float earnedEXP = 0;


    // Start is called before the first frame update
    void Start()
    {
        battleField.OnReady += InitialiseBattle;

        battleDirector.OnTargetReached += ApplyCurrentAction;
        battleDirector.OnAnimationComplete += () => readyForNextAction = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentlyBattling)
        {
            if (readyForNextAction)
            {
                LoadNextAction();
                readyForNextAction = false;
            }
        }
    }

    public System.Action OnRoundBegin, OnRoundEnd;

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

    ///
    /// Move Select Functions
    /// 
    public void AddAction(BattleAction action)
    {
        roundActions.Add(action);
    }

    public void RandomAction(UnitStats unit)
    {
        BattleAction action = new BattleAction();
        action.unit = unit;
        action.actionType = BattleAction.ActionType.Attack;
        // Get all existing and alive unit indexes
        List<int> indexes = new List<int>();
        for (int i = 0; i < battleField.playerCells.Length; i++)
        {
            if (battleField.playerCells[i].unitStats != null && battleField.playerCells[i].unitStats.CurrHP > 0)
            {
                indexes.Add(i);
            }
        }
        // Return if no units are alive
        if (indexes.Count == 0)
        {
            return;
        }
        // Get random index
        int index = indexes[Random.Range(0, indexes.Count)];
        action.target = battleField.playerCells[index].unitStats;
        action.targetIndex = index;
        action.enemy = true;

        roundActions.Add(action);
    }

    /// <summary>
    /// Battle Sequence
    /// </summary>


    public void StartBattle()
    {
        if (!currentlyBattling)
        {
            // For each enemy, add a random action
            for (int i = 0; i < battleField.enemyCells.Length; i++)
            {
                if (battleField.enemyCells[i].unitStats != null)
                {
                    RandomAction(battleField.enemyCells[i].unitStats);
                }
            }

            // For each player unit, add its action
            for (int i = 0; i < battleField.playerCells.Length; i++)
            {
                if (battleUI.Actions[i].unit != null)
                {
                    roundActions.Add(battleUI.Actions[i]);
                }
            }
            currentlyBattling = true;
            PrepareRound();
            LoadNextAction();
            battleDirector.StartBattle(currentAction);
            musicController.Stop("Battle");
            musicController.Play("Battle");
            musicController.Trigger("Battle");
        }
    }

    void PrepareRound()
    {
        roundActions = roundActions.OrderByDescending(x => x.unit.AGI).ToList();

        //Move all defend actions to the start of the list
        for (int i = 0; i < roundActions.Count; i++)
        {
            if (roundActions[i].actionType == BattleAction.ActionType.Defend)
            {
                BattleAction temp = roundActions[i];
                roundActions.RemoveAt(i);
                roundActions.Insert(0, temp);
            }
        }
    }

    void ResortActions()
    {
        // Remove all actions from units that are dead
        roundActions.RemoveAll((a) => a.unit.CurrHP <= 0);

        // Remove all actions targeting units that are dead
        roundActions.RemoveAll((a) => a.target.CurrHP <= 0);
    }

    void LoadNextAction()
    {
        //Check for win or lose
        bool lose = true;
        bool win = true;

        for (int i = 0; i < battleField.playerCells.Length; i++)
        {
            if (battleField.playerCells[i].unitStats != null && battleField.playerCells[i].unitStats.CurrHP > 0)
            {
                lose = false;
            }
        }

        for (int i = 0; i < battleField.enemyCells.Length; i++)
        {
            if (battleField.enemyCells[i].unitStats != null && battleField.enemyCells[i].unitStats.CurrHP > 0)
            {
                win = false;
            }
        }

        if (lose)
        {
            //battleUI.ShowLoseScreen();
            //return;
        }
        else if (win)
        {
            //battleUI.ShowWinScreen();
            //return;
        }

        ResortActions();
        if (roundActions.Count > 0)
        {
            currentAction = roundActions[0];
            roundActions.RemoveAt(0);
            battleDirector.StartBattle(currentAction);
        }
        else
        {
            EndRound();
        }
    }

    void ApplyCurrentAction()
    {
        int dmg = 0;
        switch (currentAction.actionType)
        {
            case BattleAction.ActionType.Attack:
                dmg = Utilities.CalcPhysDamage(currentAction.unit, currentAction.target);
                currentAction.target.TakeDamage(dmg);
                sfxController.Play("Melee");
                //battleUI.ShowDamage(currentAction.targetIndex, currentAction.enemy, dmg);
                break;
            case BattleAction.ActionType.Ability:
                switch (currentAction.ability.Type)
                {
                    case Ability.AbilityType.Attack:
                        if (currentAction.ability.IsMagic)
                        {
                            dmg = Mathf.FloorToInt(Utilities.CalcMagicDamage(currentAction.unit, currentAction.target) * currentAction.ability.EffectMult);
                            sfxController.Play("Magic");
                        }
                        else
                        {
                            dmg = Mathf.FloorToInt(Utilities.CalcPhysDamage(currentAction.unit, currentAction.target) * currentAction.ability.EffectMult);
                            sfxController.Play("Melee");
                        }
                        currentAction.target.TakeDamage(dmg);
                        //battleUI.ShowDamage(currentAction.targetIndex, currentAction.enemy, dmg);
                        break;
                    case Ability.AbilityType.Heal:
                        int heal = Mathf.FloorToInt(Utilities.CalcHeal(currentAction.unit) * currentAction.ability.EffectMult);
                        currentAction.target.Heal(heal);
                        //battleUI.ShowHeal(currentAction.targetIndex, currentAction.enemy, heal);
                        break;
                }
                break;
            case BattleAction.ActionType.Defend:
                //currentAction.unit.isDefending = true;
                break;
        }

        //If current action deals damage, check for death
        if (dmg > 0)
        {
            if (currentAction.target.CurrHP <= 0)
            {
                if (currentAction.enemy)
                {
                    //battleUI.ShowDeath(currentAction.targetIndex, currentAction.enemy);
                }
                else
                {
                    float xpVal = Mathf.Pow((currentAction.target.Level * 10f), 1.2f);
                    xpVal += (xpVal / 10f * Random.Range(-1f, 1f));
                    earnedEXP += xpVal;
                    //battleUI.ShowDeath(currentAction.targetIndex, currentAction.enemy);
                }
            }
        }
    }

    void EndRound()
    {
        currentlyBattling = false;
        //battleDirector.EndBattle();
        battleUI.ClearActions();
        musicController.Trigger("Action Select");
        OnRoundEnd?.Invoke();
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
