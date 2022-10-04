using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SpeciesData", menuName = "GameData/SpeciesData")]
public class SpeciesData : ScriptableObject
{
    [System.Serializable]
    public struct Species
    {
        public string name;
        public UnitData baseUnit;
        public GameObject prefab;
    }

    public List<Species> speciesList = new List<Species>();

    public Species GetSpecies(string name)
    {
        return speciesList.Find(x => x.name == name);
    }
}
