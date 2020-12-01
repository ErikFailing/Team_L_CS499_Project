using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    public int totalSquareFeet;
    public int uncleanableArea;
    public string FlooringType;
    public List<Vector4> Rooms;
    public List<Vector4> InnerWalls;
    public List<Vector4> Tables;
    public List<Vector4> TableLegs;
    public List<Vector4> Chests;
    public List<Vector4> Doors;
    public List<Vector4> VacuumStation;
    public Dictionary<Vector2, float> points;
    public List<Vector2> RandomPath;
    public List<Vector2> SpiralPath;
    public List<Vector2> SnakingPath;
    public List<Vector2> WallfollowPath;






    void Start()
    {
        points = new Dictionary<Vector2, float>();
    }

    public void CalculatePaths()
    {
        //Random
        //Spiral
        //Snaking
        //Wall-follow
        // Vacuum moves 27000 inches before running out of energy. 150 minutes, 3 inches a second
        // Path data structure....
    }
    public void CalculateRandomPath()
    {
        Vector3 direction = new Vector3(1, 0, 1); // y value should always be zero
        Vector3 origin = new Vector3(0, 1, 0); // y value should always be one
        Ray ray = new Ray(origin, direction);
        RaycastHit hitInfo;
        bool hit = Physics.SphereCast(ray, 12.8f, out hitInfo);

    }

    

    public bool VerifyHousePlan(out string errorMsg)
    {
        // House must be between 200 and 8,000 square feet
        if (totalSquareFeet < 200 || totalSquareFeet > 8000)
        {
            errorMsg = "ERROR: House must have a square footage in the range of [200, 8000]. Current square footage is " + totalSquareFeet;
            return false;
        }
        // All rooms must have atleast one door
        CalculatePoints();
        // If each room has a point along its edge, return true
        // Else, a room lacks a door or is otherwise unenterable, return false
        foreach (Vector4 room in Rooms)
        {
            bool containsEntry = false;
            // Top and bottom
            for (int x = (int)room.x; x <= room.z; x++)
            {
                if (points.ContainsKey(new Vector2(x, room.y)))
                {
                    containsEntry = true;
                    break;
                }
                if (points.ContainsKey(new Vector2(x, room.w)))
                {
                    containsEntry = true;
                    break;
                }
            }
            // Top and bottom
            if (!containsEntry)
            {
                for (int y = (int)room.y; y >= room.w; y--)
                {
                    if (points.ContainsKey(new Vector2(room.x, y)))
                    {
                        containsEntry = true;
                        break;
                    }
                    if (points.ContainsKey(new Vector2(room.z, y)))
                    {
                        containsEntry = true;
                        break;
                    }
                }
            }
            if (!containsEntry)
            {
                errorMsg = "ERROR: A room does not have a traversable entry (door)";
                return false;
            }
            
        }
        //Ref.I.ModelVisuals.DisplayNewPoints(points);
        errorMsg = "";
        return true;
    }
    
    
    public void CalculatePoints()
    {
        foreach (Vector4 room in Rooms)
        {
            // Add points that are contained in rooms
            for (int x = (int)room.x; x <= room.z; x++)
            {
                for (int y = (int)room.y; y >= room.w; y--)
                {
                    Vector2 point = new Vector2(x, y);
                    // Only add if it isn't already added
                    if (!points.ContainsKey(point))
                    {
                        // Make sure point isn't in an object
                        bool traversable = true;
                        foreach (Vector4 rect in InnerWalls)
                        {
                            if (RectangleContainsPoint(rect, point)) { traversable = false; break; }
                        }
                        foreach (Vector4 rect in Chests)
                        {
                            if (!traversable || RectangleContainsPoint(rect, point)) { traversable = false; break; }
                        }
                        foreach (Vector4 rect in VacuumStation)
                        {
                            if (!traversable || RectangleContainsPoint(rect, point)) { traversable = false; break; }
                        }
                        foreach (Vector4 rect in TableLegs)
                        {
                            if (!traversable || RectangleContainsPoint(rect, point)) { traversable = false; break; }
                        }
                        if (traversable)
                        {
                            points.Add(point, 0);
                        }
                        
                    }
                    
                }
            }
        }
    }
    
    
    
    
    public void CalculateTotalSquareFeet()
    {
        totalSquareFeet = 0;
        foreach (Vector4 room in Rooms)
        {
            totalSquareFeet += Mathf.RoundToInt(Mathf.Abs(((room.x - room.z)/12) * ((room.y - room.w)/12)) );
        }
        // Update GUI
        Ref.I.GUI.UpdateSquareFootage(totalSquareFeet);
    }

    public void CalculateUncleanableArea()
    {
        float area = 0;
        List<Vector4> rects = new List<Vector4>(Chests);
        rects.AddRange(TableLegs);
        rects.AddRange(VacuumStation);
        foreach (Vector4 rect in rects)
        {
            float width = Mathf.Abs(rect.x - rect.z);
            float height = Mathf.Abs(rect.y - rect.w);
            area += width * height;
        }
        //Walls
        int overlappingWalls = 0;
        foreach (Vector4 rect in InnerWalls)
        {
            float width = Mathf.Abs(rect.x - rect.z);
            float height = Mathf.Abs(rect.y - rect.w);
            area += width * height;
            overlappingWalls += -1;
            foreach (Vector4 wall in InnerWalls)
            {
                if (RectanglesOverlap(rect, wall)) overlappingWalls++;
            }
        }
        area -= (overlappingWalls / 2) * 4;
        uncleanableArea = Mathf.RoundToInt(area / 144);
        Ref.I.GUI.UpdateUncleanableArea(uncleanableArea);
    }

    public void ChangeFloorType(string type)
    {
        FlooringType = type;
        Ref.I.ModelVisuals.DisplayFloorType(type);
    }

    public void AddWall(Vector4 wall)
    {
        InnerWalls.Add(wall);
        Ref.I.ModelVisuals.DisplayNewWall(wall);
        CalculateUncleanableArea();
    }
    
    public void AddRoomIfValid(Vector4 room)
    {
        // Correct Point order
        room = Utility.CorrectRectPointOrder(room);
        // Add if valid
        if (NewRoomIsValid(room))
        {
            Rooms.Add(room);
            // Add InnerWalls
            // North
            AddWall(new Vector4(room.x, room.y, room.z, room.y - 2));
            // South
            AddWall(new Vector4(room.x, room.w + 2, room.z, room.w));
            // West
            AddWall(new Vector4(room.x, room.y, room.x + 2, room.w));
            // East
            AddWall(new Vector4(room.z - 2, room.y, room.z, room.w));

            Ref.I.ModelVisuals.DisplayNewFloor(room);
            CalculateTotalSquareFeet();
            CalculateUncleanableArea();
        }
    }

    public void AddTableLeg(Vector4 leg)
    {
        TableLegs.Add(leg);
        Ref.I.ModelVisuals.DisplayNewTableleg(leg);
    }

    public void AddTableIfValid(Vector4 table)
    {
        // Correct Point order
        table = Utility.CorrectRectPointOrder(table);
        // Add if valid
        if (NewTableIsValid(table))
        {
            Tables.Add(table);
            Ref.I.ModelVisuals.DisplayNewTabletop(table);
            // Add table legs
            // Upper left
            AddTableLeg(new Vector4(table.x+2, table.y-2, table.x+4, table.y-4));
            // Upper right
            AddTableLeg(new Vector4(table.z-4, table.y - 2, table.z-2, table.y - 4));
            // lower right
            AddTableLeg(new Vector4(table.z -4, table.w + 4, table.z -2, table.w+2));
            // lower left
            AddTableLeg(new Vector4(table.x + 2, table.w + 4, table.x + 4, table.w + 2));
            CalculateUncleanableArea();
        }
    }
    public void AddChestIfValid(Vector4 chest)
    {
        // Correct Point order
        chest = Utility.CorrectRectPointOrder(chest);
        // Add if valid
        if (NewChestIsValid(chest))
        {
            Chests.Add(chest);
            Ref.I.ModelVisuals.DisplayNewChest(chest);
            CalculateUncleanableArea();
        }
    }
    public void AddDoorIfValid(Vector4 door)
    {
        // Correct Point order
        door = Utility.CorrectRectPointOrder(door);
        // Add if valid
        if (NewDoorIsValid(door))
        {
            Doors.Add(door);
            // Find the two overlapping walls
            List<Vector4> overlappingWalls = new List<Vector4>();
            foreach (Vector4 wall in InnerWalls)
            {
                if (RectanglesOverlap(wall, door))
                {
                    overlappingWalls.Add(wall);
                }
            }
            // Split overlapping walls to make a door
            foreach (Vector4 wall in overlappingWalls)
            {
                // Remove wall
                RemoveWall(wall);
                // Split wall along sides that don't match
                // Split along Y axises
                if (wall.y != door.y && wall.w != door.w)
                {
                    if (!(wall.x == door.x && wall.y == door.y))
                    {
                        // Create upper wall
                        AddWall(new Vector4(wall.x, wall.y, wall.z, door.y));
                    }
                    if (!(wall.z == door.z && wall.w == door.w))
                    {
                        // Create lower wall
                        AddWall(new Vector4(wall.x, door.w, wall.z, wall.w));
                    }
                }
                else //Split along X axises
                {
                    if (!(wall.x == door.x && wall.y == door.y))
                    {
                        
                        // Create right wall
                        AddWall(new Vector4(door.z, wall.y, wall.z, wall.w));
                    }
                    if (!(wall.z == door.z && wall.w == door.w))
                    {
                        // Create left wall
                        AddWall(new Vector4(wall.x, wall.y, door.x, wall.w));
                    }
                }
            }
            CalculateUncleanableArea();
        }
    }
    public void AddVacuumIfValid(Vector4 vacuum)
    {
        // Correct Point order
        vacuum = Utility.CorrectRectPointOrder(vacuum);
        if (NewVacuumIsValid(vacuum))
        {
            VacuumStation.Add(vacuum);
            Ref.I.ModelVisuals.DisplayVacuum(vacuum);
            CalculateUncleanableArea();
        } 
    }

    

    public bool NewRoomIsValid(Vector4 newRoom)
    {
        // Room is not valid if it has a side with a size of zero
        if (newRoom.x == newRoom.z || newRoom.y == newRoom.w) { return false; }
        
        // NOT first room and NOT connecting to other rooms
        if (Rooms.Count > 0)
        {
            // Room isn't valid if it overlaps any other room
            foreach (Vector4 room in Rooms)
            {
                if (RectanglesOverlap(newRoom, room))
                {
                    return false;
                }
            }
            // Room is valid if it shares an edge and doesn't overlap
            foreach (Vector4 room in Rooms)
            {
                if (RectanglesShareAnEdge(newRoom, room))
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            // Room is valid if is first room.
            return true;
        }
    }
    public bool NewTableIsValid(Vector4 newTable)
    {
        if (!NewHouseObjectIsValid(newTable)) return false;
        else
        {
            // Must be larger than 1 foot by 1 foot
            if (Mathf.Abs(newTable.x - newTable.z) < 12 || Mathf.Abs(newTable.y - newTable.w) < 12) return false;
            // Can't be larger than 12 foot by 12 foot
            if (Mathf.Abs(newTable.x - newTable.z) > 144 || Mathf.Abs(newTable.y - newTable.w) > 144) return false;
            else return true;
        }
    }
    public bool NewChestIsValid(Vector4 newChest)
    {
        if (!NewHouseObjectIsValid(newChest)) return false;
        else
        {
            // Must be larger than 10 inches by 10 inches
            if (Mathf.Abs(newChest.x - newChest.z) < 10 || Mathf.Abs(newChest.y - newChest.w) < 10) return false;
            // Can't be larger than 8 foot by 8 foot
            if (Mathf.Abs(newChest.x - newChest.z) > 96 || Mathf.Abs(newChest.y - newChest.w) > 96) return false;
            else return true;
        }
    }
    public bool NewDoorIsValid(Vector4 newDoor)
    {
        // If one side of door is not greater then 14, return false
        if (Mathf.Abs(newDoor.x - newDoor.z) < 14 && Mathf.Abs(newDoor.y - newDoor.w) < 14) return false;
        
        // Overlaps exactly 2 rooms
        int roomsOverlapped = 0;
        foreach (Vector4 room in Rooms)
        {
            if (RectanglesOverlap(room, newDoor))
            {
                roomsOverlapped += 1;
            }
        }
        if (roomsOverlapped != 2) return false;
        // Overlaps less than 3 walls
        List<Vector4> overlappingWalls = new List<Vector4>();
        foreach (Vector4 wall in InnerWalls)
        {
            if (RectanglesOverlap(wall, newDoor))
            {
                overlappingWalls.Add(wall);
                if (overlappingWalls.Count > 2) return false;
            }
        }
        // Also make sure door is contained in walls
        foreach (Vector4 wall in overlappingWalls)
        {
            if (!RectangleContainsPoint(wall, new Vector2(newDoor.x, newDoor.y)) &&
                !RectangleContainsPoint(wall, new Vector2(newDoor.z, newDoor.w)))
            {
                return false;
            }
        }
        // Atleast one side has to have a length of 4
        // This ensures that the door remains confined within the walls
        if (Mathf.Abs(newDoor.x - newDoor.z) == 4 || Mathf.Abs(newDoor.y - newDoor.w) == 4) return true;
        else return false;

    }
    public bool NewVacuumIsValid(Vector4 newVacuum)
    {
        newVacuum = Utility.CorrectRectPointOrder(newVacuum);
        if (VacuumStation.Count > 0) return false;
        if (!NewHouseObjectIsValid(newVacuum)) return false;
        return true;
    }
    public bool NewHouseObjectIsValid(Vector4 newRect)
    {
        // House Object must not overlap a wall, door, chest, table or the vacuum
        foreach (Vector4 obj in Tables)
        {
            if (RectanglesOverlap(obj, newRect)) { return false; }
        }
        foreach (Vector4 obj in InnerWalls)
        {
            if (RectanglesOverlap(obj, newRect)) { return false; }
        }
        /*foreach (Vector4 obj in Doors)
        {
            if (RectanglesOverlap(obj, newRect)) { return false; }
        }*/
        foreach (Vector4 obj in Chests)
        {
            if (RectanglesOverlap(obj, newRect)) { return false; }
        }
        foreach (Vector4 obj in VacuumStation)
        {
            if (RectanglesOverlap(obj, newRect)) { return false; }
        }
        // House object must also be in the confines of the house
        bool point1InHouse = false;
        bool point2InHouse = false;
        foreach (Vector4 room in Rooms)
        {
            if (RectangleContainsPoint(room, new Vector2(newRect.x, newRect.y))) point1InHouse = true;
            if (RectangleContainsPoint(room, new Vector2(newRect.z, newRect.w))) point2InHouse = true;
        }
        if (point1InHouse && point2InHouse) return true;
        else return false;
    }
    public void RemoveTable(Vector4 table)
    {
        Tables.Remove(table);
        Ref.I.ModelVisuals.RemoveDisplayedTabletop(table);
        List<Vector4> toRemove = new List<Vector4>();
        foreach (Vector4 leg in TableLegs)
        {
            if (RectangleOneContainsRectangleTwo(table, leg))
            {
                toRemove.Add(leg);
            }
        }
        foreach (Vector4 leg in toRemove)
        {
            RemoveTableleg(leg);
        }
        CalculateUncleanableArea();
    }
    public void RemoveTableleg(Vector4 leg)
    {
        TableLegs.Remove(leg);
        Ref.I.ModelVisuals.RemoveDisplayedTableleg(leg);
    }
    public void RemoveChest(Vector4 chest)
    {
        Chests.Remove(chest);
        Ref.I.ModelVisuals.RemoveDisplayedChest(chest);
        CalculateUncleanableArea();
    }
    public void RemoveWall(Vector4 wall)
    {
        InnerWalls.Remove(wall);
        Ref.I.ModelVisuals.RemoveDisplayedWall(wall);
    }
    public void RemoveVacuum(Vector4 vacuum)
    {
        VacuumStation.Remove(vacuum);
        Ref.I.ModelVisuals.RemoveDisplayedVacuum(vacuum);
        CalculateUncleanableArea();
    }
    public void RemoveFloor(Vector4 floor)
    {
        Rooms.Remove(floor);
        Ref.I.ModelVisuals.RemoveDisplayedFloor(floor);
        List<Vector4> toRemove = new List<Vector4>();
        foreach (Vector4 wall in InnerWalls)
        {
            if (RectangleOneContainsRectangleTwo(floor, wall))
            {
                toRemove.Add(wall);
            }
        }
        foreach (Vector4 wall in toRemove)
        {
            RemoveWall(wall);
        }
        toRemove = new List<Vector4>();
        foreach (Vector4 door in Doors)
        {
            if (RectanglesOverlap(floor, door))
            {
                toRemove.Add(door);
            }
        }
        foreach (Vector4 door in toRemove)
        {
            RemoveDoor(door);
        }
        toRemove = new List<Vector4>();
        foreach (Vector4 chest in Chests)
        {
            if (RectanglesOverlap(floor, chest))
            {
                toRemove.Add(chest);
            }
        }
        foreach (Vector4 chest in toRemove)
        {
            RemoveChest(chest);
        }
        toRemove = new List<Vector4>();
        foreach (Vector4 table in Tables)
        {
            if (RectanglesOverlap(floor, table))
            {
                toRemove.Add(table);
            }
        }
        foreach (Vector4 table in toRemove)
        {
            RemoveTable(table);
        }
        toRemove = new List<Vector4>();
        foreach (Vector4 vacuum in VacuumStation)
        {
            if (RectanglesOverlap(floor, vacuum))
            {
                toRemove.Add(vacuum);
            }
        }
        foreach (Vector4 vacuum in toRemove)
        {
            RemoveVacuum(vacuum);
        }
        // Fix remaining walls by... *shrug*
        // Can just refresh all walls using currently existing doors
        // or can do some complicated math to figure it out

        //Remove all walls
        toRemove = new List<Vector4>(InnerWalls);
        foreach (Vector4 wall in toRemove)
        {
            RemoveWall(wall);
        }
        //Re-add walls
        foreach (Vector4 room in Rooms)
        {
            // North
            AddWall(new Vector4(room.x, room.y, room.z, room.y - 2));
            // South
            AddWall(new Vector4(room.x, room.w + 2, room.z, room.w));
            // West
            AddWall(new Vector4(room.x, room.y, room.x + 2, room.w));
            // East
            AddWall(new Vector4(room.z - 2, room.y, room.z, room.w));
        }
        //Re-create doors
        List<Vector4> tempDoors = new List<Vector4>(Doors);
        Doors = new List<Vector4>();
        foreach (Vector4 door in tempDoors)
        {
            AddDoorIfValid(door);
        }
        CalculateTotalSquareFeet();
        CalculateUncleanableArea();
    }
    public void RemoveDoor(Vector4 door)
    {
        Doors.Remove(door);
    }
    
    public void DeleteObject(Vector2 p)
    {
        foreach (Vector4 rect in Tables)
        {
            if (RectangleContainsPoint(rect, p))
            {
                RemoveTable(rect);
                return;
            }
        }
        foreach (Vector4 rect in Chests)
        {
            if (RectangleContainsPoint(rect, p))
            {
                RemoveChest(rect);
                return;
            }
        }
        foreach (Vector4 rect in VacuumStation)
        {
            if (RectangleContainsPoint(rect, p))
            {
                RemoveVacuum(rect);
                return;
            }
        }
        foreach (Vector4 rect in Rooms)
        {
            if (RectangleContainsPoint(rect, p))
            {
                RemoveFloor(rect);
                return;
            }
        }
    }

    public void RemoveEverything()
    {
        List<Vector4> toRemove = new List<Vector4>(Rooms);
        foreach (Vector4 room in toRemove)
        {
            RemoveFloor(room);
        }
    }

    public Vector4 FindObjectRect(Vector2 p)
    {
        foreach (Vector4 rect in Tables)
        {
            if (RectangleContainsPoint(rect, p))
            {
                return rect;
            }
        }
        foreach (Vector4 rect in Chests)
        {
            if (RectangleContainsPoint(rect, p))
            {
                return rect;
            }
        }
        foreach (Vector4 rect in VacuumStation)
        {
            if (RectangleContainsPoint(rect, p))
            {
                return rect;
            }
        }
        foreach (Vector4 rect in Rooms)
        {
            if (RectangleContainsPoint(rect, p))
            {
                return rect;
            }
        }
        return new Vector4(0,0,0,0);
    }






    static bool RectangleContainsPoint(Vector4 r, Vector2 p)
    {
        r = Utility.CorrectRectPointOrder(r);
        if (r.x <= p.x && p.x <= r.z && r.w <= p.y && p.y <= r.y) return true;
        else return false;
    }

    static bool RectangleOneContainsRectangleTwo(Vector4 r1, Vector4 r2)
    {
        r1 = Utility.CorrectRectPointOrder(r1);
        r2 = Utility.CorrectRectPointOrder(r2);
        if (r1.x <= r2.x && r2.x <= r1.z && r1.w <= r2.y && r2.y <= r1.y &&
            r1.x <= r2.z && r2.z <= r1.z && r1.w <= r2.w && r2.w <= r1.y) { return true; }
        else { return false; }
    }

    // Returns true if two rectangles overlap
    // Does not return true if the rectangles share an edge
    static bool RectanglesOverlap(Vector4 r1, Vector4 r2)
    {
        // Left
        if (r1.x <= r2.x && r1.z <= r2.x && r1.x <= r2.z && r1.z <= r2.z) { return false; }
        // Right
        if (r1.x >= r2.x && r1.z >= r2.x && r1.x >= r2.z && r1.z >= r2.z) { return false; }
        // Down
        if (r1.y <= r2.y && r1.w <= r2.y && r1.y <= r2.w && r1.w <= r2.w) { return false; }
        // Up
        if (r1.y >= r2.y && r1.w >= r2.y && r1.y >= r2.w && r1.w >= r2.w) { return false; }
        return true;
    }

    // Returns true if two rectangles share an edge
    static bool RectanglesShareAnEdge(Vector4 r1, Vector4 r2)
    {
        // If left or right edges align
        if (r1.x == r2.x || r1.z == r2.z || r1.z == r2.x || r1.x == r2.z)
        {
            // If y values of either point in r1 fall in the exclusive y range of r2
            // (r1.y is between r2.y and r2.w) or (r1.w is between r2.y and r2.w) or
            // (r2.y is between r1.y and r1.w) or (r2.w is between r1.y and r1.w) or
            // (r1.y == r2.y && r1.w == r2.w) or (r1.w == r2.y && r1.y == r2.w)
            if (((r2.y > r1.y && r1.y > r2.w) || (r2.y < r1.y && r1.y < r2.w)) || 
                ((r2.y > r1.w && r1.w > r2.w) || (r2.y < r1.w && r1.w < r2.w)) ||
                ((r1.y > r2.y && r2.y > r1.w) || (r1.y < r2.y && r2.y < r1.w)) ||
                ((r1.y > r2.w && r2.w > r1.w) || (r1.y < r2.w && r2.w < r1.w)) ||
                (r1.y == r2.y && r1.w == r2.w) || (r1.w == r2.y && r1.y == r2.w)) 
            {
                return true;
            }
        }
        // If bottom or top edges align
        if (r1.y == r2.y || r1.w == r2.w || r1.w == r2.y || r1.y == r2.w)
        {
            // If y values of either point in r1 fall in the exclusive y range of r2
            // (r1.x is betzeen r2.x and r2.z) or (r1.z is betzeen r2.x and r2.z) or
            // (r2.x is betzeen r1.x and r1.z) or (r2.z is betzeen r1.x and r1.z) or
            // (r1.x == r2.x && r1.z == r2.z) or (r1.z == r2.x && r1.x == r2.z)
            if (((r2.x > r1.x && r1.x > r2.z) || (r2.x < r1.x && r1.x < r2.z)) ||
                ((r2.x > r1.z && r1.z > r2.z) || (r2.x < r1.z && r1.z < r2.z)) ||
                ((r1.x > r2.x && r2.x > r1.z) || (r1.x < r2.x && r2.x < r1.z)) ||
                ((r1.x > r2.z && r2.z > r1.z) || (r1.x < r2.z && r2.z < r1.z)) ||
                (r1.x == r2.x && r1.z == r2.z) || (r1.z == r2.x && r1.x == r2.z))
            {
                return true;
            }
        }


        return false;
    }
}
