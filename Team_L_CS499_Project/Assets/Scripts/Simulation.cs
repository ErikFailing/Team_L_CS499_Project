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
    }
    public void StartSimulation()
    {
        if (vacuum == null)
        {
            vacuum = Ref.I.Vacuum;
        }
        if (path == null)
        {
            FindPath();
        }
        if (path != null)
        {
            StartCoroutine("FollowPath");
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
            vacuum.transform.position = path[pathPosition];
        }
    }

    public void ChangeSpeed(float speed)
    {
        this.speed = speed;
    }

    // public void MoveOverTime(Vector4 end)
    // { 
    //     float elapsedTime = 0;
    //     Vector3 startingPos = vacuum.transform.position;
    //     float distance = Vector3.Distance(startingPos, end);
    //     float duration = distance / (3.0f * speed);
    //     while (elapsedTime < duration)
    //     {
    //         vacuum.transform.position = Vector3.Lerp(startingPos, end, (elapsedTime / duration));
    //         elapsedTime += Time.deltaTime;
    //         yield return new WaitForEndOfFrame();
    //     }
    //     vacuum.transform.position = end;
    // }

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
            while (elapsedTime < duration)
            {
                vacuum.transform.position = Vector3.Lerp(startingPos, target, (elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            vacuum.transform.position = target;
            pathPosition++;
        }
    }
}
