using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Bullet : MonoBehaviour
{
    private Enemy enemyScript;
    private GameObject player;
    private Player playerScript;
    public GameObject enemy;
    public bool playerBullet;
    private Vector2 enemyPos;
    private Vector2 playerPos;
    private Rigidbody2D playerRb;
    private Vector2 startingPos;
    private Vector2 aimPos;
    private Vector2 dir;
    private Vector2 playerVel;
    private SpriteRenderer sr;
    private Gamemode gm;
    private bool passThrough;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        player = GameObject.Find("Player");
        playerScript = GameObject.Find("Player").GetComponent<Player>();
        playerRb = player.GetComponent<Rigidbody2D>();
        playerPos = player.transform.position;      
        playerVel = playerRb.velocity;
        gm = FindObjectOfType<Gamemode>();

        // Exists so each spawned bullet has a reference to the enemy that spawned it.
        // Allows multiple ranged enemies to be attacking at a time
        GameObject[] enemies;
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector2 position = transform.position;
        foreach (GameObject go in enemies)
        {
            Vector2 diff = new Vector2(go.transform.position.x, go.transform.position.y) - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }

        enemy = closest;
        enemyScript = closest.GetComponent<Enemy>();

        enemyPos = enemy.transform.position;

        sr = GetComponentInChildren<SpriteRenderer>();

        // Check what element the enemy that spawned this bullet is
        // And set this bullet to the same element
        if (enemy.GetComponent<Enemy>().isFire)
        {
            sr.color = Color.red;
        }

        if (enemy.GetComponent<Enemy>().isWater)
        {
            sr.color = Color.blue;
        }

        if (enemy.GetComponent<Enemy>().isEarth)
        {
            sr.color = Color.green;
        }


        //Increase the size of the bullet based on the damage of the enemy
        transform.localScale = new Vector2(1, 1) * gm.e_BulletDamage / gm.e_BulletScaleInc;

        // If enemy could see target, allow it to pass through obstacles
        if (gm.e_CanSeeTarget)
        {
            passThrough = true;
        }
        else
        {
            passThrough = false;
        }
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

        pos.x += dir.x * gm.e_BulletSpeed * Time.deltaTime;
        pos.y += dir.y * gm.e_BulletSpeed * Time.deltaTime;

        transform.position = pos;

        //If bullet distance goes too far
        float distance = Vector3.Distance(enemyPos, transform.position);
        if (gm.e_BulletDist <= distance)
        {
            Death();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlayerHitbox")
        {
            player.GetComponent<Player>().DecreaseHealth(gm.e_BulletDamage);
            Death();
        }

        if (!passThrough)
        {
            if (other.tag == "Wall")
            {
                Death();
            }
        }
    }
    void Death()
    {
        Destroy(gameObject);
    }
}
