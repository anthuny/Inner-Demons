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
    private Image p_HealthBar;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public Animator animator;
    private Vector2 myVelocity;

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
    public GameObject talkButton;
    private DialogueManager dg;

    //GameDownManager
    private GameDownManager gdm;


    private void Start()
    {
        gdm = FindObjectOfType<GameDownManager>();
        gm = FindObjectOfType<Gamemode>();
        dg = FindObjectOfType<DialogueManager>();
        gamemode = FindObjectOfType<Gamemode>();
        name = "Player";
        rb = GetComponent<Rigidbody2D>();
        p_HealthBar = GameObject.Find("/Player/Canvas/Health").GetComponent<Image>();
        sr = GetComponentInChildren<SpriteRenderer>();  

        Reset();
    }

    void Reset()
    {
        gdm.playerDied = false;
        gamemode.p_curHealth = gamemode.p_maxHealth;
        p_HealthBar.fillAmount = 1f;
        gamemode.isFire = true;
        
        talkButton.GetComponentInChildren<CanvasGroup>().alpha = 0;
        talkButton.SetActive(true);
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
            dg.animatorP.SetBool("isOpenP", false);

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
                room.GetComponent<Room>().CloseDoors();
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

                dg.inRange = true;
                dg.EnableTalkButton();
            }
        }
    }

    // Upon leaving the memories hitbox, remove the talk button 
    private void OnTriggerExit2D(Collider2D other)
    {
        // If player talks to a memory
        if (other.gameObject.tag == ("Memory") && dg.buttonTriggered)
        {
            dg.inRange = false;
            dg.DisableTalkButton();
        }

        if (other.gameObject.tag == ("RoomCollider"))
        {
            // Remove player dialogue box from screen
            dg.animatorP.SetBool("isOpenP", false);

            // Get reference to root of this object
            Transform roomTrans;
            roomTrans = other.transform.root;
            room = roomTrans.gameObject;
            roomScript = room.GetComponent<Room>();
            if (roomScript && gdm.playerDied)
            {
                roomScript.enabled = false;
            }

        }
    }

    public void Shoot()
    {
        // If using mobile
        if (!gm.usingPC)
        {
            hasShot = true;
            GameObject go2 = Instantiate(bullet, gunHolder.position, Quaternion.identity);
            go2.GetComponent<Bullet>().weaponHolder = gunHolder;
            bullet.GetComponent<Bullet>().playerBullet = true;
        }

        // If using PC
        if (gm.usingPC)
        {
            hasShot = true;
            GameObject go = Instantiate(bullet, gunHolder.position, Quaternion.identity);
            go.GetComponent<Bullet>().weaponHolder = gunHolder;
            bullet.GetComponent<Bullet>().playerBullet = true;
        }
    }

    IEnumerator ShootAnimation()
    {
        gm.nearestEnemy = null;

        float minDist = Mathf.Infinity;

        // Detect the nearest enemy's position
        foreach (Enemy e in FindObjectsOfType<Enemy>())
        {
            float dist = Vector3.Distance(e.transform.position, transform.position);
            if (gm.autoAimDis > dist)
            {
                if (minDist > dist)
                {
                    gm.nearestEnemy = e.transform;
                    minDist = dist;
                }
            }
        }

        if (gm.nearestEnemy != null)
        {
            gm.autoAimOn = true;

            // Set rotation of gun holder to aim at enemy position
            // Rotate gun holder
            gm.shootDir = gm.nearestEnemy.transform.position - transform.position;
            gm.shootDir.Normalize();
            gunHolder.transform.right = gm.shootDir;
        }
        else
        {
            gm.autoAimOn = false;
        }

        // if the player is NOT in a conversation, they can look around
        if (!FindObjectOfType<DialogueManager>().dialogueTriggered)
        {
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

            if (!hasShot && !dg.dialogueTriggered)
            {
                // If auto aim is on
                if (gm.autoAimOn)
                {
                    // If the player is walking Up or auto aim detects an enemy Up
                    if (gm.shootDir.y > 0 && gm.shootDir.x > -.9f && gm.shootDir.x < 0.9f)
                    {
                        // If the player has NOT shot, and the dialogue is NOT triggered
                        // If the player is touching the shoot area, OR 
                        // the player is inputing arrow key movements
                        if (RectTransformUtility.RectangleContainsScreenPoint(gm.shootArea.GetComponent<RectTransform>(), gm.touch.position)
                        || Input.GetKey(KeyCode.Space))
                        {
                            // Shoot Bullet
                            Shoot();

                            animator.SetBool("walkingUp", false);
                            animator.SetBool("isIdle", false);
                            animator.SetBool("attackingLeft", false);
                            animator.SetBool("attackingRight", false);
                            animator.SetBool("attackingDown", false);
                            animator.SetBool("attackingUp", true);

                            yield return new WaitForSeconds(0);

                            animator.SetBool("attackingUp", false);
                        }
                    }

                    // If the player is walking Down or auto aim detects an enemy Down
                    else if (gm.shootDir.y < 0 && gm.shootDir.x > -.9f && gm.shootDir.x < 0.9f)
                    {
                        // If the player has NOT shot, and the dialogue is NOT triggered
                        // If the player is touching the shoot area, OR 
                        // the player is inputing arrow key movements
                        if (RectTransformUtility.RectangleContainsScreenPoint(gm.shootArea.GetComponent<RectTransform>(), gm.touch.position)
                        || Input.GetKey(KeyCode.Space))
                        {
                            // Shoot Bullet
                            Shoot();

                            animator.SetBool("walkingDown", false);
                            animator.SetBool("isIdle", false);
                            animator.SetBool("attackingLeft", false);
                            animator.SetBool("attackingRight", false);
                            animator.SetBool("attackingUp", false);
                            animator.SetBool("attackingDown", true);

                            yield return new WaitForSeconds(0);

                            animator.SetBool("attackingDown", false);
                        }
                    }

                    // If the player is walking Left or auto aim detects an enemy Left
                    else if (gm.shootDir.x < 0 && gm.shootDir.y > -.9f && gm.shootDir.y < 0.9f)
                    {
                        // If the player has NOT shot, and the dialogue is NOT triggered
                        // If the player is touching the shoot area, OR 
                        // the player is inputing arrow key movements
                        if (RectTransformUtility.RectangleContainsScreenPoint(gm.shootArea.GetComponent<RectTransform>(), gm.touch.position)
                        || Input.GetKey(KeyCode.Space))
                        {
                            // Shoot Bullet
                            Shoot();

                            animator.SetBool("walkingLeft", false);
                            animator.SetBool("isIdle", false);
                            animator.SetBool("attackingRight", false);
                            animator.SetBool("attackingDown", false);
                            animator.SetBool("attackingUp", false);
                            animator.SetBool("attackingLeft", true);

                            yield return new WaitForSeconds(0);

                            animator.SetBool("attackingLeft", false);
                        }
                    }

                    // If the player is walking Right or auto aim detects an enemy Right
                    else if (gm.shootDir.x > 0 && gm.shootDir.y > -.9f && gm.shootDir.y < 0.9f)
                    {
                        // If the player has NOT shot, and the dialogue is NOT triggered
                        // If the player is touching the shoot area, OR 
                        // the player is inputing arrow key movements
                        if (RectTransformUtility.RectangleContainsScreenPoint(gm.shootArea.GetComponent<RectTransform>(), gm.touch.position)
                        || Input.GetKey(KeyCode.Space))
                        {
                            // Shoot Bullet
                            Shoot();

                            animator.SetBool("walkingRight", false);
                            animator.SetBool("isIdle", false);
                            animator.SetBool("attackingDown", false);
                            animator.SetBool("attackingUp", false);
                            animator.SetBool("attackingLeft", false);
                            animator.SetBool("attackingRight", true);

                            yield return new WaitForSeconds(0);

                            animator.SetBool("attackingRight", false);
                        }
                    }

                    // if the player is not moving
                    else if (animator.GetBool("isIdle"))
                    {
                        // If the player has NOT shot, and the dialogue is NOT triggered
                        // If the player is touching the shoot area, OR 
                        // the player is inputing arrow key movements
                        if (RectTransformUtility.RectangleContainsScreenPoint(gm.shootArea.GetComponent<RectTransform>(), gm.touch.position)
                        || Input.GetKey(KeyCode.Space))
                        {
                            // Shoot Bullet
                            Shoot();

                            animator.SetBool("attackingLeft", false);
                            animator.SetBool("attackingUp", false);
                            animator.SetBool("attackingRight", false);
                            animator.SetBool("isIdle", false);

                            animator.SetBool("attackingDown", true);

                            yield return new WaitForSeconds(0);

                            animator.SetBool("attackingDown", false);
                        }
                    }
                }

                // If auto aim is OFF
                else
                {
                    // If the player is walking Up or auto aim detects an enemy Up
                    if (animator.GetBool("walkingUp"))
                    {
                        // If the player has NOT shot, and the dialogue is NOT triggered
                        // If the player is touching the shoot area, OR 
                        // the player is inputing arrow key movements
                        if (RectTransformUtility.RectangleContainsScreenPoint(gm.shootArea.GetComponent<RectTransform>(), gm.touch.position)
                        || Input.GetKey(KeyCode.Space))
                        {
                            // Shoot Bullet
                            Shoot();

                            animator.SetBool("walkingUp", false);
                            animator.SetBool("isIdle", false);
                            animator.SetBool("attackingLeft", false);
                            animator.SetBool("attackingRight", false);
                            animator.SetBool("attackingDown", false);
                            animator.SetBool("attackingUp", true);

                            yield return new WaitForSeconds(0);

                            animator.SetBool("attackingUp", false);
                        }
                    }

                    // If the player is walking Down or auto aim detects an enemy Down
                    else if (animator.GetBool("walkingDown"))
                    {
                        // If the player has NOT shot, and the dialogue is NOT triggered
                        // If the player is touching the shoot area, OR 
                        // the player is inputing arrow key movements
                        if (RectTransformUtility.RectangleContainsScreenPoint(gm.shootArea.GetComponent<RectTransform>(), gm.touch.position)
                        || Input.GetKey(KeyCode.Space))
                        {
                            // Shoot Bullet
                            Shoot();

                            animator.SetBool("walkingDown", false);
                            animator.SetBool("isIdle", false);
                            animator.SetBool("attackingLeft", false);
                            animator.SetBool("attackingRight", false);
                            animator.SetBool("attackingUp", false);
                            animator.SetBool("attackingDown", true);

                            yield return new WaitForSeconds(0);

                            animator.SetBool("attackingDown", false);
                        }
                    }

                    // If the player is walking Left or auto aim detects an enemy Left
                    else if (animator.GetBool("walkingLeft"))
                    {
                        // If the player has NOT shot, and the dialogue is NOT triggered
                        // If the player is touching the shoot area, OR 
                        // the player is inputing arrow key movements
                        if (RectTransformUtility.RectangleContainsScreenPoint(gm.shootArea.GetComponent<RectTransform>(), gm.touch.position)
                        || Input.GetKey(KeyCode.Space))
                        {
                            // Shoot Bullet
                            Shoot();

                            animator.SetBool("walkingLeft", false);
                            animator.SetBool("isIdle", false);
                            animator.SetBool("attackingRight", false);
                            animator.SetBool("attackingDown", false);
                            animator.SetBool("attackingUp", false);
                            animator.SetBool("attackingLeft", true);

                            yield return new WaitForSeconds(0);

                            animator.SetBool("attackingLeft", false);
                        }
                    }

                    // If the player is walking Right or auto aim detects an enemy Right
                    else if (animator.GetBool("walkingRight"))
                    {
                        // If the player has NOT shot, and the dialogue is NOT triggered
                        // If the player is touching the shoot area, OR 
                        // the player is inputing arrow key movements
                        if (RectTransformUtility.RectangleContainsScreenPoint(gm.shootArea.GetComponent<RectTransform>(), gm.touch.position)
                        || Input.GetKey(KeyCode.Space))
                        {
                            // Shoot Bullet
                            Shoot();

                            animator.SetBool("walkingRight", false);
                            animator.SetBool("isIdle", false);
                            animator.SetBool("attackingDown", false);
                            animator.SetBool("attackingUp", false);
                            animator.SetBool("attackingLeft", false);
                            animator.SetBool("attackingRight", true);

                            yield return new WaitForSeconds(0);

                            animator.SetBool("attackingRight", false);
                        }
                    }

                    // if the player is not moving
                    else if (animator.GetBool("isIdle"))
                    {
                        // If the player has NOT shot, and the dialogue is NOT triggered
                        // If the player is touching the shoot area, OR 
                        // the player is inputing arrow key movements
                        if (RectTransformUtility.RectangleContainsScreenPoint(gm.shootArea.GetComponent<RectTransform>(), gm.touch.position)
                        || Input.GetKey(KeyCode.Space))
                        {
                            // Shoot Bullet
                            Shoot();

                            animator.SetBool("attackingLeft", false);
                            animator.SetBool("attackingUp", false);
                            animator.SetBool("attackingRight", false);
                            animator.SetBool("isIdle", false);

                            animator.SetBool("attackingDown", true);

                            yield return new WaitForSeconds(0);

                            animator.SetBool("attackingDown", false);
                        }
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
            
            // If moving Up
            if (!gm.autoAimOn && myVelocity.y > 0 && myVelocity.x > -0.9f && myVelocity.x < 0.9f)
            {
                // Rotate gun holder
                gunHolder.localEulerAngles = new Vector3(0, 0, 90);
            }

            // If moving Down
            else if (!gm.autoAimOn && myVelocity.y < 0 && myVelocity.x > -0.9f && myVelocity.x < 0.9f)
            {
                // Rotate gun holder
                gunHolder.localEulerAngles = new Vector3(0, 0, -90);
            }

            // If moving Right
            else if (!gm.autoAimOn && myVelocity.x > 0 && myVelocity.y > -0.9f && myVelocity.y < 0.9f)
            {
                // Rotate gun holder
                gunHolder.localEulerAngles = new Vector3(0, 0, 0);
            }

            // If moving Left
            else if (!gm.autoAimOn && myVelocity.x < 0 && myVelocity.y > -0.9f && myVelocity.y < 0.9f)
            {
                // Rotate gun holder
                gunHolder.localEulerAngles = new Vector3(0, 0, -180);
            }

            // If not moving
            else if (!gm.autoAimOn && myVelocity == Vector2.zero)
            {
                // Rotate gun holder
                gunHolder.localEulerAngles = new Vector3(0, 0, -90);
            }

            // Disable movement and shoot joysticks if player is in dialogue
            if (FindObjectOfType<DialogueManager>().dialogueTriggered)
            {
                rb.velocity = new Vector2(0, 0);
                gamemode.joystickHolder.SetActive(false);
            }
        }
    }

    void MovementAnimation()
    {
        // Check if player is NOT moving, to play correct animation
        if (rb.velocity == Vector2.zero)
        {
            animator.SetBool("walkingLeft", false);
            animator.SetBool("walkingUp", false);
            animator.SetBool("walkingDown", false);
            animator.SetBool("walkingRight", false);
            if (!hasShot && !dg.dialogueTriggered)
            {
                animator.SetBool("isIdle", true);
            }
            return;
        }

        // If player is moving Up play animation accordingly
        else if (myVelocity.y > 0 && myVelocity.x > -0.9f && myVelocity.x < 0.9f)
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("walkingRight", false);
            animator.SetBool("walkingLeft", false);
            animator.SetBool("walkingDown", false);
            animator.SetBool("walkingUp", true);
            return;
        }

        // If player is moving down play animation accordingly
        else if (myVelocity.y < 0 && myVelocity.x > -0.9f && myVelocity.x < 0.9f)
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("walkingRight", false);
            animator.SetBool("walkingLeft", false);
            animator.SetBool("walkingUp", false);
            animator.SetBool("walkingDown", true);
            return;
        }

        // If player is moving right play animation accordingly
        else if (myVelocity.x > 0 && myVelocity.y > -0.9f && myVelocity.y < 0.9f)
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("walkingLeft", false);
            animator.SetBool("walkingUp", false);
            animator.SetBool("walkingDown", false);
            animator.SetBool("walkingRight", true);
            return;
        }

        // If player is moving Left play animation accordingly
        else if (myVelocity.x < 0 && myVelocity.y > -0.4f && myVelocity.y < 0.4f)
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("walkingRight", false);
            animator.SetBool("walkingUp", false);
            animator.SetBool("walkingDown", false);
            animator.SetBool("walkingLeft", true);
            return;
        }
    }
 }  
