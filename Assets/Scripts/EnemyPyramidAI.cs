using System.Collections;
using UnityEngine;
using Pathfinding;

public enum PyramidState
{
    MOVING,
    SHOOTING,
    WAITING,
    SEARCHING
}

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class EnemyPyramidAI : MonoBehaviour
{
    //What to chase?
    private Transform target;
    public string targetTag;

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

    //The max distance from the AI to the waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 3f;

    //Waypoint we are currently moving towards
    private int currentWaypoint = 0;

    private bool searchingFlorPlayer = false;

    //Current enemy state
    private PyramidState _state;
    public float nextActionWaitSeconds = 3f;
    public float nextPathMoveTimeoutSeconds = 3f;
    private float _timeoutCounter;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        _state = PyramidState.SEARCHING;       
    }    

    private void FindTarget()
    {
        GameObject _target = GameObject.FindGameObjectWithTag(targetTag);
        if (_target != null)
            target = _target.transform;
    }

    private void Shoot()
    {
        Debug.Log("Pyramid shot a beam!");
        _state = PyramidState.WAITING;
        return;
    }

    private void SearchTarget()
    {
        // Check if there's a target
        if (target == null)
        {
            FindTarget();
            return;
        }

        //Start a new path to the target position, return the result to a callback
        seeker.StartPath(transform.position, target.position, OnPathComplete);
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
            _state = PyramidState.MOVING;
        }
    }

    private void MoveToTarget()
    {
        //TODO: Always look at player

        // Check if the move timeout was reached
        _timeoutCounter += Time.deltaTime;
        if(_timeoutCounter > nextPathMoveTimeoutSeconds)
        {
            _timeoutCounter = 0f;
            _state = PyramidState.SHOOTING;            
            return;
        }

        // If there's no path, search again
        if (path == null)
        {
            _state = PyramidState.SEARCHING;
            return;
        }          

        // Check if path was reached
        if (currentWaypoint >= path.vectorPath.Count)
        {
            path = null;
            _timeoutCounter = 0f;
            _state = PyramidState.SHOOTING;
            return;
        }
        

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

    private void WaitForNextAction()
    {
        _timeoutCounter += Time.deltaTime;
        if (_timeoutCounter > nextActionWaitSeconds)
        {
            _timeoutCounter = 0;
            _state = PyramidState.SEARCHING;
            return;
        }       
        
    }

    private void FixedUpdate()
    {     
        // Check state
        switch (_state)
        {
            case PyramidState.SEARCHING:
                // Get a path to target position
                SearchTarget();
                break;
            case PyramidState.MOVING:
                // Move to target position
                MoveToTarget();
                break;
            case PyramidState.SHOOTING:
                // Shoot at target direction
                Shoot();
                break;
            case PyramidState.WAITING:
                // Wait for some seconds before moving again
                WaitForNextAction();
                break;
        }        
    }
}
