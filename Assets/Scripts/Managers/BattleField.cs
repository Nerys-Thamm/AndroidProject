using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Class representing the battlefield made of two rows of 3 tiles.
/// </summary>
public class BattleField : MonoBehaviour
{
    public SpeciesData speciesData; //Species data
    public Vector3 offset; // The offset from the world origin

    public float cellRadius; // The cell radius

    public float teamDistance; // The distance between the two teams

    /// <summary>
    ///  A cell on the battlefield.
    /// </summary>
    public struct Cell
    {
        public Vector3 position; // The cell's position
        public GameObject unit; // The unit in the cell
        public UnitStats unitStats; // The unit's stats
        public bool isDefending; // Whether the unit is defending
    }

    public Cell[] playerCells; // The player's cells
    public Cell[] enemyCells; // The enemy's cells

    public System.Action OnReady; // Event for when the battlefield is ready

    // Start is called before the first frame update
    void Start()
    {
        playerCells = new Cell[3]; // Initialize the player's cells
        enemyCells = new Cell[3]; // Initialize the enemy's cells

        for (int i = 0; i < 3; i++)
        {
            playerCells[i].position = new Vector3(offset.x + i * cellRadius * 2, offset.y, offset.z) + transform.position; // Set the player's cell's position
            enemyCells[i].position = new Vector3(offset.x + i * cellRadius * 2, offset.y, offset.z + teamDistance) + transform.position; // Set the enemy's cell's position
        }

        OnReady?.Invoke(); // Invoke the ready event
    }

    /// <summary>
    ///   Attempts to place a unit in a cell.
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="cells"></param>
    /// <returns></returns>
    public bool TrySpawnUnit(UnitData unit, ref Cell[] cells)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].unit == null) //If the cell is empty
            {
                cells[i].unit = Instantiate(speciesData.GetSpecies(unit.unitSpecies).prefab, cells[i].position, Quaternion.identity); // Spawn the unit
                cells[i].unitStats = cells[i].unit.GetComponent<UnitStats>(); // Get the unit's stats
                cells[i].unitStats.SetUnitData(unit); // Set the unit's data

                return true;
            }
        }
        return false;
    }

    /// <summary>
    ///  Attempts to spawn a unit with a given level.
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="cells"></param>
    /// <param name="level"></param>
    /// <returns></returns>
    public bool TrySpawnUnit(UnitData unit, ref Cell[] cells, int level)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].unit == null) //If the cell is empty
            {
                cells[i].unit = Instantiate(speciesData.GetSpecies(unit.unitSpecies).prefab, cells[i].position, Quaternion.identity); // Spawn the unit
                cells[i].unitStats = cells[i].unit.GetComponent<UnitStats>(); // Get the unit's stats
                cells[i].unitStats.SetNewUnitData(unit, level); // Set the unit's data

                return true;
            }
        }
        return false;
    }


    void OnDrawGizmos()
    {
        // Draw three cells in a row for each team
        for (int i = 0; i < 3; i++)
        {
            // Draw the first team
            DrawCellGizmo(i, 0);
            // Draw the second team
            DrawCellGizmo(i, 1);
        }
    }

    void DrawCellGizmo(int x, int team)
    {
        Gizmos.color = Color.red;
        // Calculate the position of the cell
        Vector3 position = transform.position + offset + new Vector3(x * cellRadius * 2, 0, team * teamDistance);
        // Draw the cell
        Gizmos.DrawWireCube(position, new Vector3(cellRadius * 2, 0.1f, cellRadius * 2));
        Gizmos.color = Color.white;
    }
}
