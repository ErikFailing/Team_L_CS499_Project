using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Utility
{
    public static Vector3 PosFromRect(Vector4 rect)
    {
        return PosFromRect(new Vector3(rect.x, 0, rect.y), new Vector3(rect.z, 0, rect.w));
    }
    public static Vector3 PosFromRect(Vector3 point1, Vector3 point2)
    {
        return Vector3.MoveTowards(point1, point2, Vector3.Distance(point1, point2) * 0.5f);
    }


    public static Vector3 ScaleFromRect(Vector4 rect)
    {
        return ScaleFromRect(new Vector3(rect.x, 0, rect.y), new Vector3(rect.z, 0, rect.w));
    }
    public static Vector3 ScaleFromRect(Vector3 point1, Vector3 point2)
    {
        return new Vector3(Mathf.Abs(point1.x - point2.x), 1, Mathf.Abs(point1.z - point2.z));
    }


    public static GameObject CastHitRayToWorld()
    {
        // Create a ray from camera position to mouse point
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit info;
        if (Physics.Raycast(ray, out info, 100000f))
        {
            return info.collider.gameObject;
        }
        else return null;
        
    }

    // Include options for bounds and for rounding
    public static Vector3 CastRayToWorld(float h, Vector4 bounds)
    {
        Vector3 point;
        // Create a ray from camera position to mouse point
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Create Horizontal Plane at given height
        Plane plane = new Plane(Vector3.up, h);
        // If plane and ray intersect, return intersection point
        if (plane.Raycast(ray, out float distance))
        {
            point = ray.GetPoint(distance);
            // Clamp point between bounds
            if (bounds.x <= bounds.z) { point.x = Mathf.Clamp(point.x, bounds.x, bounds.z); }
            else { point.x = Mathf.Clamp(point.x, bounds.z, bounds.x); }
            if (bounds.y <= bounds.w) { point.z = Mathf.Clamp(point.z, bounds.y, bounds.w); }
            else { point.z = Mathf.Clamp(point.z, bounds.w, bounds.y); }
            // Ensure y = height
            point.y = h;
            return point;
        }
        else
        {
            return new Vector3(0, h, 0);
        }
    }


    public static Vector4 GetWorldBounds()
    {
        Vector4 bounds = new Vector4
        {
            x = (Ref.I.Ground.transform.localScale.x * 0.5f),
            y = (Ref.I.Ground.transform.localScale.z * 0.5f),
            z = -(Ref.I.Ground.transform.localScale.x * 0.5f),
            w = -(Ref.I.Ground.transform.localScale.z * 0.5f)
        };
        return bounds;
    }


    public static Vector3 RoundToNearestAwayFromZero(Vector3 point, float nearest, Vector3 offset)
    {
        point.x = RoundToNearestAwayFromZero(point.x, nearest, offset.x);
        point.z = RoundToNearestAwayFromZero(point.z, nearest, offset.z);
        return point;
    }
    public static float RoundToNearestAwayFromZero(float num, float nearest, float offset = 0)
    {
        // If num is greater than or equal to offset, round up
        if (num >= offset)
        {
            // Round up
            return Mathf.CeilToInt(num / nearest) * nearest;
        }
        else //Else round down
        {
            // Round down
            return Mathf.FloorToInt(num / nearest) * nearest;
        }
    }

    public static Vector3 RoundToNearest(Vector3 point, float nearest)
    {
        point.x = RoundToNearest(point.x, nearest);
        point.z = RoundToNearest(point.z, nearest);
        return point;
    }
    public static float RoundToNearest(float num, float nearest)
    {
        int multiple = 0;
        if (Mathf.Abs(num) % nearest >= nearest / 2)
        {
            // Round away from zero
            if (num < 0) { multiple = Mathf.FloorToInt(num / nearest); }
            else { multiple = Mathf.CeilToInt(num / nearest); }
        }
        else
        {
            // Round towards zero
            if (num < 0) { multiple = Mathf.CeilToInt(num / nearest); }
            else { multiple = Mathf.FloorToInt(num / nearest); }
        }
        return nearest * multiple;
    }

    // Makes sure the first vector2 is the upperleft corner and the 2nd if the bottom right corner
    public static Vector4 CorrectRectPointOrder(Vector4 rect)
    {
        Vector4 newRect = rect;
        if (rect.x > rect.z)
        {
            newRect.x = rect.z;
            newRect.z = rect.x;
        }
        if (rect.y < rect.w)
        {
            newRect.y = rect.w;
            newRect.w = rect.y;
        }
        return newRect;
    }


    public static Vector4 ObjectToRect(GameObject obj)
    {
        Vector3 pos = obj.transform.position;
        Vector3 scale = obj.transform.localScale;
        return new Vector4(pos.x - (scale.x / 2), pos.z + (scale.z / 2), pos.x + (scale.x / 2), pos.z - (scale.z / 2));
    }
}


/// <summary>
/// A wrapper structure for a List of strings
/// Used to create multi-dimensional lists of strings
/// </summary>
[Serializable]
public struct StringListWrapper
{
    public List<string> stringList;
    public StringListWrapper(List<string> l)
    {
        stringList = l;
    }
}

/// <summary>
/// A wrapper structure for a List of Vector3s
/// Used to create multi-dimensional lists of Vector3s
/// </summary>
[Serializable]
public struct VectorThreeListWrapper
{
    public List<Vector3> vectorThreeList;
    public VectorThreeListWrapper(List<Vector3> l)
    {
        vectorThreeList = l;
    }
}


