using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.Burst.Intrinsics;
using static UnityEngine.PlayerLoop.PreLateUpdate;
using JetBrains.Annotations;
using System.Threading.Tasks;

[System.Serializable]
public class LogData
{
    public int logID;
    public Dictionary<string, BaseLog> logs;
}

[System.Serializable]
public class BaseLog
{
    public string __type;
}

[System.Serializable]
public class FightCollisionLog
{
    public string __type;
    public int ExpectedCollisionType;
    public int ReceiveCollisionType;
    public string armBase;
    public string hand;
    public int id;
    public int logType;
    public int prototype;
    public string Date_Time;
    public Dictionary<string, string> additionalData;
}

[System.Serializable]
public class ArmCollisionLog
{
    public string __type;
    public Dictionary<string, string> CollidedObject;
    public string armBase;
    public string hand;
    public int ExpectedCollisionType;
    public int ReceiveCollisionType;
    public Vector3 Velocity;
    public int id;
    public int logType;
    public int prototype;
    public string Date_Time;
    public Dictionary<string, string> additionalData;
}

[System.Serializable]
public class GameStateLog
{
    public int gameState;
    public int id;
    public int logType;
    public int prototype;
    public string Date_Time;
    public Dictionary<string, string> additionalData;
}

[System.Serializable]
public class TransformLog
{
    public int TransformType;
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;
    public int id;
    public int logType;
    public int prototype;
    public string Date_Time;
    public Dictionary<string, string> additionalData;
}

[System.Serializable]
public class ArmLog
{
    public string armType;
    public int armBase;
    public Transform targetTransform;
    public Vector3 targetPosition;
    public Vector3 targetRotation;
    public Vector3 targetScale;
    public Vector3 ankerPosition;
    public Vector3 ankerRotation;
    public Vector3 ankerScale;
    public int TransformType;
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;
    public int id;
    public int logType;
    public int prototype;
    public string Date_Time;
    public Dictionary<string, string> additionalData;
}

[System.Serializable]
public class HRLog
{
    public int connectionStatus;
    public int heartRate;
    public int id;
    public int logType;
    public int prototype;
    public string Date_Time;
    public Dictionary<string, string> additionalData;
}


public class LogDataManager : MonoBehaviour
{
    public string saveFile1, saveFile2;
    private string attachedToBottom, attachedToTop, attachedToMiddle;

    List<string> armLogs = new List<string>();
    List<string> transformLogs = new List<string>();

    List<ArmLog> bottomArmLogs = new List<ArmLog>();
    List<ArmLog> middleArmLogs = new List<ArmLog>();
    List<ArmLog> topArmLogs = new List<ArmLog>();

    List<TransformLog> headLogs = new List<TransformLog>();
    List<TransformLog> leftHandLogs = new List<TransformLog>();
    List<TransformLog> rightHandLogs = new List<TransformLog>();

    Dictionary<int, HRLog> HRLogs = new Dictionary<int, HRLog>();

    List<int> bottomArmHighlights = new List<int>();
    List<int> middleArmHighlights = new List<int>();
    List<int> topArmHighlights = new List<int>();

    Dictionary<int, ArmCollisionLog> armCollisionLogs = new Dictionary<int, ArmCollisionLog>();
    Dictionary<int, FightCollisionLog> fightCollisionLogs = new Dictionary<int, FightCollisionLog>();

    private bool logsReady;

    private void Start()
    {
        ES3.CacheFile(saveFile1);
        ES3.CacheFile(saveFile2);
    }

