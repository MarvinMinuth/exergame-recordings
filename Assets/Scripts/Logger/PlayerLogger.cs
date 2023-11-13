using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogger : MonoBehaviour
{
    [SerializeField] private Transform hmdTransform, leftControllerTransform, rightControllerTransform;

    private TransformStudyLog log;

    private void Start()
    {
        log = new TransformStudyLog();
    }
    private void FixedUpdate()
    {
        // Log HMD
        log.setTransformType(Logger.TransformType.HMD);
        log.setValuesByTransform(hmdTransform);
        Logger.Instance.LogTransform(log);

        //Log LeftController
        log.setTransformType(Logger.TransformType.LeftController);
        log.setValuesByTransform(leftControllerTransform);
        Logger.Instance.LogTransform(log);

        //Log RightController
        log.setTransformType(Logger.TransformType.RightController);
        log.setValuesByTransform(rightControllerTransform);
        Logger.Instance.LogTransform(log);
    }
}
