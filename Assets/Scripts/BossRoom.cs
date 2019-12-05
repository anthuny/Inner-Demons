using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : MonoBehaviour
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
            gm.inBossRoom = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            gm.inBossRoom = false;
            am.enteredBossRoom = false;
            am.StopPlaying("BattleMusic");
            am.Play("MainMusic");
        }
    }
}
