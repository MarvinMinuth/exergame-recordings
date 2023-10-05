using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.PlayerLoop.PreLateUpdate;

public enum Savefile
{
    Tutorial,
    TaskOne,
    TaskTwo,
    TaskThree,
}

public class ReplayManager : MonoBehaviour
{   
    public GameObject loadManager1, loadManager2;
    public LogDataManager tutorialLogDataManager, taskOneLogDataManager, taskTwoLogDataManager, taskThreeLogDataManager;
    public GameObject fighter;
    public GameObject fightDummy;
    public GameObject menu;

    //private MenuCoordinator menuCoordinator;

    private LogDataManager activeLogDataManager, logDataManager1, logDataManager2;
    private FighterCoordinator fighterCoordinator;
    private ArmsCoordinator armsCoordinator;

    private GameObject head, leftHand, rightHand;
    private GameObject bottomArmBase, middleArmBase, topArmBase;

    private int frame = 6;
    private int totalFrames = 0;

    private List<TransformLog> headTransformLogs;
    private List<TransformLog> leftHandTransformLogs;
    private List<TransformLog> rightHandTransformLogs;

    private List<ArmLog> bottomArmLogs;
    private List<ArmLog> middleArmLogs;
    private List<ArmLog> topArmLogs;

    private List<int> bottomArmHighlights;
    private List<int> middleArmHighlights;
    private List<int> topArmHighlights;
    private Dictionary<int, ArmCollisionLog[]> armCollisionLogDic;
    private Dictionary<int, ArmCollisionLog[]> succsessfulArmCollisionLogDic;
    private Dictionary<int, ArmCollisionLog[]> unsuccsessfulArmCollisionLogDic;
    private Dictionary<int, FightCollisionLog[]> fightCollisionLogDic;
    private Dictionary<int, FightCollisionLog[]> succsessfulFightCollisionLogDic;
    private Dictionary<int, FightCollisionLog[]> unsuccsessfulFightCollisionLogDic;


    private Dictionary<int, HRLog> hrLogDic;

    private bool isLoading = false;
    private bool fileLoaded = false;

    void Start()
    {
        logDataManager1 = loadManager1.GetComponent<LogDataManager>();
        logDataManager2 = loadManager2.GetComponent<LogDataManager>();
        armsCoordinator = fightDummy.GetComponent<ArmsCoordinator>();

        bottomArmBase = armsCoordinator.bottomArmBase;
        middleArmBase = armsCoordinator.middleArmBase;
        topArmBase = armsCoordinator.topArmBase; 

        //menuCoordinator = menu.GetComponent<MenuCoordinator>();
    }
    private void StartLoad(Savefile saveFile)
    {
        if (isLoading) return;
        isLoading = true;
        if (fileLoaded)
        {
            Unload();
        }

        if (saveFile == Savefile.TaskOne)
        {
            activeLogDataManager = loadManager1.GetComponent<LogDataManager>();
        }
        else
        {
            activeLogDataManager = loadManager2.GetComponent<LogDataManager>();
        }

        frame = 0;

        fighter.SetActive(true);
        fighterCoordinator = fighter.GetComponent<FighterCoordinator>();
        head = fighterCoordinator.GetHead();
        leftHand = fighterCoordinator.GetLeftHand();
        rightHand = fighterCoordinator.GetRightHand();
    }

    public void Load(Savefile saveFile, Material material)
    {
        StartLoad(saveFile);

        fighterCoordinator.ChangeMaterial(material);

        if (!activeLogDataManager.AreLogsReady() && !activeLogDataManager.IsLoading())
        {
            activeLogDataManager.LoadReplay();
        }

        StartCoroutine(WaitForLogs());       
    }

    public void Load(Savefile saveFile)
    {
        StartLoad(saveFile);

        if (!activeLogDataManager.AreLogsReady() && !activeLogDataManager.IsLoading())
        {
            activeLogDataManager.LoadReplay();
        }

        StartCoroutine(WaitForLogs());
    }

    void OnLogsLoaded()
    {
        isLoading = false;

        totalFrames = headTransformLogs.Count - 1;
        //SetupMenu();

        fileLoaded = true;;
    }

