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
    public bool placing;
    public bool deleting;
    public string objectToPlace;
    public Color validColor;
    public Color invalidColor;

    public Dictionary<Vector4, GameObject> DisplayedRooms;
    public Dictionary<Vector4, GameObject> DisplayedWalls;
    public Dictionary<Vector4, GameObject> DisplayedTabletops;
    public Dictionary<Vector4, GameObject> DisplayedTablelegs;
    public Dictionary<Vector4, GameObject> DisplayedChests;
    public Dictionary<Vector4, GameObject> DisplayedVacuums;
    public Dictionary<Vector2, GameObject> DisplayedPoints;



    // Start is called before the first frame update
    void Start()
    {
        readyToSelect = false;
        selecting = false;
        placing = false;
        deleting = false;
        DisplayedRooms = new Dictionary<Vector4, GameObject>();
        DisplayedWalls = new Dictionary<Vector4, GameObject>();
        DisplayedTabletops = new Dictionary<Vector4, GameObject>();
        DisplayedTablelegs = new Dictionary<Vector4, GameObject>();
        DisplayedChests = new Dictionary<Vector4, GameObject>();
        DisplayedVacuums = new Dictionary<Vector4, GameObject>();
        DisplayedPoints = new Dictionary<Vector2, GameObject>();
        point1 = new Vector3();
        point2 = new Vector3();
    }

    void Update()
    {
        if (deleting)
        {
            // Mouse is NOT over GUI element
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                // Cast hit ray to world
                Vector3 p = Utility.RoundToNearest(Utility.CastRayToWorld(0.5f, Utility.GetWorldBounds()), 2);
                Vector4 objectRect = Ref.I.Model.FindObjectRect(new Vector2(p.x, p.z));
                if (objectRect != new Vector4(0,0,0,0))
                {
                    Ref.I.SelectionCube.SetActive(true);
                    if (DisplayedTabletops.ContainsKey(objectRect))
                    {
                        Vector3 pos = DisplayedTabletops[objectRect].transform.position;
                        pos.y = 15;
                        Vector3 scale = DisplayedTabletops[objectRect].transform.localScale * 1.001f;
                        scale.y = 34.1f;
                        Ref.I.SelectionCube.transform.position = pos;
                        Ref.I.SelectionCube.transform.localScale = scale;
                    }
                    else if (DisplayedChests.ContainsKey(objectRect))
                    {
                        Vector3 pos = DisplayedChests[objectRect].transform.position;
                        Vector3 scale = DisplayedChests[objectRect].transform.localScale * 1.001f;
                        Ref.I.SelectionCube.transform.position = pos;
                        Ref.I.SelectionCube.transform.localScale = scale;
                    }
                    else if (DisplayedVacuums.ContainsKey(objectRect))
                    {
                        Vector3 pos = DisplayedVacuums[objectRect].transform.position;
                        pos.y = 2;
                        Vector3 scale = DisplayedVacuums[objectRect].transform.localScale * 1.001f;
                        scale.y = 3;
                        Ref.I.SelectionCube.transform.position = pos;
                        Ref.I.SelectionCube.transform.localScale = scale;
                    }
                    else if (DisplayedRooms.ContainsKey(objectRect))
                    {
                        Vector3 pos = DisplayedRooms[objectRect].transform.position;
                        pos.y = 54;
                        Vector3 scale = DisplayedRooms[objectRect].transform.localScale * 1.001f;
                        scale.y = 109.1f;
                        Ref.I.SelectionCube.transform.position = pos;
                        Ref.I.SelectionCube.transform.localScale = scale;
                    }
                }
                else Ref.I.SelectionCube.SetActive(false);
            }
            else Ref.I.SelectionCube.SetActive(false);
            // Mouse down
            if (Input.GetMouseButtonDown(0))
            {
                // Mouse is NOT over GUI element
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    Vector3 p = Utility.RoundToNearest(Utility.CastRayToWorld(0.5f, Utility.GetWorldBounds()), 2);
                    Ref.I.Model.DeleteObject(new Vector2(p.x, p.z));
                }
                deleting = false;
                Ref.I.SelectionCube.SetActive(false);
            }

        }
        
        
        
        //Need a different kind of placement for vacuum
        if (placing)
        {
            // Mouse is NOT over GUI element
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (objectToPlace == "Vacuum")
                {
                    point1 = Utility.RoundToNearest(Utility.CastRayToWorld(0.5f, Utility.GetWorldBounds()), 2);
                    ColorPlacementSelection(Ref.I.Model.NewVacuumIsValid(new Vector4(point1.x - 6.4f, point1.z + 6.4f, point1.x + 6.4f, point1.z - 6.4f)));
                    PositionPlacementSelection(1);
                }
                Ref.I.PlacementSelection.SetActive(true);
            }
            else
            {
                Ref.I.PlacementSelection.SetActive(false);
            }
            // Mouse down
            if (Input.GetMouseButtonDown(0))
            {
                // Mouse is NOT over GUI element
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    if (objectToPlace == "Vacuum")
                    {
                        Ref.I.Model.AddVacuumIfValid(new Vector4(point1.x-6.4f, point1.z+6.4f, point1.x+6.4f, point1.z-6.4f));
                    }
                }
                placing = false;
                Ref.I.PlacementSelection.SetActive(false);
            }
        }

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
                        point1 = Utility.RoundToNearest(Utility.CastRayToWorld(0.5f, Utility.GetWorldBounds()), 2);
                        point2 = Utility.RoundToNearestAwayFromZero(Utility.CastRayToWorld(0.5f, Utility.GetWorldBounds()), 2, point1);
                        PositionSelectionCube(15, 32);
                        ColorSelectionCube(Ref.I.Model.NewTableIsValid(new Vector4(point1.x, point1.z, point2.x, point2.z)));
                    }
                    else if (objectToPlace == "Chest")
                    {
                        point1 = Utility.RoundToNearest(Utility.CastRayToWorld(0.5f, Utility.GetWorldBounds()), 2);
                        point2 = Utility.RoundToNearestAwayFromZero(Utility.CastRayToWorld(0.5f, Utility.GetWorldBounds()), 2, point1);
                        PositionSelectionCube(20, 40);
                        ColorSelectionCube(Ref.I.Model.NewChestIsValid(new Vector4(point1.x, point1.z, point2.x, point2.z)));
                    }
                    else if (objectToPlace == "Door")
                    {
                        point1 = Utility.RoundToNearest(Utility.CastRayToWorld(0.5f, Utility.GetWorldBounds()), 2);
                        point2 = Utility.RoundToNearestAwayFromZero(Utility.CastRayToWorld(0.5f, Utility.GetWorldBounds()), 2, point1);
                        PositionSelectionCube(54, 109.01f);
                        ColorSelectionCube(Ref.I.Model.NewDoorIsValid(new Vector4(point1.x, point1.z, point2.x, point2.z)));
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
                else if (objectToPlace == "Table")
                {
                    point2 = Utility.RoundToNearestAwayFromZero(Utility.CastRayToWorld(0.5f, Utility.GetWorldBounds()), 2, point1);
                    ColorSelectionCube(Ref.I.Model.NewTableIsValid(new Vector4(point1.x, point1.z, point2.x, point2.z)));
                    PositionSelectionCube(15, 34);
                }
                else if (objectToPlace == "Chest")
                {
                    point2 = Utility.RoundToNearestAwayFromZero(Utility.CastRayToWorld(0.5f, Utility.GetWorldBounds()), 2, point1);
                    ColorSelectionCube(Ref.I.Model.NewChestIsValid(new Vector4(point1.x, point1.z, point2.x, point2.z)));
                    PositionSelectionCube(20, 40);
                }
                else if (objectToPlace == "Door")
                {
                    point2 = Utility.RoundToNearestAwayFromZero(Utility.CastRayToWorld(0.5f, Utility.GetWorldBounds()), 2, point1);
                    ColorSelectionCube(Ref.I.Model.NewDoorIsValid(new Vector4(point1.x, point1.z, point2.x, point2.z)));
                    PositionSelectionCube(54, 109.01f);
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
                else if (objectToPlace == "Chest")
                {
                    Ref.I.Model.AddChestIfValid(new Vector4(point1.x, point1.z, point2.x, point2.z));
                }
                else if (objectToPlace == "Door")
                {
                    Ref.I.Model.AddDoorIfValid(new Vector4(point1.x, point1.z, point2.x, point2.z));
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
    public void ColorPlacementSelection(bool valid)
    {
        if (valid)
        {
            Ref.I.PlacementSelection.GetComponent<Renderer>().material.color = validColor;
        }
        else
        {
            Ref.I.PlacementSelection.GetComponent<Renderer>().material.color = invalidColor;
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
        GameObject go = Instantiate(Ref.I.TablelegPrefab, pos, new Quaternion(), Ref.I.Tablelegs.transform);
        go.transform.localScale = scale;
        DisplayedTablelegs.Add(leg, go);
    }
    public void DisplayNewChest(Vector4 Chest)
    {
        Vector3 pos = Utility.PosFromRect(Chest);
        pos.y = 20f;
        Vector3 scale = Utility.ScaleFromRect(Chest);
        scale.y = 40;
        // Instantiate, position and scale room floor
        GameObject go = Instantiate(Ref.I.ChestPrefab, pos, new Quaternion(), Ref.I.Chests.transform);
        go.transform.localScale = scale;
        DisplayedChests.Add(Chest, go);
    }
    public void DisplayVacuum(Vector4 vacuum)
    {
        Vector3 pos = Utility.PosFromRect(vacuum);
        pos.y = 1f;
        // Instantiate, position and scale room floor
        GameObject go = Instantiate(Ref.I.VacuumPrefab, pos, new Quaternion(), Ref.I.Vacuums.transform);
        DisplayedVacuums.Add(vacuum, go);
    }
    public void DisplayNewPoint(Vector2 Point)
    {
        Vector3 pos = new Vector3(Point.x, 0.75f, Point.y);
        // Instantiate, position
        GameObject go = Instantiate(Ref.I.PointPrefab, pos, new Quaternion(), Ref.I.Points.transform);
        DisplayedChests.Add(Point, go);
    }
    public void DisplayNewPoints(Dictionary<Vector2, float> points)
    {
        foreach (Vector2 point in points.Keys)
        {
            DisplayNewPoint(point);
        }
    }

    public void RemoveDisplayedFloor(Vector4 floor)
    {
        Destroy(DisplayedRooms[floor]);
        DisplayedRooms.Remove(floor);
    }
    public void RemoveDisplayedChest(Vector4 chest)
    {
        Destroy(DisplayedChests[chest]);
        DisplayedChests.Remove(chest);
    }
    public void RemoveDisplayedTabletop(Vector4 table)
    {
        Destroy(DisplayedTabletops[table]);
        DisplayedTabletops.Remove(table);
    }
    public void RemoveDisplayedTableleg(Vector4 leg)
    {
        Destroy(DisplayedTablelegs[leg]);
        DisplayedTablelegs.Remove(leg);
    }
    public void RemoveDisplayedWall(Vector4 wall)
    {
        Destroy(DisplayedWalls[wall]);
        DisplayedWalls.Remove(wall);
    }
    public void RemoveDisplayedVacuum(Vector4 vacuum)
    {
        Destroy(DisplayedVacuums[vacuum]);
        DisplayedVacuums.Remove(vacuum);
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
    public void PositionPlacementSelection(float y)
    {
        Vector3 pos = point1;
        pos.y = y;
        // Position selection cube between the two selection points
        Ref.I.PlacementSelection.transform.position = pos;
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
