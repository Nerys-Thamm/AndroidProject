using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
///  Controls camera movement, animations, and effects.
/// </summary>
public class BattleDirector : MonoBehaviour
{
    [SerializeField] BattleField battleField; // The battle field
    [SerializeField] GameObject magicEffectPrefab; // The magic effect prefab
    [SerializeField] AudioClip meleeSound, magicSound; // The melee and magic sound effects
    [SerializeField] CinemachineVirtualCamera directedCam; // The directed camera
    public Transform defaultCamTarget; // The default camera target

    public System.Action OnTargetReached, OnAnimationComplete; // Events for when the target is reached and the animation is complete

    /// <summary>
    ///  Starts the battle.
    /// </summary>
    /// <param name="action"></param>
    public void StartBattle(BattleManager.BattleAction action)
    {
        StopAllCoroutines();
        StartCoroutine(PlayAction(action));
    }

    /// <summary>
    ///  Plays the battle action.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    IEnumerator PlayAction(BattleManager.BattleAction action)
    {
        Vector3 unitInitPos = action.unit.transform.position;  // The unit's initial position
        Vector3 targetPos = action.target.transform.position;  // The target's position
        Vector3 unitTargetPos = Vector3.Lerp(unitInitPos, targetPos, 0.9f); // The unit's target position
        Quaternion unitInitRot = action.unit.transform.rotation; // The unit's initial rotation
        Quaternion targetRot = Quaternion.LookRotation(targetPos - unitInitPos); // The target's rotation
        directedCam.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 0.5f; // Set the camera's path position
        directedCam.m_Priority = 11; // Set the camera's priority
        directedCam.m_Follow = action.unit.transform; // Set the camera's follow target
        directedCam.m_LookAt = action.unit.transform; // Set the camera's look at target
        yield return new WaitForSeconds(2f); // Wait for 2 seconds
        if (action.actionType == BattleManager.BattleAction.ActionType.Attack || (action.actionType == BattleManager.BattleAction.ActionType.Ability && action.ability.Type == Ability.AbilityType.Attack))
        {

            float t = 0; // The time


            while (t < 1)
            {
                t += Time.deltaTime * 0.6f; // Increment the time
                action.unit.transform.position = Vector3.Lerp(unitInitPos, unitTargetPos, t); // Move the unit
                action.unit.transform.rotation = Quaternion.Lerp(unitInitRot, targetRot, t); // Rotate the unit
                yield return null; // Wait for the next frame
            }

            OnTargetReached?.Invoke(); // Invoke the target reached event
            directedCam.m_LookAt = action.target.transform; // Set the camera's look at target
            directedCam.m_Follow = action.target.transform; // Set the camera's follow target

            t = 0; // Reset the time

            while (t < 1)
            {
                t += Time.deltaTime * 0.6f; // Increment the time
                action.unit.transform.position = Vector3.Lerp(unitTargetPos, unitInitPos, t); // Move the unit
                action.unit.transform.rotation = Quaternion.Lerp(targetRot, unitInitRot, t); // Rotate the unit
                yield return null; // Wait for the next frame
            }

            yield return new WaitForSeconds(1f); // Wait for 1 second

        }
        else
        {
            // Point camera at unit and spawn magic effect
            GameObject magicEffect = Instantiate(magicEffectPrefab, action.unit.transform.position, Quaternion.identity);
            directedCam.m_Follow = action.unit.transform;
            directedCam.m_LookAt = action.unit.transform;

            // Wait for magic effect to finish
            yield return new WaitForSeconds(2.5f);

            // Destroy magic effect
            Destroy(magicEffect);

            // Point camera at target
            directedCam.m_Follow = action.target.transform;
            directedCam.m_LookAt = action.target.transform;

            // Wait for magic effect to finish
            yield return new WaitForSeconds(1.5f);

            OnTargetReached?.Invoke();

            // Wait for a bit
            yield return new WaitForSeconds(2.5f);

        }
        directedCam.m_Priority = 1;
        yield return new WaitForSeconds(0.5f);
        directedCam.m_Follow = defaultCamTarget;
        directedCam.m_LookAt = defaultCamTarget;
        OnAnimationComplete?.Invoke();

    }
}
