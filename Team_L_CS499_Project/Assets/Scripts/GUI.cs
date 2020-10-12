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
        // Can be used by any menu's back button
        if (Ref.I.HousePlannerMenu.activeInHierarchy)
        {
            Ref.I.HousePlannerMenu.SetActive(false);
            Ref.I.MainMenu.SetActive(true);
        }
    }
    
    public void HelpButtonClick()
    {
        // Can be used by any menu's help button
        if (Ref.I.HousePlannerMenu.activeInHierarchy)
        {
            if (Ref.I.HousePlannerHelpOverlay.activeInHierarchy)
            {
                Ref.I.HousePlannerHelpOverlay.SetActive(false);
            }
            else
            {
                Ref.I.HousePlannerHelpOverlay.SetActive(true);
            }
        }
        else if (Ref.I.MainMenu.activeInHierarchy)
        {
            if (Ref.I.MainHelpOverlay.activeInHierarchy)
            {
                Ref.I.MainHelpOverlay.SetActive(false);
            }
            else
            {
                Ref.I.MainHelpOverlay.SetActive(true);
            }
        }
    }




    public void TwoDButtonClick()
    {
        Ref.I.TwoDButton.GetComponent<Button>().interactable = false;
        Ref.I.ThreeDButton.GetComponent<Button>().interactable = true;
        Ref.I.Camera.GetComponent<CameraControl>().SwapCameraViewTo2D();
    }

    public void ThreeDButtonClick()
    {
        Ref.I.TwoDButton.GetComponent<Button>().interactable = true;
        Ref.I.ThreeDButton.GetComponent<Button>().interactable = false;
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

    }




}
