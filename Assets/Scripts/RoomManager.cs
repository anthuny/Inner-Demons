using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject[] doors;

    // Start is called before the first frame update
    void Start()
    {
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
