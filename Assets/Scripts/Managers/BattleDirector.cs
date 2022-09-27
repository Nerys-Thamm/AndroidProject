using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BattleDirector : MonoBehaviour
{
    [SerializeField] BattleField battleField;
    [SerializeField] AnimationCurve leapMoveCurve, hopMoveCurve, walkMoveCurve;
    [SerializeField] GameObject magicEffectPrefab;
    [SerializeField] AudioClip meleeSound, magicSound;
    [SerializeField] CinemachineVirtualCamera directedCam;
    public Transform defaultCamTarget;

    public System.Action OnTargetReached, OnAnimationComplete;

    public void StartBattle(BattleManager.BattleAction action)
    {
        StartCoroutine(PlayAction(action));
    }


    IEnumerator PlayAction(BattleManager.BattleAction action)
    {
        Vector3 unitInitPos = action.unit.transform.position;
        Vector3 targetPos = action.target.transform.position;
        Vector3 unitTargetPos = Vector3.Lerp(unitInitPos, targetPos, 0.9f);
        Quaternion unitInitRot = action.unit.transform.rotation;
        Quaternion targetRot = Quaternion.LookRotation(targetPos - unitInitPos);
        directedCam.GetCinemachineComponent<CinemachineTrackedDolly>().m_PathPosition = 0.5f;
        directedCam.m_Priority = 11;
        directedCam.m_Follow = action.unit.transform;
        directedCam.m_LookAt = action.unit.transform;
        yield return new WaitForSeconds(2f);
        if (action.actionType == BattleManager.BattleAction.ActionType.Attack || (action.actionType == BattleManager.BattleAction.ActionType.Ability && !action.ability.IsMagic))
        {

            float t = 0;


            while (t < 1)
            {
                t += Time.deltaTime * 0.6f;
                action.unit.transform.position = Vector3.Lerp(unitInitPos, unitTargetPos, t);
                action.unit.transform.rotation = Quaternion.Lerp(unitInitRot, targetRot, t);
                yield return null;
            }

            OnTargetReached?.Invoke();
            directedCam.m_LookAt = action.target.transform;
            directedCam.m_Follow = action.target.transform;

            t = 0;

            while (t < 1)
            {
                t += Time.deltaTime * 0.6f;
                action.unit.transform.position = Vector3.Lerp(unitTargetPos, unitInitPos, t);
                action.unit.transform.rotation = Quaternion.Lerp(targetRot, unitInitRot, t);
                yield return null;
            }
            directedCam.m_Follow = defaultCamTarget;
            directedCam.m_LookAt = defaultCamTarget;
        }
        else
        {
            // Point camera at unit and spawn magic effect
            GameObject magicEffect = Instantiate(magicEffectPrefab, action.unit.transform.position, Quaternion.identity);
            directedCam.m_Follow = action.unit.transform;
            directedCam.m_LookAt = action.unit.transform;

            // Wait for magic effect to finish
            yield return new WaitForSeconds(1.5f);

            // Destroy magic effect
            Destroy(magicEffect);

            // Point camera at target
            directedCam.m_Follow = action.target.transform;
            directedCam.m_LookAt = action.target.transform;

            // Wait for magic effect to finish
            yield return new WaitForSeconds(1.5f);

            OnTargetReached?.Invoke();

            // Wait for a bit
            yield return new WaitForSeconds(0.5f);

            // Point camera at nothing
            directedCam.m_Follow = defaultCamTarget;
            directedCam.m_LookAt = defaultCamTarget;
        }
        directedCam.m_Priority = 1;
        yield return new WaitForSeconds(0.5f);
        OnAnimationComplete?.Invoke();

    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
