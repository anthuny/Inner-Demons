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
    private SpriteRenderer sr;
    private string currentElement;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerScript = GameObject.Find("Player").GetComponent<Player>();
        enemy = GameObject.Find("Enemy 1");

        forward = player.transform.right;

        playerPos = player.transform.position;

        sr = GetComponentInChildren<SpriteRenderer>();

        // Check what element the enemy that spawned this bullet is
        // And set this bullet to the same element
        if (player.GetComponent<Player>().isFire)
        {
            sr.color = Color.red;
            currentElement = "Fire";
        }

        if (player.GetComponent<Player>().isWater)
        {
            sr.color = Color.blue;
            currentElement = "Water";
        }

        if (player.GetComponent<Player>().isEarth)
        {
            sr.color = Color.green;
            currentElement = "Earth";
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += forward * Time.deltaTime * playerScript.bulletSpeed;

        //If bullet distance goes too far
        float distance = Vector2.Distance(playerPos, transform.position);
        if (playerScript.bulletDist <= distance)
        {
            Death();
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            other.GetComponent<Enemy>().DecreaseHealth(playerScript.bulletDamage, currentElement);
            Death();
        }

    }

    void Death()
    {
        Destroy(gameObject);
    }
}
