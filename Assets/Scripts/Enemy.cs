using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
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

        //Increase enemy count
        gm.enemyCount++;

        //Change name of enemy, including the enemy count
        name = "Enemy " + gm.enemyCount;

        sr = GetComponentInChildren<SpriteRenderer>();
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();

        Reset();
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
        Evade();
        LookAt();
        StartCoroutine("Shoot");
        ElementManager();
        AllowBossDialogue();

        Vector3 z;
        z = transform.position;
        z.z = 0f;
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

            //If in shooting range, shoot
            if (targetInShootRange)
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

        if (targetInViewRange)
        {
            //If not in shooting range, chase
            if (distance > gm.e_BulletDist)
            {
                targetInShootRange = false;

                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, gm.e_ChaseSpeed * Time.deltaTime);
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
            random = Random.Range(1, 3);
        }

        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);

            //If in shooting range, stop chasing and begin evading
            if (distance <= gm.e_BulletDist)
            {
                targetInShootRange = true;

                if (random == 1)
                {
                    transform.Translate(Vector2.up * Time.deltaTime * gm.e_EvadeSpeed, Space.Self);
                }

                if (random == 2)
                {
                    transform.Translate(Vector2.down * Time.deltaTime * gm.e_EvadeSpeed, Space.Self);
                }
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

////        if (player != null)
//        {
//            float distance = Vector2.Distance(transform.position, player.transform.position);

//            //Is the player further away then e_ViewDis?
//            if (distance >= e_ViewDis)
//            {
//                targetInViewRange = false;
//            }
//            else
//            {
//                targetInViewRange = true;
//            }

//            if (!targetInViewRange)
//            {
//                Vector2 destination = patrolPoints[currentPoint].position;
//transform.right = player.transform.position - (patrolPoints[currentPoint].position);

//                transform.position = Vector2.MoveTowards(transform.position, destination, e_MoveSpeed* Time.deltaTime);

//                // Compare how far we are to the destination.
//                float distanceToDestination = Vector3.Distance(transform.position, destination);
//                if (distanceToDestination< 0.2f) // 0.2 is tolerance value.
//                {
//                    // So, we have reached the destination.

//                    // Set the next waypoint.

//                    if (isMovingForward)
//                        currentPoint++;
//                    else // we are moving backward
//                        currentPoint--;

//                    if (currentPoint >= patrolPoints.Length)
//                    {// We have reached the last waypoint, now go backward.
//                        isMovingForward = false;
//                        currentPoint = patrolPoints.Length - 2;
//                    }

//                    if (currentPoint< 0)
//                    {// We have reached the first waypoint, now go forward.
//                        isMovingForward = true;
//                        currentPoint = 1;
//                    }
//                }
//            }
//        }
