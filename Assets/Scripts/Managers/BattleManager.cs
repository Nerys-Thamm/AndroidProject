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
            //If damage is 0 or smaller, set it to 0 or 1 randomly
            if (dmg <= 0)
            {
                dmg = Random.Range(0, 2);
            }
            return dmg;
        }

        public static int CalcMagicDamage(UnitStats attacker, UnitStats defender)
        {
            int dmg = Mathf.FloorToInt(attacker.INT * 0.5f - defender.DEF * 0.15f);
            dmg = dmg + Random.Range(-dmg / 8, dmg / 8);
            //If damage is 0 or smaller, set it to 0 or 1 randomly
            if (dmg <= 0)
            {
                dmg = Random.Range(0, 2);
            }
            return dmg;
        }

        public static int CalcHeal(UnitStats healer)
        {
            int heal = Mathf.FloorToInt(healer.INT * 0.5f);
            heal = heal + Random.Range(-heal / 8, heal / 8);
            //If heal is 0 or smaller, set it to 0 or 1 randomly
            if (heal <= 0)
            {
                heal = Random.Range(0, 2);
            }
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

    public GameObject hitEffect;
    public GameObject deathEffect;
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
            battleUI.SetMainMenuActive(false);
            battleDirector.StartBattle(currentAction);
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
        GameObject hitEffectInstance = null;
        HitScore hitScore = null;
        switch (currentAction.actionType)
        {
            case BattleAction.ActionType.Attack:
                dmg = Utilities.CalcPhysDamage(currentAction.unit, currentAction.target);
                currentAction.target.TakeDamage(dmg);
                sfxController.Play("Melee");
                hitEffectInstance = Instantiate(hitEffect, currentAction.target.transform.position, Quaternion.identity);
                hitScore = hitEffectInstance.GetComponent<HitScore>();
                hitScore.SetText(dmg.ToString());
                hitScore.SetType(HitScore.Type.Damage);
                break;
            case BattleAction.ActionType.Ability:
                currentAction.unit.UseMP(currentAction.ability.MPCost);
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
                        hitEffectInstance = Instantiate(hitEffect, currentAction.target.transform.position, Quaternion.identity);
                        hitScore = hitEffectInstance.GetComponent<HitScore>();
                        hitScore.SetText(dmg.ToString());
                        hitScore.SetType(HitScore.Type.Damage);
                        currentAction.target.ApplyDebuff(currentAction.ability.Effect);
                        break;
                    case Ability.AbilityType.Heal:
                        int heal = Mathf.FloorToInt(Utilities.CalcHeal(currentAction.unit) * currentAction.ability.EffectMult);
                        currentAction.target.Heal(heal);
                        hitEffectInstance = Instantiate(hitEffect, currentAction.target.transform.position, Quaternion.identity);
                        hitScore = hitEffectInstance.GetComponent<HitScore>();
                        hitScore.SetText(heal.ToString());
                        hitScore.SetType(HitScore.Type.Heal);
                        break;
                    case Ability.AbilityType.Buff:
                        currentAction.target.ApplyBuff(currentAction.ability.Effect);
                        break;
                    case Ability.AbilityType.Debuff:
                        currentAction.target.ApplyDebuff(currentAction.ability.Effect);
                        break;
                }
                if ((currentAction.ability.Effect & Ability.EffectMask.AGI) != 0)
                {
                    hitEffectInstance = Instantiate(hitEffect, currentAction.target.transform.position, Quaternion.identity);
                    hitScore = hitEffectInstance.GetComponent<HitScore>();
                    hitScore.SetText("AGI");
                    hitScore.SetType(currentAction.ability.Type == Ability.AbilityType.Buff ? HitScore.Type.Buff : HitScore.Type.Debuff);
                }
                if ((currentAction.ability.Effect & Ability.EffectMask.STR) != 0)
                {
                    hitEffectInstance = Instantiate(hitEffect, currentAction.target.transform.position, Quaternion.identity);
                    hitScore = hitEffectInstance.GetComponent<HitScore>();
                    hitScore.SetText("STR");
                    hitScore.SetType(currentAction.ability.Type == Ability.AbilityType.Buff ? HitScore.Type.Buff : HitScore.Type.Debuff);
                }
                if ((currentAction.ability.Effect & Ability.EffectMask.INT) != 0)
                {
                    hitEffectInstance = Instantiate(hitEffect, currentAction.target.transform.position, Quaternion.identity);
                    hitScore = hitEffectInstance.GetComponent<HitScore>();
                    hitScore.SetText("INT");
                    hitScore.SetType(currentAction.ability.Type == Ability.AbilityType.Buff ? HitScore.Type.Buff : HitScore.Type.Debuff);
                }
                if ((currentAction.ability.Effect & Ability.EffectMask.DEF) != 0)
                {
                    hitEffectInstance = Instantiate(hitEffect, currentAction.target.transform.position, Quaternion.identity);
                    hitScore = hitEffectInstance.GetComponent<HitScore>();
                    hitScore.SetText("DEF");
                    hitScore.SetType(currentAction.ability.Type == Ability.AbilityType.Buff ? HitScore.Type.Buff : HitScore.Type.Debuff);
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
                    StartCoroutine(DeathAnimation(currentAction.target.gameObject));
                    battleUI.SetDead(currentAction.targetIndex, false);
                    //battleUI.ShowDeath(currentAction.targetIndex, currentAction.enemy);
                }
                else
                {
                    float xpVal = Mathf.Pow((currentAction.target.Level * 10f), 1.2f);
                    xpVal += (xpVal / 10f * Random.Range(-1f, 1f));
                    earnedEXP += xpVal;
                    StartCoroutine(DeathAnimation(currentAction.target.gameObject));
                    battleUI.SetDead(currentAction.targetIndex, true);
                    //battleUI.ShowDeath(currentAction.targetIndex, currentAction.enemy);
                }
            }
        }
    }

    IEnumerator DeathAnimation(GameObject unit)
    {
        //Place a death effect on the unit, then fade out all materials over time before destroying the death effect and unit
        GameObject deathEffectInstance = Instantiate(deathEffect, unit.transform.position, Quaternion.identity);

        //Fade out all materials
        Renderer[] renderers = unit.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            foreach (Material m in r.materials)
            {
                Color c = m.color;
                while (c.a > 0)
                {
                    c.a -= Time.deltaTime / 2.0f;
                    m.color = c;
                    yield return null;
                }
            }
        }

        Destroy(deathEffectInstance);
        Destroy(unit, 5f);
    }

    void EndRound()
    {
        currentlyBattling = false;
        //battleDirector.EndBattle();
        battleUI.ClearActions();
        battleUI.SetMainMenuActive(true);
        musicController.Trigger("Action Select");
        OnRoundEnd?.Invoke();
    }



    void InitialiseBattle()
    {
        // Spawn the player units
        for (int i = 0; i < teamManager.playerUnits.Length; i++)
        {
            battleField.TrySpawnUnit(teamManager.playerUnits[i], ref battleField.playerCells);
            battleUI.CreateUnitInfoPanel(battleField.playerCells[i].unitStats);
        }

        // Set the player unit names
        string[] playerNames = { "", "", "" };
        for (int i = 0; i < teamManager.playerUnits.Length; i++)
        {
            playerNames[i] = battleField.playerCells[i].unitStats.Name;
        }
        battleUI.SetAllyNames(playerNames);

        // Decide how many enemies to spawn between 1 and 3
        int enemyCount = Random.Range(1, 4);
        for (int i = 0; i < enemyCount; i++)
        {
            // Decide which enemy to spawn
            int enemyIndex = Random.Range(0, enemyData.units.Count);
            battleField.TrySpawnUnit(enemyData.units[enemyIndex], ref battleField.enemyCells);
            battleField.enemyCells[i].unitStats.SetNewUnitData(Random.Range(enemyData.minLevel, enemyData.maxLevel));
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
