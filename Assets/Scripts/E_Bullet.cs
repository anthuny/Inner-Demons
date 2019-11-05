using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Bullet : MonoBehaviour
{
    private Enemy enemyScript;
    private GameObject player;
    private Player playerScript;
    public GameObject enemy;
    private Vector3 forward;
    public bool playerBullet;
    private Vector3 enemyPos;
    private Vector3 playerPos;
    private Rigidbody playerRb;
    private Vector3 startingPos;
    private Vector3 aimPos;
    private Vector3 dir;
    private Vector3 playerVel;


    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        player = GameObject.Find("Player");
        playerScript = GameObject.Find("Player").GetComponent<Player>();
        enemy = GameObject.Find("Enemy 1");
        enemyScript = GameObject.Find("Enemy 1").GetComponent<Enemy>();
        playerRb = player.GetComponent<Rigidbody>();
        forward = enemy.transform.forward;
        enemyPos = enemy.transform.position;
        playerPos = player.transform.position;      
        playerVel = playerRb.velocity;
    }

    // Update is called once per frame
    void Update()
    {
        //Aim bot
        aimPos = playerPos + playerVel / 2;
        dir = aimPos - startingPos;
        dir.Normalize();
        transform.position += dir * enemyScript.e_BulletSpeed * Time.deltaTime;

        //If bullet distance goes too far
        float distance = Vector3.Distance(enemyPos, transform.position);
        if (enemyScript.e_BulletDist <= distance)
        {
            Death();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player.GetComponent<Player>().DecreaseHealth(enemyScript.e_BulletDamage);
            Death();
        }
    }
    void Death()
    {
        Destroy(gameObject);
    }
}
