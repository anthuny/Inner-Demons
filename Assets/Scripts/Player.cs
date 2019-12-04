using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //Bullet
    public GameObject bullet;
    public Transform gunHolder;
    public bool hasShot;
    private float shotTimer = 0f;

    //Player
    private Vector3 inputVector;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public Animator animator;
    public Vector2 myVelocity;

    //bullet holder
    private Transform gunHolderLeft;
    private Transform gunHolderRight;

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

    //Screen shake
    private ScreenShake ss;
    private void Start()
    {
        gunHolderLeft = GameObject.Find("Weapon Holder Left").transform;
        gunHolderRight = GameObject.Find("Weapon Holder Right").transform;
        gdm = FindObjectOfType<GameDownManager>();
        gm = FindObjectOfType<Gamemode>();
        dm = FindObjectOfType<DialogueManager>();
        ss = FindObjectOfType<ScreenShake>();
        name = "Player";
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();

        gdm.playerDied = false;
        dm.choices.SetActive(false);
        gm.Reset();
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
            gm.p_healthDeath = -10000;
            gm.bulletSpeed += 20;
            gm.playerSpeedCur += 2.5f;
            gm.shotCooldown -= .3f;
        }

        if (Input.GetKeyDown("h"))
        {
            gm.p_healthDeath = -10000f;
        }
    }

    public void DecreaseHealth(float bulletDamage)
    {
        // Decrease player's health if they shouldn't be dead already
        if (gm.p_curHealth > gm.p_healthDeath)
        {
            gm.p_curHealth -= bulletDamage;
        }

        // Kill the player 
        if (gm.p_curHealth <= gm.p_healthDeath)
        {
            gdm.playerDied = true;
            Destroy(gameObject);
        }
    }

    void ShotCooldown()
    {
        //Shot on cooldown
        if (shotTimer <= gm.shotCooldown)
        {
            hasShot = true;
            shotTimer += Time.deltaTime;
        }

        if (shotTimer >= gm.shotCooldown)
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
        hasShot = true;
        GameObject go2 = Instantiate(bullet, gunHolder.position, gunHolder.transform.rotation);
        go2.GetComponent<Bullet>().weaponHolder = gunHolder;
        bullet.GetComponent<Bullet>().playerBullet = true;

        //Screen shake
        ScreenShakeInfo Info = new ScreenShakeInfo();
        Info.shakeMag = gm.shakeMagShoot;
        Info.shakeRou = gm.shakeRouShoot;
        Info.shakeFadeIDur = gm.shakeFadeIDurShoot;
        Info.shakeFadeODur = gm.shakeFadeODurShoot;
        Info.shakePosInfluence = gm.shakePosInfluenceShoot;
        Info.shakeRotInfluence = gm.shakeRotInfluenceShoot;

        ss.StartShaking(Info, .1f, 1);
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
            RaycastHit2D hitinfo = Physics2D.Linecast(transform.position, gm.nearestEnemy.transform.position, (1 << 15) | (1 << 14));

            // If cannot see target
            if (hitinfo.transform.tag != "Enemy")
            {
                //Debug.DrawLine(transform.position, gm.nearestEnemy.transform.position, Color.red, .1f);
                //Debug.Log("Can't see target, Hitting: " + hitinfo.transform.name);
                gm.p_CanSeeTarget = false;
            }
            //If CAN see target
            else
            {
                //Debug.DrawLine(transform.position, gm.nearestEnemy.transform.position, Color.green, .1f);
                //Debug.Log("CAN see target, Hitting: " + hitinfo.transform.name);
                gm.p_CanSeeTarget = true;
            }

            Vector3 viewPos = Camera.main.WorldToViewportPoint(gm.nearestEnemy.transform.position);

            // If nearest enemy is in range, and player can see them, allow auto aim to be on
            if (viewPos.y >= -0.2f && viewPos.y <= 1.2f && viewPos.x >= -0.2f && viewPos.x <= 1.2f && gm.p_CanSeeTarget)
            {
                gm.shootDir = gm.nearestEnemy.transform.position - transform.position;
                gm.shootDir.Normalize();
                gunHolderLeft.transform.right = gm.shootDir;
                gunHolderRight.transform.right = gm.shootDir;
                gunHolder.transform.right = gm.shootDir;

                gm.autoAimOn = true;
            }

            else
            {
                gm.autoAimOn = false;
            }
        }

        else
        {
            gm.autoAimOn = false;
        }


    }

    IEnumerator ShootAnimation()
    {
        // if the player is NOT in a conversation, they can look around
        if (!FindObjectOfType<DialogueManager>().dialogueTriggered)
        {
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
                // If the player has NOT shot, and the dialogue is NOT triggered
                // If the player is touching the shoot area, OR 
                // the player is inputing arrow key movements
                if (RectTransformUtility.RectangleContainsScreenPoint(gm.shootArea.GetComponent<RectTransform>(), gm.touch.position)
                || Input.GetKey(KeyCode.Space))
                {
                    // If auto aim is OFF
                    if (!gm.autoAimOn)
                    {
                        if (!gm.usingPC && !playerStill)
                        {
                            gunHolder.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(gm.joystickMove.Vertical, gm.joystickMove.Horizontal) * 180 / Mathf.PI);
                            //gunHolderLeft.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(gm.joystickMove.Vertical, gm.joystickMove.Horizontal) * 180 / Mathf.PI);
                            //gunHolderRight.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(gm.joystickMove.Vertical, gm.joystickMove.Horizontal) * 180 / Mathf.PI);
                        }

                        if (gm.usingPC && !playerStill)
                        {
                            gunHolder.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(myVelocity.y, myVelocity.x) * 180 / Mathf.PI);
                            //gunHolderLeft.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(myVelocity.y, myVelocity.x) * 180 / Mathf.PI);
                            //gunHolderRight.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(myVelocity.y, myVelocity.x) * 180 / Mathf.PI);
                        }

                        // Shoot Bullet
                        Shoot();

                        // Play shoot animation
                        animator.SetInteger("playerBrain", 2);
                    }
                    
                    // If auto aim is ON
                    if (gm.autoAimOn)
                    {
                        // Shoot Bullet
                        Shoot();

                        // Play shoot animation
                        animator.SetInteger("playerBrain", 2);

                        // If nearest enemy exists
                        if (gm.nearestEnemy)
                        {
                            // Rotate player to correct direction
                            if (gm.nearestEnemy.position.x >= transform.position.x)
                            {
                                // Make the sprite face RIGHT
                                GetComponentInChildren<SpriteRenderer>().flipX = false;

                                // Move the gun holder to the RIGHT side of the player's sprite
                                gunHolder.transform.position = gunHolderRight.transform.position;
                            }
                            else
                            {
                                // Make the sprite face LEFT
                                GetComponentInChildren<SpriteRenderer>().flipX = true;

                                // Move the gun holder to the LEFT side of the player's sprite
                                gunHolder.transform.position = gunHolderLeft.transform.position;
                            }
                        }
                        yield return new WaitForSeconds(0);
                    }
                }
            }

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
                rb.velocity = inputVector * gm.playerSpeedCur;

                if (myVelocity == new Vector2(0, 0))
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
                gm.joystickHolder.SetActive(true);

                if (gm.joystickMove.Horizontal == 0 && gm.joystickMove.Vertical == 0)
                {
                    playerStill = true;
                }

                else
                {
                    playerStill = false;
                }

                if (gm.joystickMove.Horizontal >= gm.joystickMove.DeadZone || gm.joystickMove.Horizontal <= gm.joystickMove.DeadZone
                    || gm.joystickMove.Vertical >= gm.joystickMove.DeadZone || gm.joystickMove.Vertical <= gm.joystickMove.DeadZone)
                {
                    inputVector = new Vector2(gm.joystickMove.Horizontal, gm.joystickMove.Vertical);
                    rb.velocity = inputVector * gm.playerSpeedCur;
                }
            }

            // Disable movement and shoot joysticks if player is in dialogue
            if (FindObjectOfType<DialogueManager>().dialogueTriggered)
            {
                rb.velocity = new Vector2(0, 0);
                gm.joystickHolder.SetActive(false);
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

        // If player is moving play animation accordingly
        else if (myVelocity != Vector2.zero)
        {
            animator.SetInteger("playerBrain", 1);
        }

        // right
        if (myVelocity.x > 0 && !gm.autoAimOn)
        {
            // Make the sprite face RIGHT
            GetComponentInChildren<SpriteRenderer>().flipX = false;

            // Move the gun holder to the RIGHT side of the player's sprite
            gunHolder.transform.position = gunHolderRight.transform.position;
        }
        // left
        else if (myVelocity.x < 0 && !gm.autoAimOn)
        {
            // Make the sprite face LEFT
            GetComponentInChildren<SpriteRenderer>().flipX = true;

            // Move the gun holder to the LEFFT side of the player's sprite
            gunHolder.transform.position = gunHolderLeft.transform.position;
        }
    }
 }  