    IEnumerator WaitForLogs()
    {
        while (!activeLogDataManager.AreLogsReady())
        {
            yield return null;
        }

        headTransformLogs = activeLogDataManager.GetHeadTransformLogs();
        leftHandTransformLogs = activeLogDataManager.GetLeftHandTransformLogs();
        rightHandTransformLogs = activeLogDataManager.GetRightHandTransformLogs();

        bottomArmLogs = activeLogDataManager.GetBottomArmLogs();
        middleArmLogs = activeLogDataManager.GetMiddleArmLogs();
        topArmLogs = activeLogDataManager.GetTopArmLogs();

        bottomArmHighlights = activeLogDataManager.GetBottomArmHighlights();
        middleArmHighlights = activeLogDataManager.GetMiddleArmHighlights();
        topArmHighlights = activeLogDataManager.GetTopArmHighlights();

        armCollisionLogDic = activeLogDataManager.GetArmCollisionLogs();
        succsessfulArmCollisionLogDic = activeLogDataManager.GetSuccsessfulArmCollisionLogs();
        unsuccsessfulArmCollisionLogDic = activeLogDataManager.GetUnsuccsessfulArmCollisionLogs();
        fightCollisionLogDic = activeLogDataManager.GetFightCollisionLogs();
        succsessfulFightCollisionLogDic = activeLogDataManager.GetSuccsessfulFightCollisionLogs();
        unsuccsessfulFightCollisionLogDic = activeLogDataManager.GetUnsuccsessfulFightCollisionLogs();
        hrLogDic = activeLogDataManager.GetHRLogs();

        OnLogsLoaded();
    }

    public void Stop()
    {
        frame = 6;
        LoadFrame(frame);
        DestroyTrajectories();
    }

    void Update()
    {
    }

    public int GetReplayLength()
    {
        return totalFrames;
    }

    public int GetReplayTime()
    {
        return frame;
    }

    public void LoadFrame(int frame)
    {
        if(isLoading)
        {
            return;
        }

        if(frame < 0 || frame > headTransformLogs.Count)
        {
            return;
        }
        this.frame = frame;

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
        //menuCoordinator.SetupMenu();
    }
    

    public void Unload()
    {
        //menuCoordinator.DestroyMenu();
        fighterCoordinator.ChangeToIdleMaterial();
        fighter.SetActive(false);
        Stop();
        armsCoordinator.ResetArms();
        fileLoaded = false;
        //ClearLogs();    
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

    public void CreateTrajectories(int minFrame, int maxFrame)
    {
        Trajectories trajectories = head.GetComponent<Trajectories>();
        trajectories.SetFrames(minFrame, maxFrame);
        trajectories.SetLogs(headTransformLogs);
        trajectories.CreateTrajectories();

        trajectories = leftHand.GetComponent<Trajectories>();
        trajectories.SetFrames(minFrame, maxFrame);
        trajectories.SetLogs(leftHandTransformLogs);
        trajectories.CreateTrajectories();

        trajectories = rightHand.GetComponent<Trajectories>();
        trajectories.SetFrames(minFrame, maxFrame);
        trajectories.SetLogs(rightHandTransformLogs);
        trajectories.CreateTrajectories();
    }

    public void DestroyTrajectories()
    {
        Trajectories trajectories = head.GetComponent<Trajectories>();
        trajectories.DestroyTrajectories();

        trajectories = leftHand.GetComponent<Trajectories>(); 
        trajectories.DestroyTrajectories();

        trajectories = rightHand.GetComponent<Trajectories>();
        trajectories.DestroyTrajectories();
    }

    public List<int> GetBottomArmHighlights(){ return bottomArmHighlights; }
    public List<int> GetTopArmHighlights() { return topArmHighlights; }
    public List<int> GetMiddleArmHighlights() { return middleArmHighlights; }

    public Dictionary<int, ArmCollisionLog[]> GetArmCollisionDic() { return armCollisionLogDic; }

    public Dictionary<int, ArmCollisionLog[]> GetSuccsessfulArmCollisionDic() { return succsessfulArmCollisionLogDic; }
    public Dictionary<int, ArmCollisionLog[]> GetUnsuccsessfulArmCollisionDic() { return unsuccsessfulArmCollisionLogDic; }

    public Dictionary<int, FightCollisionLog[]> GetFightCollisionDic() { return fightCollisionLogDic; }
    public Dictionary<int, FightCollisionLog[]> GetSuccsessfulFightCollisionDic() { return succsessfulFightCollisionLogDic; }
    public Dictionary<int, FightCollisionLog[]> GetUnsuccsessfulFightCollisionDic() { return unsuccsessfulFightCollisionLogDic; }

    public Dictionary<int, HRLog> GetHRLog() { return hrLogDic; }

    public bool IsLoading() { return isLoading; }

    public bool FileIsLoaded()
    {
        return fileLoaded;
    }
}
