using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    //Bullet
    private bool e_HasShot;
    private float e_ShotTimer = 0f;
    public GameObject bullet;
    public Transform e_GunHolder;

    //Evading
    private float random;

    [SerializeField]
    private Image e_HealthBar;
    [SerializeField]
    public Image e_HealthBarBG;

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

    private Gamemode gamemode;

    void Start()
    {
        gamemode = GameObject.Find("EGOGamemode").GetComponent<Gamemode>();

        //Increase enemy count
        gamemode.enemyCount++;

        //Change name of enemy, including the enemy count
        name = "Enemy " + gamemode.enemyCount;

        sr = GetComponentInChildren<SpriteRenderer>();
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();


        // Exists so each enemy has a reference to the room that it's in
        GameObject[] rooms;
        rooms = GameObject.FindGameObjectsWithTag("Room");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector2 position = transform.position;
        foreach (GameObject enemy in rooms)
        {
            Vector2 diff = new Vector2(enemy.transform.position.x, enemy.transform.position.y) - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = enemy;
                distance = curDistance;
            }
        }

        //Set the nearest room as the reference for room
        room = closest;

        Reset();
    }

    void Reset()
    {
        gamemode.e_CurHealth = gamemode.e_MaxHealth;
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
        Shoot();
        ElementManager();

        Vector3 z;
        z = transform.position;
        z.z = 0f;
    }

    void ElementManager()
    {
        if (isFire)
        {
            sr.color = Color.red;
        }

        if (isWater)
        {
            sr.color = Color.blue;
        }

        if (isEarth)
        {
            sr.color = Color.green;
        }
    }


    public void DecreaseHealth(float bulletDamage, string playersCurElement)
    {
        if (gamemode.e_CurHealth > gamemode.e_HealthDeath)
        {
            // If player countered the enemy with their hit, take bonus damage
            if (playersCurElement == "Fire" && isEarth)
            {
                gamemode.e_CurHealth -= bulletDamage + gamemode.fireDamage;
                e_HealthBar.fillAmount = gamemode.e_CurHealth / 100;
            }

            // If player countered the enemy with their hit, take bonus damage
            if (playersCurElement == "Water" && isFire)
            {
                gamemode.e_CurHealth -= bulletDamage + gamemode.waterDamage;
                e_HealthBar.fillAmount = gamemode.e_CurHealth / 100;
            }

            // If player countered the enemy with their hit, take bonus damage
            if (playersCurElement == "Earth" && isWater)
            {
                gamemode.e_CurHealth -= bulletDamage + gamemode.earthDamage;
                e_HealthBar.fillAmount = gamemode.e_CurHealth / 100;
            }


            // If there is no element counter, do regular damage
            if (playersCurElement == "Fire" && isWater)
            {
                gamemode.e_CurHealth -= bulletDamage;
                e_HealthBar.fillAmount = gamemode.e_CurHealth / 100;
            }

            // If there is no element counter, do regular damage
            if (playersCurElement == "Fire" && isFire)
            {
                gamemode.e_CurHealth -= bulletDamage;
                e_HealthBar.fillAmount = gamemode.e_CurHealth / 100;
            }

            // If there is no element counter, do regular damage
            if (playersCurElement == "Water" && isEarth)
            {
                gamemode.e_CurHealth -= bulletDamage;
                e_HealthBar.fillAmount = gamemode.e_CurHealth / 100;
            }

            // If there is no element counter, do regular damage
            if (playersCurElement == "Water" && isWater)
            {
                gamemode.e_CurHealth -= bulletDamage;
                e_HealthBar.fillAmount = gamemode.e_CurHealth / 100;
            }

            // If there is no element counter, do regular damage
            if (playersCurElement == "Earth" && isFire)
            {
                gamemode.e_CurHealth -= bulletDamage;
                e_HealthBar.fillAmount = gamemode.e_CurHealth / 100;
            }

            // If there is no element counter, do regular damage
            if (playersCurElement == "Earth" && isEarth)
            {
                gamemode.e_CurHealth -= bulletDamage;
                e_HealthBar.fillAmount = gamemode.e_CurHealth / 100;
            }
        }
        
        if (gamemode.e_CurHealth <= gamemode.e_HealthDeath)
        {
            //Decrease enemy count
            gamemode.enemyCount--;

            //Decrease room enemy count
            room.GetComponent<Room>().roomEnemyCount--;

            //Kill enemy
            Destroy(gameObject);
        }
    }

    void LookAt()
    {
        if (player != null)
        {
            transform.right = player.transform.position - transform.position;
        }
    }

    void Shoot()
    {
        if (player == null)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, player.transform.position);

        //Is the player further away then e_ViewDis?
        if (distance >= gamemode.e_ViewDis)
        {
            targetInViewRange = false;
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

        // if CAN shoot
        if (!e_HasShot && targetInViewRange)
        {
            e_HasShot = true;

            //If in shooting range, shoot
            if (targetInShootRange)
            {
                Instantiate(bullet, e_GunHolder.position, Quaternion.identity);
            }
        }

        if (targetInViewRange)
        {
            //If not in shooting range, chase
            if (distance > gamemode.e_BulletDist)
            {
                targetInShootRange = false;

                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, gamemode.e_ChaseSpeed * Time.deltaTime);
                transform.right = player.transform.position - transform.position;
            }
        }
    }

    void Evade()
    {
        if (timer <= gamemode.evadetimerMax)
        {
            alreadyChosen = true;
            timer += Time.deltaTime;
        }

        if (timer >= gamemode.evadetimerMax)
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
            if (distance <= gamemode.e_BulletDist)
            {
                targetInShootRange = true;
                transform.right = player.transform.position - transform.position;

                if (random == 1)
                {
                    transform.Translate(Vector2.up * Time.deltaTime * gamemode.e_EvadeSpeed, Space.Self);
                }

                if (random == 2)
                {
                    transform.Translate(Vector2.down * Time.deltaTime * gamemode.e_EvadeSpeed, Space.Self);
                }
            }
        }
    }

    void ShotCooldown()
    {
        if (e_ShotTimer <= gamemode.e_ShotCooldown)
        {
            e_ShotTimer += Time.deltaTime;
        }

        if (e_ShotTimer >= gamemode.e_ShotCooldown)
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
