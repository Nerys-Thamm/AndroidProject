using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
///  Stores data about an ability.
/// </summary>
[CreateAssetMenu(fileName = "New Ability", menuName = "GameData/Combat/Create Ability")]
public class Ability : ScriptableObject
{
    [Header("Ability Info")]
    [SerializeField] private string _abilityName; // Name of the ability
    public string Name { get { return _abilityName; } } // Name of the ability
    [SerializeField] private string _description; // Description of the ability
    public string Description { get { return _description; } } // Description of the ability
    [SerializeField] private int _mpCost; // MP cost of the ability
    public int MPCost { get { return _mpCost; } } // MP cost of the ability
    [SerializeField] private float _effectMult = 1f; // Multiplier for the effect of the ability
    public float EffectMult { get { return _effectMult; } } // Multiplier for the effect of the ability

    /// <summary>
    ///  The type of ability.
    /// </summary>
    public enum AbilityType
    {
        Attack, // Attack
        Heal, // Heal
        Buff, // Buff
        Debuff // Debuff
    }

    /// <summary>
    ///  The target of the ability.
    /// </summary>
    public enum TargetType
    {
        Single, // Single target
        All, // All targets
        Self // Self
    }

    [SerializeField] private AbilityType _type; // Type of ability
    public AbilityType Type { get { return _type; } } // Type of ability
    [SerializeField] private TargetType _target; // Target of the ability
    public TargetType Target { get { return _target; } } // Target of the ability

    [SerializeField] private bool _isMagic; // Whether the ability is magic or not
    public bool IsMagic { get { return _isMagic; } } // Whether the ability is magic or not

    /// <summary>
    ///  Mask for the abilities effect on stats.
    /// </summary>
    [Flags]
    public enum EffectMask : byte
    {
        None = 0, // No effect
        HP = 1, // Effect on HP
        MP = 2, // Effect on MP
        STR = 4, // Effect on STR
        DEF = 8, // Effect on DEF
        AGI = 16, // Effect on AGI
        INT = 32 // Effect on INT
    }

    [SerializeField] private EffectMask _effectMask; // Mask for the abilities effect on stats
    public EffectMask Effect { get { return _effectMask; } } // Mask for the abilities effect on stats

    /// <summary>
    ///  Serialized version of an ability.
    /// </summary>
    [System.Serializable]
    public class SerializedAbility
    {
        public string abilityName; // Name of the ability
        public string description; // Description of the ability
        public int mpCost; // MP cost of the ability
        public float effectMult; // Multiplier for the effect of the ability
        public AbilityType type; // Type of ability
        public TargetType target; // Target of the ability
        public bool isMagic; // Whether the ability is magic or not
        public EffectMask effectMask; // Mask for the abilities effect on stats

        /// <summary>
        ///  Creates a serialized ability from an ability.
        /// </summary>
        /// <param name="ability"></param>
        public SerializedAbility(Ability ability)
        {
            abilityName = ability._abilityName;
            description = ability._description;
            mpCost = ability._mpCost;
            effectMult = ability._effectMult;
            type = ability._type;
            target = ability._target;
            isMagic = ability._isMagic;
            effectMask = ability._effectMask;
        }

        /// <summary>
        ///  Creates an ability from a serialized ability.
        /// </summary>
        /// <returns></returns>
        public Ability GetAbility()
        {
            Ability ability = ScriptableObject.CreateInstance<Ability>();
            ability._abilityName = abilityName;
            ability._description = description;
            ability._mpCost = mpCost;
            ability._effectMult = effectMult;
            ability._type = type;
            ability._target = target;
            ability._isMagic = isMagic;
            ability._effectMask = effectMask;
            return ability;
        }
    }


}
