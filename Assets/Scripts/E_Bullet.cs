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
    private Vector2 diff;

    private void Awake()
    {
        gm = FindObjectOfType<Gamemode>();
        animator = GetComponentInChildren<Animator>();
        SetElement();
    }

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;
        player = GameObject.Find("Player");
        playerScript = GameObject.Find("Player").GetComponent<Player>();
        playerRb = player.GetComponent<Rigidbody2D>();
        playerPos = player.transform.position;
        playerVel = playerRb.velocity;
        sr = GetComponentInChildren<SpriteRenderer>();


        IncreaseSize();
        AimBotStart();
    }

    void SetElement()
    {
        // Check what element the enemy that spawned this bullet is
        // And set this bullet to the same element
        if (gm.e_IsFire)
        {
            animator.SetInteger("curElement", 1);
        }

        else if (gm.e_IsWater)
        {
            animator.SetInteger("curElement", 0);
        }

        else if (gm.e_IsEarth)
        {
            animator.SetInteger("curElement", 2);
        }
    }

    void Update()
    {
        AimBot();
        DistanceCheck();
        IncreaseSize();
    }

    // Exists so that the rotation is only set once.
    void AimBotStart()
    {
        if (!enemy.GetComponent<Enemy>().isBoss)
        {
            aimPos = playerPos;
            dir = aimPos - startingPos;
            dir.Normalize();
            Vector2 pos1;
            pos1.x = transform.position.x;
            pos1.y = transform.position.y;

            pos1.x += dir.x * gm.e_BulletSpeed * Time.deltaTime;
            pos1.y += dir.y * gm.e_BulletSpeed * Time.deltaTime;

            transform.position = pos1;

            //Always look at player
            Vector2 diff1 = dir;
            diff1.Normalize();

            float rot_z1 = Mathf.Atan2(diff1.y, diff1.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z1 + 90);
        }
        else
        {
            //Aim bot (make sure this happens at least once before look at target (this also happens in update)
            aimPos = playerPos + playerVel;
            dir = aimPos - startingPos;
            dir.Normalize();
            Vector2 pos2;
            pos2.x = transform.position.x;
            pos2.y = transform.position.y;

            pos2.x += dir.x * gm.e_BulletSpeed * Time.deltaTime;
            pos2.y += dir.y * gm.e_BulletSpeed * Time.deltaTime;

            transform.position = pos2;

            //Always look at player
            Vector2 diff2 = dir;
            diff2.Normalize();

            float rot_z2 = Mathf.Atan2(diff2.y, diff2.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z2 + 90);
        }
    }
    void AimBot()
    {
        // If enemy dies while projectile is in flight
        if (!enemy)
        {
            //Aim bot 
            aimPos = playerPos;
            dir = aimPos - startingPos;
            dir.Normalize();
            Vector2 pos;
            pos.x = transform.position.x;
            pos.y = transform.position.y;

            pos.x += dir.x * (gm.e_BulletSpeed * gm.bossBulletSpeedCur) * Time.deltaTime;
            pos.y += dir.y * (gm.e_BulletSpeed * gm.bossBulletSpeedCur) * Time.deltaTime;

            transform.position = pos;
        }

        // If enemy is still alive when projectile is in flight
        else
        {
            if (enemy.GetComponent<Enemy>().isBoss)
            {
                //Aim bot 
                aimPos = playerPos + playerVel;
                dir = aimPos - startingPos;
                dir.Normalize();
                Vector2 pos;
                pos.x = transform.position.x;
                pos.y = transform.position.y;

                pos.x += dir.x * (gm.e_BulletSpeed * gm.bossBulletSpeedCur) * Time.deltaTime;
                pos.y += dir.y * (gm.e_BulletSpeed * gm.bossBulletSpeedCur) * Time.deltaTime;

                transform.position = pos;
            }

            else
            {
                //Aim bot 
                aimPos = playerPos;
                dir = aimPos - startingPos;
                dir.Normalize();
                Vector2 pos;
                pos.x = transform.position.x;
                pos.y = transform.position.y;

                pos.x += dir.x * (gm.e_BulletSpeed * gm.bossBulletSpeedCur) * Time.deltaTime;
                pos.y += dir.y * (gm.e_BulletSpeed * gm.bossBulletSpeedCur) * Time.deltaTime;

                transform.position = pos;
            }
        }
        
    }

    void DistanceCheck()
    {
        if (enemy)
        {
            enemyPos = enemy.transform.position;
        }

        //If bullet distance goes too far, kill it
        float distance = Vector3.Distance(enemyPos, transform.position);
        if ((gm.e_BulletDist * gm.bossBulletDistCur) <= distance)
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
            x += (0.1f * Time.deltaTime * gm.e_IncScaleRate) * gm.bossBulletIncScaleRateCur;
            y += (0.1f * Time.deltaTime * gm.e_IncScaleRate) * gm.bossBulletIncScaleRateCur;

            //Increase the size of the light
            GetComponentInChildren<HardLight2D>().Range += 0.75f * gm.bossBulletIncScaleRateCur;

            // If projectile size has reached it's max scale, stop increasing size.
            if (x >= (gm.e_MaxScaleX + (gm.e_BulletDamage * gm.bossBulletDamageCur * gm.bossBulletSizeInfCur * gm.bossEnragedBulletSizeInfCur * gm.e_bulletSize)) 
                || y >= (gm.e_MaxScaleY * gm.e_BulletDamage * gm.bossBulletDamageCur * gm.bossBulletSizeInfCur * gm.bossEnragedBulletSizeInfCur * gm.e_bulletSize))
            {
                incSize = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "PlayerHitbox")
        {
            player.GetComponent<Player>().DecreaseHealth(gm.e_BulletDamage * gm.bossBulletDamageCur);
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
