using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Player playerScript;
    private GameObject player;
    private Vector3 forward;
    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<Player>();
        player = GameObject.Find("Player");
        forward = player.transform.forward;

        Invoke("Death", playerScript.bulletLife);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += forward * Time.deltaTime * playerScript.bulletSpeed;
    }

    void Death()
    {
        Destroy(gameObject);
    }
}
