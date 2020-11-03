using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUI : MonoBehaviour
{
    //
    
    
    
    
    
    
    
    
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
    }

    public void QuitButtonClick()
    {
        Application.Quit();
    }


    // House Planner GUI Methods
    

    public void DrawRoomButtonClick()
    {

    }

    public void FloorTypeDropdownValueChanged()
    {

    }

    public void PlaceTableButtonClick()
    {

    }

    public void PlaceChestButtonClick()
    {

    }

    public void PlaceVacuumButtonClick()
    {

    }

    public void FinalizeDesignButtonClick()
    {
        Ref.I.HousePlannerMenu.SetActive(false);
        Ref.I.HouseSimulationMenu.SetActive(true);
    }


    // House Simulation GUI Methods

    public void OneSpeedButtonClick()
    {
        Ref.I.OneSpeedButton.GetComponent<Button>().interactable = false;
        Ref.I.FiftySpeedButton.GetComponent<Button>().interactable = true;
        Ref.I.HundredSpeedButton.GetComponent<Button>().interactable = true;
    }

    public void FiftySpeedButtonClick()
    {
        Ref.I.OneSpeedButton.GetComponent<Button>().interactable = true;
        Ref.I.FiftySpeedButton.GetComponent<Button>().interactable = false;
        Ref.I.HundredSpeedButton.GetComponent<Button>().interactable = true;
    }

    public void HundredSpeedButtonClick()
    {
        Ref.I.OneSpeedButton.GetComponent<Button>().interactable = true;
        Ref.I.FiftySpeedButton.GetComponent<Button>().interactable = true;
        Ref.I.HundredSpeedButton.GetComponent<Button>().interactable = false;
    }


    // Load Simulation GUI Methods
    public void LoadSimulation()
    {
        Ref.I.LoadSimulationMenu.SetActive(false);
        Ref.I.HouseSimulationMenu.SetActive(true);
    }
}
