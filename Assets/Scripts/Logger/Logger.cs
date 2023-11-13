using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


// Transforms: Player, Teleport Recticles, Ghost-Objects
// ReplayManager: Datei, aktueller Frame
// Trajectories: ausgewählte Markierung etc.
// ReplayController: Wiedergabestatus (welche Optionen ausgewählt)
// HeartrateMenu / -Coordinator: ausgewählte Optionen, HR
// Fighter mini: ein/aus, welche Körperteile angezeigt

public class Logger : MonoBehaviour
{
    public static Logger Instance { get; private set; }
    public enum LogType { UncategorizedLog, TransformLog, LoadFileLog, ReplayControllerLog, FighterLog, HROptionsLog }
    public enum TransformType { Undefined, HMD, LeftController, RightController }

    private static int frame = 0;
    private static int logID = 0;
    private const string IDKEY = "logID";
    private ES3Settings saveSettings;
    public const string SAVEFILE = "StudyLog";


    public void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("More than one instance of Logger found.");
        }

        saveSettings = new ES3Settings(ES3.Location.Cache);
        saveSettings.path = Application.persistentDataPath + "/Logs/" + Logger.SAVEFILE + "_" + DateTime.Now.ToString("yyyy-MM-dd-THH-mm-ss") + ".json";

        ES3.CacheFile(saveSettings.path);

        try
        {
            Logger.logID = (int)ES3.Load(IDKEY, saveSettings.path);
        }
        catch (Exception exception)
        {
            if(exception is KeyNotFoundException ||  exception is FileNotFoundException)
            {
                logID = 0;
                ES3.Save(IDKEY, logID, saveSettings.path);
            }
            else
            {
                throw exception;
            }
        }
    }

    private void FixedUpdate()
    {
        frame++;
    }

    private void OnDestroy()
    {
        ES3.Save(IDKEY, logID, saveSettings);

        ES3.StoreCachedFile(saveSettings);
    }

    public void Log(BaseStudyLog log)
    {
        log.frame = frame;
        log.Date_Time = DateTime.Now.ToString("") + ":" + DateTime.Now.Millisecond;
        log.id = logID;
        ES3.Save("Log_" + logID + "_" + log.GetType(), log, saveSettings);
        logID++;
    }

    public void LogTransform(TransformStudyLog log)
    {
        log.frame = frame;
        log.Date_Time = DateTime.Now.ToString("") + ":" + DateTime.Now.Millisecond;
        log.id = logID;
        ES3.Save("Log_" + logID + "_" + log.GetType(), log, saveSettings);
        logID++;
    }

    public void LogLoadFile(LoadFileStudyLog log)
    {
        log.frame = frame;
        log.Date_Time = DateTime.Now.ToString("") + ":" + DateTime.Now.Millisecond;
        log.id = logID;
        ES3.Save("Log_" + logID + "_" + log.GetType(), log, saveSettings);
        logID++;
    }

    public void LogReplayControllerSettings(ReplayControllerStudyLog log)
    {
        log.frame = frame;
        log.Date_Time = DateTime.Now.ToString("") + ":" + DateTime.Now.Millisecond;
        log.id = logID;
        ES3.Save("Log_" + logID + "_" + log.GetType(), log, saveSettings);
        logID++;
    }

    public int GetFrame()
    {
        return frame;
    }
}

public class BaseStudyLog
{
    public int id;
    public int frame;
    public Logger.LogType logType;
    public string Date_Time;

    public BaseStudyLog()
    {
        logType = Logger.LogType.UncategorizedLog;
        Date_Time = DateTime.Now.ToString("") + ":" + DateTime.Now.Millisecond;
        frame = Logger.Instance.GetFrame();
    }
}

public class TransformStudyLog : BaseStudyLog
{
    public Logger.TransformType transformType;
    public Vector3 position, rotation, scale;

    public TransformStudyLog()
    {
        logType = Logger.LogType.TransformLog;
        transformType = Logger.TransformType.Undefined;
    }

    public void setValuesByTransform(Transform transform)
    {
        this.position = transform.position;
        this.rotation = transform.eulerAngles;
        this.scale = transform.localScale;
    }

    public void setTransformType(Logger.TransformType transformType)
    {
        this.transformType = transformType;
    }
}

public class LoadFileStudyLog : BaseStudyLog
{
    public Savefile savefile;
    public LoadFileStudyLog()
    {
        logType = Logger.LogType.LoadFileLog;
    }
}

public class ReplayControllerStudyLog : BaseStudyLog
{
    public bool isRunning;
    public bool isLooping;
    public float playSpeed;
    public Direction playDirection;
    public int loadedFrame;

    public ReplayControllerStudyLog()
    {
        logType = Logger.LogType.ReplayControllerLog;
    }
}

public class FighterStudyLog : BaseStudyLog
{
    public bool headShown, leftHandShown, rightHandShown, bodyShown;

    public FighterStudyLog()
    {
        logType = Logger.LogType.FighterLog;
    }
}

public class HeartrateOptionsStudyLog : BaseStudyLog
{
    public bool audioFeedbackActivated, hapticFeedbackActivated, visualFeedbackActivated;
    public Position position;

    public HeartrateOptionsStudyLog()
    {
        logType = Logger.LogType.HROptionsLog;
    }
}


