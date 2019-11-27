using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject enemy;
    public bool playerBullet;
    public Transform weaponHolder;

    private Vector3 playerPos;
    private SpriteRenderer sr;
    private string currentElement;
    private Gamemode gamemode;
    private Vector3 forward;
    private Player playerScript;
    private GameObject player;
    private Rigidbody2D rb;
    private Gamemode gm;
    private bool passThrough;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<Gamemode>();
        rb = GetComponent<Rigidbody2D>();
        gamemode = FindObjectOfType<Gamemode>();
        player = GameObject.Find("Player");
        playerScript = GameObject.Find("Player").GetComponent<Player>();
        enemy = GameObject.Find("Enemy 1");

        //forward = playerScript.gunHolder.transform.localEulerAngles;
        //Debug.Log(forward);

        playerPos = player.transform.position;

        sr = GetComponentInChildren<SpriteRenderer>();

        // Check what element the enemy that spawned this bullet is
        // And set this bullet to the same element
        if (gamemode.isFire)
        {
            sr.color = Color.red;
            currentElement = "Fire";
        }

        if (gamemode.isWater)
        {
            sr.color = Color.blue;
            currentElement = "Water";
        }

        if (gamemode.isEarth)
        {
            sr.color = Color.green;
            currentElement = "Earth";
        }

        //Increase the size of the bullet based on the damage of the player
        transform.localScale = new Vector2(5, 5) * gamemode.bulletDamage / 15;

        // If enemy could see target, allow it to pass through obstacles
        if (gm.p_CanSeeTarget)
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
        transform.Translate(Vector2.right * Time.deltaTime * gamemode.bulletSpeed);

        //If bullet distance goes too far
        float distance = Vector2.Distance(playerPos, transform.position);
        if (gamemode.bulletDist <= distance)
        {
            Death();
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            other.GetComponent<Enemy>().DecreaseHealth(gamemode.bulletDamage, currentElement);
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
 