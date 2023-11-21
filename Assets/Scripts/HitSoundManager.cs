using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource punchAudioSource, swordAudioSource;
    

    private ReplayManager replayManager;
    private Dictionary<int, FightCollisionLog[]> fightCollisionDic;

    private void Start()
    {
        replayManager = ReplayManager.Instance;
        replayManager.OnReplayLoaded += ReplayManager_OnReplayLoaded;
        replayManager.OnReplayUnloaded += ReplayManager_OnReplayUnloaded;
        replayManager.OnFrameLoaded += ReplayManager_OnFrameLoaded;
    }

    private void ReplayManager_OnReplayUnloaded(object sender, System.EventArgs e)
    {
        fightCollisionDic.Clear();
    }

    private void ReplayManager_OnReplayLoaded(object sender, ReplayManager.OnReplayLoadedEventArgs e)
    {
        fightCollisionDic = replayManager.GetFightCollisionDic();
    }

    private void ReplayManager_OnFrameLoaded(object sender, ReplayManager.OnFrameLoadedEventArgs e)
    {
        int frame = replayManager.GetFrame();

        if (!ReplayController.Instance.IsRunning()) return;

        if (fightCollisionDic.ContainsKey(frame))
        {
            foreach (FightCollisionLog fightCollisionLog in fightCollisionDic[frame]) 
            {
                if (fightCollisionLog != null) {
                    if(fightCollisionLog.ExpectedCollisionType == 2) swordAudioSource.Play();
                    else punchAudioSource.Play();
                }
            }
        }
    }
}
