using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.IO;

public class GUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // General GUI methods

    public void BackButtonClick()
    {
        // Set Main Menu active and everything else not active (and setting help overlays to not active too)
        Ref.I.MainMenu.SetActive(true);
        Ref.I.MainHelpOverlay.SetActive(false);
        Ref.I.HousePlannerMenu.SetActive(false);
        Ref.I.HousePlannerHelpOverlay.SetActive(false);
        Ref.I.LoadSimulationMenu.SetActive(false);
        Ref.I.LoadSimulationHelpOverlay.SetActive(false);
        Ref.I.HouseSimulationMenu.SetActive(false);
        Ref.I.HouseSimulationHelpOverlay.SetActive(false);
        StopButtonClick();
        Ref.I.Simulation.Reset();
        Ref.I.Model.RemoveEverything();
        Ref.I.PlanNameInput.GetComponent<TMP_InputField>().text = "";
    }

    private void OverlayToggle(GameObject o)
    {
        if (o.activeInHierarchy)
        {
            o.SetActive(false);
        }
        else
        {
            o.SetActive(true);
        }
    }
    
    public void HelpButtonClick()
    {
        // Can be used by any menu's help button
        if (Ref.I.HousePlannerMenu.activeInHierarchy)
        {
            OverlayToggle(Ref.I.HousePlannerHelpOverlay);
        }
        else if (Ref.I.HouseSimulationMenu.activeInHierarchy)
        {
            OverlayToggle(Ref.I.HouseSimulationHelpOverlay);
        }
        else if (Ref.I.LoadSimulationMenu.activeInHierarchy)
        {
            OverlayToggle(Ref.I.LoadSimulationHelpOverlay);
        }
        else if (Ref.I.MainMenu.activeInHierarchy)
        {
            OverlayToggle(Ref.I.MainHelpOverlay);
        }
    }

    public void TwoDButtonClick()
    {
        foreach (GameObject o in Ref.I.TwoDButtons)
        {
            o.GetComponent<Button>().interactable = false;
        }
        foreach (GameObject o in Ref.I.ThreeDButtons)
        {
            o.GetComponent<Button>().interactable = true;
        }
        Ref.I.Camera.GetComponent<CameraControl>().SwapCameraViewTo2D();
    }

    public void ThreeDButtonClick()
    {
        foreach (GameObject o in Ref.I.TwoDButtons)
        {
            o.GetComponent<Button>().interactable = true;
        }
        foreach (GameObject o in Ref.I.ThreeDButtons)
        {
            o.GetComponent<Button>().interactable = false;
        }
        Ref.I.Camera.GetComponent<CameraControl>().SwapCameraViewTo3D();
    }
    
    
    // Main Menu GUI Methods

    public void HousePlannerButtonClick()
    {
        // Close help overlay if necessary
        if (Ref.I.MainHelpOverlay.activeInHierarchy)
        {
            Ref.I.MainHelpOverlay.SetActive(false);
        }
        Ref.I.MainMenu.SetActive(false);
        Ref.I.HousePlannerMenu.SetActive(true);
        Ref.I.FlooringDropdown.GetComponent<TMP_Dropdown>().SetValueWithoutNotify(0);
    }

    public void LoadSimulationButtonClick()
    {
        // Close help overlay if necessary
        if (Ref.I.MainHelpOverlay.activeInHierarchy)
        {
            Ref.I.MainHelpOverlay.SetActive(false);
        }
        Ref.I.MainMenu.SetActive(false);
        Ref.I.LoadSimulationMenu.SetActive(true);
        InstantiateSaveFileMenus();
    }

    public void QuitButtonClick()
    {
        Application.Quit();
    }


    // House Planner GUI Methods
    public void DrawRoomButtonClick()
    {
        Ref.I.ModelVisuals.objectToPlace = "Room";
        Ref.I.ModelVisuals.readyToSelect = true;
        Ref.I.ModelVisuals.placing = false;
    }

    public void FloorTypeDropdownValueChanged(TextMeshProUGUI tmpro)
    {
        Ref.I.Model.ChangeFloorType(tmpro.text);
        Ref.I.Simulation.ChangeFloorType(tmpro.text);
    }

    public void PlaceTableButtonClick()
    {
        
        Ref.I.ModelVisuals.objectToPlace = "Table";
        Ref.I.ModelVisuals.readyToSelect = true;
        Ref.I.ModelVisuals.placing = false;
    }

    public void PlaceChestButtonClick()
    {
        Ref.I.ModelVisuals.objectToPlace = "Chest";
        Ref.I.ModelVisuals.readyToSelect = true;
        Ref.I.ModelVisuals.placing = false;
    }
    public void DrawDoorButtonClick()
    {

        Ref.I.ModelVisuals.objectToPlace = "Door";
        Ref.I.ModelVisuals.readyToSelect = true;
        Ref.I.ModelVisuals.placing = false;

    }

    public void PlaceVacuumButtonClick()
    {
        
        Ref.I.ModelVisuals.objectToPlace = "Vacuum";
        Ref.I.ModelVisuals.placing = true;
        Ref.I.ModelVisuals.readyToSelect = false;

    }

    public void DeleteObjectButtonClick()
    {
        Ref.I.ModelVisuals.deleting = true;
        Ref.I.ModelVisuals.placing = false;
        Ref.I.ModelVisuals.readyToSelect = false;
        Ref.I.ModelVisuals.ColorSelectionCube(false);
    }

    public void FinalizeDesignButtonClick()
    {
        string errorMsg = "";
        if (Ref.I.Model.VerifyHousePlan(out errorMsg))
        {
            // Valid House
            Ref.I.HousePlannerMenu.SetActive(false);
            Ref.I.HouseSimulationMenu.SetActive(true);
            // Create save
            string saveName = SaveSystem.CreateSaveFile(Ref.I.Model.data.name);
            SaveSystem.SaveToFile(saveName);

        }
        else
        {
            // Invalid House
            Debug.Log(errorMsg);
        }
    }

    public void UpdateSquareFootage(int total)
    {
        Ref.I.SquareFeetText.GetComponent<TextMeshProUGUI>().text = "Total Area:\n" + total + " ft<sup>2</sup>";
    }

    public void UpdateUncleanableArea(int area)
    {
        Ref.I.UncleanableAreaText.GetComponent<TextMeshProUGUI>().text = "Uncleanable Area:\n" + area + " ft<sup>2</sup>";
    }

    public void HousePlanNameChanged(TextMeshProUGUI tmp)
    {
        Ref.I.Model.data.name = tmp.text;
    }


    // House Simulation GUI Methods
    public void PlayButtonClick()
    {
        Ref.I.PlayButton.GetComponent<Button>().interactable = false;
        Ref.I.Simulation.StartSimulation();
        Ref.I.PauseButton.GetComponent<Button>().interactable = true;
        Ref.I.StopButton.GetComponent<Button>().interactable = true;
        Ref.I.PathingDropdown.GetComponent<TMP_Dropdown>().interactable = false;
        Ref.I.RunDropdown.GetComponent<TMP_Dropdown>().interactable = false;
    }

    public void PauseButtonClick()
    {
        Ref.I.PauseButton.GetComponent<Button>().interactable = false;
        Ref.I.Simulation.PauseSimulation();
        Ref.I.PlayButton.GetComponent<Button>().interactable = true;
    }

    public void StopButtonClick()
    {
        Ref.I.StopButton.GetComponent<Button>().interactable = false;
        Ref.I.Simulation.StopSimulation();
        Ref.I.PauseButton.GetComponent<Button>().interactable = false;
        Ref.I.PlayButton.GetComponent<Button>().interactable = true;
        Ref.I.PathingDropdown.GetComponent<TMP_Dropdown>().interactable = true;
        Ref.I.RunDropdown.GetComponent<TMP_Dropdown>().interactable = true;
    }

    public void SaveSimulation()
    {
        SaveSystem.SaveToFile(Ref.I.Model.data.name);
        BackButtonClick();
    }

    public void OneSpeedButtonClick()
    {
        Ref.I.Simulation.ChangeSpeed(1.0f);
        Ref.I.OneSpeedButton.GetComponent<Button>().interactable = false;
        Ref.I.FiftySpeedButton.GetComponent<Button>().interactable = true;
        Ref.I.HundredSpeedButton.GetComponent<Button>().interactable = true;
    }

    public void FiftySpeedButtonClick()
    {
        Ref.I.Simulation.ChangeSpeed(50.0f);
        Ref.I.OneSpeedButton.GetComponent<Button>().interactable = true;
        Ref.I.FiftySpeedButton.GetComponent<Button>().interactable = false;
        Ref.I.HundredSpeedButton.GetComponent<Button>().interactable = true;
    }

    public void HundredSpeedButtonClick()
    {
        Ref.I.Simulation.ChangeSpeed(100.0f);
        Ref.I.OneSpeedButton.GetComponent<Button>().interactable = true;
        Ref.I.FiftySpeedButton.GetComponent<Button>().interactable = true;
        Ref.I.HundredSpeedButton.GetComponent<Button>().interactable = false;
    }

    public void SummaryButtonClick()
    {
        Ref.I.Simulation.UpdateSummaryOverlay();
        OverlayToggle(Ref.I.SummaryOverlay);
    }

    public void VacuumRunDropdownValueChanged(TextMeshProUGUI tmpro)
    {
        Ref.I.Simulation.ChangeRun(tmpro.text);
    }

    public void PathAlgorithmDropdownValueChanged(TextMeshProUGUI tmpro)
    {
        Ref.I.Simulation.ChangeAlgorithm(tmpro.text);
    }


    // Load Simulation GUI Methods
    public void LoadSimulation(string saveName)
    {
        Ref.I.LoadSimulationMenu.SetActive(false);
        Ref.I.HouseSimulationMenu.SetActive(true);
        // Load Simulation
        SaveSystem.LoadSaveFile(saveName);
        Ref.I.Simulation.Load();

        // Delete all current listeners
        Ref.I.LoadSaveButton.GetComponent<Button>().onClick.RemoveAllListeners();
        Ref.I.DeleteSaveButton.GetComponent<Button>().onClick.RemoveAllListeners();
    }

    public void DeleteSimulation(string saveName)
    {
        SaveSystem.DeleteSaveFile(saveName);
        // Delete Button
        Destroy(Ref.I.LoadSimulationViewport.transform.Find(saveName).gameObject);
        // Delete all current listeners
        Ref.I.DeleteSaveButton.GetComponent<Button>().onClick.RemoveAllListeners();
        Ref.I.LoadSaveButton.GetComponent<Button>().onClick.RemoveAllListeners();
    }

    public void OpenDatabaseFolder()
    {
        Application.OpenURL("file://" + Application.persistentDataPath);
    }

    public void UpdateDeleteSaveListeners(string fileName)
    {
        // Delete all current listeners
        Ref.I.DeleteSaveButton.GetComponent<Button>().onClick.RemoveAllListeners();
        // Add listener
        Ref.I.DeleteSaveButton.GetComponent<Button>().onClick.AddListener(delegate { DeleteSimulation(fileName); });
    }
    public void UpdateLoadSaveListeners(string fileName)
    {
        // Delete all current listeners
        Ref.I.LoadSaveButton.GetComponent<Button>().onClick.RemoveAllListeners();
        // Add listener
        Ref.I.LoadSaveButton.GetComponent<Button>().onClick.AddListener(delegate { LoadSimulation(fileName); });
    }


    /// <summary>
    /// Instantiates the gameFileMenus in the load game menu
    /// </summary>
    private void InstantiateSaveFileMenus()
    {
        //Destroy any current loadSaveFile buttons
        for (int i = Ref.I.LoadSimulationViewport.childCount-1; i >= 0; i--)
        {
            Destroy(Ref.I.LoadSimulationViewport.GetChild(i).gameObject);
        }
        
        //Instantiate a game file menu for each saved game
        foreach (string saveName in SaveSystem.GetSaves())
        {
            //Instantiate a menu corresponding to the saved game
            GameObject saveFileButton = Instantiate(Ref.I.saveFileMenuPrefab, Ref.I.LoadSimulationViewport);
            // Set text & Name
            saveFileButton.name = saveName;
            saveFileButton.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = saveName;
            var lastModified = File.GetLastWriteTime(SaveSystem.SaveNameToFilePath(saveName));
            saveFileButton.transform.Find("Date").GetComponent<TextMeshProUGUI>().text = lastModified.ToString("MM/dd/yyyy HH:mm:ss");

            //Assign the onclick functions to the buttons
            saveFileButton.GetComponent<Button>().onClick.AddListener(delegate { UpdateLoadSaveListeners(saveName); });
            saveFileButton.GetComponent<Button>().onClick.AddListener(delegate { UpdateDeleteSaveListeners(saveName); });
        }
    }

}
