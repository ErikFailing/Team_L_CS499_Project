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
    
    
    
    
    
    
    
    // House Planner GUI Methods



}
