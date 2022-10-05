using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
///  Manages the state and flow of battle.
/// </summary>
public class BattleManager : MonoBehaviour
{
    /// <summary>
    ///  Utility calculation methods.
    /// </summary>
    private static class Utilities
    {
        /// <summary>
        ///  Calculates the damage dealt by an attack.
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <returns></returns>
        public static int CalcPhysDamage(UnitStats attacker, UnitStats defender)
        {
            int dmg = Mathf.FloorToInt(attacker.STR * 0.5f - defender.DEF * 0.25f); // Calculate damage
            dmg = dmg + Random.Range(-dmg / 8, dmg / 8); // Add random variance
            //If damage is 0 or smaller, set it to 0 or 1 randomly
            if (dmg <= 0)
            {
                dmg = Random.Range(0, 2);
            }
            return dmg;
        }

        /// <summary>
        ///  Calculates the damage dealt by a magic attack.
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <returns></returns>
        public static int CalcMagicDamage(UnitStats attacker, UnitStats defender)
        {
            int dmg = Mathf.FloorToInt(attacker.INT * 0.5f - defender.DEF * 0.15f); // Calculate damage
            dmg = dmg + Random.Range(-dmg / 8, dmg / 8); // Add random variance
            //If damage is 0 or smaller, set it to 0 or 1 randomly
            if (dmg <= 0)
            {
                dmg = Random.Range(0, 2);
            }
            return dmg;
        }

        /// <summary>
        ///  Calculates the heal amount of a heal.
        /// </summary>
        /// <param name="healer"></param>
        /// <returns></returns>
        public static int CalcHeal(UnitStats healer)
        {
            int heal = Mathf.FloorToInt(healer.INT * 0.5f); // Calculate heal
            heal = heal + Random.Range(-heal / 8, heal / 8); // Add random variance
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
    public BattleField battleField; // Battle field
    public TeamManager teamManager; // Team manager
    public BattleDirector battleDirector; // Battle director
    public SpeciesData enemyData; // Enemy data
    public BattleUI battleUI; // Battle UI
    public WinLoseScreen winLoseScreen; // Win/Lose screen
    public GameObject gameOverScreen; // Game over screen

    /// <summary>
    ///  Effects and Sounds
    /// </summary>

    public GameObject hitEffect; // Hit effect
    public GameObject deathEffect; // Death effect
    public AudioController musicController, sfxController; // Music and SFX controllers

    /// <summary>
    ///  Struct representing an action to be taken by a unit in battle.
    /// </summary>
    public struct BattleAction
    {
        public UnitStats unit; // Unit to take action
        public UnitStats target; // Target of action
        public int targetIndex; // Index of target
        public int unitIndex; // Index of unit
        public bool enemy; // Is the unit an enemy?
        public Ability ability; // Ability to use
        /// <summary>
        ///  Type of action to take.
        /// </summary>
        public enum ActionType
        {
            Attack, // Attack an enemy
            Ability, // Use an ability
            Defend // Defend
        }
        public ActionType actionType;
    }

    List<BattleAction> roundActions = new List<BattleAction>(); // List of actions to take in the current round
    BattleAction currentAction; // Current action being taken

    bool currentlyBattling = false; // Is the battle currently in progress?
    bool win = false; // Did the player win?
    bool readyForNextAction = false; // Is the battle ready for the next action?
    float earnedEXP = 0; // Amount of EXP earned in battle


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

    public System.Action OnRoundBegin, OnRoundEnd; // Events for round begin and end

    ///
    /// Helper Functions for UI
    /// 

    /// <summary>
    ///  Checks whether a unit is selectable.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="enemy"></param>
    /// <returns></returns>
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

    /// <summary>
    ///  Chooses a random action for the given unit.
    /// </summary>
    /// <param name="unit"></param>
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

    /// <summary>
    ///  Starts the next round of battle.
    /// </summary>
    public void StartBattle()
    {
        if (!currentlyBattling) //If the battle isnt in progress
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
            currentlyBattling = true; // Set battle in progress
            PrepareRound(); // Prepare the round
            LoadNextAction(); // Load the next action
            battleUI.SetMainMenuActive(false);
            battleDirector.StartBattle(currentAction);
            musicController.Trigger("Battle");
        }
    }

    /// <summary>
    ///  Prepares the round.
    /// </summary>
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

    /// <summary>
    ///  Resorts actions to account for deaths.
    /// </summary>
    void ResortActions()
    {
        // Remove all actions from units that are dead
        roundActions.RemoveAll((a) => a.unit.CurrHP <= 0);

        // Remove all actions targeting units that are dead
        roundActions.RemoveAll((a) => a.target.CurrHP <= 0);
    }

