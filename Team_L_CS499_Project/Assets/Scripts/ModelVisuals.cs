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
    public string objectToPlace;
    public Color validColor;
    public Color invalidColor;

    public Dictionary<Vector4, GameObject> DisplayedRooms;
    public Dictionary<Vector4, GameObject> DisplayedWalls;
    public Dictionary<Vector4, GameObject> DisplayedTabletops;
    public Dictionary<Vector4, GameObject> DisplayedTablelegs;
    public Dictionary<Vector4, GameObject> DisplayedChests;



    // Start is called before the first frame update
    void Start()
    {
        readyToSelect = false;
        selecting = false;
        DisplayedRooms = new Dictionary<Vector4, GameObject>();
        DisplayedWalls = new Dictionary<Vector4, GameObject>();
        DisplayedTabletops = new Dictionary<Vector4, GameObject>();
        DisplayedTablelegs = new Dictionary<Vector4, GameObject>();
        DisplayedChests = new Dictionary<Vector4, GameObject>();
        point1 = new Vector3();
        point2 = new Vector3();

    }

    void Update()
    {
        //Need a different kind of placement for vacuum

        // Ready to Select
        // Selection is placement for doors, tables, rooms and chests
        if (readyToSelect)
        {
            // Mouse down
            if (Input.GetMouseButtonDown(0))
            {
                // Mouse is NOT over GUI element
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    Ref.I.SelectionCube.SetActive(true);
                    if (objectToPlace == "Room")
                    {
                        point1 = Utility.RoundToNearest(Utility.CastRayToWorld(0, Utility.GetWorldBounds()), 24);
                        point2 = Utility.RoundToNearestAwayFromZero(Utility.CastRayToWorld(0, Utility.GetWorldBounds()), 24, point1);
                        PositionSelectionCube(54, 109);
                        ColorSelectionCube(Ref.I.Model.NewRoomIsValid(new Vector4(point1.x, point1.z, point2.x, point2.z)));
                    }
                    else if (objectToPlace == "Table")
                    {
                        point1 = Utility.RoundToNearest(Utility.CastRayToWorld(0, Utility.GetWorldBounds()), 2);
                        point2 = Utility.RoundToNearestAwayFromZero(Utility.CastRayToWorld(0, Utility.GetWorldBounds()), 2, point1);
                        PositionSelectionCube(15, 32);
                        ColorSelectionCube(Ref.I.Model.NewTableIsValid(new Vector4(point1.x, point1.z, point2.x, point2.z)));
                    }
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
                if (objectToPlace == "Room")
                {
                    point2 = Utility.RoundToNearestAwayFromZero(Utility.CastRayToWorld(0, Utility.GetWorldBounds()), 24, point1);
                    ColorSelectionCube(Ref.I.Model.NewRoomIsValid(new Vector4(point1.x, point1.z, point2.x, point2.z)));
                    PositionSelectionCube(54, 109);
                }
                if (objectToPlace == "Table")
                {
                    point2 = Utility.RoundToNearestAwayFromZero(Utility.CastRayToWorld(0, Utility.GetWorldBounds()), 2, point1);
                    ColorSelectionCube(Ref.I.Model.NewTableIsValid(new Vector4(point1.x, point1.z, point2.x, point2.z)));
                    PositionSelectionCube(15, 34);
                }
                
            }

            //Mouse up (Selection finished)
            if (Input.GetMouseButtonUp(0))
            {
                Ref.I.SelectionCube.SetActive(false);
                selecting = false;
                if (objectToPlace == "Room") 
                {
                    Ref.I.Model.AddRoomIfValid(new Vector4(point1.x, point1.z, point2.x, point2.z));
                }
                else if (objectToPlace == "Table")
                {
                    Ref.I.Model.AddTableIfValid(new Vector4(point1.x, point1.z, point2.x, point2.z));
                }
                /*else if (objectToPlace == "Chest")
                {
                    Ref.I.Model.AddChestIfValid(new Vector4(point1.x, point1.z, point2.x, point2.z));
                }
                else if (objectToPlace == "Door")
                {
                    Ref.I.Model.AddDoorIfValid(new Vector4(point1.x, point1.z, point2.x, point2.z));
                }*/
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
        GameObject go = Instantiate(Ref.I.FloorPrefab, Utility.PosFromRect(floor), new Quaternion(), Ref.I.Floors.transform);
        go.transform.localScale = Utility.ScaleFromRect(floor);
        // Set correct floor type
        DisplayFloorType(Ref.I.Model.FlooringType);

        DisplayedRooms.Add(floor, go);
    }

    public void DisplayNewWall(Vector4 wall)
    {
        Vector3 pos = Utility.PosFromRect(wall);
        pos.y = 54.5f;
        Vector3 scale = Utility.ScaleFromRect(wall);
        scale.y = 108; //108 inches tall
        // Instantiate, position and scale room floor
        GameObject go = Instantiate(Ref.I.WallPrefab, pos, new Quaternion(), Ref.I.Walls.transform);
        go.transform.localScale = scale;

        DisplayedWalls.Add(wall, go);
    }
    public void DisplayNewTabletop(Vector4 table)
    {
        Vector3 pos = Utility.PosFromRect(table);
        pos.y = 30.5f;
        Vector3 scale = Utility.ScaleFromRect(table);
        scale.y = 3;
        // Instantiate, position and scale room floor
        GameObject go = Instantiate(Ref.I.TablePrefab, pos, new Quaternion(), Ref.I.Tabletops.transform);
        go.transform.localScale = scale;
        DisplayedTabletops.Add(table, go);
    }
    public void DisplayNewTableleg(Vector4 leg)
    {
        Vector3 pos = Utility.PosFromRect(leg);
        pos.y = 15f;
        Vector3 scale = Utility.ScaleFromRect(leg);
        scale.y = 30;
        // Instantiate, position and scale room floor
        GameObject go = Instantiate(Ref.I.TablePrefab, pos, new Quaternion(), Ref.I.Tablelegs.transform);
        go.transform.localScale = scale;
        DisplayedTablelegs.Add(leg, go);
    }


    public void DestroyDisplayedRoom(Vector4 room)
    {
        Destroy(DisplayedRooms[room]);
        // Update Walls accordingly?

        DisplayedRooms.Remove(room);
        
    }

    public void PositionSelectionCube(float y, float height)
    {
        Vector3 pos = Utility.PosFromRect(point1, point2);
        pos.y = y;
        Vector3 scale = Utility.ScaleFromRect(point1, point2);
        scale.y = height;
        // Position selection cube between the two selection points
        Ref.I.SelectionCube.transform.position = pos;
        // Resize selection cube appropritely
        Ref.I.SelectionCube.transform.localScale = scale;
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

        foreach (MeshRenderer floor in Ref.I.Floors.transform.GetComponentsInChildren<MeshRenderer>())
        {
            floor.material = newMaterial;
        }
    }
}
