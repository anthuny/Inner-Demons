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
    private GameObject room;

    //Memory
    private GameObject memory;

    private void Start()
    {
        gm = FindObjectOfType<Gamemode>();
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
    }

    void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            Shoot();
        }

        if (hasShot)
        {
            ShotCooldown();
        }

        AnimationController();
        Movement();
        StartCoroutine(LookAtMouse());
        GodMode();
        ElementManager();
    }

    void ElementManager()
    {
        // If player presses 1, switch to fire element
        if (Input.GetKeyDown("1"))
        {
            gamemode.isEarth = false;
            gamemode.isWater = false;
            gamemode.isFire = true;
        }

        // If player pressed 2, switch to water element
        if (Input.GetKeyDown("2"))
        {
            gamemode.isFire = false;
            gamemode.isEarth = false;
            gamemode.isWater = true;
        }

        // If player presses 3, switch to earth element
        if (Input.GetKeyDown("3"))
        {
            gamemode.isFire = false;
            gamemode.isWater = false;
            gamemode.isEarth = true;
        }

        // If player is Fire element, change visually
        if (gamemode.isFire)
        {
            //sr.color = Color.red;
        }

        // If player is Fire element, change visually
        if (gamemode.isWater)
        {
            //sr.color = Color.blue;
        }

        // If player is Fire element, change visually
        if (gamemode.isEarth)
        {
            //sr.color = Color.green;
        }
    }
    void GodMode()
    {
        // If the player presses 'G' their stats are increased
        if (Input.GetKeyDown("g"))
        {
            gamemode.p_healthDeath = -10000;
            gamemode.bulletSpeed += 20;
            gamemode.playerSpeed += 2.5f;
            gamemode.shotCooldown -= .1f;
        }

        if (Input.GetKeyDown("h"))
        {
            gamemode.p_healthDeath = -10000f;
        }
    }

    void Shoot()
    {
        // If the player has NOT shot, and the dialogue is NOT triggered
        if (!hasShot && !FindObjectOfType<DialogueManager>().dialogueTriggered)
        {
            hasShot = true;
            GameObject go = Instantiate(bullet, gunHolder.position, Quaternion.identity);
            go.GetComponent<Bullet>().weaponHolder = gunHolder;
            bullet.GetComponent<Bullet>().playerBullet = true;
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
        // If player enters a room, close all doors
        if (other.gameObject.tag == ("RoomCollider"))
        {
            doors.SetActive(true);

            // Get reference to root of this object
            // Spawn enemies in that room
            Transform roomTrans;
            roomTrans = other.transform.root;
            room = roomTrans.gameObject;
            room.GetComponent<Room>().beenEntered = true;
            room.GetComponent<Room>().StartCoroutine("SpawnEnemies");
        }

        // If player talks to a memory
        if (other.gameObject.tag == ("Memory"))
        {
            memory = other.gameObject;

            // Only allow player to interact with each memory once
            if (!memory.GetComponent<Memory>().interacted)
            {
                memory.GetComponent<Memory>().interacted = true;

                if (!FindObjectOfType<DialogueManager>().dialogueTriggered)
                {
                    TriggerDialogue();
                }
            }
        }
    }

    public void TriggerDialogue()
    {
        // Get reference to the memory's sentences, and send them to the dialogue manager
        FindObjectOfType<DialogueManager>().StartDialogue(memory.GetComponent<Memory>().dialogue);
    }

    IEnumerator LookAtMouse()
    {
        Debug.Log(animator.speed);

        // if the player is NOT in a conversation, they can look around
        if (!FindObjectOfType<DialogueManager>().dialogueTriggered)
        {
            if (animator.GetBool("attackingLeft") || (animator.GetBool("attackingRight")) || (animator.GetBool("attackingUp")) || (animator.GetBool("attackingDown")))
            {
                // This is a temporary fix, Needs speed to increase depending on how small gm.shotSpeed is
                animator.speed = 1.5f;
            }

            if (!animator.GetBool("attackingLeft") && (!animator.GetBool("attackingRight")) && (!animator.GetBool("attackingUp")) && (!animator.GetBool("attackingDown")))
            {
                animator.speed = 1;
            }

            Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            //Rotate the Weapon holder to rotate towards mouse location
            transform.GetChild(1).gameObject.GetComponent<Transform>().rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // If facing RIGHT
            if (angle > -45 && angle < 45)
            {
                if (Input.GetMouseButton(0))
                {
                    animator.SetBool("attackingRight", true);
                    animator.SetBool("attackingLeft", false);
                    animator.SetBool("attackingDown", false);
                    animator.SetBool("attackingUp", false);

                    yield return new WaitForSeconds(0);

                    animator.SetBool("attackingRight", false);
                }
            }

            // If facing UP
            if (angle > 45 && angle < 135)
            {
                if (Input.GetMouseButton(0))
                {
                    animator.SetBool("attackingUp", true);
                    animator.SetBool("attackingLeft", false);
                    animator.SetBool("attackingDown", false);
                    animator.SetBool("attackingRight", false);

                    yield return new WaitForSeconds(0);

                    animator.SetBool("attackingUp", false);
                }
            }

            // If facing LEFT
            if (angle > 135 && angle < 180 || angle > -180 && angle < -135)
            {
                if (Input.GetMouseButton(0))
                {
                    animator.SetBool("attackingLeft", true);
                    animator.SetBool("attackingRight", false);
                    animator.SetBool("attackingDown", false);
                    animator.SetBool("attackingUp", false);

                    yield return new WaitForSeconds(0);

                    animator.SetBool("attackingLeft", false);
                }
            }

            // If facing DOWN
            if (angle > -135 && angle < -45)
            {
                if (Input.GetMouseButton(0))
                {
                    animator.SetBool("attackingDown", true);
                    animator.SetBool("attackingLeft", false);
                    animator.SetBool("attackingRight", false);
                    animator.SetBool("attackingUp", false);

                    yield return new WaitForSeconds(0);

                    animator.SetBool("attackingDown", false);
                }
            }
        }
    }

    void Movement()
    {
        // if the player is NOT in a conversation, they can move
        if (!FindObjectOfType<DialogueManager>().dialogueTriggered)
        {
            inputVector = new Vector2(Input.GetAxisRaw("Horizontal") * gamemode.playerSpeed, Input.GetAxisRaw("Vertical") * gamemode.playerSpeed);
            rb.velocity = inputVector;
        }

        // If the player IS in a conversation, stop all velocity
        if (FindObjectOfType<DialogueManager>().dialogueTriggered)
        {
            rb.velocity = new Vector2(0, 0);
        }
    }

    void AnimationController()
    {
        // Check if player is NOT moving, to play correct animation
        if (rb.velocity == new Vector2(0, 0))
        {
            animator.SetBool("isIdle", true);
            animator.SetBool("walkingLeft", false);
            animator.SetBool("walkingUp", false);
            animator.SetBool("walkingDown", false);
            animator.SetBool("walkingRight", false);
        }

        // If player is moving Left/Right, play animation accordingly
        if (rb.velocity.x > 0)
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("walkingLeft", false);
            animator.SetBool("walkingUp", false);
            animator.SetBool("walkingDown", false);
            animator.SetBool("walkingRight", true);
        }

        else if (rb.velocity.x < 0)
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("walkingRight", false);
            animator.SetBool("walkingUp", false);
            animator.SetBool("walkingDown", false);
            animator.SetBool("walkingLeft", true);
        }

        // If player is moving Up/Down, play animation accordingly
        else if (rb.velocity.y > 0)
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("walkingRight", false);
            animator.SetBool("walkingLeft", false);
            animator.SetBool("walkingDown", false);
            animator.SetBool("walkingUp", true);
        }

        else if (rb.velocity.y < 0)
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("walkingRight", false);
            animator.SetBool("walkingLeft", false);
            animator.SetBool("walkingUp", false);
            animator.SetBool("walkingDown", true);
        }
    }
 }  
