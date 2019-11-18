 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private Transform[] enemySpawns;
    public GameObject enemyPrefab;
    private GameObject[] doors;
    public int roomEnemyCount = 0;
    public bool beenEntered;
    public float spawnWait;
    private bool enemiesHaveSpawned;
    
    // Start is called before the first frame update
    void Start()
    {
        doors = GameObject.FindGameObjectsWithTag("Doors");
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].SetActive(false);
        }
    }

    public IEnumerator SpawnEnemies()
    {
        // For each door, close it
        for (int f = 0; f < doors.Length; f++)
        {
            doors[f].SetActive(true);
        }

        yield return new WaitForSeconds(spawnWait);
        enemySpawns = GetComponentsInChildren<Transform>();

        foreach (Transform i in enemySpawns)
        {
            //Make sure not to spawn an enemy for the root, only children
            if (i != this.gameObject.transform && i.tag == "SpawnPoint")
            {
                Instantiate(enemyPrefab, i.position, Quaternion.identity);
                roomEnemyCount++;
                enemiesHaveSpawned = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (beenEntered && roomEnemyCount <= 0 && enemiesHaveSpawned)
        {
            for (int i = 0; i < doors.Length; i++)
            {
                doors[i].SetActive(false);
            }
        }
    }
}
