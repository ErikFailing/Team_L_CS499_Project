using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using TMPro;

public class Run
{
    public string name;
    public string algorithm;
    public float duration;
    public float currentCoverage;
    public float finishedCoverage;
    public List<Vector3> path;

    public Run()
    {
        finishedCoverage = 0;
    }
    public Run(string alg, int index, List<Vector3> p)
    {
        algorithm = alg;
        name = alg + " #" + index;
        duration = 0.0f;
        currentCoverage = 0.0f;
        finishedCoverage = 0;
        path = p;
    }
}

public class Simulation : MonoBehaviour
{
    private float speed;
    private int pathPosition;
    public int runNum;
    
    private GameObject vacuum;
    private List<Vector3> path;
    private bool paused;
    private bool pathFinished;
    private bool autopilotFinished;

    public string algorithmType;
    private string floorType;

    public List<Run> runs;

    private Gradient trailGradient;

    private bool stopped;
    
    // For updating the clean percentage
    private int nextUpdate;

    void Start()
    {
        // Initialize Simulation with default values
        speed = 1.0f;
        floorType = "Hardwood";
        Reset();
        stopped = true;
    }

    public void CalculateFinishedCoverages()
    {
        int initialRunNum = runNum;
        for (int i =0; i < runs.Count; i++)
        {
            if (runs[i].finishedCoverage == 0)
            {
                runNum = i;
                Ref.I.Model.CalculateCleanliness(26999);
                runs[i].finishedCoverage = Ref.I.Model.CalculateCoveragePercentage();
                // Reset distance tracker for the coverage calculations
                Ref.I.Model.currentDistance = 0;
                Ref.I.Model.ResetPoints();
            }
        }
        runNum = initialRunNum;
        
    }

    public void Load()
    {
        // Re-Initialize simulation with default values
        StopCoroutine("FollowPath");

        algorithmType = "Random";

        pathPosition = 0;
        runNum = 0;

        paused = false;
        pathFinished = false;
        stopped = true;
        autopilotFinished = true;

        // Add algorithms to the Run list
        int algsNum = 0;
        foreach (var listWrapper in Ref.I.Model.data.RandomPaths)
        {
            runs.Add(new Run("Random", algsNum, listWrapper.vectorThreeList));
            algsNum += 1;
        }
        foreach (var listWrapper in Ref.I.Model.data.SnakingPaths)
        {
            runs.Add(new Run("Snaking", algsNum, listWrapper.vectorThreeList));
            algsNum += 1;
        }
        foreach (var listWrapper in Ref.I.Model.data.SpiralPaths)
        {
            runs.Add(new Run("Spiral", algsNum, listWrapper.vectorThreeList));
            algsNum += 1;
        }
        foreach (var listWrapper in Ref.I.Model.data.WallfollowPaths)
        {
            runs.Add(new Run("Wall follow", algsNum, listWrapper.vectorThreeList));
            algsNum += 1;
        }

        if (algsNum > 0) path = runs[0].path;
        else path = null;

        // Update dropdowns
        UpdateRunDropdown();
        Ref.I.PathingDropdown.GetComponent<TMP_Dropdown>().SetValueWithoutNotify(0);
        Ref.I.RunDropdown.GetComponent<TMP_Dropdown>().SetValueWithoutNotify(0);

        // Reset distance tracker for the coverage calculations
        Ref.I.Model.currentDistance = 0;
        Ref.I.Model.ResetPoints();
    }

    public void Reset()
    {
        // Re-Initialize simulation with default values
        StopCoroutine("FollowPath");
        
        algorithmType = "Random";

        pathPosition = 0;
        runNum = 0;
        path = null;

        paused = false;
        pathFinished = false;
        autopilotFinished = false;

        runs = new List<Run>();
        stopped = true;

        UpdateRunDropdown();
        Ref.I.PathingDropdown.GetComponent<TMP_Dropdown>().SetValueWithoutNotify(0);
        
        // Reset distance tracker for the coverage calculations
        Ref.I.Model.currentDistance = 0;
        Ref.I.Model.ResetPoints();
    }

