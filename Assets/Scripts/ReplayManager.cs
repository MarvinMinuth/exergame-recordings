using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.PlayerLoop.PreLateUpdate;

public class ReplayManager : MonoBehaviour
{
    [SerializeField]
    public GameObject loadManager;
    public GameObject fighter;
    public GameObject fightDummy;
    public GameObject menu;

    GameObject timeSlider;
    private MenuCoordinator menuCoordinator;

    private LogDataManager logDataManager;
    private FighterCoordinator fighterCoordinator;
    private ArmsCoordinator armsCoordinator;

    private GameObject head, leftHand, rightHand;
    private GameObject bottomArmBase, middleArmBase, topArmBase;

    private int frame = 6;
    private int totalFrames = 0;
    private float nextUpdate = 0f;

    private List<TransformLog> headTransformLogs;
    private List<TransformLog> leftHandTransformLogs;
    private List<TransformLog> rightHandTransformLogs;

    private List<ArmLog> bottomArmLogs;
    private List<ArmLog> middleArmLogs;
    private List<ArmLog> topArmLogs;

    private List<int> bottomArmHighlights;
    private List<int> middleArmHighlights;
    private List<int> topArmHighlights;
    private Dictionary<int, ArmCollisionLog> armCollisionLogDic;
    private Dictionary<int, FightCollisionLog> fightCollisionLogDic;
    private Dictionary<int, HRLog> hrLogDic;

    private bool areLogsReady = false;
    private bool isRunning = false;
    private bool isLoading = false;
    private bool fileLoaded = false;

    private int playSpeed = 1;
    private int playDirection = 1;

    // Start is called before the first frame update
    void Start()
    {
        logDataManager = loadManager.GetComponent<LogDataManager>();
        armsCoordinator = fightDummy.GetComponent<ArmsCoordinator>();

        bottomArmBase = armsCoordinator.bottomArmBase;
        middleArmBase = armsCoordinator.middleArmBase;
        topArmBase = armsCoordinator.topArmBase; 

        menuCoordinator = menu.GetComponent<MenuCoordinator>();
    }

    public void Load(int saveFile, Material material)
    {
        if (isLoading) return;
        if (fileLoaded)
        {
            Unload();
        }

        Debug.Log(fighter.name);
        fighter.SetActive(true);
        fighterCoordinator = fighter.GetComponent<FighterCoordinator>();
        head = fighterCoordinator.GetHead();
        leftHand = fighterCoordinator.GetLeftHand();
        rightHand = fighterCoordinator.GetRightHand();

        isLoading = true;
        fighterCoordinator.ChangeMaterial(material);
        logDataManager.LoadReplay(saveFile);

        StartCoroutine(WaitForLogs());       
    }

    void OnLogsLoaded()
    {
        Debug.Log("OnLogsLoaded");
        isLoading = false;

        totalFrames = headTransformLogs.Count - 1;
        SetupMenu();

        fileLoaded = true;
        areLogsReady = true;
        Play();
    }

    IEnumerator WaitForLogs()
    {
        while (!logDataManager.AreLogsReady())
        {
            yield return null;
        }
        Debug.Log("Logs ready");

        headTransformLogs = logDataManager.GetHeadTransformLogs();
        leftHandTransformLogs = logDataManager.GetLeftHandTransformLogs();
        rightHandTransformLogs = logDataManager.GetRightHandTransformLogs();

        bottomArmLogs = logDataManager.GetBottomArmLogs();
        middleArmLogs = logDataManager.GetMiddleArmLogs();
        topArmLogs = logDataManager.GetTopArmLogs();

        bottomArmHighlights = logDataManager.GetBottomArmHighlights();
        middleArmHighlights = logDataManager.GetMiddleArmHighlights();
        topArmHighlights = logDataManager.GetTopArmHighlights();

        armCollisionLogDic = logDataManager.GetArmCollisionLogs();
        fightCollisionLogDic = logDataManager.GetFightCollisionLogs();
        hrLogDic = logDataManager.GetHRLogs();

        OnLogsLoaded();
    }

    public void Play()
    {
        isRunning = true;
    }

    public void Pause()
    {
        isRunning = false;
    }

    public void SetPlaySpeed(int speed)
    {
        playSpeed = speed;
    }
    public void SetPlayDirection(int direction)
    {
        playDirection = direction;
    }

    public int GetPlaySpeed() { return playSpeed; }
    public int GetPlayDirection() {  return playDirection; }

    public void Stop()
    {
        isRunning = false;
        frame = 6;
        LoadFrame(frame);
        playDirection = 1;
        playSpeed = 1;
    }

   

    // Update is called once per frame
    void Update()
    {
        if (!isRunning)
        {
            return;
        }
        // Überprüfe, ob 1/60 Sekunde vergangen ist
        if (Time.time >= nextUpdate)
        {
            LoadNextFrame();
            // Berechne den Zeitpunkt des nächsten Updates
            nextUpdate = Time.time + (1f / (60f * playSpeed));
        }
    }

    public int GetReplayLength()
    {
        return totalFrames;
    }

    public void SetReplayTime(int newFrame)
    {
        LoadFrame(newFrame);
        frame = newFrame;
    }

    public int GetReplayTime()
    {
        return frame;
    }

