using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterLogger : MonoBehaviour
{
    private FighterStudyLog log;
    private FighterCoordinator coordinator;

    private void Start()
    {
        log = new FighterStudyLog();

        coordinator = FighterCoordinator.Instance;

        coordinator.OnBodyHidden += Coordinator_OnHiddenShownChange;
        coordinator.OnBodyShown += Coordinator_OnHiddenShownChange;
        coordinator.OnHeadShown += Coordinator_OnHiddenShownChange;
        coordinator.OnHeadHidden += Coordinator_OnHiddenShownChange;
        coordinator.OnLeftHandHidden += Coordinator_OnHiddenShownChange;
        coordinator.OnLeftHandShown += Coordinator_OnHiddenShownChange;
        coordinator.OnRightHandHidden += Coordinator_OnHiddenShownChange;
        coordinator.OnRightHandShown += Coordinator_OnHiddenShownChange;
    }

    private void Coordinator_OnHiddenShownChange(object sender, System.EventArgs e)
    {
        log.rightHandShown = coordinator.IsRightHandShown();
        log.leftHandShown = coordinator.IsLeftHandShown();
        log.headShown = coordinator.IsHeadShown();
        log.bodyShown = coordinator.IsBodyShown();

        Logger.Instance.Log(log);
    }
}