    private void UpdateRunDropdown()
    {
        // The Vacuum Run Dropdown is updated here to reflect the instances of Run that were created
        //runNum = runs.Count;
        int nextRunNum = runs.Count;
        // Debug.Log("Number of instances of Run: " + nextRunNum);
        TMP_Dropdown dropdown = Ref.I.RunDropdown.GetComponent<TMP_Dropdown>();
        List<string> options = new List<string>();
        foreach (Run run in runs)
        {
            options.Add(run.name);
        }
        // Add new options
        if (!autopilotFinished)
        {
            options.Add("(Auto) " + algorithmType + " #" + nextRunNum);
        }
        options.Add("(New) " + algorithmType + " #" + nextRunNum);
        // Save previously dropdown
        int prevValue = dropdown.value;
        // Remove all options displayed
        dropdown.ClearOptions();
        // Display all of the new options
        dropdown.AddOptions(options);
        if (!autopilotFinished && pathFinished)
        {
            // I don't think this is not working
            dropdown.SetValueWithoutNotify(options.Count - 2);
        } else {
            dropdown.SetValueWithoutNotify(prevValue);
        }
    }

    private void CreateNewRun()
    {
        // When creating an instance of Run all of the properties need to be initialized
        runNum = runs.Count;
        Run run = new Run();
        runs.Add(run);
        run.name = algorithmType + " #" + runNum;
        run.algorithm = algorithmType;
        run.duration = 0.0f;
        run.currentCoverage = 0.0f;
        path = FindPath();
        run.path = path;
    }

    public List<Vector3> FindPath()
    {
        // Create the new path and return it
        if (algorithmType == "Random")
        {
            Ref.I.Model.CalculateRandomPath();
            return Ref.I.Model.data.RandomPaths[Ref.I.Model.data.RandomPaths.Count - 1].vectorThreeList;
        }
        else if (algorithmType == "Spiral")
        {
            Ref.I.Model.CalculateSpiralPath();
            return Ref.I.Model.data.SpiralPaths[Ref.I.Model.data.SpiralPaths.Count - 1].vectorThreeList;
        }
        else if (algorithmType == "Snaking")
        {
            Ref.I.Model.CalculateSnakingPath();
            return Ref.I.Model.data.SnakingPaths[Ref.I.Model.data.SnakingPaths.Count - 1].vectorThreeList;
        }
        else if (algorithmType == "Wall follow")
        {
            Ref.I.Model.CalculateWallFollowPath();
            return Ref.I.Model.data.WallfollowPaths[Ref.I.Model.data.WallfollowPaths.Count - 1].vectorThreeList;
        }
        return null;
    }

    public void StartSimulation()
    {
        paused = false;
        stopped = false;

        // If the vacuum hasn't been assigned, then assign it and set the trail gradient based off of the floor type
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

        // Find a new path if the path does not exist or the path is empty
        if (path == null || path.Count == 0)
        {
            TMP_Dropdown dropdown = Ref.I.RunDropdown.GetComponent<TMP_Dropdown>();
            string text = dropdown.options[dropdown.value].text;
            if (text.Contains("Auto"))
            {
                CreateNewRun();
            }
            else if (text.Contains("New"))
            {
                autopilotFinished = true;
                CreateNewRun();
            }
            else
            {
                runNum = dropdown.value;
            }
        }
        UpdateRunDropdown();

        // Start the Coroutine to make the vacuum follow the path
        if (path != null)
        {
            if (pathPosition != path.Count)
            {
                pathFinished = false;
                StartCoroutine("FollowPath");
            }
        }
    }

    public void PauseSimulation()
    {
        paused = true;

        // Stop the Coroutine and inside of the function it handles stoping the movement execution loop
        if (path != null)
        {
            StopCoroutine("FollowPath");
        }
    }

    public void StopSimulation()
    {
        paused = false;
        autopilotFinished = true;
        stopped = true;

        // Stop the simulation run if the run had already started
        if (path != null && path.Count > 0)
        {
            StopCoroutine("FollowPath");
            pathPosition = 0;
            if (!pathFinished)
            {
                runs[runNum].duration = 0.0f;
                runs[runNum].currentCoverage = 0.0f;
            }
            vacuum.transform.position = path[pathPosition];
            vacuum.transform.GetChild(1).GetComponent<TrailRenderer>().Clear();
            vacuum.transform.GetChild(2).GetComponent<TrailRenderer>().Clear();
        }
        UpdateRunDropdown();

        Ref.I.Model.currentDistance = 0;
        Ref.I.Model.ResetPoints();
    }

