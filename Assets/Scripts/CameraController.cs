using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float camHeight;
    private Gamemode gm;

    // Update is called once per frame

    private void Start()
    {
        gm = FindObjectOfType<Gamemode>();
    }
    void Update()
    {
        if (gm.player)
        {
            transform.position = new Vector3(gm.player.transform.position.x, gm.player.transform.position.y, camHeight);
        }

    }
}
