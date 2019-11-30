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
    private Animator animator;
    private bool incSize = true;
    private float x = .1f;
    private float y = .1f;
    // Start is called before the first frame update
    void Start()
    {
        gm = FindObjectOfType<Gamemode>();
        rb = GetComponent<Rigidbody2D>();
        gamemode = FindObjectOfType<Gamemode>();
        player = GameObject.Find("Player");
        playerScript = GameObject.Find("Player").GetComponent<Player>();
        enemy = GameObject.Find("Enemy 1");

        animator = GetComponentInChildren<Animator>();

        //forward = playerScript.gunHolder.transform.localEulerAngles;
        //Debug.Log(forward);

        playerPos = player.transform.position;

        sr = GetComponentInChildren<SpriteRenderer>();

        //Set correct element visually.
        SetElement();

        // If enemy could see target, allow it to pass through obstacles
        if (gm.p_CanSeeTarget && gm.nearestEnemy)
        {
            passThrough = true;
        }
        else
        {
            passThrough = false;
        }
    }

    void SetElement()
    {
        // Check what element the enemy that spawned this bullet is
        // And set this bullet to the same element
        if (gamemode.isFire)
        {
            //Change visual colour
            animator.SetInteger("curElement", gm.currentElement);

            currentElement = "Fire";
        }

        if (gamemode.isWater)
        {
            //Change visual colour
            animator.SetInteger("curElement", gm.currentElement);

            currentElement = "Water";
        }

        if (gamemode.isEarth)
        {
            //Change visual colour
            animator.SetInteger("curElement", gm.currentElement);

            currentElement = "Earth";
        }
    }
    // Update is called once per frame
    void Update()
    {
        // Increase the projectile's size
        IncreaseSize();

        // Move the projectile forwards
        transform.Translate(Vector2.right * Time.deltaTime * gamemode.bulletSpeed);

        //If bullet distance goes too far
        float distance = Vector2.Distance(playerPos, transform.position);
        if (gamemode.bulletDist <= distance)
        {
            Death();
        }

    }

    void IncreaseSize()
    {
        if (incSize)
        {
            transform.localScale = new Vector2(x, y);

            // Increase the scale of the projectile
            x += .1f * Time.deltaTime * gm.incScaleRate;
            y += .1f * Time.deltaTime * gm.incScaleRate;

            // If projectile size has reached it's max scale, stop increasing size.
            if (x >= gm.maxScaleX + gm.bulletDamage / 5 || y >= gm.maxScaleY + gm.bulletDamage / 2)
            {
                incSize = false;
            }
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
            if (other.tag == "Wall" || other.tag == "Doors" || other.tag == "Obstacle")
            {
                Death();
            }
        }
    }

    void Death()
    {
        //Spawn particle effect
        Instantiate(gm.bulletDeathParticle, transform.position, Quaternion.identity);

        ParticleSystem ps = gm.bulletDeathParticle.GetComponent<ParticleSystem>();

        ParticleSystem.MainModule ma = ps.main;

        if (currentElement == "Fire")
        {
            ma.startColor = new ParticleSystem.MinMaxGradient(new Color32(255, 59, 59, 255));
        }

        if (currentElement == "Water")
        {
            ma.startColor = new ParticleSystem.MinMaxGradient(new Color32(76, 189, 255, 255));
        }

        if (currentElement == "Earth")
        {
            ma.startColor = new ParticleSystem.MinMaxGradient(new Color32(66, 255, 85, 255));
        }


        // Destroy projetile
        Destroy(gameObject);
    }
}
 