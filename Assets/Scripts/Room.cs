 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private Transform[] enemySpawns;
    public GameObject enemyPrefab;
    private GameObject[] doors;
    private GameObject[] enemies;
    public int roomEnemyCount = 0;
    public bool beenEntered;
    public float spawnWait;
    public bool enemiesHaveSpawned;
    public bool doorsClosed;
    public bool isBossRoom;
    private GameDownManager gdm;
    private bool doneOnce;
    private bool playerDied;
    public Room room;

    private void Start()
    {
        gdm = FindObjectOfType<GameDownManager>();

        // Doing this to get a reference to the room 
        // only this script is on. NOT all rooms
        room = GetComponent<Room>();
    }

    public IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(spawnWait);
        room.enemySpawns = GetComponentsInChildren<Transform>();

        foreach (Transform i in enemySpawns)
        {
            //Make sure not to spawn an enemy for the root, only children
            if (i != this.gameObject.transform && i.tag == "SpawnPoint")
            {
                // Spawning regular enemies
                if (!room.isBossRoom)
                {
                    GameObject go = Instantiate(enemyPrefab, i.position, Quaternion.identity);
                    go.GetComponent<Enemy>().room = gameObject;
                    room.roomEnemyCount++;
                    room.enemiesHaveSpawned = true;
                }

                //Spawning boss
                else
                {
                    GameObject go = Instantiate(enemyPrefab, i.position, Quaternion.identity);
                    go.GetComponent<Enemy>().room = gameObject;
                    go.GetComponent<Enemy>().isBoss = true;
                    go.GetComponent<Memory>().enabled = true;
                    room.roomEnemyCount++;
                    room.enemiesHaveSpawned = true;
                }
            }
        }
    }
    public void CloseDoors()
    {
        room.doors = FindObjectOfType<RoomManager>().doors;
        room.doorsClosed = true;

        // For each door, close it
        for (int f = 0; f < doors.Length; f++)
        {
            room.doors[f].SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gdm.playerDied)
        {
            room.playerDied = true;
            Reset();
        }

        else
        {
            room.playerDied = false;
        }

        room.doors = FindObjectOfType<RoomManager>().doors;

        if (room.beenEntered && room.roomEnemyCount > 0 && room.enemiesHaveSpawned)
        {
            for (int i = 0; i < doors.Length; i++)
            {
                Debug.Log("closing doors");
                doors[i].SetActive(true);
            }
        }

        else
        {
            for (int i = 0; i < doors.Length; i++)
            {
                Debug.Log("opening doors 1");
                doors[i].SetActive(false);
            }
        }
    }

    // If player dies...
    private void Reset()
    {
        room.beenEntered = false;
        room.doorsClosed = false;

        room.enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
            GameObject.Destroy(enemy);

        room.roomEnemyCount = 0;

        doors = FindObjectOfType<RoomManager>().doors;

        for (int i = 0; i < doors.Length; i++)
        {
            Debug.Log("opening doors 2");
            doors[i].SetActive(false);
        }
    }
}