    public void LoadFrame(int frame)
    {
        if(!areLogsReady || !fileLoaded)
        {
            return;
        }

        if(frame < 0 || frame > headTransformLogs.Count)
        {
            return;
        }

        SetBodyPosition(head, headTransformLogs[frame]);
        SetBodyPosition(leftHand, leftHandTransformLogs[frame]);
        SetBodyPosition(rightHand, rightHandTransformLogs[frame]);

        SetBodyRotation(head, headTransformLogs[frame]);
        SetBodyRotation(leftHand, leftHandTransformLogs[frame]);
        SetBodyRotation(rightHand, rightHandTransformLogs[frame]);

        SetDummyAttachement(bottomArmBase, bottomArmLogs[frame]);
        SetDummyAttachement(middleArmBase, middleArmLogs[frame]);
        SetDummyAttachement(topArmBase, topArmLogs[frame]);

        SetDummyPosition(bottomArmBase, bottomArmLogs[frame]);
        SetDummyPosition(middleArmBase, middleArmLogs[frame]);
        SetDummyPosition(topArmBase, topArmLogs[frame]);

        SetDummyRotation(bottomArmBase, bottomArmLogs[frame]);
        SetDummyRotation(middleArmBase, middleArmLogs[frame]);
        SetDummyRotation(topArmBase, topArmLogs[frame]);

        SetDummyTargetPosition(bottomArmBase, bottomArmLogs[frame]);
        SetDummyTargetPosition(middleArmBase, middleArmLogs[frame]);
        SetDummyTargetPosition(topArmBase, topArmLogs[frame]);

        SetDummyTargetRotation(bottomArmBase, bottomArmLogs[frame]);
        SetDummyTargetRotation(middleArmBase, middleArmLogs[frame]);
        SetDummyTargetRotation(topArmBase.gameObject, topArmLogs[frame]);
    }

    public void SetupMenu()
    {
        menuCoordinator.SetupMenu();
        timeSlider = menuCoordinator.GetTimeline();
    }

    public void LoadNextFrame()
    {
        LoadFrame(frame);
        frame += playDirection;

        if(frame > totalFrames )
        {
            Stop();
        }
    }

    public void Unload()
    {     
        fighterCoordinator.ChangeToIdleMaterial();
        fighter.SetActive(false);
        Stop();
        armsCoordinator.ResetArms();
        fileLoaded = false;
        menuCoordinator.DestroyMenu();

        ClearLogs();    
    }

    void ClearLogs()
    {
        headTransformLogs.Clear();
        leftHandTransformLogs.Clear();
        rightHandTransformLogs.Clear();
        bottomArmLogs.Clear();
        bottomArmHighlights.Clear();
        middleArmLogs.Clear();
        middleArmHighlights.Clear();
        topArmLogs.Clear();
        topArmHighlights.Clear();
        armCollisionLogDic = null;
        fightCollisionLogDic = null;
        hrLogDic = null;
    }

    // Gib den HRLog zurück, der dem gegebenen Frame am nächsten kommt
    public int GetCurrentHeartRate()
    {
        if (!fileLoaded || isLoading)
        {
            return 0;
        }
        int nearestFrame = -1;
        foreach (int key in hrLogDic.Keys)
        {
            // Wenn der Schlüssel kleiner oder gleich dem gegebenen Frame ist
            if (key <= frame)
            {
                // Aktualisiere den Wert von nearestFrame, wenn der Schlüssel größer ist
                if (key > nearestFrame)
                {
                    nearestFrame = key;
                }
            }
        }

        // Wenn nearestFrame aktualisiert wurde, gib den entsprechenden HRLog zurück
        if (nearestFrame != -1)
        {
            return hrLogDic[nearestFrame].heartRate;
        }

        // Wenn kein passender HRLog gefunden wurde, gib null zurück
        return 0;
    }

    void SetDummyPosition(GameObject armBase, ArmLog armLog)
    {
        armsCoordinator.SetPosition(armBase, armLog.Position);
    }

    void SetDummyRotation(GameObject armBase, ArmLog armLog)
    {
        armsCoordinator.SetRotation(armBase, Quaternion.Euler(armLog.Rotation));
    }

    void SetDummyTargetPosition(GameObject armBase, ArmLog armLog)
    {
        armsCoordinator.SetTargetPosition(armBase, armLog.targetPosition);
    }

    void SetDummyTargetRotation(GameObject armBase, ArmLog armLog)
    {
        armsCoordinator.SetTargetRotation(armBase, Quaternion.Euler(armLog.targetRotation));
    }

    void SetDummyAttachement(GameObject armBase, ArmLog armLog)
    {
        armsCoordinator.AttachToArmBase(armBase, armLog.armType);
    }

    void SetBodyPosition(GameObject gameObject,  TransformLog transformLog)
    {
        fighterCoordinator.SetPosition(gameObject, transformLog.Position);
    }

    void SetBodyRotation(GameObject gameObject, TransformLog transformLog)
    {
        fighterCoordinator.SetRotation(gameObject, Quaternion.Euler(transformLog.Rotation));
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    public List<int> GetBottomArmHighlights(){ return bottomArmHighlights; }
    public List<int> GetTopArmHighlights() { return topArmHighlights; }
    public List<int> GetMiddleArmHighlights() { return middleArmHighlights; }

    public Dictionary<int, ArmCollisionLog> GetArmCollisionDic() { return armCollisionLogDic; }

    public Dictionary<int, FightCollisionLog> GetFightCollisionDic() { return fightCollisionLogDic; }

    public Dictionary<int, HRLog> GetHRLog() { return hrLogDic; }

    public bool IsLoading() { return isLoading; }
}
