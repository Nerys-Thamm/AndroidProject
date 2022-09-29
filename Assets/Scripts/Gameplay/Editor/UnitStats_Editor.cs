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
        EditorGUILayout.LabelField("Unit Name", unitStats.Name);
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

        //Current Stats
        EditorGUILayout.LabelField("Current Stats", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Current HP");
        //Progress bar for HP
        float hpPercent = (float)unitStats.CurrHP / (float)unitStats.MaxHP;
        EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(), hpPercent, unitStats.CurrHP.ToString() + "/" + unitStats.MaxHP.ToString());
        EditorGUILayout.LabelField("Current MP");
        //Progress bar for MP
        float mpPercent = (float)unitStats.CurrMP / (float)unitStats.MaxMP;
        EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(), mpPercent, unitStats.CurrMP.ToString() + "/" + unitStats.MaxMP.ToString());

        EditorGUILayout.Space();

        base.OnInspectorGUI();
    }
}