    private void Autopilot()
    {
        if (algorithmType == "Random")
        {
            ChangeAlgorithm("Spiral");
            autopilotFinished = false;
            Ref.I.PathingDropdown.GetComponent<TMP_Dropdown>().SetValueWithoutNotify(1);
            Ref.I.RunDropdown.GetComponent<TMP_Dropdown>().SetValueWithoutNotify(1);
        }
        else if (algorithmType == "Spiral")
        {
            ChangeAlgorithm("Snaking");
            autopilotFinished = false;
            Ref.I.PathingDropdown.GetComponent<TMP_Dropdown>().SetValueWithoutNotify(2);
            Ref.I.RunDropdown.GetComponent<TMP_Dropdown>().SetValueWithoutNotify(2);
        }
        else if (algorithmType == "Snaking")
        {
            ChangeAlgorithm("Wall follow");
            autopilotFinished = true;
            Ref.I.PathingDropdown.GetComponent<TMP_Dropdown>().SetValueWithoutNotify(3);
            Ref.I.RunDropdown.GetComponent<TMP_Dropdown>().SetValueWithoutNotify(3);
        }
        CreateNewRun();
        Ref.I.GUI.PlayButtonClick();
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

    public void ChangeRun(string text)
    {
        if (!text.Contains("Auto") && !text.Contains("New"))
        {
            // Use old run
            runNum = Ref.I.RunDropdown.GetComponent<TMP_Dropdown>().value;
            path = runs[runNum].path;
        }
        else
        {
            // Create new run
            path = null;
        }
    }

    public void ChangeAlgorithm(string algorithm)
    {
        StopSimulation();
        algorithmType = algorithm;
        Ref.I.Model.ResetPoints();
        Ref.I.Model.currentDistance = 0;
        UpdateRunDropdown();
        Ref.I.RunDropdown.GetComponent<TMP_Dropdown>().SetValueWithoutNotify(Ref.I.RunDropdown.GetComponent<TMP_Dropdown>().options.Count);
        ChangeRun("New");
    }

    void Update()
    {
        // Coverage
        if (vacuum != null && !paused && !stopped)
        {
            // If the next update is reached
            if (Time.time >= nextUpdate)
            {
                // Change the next update (current second+1)
                nextUpdate = Mathf.FloorToInt(Time.time) + 1;
                // Call your fonction
                // Also need to retrieve the value here, not just calculate it
                
                runs[runNum].currentCoverage = Ref.I.Model.CalculateCoveragePercentage();
            }
            Ref.I.Model.CalculateCleanliness(Mathf.FloorToInt(runs[runNum].duration * 3));
        }
        // GUI
        if (runs.Count > 0) {
            Ref.I.SimFloorTypeText.GetComponent<TextMeshProUGUI>().text = FloorTypeString();
            Ref.I.SimCoverageText.GetComponent<TextMeshProUGUI>().text = CoverageString();
            Ref.I.SimDurationText.GetComponent<TextMeshProUGUI>().text = DurationString();
            Ref.I.SimRemainingText.GetComponent<TextMeshProUGUI>().text = BatteryString();

            
        }
    }

    
    

    public IEnumerator FollowPath()
    {
        bool finished = false;
        // This function will move the vacuum GameObject through the path at the speed of 3 in/s (3 units/s)
        while (pathPosition < path.Count && !finished)
        {
            Vector3 target = path[pathPosition];
            float elapsedTime = 0;
            Vector3 startingPos = vacuum.transform.position;
            float distance = Vector3.Distance(startingPos, target);
            float duration = distance / (3.0f * speed);
            float currentSpeed = speed;
            while (elapsedTime < duration)
            {
                if (!paused && RemainingBattery(runs[runNum].duration) > 0)
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
                    runs[runNum].duration += Time.deltaTime * currentSpeed;
                    yield return new WaitForEndOfFrame();
                }
                else
                {
                    // Exit function loop variable
                    finished = true;
                    if (!paused)
                    {
                        pathFinished = true;

                        Ref.I.PathingDropdown.GetComponent<TMP_Dropdown>().interactable = true;
                        Ref.I.RunDropdown.GetComponent<TMP_Dropdown>().interactable = true;
                        
                        if (!autopilotFinished)
                        {
                            // Start next autopilot algorithm run
                            Autopilot();
                        }
                    }
                    // Exit movement loop
                    break;
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
        sb.Append(runs[runNum].currentCoverage.ToString("0.00%"));
        return sb.ToString();
    }

    private string DurationString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Duration (h:mm:ss):");
        sb.Append(DurationFormat(runs[runNum].duration));
        return sb.ToString();
    }

    private string BatteryString()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Battery Remaining:");
        sb.Append(RemainingBattery(runs[runNum].duration));
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



    public void UpdateSummaryOverlay()
    {
        CalculateFinishedCoverages();
        
        
        Ref.I.SummaryTopLeftText.GetComponent<TextMeshProUGUI>().text = "Run one or more RANDOM simulations to recieve a results summary.";
        Ref.I.SummaryTopRightText.GetComponent<TextMeshProUGUI>().text = "Run one or more SPIRAL simulations to recieve a results summary.";
        Ref.I.SummaryBottomLeftText.GetComponent<TextMeshProUGUI>().text = "Run one or more SNAKING simulations to recieve a results summary.";
        Ref.I.SummaryBottomRightText.GetComponent<TextMeshProUGUI>().text = "Run one or more WALL FOLLOW simulations to recieve a results summary.";

        if (Ref.I.Model.data.RandomPaths.Count > 0)
        {
            float totalCoverage = 0;
            foreach (Run r in runs)
            {
                if (r.algorithm == "Random")
                {
                    totalCoverage += r.finishedCoverage;
                }
            }
            float avgCoverage = totalCoverage / Ref.I.Model.data.RandomPaths.Count;

            Ref.I.SummaryTopLeftText.GetComponent<TextMeshProUGUI>().text = SummaryString("Random Summary: ",
                Ref.I.Model.data.RandomPaths.Count, avgCoverage);
        }
        if (Ref.I.Model.data.SpiralPaths.Count > 0)
        {
            float totalCoverage = 0;
            foreach (Run r in runs)
            {
                if (r.algorithm == "Spiral")
                {
                    totalCoverage += r.finishedCoverage;
                }
            }
            float avgCoverage = totalCoverage / Ref.I.Model.data.SpiralPaths.Count;

            Ref.I.SummaryTopRightText.GetComponent<TextMeshProUGUI>().text = SummaryString("Spiral Summary: ",
                Ref.I.Model.data.SpiralPaths.Count, avgCoverage);
        }
        if (Ref.I.Model.data.SnakingPaths.Count > 0)
        {
            float totalCoverage = 0;
            foreach (Run r in runs)
            {
                if (r.algorithm == "Snaking")
                {
                    totalCoverage += r.finishedCoverage;
                }
            }
            float avgCoverage = totalCoverage / Ref.I.Model.data.SnakingPaths.Count;

            Ref.I.SummaryBottomLeftText.GetComponent<TextMeshProUGUI>().text = SummaryString("Snaking Summary: ",
                Ref.I.Model.data.SnakingPaths.Count, avgCoverage);
        }
        if (Ref.I.Model.data.WallfollowPaths.Count > 0)
        {
            float totalCoverage = 0;
            foreach (Run r in runs)
            {
                if (r.algorithm == "Wall follow")
                {
                    totalCoverage += r.finishedCoverage;
                }
            }
            float avgCoverage = totalCoverage / Ref.I.Model.data.WallfollowPaths.Count;

            Ref.I.SummaryBottomLeftText.GetComponent<TextMeshProUGUI>().text = SummaryString("Wall Follow Summary: ",
                Ref.I.Model.data.WallfollowPaths.Count, avgCoverage);
        }

    }


    private string SummaryString(string type, int numberOfRuns, float avgCoverage)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("<u><b>");
        sb.Append(type);
        sb.AppendLine("</u></b>");
        sb.Append("Total Run Time: ");
        sb.AppendLine(DurationFormat(numberOfRuns * 150 * 60));
        sb.Append("Average Coverage: ");
        sb.AppendLine(avgCoverage.ToString("0.00%"));
        sb.Append("Runs: ");
        sb.AppendLine(numberOfRuns.ToString());
        return sb.ToString();
    }
}
