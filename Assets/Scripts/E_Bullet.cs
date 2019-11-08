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
        playerRb = player.GetComponent<Rigidbody2D>();
        playerPos = player.transform.position;      
        playerVel = playerRb.velocity;

        // Exists so each spawned bullet has a reference to the enemy that spawned it.
        // Allows multiple ranged enemies to be attacking at a time
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector2 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector2 diff = new Vector2(go.transform.position.x, go.transform.position.x) - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }

        enemy = closest;
        enemyScript = closest.GetComponent<Enemy>();

        forward = enemy.transform.forward;
        enemyPos = enemy.transform.position;
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

        //If bullet distance goes too far
        float distance = Vector3.Distance(enemyPos, transform.position);
        if (enemyScript.e_BulletDist <= distance)
        {
            Death();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            player.GetComponent<Player>().DecreaseHealth(enemyScript.e_BulletDamage);
            Death();
        }

        if (other.tag == "Enemy")
        {
            Debug.Log(enemy);
            enemy = other.gameObject;
            enemyScript = enemy.GetComponent<Enemy>();
        }
    }
    void Death()
    {
        Destroy(gameObject);
    }
}
