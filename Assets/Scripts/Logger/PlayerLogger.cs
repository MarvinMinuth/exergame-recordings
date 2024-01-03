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

        Logger.Instance.OnLogging += Logger_OnLogging;
    }

    private void Logger_OnLogging(object sender, System.EventArgs e)
    {
        LogTransforms();
    }

    private void LogTransforms()
    {
        // Log HMD
        log.setTransformType(Logger.TransformType.HMD);
        log.setValuesByTransform(hmdTransform);
        Logger.Instance.LogOnLogging(log);

        //Log LeftController
        log.setTransformType(Logger.TransformType.LeftController);
        log.setValuesByTransform(leftControllerTransform);
        Logger.Instance.LogOnLogging(log);

        //Log RightController
        log.setTransformType(Logger.TransformType.RightController);
        log.setValuesByTransform(rightControllerTransform);
        Logger.Instance.LogOnLogging(log);
    }
}
