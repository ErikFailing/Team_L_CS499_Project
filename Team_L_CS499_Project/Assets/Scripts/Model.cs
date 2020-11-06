using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour
{
    public int totalSquareFeet;
    public string FlooringType;
    public List<Vector4> Rooms;
    public List<Vector4> InnerWalls;

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

    public void ChangeFloorType(string type)
    {
        FlooringType = type;
        Ref.I.ModelVisuals.DisplayFloorType(type);
    }

    public void AddWall(Vector4 wall)
    {
        InnerWalls.Add(wall);
        Ref.I.ModelVisuals.DisplayNewWall(wall);
    }
    
    public void AddRoom(Vector4 room)
    {
        //Correct Point order
        room = Utility.CorrectRectPointOrder(room);
        Rooms.Add(room);
        // Add InnerWalls
        // North
        AddWall(new Vector4(room.x, room.y, room.z, room.y-2));
        // South
        AddWall(new Vector4(room.x, room.w+2, room.z, room.w));
        // West
        AddWall(new Vector4(room.x, room.y, room.x+2, room.w));
        // East
        AddWall(new Vector4(room.z-2, room.y, room.z, room.w));

        Ref.I.ModelVisuals.DisplayNewFloor(room);
        CalculateTotalSquareFeet();
    }

    public void RemoveRoom(Vector4 room)
    {
        //Correct point order
        room = Utility.CorrectRectPointOrder(room);
        Rooms.Remove(room);
        Ref.I.ModelVisuals.DestroyDisplayedRoom(room);
        CalculateTotalSquareFeet();
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
