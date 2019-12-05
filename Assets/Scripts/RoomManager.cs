using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject[] doors;
    public GameObject bossHealth;

    // Start is called before the first frame update
    void Start()
    {
        bossHealth = GameObject.Find("EGO Boss Health");
        bossHealth.SetActive(false);

        doors = GameObject.FindGameObjectsWithTag("Doors");
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
