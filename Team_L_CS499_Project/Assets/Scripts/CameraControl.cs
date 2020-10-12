﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public bool TwoDMode;
    public float CamSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        TwoDMode = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Horizontal movement
        if (Input.GetAxis("Horizontal") > 0) { transform.position += transform.right * CamSpeed; }
        else if (Input.GetAxis("Horizontal") < 0) { transform.position -= transform.right * CamSpeed; }
        



        if (TwoDMode)
        {
            // 2-D Vertical movement
            if (Input.GetAxis("Vertical") > 0) { transform.position += transform.up * CamSpeed; }
            else if (Input.GetAxis("Vertical") < 0) { transform.position -= transform.up * CamSpeed; }

            // Zoom
            if (Input.GetAxis("Zoom") > 0)
            {
                GetComponent<Camera>().orthographicSize =
                    Mathf.Clamp(GetComponent<Camera>().orthographicSize - CamSpeed * 3, 1, 50);
            }
            else if (Input.GetAxis("Zoom") < 0) 
            { 
                GetComponent<Camera>().orthographicSize = 
                    Mathf.Clamp(GetComponent<Camera>().orthographicSize + CamSpeed * 3, 1, 50); 
            }
            
        }
        else 
        {
            // 3-D Zoom/Vertical movement
            if (Input.GetAxis("Vertical") > 0) { transform.position += transform.forward * CamSpeed; }
            else if (Input.GetAxis("Vertical") < 0) { transform.position -= transform.forward * CamSpeed; }

            // Rotation
            if (Input.GetAxis("Rotate") > 0) 
            {
                Cursor.lockState = CursorLockMode.Locked;
                transform.Rotate(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);
                transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }

        //Ensure camera never goes below a certain y level
        transform.position = new Vector3(transform.position.x, 
            Mathf.Clamp(transform.position.y, 1, 100), transform.position.z);

    }

    public void SwapCameraViewTo2D()
    {
        // Set position
        transform.position = new Vector3(-1.4f, 10, 0);
        // Set rotation
        transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));

        // Set camera view settings to be 2D
        GetComponent<Camera>().orthographic = true;

        TwoDMode = true;
    }

    public void SwapCameraViewTo3D()
    {
        // Set position
        transform.position = new Vector3(6, 10, -9);
        // Set rotation
        transform.rotation = Quaternion.Euler(new Vector3(45, -45, 0));


        // Set camera view settings to be 3D
        GetComponent<Camera>().orthographic = false;

        TwoDMode = false;
    }

}
