using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeartrateGraph : MonoBehaviour
{
    public MenuCoordinator menuCoordinator;

    private LineRenderer lineRenderer;
    void Start()
    {
        if (menuCoordinator == null)
        {
            menuCoordinator = GameObject.FindGameObjectWithTag("Menu").GetComponent<MenuCoordinator>();
        }
        lineRenderer = GameObject.FindGameObjectWithTag("Heartrate Graph Target").GetComponent<LineRenderer>();
    }
    public void SetupHeartrateGraph(Dictionary<int, HRLog> hrLogDic)
    {      
        lineRenderer.positionCount = 0;
        lineRenderer.positionCount = menuCoordinator.GetReplayManager().GetHRLog().Count + 1;
        int point = 0;
        Vector3 position = new Vector3(-400, -20, -1);
        lineRenderer.SetPosition(point, position);

        point++;

        float normalizedXValue = 800 / (menuCoordinator.GetTimelineMaxValue() - menuCoordinator.GetTimelineMinValue());

        foreach (KeyValuePair<int, HRLog> log in hrLogDic)
        {
            if (log.Value.heartRate < 20)
            {
                lineRenderer.positionCount--;
            }
            else
            {
                Color currentColor = GetLineColorBasedOnHeartRate(log.Value.heartRate);

                position.x = (log.Key * normalizedXValue) - 400;
                position.y = (log.Value.heartRate) - 100;
                lineRenderer.SetPosition(point, position);
                lineRenderer.startColor = currentColor;
                lineRenderer.endColor = currentColor;
                point++;
            }
        }

        lineRenderer.Simplify(0.01f);
    }

    private Color GetLineColorBasedOnHeartRate(int heartRate)
    {
        if (heartRate < 60)
        {
            return Color.blue;
        }
        else if (heartRate >= 60 && heartRate < 100)
        {
            return Color.green;
        }
        else if (heartRate >= 100 && heartRate < 140)
        {
            return Color.yellow;
        }
        else if (heartRate >= 140 &&  heartRate < 170)
        {
            return new Color(241, 90, 34);
        }
        else
        {
            return Color.red;
        }
    }

    public void DeleteGraph()
    {
        lineRenderer.positionCount = 0;
    }

}
