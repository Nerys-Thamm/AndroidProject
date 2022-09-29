using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability", menuName = "GameData/Combat/Create Ability")]
public class Ability : ScriptableObject
{
    [Header("Ability Info")]
    [SerializeField] private string _abilityName;
    public string Name { get { return _abilityName; } }
    [SerializeField] private string _description;
    public string Description { get { return _description; } }
    [SerializeField] private int _mpCost;
    public int MPCost { get { return _mpCost; } }
    [SerializeField] private float _effectMult = 1f;
    public float EffectMult { get { return _effectMult; } }

    public enum AbilityType
    {
        Attack,
        Heal,
        Buff,
        Debuff
    }

    public enum TargetType
    {
        Single,
        All,
        Self
    }

    [SerializeField] private AbilityType _type;
    public AbilityType Type { get { return _type; } }
    [SerializeField] private TargetType _target;
    public TargetType Target { get { return _target; } }

    [SerializeField] private bool _isMagic;
    public bool IsMagic { get { return _isMagic; } }

    [Flags]
    public enum EffectMask : byte
    {
        None = 0,
        HP = 1,
        MP = 2,
        STR = 4,
        DEF = 8,
        AGI = 16,
        INT = 32
    }

    [SerializeField] private EffectMask _effectMask;
    public EffectMask Effect { get { return _effectMask; } }

    [System.Serializable]
    public class SerializedAbility
    {
        public string abilityName;
        public string description;
        public int mpCost;
        public float effectMult;
        public AbilityType type;
        public TargetType target;
        public bool isMagic;
        public EffectMask effectMask;

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
