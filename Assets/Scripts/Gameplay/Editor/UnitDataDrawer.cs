using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(UnitData))]
public class UnitDataDrawer : PropertyDrawer
{
    // public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    // {
    //     EditorGUI.BeginProperty(position, label, property);

    //     // Draw label
    //     position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

    //     // Don't make child fields be indented
    //     var indent = EditorGUI.indentLevel;
    //     EditorGUI.indentLevel = 0;

    //     // Calculate rects
    //     var nameRect = new Rect(position.x, position.y, 100, position.height);
    //     var hpRect = new Rect(position.x + 105, position.y, 30, position.height);
    //     var mpRect = new Rect(position.x + 140, position.y, 30, position.height);
    //     var strRect = new Rect(position.x + 175, position.y, 30, position.height);
    //     var defRect = new Rect(position.x + 210, position.y, 30, position.height);
    //     var agiRect = new Rect(position.x + 245, position.y, 30, position.height);
    //     var intRect = new Rect(position.x + 280, position.y, 30, position.height);

    //     SerializedProperty attr = property.serializedObject.FindProperty("attributes");

    //     int currentHP = attr.FindPropertyRelative("_HP_Base").intValue + Mathf.FloorToInt(attr.FindPropertyRelative("_HP_Growth").floatValue * (property.FindPropertyRelative("level").intValue - 1));
    //     int currentMP = attr.FindPropertyRelative("_MP_Base").intValue + Mathf.FloorToInt(attr.FindPropertyRelative("_MP_Growth").floatValue * (property.FindPropertyRelative("level").intValue - 1));
    //     int currentSTR = attr.FindPropertyRelative("_STR_Base").intValue + Mathf.FloorToInt(attr.FindPropertyRelative("_STR_Growth").floatValue * (property.FindPropertyRelative("level").intValue - 1));
    //     int currentDEF = attr.FindPropertyRelative("_DEF_Base").intValue + Mathf.FloorToInt(attr.FindPropertyRelative("_DEF_Growth").floatValue * (property.FindPropertyRelative("level").intValue - 1));
    //     int currentAGI = attr.FindPropertyRelative("_AGI_Base").intValue + Mathf.FloorToInt(attr.FindPropertyRelative("_AGI_Growth").floatValue * (property.FindPropertyRelative("level").intValue - 1));
    //     int currentINT = attr.FindPropertyRelative("_INT_Base").intValue + Mathf.FloorToInt(attr.FindPropertyRelative("_INT_Growth").floatValue * (property.FindPropertyRelative("level").intValue - 1));



    //     // Draw readonly fields for current stats in a table
    //     EditorGUI.LabelField(nameRect, property.FindPropertyRelative("name").stringValue);
    //     EditorGUI.LabelField(hpRect, currentHP.ToString());
    //     EditorGUI.LabelField(mpRect, currentMP.ToString());
    //     EditorGUI.LabelField(strRect, currentSTR.ToString());
    //     EditorGUI.LabelField(defRect, currentDEF.ToString());
    //     EditorGUI.LabelField(agiRect, currentAGI.ToString());
    //     EditorGUI.LabelField(intRect, currentINT.ToString());



    //     // Set indent back to what it was
    //     EditorGUI.indentLevel = indent;

    //     EditorGUI.EndProperty();
    // }
}

