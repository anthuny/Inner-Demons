using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartCamera : MonoBehaviour
{
    public Vector3 offset;

    private Gamemode gm;

    private Vector3 velocity;
    private Vector3 newPosition;
    private Vector3 distanceAdded;

    private void Start()
    {
        gm = FindObjectOfType<Gamemode>();
    }

    private void LateUpdate()
    {
        newPosition = distanceAdded + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, gm.camSmoothTime);

        GetCenterPoint();
    }

    void GetCenterPoint()
    {
        // If player exists, and there is NOT and enemy
        if (gm.player && !gm.nearestEnemy)
        {
            distanceAdded = gm.player.transform.position;
        }

        // If player exists, and there IS an enemy 
        if (gm.player && gm.nearestEnemy)
        {
            distanceAdded = gm.player.transform.position + gm.nearestEnemy.transform.position;
            distanceAdded /= 2;
        }
    }
}
