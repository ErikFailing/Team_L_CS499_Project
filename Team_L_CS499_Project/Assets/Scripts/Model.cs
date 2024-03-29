﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Model : MonoBehaviour
{
    public Data data;

    public Dictionary<Vector2, float> cleanablePoints;
    public int currentDistance;

    [Serializable]
    public class Data
    {
        public string name;
        public string pathingVersion;
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
        public List<VectorThreeListWrapper> RandomPaths;
        public List<VectorThreeListWrapper> SpiralPaths;
        public List<VectorThreeListWrapper> SnakingPaths;
        public List<VectorThreeListWrapper> WallfollowPaths;

        // Updated at save only
        public List<Vector2> cleanablePointsVectors;
    }




    void Start()
    {
        cleanablePoints = new Dictionary<Vector2, float>();
        data.pathingVersion = "v1.2.4";
        data.FlooringType = "Hardwood";
    }

    public void CalculateRandomPath()
    {
        // Add new path to RandomPaths
        data.RandomPaths.Add(new VectorThreeListWrapper(new List<Vector3>()));

        int pathIndex = data.RandomPaths.Count - 1;
        float totalDist = 0;
        Vector3 startPos = Ref.I.Vacuum.transform.position;
        
        // Add starting position to path
        data.RandomPaths[pathIndex].vectorThreeList.Add(startPos);

        // Start in a random direction
        Ref.I.Vacuum.transform.Rotate(0, UnityEngine.Random.Range(0, 359), 0);

        while (totalDist < 27000)
        {
            Ray ray = new Ray(Ref.I.Vacuum.transform.position, Ref.I.Vacuum.transform.forward);
            Physics.SphereCast(ray, 6.4f, out RaycastHit hitInfo);
            Vector3 target = ray.GetPoint(hitInfo.distance - 0.1f);
            // If vacuum can make the full distance without running out of battery, go the distance
            if (totalDist + hitInfo.distance <= 27000)
            {
                data.RandomPaths[pathIndex].vectorThreeList.Add(target);
                totalDist += hitInfo.distance - 0.1f;
            }
            else // Only go the remaning distance until the vacuum runs out of battery
            {
                float remainingDist = -totalDist + 27000;
                data.RandomPaths[pathIndex].vectorThreeList.Add(ray.GetPoint(remainingDist));
                totalDist += remainingDist;
                break;
            }
            
            if (Physics.OverlapSphere(target, 6.4f).Length > 0)
            {
                Debug.LogWarning("WARNING: Random Path may be incorrect.");
            }

            // Decide which way to turn
            // Cast two spheres, one to the left and another to the right
            // Turn in the direction of the sphere that got the farthest
            Ray rightRay = new Ray(target, Ref.I.Vacuum.transform.right);
            Physics.SphereCast(rightRay, 6.4f, out RaycastHit rightHitInfo);
            Ray leftRay = new Ray(target, -Ref.I.Vacuum.transform.right);
            Physics.SphereCast(leftRay, 6.4f, out RaycastHit leftHitInfo);

            if (rightHitInfo.distance > leftHitInfo.distance)
            {
                // Turn Right by a random amount
                Ref.I.Vacuum.transform.Rotate(0, UnityEngine.Random.Range(45, 135), 0);
            }
            else
            {
                // Turn Left by a random amount
                Ref.I.Vacuum.transform.Rotate(0, -UnityEngine.Random.Range(45, 135), 0);
            }

            // Move vacuum
            Ref.I.Vacuum.transform.position = target;
        }

        // Reset vacuum
        Ref.I.Vacuum.transform.position = startPos;
        Ref.I.Vacuum.transform.rotation = new Quaternion();
    }
    public void CalculateSnakingPath()
    {
        // Add new path to SnakingPaths
        data.SnakingPaths.Add(new VectorThreeListWrapper(new List<Vector3>()));

        int pathIndex = data.SnakingPaths.Count - 1;
        float totalDist = 0;
        Vector3 startPos = Ref.I.Vacuum.transform.position;

        // Add starting position to path
        data.SnakingPaths[pathIndex].vectorThreeList.Add(startPos);

        // Start in a random direction
        Ref.I.Vacuum.transform.Rotate(0, UnityEngine.Random.Range(0, 359), 0);

        while (totalDist < 27000)
        {

            int turnMod = 1;
            int directionMod = 0;

            while (totalDist < 27000)
            {

                // Go straight until wall is hit
                Ray ray = new Ray(Ref.I.Vacuum.transform.position, Ref.I.Vacuum.transform.forward);
                Physics.SphereCast(ray, 6.4f, out RaycastHit hitInfo);
                Vector3 target = ray.GetPoint(hitInfo.distance - 0.2f);

                if (directionMod == 0)
                {
                    // Decide which way to turn and set direction mod
                    // Cast two spheres, one to the left and another to the right
                    // Turn in the direction of the sphere that got the farthest
                    Ray rightRay = new Ray(target, Ref.I.Vacuum.transform.right);
                    Physics.SphereCast(rightRay, 6.4f, out RaycastHit rightHitInfo);
                    Ray leftRay = new Ray(target, -Ref.I.Vacuum.transform.right);
                    Physics.SphereCast(leftRay, 6.4f, out RaycastHit leftHitInfo);
                    if (rightHitInfo.distance > leftHitInfo.distance)
                    {
                        // Turn Right
                        directionMod = 1;
                    }
                    else
                    {
                        // Turn Left
                        directionMod = -1;
                    }
                }
                // If vacuum can make the full distance without running out of battery, go the distance
                if (totalDist + hitInfo.distance <= 27000)
                {
                    data.SnakingPaths[pathIndex].vectorThreeList.Add(target);
                    totalDist += hitInfo.distance - 0.2f;
                }
                else // Only go the remaning distance until the vacuum runs out of battery
                {
                    float remainingDist = -totalDist + 27000;
                    data.SnakingPaths[pathIndex].vectorThreeList.Add(ray.GetPoint(remainingDist));
                    totalDist += remainingDist;
                    break;
                }

                // Move vacuum
                Ref.I.Vacuum.transform.position = target;

                // If there is room to turn in turn direction, then turn
                Ray turnRay = new Ray(target, directionMod * turnMod * Ref.I.Vacuum.transform.right);
                Physics.SphereCast(turnRay, 6.4f, out RaycastHit turnHitInfo);
                if (turnHitInfo.distance > 13)
                {
                    Vector3 turnTarget = turnRay.GetPoint(12.8f);
                    if (totalDist + 12.8f <= 27000)
                    {
                        data.SnakingPaths[pathIndex].vectorThreeList.Add(turnTarget);
                        totalDist += 12.8f;
                    }
                    else // Only go the remaning distance until the vacuum runs out of battery
                    {
                        float remainingDist = -totalDist + 27000;
                        data.SnakingPaths[pathIndex].vectorThreeList.Add(turnRay.GetPoint(remainingDist));
                        totalDist += remainingDist;
                        break;
                    }


                    // Move vacuum in that direction
                    Ref.I.Vacuum.transform.position = turnRay.GetPoint(12.8f);

                    // Rotate vacuum 180
                    Ref.I.Vacuum.transform.Rotate(0, 180, 0);
                }
                else // Else turn randomly until can go forward again and change directionMod
                {
                    while (true)
                    {
                        // Turn by a random amount
                        Ref.I.Vacuum.transform.Rotate(0, UnityEngine.Random.Range(10, 135), 0);
                        // Check to see if forward movement is possible
                        Ray forwardRay = new Ray(target, Ref.I.Vacuum.transform.forward);
                        Physics.SphereCast(forwardRay, 6.4f, out RaycastHit forwardHitInfo);
                        if (forwardHitInfo.distance > 3) break;
                    }
                    break;
                }

                turnMod = turnMod * -1;
            }



        }

        // Reset vacuum
        Ref.I.Vacuum.transform.position = startPos;
        Ref.I.Vacuum.transform.rotation = new Quaternion();

    }
    public void CalculateSpiralPath()
    {
        // Add new path to SpiralPaths
        data.SpiralPaths.Add(new VectorThreeListWrapper(new List<Vector3>()));

        int pathIndex = data.SpiralPaths.Count - 1;
        float totalDist = 0;
        Vector3 startPosition = Ref.I.Vacuum.transform.position;

        // Add starting position to path
        data.SpiralPaths[pathIndex].vectorThreeList.Add(startPosition);

        // Start in a random direction
        Ref.I.Vacuum.transform.Rotate(0, UnityEngine.Random.Range(0, 359), 0);

        while (totalDist < 27000)
        {
            // Basic move in straight line forward
            Ray ray = new Ray(Ref.I.Vacuum.transform.position, Ref.I.Vacuum.transform.forward);
            Physics.SphereCast(ray, 6.4f, out RaycastHit hitInfo);
            Vector3 target = ray.GetPoint(hitInfo.distance - 0.1f);
            // Check to see if there is enough space to make a spiral along the path forward
            bool spiralCreated = false;
            float currentDistance = 5f;
            while (currentDistance < hitInfo.distance - 0.1f && totalDist > 0)
            {
                // If enough room to create spiral exists, create spiral
                if (Physics.OverlapSphere(ray.GetPoint(currentDistance), 25).Length == 0)
                {
                    spiralCreated = true;
                    // Navigate to spiral start point
                    Vector3 center = ray.GetPoint(currentDistance);
                    // If vacuum can make the full distance without running out of battery, go the distance
                    if (totalDist + currentDistance <= 27000)
                    {
                        data.SpiralPaths[pathIndex].vectorThreeList.Add(center);
                        totalDist += currentDistance;
                    }
                    else // Only go the remaning distance until the vacuum runs out of battery
                    {
                        float remainingDist = -totalDist + 27000;
                        data.SpiralPaths[pathIndex].vectorThreeList.Add(ray.GetPoint(remainingDist));
                        totalDist += remainingDist;
                        break;
                    }
                    // Move vacuum
                    Ref.I.Vacuum.transform.position = center;

                    // Create spiral
                    float distanceFromCenter = 1f;
                    float rotation = 30f; 
                    while (totalDist < 27000)
                    {
                        // Save starting position
                        Vector3 startPos = Ref.I.Vacuum.transform.position;
                        // Increase distance from center
                        Ref.I.Vacuum.transform.position = new Vector3(center.x, center.y, center.z + distanceFromCenter);
                        // Rotate
                        Ref.I.Vacuum.transform.RotateAround(center, Ref.I.Vacuum.transform.up, rotation);
                        // Save target position
                        Vector3 targetPos = Ref.I.Vacuum.transform.position;
                        //Reset position
                        Ref.I.Vacuum.transform.position = startPos;
                        //Look at target
                        Ref.I.Vacuum.transform.LookAt(targetPos);
                        // Calculate distance to target
                        float distanceToTarget = Vector3.Distance(startPos, targetPos);
                        // Cast a sphere towards target
                        Ray spiralRay = new Ray(startPos, Ref.I.Vacuum.transform.forward);
                        Physics.SphereCast(spiralRay, 6.4f, out RaycastHit spiralHitInfo);
                        // If vacuum can't completely move to target
                        if (spiralHitInfo.distance < distanceToTarget - 0.2) 
                        {
                            // If vacuum can make the full distance without running out of battery, go the distance
                            if (totalDist + spiralHitInfo.distance <= 27000)
                            {
                                data.SpiralPaths[pathIndex].vectorThreeList.Add(spiralRay.GetPoint(spiralHitInfo.distance));
                                totalDist += spiralHitInfo.distance;
                            }
                            else // Only go the remaning distance until the vacuum runs out of battery
                            {
                                float remainingDist = -totalDist + 27000;
                                data.SpiralPaths[pathIndex].vectorThreeList.Add(Vector3.MoveTowards(startPos, targetPos, remainingDist));
                                totalDist += remainingDist;
                                break;
                            }
                            Ref.I.Vacuum.transform.position = spiralRay.GetPoint(spiralHitInfo.distance);
                            break;
                        }
                        else // If vacuum can completely move to target
                        {
                            // If vacuum can make the full distance without running out of battery, go the distance
                            if (totalDist + distanceToTarget <= 27000)
                            {
                                data.SpiralPaths[pathIndex].vectorThreeList.Add(targetPos);
                                totalDist += distanceToTarget;
                            }
                            else // Only go the remaning distance until the vacuum runs out of battery
                            {
                                float remainingDist = -totalDist + 27000;
                                data.SpiralPaths[pathIndex].vectorThreeList.Add(Vector3.MoveTowards(startPos, targetPos, remainingDist));
                                totalDist += remainingDist;
                                break;
                            }
                            Ref.I.Vacuum.transform.position = targetPos;
                        }

                        rotation += 15f;
                        distanceFromCenter += 0.55f;
                    }
                    
                    break;
                }

                currentDistance += UnityEngine.Random.Range(25, 300);
            }

            if (!spiralCreated)
            {
                // If vacuum can make the full distance without running out of battery, go the distance
                if (totalDist + hitInfo.distance <= 27000)
                {
                    data.SpiralPaths[pathIndex].vectorThreeList.Add(target);
                    totalDist += hitInfo.distance - 0.1f;
                }
                else // Only go the remaning distance until the vacuum runs out of battery
                {
                    float remainingDist = -totalDist + 27000;
                    data.SpiralPaths[pathIndex].vectorThreeList.Add(ray.GetPoint(remainingDist));
                    totalDist += remainingDist;
                    break;
                }


                // Decide which way to turn
                // Cast two spheres, one to the left and another to the right
                // Turn in the direction of the sphere that got the farthest
                Ray rightRay = new Ray(target, Ref.I.Vacuum.transform.right);
                Physics.SphereCast(rightRay, 6.4f, out RaycastHit rightHitInfo);
                Ray leftRay = new Ray(target, -Ref.I.Vacuum.transform.right);
                Physics.SphereCast(leftRay, 6.4f, out RaycastHit leftHitInfo);

                if (rightHitInfo.distance > leftHitInfo.distance)
                {
                    // Turn Right by a random amount
                    Ref.I.Vacuum.transform.Rotate(0, UnityEngine.Random.Range(45, 135), 0);
                }
                else
                {
                    // Turn Left by a random amount
                    Ref.I.Vacuum.transform.Rotate(0, -UnityEngine.Random.Range(45, 135), 0);
                }

                // Move vacuum
                Ref.I.Vacuum.transform.position = target;
            }
            
        }

        // Reset vacuum
        Ref.I.Vacuum.transform.position = startPosition;
        Ref.I.Vacuum.transform.rotation = new Quaternion();

    }
    public void CalculateWallFollowPath()
    {
        // Add new path to WallfollowPaths
        data.WallfollowPaths.Add(new VectorThreeListWrapper(new List<Vector3>()));

        // Setup
        int pathIndex = data.WallfollowPaths.Count - 1;
        float totalDist = 0;
        Vector3 startPos = Ref.I.Vacuum.transform.position;

        // Add starting position to path
        data.WallfollowPaths[pathIndex].vectorThreeList.Add(startPos);

        // Start in a random direction
        Ref.I.Vacuum.transform.Rotate(0, UnityEngine.Random.Range(0, 359), 0);

        while (totalDist < 27000)
        {
            Ray ray = new Ray(Ref.I.Vacuum.transform.position, Ref.I.Vacuum.transform.forward);
            Physics.SphereCast(ray, 6.4f, out RaycastHit hitInfo);
            float distance = hitInfo.distance - 0.1f;
            Vector3 target = ray.GetPoint(distance);
            // While going towards target, if there sphere cast left goes a large distance
            // Skip this check on the first loop
            float currentDistance = 1f;
            while (currentDistance < distance && totalDist > 0)
            {
                
                Ray leftRay = new Ray(ray.GetPoint(currentDistance), -Ref.I.Vacuum.transform.right);
                Physics.SphereCast(leftRay, 6.4f, out RaycastHit leftHitInfo);
                if (leftHitInfo.distance > 2)
                {
                    // Turn Left
                    Ref.I.Vacuum.transform.Rotate(0, -100, 0);
                    // Set new target
                    target = ray.GetPoint(currentDistance);
                    // Set distance
                    distance = currentDistance;
                    // break
                    break;
                }
                currentDistance += 0.5f;
            }
            // If vacuum can make the full distance without running out of battery, go the distance
            if (totalDist + distance <= 27000)
            {
                data.WallfollowPaths[pathIndex].vectorThreeList.Add(target);
                totalDist += hitInfo.distance - 0.1f;
            }
            else // Only go the remaining distance until the vacuum runs out of battery
            {
                float remainingDist = -totalDist + 27000;
                data.WallfollowPaths[pathIndex].vectorThreeList.Add(ray.GetPoint(remainingDist));
                totalDist += remainingDist;
                break;
            }

            if (Physics.OverlapSphere(target, 6.4f).Length > 0)
            {
                Debug.LogWarning("WARNING: Spiral Path may be incorrect.");
            }


            // Turn until robot can move forward again
            while (true)
            {
                // Turn
                Ref.I.Vacuum.transform.Rotate(0, 1, 0);
                // Spherecast
                Ray forwardRay = new Ray(target, Ref.I.Vacuum.transform.forward);
                Physics.SphereCast(forwardRay, 6.4f, out RaycastHit forwardHitInfo);
                if (forwardHitInfo.distance > 0.7)
                {
                    break;
                }
            }
            
            // Move vacuum
            Ref.I.Vacuum.transform.position = target;
        }

        // Reset vacuum
        Ref.I.Vacuum.transform.position = startPos;
        Ref.I.Vacuum.transform.rotation = new Quaternion();
    }


    public float CalculateCoveragePercentage()
    {
        float maxCleanliness = cleanablePoints.Keys.Count * 350;
        float currentCleanliness = 0;
        foreach (Vector2 key in cleanablePoints.Keys)
        {
            currentCleanliness += cleanablePoints[key];
        }

        return (currentCleanliness / maxCleanliness);
    }


    // Takes in the distance the robot is into the path and adjusts 
    // the model to represent current cleanliness
    // distance is how far (in inches) the robot is along the path
    public void CalculateCleanliness(int distance)
    {
        if (distance < 1 && distance == 27000) return;
        
        // Need to make this switch based on current path
        List<Vector3> path = Ref.I.Simulation.runs[Ref.I.Simulation.runNum].path;
        //if (Ref.I.Simulation.algorithmType == "Random")
        //{
        //    path = Ref.I.Simulation.runs[Ref.I.Simulation.runNum].path;
        //}
        //else if (Ref.I.Simulation.algorithmType == "Spiral")
        //{
        //    path = data.SpiralPaths[0].vectorThreeList;
        //}
        //else if (Ref.I.Simulation.algorithmType == "Snaking")
        //{
        //    path = data.SnakingPaths[0].vectorThreeList;
        //}
        //else if (Ref.I.Simulation.algorithmType == "Wall follow")
        //{
        //    path = data.WallfollowPaths[0].vectorThreeList;
        //}


        float cleaningEfficieny = 0;

        // Set cleaning efficiency based on floor type
        if (data.FlooringType == "Hardwood")
        {
            cleaningEfficieny = 4;
        }
        else if (data.FlooringType == "Loop Pile Carpet")
        {
            cleaningEfficieny = 3;
        }
        else if (data.FlooringType == "Cut Pile Carpet")
        {
            cleaningEfficieny = 2;
        }
        else if (data.FlooringType == "Frieze-cut Pile Carpet")
        {
            cleaningEfficieny = 1;
        }

        //float distanceToMove = distanceDesired - currentDistance;
        Vector3 currentPos = path[0];
        int targetIndex = 1;

        // Advance along path until currentDistance is reached
        float remainingDistance = currentDistance;
        while (remainingDistance > 0 && targetIndex < path.Count)
        {
            float distanceToNextPoint = Vector3.Distance(currentPos, path[targetIndex]);
            if (distanceToNextPoint < remainingDistance)
            {
                // Advance to next point
                currentPos = Vector3.MoveTowards(currentPos, path[targetIndex], distanceToNextPoint);
                remainingDistance -= distanceToNextPoint;
                targetIndex += 1;
            }
            else
            {
                // Advance along path as normal
                currentPos = Vector3.MoveTowards(currentPos, path[targetIndex], remainingDistance);
                remainingDistance -= remainingDistance;
            }
        }




        // Clean
        while (currentDistance <= distance)
        {
            // Clean current point
            // Vacuum
            List<Vector2> vacuumPoints = GetPointsWithinCircle(new Vector2(currentPos.x, currentPos.z), 5.8f);
            foreach (Vector2 p in vacuumPoints)
            {
                cleanablePoints[p] = Mathf.Clamp(cleanablePoints[p] + cleaningEfficieny, 0, 350);
            }
            // Whiskers
            List<Vector2> whiskerPoints = GetPointsWithinCircle(new Vector2(currentPos.x, currentPos.z), 13.5f);
            // Remove vacuum points so they do get cleaned twice
            foreach (Vector2 p in vacuumPoints)
            {
                whiskerPoints.Remove(p);
            }
            foreach (Vector2 p in whiskerPoints)
            {
                cleanablePoints[p] = Mathf.Clamp(cleanablePoints[p] + (cleaningEfficieny * 0.7f), 0, 350);
            }

            // Advance current position one inch along the path
            remainingDistance = 1;
            while (remainingDistance > 0 && targetIndex < path.Count)
            {
                float distanceToNextPoint = Vector3.Distance(currentPos, path[targetIndex]);
                if (distanceToNextPoint < remainingDistance)
                {
                    // Advance to next point
                    currentPos = Vector3.MoveTowards(currentPos, path[targetIndex], distanceToNextPoint);
                    remainingDistance -= distanceToNextPoint;
                    targetIndex += 1;
                }
                else
                {
                    // Advance along path as normal
                    currentPos = Vector3.MoveTowards(currentPos, path[targetIndex], remainingDistance);
                    remainingDistance -= remainingDistance;
                }
            }


            currentDistance += 1;
        }
        
        currentDistance = distance;
    }

    // Used to points within the radius of the whiskers and vacuum
    public List<Vector2> GetPointsWithinCircle(Vector2 center, float radius)
    {
        List<Vector2> pList = new List<Vector2>();
        for (float x = center.x-radius-1; x < center.x+radius+1; x +=1)
        {
            for (float y = center.y - radius - 1; y < center.y + radius + 1; y += 1)
            {
                Vector2 p = new Vector2(Mathf.Round(x), Mathf.Round(y));
                if (cleanablePoints.ContainsKey(p) && Vector2.Distance(p, center) <= radius)
                {
                    // Point exists and is within radius, add to list
                    pList.Add(p);
                }
            }
        }
        return pList;
    }

    public void ResetPoints()
    {
        List<Vector2> keys = new List<Vector2>(cleanablePoints.Keys);
        cleanablePoints.Clear();
        foreach (Vector2 p in keys)
        {
            cleanablePoints.Add(p, 0);
        }
    }




    public bool VerifyHousePlan(out string errorMsg)
    {
        errorMsg = "";
        // House must be between 200 and 8,000 square feet
        if (data.totalSquareFeet < 200 || data.totalSquareFeet > 8000)
        {
            errorMsg = "ERROR: House must have a square footage in the range of [200, 8000]. Current square footage is " + data.totalSquareFeet;
            return false;
        }
        // House must have a vacuum
        if (data.VacuumStation.Count < 1)
        {
            errorMsg = "ERROR: House must have a robot vacuum in it.";
            return false;
        }
        // House must have a name
        if (data.name == "")
        {
            errorMsg = "ERROR: House plan must have a name";
            return false;
        }
        CalculatePoints();
        // If house has one room, return true
        if (data.Rooms.Count == 1)
        {
            //Ref.I.ModelVisuals.DisplayNewPoints(cleanablePoints);
            //Debug.Log(cleanablePoints.Keys.Count);
            return true;
        }
        // All rooms must have atleast one door
        
        // If each room has a point along its edge, return true
        // Else, a room lacks a door or is otherwise unenterable, return false
        foreach (Vector4 room in data.Rooms)
        {
            bool containsEntry = false;
            // Create list of points along room's edge
            List<Vector2> edges = new List<Vector2>();
            // Top and bottom
            for (int x = (int)room.x; x <= room.z; x++)
            {
                edges.Add(new Vector2(x, room.y));
                edges.Add(new Vector2(x, room.w));
            }
            // Left and Right
            for (int y = (int)room.y; y >= room.w; y--)
            {
                edges.Add(new Vector2(room.x, y));
                edges.Add(new Vector2(room.z, y));
            }
            foreach (Vector2 p in edges)
            {
                if (cleanablePoints.ContainsKey(p) && Physics.OverlapSphere(new Vector3(p.x, 2, p.y), 7).Length < 1)
                {
                    containsEntry = true;
                    break;
                }
            }
            if (!containsEntry)
            {
                errorMsg = "ERROR: A room does not have a traversable entry (door)";
                return false;
            }
            
        }
        //Ref.I.ModelVisuals.DisplayNewPoints(cleanablePoints);
        //Debug.Log(cleanablePoints.Keys.Count);
        return true;
    }
    
    
    public void CalculatePoints()
    {
        foreach (Vector4 room in data.Rooms)
        {
            // Add points that are contained in rooms
            for (int x = (int)room.x; x <= room.z; x +=4)
            {
                for (int y = (int)room.y; y >= room.w; y -=4)
                {
                    Vector2 point = new Vector2(x, y);
                    // Only add if it isn't already added
                    if (!cleanablePoints.ContainsKey(point))
                    {
                        // Make sure point isn't in an object
                        bool traversable = true;
                        foreach (Vector4 rect in data.InnerWalls)
                        {
                            if (RectangleContainsPoint(rect, point)) { traversable = false; break; }
                        }
                        foreach (Vector4 rect in data.Chests)
                        {
                            if (!traversable || RectangleContainsPoint(rect, point)) { traversable = false; break; }
                        }
                        foreach (Vector4 rect in data.TableLegs)
                        {
                            if (!traversable || RectangleContainsPoint(rect, point)) { traversable = false; break; }
                        }
                        if (traversable)
                        {
                            cleanablePoints.Add(point, 0);
                        }
                        
                    }
                    
                }
            }
        }
    }
    
    
    
    
    public void CalculateTotalSquareFeet()
    {
        data.totalSquareFeet = 0;
        foreach (Vector4 room in data.Rooms)
        {
            data.totalSquareFeet += Mathf.RoundToInt(Mathf.Abs(((room.x - room.z)/12) * ((room.y - room.w)/12)) );
        }
        // Update GUI
        Ref.I.GUI.UpdateSquareFootage(data.totalSquareFeet);
    }

    public void CalculateUncleanableArea()
    {
        float area = 0;
        List<Vector4> rects = new List<Vector4>(data.Chests);
        rects.AddRange(data.TableLegs);
        rects.AddRange(data.VacuumStation);
        foreach (Vector4 rect in rects)
        {
            float width = Mathf.Abs(rect.x - rect.z);
            float height = Mathf.Abs(rect.y - rect.w);
            area += width * height;
        }
        //Walls
        int overlappingWalls = 0;
        foreach (Vector4 rect in data.InnerWalls)
        {
            float width = Mathf.Abs(rect.x - rect.z);
            float height = Mathf.Abs(rect.y - rect.w);
            area += width * height;
            overlappingWalls += -1;
            foreach (Vector4 wall in data.InnerWalls)
            {
                if (RectanglesOverlap(rect, wall)) overlappingWalls++;
            }
        }
        area -= (overlappingWalls / 2) * 4;
        data.uncleanableArea = Mathf.RoundToInt(area / 144);
        Ref.I.GUI.UpdateUncleanableArea(data.uncleanableArea);
    }

    public void ChangeFloorType(string type)
    {
        data.FlooringType = type;
        Ref.I.ModelVisuals.DisplayFloorType(type);
    }

    public void AddWall(Vector4 wall)
    {
        data.InnerWalls.Add(wall);
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
            data.Rooms.Add(room);
            // Add data.InnerWalls
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
        data.TableLegs.Add(leg);
        Ref.I.ModelVisuals.DisplayNewTableleg(leg);
    }

    public void AddTableIfValid(Vector4 table)
    {
        // Correct Point order
        table = Utility.CorrectRectPointOrder(table);
        // Add if valid
        if (NewTableIsValid(table))
        {
            data.Tables.Add(table);
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
            data.Chests.Add(chest);
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
            data.Doors.Add(door);
            // Find the two overlapping walls
            List<Vector4> overlappingWalls = new List<Vector4>();
            foreach (Vector4 wall in data.InnerWalls)
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
            data.VacuumStation.Add(vacuum);
            Ref.I.ModelVisuals.DisplayVacuum(vacuum);
            CalculateUncleanableArea();
        } 
    }

    

    public bool NewRoomIsValid(Vector4 newRoom)
    {
        // Room is not valid if it has a side with a size of zero
        if (newRoom.x == newRoom.z || newRoom.y == newRoom.w) { return false; }
        
        // NOT first room and NOT connecting to other rooms
        if (data.Rooms.Count > 0)
        {
            // Room isn't valid if it overlaps any other room
            foreach (Vector4 room in data.Rooms)
            {
                if (RectanglesOverlap(newRoom, room))
                {
                    return false;
                }
            }
            // Room is valid if it shares an edge and doesn't overlap
            foreach (Vector4 room in data.Rooms)
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
        foreach (Vector4 room in data.Rooms)
        {
            if (RectanglesOverlap(room, newDoor))
            {
                roomsOverlapped += 1;
            }
        }
        if (roomsOverlapped != 2) return false;
        // Overlaps less than 3 walls
        List<Vector4> overlappingWalls = new List<Vector4>();
        foreach (Vector4 wall in data.InnerWalls)
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
        if (data.VacuumStation.Count > 0) return false;
        if (!NewHouseObjectIsValid(newVacuum)) return false;
        return true;
    }
    public bool NewHouseObjectIsValid(Vector4 newRect)
    {
        // House Object must not overlap a wall, door, chest, table or the vacuum
        foreach (Vector4 obj in data.Tables)
        {
            if (RectanglesOverlap(obj, newRect)) { return false; }
        }
        foreach (Vector4 obj in data.InnerWalls)
        {
            if (RectanglesOverlap(obj, newRect)) { return false; }
        }
        /*foreach (Vector4 obj in data.Doors)
        {
            if (RectanglesOverlap(obj, newRect)) { return false; }
        }*/
        foreach (Vector4 obj in data.Chests)
        {
            if (RectanglesOverlap(obj, newRect)) { return false; }
        }
        foreach (Vector4 obj in data.VacuumStation)
        {
            if (RectanglesOverlap(obj, newRect)) { return false; }
        }
        // House object must also be in the confines of the house
        bool point1InHouse = false;
        bool point2InHouse = false;
        foreach (Vector4 room in data.Rooms)
        {
            if (RectangleContainsPoint(room, new Vector2(newRect.x, newRect.y))) point1InHouse = true;
            if (RectangleContainsPoint(room, new Vector2(newRect.z, newRect.w))) point2InHouse = true;
        }
        if (point1InHouse && point2InHouse) return true;
        else return false;
    }
    public void RemoveTable(Vector4 table)
    {
        data.Tables.Remove(table);
        Ref.I.ModelVisuals.RemoveDisplayedTabletop(table);
        List<Vector4> toRemove = new List<Vector4>();
        foreach (Vector4 leg in data.TableLegs)
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
        data.TableLegs.Remove(leg);
        Ref.I.ModelVisuals.RemoveDisplayedTableleg(leg);
    }
    public void RemoveChest(Vector4 chest)
    {
        data.Chests.Remove(chest);
        Ref.I.ModelVisuals.RemoveDisplayedChest(chest);
        CalculateUncleanableArea();
    }
    public void RemoveWall(Vector4 wall)
    {
        data.InnerWalls.Remove(wall);
        Ref.I.ModelVisuals.RemoveDisplayedWall(wall);
    }
    public void RemoveVacuum(Vector4 vacuum)
    {
        data.VacuumStation.Remove(vacuum);
        Ref.I.ModelVisuals.RemoveDisplayedVacuum(vacuum);
        CalculateUncleanableArea();
    }
    public void RemoveFloor(Vector4 floor)
    {
        data.Rooms.Remove(floor);
        Ref.I.ModelVisuals.RemoveDisplayedFloor(floor);
        List<Vector4> toRemove = new List<Vector4>();
        foreach (Vector4 wall in data.InnerWalls)
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
        foreach (Vector4 door in data.Doors)
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
        foreach (Vector4 chest in data.Chests)
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
        foreach (Vector4 table in data.Tables)
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
        foreach (Vector4 vacuum in data.VacuumStation)
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
        toRemove = new List<Vector4>(data.InnerWalls);
        foreach (Vector4 wall in toRemove)
        {
            RemoveWall(wall);
        }
        //Re-add walls
        foreach (Vector4 room in data.Rooms)
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
        List<Vector4> tempDoors = new List<Vector4>(data.Doors);
        data.Doors = new List<Vector4>();
        foreach (Vector4 door in tempDoors)
        {
            AddDoorIfValid(door);
        }
        CalculateTotalSquareFeet();
        CalculateUncleanableArea();
    }
    public void RemoveDoor(Vector4 door)
    {
        data.Doors.Remove(door);
    }
    
    public void DeleteObject(Vector2 p)
    {
        foreach (Vector4 rect in data.Tables)
        {
            if (RectangleContainsPoint(rect, p))
            {
                RemoveTable(rect);
                return;
            }
        }
        foreach (Vector4 rect in data.Chests)
        {
            if (RectangleContainsPoint(rect, p))
            {
                RemoveChest(rect);
                return;
            }
        }
        foreach (Vector4 rect in data.VacuumStation)
        {
            if (RectangleContainsPoint(rect, p))
            {
                RemoveVacuum(rect);
                return;
            }
        }
        foreach (Vector4 rect in data.Rooms)
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
        List<Vector4> toRemove = new List<Vector4>(data.Rooms);
        foreach (Vector4 room in toRemove)
        {
            RemoveFloor(room);
        }
        data.RandomPaths.Clear();
        cleanablePoints = new Dictionary<Vector2, float>();
        data.SpiralPaths.Clear();
        data.WallfollowPaths.Clear();
        data.SnakingPaths.Clear();
        data.cleanablePointsVectors =  new List<Vector2>();
        ChangeFloorType("Hardwood");
        data.name = "";
        data.pathingVersion = "v1.2.4";
        currentDistance = 0;
    }

    public Vector4 FindObjectRect(Vector2 p)
    {
        foreach (Vector4 rect in data.Tables)
        {
            if (RectangleContainsPoint(rect, p))
            {
                return rect;
            }
        }
        foreach (Vector4 rect in data.Chests)
        {
            if (RectangleContainsPoint(rect, p))
            {
                return rect;
            }
        }
        foreach (Vector4 rect in data.VacuumStation)
        {
            if (RectangleContainsPoint(rect, p))
            {
                return rect;
            }
        }
        foreach (Vector4 rect in data.Rooms)
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
