using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartCamera : MonoBehaviour
{
    public List<Transform> targets;

    public Vector3 offset;

    private Gamemode gm;
    public bool beenAdded;

    private Vector3 velocity;

    private void Start()
    {
        gm = FindObjectOfType<Gamemode>();
    }
    private void Update()
    {
        if (gm.player)
        {
            targets[0] = gm.player.transform;
        }

        if (gm.nearestEnemy && !beenAdded)
        {
            if (targets.Count < 2)
            {
                beenAdded = true;
                targets.Add(gm.nearestEnemy.transform);
            }
        }

        if (targets.Count > 1 && !gm.player)
        {

        }
    }

    private void LateUpdate()
    {
        if (targets.Count == 0)
        {
            return;
        }

        Vector3 centerPoint = GetCenterPoint();

        Vector3 newPosition = centerPoint + offset;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, gm.camSmoothTime);
    }

    Vector3 GetCenterPoint()
    {
        if (targets.Count == 1)
        {
            return targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }
}
