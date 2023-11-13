using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingStatueFighter : MonoBehaviour
{
    [SerializeField] private Transform headTransform, leftHandTransform, rightHandTransform;
    [SerializeField] private Transform fighterVisuals;

    private Vector3 initialHeadPosition, initialLeftHandPosition, initialRightHandPosition;
    private Quaternion initialHeadRotation, initialLeftHandRotation, initialRightHandRotation;

    private void Awake()
    {
        initialHeadPosition = headTransform.position;
        initialLeftHandPosition = leftHandTransform.position;
        initialRightHandPosition = rightHandTransform.position;

        initialHeadRotation = headTransform.rotation;
        initialLeftHandRotation = leftHandTransform.rotation;
        initialRightHandRotation = rightHandTransform.rotation;
    }

    public void MoveHeadToTargetLocation(Vector3 targetPosition, float time)
    {
        Vector3.Lerp(initialHeadPosition, targetPosition, time);
    }

    public void MoveLeftHandToTargetLocation(Vector3 targetPosition, float time)
    {
        Vector3.Lerp(initialLeftHandPosition, targetPosition, time);
    }

    public void MoveRightHandToTargetLocation(Vector3 targetPosition, float time)
    {
        Vector3.Lerp(initialRightHandPosition, targetPosition, time);
    }

    IEnumerator MoveToTargetPosition(Vector3 targetHeadPosition, Vector3 targetLeftHandPosition, Vector3 targetRightHandPosition, float time)
    {
        float timeElapsed = 0;

        while (timeElapsed < time)
        {
            headTransform.position = Vector3.Lerp(initialHeadPosition, targetHeadPosition, timeElapsed / time);

            leftHandTransform.position = Vector3.Lerp(initialLeftHandPosition, targetLeftHandPosition, timeElapsed / time);

            rightHandTransform.position = Vector3.Lerp(initialRightHandPosition, targetRightHandPosition, timeElapsed / time);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        FighterLoader.Instance.InvokeOnFighterInPosition();
        
    }

    public void MoveToStartPosition(float time)
    {
        StopAllCoroutines();

        Vector3 headTargetPosition = ReplayManager.Instance.GetHeadTransformLogs()[0].Position;

        Vector3 targetPosition = new Vector3(headTargetPosition.x, 0, headTargetPosition.z);
        Vector3 leftHandTargetPosition = ReplayManager.Instance.GetLeftHandTransformLogs()[0].Position;
        Vector3 rightHandTargetPosition = ReplayManager.Instance.GetRightHandTransformLogs()[0].Position;

        //StartCoroutine(MoveToTargetPosition(headTargetPosition, leftHandTargetPosition, rightHandTargetPosition, time));
        StartCoroutine(MoveFighterVisualsToTargetPosition(targetPosition, time));
    }

    IEnumerator MoveFighterVisualsToTargetPosition(Vector3 targetPosition, float time)
    {
        float timeElapsed = 0;

        while (timeElapsed < time)
        {
            fighterVisuals.position = Vector3.Lerp(fighterVisuals.position, targetPosition, timeElapsed / time);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        FighterLoader.Instance.InvokeOnFighterInPosition();
    }

    public void ResetPositions()
    {
        /*
        headTransform.position = initialHeadPosition;
        headTransform.rotation = initialHeadRotation;

        leftHandTransform.position = initialLeftHandPosition;
        leftHandTransform.rotation = initialLeftHandRotation;

        rightHandTransform.position = initialRightHandPosition;
        rightHandTransform.rotation = initialRightHandRotation;
        */

        fighterVisuals.localPosition = Vector3.zero;
    }
}
