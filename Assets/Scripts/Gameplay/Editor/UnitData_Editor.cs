using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
}
