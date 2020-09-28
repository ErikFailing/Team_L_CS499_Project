using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public bool TwoDMode;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwapCameraViewTo2D()
    {
        // Set position
        transform.position = new Vector3(-1.4f, 10, 0);
        // Set rotation
        transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));

        // Set camera view settings to be 2D
        GetComponent<Camera>().orthographic = true;
    }

    public void SwapCameraViewTo3D()
    {
        // Set position
        transform.position = new Vector3(6, 10, -9);
        // Set rotation
        transform.rotation = Quaternion.Euler(new Vector3(45, -45, 0));


        // Set camera view settings to be 3D
        GetComponent<Camera>().orthographic = false;
    }

}
