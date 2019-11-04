using UnityEngine;

public class Enemy : MonoBehaviour
{

    private int currentPoint;
    public Transform[] patrolPoints;
    public float moveSpeed;
    bool isMovingForward = true;

    // Use this for initialization
    void Start()
    {
        //transform.position = patrolPoints[0].position;
        currentPoint = 0;
    }


    void Update()
    {
        Vector3 destination = patrolPoints[currentPoint].position;

        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

        // Compare how far we are to the destination.
        float distanceToDestination = Vector3.Distance(transform.position, destination);
        if (distanceToDestination < 0.2f) // 0.2 is tolerance value.
        {
            // So, we have reached the destination.

            // Set the next waypoint.

            if (isMovingForward)
                currentPoint++;
            else // we are moving backward
                currentPoint--;

            if (currentPoint >= patrolPoints.Length)
            {// We have reached the last waypoint, now go backward.
                isMovingForward = false;
                currentPoint = patrolPoints.Length - 2;
            }

            if (currentPoint < 0)
            {// We have reached the first waypoint, now go forward.
                isMovingForward = true;
                currentPoint = 1;
            }
        }
    }
}
