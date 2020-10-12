using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousePlanner : MonoBehaviour
{
    public Vector3 point1;
    public Vector3 point2;
    public GameObject selectionCube;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnMouseDown()
    {
        Debug.Log("Mouse down on ground");
        selectionCube.SetActive(true);
        point1 = CastRayToWorld();
        point2 = CastRayToWorld();
        PositionSelectionCube();
        
    }

    void OnMouseDrag()
    {
        Debug.Log("Mouse drag on ground");
        point2 = CastRayToWorld();
        PositionSelectionCube();
    }

    void OnMouseUp()
    {
        Debug.Log("Mouse up");
        selectionCube.SetActive(false);
    }

    void OnMouseUpAsButton()
    {
        Debug.Log("Mouse up on ground");

    }

    public void PositionSelectionCube()
    {
        // Position selection cube between the two selection points
        Vector3 center = Vector3.MoveTowards(point1, point2, Vector3.Distance(point1, point2) * 0.5f);
        selectionCube.transform.position = center;
        // Resize selection cube appropritely
        selectionCube.transform.localScale = 
            new Vector3(Mathf.Abs(point1.x - point2.x), 1, Mathf.Abs(point1.z - point2.z));
    }

    public Vector3 CastRayToWorld()
    {
        // Create a ray from camera position to mouse point
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Raycast following ray direction
        RaycastHit hitinfo;
        Physics.Raycast(ray.origin, ray.direction, out hitinfo, 1000);
        // If raycast doesn't hit anything, take a point 1000 distance away
        if (hitinfo.point == new Vector3(0,0,0))
        {
            hitinfo.point = ray.origin + (ray.direction * 1000);
        }
        // Return a point clamped to the bounds of the ground
        return new Vector3(Mathf.Clamp(hitinfo.point.x, -transform.localScale.x * 0.5f, transform.localScale.x * 0.5f), 
            0, Mathf.Clamp(hitinfo.point.z, -transform.localScale.z * 0.5f, transform.localScale.z * 0.5f));
    }
}
