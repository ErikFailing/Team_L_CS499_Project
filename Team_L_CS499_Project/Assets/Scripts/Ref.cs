using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ref, short for References, is a script that stores references to commonly accessed scripts and objects
/// </summary>
public class Ref : MonoBehaviour
{
    /// <summary>
    /// The single instance of the References script
    /// </summary>
    public static Ref I { get; private set; }
    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
    }


    [Header("GUI")]
    public GUI GUI;
    public GameObject[] TwoDButtons;
    public GameObject[] ThreeDButtons;
    public GameObject MainMenu;
    public GameObject MainHelpOverlay;
    public GameObject HousePlannerMenu;
    public GameObject HousePlannerHelpOverlay;
    public GameObject LoadSimulationMenu;
    public GameObject LoadSimulationHelpOverlay;
    public GameObject HouseSimulationMenu;
    public GameObject HouseSimulationHelpOverlay;
    public GameObject OneSpeedButton;
    public GameObject FiftySpeedButton;
    public GameObject HundredSpeedButton;
    public GameObject SquareFeetText;
    public GameObject UncleanableAreaText;
    public Transform LoadSimulationViewport;
    public GameObject LoadSaveButton;
    public GameObject DeleteSaveButton;
    public GameObject PlanNameInput;
    public GameObject SimFloorTypeText;
    public GameObject SimCoverageText;
    public GameObject SimDurationText;
    public GameObject SimRemainingText;
    public GameObject SummaryOverlay;
    public GameObject SummaryTopLeftText;
    public GameObject SummaryTopRightText;
    public GameObject SummaryBottomLeftText;
    public GameObject SummaryBottomRightText;
    public GameObject PlayButton;
    public GameObject PauseButton;
    public GameObject StopButton;
    public GameObject RunDropdown;
    public GameObject PathingDropdown;
    public GameObject FlooringDropdown;

    [Header("Prefabs")]
    public GameObject FloorPrefab;
    public GameObject WallPrefab;
    public GameObject TablePrefab;
    public GameObject TablelegPrefab;
    public GameObject ChestPrefab;
    public GameObject VacuumPrefab;
    public GameObject PointPrefab;
    public GameObject saveFileMenuPrefab;

    [Header("Materials")]
    public Material Hardwood;
    public Material LoopPileCarpet;
    public Material CutPileCarpet;
    public Material FriezeCutPileCarpet;

    [Header("Misc.")]
    public GameObject Camera;
    public GameObject SelectionCube;
    public GameObject PlacementSelection;
    public GameObject Ground;
    public GameObject Floors;
    public GameObject Walls;
    public GameObject Tabletops;
    public GameObject Tablelegs;
    public GameObject Chests;
    public GameObject Vacuums;
    public GameObject Points;
    public Model Model;
    public ModelVisuals ModelVisuals;
    public Simulation Simulation;
    public GameObject Vacuum;
}