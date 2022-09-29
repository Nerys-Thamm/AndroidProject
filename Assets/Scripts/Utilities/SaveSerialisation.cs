using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveSerialisation
{
    // Get instance of the save manager singleton
    public static SaveSerialisation Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SaveSerialisation();
            }
            return instance;
        }
    }
    private static SaveSerialisation instance;
    // Save data
    [System.Serializable]
    public class SaveData
    {
        [SerializeField] public List<UnitData.SerializedUnitData> playerCreatureStorage;
        [SerializeField] public UnitData.SerializedUnitData PartyMonsterA;
        [SerializeField] public UnitData.SerializedUnitData PartyMonsterB;
        [SerializeField] public UnitData.SerializedUnitData PartyMonsterC;
    }

    private SaveData saveData;


    // Save file path
    private string saveFilePath;

    // Constructor
    private SaveSerialisation()
    {
        saveFilePath = Application.persistentDataPath + "/saveData.json";
        saveData = null;
    }

    // Serialise and save to file using binary formatter
    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/save.dat");
        bf.Serialize(file, saveData);
        file.Close();
    }

    // Load from file using binary formatter
    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/save.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/save.dat", FileMode.Open);
            saveData = (SaveData)bf.Deserialize(file);
            file.Close();
        }
        else // If no save file exists, create a new one
        {
            saveData = new SaveData();
            Save();
        }
    }

    public void DeleteSave()
    {
        if (File.Exists(Application.persistentDataPath + "/save.dat"))
        {
            File.Delete(Application.persistentDataPath + "/save.dat");
        }
    }

    // Save data getters and setters converting to and from serialised data
    public List<UnitData> PlayerCreatureStorage
    {
        get
        {
            List<UnitData> playerCreatureStorage = new List<UnitData>();
            foreach (UnitData.SerializedUnitData serializedUnitData in saveData.playerCreatureStorage)
            {
                playerCreatureStorage.Add(serializedUnitData.Deserialise());
            }
            return playerCreatureStorage;
        }
        set
        {
            List<UnitData.SerializedUnitData> playerCreatureStorage = new List<UnitData.SerializedUnitData>();
            foreach (UnitData unitData in value)
            {
                playerCreatureStorage.Add(new UnitData.SerializedUnitData(unitData));
            }
            saveData.playerCreatureStorage = playerCreatureStorage;
        }
    }

    public UnitData PartyMonsterA
    {
        get
        {
            return saveData.PartyMonsterA.Deserialise();
        }
        set
        {
            saveData.PartyMonsterA = new UnitData.SerializedUnitData(value);
        }
    }

    public UnitData PartyMonsterB
    {
        get
        {
            return saveData.PartyMonsterB.Deserialise();
        }
        set
        {
            saveData.PartyMonsterB = new UnitData.SerializedUnitData(value);
        }
    }

    public UnitData PartyMonsterC
    {
        get
        {
            return saveData.PartyMonsterC.Deserialise();
        }
        set
        {
            saveData.PartyMonsterC = new UnitData.SerializedUnitData(value);
        }
    }

    public void SaveMonsters(UnitData monsterA, UnitData monsterB, UnitData monsterC)
    {
        PartyMonsterA = monsterA;
        PartyMonsterB = monsterB;
        PartyMonsterC = monsterC;
        Save();
    }


}