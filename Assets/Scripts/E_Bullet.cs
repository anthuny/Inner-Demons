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
    private Animator animator;
    private bool incSize = true;
    private float x = .1f;
    private float y = .1f;

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
        animator = GetComponentInChildren<Animator>();

        sr = GetComponentInChildren<SpriteRenderer>();

        SetElement();


        //Increase the size of the bullet based on the damage of the enemy
        transform.localScale = new Vector2(1, 1) * gm.e_BulletDamage / gm.e_BulletScaleInc;

        // If enemy could see target, allow it to pass through obstacles
        if (enemy.GetComponent<Enemy>().e_CanSeeTarget)
        {
            passThrough = true;
        }
        else
        {
            passThrough = false;
        }

        //Aim bot (make sure this happens at least once before look at target (this also happens in update)
        aimPos = playerPos + playerVel / 2;
        dir = aimPos - startingPos;
        dir.Normalize();
        Vector2 pos;
        pos.x = transform.position.x;
        pos.y = transform.position.y;

        pos.x += dir.x * gm.e_BulletSpeed * Time.deltaTime;
        pos.y += dir.y * gm.e_BulletSpeed * Time.deltaTime;

        transform.position = pos;

        //Always look at player
        Vector2 diff2 = dir;
        diff2.Normalize();

        float rot_z = Mathf.Atan2(diff2.y, diff2.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z + 90);
    }

    void SetElement()
    {
        // Check what element the enemy that spawned this bullet is
        // And set this bullet to the same element
        if (enemy.GetComponent<Enemy>().isFire)
        {
            animator.SetInteger("curElement", 1);
        }

        else if (enemy.GetComponent<Enemy>().isWater)
        {
            animator.SetInteger("curElement", 0);
        }

        else if (enemy.GetComponent<Enemy>().isEarth)
        {
            animator.SetInteger("curElement", 2);
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

        if (enemy)
        {
            enemyPos = enemy.transform.position;
        }

        //If bullet distance goes too far
        float distance = Vector3.Distance(enemyPos, transform.position);
        if (gm.e_BulletDist <= distance)
        {
            Death();
        }

        IncreaseSize();
    }

    void IncreaseSize()
    {
        if (incSize)
        {
            transform.localScale = new Vector2(x, y);

            // Increase the scale of the projectile
            x += .1f * Time.deltaTime * gm.p_IncScaleRate;
            y += .1f * Time.deltaTime * gm.p_IncScaleRate;

            // If projectile size has reached it's max scale, stop increasing size.
            if (x >= gm.e_MaxScaleX + gm.e_BulletDamage / 5 || y >= gm.e_MaxScaleY + gm.e_BulletDamage / 5)
            {
                incSize = false;
            }
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
