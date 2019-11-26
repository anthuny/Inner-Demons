using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Gamemode gamemode;

    //Bullet
    public GameObject bullet;
    public Transform gunHolder;
    public bool hasShot;
    private float shotTimer = 0f;

    //Player
    private Vector3 inputVector;
    public Image p_HealthBar;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public Animator animator;
    public Vector2 myVelocity;

    //Enemies
    public GameObject[] enemies;

    //Gamemode
    private Gamemode gm;

    //Room
    public GameObject doors;
    public GameObject room;
    public Room roomScript;

    //Memory
    public GameObject memory;
    public bool playerStill;
    public bool canInteract;
    private DialogueManager dm;

    //GameDownManager
    private GameDownManager gdm;

    //aa
    private bool leftPressed;


    private void Start()
    {
        gdm = FindObjectOfType<GameDownManager>();
        gm = FindObjectOfType<Gamemode>();
        dm = FindObjectOfType<DialogueManager>();
        gamemode = FindObjectOfType<Gamemode>();
        name = "Player";
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        gdm.playerDied = false;
    }

    private void Update()
    {
        myVelocity = rb.velocity;
        myVelocity.Normalize();

    }
    void FixedUpdate()
    {
        if (hasShot)
        {
            ShotCooldown();
        }

        MovementAnimation();
        Movement();
        StartCoroutine("ShootAnimation");
        GodMode();
        CanSeeTarget();
    }
    void GodMode()
    {
        // If the player presses 'G' their stats are increased
        if (Input.GetKeyDown("g"))
        {
            gamemode.p_healthDeath = -10000;
            gamemode.bulletSpeed += 20;
            gamemode.playerSpeedCur += 2.5f;
            gamemode.shotCooldown -= .1f;
        }

        if (Input.GetKeyDown("h"))
        {
            gamemode.p_healthDeath = -10000f;
        }
    }

    public void DecreaseHealth(float bulletDamage)
    {
        if (gamemode.p_curHealth > gamemode.p_healthDeath)
        {
            gamemode.p_curHealth -= bulletDamage;
            p_HealthBar.fillAmount -= bulletDamage / 100;
        }

        if (gamemode.p_curHealth <= gamemode.p_healthDeath)
        {
            gdm.playerDied = true;
            Destroy(gameObject);
        }
    }

    void ShotCooldown()
    {
        //Shot on cooldown
        if (shotTimer <= gamemode.shotCooldown)
        {
            hasShot = true;
            shotTimer += Time.deltaTime;
        }

        if (shotTimer >= gamemode.shotCooldown)
        {
            hasShot = false;
            shotTimer = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If player enters a room, trigger enemies to spawn
        // And check if room has already been entered before
        if (other.gameObject.tag == ("RoomCollider"))
        {
            // Remove player dialogue box from screen
            dm.animatorP.SetBool("isOpenP", false);

            // Get reference to root of this object
            Transform roomTrans;
            roomTrans = other.transform.root;
            room = roomTrans.gameObject;

            if (room.GetComponent<Room>())
            {
                if (!room.GetComponent<Room>().beenEntered)
                {
                    room.GetComponent<Room>().beenEntered = true;
                    room.GetComponent<Room>().StartCoroutine("SpawnEnemies");
                }
            }
        }

        // If player triggers with room collider, close doors
        if (other.gameObject.tag == ("RoomEnter"))
        {
            // Get reference to root of this object
            Transform roomTrans;
            roomTrans = other.transform.root;
            room = roomTrans.gameObject;

            if (room.GetComponent<Room>() && !room.GetComponent<Room>().doorsClosed)
            {
                room.GetComponent<Room>().StartCoroutine("TriggerDoorClose");
            }
        }

        // If player talks to a memory
        if (other.gameObject.tag == ("Memory"))
        {
            // If the object is a memory
            if (!GameObject.FindGameObjectWithTag("Enemy"))
            {
                memory = other.gameObject;
            }
            
            // if the object is the boss
            else
            {
                memory = other.transform.parent.gameObject;
            }

            // Only allow player to interact with each memory once
            if (!memory.GetComponent<Memory>().interacted)
            {
                memory.GetComponent<Memory>().interacted = true;

                dm.inRange = true;
                dm.EnableTalkButton();
            }
        }
    }

    // Upon leaving the memories hitbox, remove the talk button 
    private void OnTriggerExit2D(Collider2D other)
    {
        // If player talks to a memory
        if (other.gameObject.tag == ("Memory") && dm.buttonTriggered)
        {
            dm.inRange = false;
            dm.DisableTalkButton();
        }

        if (other.gameObject.tag == ("RoomCollider"))
        {
            // Remove player dialogue box from screen
            dm.animatorP.SetBool("isOpenP", false);

            // Get reference to root of this object
            Transform roomTrans;
            roomTrans = other.transform.root;
            room = roomTrans.gameObject;
            roomScript = room.GetComponent<Room>();
            if (roomScript)
            {
                room.GetComponent<Room>().beenCleared = true;
            }
        }

        // If player leaves memory's range. remove 
        // the memory variable from player
        if (other.gameObject.tag == ("Memory"))
        {
            memory = null;
        }
    }

    public void Shoot()
    {
        // If using mobile
        if (!gm.usingPC)
        {
            hasShot = true;
            GameObject go2 = Instantiate(bullet, gunHolder.position, gunHolder.transform.rotation);
            go2.GetComponent<Bullet>().weaponHolder = gunHolder;
            bullet.GetComponent<Bullet>().playerBullet = true;
        }

        // If using PC
        if (gm.usingPC)
        {
            hasShot = true;
            GameObject go = Instantiate(bullet, gunHolder.position, gunHolder.transform.rotation);
            go.GetComponent<Bullet>().weaponHolder = gunHolder;
            bullet.GetComponent<Bullet>().playerBullet = true;
        }
    }

    void CanSeeTarget()
    {
        float minDist = Mathf.Infinity;

        // Detect the nearest enemy's position
        foreach (Enemy e in FindObjectsOfType<Enemy>())
        {
            float dist = Vector3.Distance(e.transform.position, transform.position);

            if (minDist > dist)
            {
                gm.nearestEnemy = e.transform;
                minDist = dist;
            }
        }

        // If there ever was a nearest enemy
        if (gm.nearestEnemy)
        {
            Vector3 viewPos = Camera.main.WorldToViewportPoint(gm.nearestEnemy.transform.position);

            // If nearest enemy is in range, and player can see them, allow auto aim to be on
            if (viewPos.y >= -0.2 && viewPos.y <= 1.2f && viewPos.x >= -.1f && viewPos.x <= 1.1f && gm.p_CanSeeTarget)
            {
                gm.shootDir = gm.nearestEnemy.transform.position - transform.position;
                gm.shootDir.Normalize();
                gunHolder.transform.right = gm.shootDir;

                gm.autoAimOn = true;
            }

            else
            {
                gm.autoAimOn = false;
            }
        }

        if (gm.nearestEnemy)
        {
            RaycastHit2D hitinfo = Physics2D.Linecast(transform.position, gm.nearestEnemy.transform.position);

            // See if can see target
            if (hitinfo)
            {
                if (hitinfo.transform.tag != "Enemy" && hitinfo.transform.tag != "Bullet" && hitinfo.transform.tag != "EBullet")
                {
                    //Debug.DrawLine(transform.position, gm.nearestEnemyOR.transform.position, Color.red, .1f);
                    gm.p_CanSeeTarget = false;
                }
                else
                {
                    //Debug.DrawLine(transform.position, gm.nearestEnemyOR.transform.position, Color.green, .1f);
                    gm.p_CanSeeTarget = true;
                }
            }
        }    
    }

    IEnumerator ShootAnimation()
    {
        Debug.Log("here1");
        // if the player is NOT in a conversation, they can look around
        if (!FindObjectOfType<DialogueManager>().dialogueTriggered)
        {
            Debug.Log("here2");
            if (animator.GetBool("attackingLeft") || (animator.GetBool("attackingRight")) || (animator.GetBool("attackingUp")) || (animator.GetBool("attackingDown")))
            {
                // This is a temporary fix, Needs speed to increase depending on how small gm.shotSpeed is
                animator.speed = 1;
            }

            if (!animator.GetBool("attackingLeft") && (!animator.GetBool("attackingRight")) && (!animator.GetBool("attackingUp")) && (!animator.GetBool("attackingDown")))
            {
                animator.speed = 1;
            }

            // If there is just one touch on screen, set touch to that one
            if (Input.touchCount == 1)
            {
                gm.touch = Input.touches[0];
            }
            // If there is already a touch on the screen, set touch to the next one 
            else if (Input.touchCount == 2)
            {
                gm.touch = Input.touches[1];
            }
            //  If there is already TWO touches on the screen, set touch to the next one 
            else if (Input.touchCount == 3)
            {
                gm.touch = Input.touches[2];
            }

            if (!hasShot && !dm.dialogueTriggered)
            {
                Debug.Log("here3");
                // If auto aim is on
                if (!gm.autoAimOn)
                {
                    Debug.Log("here4");

                    // If the player has NOT shot, and the dialogue is NOT triggered
                    // If the player is touching the shoot area, OR 
                    // the player is inputing arrow key movements
                    if (RectTransformUtility.RectangleContainsScreenPoint(gm.shootArea.GetComponent<RectTransform>(), gm.touch.position)
                    || Input.GetKey(KeyCode.Space))
                    {
                        Debug.Log("here5");
                        // Shoot Bullet
                        Shoot();

                        animator.SetInteger("playerBrain", 2);

                        yield return new WaitForSeconds(0);

                        animator.SetInteger("playerBrain", 0);

                    }
                }
            }

            //gunHolder.transform.eulerAngles = new Vector3(gunHolder.eulerAngles.x, gunHolder.eulerAngles.y, Mathf.Atan2(gm.joystickMove.Horizontal, gm.joystickMove.Vertical) * Mathf.Rad2Deg);
            //Vector3 rot = new Vector3(Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));

            gunHolder.transform.localEulerAngles = new Vector3(0f, 0f, Mathf.Atan2(gm.joystickMove.Horizontal, gm.joystickMove.Vertical) * 180 / Mathf.PI); // this does the actual rotaion according to inputs
        }
    }

    void Movement()
    {
        // if the player is NOT in a conversation, they can move
        if (!FindObjectOfType<DialogueManager>().dialogueTriggered)
        {
            gm.joystickHolder.SetActive(true);

            if (gm.usingPC)
            {
                inputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                rb.velocity = inputVector * gamemode.playerSpeedCur;

                if (rb.velocity == new Vector2(0, 0))
                {
                    playerStill = true;
                }
                else
                {
                    playerStill = false;
                }
            }

            if (!gm.usingPC)
            {
                // disable movement and shoot joysticks if player is in dialogue
                gamemode.joystickHolder.SetActive(true);

                if (gamemode.joystickMove.Horizontal == 0 && gamemode.joystickMove.Vertical == 0)
                {
                    playerStill = true;
                }

                else
                {
                    playerStill = false;
                }

                if (gamemode.joystickMove.Horizontal >= gamemode.joystickMove.DeadZone || gamemode.joystickMove.Horizontal <= gamemode.joystickMove.DeadZone
                    || gamemode.joystickMove.Vertical >= gamemode.joystickMove.DeadZone || gamemode.joystickMove.Vertical <= gamemode.joystickMove.DeadZone)
                {
                    inputVector = new Vector2(gamemode.joystickMove.Horizontal, gamemode.joystickMove.Vertical);
                    rb.velocity = inputVector * gamemode.playerSpeedCur;
                }
            }

            // Rotate the gunHolder towards the direction the player is facing

            //// If moving Up
            //if (!gm.autoAimOn && myVelocity.y > 0 && myVelocity.x > -0.9f && myVelocity.x < 0.9f)
            //{
            //    // Rotate gun holder
            //    gunHolder.localEulerAngles = new Vector3(0, 0, 90);
            //}

            //// If moving Down
            //else if (!gm.autoAimOn && myVelocity.y < 0 && myVelocity.x > -0.9f && myVelocity.x < 0.9f)
            //{
            //    // Rotate gun holder
            //    gunHolder.localEulerAngles = new Vector3(0, 0, -90);
            //}

            //// If moving Right
            //else if (!gm.autoAimOn && myVelocity.x > 0 && myVelocity.y > -0.9f && myVelocity.y < 0.9f)
            //{
            //    // Rotate gun holder
            //    gunHolder.localEulerAngles = new Vector3(0, 0, 0);
            //}

            //// If moving Left
            //else if (!gm.autoAimOn && myVelocity.x < 0 && myVelocity.y > -0.9f && myVelocity.y < 0.9f)
            //{
            //    // Rotate gun holder
            //    gunHolder.localEulerAngles = new Vector3(0, 0, -180);
            //}

            //// If not moving
            //else if (!gm.autoAimOn && myVelocity == Vector2.zero)
            //{
            //    // Rotate gun holder
            //    gunHolder.localEulerAngles = new Vector3(0, 0, -90);
            //}

            // Disable movement and shoot joysticks if player is in dialogue
            if (FindObjectOfType<DialogueManager>().dialogueTriggered)
            {
                rb.velocity = new Vector2(0, 0);
                gamemode.joystickHolder.SetActive(false);
            }
        }
        else
        {
            gm.joystickHolder.SetActive(false);
        }
    }

    void MovementAnimation()
    {
        // Check if player is NOT moving, to play correct animation
        if (myVelocity == Vector2.zero)
        {
            if (!hasShot && !dm.dialogueTriggered)
            {
                animator.SetInteger("playerBrain", 0);
            }
        }

        // If player is moving Up play animation accordingly
        else if (myVelocity != Vector2.zero)
        {
            animator.SetInteger("playerBrain", 1);
        }

        // right
        if (myVelocity.x > 0)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = false;
        }
        // left
        if (myVelocity.x < 0)
        {
            GetComponentInChildren<SpriteRenderer>().flipX = true;
        }
    }
 }  
