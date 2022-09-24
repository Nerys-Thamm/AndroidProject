using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleField : MonoBehaviour
{
    public Vector3 offset;

    public float cellRadius;

    public float teamDistance;

    public struct Cell
    {
        public Vector3 position;
        public GameObject unit;
    }

    public Cell[] playerCells;
    public Cell[] enemyCells;

    public System.Action OnReady;

    // Start is called before the first frame update
    void Start()
    {
        playerCells = new Cell[3];
        enemyCells = new Cell[3];

        for (int i = 0; i < 3; i++)
        {
            playerCells[i].position = new Vector3(offset.x + i * cellRadius * 2, offset.y, offset.z) + transform.position;
            enemyCells[i].position = new Vector3(offset.x + i * cellRadius * 2, offset.y, offset.z + teamDistance) + transform.position;
        }

        OnReady?.Invoke();
    }

    public bool TrySpawnUnit(GameObject unit, ref Cell[] cells)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].unit == null)
            {
                cells[i].unit = Instantiate(unit, cells[i].position, Quaternion.identity);
                return true;
            }
        }
        return false;
    }


    // Update is called once per frame
    void Update()
    {

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
