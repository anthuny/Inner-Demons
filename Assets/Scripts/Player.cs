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
    private bool fired;
    public GameObject[] enemiesInRange;
    private Vector2 myVelocity;

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

    public void Shoot()
    {
        //float Dist = Vector2.Distance(transform.position, )
        //// If player inputs to shoot
        //for (int i = 0; i < enemiesInRange.Length; i++)
        //{

        //}

        // If using PC
        if (gm.usingPC)
        {
            hasShot = true;
            GameObject go = Instantiate(bullet, gunHolder.position, Quaternion.identity);
            go.GetComponent<Bullet>().weaponHolder = gunHolder;
            bullet.GetComponent<Bullet>().playerBullet = true;
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

        // If using mobile
        if (!gm.usingPC)
        {
            if (gm.touch.phase == TouchPhase.Stationary && RectTransformUtility.RectangleContainsScreenPoint(gm.swipeArea.GetComponent<RectTransform>(), gm.touch.position))
            {
                hasShot = true;
                GameObject go2 = Instantiate(bullet, gunHolder.position, Quaternion.identity);
                go2.GetComponent<Bullet>().weaponHolder = gunHolder;
                bullet.GetComponent<Bullet>().playerBullet = true;
            }
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
                animator.speed = 1;
            }

            if (!animator.GetBool("attackingLeft") && (!animator.GetBool("attackingRight")) && (!animator.GetBool("attackingUp")) && (!animator.GetBool("attackingDown")))
            {
                animator.speed = 1;
            }

            // If the player is touching the screen, OR 
            // the player is inputing arrow key movements

            // TODO: NEED TO ALLOW PHONE INPUT IN THIS LINE. CURRENTLY ONLY PC WORKS WITH THIS. NEED TO FIND A WAY TO CHECK THIS (I COULDN'T TEST ON PHONE, THIS MIGHT WORK ON PHONE, CHECK
            // IF IT DOES, IF NOT, ADD INPUT HERE)
            if (Input.touchCount > 0 || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Debug.Log("Attacking up animation playing");
                // If the player is walking Up
                // If the player has NOT shot, and the dialogue is NOT triggered
                if (animator.GetBool("walkingUp") && !hasShot && !dg.dialogueTriggered)
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

                // If the player is walking Down
                // If the player has NOT shot, and the dialogue is NOT triggered
                else if (animator.GetBool("walkingDown") && !hasShot && !dg.dialogueTriggered)
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

                // If the player is walking Left
                // If the player has NOT shot, and the dialogue is NOT triggered
                else if (animator.GetBool("walkingLeft") && !hasShot && !dg.dialogueTriggered)
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

                // If the player is walking right
                // If the player has NOT shot, and the dialogue is NOT triggered
                else if (animator.GetBool("walkingRight") && !hasShot && !dg.dialogueTriggered)
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

                // if the player is not moving
                else if (animator.GetBool("isIdle") && !hasShot && !dg.dialogueTriggered)
                {

                    animator.SetBool("attackingLeft", false);
                    animator.SetBool("attackingDown", false);
                    animator.SetBool("attackingUp", false);
                    animator.SetBool("attackingRight", false);
                    animator.SetBool("isIdle", true);
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

            // If there are no enemies near the player
            // Rotate the gunHolder towards the direction the player is facing
            if (enemiesInRange.Length == 0)
            {
                // If moving Up
                if (rb.velocity.y > 0)
                {
                    // Rotate gun holder
                    gunHolder.localEulerAngles = new Vector3(0, 0, 90);
                }

                // If moving Down
                if (rb.velocity.y < 0)
                {
                    // Rotate gun holder
                    gunHolder.localEulerAngles = new Vector3(0, 0, -90);
                }

                // If moving Right
                if (rb.velocity.x > 0)
                {
                    // Rotate gun holder
                    gunHolder.localEulerAngles = new Vector3(0, 0, 0);
                }

                // If moving Left
                if (rb.velocity.x < 0)
                {
                    // Rotate gun holder
                    gunHolder.localEulerAngles = new Vector3(0, 0, -180);
                }

                // If not moving
                if (rb.velocity == Vector2.zero)
                {
                    // Rotate gun holder
                    gunHolder.localEulerAngles = new Vector3(0, 0, -90);
                }
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
