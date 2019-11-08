using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemode : MonoBehaviour
{
    private GameObject player;
    private Transform spawnPos;
    public GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        spawnPos = GameObject.Find("EGOSpawnPoint").transform;
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.Find("Player");

        if (player == null)
        {
            if (Input.GetKeyDown("r"))
            {
                Instantiate(playerPrefab, spawnPos.position, Quaternion.identity);
                player = GameObject.Find("Player");
            }
        }
    }
}
