using UnityEngine;

public class HeartAnchor : MonoBehaviour, IHeartrateObserver
{
    [SerializeField] private HeartMenu menu;
    [SerializeField] private Transform heartSymbol;
    [SerializeField] private HeartSymbolVisual heartSymbolVisual;


    private VisualFeedback visualFeedback;
    private AudioFeedback audioFeedback;

    private void Start()
    {
        //heartSymbolVisual.Hide();
        //DeactivateMenu();
    }

    public void Initialize(AudioFeedback audio, VisualFeedback visual)
    {
        audioFeedback = audio;
        visualFeedback = visual;
    }

    public void ActivateSymbol()
    {
        heartSymbolVisual.Show();
        visualFeedback.Initialize(heartSymbol.gameObject);
        audioFeedback.Initialize(heartSymbol.gameObject);
    }
    public void DeactivateSymbol()
    {
        menu.Deactivate();
        visualFeedback.End();
        audioFeedback.End();
        heartSymbolVisual.Hide();
    }
    public void ActivateMenu()
    {
        menu.Activate();
    }

    public void DeactivateMenu()
    {
        menu.Deactivate();
    }

    public void SwitchMenu()
    {
        if (menu.isActivated) DeactivateMenu();
        else ActivateMenu();
    }

    public void UpdateHeartbeat(IHeartbeatState state, bool useAudioFeedback, bool useHapticFeedback, bool useVisualFeedback)
    {
        if (state is LubState)
        {
            if (useVisualFeedback) visualFeedback.ScaleUp();
            if (useAudioFeedback) audioFeedback.PlayLub();
        }
        else if (state is DubState)
        {
            if (useVisualFeedback) visualFeedback.ScaleUp();
            if (useAudioFeedback) audioFeedback.PlayDub();
        }
        else if (state is ShortPauseState || state is LongPauseState)
        {
            if (useVisualFeedback) visualFeedback.ScaleDown();
        }
    }
}
