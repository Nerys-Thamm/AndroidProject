using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Stores all of the species in the game, their prefabs, names, and unit data.
/// </summary>
[CreateAssetMenu(fileName = "New SpeciesData", menuName = "GameData/SpeciesData")]
public class SpeciesData : ScriptableObject
{
    /// <summary>
    ///  The species in the game.
    /// </summary>
    [System.Serializable]
    public struct Species
    {
        public string name; // The name of the species
        public UnitData baseUnit; // The base unit data for the species
        public GameObject prefab; // The prefab for the species
    }

    public List<Species> speciesList = new List<Species>(); // The list of species in the game

    /// <summary>
    ///  Get the species with the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Species GetSpecies(string name)
    {
        return speciesList.Find(x => x.name == name);
    }
}
