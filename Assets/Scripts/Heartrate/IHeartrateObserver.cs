using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHeartrateObserver
{
    void Initialize(AudioFeedback audioFeedback, VisualFeedback visualFeedback);
    void UpdateHeartbeat(IHeartbeatState state, bool useAudioFeedback, bool useHapticFeedback, bool useVisualFeedback);
}
