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
    private bool hasShot;
    private float shotTimer = 0f;

    //Player
    private Vector3 inputVector;
    private Image p_HealthBar;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public Animator animator;

    //Gamemode
    private Gamemode gm;

    //Room
    public GameObject doors;
    public GameObject room;

    //Memory
    public GameObject memory;
    public bool playerStill;
    public bool canInteract;
    public GameObject talkButton;
    private DialogueManager dg;


    private void Start()
    {
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
        gamemode.p_curHealth = gamemode.p_maxHealth;
        p_HealthBar.fillAmount = 1f;
        gamemode.isFire = true;
        
        talkButton.GetComponentInChildren<CanvasGroup>().alpha = 0;
        talkButton.SetActive(true);
    }

    private void Update()
    {
        MovementAnimation();
    }
    void FixedUpdate()
    {

        if (hasShot)
        {
            ShotCooldown();
        }


        Movement();
        StartCoroutine(ShootAnimation());
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
        if (shotTimer <= gamemode.shotCooldown)
        {
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
            Room roomScript = room.GetComponent<Room>();
            Destroy(roomScript);
        }
    }

    void Shoot()
    {
        // If the player has NOT shot, and the dialogue is NOT triggered
        if (!hasShot && !dg.dialogueTriggered)
        {
            hasShot = true;
            GameObject go = Instantiate(bullet, gunHolder.position, Quaternion.identity);
            go.GetComponent<Bullet>().weaponHolder = gunHolder;
            bullet.GetComponent<Bullet>().playerBullet = true;
        }
    }

    IEnumerator ShootAnimation()
    {
        // if the player is NOT in a conversation, they can look around
        if (!FindObjectOfType<DialogueManager>().dialogueTriggered)
        {
            if (animator.GetBool("attackingLeft") || (animator.GetBool("attackingRight")) || (animator.GetBool("attackingUp")) || (animator.GetBool("attackingDown")))
            {
                // This is a temporary fix, Needs speed to increase depending on how small gm.shotSpeed is
                animator.speed = 1f;
            }

            if (!animator.GetBool("attackingLeft") && (!animator.GetBool("attackingRight")) && (!animator.GetBool("attackingUp")) && (!animator.GetBool("attackingDown")))
            {
                animator.speed = 1;
            }

            // If the player is touching the screen, OR 
            // the player is imputing arrow key movements
            if (Input.touchCount > 0 || Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow))
            {
                // If Joystick moves upwards
                if (gamemode.joystickShoot.Vertical >= .1f || Input.GetKey(KeyCode.UpArrow))
                {
                    // Rotate gun holder
                    gunHolder.localEulerAngles = new Vector3(0, 0, 90);

                    // Shoot Bullet
                    Shoot();

                    animator.SetBool("attackingLeft", false);
                    animator.SetBool("attackingDown", false);
                    animator.SetBool("attackingRight", false);
                    animator.SetBool("attackingUp", true);

                    yield return new WaitForSeconds(0);
                }

                // If Joystick moves downwards
                else if (gamemode.joystickShoot.Vertical <= -.1f || Input.GetKey(KeyCode.DownArrow))
                {
                    // Rotate gun holder
                    gunHolder.localEulerAngles = new Vector3(0, 0, -90);

                    // Shoot Bullet
                    Shoot();

                    animator.SetBool("attackingLeft", false);
                    animator.SetBool("attackingRight", false);
                    animator.SetBool("attackingUp", false);
                    animator.SetBool("attackingDown", true);

                    yield return new WaitForSeconds(0);
                }

                // If Joystick is moving Left
                else if (gamemode.joystickShoot.Horizontal <= -.1f || Input.GetKey(KeyCode.LeftArrow))
                {
                    // Rotate gun holder
                    gunHolder.localEulerAngles = new Vector3(0, 0, -180);

                    // Shoot Bullet
                    Shoot();

                    animator.SetBool("attackingRight", false);
                    animator.SetBool("attackingDown", false);
                    animator.SetBool("attackingUp", false);
                    animator.SetBool("attackingLeft", true);

                    yield return new WaitForSeconds(0);
                }

                // If Joystick is moving Right
                else if (gamemode.joystickShoot.Horizontal >= .1f || Input.GetKey(KeyCode.RightArrow))
                {
                    // Rotate gun holder
                    gunHolder.localEulerAngles = new Vector3(0, 0, 0);

                    // Shoot Bullet
                    Shoot();

                    animator.SetBool("attackingLeft", false);
                    animator.SetBool("attackingDown", false);
                    animator.SetBool("attackingUp", false);
                    animator.SetBool("attackingRight", true);

                    yield return new WaitForSeconds(0);

                    //animator.SetBool("attackingRight", false);
                }

                // If the player is NOT shooting
                if (gamemode.joystickShoot.Horizontal >= -.1f && gamemode.joystickShoot.Horizontal <= .1f && gamemode.joystickShoot.Vertical >= -.1f && gamemode.joystickShoot.Vertical <= .1f)
                {
                    // Rotate gun holder
                    animator.SetBool("attackingLeft", false);
                    animator.SetBool("attackingDown", false);
                    animator.SetBool("attackingUp", false);
                    animator.SetBool("attackingRight", false);
                    gunHolder.localEulerAngles = new Vector3(0, 0, -90);
                }
            }
            //// If facing RIGHT
            //if (angle > -45 && angle < 45)
            //{
            //    if (Input.GetMouseButton(0))
            //    {
            //        animator.SetBool("attackingRight", true);
            //        animator.SetBool("attackingLeft", false);
            //        animator.SetBool("attackingDown", false);
            //        animator.SetBool("attackingUp", false);

            //        yield return new WaitForSeconds(0);

            //        animator.SetBool("attackingRight", false);
            //    }
            //}

            //// If facing UP
            //if (angle > 45 && angle < 135)
            //{
            //    if (Input.GetMouseButton(0))
            //    {
            //        animator.SetBool("attackingUp", true);
            //        animator.SetBool("attackingLeft", false);
            //        animator.SetBool("attackingDown", false);
            //        animator.SetBool("attackingRight", false);

            //        yield return new WaitForSeconds(0);

            //        animator.SetBool("attackingUp", false);
            //    }
            //}

            //// If facing LEFT
            //if (angle > 135 && angle < 180 || angle > -180 && angle < -135)
            //{
            //    if (Input.GetMouseButton(0))
            //    {
            //        animator.SetBool("attackingLeft", true);
            //        animator.SetBool("attackingRight", false);
            //        animator.SetBool("attackingDown", false);
            //        animator.SetBool("attackingUp", false);

            //        yield return new WaitForSeconds(0);

            //        animator.SetBool("attackingLeft", false);
            //    }
            //}

            //// If facing DOWN
            //if (angle > -135 && angle < -45)
            //{
            //    if (Input.GetMouseButton(0))
            //    {
            //        animator.SetBool("attackingDown", true);
            //        animator.SetBool("attackingLeft", false);
            //        animator.SetBool("attackingRight", false);
            //        animator.SetBool("attackingUp", false);

            //        yield return new WaitForSeconds(0);

            //        animator.SetBool("attackingDown", false);
            //    }
            //}
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

                if (rb.velocity == new Vector2(0,0))
                {
                    playerStill = true;
                } else
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

                // Only start moving the player if the joystick moves enough
                //if (gamemode.joystickMove.Horizontal >= .2f || gamemode.joystickMove.Vertical >= .2f)
                //{
                //    inputVector = new Vector2(gamemode.joystickMove.Horizontal * gamemode.playerSpeedCur, gamemode.joystickMove.Vertical * gamemode.playerSpeedCur);
                //    rb.velocity = inputVector;

                //}
                //// Only start moving the player if the joystick moves enough
                //else if (gamemode.joystickMove.Horizontal <= -.2f || gamemode.joystickMove.Vertical <= -.2f)
                //{
                //    inputVector = new Vector2(gamemode.joystickMove.Horizontal * gamemode.playerSpeedCur, gamemode.joystickMove.Vertical * gamemode.playerSpeedCur);
                //    rb.velocity = inputVector;
                //}
                else
                {
                    inputVector = Vector2.zero;
                    rb.velocity = inputVector;
                }
            }
        }

        // Disable movement and shoot joysticks if player is in dialogue
        if (FindObjectOfType<DialogueManager>().dialogueTriggered)
        {
            rb.velocity = new Vector2(0, 0);
            gamemode.joystickHolder.SetActive(false);
        }
    }

    void MovementAnimation()
    {
        Vector2 myVelocity = rb.velocity;
        myVelocity.Normalize();

        // Check if player is NOT moving, to play correct animation
        if (rb.velocity == Vector2.zero)
        {
            animator.SetBool("walkingLeft", false);
            animator.SetBool("walkingUp", false);
            animator.SetBool("walkingDown", false);
            animator.SetBool("walkingRight", false);
            animator.SetBool("isIdle", true);
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
