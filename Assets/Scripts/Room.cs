 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private Transform[] enemySpawns;
    public GameObject enemyPrefab;
    public GameObject doors;
    public int roomEnemyCount = 0;
    public bool beenEntered;
    public float spawnWait;
    public bool enemiesHaveSpawned;
    
    // Start is called before the first frame update
    void Start()
    {
        doors.SetActive(false);
    }

    public IEnumerator SpawnEnemies()
    {
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
            doors.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
