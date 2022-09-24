using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private string _description;
    [SerializeField] private int _mpCost;
    [SerializeField] private float _effectMult;
    []
    [SerializeField] private bool _targetEnemies;
    [SerializeField] private bool _targetSelf;
    [SerializeField] private bool _targetAllies;
    [SerializeField] private bool _multiTarget;
}
