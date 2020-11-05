using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModelVisuals : MonoBehaviour
{
    public Vector3 point1;
    public Vector3 point2;
    public bool readyToSelect;
    public bool selecting;
    public Color validColor;
    public Color invalidColor;

    public Dictionary<Vector4, GameObject> DisplayedRooms;

    // Start is called before the first frame update
    void Start()
    {
        readyToSelect = false;
        selecting = false;
        DisplayedRooms = new Dictionary<Vector4, GameObject>();
        point1 = new Vector3();
        point2 = new Vector3();

    }

    void Update()
    {
        // Ready to Select
        if (readyToSelect)
        {
            // Mouse down
            if (Input.GetMouseButtonDown(0))
            {
                // Mouse is NOT over GUI element
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    Ref.I.SelectionCube.SetActive(true);
                    point1 = Utility.RoundToNearest(Utility.CastRayToWorld(0, Utility.GetWorldBounds()), 24);
                    point2 = Utility.RoundToNearestAwayFromZero(Utility.CastRayToWorld(0, Utility.GetWorldBounds()), 24, point1);
                    PositionSelectionCube();
                    ColorSelectionCube(Ref.I.Model.NewRoomIsValid(new Vector4(point1.x, point1.z, point2.x, point2.z)));
                    selecting = true;
                }
                readyToSelect = false;
            }
        }

        // Selecting
        if (selecting)
        {
            //Mouse held down
            if (Input.GetMouseButton(0))
            {
                point2 = Utility.RoundToNearestAwayFromZero(Utility.CastRayToWorld(0, Utility.GetWorldBounds()), 24, point1);
                PositionSelectionCube(); 
                ColorSelectionCube(Ref.I.Model.NewRoomIsValid(new Vector4(point1.x, point1.z, point2.x, point2.z)));
            }

            //Mouse up
            if (Input.GetMouseButtonUp(0))
            {
                Ref.I.SelectionCube.SetActive(false);
                selecting = false;
                if (Ref.I.Model.NewRoomIsValid(new Vector4(point1.x, point1.z, point2.x, point2.z)))
                {
                    Ref.I.Model.AddRoom(new Vector4(point1.x, point1.z, point2.x, point2.z));
                }
            }
        }
    }

    public void ColorSelectionCube(bool valid)
    {
        if (valid)
        {
            Ref.I.SelectionCube.GetComponent<Renderer>().material.color = validColor;
        }
        else
        {
            Ref.I.SelectionCube.GetComponent<Renderer>().material.color = invalidColor;
        }
    }


    public void DisplayNewFloor(Vector4 floor)
    {
        // Instantiate, position and scale room floor
        GameObject go = Instantiate(Ref.I.FloorPrefab, Utility.PosFromRect(floor), new Quaternion(), Ref.I.Rooms.transform);
        go.transform.localScale = Utility.ScaleFromRect(floor);
        // Set correct floor type
        DisplayFloorType(Ref.I.Model.FlooringType);
        // Walls are 4 inchest thick total (2 inches each room), 108 inches tall and the width of the room

        DisplayedRooms.Add(floor, go);
    }

    public void DestroyDisplayedRoom(Vector4 room)
    {
        Destroy(DisplayedRooms[room]);
        // Update Walls accordingly?

        DisplayedRooms.Remove(room);
        
    }

    public void PositionSelectionCube()
    {
        // Position selection cube between the two selection points
        Ref.I.SelectionCube.transform.position = Utility.PosFromRect(point1, point2);
        // Resize selection cube appropritely
        Ref.I.SelectionCube.transform.localScale = Utility.ScaleFromRect(point1, point2);
    }

    public void DisplayFloorType(string type)
    {
        Material newMaterial = Ref.I.Hardwood;
        if (type == "Hardwood")
        {
            newMaterial = Ref.I.Hardwood;
        }
        else if (type == "Loop Pile Carpet")
        {
            newMaterial = Ref.I.LoopPileCarpet;
        }
        else if (type == "Cut Pile Carpet")
        {
            newMaterial = Ref.I.CutPileCarpet;
        }
        else if (type == "Frieze-cut Pile Carpet")
        {
            newMaterial = Ref.I.FriezeCutPileCarpet;
        }

        foreach (MeshRenderer floor in Ref.I.Rooms.transform.GetComponentsInChildren<MeshRenderer>())
        {
            floor.material = newMaterial;
        }
    }
}
