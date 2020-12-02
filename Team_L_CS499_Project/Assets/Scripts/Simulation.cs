using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using TMPro;

public class Simulation : MonoBehaviour
{
    private float speed = 1.0f;
    private int pathPosition = 1;
    private GameObject vacuum;
    private List<Vector3> path;
    private bool paused = false;

    // 0 Random, 1 Spiral, 2 Snaking, 3 Wall follow
    private int pathAlgorithm;
    private string floorType = "Hardwood";

    // [0] Random, [1] Spiral, [2] Snaking, [3] Wall follow
    private float[] durations = new float[4] {0.0f, 0.0f, 0.0f, 0.0f};
    private float[] coverages = new float[4] {0.0f, 0.0f, 0.0f, 0.0f};

    public void FindPath()
    {
        path = Ref.I.Model.data.RandomPaths[0].vectorThreeList;
    }

    public void StartSimulation()
    {
        paused = false;
        if (vacuum == null)
        {
            vacuum = Ref.I.Vacuum;
        }
        if (path == null)
        {
            FindPath();
        }
        if (path != null)
        {
            if (pathPosition != path.Count)
            {
                StartCoroutine("FollowPath");
            }
            else
            {
                if (!paused)
                {
                    //Invoke("", 5)
                }
            }
        }
    }

    public void PauseSimulation()
    {
        paused = true;
        if (path != null)
        {
            StopCoroutine("FollowPath");
        }
    }

    public void StopSimulation()
    {
        paused = false;
        if (path != null)
        {
            StopCoroutine("FollowPath");
            pathPosition = 1;
            vacuum.transform.position = path[pathPosition];
            vacuum.GetComponent<TrailRenderer>().Clear();
        }
    }

    public void ChangeFloorType(string type)
    {
        floorType = type;
    }

    public void ChangeSpeed(float speed)
    {
        this.speed = speed;
    }

    public void ChangeAlgorithm(int algorithm)
    {
        pathAlgorithm = algorithm;
        pathPosition = 1;
        vacuum.transform.position = path[pathPosition];
    }

    void Update()
    {
        // Coverage
        if (vacuum != null)
        {
            //Ref.I.Model.CalculateCleanliness(durations[0] * 3);
        }
        // GUI
        Ref.I.SimFloorTypeText.GetComponent<TextMeshProUGUI>().text = FloorTypeString();
        Ref.I.SimCoverageText.GetComponent<TextMeshProUGUI>().text = CoverageString();
        Ref.I.SimDurationText.GetComponent<TextMeshProUGUI>().text = DurationString();
        Ref.I.SimRemainingText.GetComponent<TextMeshProUGUI>().text = BatteryString();
        Ref.I.SummaryRandomText.GetComponent<TextMeshProUGUI>().text = SummaryString(0);
        Ref.I.SummarySpiralText.GetComponent<TextMeshProUGUI>().text = SummaryString(1);
        Ref.I.SummarySnakingText.GetComponent<TextMeshProUGUI>().text = SummaryString(2);
        Ref.I.SummaryWallFollowText.GetComponent<TextMeshProUGUI>().text = SummaryString(3);
    }

    

    public IEnumerator FollowPath()
    {
        // This function will move the vacuum GameObject through the path at the speed of 3 in/s (3 units/s)
        while (pathPosition < path.Count)
        {
            Vector3 target = path[pathPosition];
            float elapsedTime = 0;
            Vector3 startingPos = vacuum.transform.position;
            float distance = Vector3.Distance(startingPos, target);
            float duration = distance / (3.0f * speed);
            float currentSpeed = speed;
            while (elapsedTime < duration)
            {
                if (!paused && RemainingBattery(durations[pathAlgorithm]) > 0)
                {
                    if (speed != currentSpeed)
                    {
                        // Updating variables to use the new speed
                        elapsedTime = 0;
                        startingPos = vacuum.transform.position;
                        distance = Vector3.Distance(startingPos, target);
                        duration = distance / (3.0f * speed);
                        currentSpeed = speed;
                    }
                    vacuum.transform.position = Vector3.Lerp(startingPos, target, (elapsedTime / duration));
                    elapsedTime += Time.deltaTime;
                    durations[pathAlgorithm] += Time.deltaTime * currentSpeed;
                    yield return new WaitForEndOfFrame();
                }
                else
                {
                    elapsedTime = duration;
                }
            }
            pathPosition++;
        }
    }

    private string FloorTypeString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Floor Type:");
        sb.Append(floorType);
        return sb.ToString();
    }

    private string CoverageString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("Coverage: ");
        sb.Append(coverages[pathAlgorithm].ToString("0.00%"));
        return sb.ToString();
    }

    private string DurationString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Duration (h:mm:ss):");
        sb.Append(DurationFormat(durations[pathAlgorithm]));
        return sb.ToString();
    }

    private string BatteryString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Battery Remaining:");
        sb.Append(RemainingBattery(durations[pathAlgorithm]));
        sb.Append(" Minutes");
        return sb.ToString();
    }

    private string AlgorithmToString(int algorithm)
    {
        switch (algorithm)
        {
            case 0:
                return "Random";
            case 1:
                return "Spiral";
            case 2:
                return "Snaking";
            case 3:
                return "Wall follow";
            default:
                return "Undefined";
        }
    }

    private int RemainingBattery(float duration)
    {
        // returns in minutes
        int maxBattery = 150;
        return Mathf.Max(maxBattery - (int)Math.Floor(duration / 60), 0);
    }

    private string DurationFormat(float duration)
    {
        // returns in format h:mm:ss
        StringBuilder sb = new StringBuilder();
        int seconds = (int)Math.Floor(duration) % 60;
        int minutes = (int)Math.Floor(duration / 60) % 60;
        int hours = (int)Math.Floor(duration / (60 * 60)) % 24;
        sb.Append(hours);
        sb.Append(":");
        sb.Append(minutes.ToString("00"));
        sb.Append(":");
        sb.Append(seconds.ToString("00"));
        return sb.ToString();
    }

    private string SummaryString(int algorithm)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<u><b>");
        sb.Append(AlgorithmToString(algorithm));
        sb.AppendLine("</u></b>");
        sb.Append("Duration: ");
        sb.AppendLine(DurationFormat(durations[algorithm]));
        sb.Append("Battery Remaining: ");
        sb.Append(RemainingBattery(durations[algorithm]));
        sb.AppendLine(" Minutes");
        sb.Append("Coverage: ");
        sb.Append(coverages[algorithm].ToString("0.00%"));
        return sb.ToString();
    }
}
