 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

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
    private Gamemode gm;
    private bool doneOnce;
    private bool playerDied;
    public Room room;
    public bool canOpen;
    public bool beenCleared;
    private ScreenShake ss;
    private RoomManager rm;


    private void Start()
    {
        gdm = FindObjectOfType<GameDownManager>();
        gm = FindObjectOfType<Gamemode>();
        ss = FindObjectOfType<ScreenShake>();

        // Doing this to get a reference to the room 
        // only this script is on. NOT all rooms
        room = GetComponent<Room>();
        rm = FindObjectOfType<RoomManager>();
    }

    public IEnumerator SpawnEnemies()
    {
        room.enemiesHaveSpawned = true;

        yield return new WaitForSeconds(spawnWait);
        room.enemySpawns = GetComponentsInChildren<Transform>();

        foreach (Transform i in enemySpawns)
        {
            //Make sure not to spawn an enemy for the root, only children
            if (i != this.gameObject.transform && i.tag == "SpawnPoint")
            {
                int rand = Random.Range(0, 3);

                // Playing enemy spawn SFX
                FindObjectOfType<AudioManager>().Play("EnemySpawn");
                    
                // Spawning regular enemies
                if (!room.isBossRoom)
                {
                    GameObject go = Instantiate(enemyPrefab, i.position, Quaternion.identity);
                    go.GetComponent<Enemy>().room = gameObject;
                    gm.ChooseElement(rand);
                    room.roomEnemyCount++;

                }

                //Spawning boss
                else
                {
                    GameObject go = Instantiate(enemyPrefab, i.position, Quaternion.identity);
                    go.GetComponent<Enemy>().room = gameObject;
                    go.GetComponent<Enemy>().isBoss = true;
                    go.GetComponent<Memory>().enabled = true;
                    rm.bossHealth.SetActive(true);
                    gm.ChooseElement(rand);
                    go.GetComponent<Enemy>().EGOBossHealth = rm.bossHealth;
                    room.roomEnemyCount++;
                }
            }
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

        // Open doors
        if (room.beenEntered && room.roomEnemyCount == 0 && doorsClosed && canOpen)
        {
            for (int i = 0; i < doors.Length; i++)
            {
                room.doorsClosed = false;
                doors[i].SetActive(false);
            }

            // Play audio
            FindObjectOfType<AudioManager>().Play("DoorOpen");
        }
    }

    public IEnumerator TriggerDoorClose()
    {
        // If the player has not already cleared this room,
        // close the doors on entry
        if (!beenCleared)
        {
            // Close doors
            if (room.beenEntered && room.enemiesHaveSpawned && !doorsClosed)
            {
                for (int i = 0; i < doors.Length; i++)
                {
                    //Screen shake
                    ScreenShakeInfo Info = new ScreenShakeInfo();
                    Info.shakeMag = gm.shakeMagDClose;
                    Info.shakeRou = gm.shakeRouDClose;
                    Info.shakeFadeIDur = gm.shakeFadeIDurDClose;
                    Info.shakeFadeODur = gm.shakeFadeODurDClose;
                    Info.shakePosInfluence = gm.shakePosInfluenceDClose;
                    Info.shakeRotInfluence = gm.shakeRotInfluenceDClose;

                    ss.StartShaking(Info, 0.25f, 3);

                    //depth shake
                    gm.depthShaking = true;
                    gm.CameraDepthShake();

                    room.canOpen = false;
                    room.doorsClosed = true;
                    doors[i].SetActive(true);
                }

                // Play audio
                FindObjectOfType<AudioManager>().Play("DoorClose");
            }

            yield return new WaitForSeconds(spawnWait + 0.25f);

            canOpen = true;
        }

    }

    // If player dies...
    public void Reset()
    {
        room.beenCleared = false;
        room.beenEntered = false;
        room.canOpen = false;
        room.doorsClosed = false;
        room.enemiesHaveSpawned = false;

        room.enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
            GameObject.Destroy(enemy);

        room.roomEnemyCount = 0;

        doors = FindObjectOfType<RoomManager>().doors;

        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].SetActive(false);
        }
    }
}

