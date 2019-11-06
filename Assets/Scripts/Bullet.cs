using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Player playerScript;
    private GameObject player;
    public GameObject enemy;
    private Vector3 forward;
    public bool playerBullet;
    private Vector3 playerPos;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerScript = GameObject.Find("Player").GetComponent<Player>();
        enemy = GameObject.Find("Enemy 1");

        forward = player.transform.forward;

        //Invoke("Death", playerScript.bulletLife);
        playerPos = player.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += forward * Time.deltaTime * playerScript.bulletSpeed;

        //If bullet distance goes too far
        float distance = Vector3.Distance(playerPos, transform.position);
        if (playerScript.bulletDist <= distance)
        {
            Death();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            enemy.GetComponent<Enemy>().DecreaseHealth(playerScript.bulletDamage);
            Death();
        }
    }

    void Death()
    {
        Destroy(gameObject);
    }
}
