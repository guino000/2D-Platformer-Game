using System.Collections;
using UnityEngine;
using Pathfinding;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class EnemyAI : MonoBehaviour
{
    //What to chase?
    public Transform target;

    //How many times each second we are going to update our path
    public float updateRate = 2f;

    //Caching
    private Seeker seeker;
    private Rigidbody2D rb;

    //The calculated path
    public Path path;

    //The AI Speed per second
    public float speed = 300f;
    public ForceMode2D fMode;

    [HideInInspector]
    public bool pathIsEnded = false;

    //The max distance from the AI to the waipoint for it to continue to the next wazpoint
    public float nextWaypointDistance = 3f;

    //Waypoint we are currently moving towards
    private int currentWaypoint = 0;

    private bool searchingFlorPlayer = false;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        if(target == null)
        {
            if (!searchingFlorPlayer)
            {
                searchingFlorPlayer = true;
                StartCoroutine(SearchForPlayer());
            }

            return;
        }

        //Start a new path to the target position, return the result to a callback
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        StartCoroutine(UpdatePath());
    }

    IEnumerator SearchForPlayer()
    {
        GameObject sResult = GameObject.FindGameObjectWithTag("Player");
        if (sResult == null)
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(SearchForPlayer());
        }
        else
        {
            searchingFlorPlayer = false;
            target = sResult.transform;
            StartCoroutine(UpdatePath());
            yield break;
        }
    }

    IEnumerator UpdatePath()
    {
        if(target == null)
        {
            if (!searchingFlorPlayer)
            {
                searchingFlorPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            yield break;
        }

        //Start a new path to the target position, return the result to a callback
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        yield return new WaitForSeconds(1 / updateRate);
        StartCoroutine(UpdatePath());
    }

    public void OnPathComplete(Path p)
    {
        Debug.Log("We got a path. Did it have an error? " + p.error);
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            if (!searchingFlorPlayer)
            {
                searchingFlorPlayer = true;
                StartCoroutine(SearchForPlayer());
            }
            return;
        }

        //TODO: Always look at player

        if(path == null)
        {
            return;
        }

        if(currentWaypoint >= path.vectorPath.Count)
        {
            if (pathIsEnded)
                return;

            Debug.Log("End of path reached;");
            pathIsEnded = true;
            return;
        }
        pathIsEnded = false;

        //Direction to the next waypoint
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= speed * Time.fixedDeltaTime;

        //Move the AI
        rb.AddForce(dir, fMode);

        float dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
        if (dist < nextWaypointDistance)
        {
            currentWaypoint++;
            return;
        }
    }
}
