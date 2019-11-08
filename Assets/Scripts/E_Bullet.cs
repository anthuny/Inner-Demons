using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Bullet : MonoBehaviour
{
    private Enemy enemyScript;
    private GameObject player;
    private Player playerScript;
    public GameObject enemy;
    private Vector2 forward;
    public bool playerBullet;
    private Vector2 enemyPos;
    private Vector2 playerPos;
    private Rigidbody2D playerRb;
    private Vector2 startingPos;
    private Vector2 aimPos;
    private Vector2 dir;
    private Vector2 playerVel;


    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        player = GameObject.Find("Player");
        playerScript = GameObject.Find("Player").GetComponent<Player>();
        enemy = GameObject.Find("Enemy 1");
        enemyScript = GameObject.Find("Enemy 1").GetComponent<Enemy>();
        playerRb = player.GetComponent<Rigidbody2D>();
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
        Vector2 pos;
        pos.x = transform.position.x;
        pos.y = transform.position.y;

        pos.x += dir.x * enemyScript.e_BulletSpeed * Time.deltaTime;
        pos.y += dir.y * enemyScript.e_BulletSpeed * Time.deltaTime;

        transform.position = pos;

        //transform.position.x = position.x;
        //transform.position.y = position.y;

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
