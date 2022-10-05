using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
///  Custom editor for <see cref="UnitData"/>.
/// </summary>
[CustomEditor(typeof(UnitData))]
public class UnitData_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        UnitData unitData = (UnitData)target;

        // Draw the default inspector
        DrawDefaultInspector();

        // Display the current values of the attributes
        EditorGUILayout.LabelField("Current Values", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("HP", unitData.attributes.HP(unitData.Level).ToString());
        EditorGUILayout.LabelField("MP", unitData.attributes.MP(unitData.Level).ToString());
        EditorGUILayout.LabelField("STR", unitData.attributes.STR(unitData.Level).ToString());
        EditorGUILayout.LabelField("DEF", unitData.attributes.DEF(unitData.Level).ToString());
        EditorGUILayout.LabelField("AGI", unitData.attributes.AGI(unitData.Level).ToString());
        EditorGUILayout.LabelField("INT", unitData.attributes.INT(unitData.Level).ToString());
    }

    [MenuItem("CONTEXT/Create/UnitData")]
    public void CreateFromBase(MenuCommand command)
    {
        UnitData unitData = (UnitData)command.context;

        // Create a new instance of the base unit
        UnitData newUnit = UnitData.CreateFromBase(unitData);

        // Set the name of the new unit
        newUnit.unitName = "New " + newUnit.unitSpecies;

        // Save the new unit as a new asset in the project
        AssetDatabase.CreateAsset(newUnit, "Assets/Units/" + newUnit.unitName + ".asset");
        AssetDatabase.SaveAssets();

    }
}