    /// <summary>
    ///  Loads the next action in the round.
    /// </summary>
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
            gameOverScreen.SetActive(true);
            winLoseScreen.Show(false);
            winLoseScreen.AddReward("No EXP Gained.");
            musicController.Trigger("GameOver");
            teamManager.SaveMonsters();
            return;
        }
        else if (win)
        {
            Social.ReportProgress("CgkIgoby7MoHEAIQAQ", 100.0f, (bool success) => { });
            gameOverScreen.SetActive(true);
            winLoseScreen.Show(true);
            musicController.Trigger("GameOver");

            // Calculate EXP per unit
            List<UnitStats> units = new List<UnitStats>();
            for (int i = 0; i < battleField.playerCells.Length; i++)
            {
                if (battleField.playerCells[i].unitStats != null && battleField.playerCells[i].unitStats.CurrHP > 0)
                {
                    units.Add(battleField.playerCells[i].unitStats);
                }
            }
            float expPerUnit = earnedEXP / units.Count;
            for (int i = 0; i < units.Count; i++)
            {
                bool levelUp = units[i].AddXP(Mathf.CeilToInt(expPerUnit));
                winLoseScreen.AddReward(units[i].Name + " gained " + Mathf.CeilToInt(expPerUnit) + " EXP.");
                if (levelUp)
                {
                    Social.ReportProgress("CgkIgoby7MoHEAIQAg", 100.0f, (bool success) => { });
                    winLoseScreen.AddReward(units[i].Name + " leveled up!");
                }
            }
            int currLevel = PlayerPrefs.GetInt("CurrLevel", 1);
            int highestLevel = PlayerPrefs.GetInt("HighestCompletedLevel", 1);
            if (currLevel >= highestLevel)
            {
                PlayerPrefs.SetInt("HighestCompletedLevel", currLevel);
                PlayerPrefs.Save();
            }
            teamManager.SaveMonsters();
            return;
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

    /// <summary>
    ///  Applies the effects of an action.
    /// </summary>
    void ApplyCurrentAction()
    {
        int dmg = 0;
        GameObject hitEffectInstance = null;
        HitScore hitScore = null;
        switch (currentAction.actionType)
        {
            case BattleAction.ActionType.Attack: // If the action is an attack
                dmg = Utilities.CalcPhysDamage(currentAction.unit, currentAction.target); // Calculate the damage
                currentAction.target.TakeDamage(dmg); // Apply the damage
                sfxController.Play("Melee"); // Play the melee sound effect
                hitEffectInstance = Instantiate(hitEffect, currentAction.target.transform.position, Quaternion.identity); // Create the hit effect
                hitScore = hitEffectInstance.GetComponent<HitScore>(); // Get the hit score component
                hitScore.SetText(dmg.ToString()); // Set the text
                hitScore.SetType(HitScore.Type.Damage); // Set the type
                break;
            case BattleAction.ActionType.Ability: // If the action is an ability
                currentAction.unit.UseMP(currentAction.ability.MPCost);
                switch (currentAction.ability.Type)
                {
                    case Ability.AbilityType.Attack: // If the ability is an attack
                        if (currentAction.ability.IsMagic) // If it is magical
                        {
                            dmg = Mathf.FloorToInt(Utilities.CalcMagicDamage(currentAction.unit, currentAction.target) * currentAction.ability.EffectMult); // Calculate the damage
                            sfxController.Play("Magic"); // Play the magic sound effect
                        }
                        else // Otherwise
                        {
                            dmg = Mathf.FloorToInt(Utilities.CalcPhysDamage(currentAction.unit, currentAction.target) * currentAction.ability.EffectMult); // Calculate the damage
                            sfxController.Play("Melee"); // Play the melee sound effect
                        }
                        currentAction.target.TakeDamage(dmg); // Apply the damage
                        hitEffectInstance = Instantiate(hitEffect, currentAction.target.transform.position, Quaternion.identity); // Create the hit effect
                        hitScore = hitEffectInstance.GetComponent<HitScore>(); // Get the hit score component
                        hitScore.SetText(dmg.ToString()); // Set the text
                        hitScore.SetType(HitScore.Type.Damage); // Set the type
                        currentAction.target.ApplyDebuff(currentAction.ability.Effect); // Apply the debuff
                        break;
                    case Ability.AbilityType.Heal: // If the ability is a heal
                        int heal = Mathf.FloorToInt(Utilities.CalcHeal(currentAction.unit) * currentAction.ability.EffectMult); // Calculate the heal
                        currentAction.target.Heal(heal); // Apply the heal
                        hitEffectInstance = Instantiate(hitEffect, currentAction.target.transform.position, Quaternion.identity); // Create the hit effect
                        hitScore = hitEffectInstance.GetComponent<HitScore>(); // Get the hit score component
                        hitScore.SetText(heal.ToString()); // Set the text
                        hitScore.SetType(HitScore.Type.Heal); // Set the type
                        break;
                    case Ability.AbilityType.Buff: // If the ability is a buff
                        currentAction.target.ApplyBuff(currentAction.ability.Effect); // Apply the buff
                        break;
                    case Ability.AbilityType.Debuff: // If the ability is a debuff
                        currentAction.target.ApplyDebuff(currentAction.ability.Effect); // Apply the debuff
                        break;
                }

                /// Apply buff and debuff effects
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
                    StartCoroutine(DeathAnimation(currentAction.target.gameObject)); // Play the death animation
                    battleUI.SetDead(currentAction.targetIndex, false);
                }
                else
                {
                    float xpVal = Mathf.Pow((currentAction.target.Level * 2f), 1.2f); // Calculate the xp value
                    xpVal += (xpVal / 10f * Random.Range(-1f, 1f)); // Add a random value
                    earnedEXP += xpVal; // Add the xp value to the earned xp
                    StartCoroutine(DeathAnimation(currentAction.target.gameObject)); // Play the death animation
                    battleUI.SetDead(currentAction.targetIndex, true);
                }
            }
        }
    }

    /// <summary>
    ///  Plays the death animation
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
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

    /// <summary>
    ///  Ends the battle
    /// </summary>
    void EndRound()
    {
        currentlyBattling = false;
        //battleDirector.EndBattle();
        battleUI.ClearActions();
        battleUI.SetMainMenuActive(true);
        musicController.Trigger("Action Select");
        OnRoundEnd?.Invoke();
    }


    /// <summary>
    ///  Initializes the battle
    /// </summary>
    void InitialiseBattle()
    {
        // Spawn the player units
        for (int i = 0; i < teamManager.playerUnits.Length; i++)
        {
            if (teamManager.playerUnits[i] == null) continue;
            teamManager.playerUnits[i].Heal(9999999);
            teamManager.playerUnits[i].RestoreMP(9999999);
            battleField.TrySpawnUnit(teamManager.playerUnits[i], ref battleField.playerCells);
            battleUI.CreateUnitInfoPanel(battleField.playerCells[i].unitStats);
        }

        // Set the player unit names
        string[] playerNames = { "", "", "" };
        for (int i = 0; i < teamManager.playerUnits.Length; i++)
        {
            if (battleField.playerCells[i].unitStats == null) continue;
            playerNames[i] = battleField.playerCells[i].unitStats.Name;
        }
        battleUI.SetAllyNames(playerNames);

        //Get Enemy Level Range
        int midLevel = PlayerPrefs.GetInt("CurrLevel", 1) * PlayerPrefs.GetInt("LevelInterval", 1);
        Debug.Log("Mid Level: " + midLevel);
        int minLevel = midLevel - 2;
        int maxLevel = midLevel;
        if (minLevel < 1) minLevel = 1;

        // Decide how many enemies to spawn between 1 and 3
        int enemyCount = Random.Range(1, 4);
        for (int i = 0; i < enemyCount; i++)
        {
            // Decide which enemy to spawn
            int enemyIndex = Random.Range(0, enemyData.speciesList.Count);
            battleField.TrySpawnUnit(enemyData.speciesList[enemyIndex].baseUnit, ref battleField.enemyCells, Random.Range(minLevel, maxLevel));
            Debug.Log("Spawning " + battleField.enemyCells[i].unitStats.Name + " at level " + battleField.enemyCells[i].unitStats.Level);
        }

        // Apply animation offsets
        for (int i = 0; i < battleField.playerCells.Length; i++)
        {
            if (battleField.playerCells[i].unit != null && battleField.playerCells[i].unit.GetComponent<Animator>())
            {
                battleField.playerCells[i].unit.GetComponent<Animator>()?.SetFloat("Offset", i / 3f);
            }
        }

        for (int i = 0; i < battleField.enemyCells.Length; i++)
        {
            if (battleField.enemyCells[i].unit != null && battleField.enemyCells[i].unit.GetComponent<Animator>())
            {
                battleField.enemyCells[i].unit.GetComponent<Animator>()?.SetFloat("Offset", i / 3f);
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
