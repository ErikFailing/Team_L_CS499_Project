﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using TMPro;

public class Simulation : MonoBehaviour
{
    private float speed;
    private int pathPosition;
    private int simNum;
    
    private GameObject vacuum;
    private List<Vector3> path;
    private bool paused;
    private bool pathFinished;
    private bool autopilotFinished;

    private string algorithmType;
    private string floorType;

    private List<string> algorithms;
    private List<float> durations;
    private List<float> coverages;

    private Gradient trailGradient;
    

    void Start()
    {
        speed = 1.0f;
        pathPosition = 0;
        simNum = 0;

        paused = false;
        pathFinished = false;
        autopilotFinished = false;

        algorithmType = "Random";
        floorType = "Hardwood";

        // By default indicies 0 to 3 in order are Random, Spiral, Snaking, and Wall follow
        
        algorithms = new List<string>(){"Random", "Spiral", "Snaking", "Wall follow"};
        durations = new List<float>(){0.0f, 0.0f, 0.0f, 0.0f};
        coverages = new List<float>(){0.0f, 0.0f, 0.0f, 0.0f};
    }

    public void Reset()
    {
        StopCoroutine("FollowPath");
        
        pathPosition = 0;
        simNum = 0;
        path = null;

        paused = false;
        
        algorithms = new List<string>(){"Random", "Spiral", "Snaking", "Wall follow"};
        durations = new List<float>(){0.0f, 0.0f, 0.0f, 0.0f};
        coverages = new List<float>(){0.0f, 0.0f, 0.0f, 0.0f};
    }

    private void AddNewSim(string algorithm)
    {
        simNum = algorithms.Count;
        algorithms.Add(algorithm);
        durations.Add(0.0f);
        coverages.Add(0.0f);
        // Rerun algorithm and set the path to it
    }

    public void FindPath()
    {
        if (algorithmType == "Random")
        {
            path = Ref.I.Model.data.RandomPaths[0].vectorThreeList;
            simNum = 0;
        }
        else if (algorithmType == "Spiral")
        {
            path = Ref.I.Model.data.SpiralPaths[0].vectorThreeList;
            simNum = 1;
        }
        else if (algorithmType == "Snaking")
        {
            path = Ref.I.Model.data.SnakingPaths[0].vectorThreeList;
            simNum = 2;
        }
        else if (algorithmType == "Wall follow")
        {
            path = Ref.I.Model.data.WallfollowPaths[0].vectorThreeList;
            simNum = 3;
        }
    }

    public void StartSimulation()
    {
        paused = false;
        if (vacuum == null)
        {
            vacuum = Ref.I.Vacuum;
            if (trailGradient == null)
            {
                ChangeFloorType(floorType);
            }
            vacuum.transform.GetChild(1).GetComponent<TrailRenderer>().colorGradient = trailGradient;
            vacuum.transform.GetChild(2).GetComponent<TrailRenderer>().colorGradient = trailGradient;
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
        }
    }

    public void PauseSimulation()
    {
        paused = true;
        autopilotFinished = true;
        if (path != null)
        {
            StopCoroutine("FollowPath");
        }
    }

    public void StopSimulation()
    {
        paused = false;
        autopilotFinished = true;
        if (path != null)
        {
            StopCoroutine("FollowPath");
            pathPosition = 0;
            if (!pathFinished)
            {
                durations[simNum] = 0.0f;
                coverages[simNum] = 0.0f;
                vacuum.transform.position = path[pathPosition];
                vacuum.transform.GetChild(1).GetComponent<TrailRenderer>().Clear();
                vacuum.transform.GetChild(2).GetComponent<TrailRenderer>().Clear();
            }
        }
    }

    private void Autopilot()
    {
        if (algorithmType == "Random")
        {
            //simNum = 1;
            //ChangeAlgorithm("Spiral");
            simNum = 4;
            ChangeAlgorithm("Wall follow");
            autopilotFinished = true;
        }
        // else if (algorithmType == "Spiral")
        // {
        //     simNum = 2;
        //     ChangeAlgorithm("Snaking");
        // }
        // else if (algorithmType == "Snaking")
        // {
        //     simNum = 3;
        //     ChangeAlgorithm("Wall follow");
        //     autopilotFinished = true;
        // }
    }

    public void ChangeFloorType(string type)
    {
        floorType = type;

        float alpha = 0.0f;
        switch (type)
        {
            case "Hardwood":
                alpha = 0.4f;
                break;
            case "Loop Pile Carpet":
                alpha = 0.3f;
                break;
            case "Cut Pile Carpet":
                alpha = 0.2f;
                break;
            case "Frieze-cut Pile Carpet":
                alpha = 0.1f;
                break;
        }
        trailGradient = new Gradient();
        trailGradient.SetKeys(
            new GradientColorKey[] {new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.blue, 1.0f)},
            new GradientAlphaKey[] {new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f)}
        );
    }

    public void ChangeSpeed(float speed)
    {
        this.speed = speed;
    }

    public void ChangeAlgorithm(string algorithm)
    {
        StopSimulation();
        algorithmType = algorithm;
        FindPath();
    }

    void Update()
    {
        // Coverage
        if (vacuum != null)
        {
            // Also need to retrieve the value here, not just calculate it
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
                if (!paused && RemainingBattery(durations[simNum]) > 0)
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
                    durations[simNum] += Time.deltaTime * currentSpeed;
                    yield return new WaitForEndOfFrame();
                }
                else
                {
                    elapsedTime = duration;
                    if (!paused)
                    {
                        pathFinished = true;
                        Ref.I.PathingDropdown.GetComponent<TMP_Dropdown>().interactable = true;
                        if (!autopilotFinished)
                        {
                            Invoke("Autopilot", 5.0f);
                        }
                    }
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
        sb.Append(coverages[simNum].ToString("0.00%"));
        return sb.ToString();
    }

    private string DurationString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Duration (h:mm:ss):");
        sb.Append(DurationFormat(durations[simNum]));
        return sb.ToString();
    }

    private string BatteryString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Battery Remaining:");
        sb.Append(RemainingBattery(durations[simNum]));
        sb.Append(" Minutes");
        return sb.ToString();
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

    private string SummaryString(int num)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<u><b>");
        sb.Append(algorithms[num]);
        sb.AppendLine("</u></b>");
        sb.Append("Duration: ");
        sb.AppendLine(DurationFormat(durations[num]));
        sb.Append("Battery Remaining: ");
        sb.Append(RemainingBattery(durations[num]));
        sb.AppendLine(" Minutes");
        sb.Append("Coverage: ");
        sb.Append(coverages[num].ToString("0.00%"));
        return sb.ToString();
    }
}
