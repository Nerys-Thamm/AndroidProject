using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnitStats))]
public class UnitStats_Editor : Editor
{

    public override void OnInspectorGUI()
    {


        UnitStats unitStats = (UnitStats)target;


        EditorGUILayout.LabelField("Unit Info", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Unit Name", unitStats.name);
        EditorGUILayout.LabelField("Unit Level", unitStats.Level.ToString());

        //Data

        EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Max HP", unitStats.MaxHP.ToString());
        EditorGUILayout.LabelField("Curr HP", unitStats.CurrHP.ToString());
        EditorGUILayout.LabelField("Max MP", unitStats.MaxMP.ToString());
        EditorGUILayout.LabelField("Curr MP", unitStats.CurrMP.ToString());
        EditorGUILayout.LabelField("STR", unitStats.STR.ToString());
        EditorGUILayout.LabelField("DEF", unitStats.DEF.ToString());
        EditorGUILayout.LabelField("AGI", unitStats.AGI.ToString());
        EditorGUILayout.LabelField("INT", unitStats.INT.ToString());

        EditorGUILayout.Space();




        base.OnInspectorGUI();
    }
}
