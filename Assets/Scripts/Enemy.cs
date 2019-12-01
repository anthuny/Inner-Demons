using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Pathfinding;
using EZCameraShake;

public class Enemy : MonoBehaviour
{

    //AI
    private Rigidbody2D rb;
    [HideInInspector]
    public Seeker seeker;
    public bool e_CanSeeTarget;
    public GameObject nearestEnemy;
    public bool tooCloseEnemy;
    public float dist2;
    public float nexWaypointDistance = 3f;
    public float navUpdateTimer = 0.25f;
    [HideInInspector]
    public Path path;
    [HideInInspector]
    public int currentWaypoint = 0;

    [HideInInspector]
    public bool reachedEndOfPath = false;


    //Element bg
    public bool gettingBigger = true;

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
    public Animator elementBGAnimator;
    public GameObject BGElement;


    private Gamemode gm;
    private ScreenShake ss;

    //element bg
    private float x = .1f;
    private float y = .1f;

    void Start()
    {
        gm = GameObject.Find("EGOGamemode").GetComponent<Gamemode>();
        rb = GetComponent<Rigidbody2D>();
        ss = FindObjectOfType<ScreenShake>();

        //Increase enemy count
        gm.enemyCount++;

        //Change name of enemy, including the enemy count
        name = "Enemy " + gm.enemyCount;

        sr = GetComponentInChildren<SpriteRenderer>();
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();


        seeker = GetComponent<Seeker>();

        InvokeRepeating("UpdatePath", 0f, navUpdateTimer);

        Reset();
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, player.transform.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void Reset()
    {
        // Set evade speed to default value
        gm.e_EvadeSpeed = gm.e_EvadeSpeedDef;

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

    void Move()
    {
        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * gm.e_MoveSpeed * Time.deltaTime;

        // Move towards player
        if (!targetInShootRange || !e_CanSeeTarget)
        {
            rb.AddForce(force);
        }

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if (distance < nexWaypointDistance)
        {
            currentWaypoint++;
        }
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
            elementBGAnimator.SetInteger("curElement", 1);
        }

        if (isWater)
        {
            elementBGAnimator.SetInteger("curElement", 0);
        }

        if (isEarth)
        {
            elementBGAnimator.SetInteger("curElement", 2);
        }

        BGElement.transform.localScale = new Vector2(x, y);

        if (gettingBigger)
        {
            // Increase the scale of the background
            x += .1f * Time.deltaTime * gm.incScaleRate;
            y += .1f * Time.deltaTime * gm.incScaleRate;

            // If projectile size has reached it's max scale, stop increasing size.
            if (x >= gm.maxScaleX || y >= gm.maxScaleY)
            {
                gettingBigger = false;
            }
        }

        if (!gettingBigger)
        {
            // Decrease the scale of the background
            x -= .1f * Time.deltaTime * gm.incScaleRate;
            y -= .1f * Time.deltaTime * gm.incScaleRate;

            // If projectile size has reached it's lowest scale, stop increasing size.
            if (x <= gm.minScaleX || y <= gm.minScaleY)
            {
                gettingBigger = true;
            }
        }

    }

    public void DecreaseHealth(float bulletDamage, string playersCurElement)
    {
        if (e_CurHealth > gm.e_HealthDeath)
        {
            //Screen shake
            ScreenShakeInfo Info = new ScreenShakeInfo();
            Info.shakeMag = gm.shakeMagHit;
            Info.shakeRou = gm.shakeRouHit;
            Info.shakeFadeIDur = gm.shakeFadeIDurHit;
            Info.shakeFadeODur = gm.shakeFadeODurHit;
            Info.shakePosInfluence = gm.shakePosInfluenceHit;
            Info.shakeRotInfluence = gm.shakeRotInfluenceHit;

            ss.StartShaking(Info, .1f, 2);

            //Freeze the game for a split second
            gm.Freeze();

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

            Debug.Log("removed");
            FindObjectOfType<SmartCamera>().targets.Remove(gm.nearestEnemy.transform);
            FindObjectOfType<SmartCamera>().beenAdded = false;


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
            e_CanSeeTarget = false;
        }

        else
        {
            //Debug.DrawLine(transform.position, gm.player.transform.position, Color.green, .1f);
            //Debug.Log("Hitting player, hitting: " + hitinfo.transform.tag);
            e_CanSeeTarget = true;
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
            if (targetInShootRange && e_CanSeeTarget)
            {
                GameObject go = Instantiate(bullet, e_GunHolder.position, Quaternion.identity);
                go.GetComponent<E_Bullet>().enemy = gameObject;

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
        if (timer <= gm.evadeTimerCur)
        {
            alreadyChosen = true;
            timer += Time.deltaTime;
        }

        if (timer >= gm.evadeTimerCur)
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
            if (distance <= gm.e_BulletDist - gm.e_rangeOffset && e_CanSeeTarget || tooCloseEnemy)
            {
                if (!tooCloseEnemy)
                {
                    gm.evadeTimerCur = gm.evadeTimerDef;
                }

                targetInShootRange = true;

                // Play run animation
                animator.SetInteger("EnemyBrain", 1);

                // Evade Left
                if (random == 1 || random == 6 && !tooCloseEnemy)
                {
                    rb.AddRelativeForce(e_GunHolder.transform.up * gm.e_EvadeSpeed * Time.deltaTime);
                }

                // go left, the opposite of what the close enemy is now doing
                if (random == 1 || random == 6 && tooCloseEnemy)
                {
                    gm.evadeTimerCur = gm.enemyOverlapEvadeTimer;

                    // Send close enemy right
                    if (nearestEnemy)
                    {
                        nearestEnemy.GetComponent<Enemy>().random = 2;
                        rb.AddRelativeForce(e_GunHolder.transform.up * gm.e_EvadeSpeed * Time.deltaTime);
                    }
                }

                // Evade Right
                if (random == 2 || random == 5 && !tooCloseEnemy)
                {
                    rb.AddRelativeForce(-e_GunHolder.transform.up * gm.e_EvadeSpeed * Time.deltaTime);
                }

                // go right, the opposite of what the close enemy is now doing
                if (random == 2 || random == 5 && tooCloseEnemy)
                {
                    gm.evadeTimerCur = gm.enemyOverlapEvadeTimer;

                    if (nearestEnemy)
                    {
                        // Send close enemy left
                        nearestEnemy.GetComponent<Enemy>().random = 1;
                        rb.AddRelativeForce(-e_GunHolder.transform.up * gm.e_EvadeSpeed * Time.deltaTime);
                    }
                }

                // Evade forwards
                if (random == 3 && !tooCloseEnemy)
                {
                    rb.AddRelativeForce(-e_GunHolder.transform.right * gm.e_EvadeSpeed * Time.deltaTime);
                }

                // go forwards, the opposite of what the close enemy is now doing
                if (random == 3 && tooCloseEnemy)
                {
                    gm.evadeTimerCur = gm.enemyOverlapEvadeTimer;

                    if (nearestEnemy)
                    {
                        // Send close enemy backwards
                        nearestEnemy.GetComponent<Enemy>().random = 4;
                        rb.AddRelativeForce(-e_GunHolder.transform.right * gm.e_EvadeSpeed * Time.deltaTime);
                    }
                }

                // Evade Backwards
                if (random == 4 && !tooCloseEnemy)
                {
                    rb.AddRelativeForce(e_GunHolder.transform.right * gm.e_EvadeSpeed * Time.deltaTime);
                }

                // go backwards, the opposite of what the close enemy is now doing
                if (random == 4 && tooCloseEnemy)
                {
                    gm.evadeTimerCur = gm.enemyOverlapEvadeTimer;

                    if (nearestEnemy)
                    {
                        // Send close enemy forwards
                        nearestEnemy.GetComponent<Enemy>().random = 3;
                        rb.AddRelativeForce(e_GunHolder.transform.right * gm.e_EvadeSpeed * Time.deltaTime);
                    }
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
//void Move()
//{
//    if enemy is not in shoot range and enemy cannot see player
//    if (!targetInShootRange || !e_CanSeeTarget)
//    {
//        float minDist = Mathf.Infinity;

//        Check if there is an enemy very close to this enemy
//        foreach (Enemy e in FindObjectsOfType<Enemy>())
//        {
//            Don't include this enemy in the foreach
//            if (e != this.gameObject.GetComponent<Enemy>())
//            {
//                float dist = Vector2.Distance(e.transform.position, transform.position);

//                if (minDist > dist)
//                {
//                    nearestEnemy = e.transform.gameObject;
//                    minDist = dist;
//                }
//            }
//        }

//        If there is a close enemy, check...
//        if (nearestEnemy)
//        {
//            float dist2 = Vector2.Distance(nearestEnemy.transform.position, transform.position);

//            If the other enemy is too close
//            if (dist2 < gm.enemyTooCloseDis)
//            {
//                tooCloseEnemy = true;
//            }
//            else
//            {
//                tooCloseEnemy = false;
//            }

//            If there is an enemy, but it's not too close
//            if (!tooCloseEnemy)
//            {
//                Set collider to normal size, so enemies cannot walk ontop of eachother
//                GetComponent<CircleCollider2D>().radius = 7;
//                gm.e_EvadeSpeed = gm.e_EvadeSpeedDef;

//                if (path == null)
//                {
//                    return;
//                }

//                if (currentWaypoint >= path.vectorPath.Count)
//                {
//                    reachedEndOfPath = true;
//                    return;
//                }
//                else
//                {
//                    reachedEndOfPath = false;
//                }

//                Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
//                Vector2 force = direction * gm.e_MoveSpeed * Time.deltaTime;

//                Move the enemy towards player
//                rb.AddForce(force);

//                Play run animation
//                animator.SetInteger("EnemyBrain", 1);

//                float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

//                if (distance < nexWaypointDistance)
//                {
//                    currentWaypoint++;
//                }
//            }

//            if (tooCloseEnemy)
//            {
//                Set colldier radius to very small, so enemies CAN walk ontop of eachother
//                GetComponent<CircleCollider2D>().radius = 7;
//                gm.e_EvadeSpeed = gm.enemyOverlapSpeed / 2;
//            }
//        }

//        if there is NO close enemy(only one enemy)
//        else
//        {
//            if (path == null)
//            {
//                return;
//            }

//            if (currentWaypoint >= path.vectorPath.Count)
//            {
//                reachedEndOfPath = true;
//                return;
//            }
//            else
//            {
//                reachedEndOfPath = false;
//            }

//            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
//            Vector2 force = direction * gm.e_MoveSpeed * Time.deltaTime;

//            Move the enemy towards player
//            rb.AddForce(force);

//            Play run animation
//            animator.SetInteger("EnemyBrain", 1);

//            float distance2 = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

//            if (distance2 < nexWaypointDistance)
//            {
//                currentWaypoint++;
//            }
//        }
//    }
//}