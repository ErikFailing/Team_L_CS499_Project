using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    private float speed = 50.0f;
    private int pathPosition = 1;
    private GameObject vacuum;
    private List<Vector3> path;

    public void FindPath()
    {
<<<<<<< HEAD
        path = new List<Vector3> {
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(30.0f, 1.0f, 0.0f),
            new Vector3(30.0f, 1.0f, 30.0f),
            new Vector3(0.0f, 1.0f, 30.0f),
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(0.0f, 1.0f, -30.0f),
            new Vector3(-30.0f, 1.0f, -30.0f),
            new Vector3(-30.0f, 1.0f, 0.0f),
            new Vector3(0.0f, 1.0f, 0.0f)};
=======
        path = Ref.I.Model.data.RandomPaths[0].vectorThreeList;
        
        //path = new List<Vector3> {
        //    new Vector3(0.0f, 1.0f, 0.0f),
        //    new Vector3(30.0f, 1.0f, 0.0f),
        //    new Vector3(30.0f, 1.0f, 30.0f),
        //    new Vector3(0.0f, 1.0f, 30.0f),
        //    new Vector3(0.0f, 1.0f, 0.0f),
        //    new Vector3(0.0f, 1.0f, -30.0f),
        //    new Vector3(-30.0f, 1.0f, -30.0f),
        //    new Vector3(-30.0f, 1.0f, 0.0f)};
>>>>>>> New_Merge_Branch
    }

    public void SetVacuum()
    {
        if (Ref.I.Vacuums.transform.childCount > 0)
        {
            vacuum = Ref.I.Vacuums.transform.GetChild(0).gameObject;
        }
        else
        {
            vacuum = null;
            Debug.Log("Vacuum object does not exist.");
        }
    }

    public void StartSimulation()
    {
        SetVacuum();
        if (vacuum == null)
        {
            Debug.Log("Vacuum object has not been set.");
        }
        else
        {
            if (path == null)
            {
                FindPath();
            }
            if (path != null)
            {
                StartCoroutine("FollowPath");
            }
        }
    }

    public void PauseSimulation()
    {
        if (path != null)
        {
            StopCoroutine("FollowPath");
            vacuum.transform.position = path[pathPosition];
        }
    }

    public void StopSimulation()
    {
        if (path != null)
        {
            StopCoroutine("FollowPath");
            pathPosition = 1;
            DestroyTrail();
            vacuum.transform.position = path[pathPosition];
        }
    }

    public void ChangeSpeed(float speed)
    {
        this.speed = speed;
    }

    private void DestroyTrail()
    {
        foreach(Transform child in Ref.I.Simulation.gameObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void UpdateTrail(GameObject trail, Vector3 startingPos)
    {
        GameObject head = trail.transform.GetChild(0).gameObject;
        GameObject body = trail.transform.GetChild(1).gameObject;
        GameObject tail = trail.transform.GetChild(2).gameObject;
        head.transform.position = vacuum.transform.position;
        tail.transform.position = startingPos;
        body.transform.position = new Vector3((head.transform.position.x + tail.transform.position.x) / 2.0f,
            startingPos.y,
            (head.transform.position.z + tail.transform.position.z) / 2.0f);
        body.transform.rotation = Quaternion.FromToRotation(Vector3.forward, head.transform.position - tail.transform.position);
        body.transform.localScale = new Vector3(12.8f, 0.2f, Vector3.Distance(head.transform.position, tail.transform.position));
    }

    public IEnumerator FollowPath()
    {
        // This function will move the vacuum GameObject through the path at the speed of 3 in/s (3 units/s)
        while (pathPosition < path.Count)
        {
            Vector3 target = path[pathPosition];
            float elapsedTime = 0;
            Vector3 startingPos = vacuum.transform.position;
            float distance = Vector3.Distance(startingPos, target);
            float duration = distance / (3.0f * speed);
            GameObject trail = GameObject.Instantiate(Ref.I.TrailPrefab, new Vector3(0,0,0), vacuum.transform.rotation);
            trail.transform.parent = Ref.I.Simulation.gameObject.transform;
            trail.name = "Trail to " + pathPosition;

            while (elapsedTime < duration)
            {
                vacuum.transform.position = Vector3.Lerp(startingPos, target, (elapsedTime / duration));
                UpdateTrail(trail, startingPos);
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            vacuum.transform.position = target;
            pathPosition++;
        }
    }
}
