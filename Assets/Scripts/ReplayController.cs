using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.PlayerLoop.PreLateUpdate;

public enum Speed
{
    Half,
    Normal,
    Double
}
public enum Direction
{
    Forwards,
    Backwards
}
public class ReplayController : MonoBehaviour
{
    public bool loadReplayOnStart = false;

    public ReplayManager manager;
    public MenuCoordinator menuCoordinator;
    public HeartrateCoordinator heartrateCoordinator;
    
    bool isRunning;
    bool isLooping;
    bool replayIsReady;
    bool isReceivingInput;

    float playSpeed = 1;
    Direction playDirection = Direction.Forwards;

    int frame;
    int totalFrames;
    int minPlayFrame = 0;
    int maxPlayFrame;
    float nextUpdate = 0f;

    // Start is called before the first frame update
    void Start()
    {
        if (manager == null)
        {
            manager = transform.GetComponent<ReplayManager>();
        }

        if (menuCoordinator == null)
        {
            menuCoordinator = GameObject.FindGameObjectWithTag("Menu").GetComponent<MenuCoordinator>();
        }
        
        if (loadReplayOnStart) { Load(Savefile.TaskTwo, null); }
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

    public void ChangeSpeed(Speed speed)
    {
        switch (speed)
        {
            case Speed.Normal:  playSpeed = 1f; break;
            case Speed.Half: playSpeed = 0.5f; break;
            case Speed.Double: playSpeed = 2f; break;
        }
    }
    public void Play()
    {
        isRunning = true;
    }
    public void Pause()
    {
        isRunning = false;
    }
    public void Stop()
    {
        Pause();
        minPlayFrame = 0;
        maxPlayFrame = totalFrames;
        SetFrame(0);
        playDirection = Direction.Forwards;
        playSpeed = 1f;
        manager.Stop();
        menuCoordinator.Stop();
    }
    public void Load(Savefile saveFile, Material material)
    {
        isRunning = false;
        minPlayFrame = 0;
        frame = 0;
        if (manager.FileIsLoaded()) { Unload(); }
        if (material == null) { manager.Load(saveFile); }
        else { manager.Load(saveFile, material); }

        totalFrames = manager.GetReplayLength();
        maxPlayFrame = totalFrames;

        menuCoordinator.SetupMenu();
        heartrateCoordinator.Setup();
        Play();     
    }
    public void SetPlaySpeed(float speed)
    {
        playSpeed = speed;
    }
    public void SetPlayDirection(Direction direction)
    {
        playDirection = direction;
    }
    public float GetPlaySpeed() { return playSpeed; }
    public Direction GetPlayDirection() { return playDirection; }

    public void SetFrame(int newFrame)
    {
        frame = AdjustToWindow(newFrame);
        if (newFrame < 0) { frame = 0; }
        LoadFrame(frame);
    }

    public int GetFrame()
    {
        return frame;
    }

    private int AdjustToWindow(int frame)
    {
        if (frame < minPlayFrame) frame = minPlayFrame;
        if (frame > maxPlayFrame) frame = maxPlayFrame;

        return frame;
    }

    public void LoadFrame(int frame)
    {
        frame = AdjustToWindow(frame);
        manager.LoadFrame(frame);
    }

    public void LoadNextFrame()
    {
        frame = AdjustToWindow(frame);
        LoadFrame(frame);
        if (playDirection == Direction.Forwards) 
        {
            frame += 1;
            if (frame > totalFrames)
            {
                if (isLooping)
                {
                    frame = minPlayFrame;
                }
                else
                {
                    Stop();
                }
            }
            else if (frame > maxPlayFrame)
            {
                if (isLooping)
                {
                    frame = minPlayFrame;
                }
                else
                {
                    Pause();
                }
            }
        }
        else
        {
            frame -= 1;
            if (frame < 0)
            {
                if (isLooping)
                {
                    frame = maxPlayFrame;
                }
                else
                {
                    Stop();
                }
            }
            else if (frame < minPlayFrame)
            {
                if (isLooping)
                {
                    frame = maxPlayFrame;
                }
                else { Pause(); }
            }
        }
        
        
        
    }

    public void Unload()
    {
        Stop();
        minPlayFrame = 0;
        totalFrames = 0;
        maxPlayFrame = 0;
        heartrateCoordinator.ResetCoordinator();
        menuCoordinator.DestroyMenu();
        manager.Unload();  
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    public int GetReplayLength()
    {
        return manager.GetReplayLength();
    }

    public bool IsReplayReady()
    {
        return (!manager.IsLoading() && manager.FileIsLoaded());
    }

    public void ChangeLooping()
    {
        if(isLooping) isLooping = false;
        else isLooping = true;
    }
    public void ChangeDirection()
    {
        if (playDirection == Direction.Forwards) playDirection = Direction.Backwards;
        else playDirection = Direction.Forwards;
    }

    public bool IsLooping()
    {
        return isLooping;
    }

    public bool IsControllable()
    {
        return (IsReplayReady() && !isReceivingInput);
    }

    public void SetReceivingInput(bool isReceivingInput)
    {
        this.isReceivingInput = isReceivingInput;
    }

    public void ChangeReplayWindow(int minFrame, int maxFrame)
    {
        minPlayFrame = Mathf.Min(minFrame, maxFrame);
        maxPlayFrame = Mathf.Max(minFrame, maxFrame);
    }

    public void ResetReplayWindow()
    {
        minPlayFrame = 0;
        maxPlayFrame = totalFrames;
    }

    public bool ManagerIsLoading()
    {
        return manager.IsLoading();
    }
}