    public async void LoadReplay(int saveFile)
    {
        logsReady = false;

        await Task.Run(() =>
        {         
            var recording = new ES3Settings();

            if (saveFile == 1)
            {
                recording = new ES3Settings(saveFile1, ES3.Location.Cache);
            }
            else
            {
                recording = new ES3Settings(saveFile2, ES3.Location.Cache);
            }

            int frame = 0;
            foreach (string key in ES3.GetKeys(recording))
            {
                if (key.Contains("TransformLog"))
                {
                    TransformLog transformLog = ES3.Load<TransformLog>(key, recording);
                    if (transformLog.TransformType == 1)
                    {
                        headLogs.Add(transformLog);
                    }
                    else if (transformLog.TransformType == 2)
                    {
                        leftHandLogs.Add(transformLog);
                    }
                    else
                    {
                        rightHandLogs.Add(transformLog);
                    }
                }
                else if (key.Contains("ArmLog"))
                {
                    ArmLog armLog = ES3.Load<ArmLog>(key, recording);
                    if (armLog.armBase == 1)
                    {
                        bottomArmLogs.Add(armLog);
                        if (!armLog.armType.Equals(attachedToBottom))
                        {
                            bottomArmHighlights.Add(frame);
                            attachedToBottom = armLog.armType;
                        }
                    }
                    else if (armLog.armBase == 2)
                    {
                        middleArmLogs.Add(armLog);
                        if (!armLog.armType.Equals(attachedToMiddle))
                        {
                            middleArmHighlights.Add(frame);
                            attachedToMiddle = armLog.armType;
                        }
                    }
                    else
                    {
                        topArmLogs.Add(armLog);
                        if (!armLog.armType.Equals(attachedToTop))
                        {
                            topArmHighlights.Add(frame);
                            attachedToTop = armLog.armType;
                        }
                        frame++;
                    }
                }
                else if (key.Contains("ArmCollision"))
                {
                    // TODO: FCLogs nach top, mid, bot aufteilen; Kann sonst Probleme mit Keys geben
                    ArmCollisionLog armCollisionLog = ES3.Load<ArmCollisionLog>(key, recording);
                    if (armCollisionLogs.ContainsKey(frame))
                    {
                        armCollisionLogs.Remove(frame);
                    }
                    armCollisionLogs.Add(frame, armCollisionLog);
                }
                else if (key.Contains("FightCollision"))
                {
                    // TODO: FCLogs nach top, mid, bot aufteilen; Kann sonst Probleme mit Keys geben
                    FightCollisionLog fightCollisionLog = ES3.Load<FightCollisionLog>(key, recording);
                    if (fightCollisionLogs.ContainsKey(frame))
                    {
                        fightCollisionLogs.Remove(frame);
                    }
                    fightCollisionLogs.Add(frame, fightCollisionLog);
                }
                else if (key.Contains("HRLog"))
                {
                    HRLog hrLog = ES3.Load<HRLog>(key, recording);
                    if (HRLogs.ContainsKey(frame))
                    {
                        HRLogs.Remove(frame);
                    }
                    HRLogs.Add(frame, hrLog);
                }
                else
                {
                    continue;
                }
            }
        });

        attachedToBottom = "";
        attachedToTop = "";
        attachedToMiddle = "";

        logsReady = true;
    }

    public List<ArmLog> GetBottomArmLogs()   { return bottomArmLogs; }
    public List<int> GetBottomArmHighlights() { return bottomArmHighlights; }
    public List<ArmLog> GetMiddleArmLogs() { return middleArmLogs; }
    public List<int> GetMiddleArmHighlights() { return middleArmHighlights; }
    public List<ArmLog> GetTopArmLogs() { return topArmLogs; }
    public List <int> GetTopArmHighlights() { return topArmHighlights; }

    public List<TransformLog> GetHeadTransformLogs()
    {
        return headLogs;
    }
    public List<TransformLog> GetLeftHandTransformLogs() { return leftHandLogs; }
    public List<TransformLog> GetRightHandTransformLogs() { return rightHandLogs; }

    public Dictionary<int, ArmCollisionLog> GetArmCollisionLogs() { return armCollisionLogs; }

    public Dictionary<int, FightCollisionLog> GetFightCollisionLogs() { return fightCollisionLogs; }

    public Dictionary<int, HRLog> GetHRLogs() { return HRLogs; }

    public bool AreLogsReady()
    {
        return logsReady;
    }
}
