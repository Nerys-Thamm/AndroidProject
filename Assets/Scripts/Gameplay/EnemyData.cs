using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    public int minLevel;
    public int maxLevel;
    public List<GameObject> units = new List<GameObject>();
}
