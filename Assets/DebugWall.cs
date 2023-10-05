using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class DebugWall : MonoBehaviour
{
    private TMP_Text textComponent;
    bool isInitialized = false;
    public string saveFile;
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

    Dictionary<int, ArmCollisionLog[]> armCollisionLogs = new Dictionary<int, ArmCollisionLog[]>();
    Dictionary<int, ArmCollisionLog[]> successfulArmCollisionDic = new Dictionary<int, ArmCollisionLog[]>();
    Dictionary<int, ArmCollisionLog[]> unsuccessfulArmCollisionDic = new Dictionary<int, ArmCollisionLog[]>();

    Dictionary<int, FightCollisionLog[]> fightCollisionLogs = new Dictionary<int, FightCollisionLog[]>();
    Dictionary<int, FightCollisionLog[]> successfulFightCollisionDic = new Dictionary<int, FightCollisionLog[]>();
    Dictionary<int, FightCollisionLog[]> unsuccessfulFightCollisionDic = new Dictionary<int, FightCollisionLog[]>();

    private bool logsReady;
    private bool loading;

    void Start()
    {
        Initialize();
        EnsureFolderExistsInPersistentDataPath("Logs");
        SetText(Application.persistentDataPath + "/Logs/");
        AddText(GetFileNamesInPath(Application.persistentDataPath));
        AddText(GetFileNamesInPath(Application.persistentDataPath + "/Logs/"));

        saveFile = Application.persistentDataPath + "/Logs/" + saveFile;
        AddText(saveFile);
        LoadReplay();
        //AddText(PrintJsonContent(saveFile));
        AddText(bottomArmLogs[1].Position.ToString());
    }

    private void Initialize()
    {
        textComponent = transform.Find("Canvas").Find("Panel").Find("Text (TMP)").GetComponent<TMP_Text>();
        isInitialized = true;
    }
    public void SetText(string text)
    {
        if (!isInitialized) Initialize();
        textComponent.text = text;
    }

    public void DeleteText()
    {
        if (!isInitialized) Initialize();
        textComponent.text = string.Empty;
    }

    public void AddText(string text)
    {
        if (!isInitialized) Initialize();
        textComponent.text += "\n" + text;
    }

    public static string GetFileNamesInPath(string path)
    {
        if (!Directory.Exists(path))
        {
            return "Der angegebene Pfad existiert nicht.";
        }

        string[] files = Directory.GetFiles(path);
        string result = "";

        foreach (string file in files)
        {
            result += Path.GetFileName(file) + "\n";
        }

        return result.TrimEnd('\n');  // Entfernt den letzten Zeilenumbruch
    }

    void EnsureFolderExistsInPersistentDataPath(string folder)
    {
        string destinationFolderPath = Path.Combine(Application.persistentDataPath, folder);

        // Überprüfen, ob der Ordner im persistentDataPath existiert.
        if (!Directory.Exists(destinationFolderPath))
        {
            // Erstellen des Ordners im persistentDataPath.
            Directory.CreateDirectory(destinationFolderPath);

            // Laden Sie alle Dateien aus dem Ordner im Resources-Verzeichnis.
            TextAsset[] assets = Resources.LoadAll<TextAsset>(folder);

            // Jede Datei im Resources-Ordner in den persistentDataPath kopieren.
            foreach (TextAsset asset in assets)
            {
                string destinationFilePath = Path.Combine(destinationFolderPath, asset.name + ".json"); // Fügt die ".json"-Erweiterung hinzu.
                File.WriteAllText(destinationFilePath, asset.text);
            }
        }
    }

    public void LoadReplay()
    {
        loading = true;
        logsReady = false;
        ES3.CacheFile(saveFile);

        var recording = new ES3Settings(saveFile, ES3.Location.Cache);

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
                ArmCollisionLog armCollisionLog = ES3.Load<ArmCollisionLog>(key, recording);
                if (!armCollisionLogs.ContainsKey(frame))
                {
                    armCollisionLogs.Add(frame, new ArmCollisionLog[3]);
                }

                string armBase = armCollisionLog.armBase;
                int position = 0;
                if (armBase.Equals("ArmBase 1")) position = 0;
                else if (armBase.Equals("ArmBase 2")) position = 1;
                else position = 2;

                armCollisionLogs[frame][position] = armCollisionLog;
                if (armCollisionLog.ExpectedCollisionType == armCollisionLog.ReceiveCollisionType)
                {
                    if (!successfulArmCollisionDic.ContainsKey(frame))
                    {
                        successfulArmCollisionDic.Add(frame, new ArmCollisionLog[3]);
                    }
                    successfulArmCollisionDic[frame][position] = armCollisionLog;
                }
                else
                {
                    if (!unsuccessfulArmCollisionDic.ContainsKey(frame))
                    {
                        unsuccessfulArmCollisionDic.Add(frame, new ArmCollisionLog[3]);
                    }
                    unsuccessfulArmCollisionDic[frame][position] = armCollisionLog;
                }
            }
            else if (key.Contains("FightCollision"))
            {
                FightCollisionLog fightCollisionLog = ES3.Load<FightCollisionLog>(key, recording);
                if (!fightCollisionLogs.ContainsKey(frame))
                {
                    fightCollisionLogs.Add(frame, new FightCollisionLog[3]);
                }

                string armBase = fightCollisionLog.armBase;
                int position = 0;
                if (armBase.Equals("ArmBase 1")) position = 0;
                else if (armBase.Equals("ArmBase 2")) position = 1;
                else position = 2;

                fightCollisionLogs[frame][position] = fightCollisionLog;
                if (fightCollisionLog.ExpectedCollisionType == fightCollisionLog.ReceiveCollisionType)
                {
                    if (!successfulFightCollisionDic.ContainsKey(frame))
                    {
                        successfulFightCollisionDic.Add(frame, new FightCollisionLog[3]);
                    }
                    successfulFightCollisionDic[frame][position] = fightCollisionLog;
                }
                else
                {
                    if (!unsuccessfulFightCollisionDic.ContainsKey(frame))
                    {
                        unsuccessfulFightCollisionDic.Add(frame, new FightCollisionLog[3]);
                    }
                    unsuccessfulFightCollisionDic[frame][position] = fightCollisionLog;
                }
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

        attachedToBottom = "";
        attachedToTop = "";
        attachedToMiddle = "";

        logsReady = true;
        loading = false;
    }

    public string PrintJsonContent(string path)
    {
        if (File.Exists(path))
        {
            string jsonContent = File.ReadAllText(path);
            return jsonContent;
        }
        else
        {
            return "Datei nicht gefunden: " + path;
        }
    }
}
