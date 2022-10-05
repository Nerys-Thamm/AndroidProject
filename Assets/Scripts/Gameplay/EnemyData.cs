using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Deprecated class. Use <see cref="SpeciesData"/> instead.
/// </summary>
[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    public int minLevel;
    public int maxLevel;
    public List<UnitData> units = new List<UnitData>();
}
