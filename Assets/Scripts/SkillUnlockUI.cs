using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillUnlockUI : MonoBehaviour
{
    public TMP_Text skillName;
    public TMP_Text skillSP;
    public Color unlockedColor;
    public Color lockedColor;

    public void SetContent(string name, int sp, bool unlocked)
    {
        skillName.text = name;
        skillSP.text = sp.ToString() + "SP";
        if (unlocked)
        {
            skillName.color = unlockedColor;
            skillSP.color = unlockedColor;
        }
        else
        {
            skillName.color = lockedColor;
            skillSP.color = lockedColor;
        }
    }
}
