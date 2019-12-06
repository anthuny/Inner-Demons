using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryRoom : MonoBehaviour
{
    private Gamemode gm;
    private AudioManager am;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<Gamemode>();
        am = FindObjectOfType<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            //Debug.Log("Setting inmemoryroom to true");
            gm.inMemoryRoom = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            //Debug.Log("stopping mem song");
            gm.inMemoryRoom = false;
            am.enteredMemRoom = false;
            am.StopPlaying("TalkingMusic");
            am.Play("MainMusic");
        }
    }
}
