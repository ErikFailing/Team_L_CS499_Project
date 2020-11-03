using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    [Header("Refs set before runtime")]
    public GameObject Camera;
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
}