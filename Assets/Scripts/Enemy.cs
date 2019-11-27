using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Pathfinding;

public class Enemy : MonoBehaviour
{

    //AI
    private Rigidbody2D rb;
    [HideInInspector]
    public Seeker seeker;

    //Bullet
    private bool e_HasShot;
    private float e_ShotTimer = 0f;
    public GameObject bullet;
    public Transform e_GunHolder;

    //Evading
    private float random;

    public Image e_HealthBar;
    public Image e_HealthBarBG;

    public float e_CurHealth;
    private GameObject player;
    private Player playerScript;
    private bool targetInViewRange;
    private bool targetInShootRange;
    private bool alreadyChosen;
    private float timer;

    private SpriteRenderer sr;

    //Elements
    public bool isFire;
    public bool isWater;
    public bool isEarth;

    //Room
    public GameObject room;

    //Boss
    public bool isBoss;
    public bool bossDialogueReady;
    private bool changedTag;
    public GameObject memoryRange;

    //Animation
    public Animator animator;

    private Gamemode gm;

    void Start()
    {
        gm = GameObject.Find("EGOGamemode").GetComponent<Gamemode>();
        rb = GetComponent<Rigidbody2D>();

        //Increase enemy count
        gm.enemyCount++;

        //Change name of enemy, including the enemy count
        name = "Enemy " + gm.enemyCount;

        sr = GetComponentInChildren<SpriteRenderer>();
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();


        seeker = GetComponent<Seeker>();

        InvokeRepeating("UpdatePath", 0f, gm.navUpdateTimer);

        Reset();
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, gm.player.transform.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            gm.path = p;
            gm.currentWaypoint = 0;
        }
    }

    void Move()
    {
        if (gm.path == null)
        {
            return;
        }

        if (gm.currentWaypoint >= gm.path.vectorPath.Count)
        {
            gm.reachedEndOfPath = true;
            return;
        }
        else
        {
            gm.reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)gm.path.vectorPath[gm.currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * gm.e_MoveSpeed * Time.deltaTime;

        // Move towards player
        if (!targetInShootRange)
        {
            rb.AddForce(force);
        }

        if (!gm.e_CanSeeTarget)
        {
            rb.AddForce(force);
        }

        float distance = Vector2.Distance(rb.position, gm.path.vectorPath[gm.currentWaypoint]);

        if (distance < gm.nexWaypointDistance)
        {
            gm.currentWaypoint++;
        }
    }
    void Reset()
    {
        e_CurHealth = gm.e_MaxHealth;
        e_HealthBar.fillAmount = 1f;
        // Current patrol point
        //currentPoint = 0;

        // Randomly choose an element for the enemy to be
        float randNum;
        randNum = Random.Range(1, 4);

        // If random number is 1, enemy is fire
        if (randNum == 1)
        {
            isEarth = false;
            isWater = false;
            isFire = true;
        }

        // If random number is 2, enemy is water
        if (randNum == 2)
        {
            isEarth = false;
            isFire = false;
            isWater = true;
        }

        // If random number is 3, enemy is earth
        if (randNum == 3)
        {
            isWater = false;
            isFire = false;
            isEarth = true;
        }

    }
    void Update()
    {
        LookAt();
        StartCoroutine("Shoot");
        ElementManager();
        AllowBossDialogue();

        Vector3 z;
        z = transform.position;
        z.z = 0f;
    }

    private void FixedUpdate()
    {
        Move();
        Evade();
    }
    void AllowBossDialogue()
    {
        //Check if this object is a boss, and if it's tag hasn't already been changed
        if (bossDialogueReady)
        {
            memoryRange.SetActive(true);
        }
    }

    void ElementManager()
    {
        if (isFire)
        {
            //sr.color = Color.red;
        }

        if (isWater)
        {
            //sr.color = Color.blue;
        }

        if (isEarth)
        {
            //sr.color = Color.green;
        }
    }

    public void DecreaseHealth(float bulletDamage, string playersCurElement)
    {
        if (e_CurHealth > gm.e_HealthDeath)
        {
            // If player countered the enemy with their hit, take bonus damage
            if (playersCurElement == "Fire" && isEarth)
            {
                e_CurHealth -= bulletDamage + gm.fireDamage;
                e_HealthBar.fillAmount = e_CurHealth / 100;
            }

            // If player countered the enemy with their hit, take bonus damage
            if (playersCurElement == "Water" && isFire)
            {
                e_CurHealth -= bulletDamage + gm.waterDamage;
                e_HealthBar.fillAmount = e_CurHealth / 100;
            }

            // If player countered the enemy with their hit, take bonus damage
            if (playersCurElement == "Earth" && isWater)
            {
                e_CurHealth -= bulletDamage + gm.earthDamage;
                e_HealthBar.fillAmount = e_CurHealth / 100;
            }

            // If there is no element counter, do regular damage
            if (playersCurElement == "Fire" && isWater)
            {
                e_CurHealth -= bulletDamage;
                e_HealthBar.fillAmount = e_CurHealth / 100;
            }

            // If there is no element counter, do regular damage
            if (playersCurElement == "Fire" && isFire)
            {
                e_CurHealth -= bulletDamage;
                e_HealthBar.fillAmount = e_CurHealth / 100;
            }

            // If there is no element counter, do regular damage
            if (playersCurElement == "Water" && isEarth)
            {
                e_CurHealth -= bulletDamage;
                e_HealthBar.fillAmount = e_CurHealth / 100;
            }

            // If there is no element counter, do regular damage
            if (playersCurElement == "Water" && isWater)
            {
                e_CurHealth -= bulletDamage;
                e_HealthBar.fillAmount = e_CurHealth / 100;
            }

            // If there is no element counter, do regular damage
            if (playersCurElement == "Earth" && isFire)
            {
                e_CurHealth -= bulletDamage;
                e_HealthBar.fillAmount = e_CurHealth / 100;
            }

            // If there is no element counter, do regular damage
            if (playersCurElement == "Earth" && isEarth)
            {
                e_CurHealth -= bulletDamage;
                e_HealthBar.fillAmount = e_CurHealth / 100;
            }
        }
        
        // If this object dies
        if (e_CurHealth <= gm.e_HealthDeath && !isBoss)
        {
            //Decrease enemy count
            gm.enemyCount--;

            //Decrease room enemy count
            room.GetComponent<Room>().roomEnemyCount--;

            //Kill enemy
            Destroy(gameObject);
        }

        if (e_CurHealth <= gm.e_HealthDeath && isBoss)
        {
            bossDialogueReady = true;
            //Trigger dialogue system
        }
    }

    void LookAt()
    {
        if (player != null && !bossDialogueReady)
        {
            // Set rotation of gun holder to aim at player position
            // Rotate gun holder
            gm.e_ShootDir = player.transform.position - transform.position;
            gm.e_ShootDir.Normalize();
            e_GunHolder.transform.right = gm.shootDir;
        }

        if (player.transform.position.x > transform.position.x)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = false;
        }

        else
        {
            GetComponentInChildren<SpriteRenderer>().flipX = true;
        }

        RaycastHit2D hitinfo = Physics2D.Linecast(transform.position, player.transform.position, (1 << 11) | (1 << 14));

        // See if can see target
        if (hitinfo.transform.tag != "Player")
        {
            //Debug.DrawLine(transform.position, gm.player.transform.position, Color.red, .1f);
            //Debug.Log("NOT hitting player, hitting: " + hitinfo.transform.tag);
            gm.e_CanSeeTarget = false;
        }

        else
        {
            //Debug.DrawLine(transform.position, gm.player.transform.position, Color.green, .1f);
            //Debug.Log("Hitting player, hitting: " + hitinfo.transform.tag);
            gm.e_CanSeeTarget = true;
        }
    }

    IEnumerator Shoot()
    {
        if (player == null)
        {
            yield return 0;
        }

        float distance = Vector2.Distance(transform.position, player.transform.position);

        //Is the player further away then e_ViewDis?
        if (distance >= gm.e_ViewDis)
        {
            targetInViewRange = false;

            // Play idle animation
            animator.SetInteger("EnemyBrain", 0);
        }
        else
        {
            targetInViewRange = true;
        }

        // If CAN'T shoot (waiting for shot cooldown)
        if (e_HasShot)
        {
            ShotCooldown();
        }

        // If the enemy can shoot, check if enemy has had their tag changed
        // to memory (for boss), if so, don't allow it to shoot
        if (!e_HasShot && targetInViewRange && !bossDialogueReady)
        {
            e_HasShot = true;

            // If in shooting range and can see target, shoot
            if (targetInShootRange && gm.e_CanSeeTarget)
            {
                Instantiate(bullet, e_GunHolder.position, Quaternion.identity);

                // Play shooting animation
                animator.SetInteger("EnemyBrain", 2);

                // Wait x seconds
                yield return new WaitForSeconds(0.25f);

                // Play idle animation
                animator.SetInteger("EnemyBrain", 0);
            }
        }
    }

    void Evade()
    {
        if (timer <= gm.evadetimerMax)
        {
            alreadyChosen = true;
            timer += Time.deltaTime;
        }

        if (timer >= gm.evadetimerMax)
        {
            timer = 0;
            alreadyChosen = false;
        }

        if (!alreadyChosen)
        {
            alreadyChosen = true;
            random = Random.Range(1, 7);
        }

        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);

            // If in shooting range, stop chasing and begin evading
            // and if it can see target
            if (distance <= gm.e_BulletDist - gm.e_rangeOffset && gm.e_CanSeeTarget)
            {
                targetInShootRange = true;

                // Evade Left
                if (random == 1 || random == 6)
                {
                    rb.AddRelativeForce(e_GunHolder.transform.up * gm.e_EvadeSpeed * Time.deltaTime);
                }

                // Evade Right
                if (random == 2 || random == 5)
                {
                    rb.AddRelativeForce(-e_GunHolder.transform.up * gm.e_EvadeSpeed * Time.deltaTime);
                }

                // Evade forwards
                if (random == 3)
                {
                    rb.AddRelativeForce(-e_GunHolder.transform.right * gm.e_EvadeSpeed * Time.deltaTime);
                }

                // Evade Backwards
                if (random == 4)
                {
                    rb.AddRelativeForce(e_GunHolder.transform.right * gm.e_EvadeSpeed * Time.deltaTime);
                }
            }
            else
            {
                targetInShootRange = false;
            }
        }
    }

    void ShotCooldown()
    {
        if (e_ShotTimer <= gm.e_ShotCooldown)
        {
            e_ShotTimer += Time.deltaTime;
        }

        if (e_ShotTimer >= gm.e_ShotCooldown)
        {
            e_HasShot = false;
            e_ShotTimer = 0;
        }
    }

}
