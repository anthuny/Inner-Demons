using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //Bullet
    public float bulletSpeed;
    public float bulletDist;
    private bool hasShot;
    public float bulletDamage;
    private float shotTimer = 0f;
    public float shotCooldown = 0.25f;
    public GameObject bullet;
    public Transform gunHolder;

    //Player
    public float playerSpeed;
    public float p_maxHealth = 100;
    public float p_curHealth;
    public float p_healthDeath = 0;
    private Image p_HealthBar;
    private Rigidbody2D rb;
    private Vector3 inputVector;
    public bool isFire;
    public bool isWater;
    public bool isEarth;
    private SpriteRenderer sr;
    public float fireDamage;
    public float waterDamage;
    public float earthDamage;

    //Room
    public GameObject doors;
    private GameObject room;

    private void Start()
    {
        name = "Player";
        rb = GetComponent<Rigidbody2D>();
        p_HealthBar = GameObject.Find("/Player/Canvas/Health").GetComponent<Image>();
        sr = GetComponentInChildren<SpriteRenderer>();
        Reset();
    }

    void Reset()
    {
        p_curHealth = p_maxHealth;
        p_HealthBar.fillAmount = 1f;
        isFire = true;
    }
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Shoot();
        }

        if (hasShot)
        {
            ShotCooldown();
        }
    }

    void FixedUpdate()
    {
        Movement();
        LookAtMouse();
        GodMode();
        ElementManager();
    }

    void ElementManager()
    {
        // If player presses 1, switch to fire element
        if (Input.GetKeyDown("1"))
        {
            isEarth = false;
            isWater = false;
            isFire = true;
        }

        // If player pressed 2, switch to water element
        if (Input.GetKeyDown("2"))
        {
            isFire = false;
            isEarth = false;
            isWater = true;
        }

        // If player presses 3, switch to earth element
        if (Input.GetKeyDown("3"))
        {
            isFire = false;
            isWater = false;
            isEarth = true;
        }

        // If player is Fire element, change visually
        if (isFire)
        {
            sr.color = Color.red;
        }

        // If player is Fire element, change visually
        if (isWater)
        {
            sr.color = Color.blue;
        }

        // If player is Fire element, change visually
        if (isEarth)
        {
            sr.color = Color.green;
        }
    }
    void GodMode()
    {
        if (Input.GetKeyDown("g"))
        {
            p_healthDeath = -10000;
        }
    }

    void Shoot()
    {
        if (!hasShot)
        {
            hasShot = true;
            Instantiate(bullet, gunHolder.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().playerBullet = true;
        }
    }

    public void DecreaseHealth(float bulletDamage)
    {
        if (p_curHealth > p_healthDeath)
        {
            p_curHealth -= bulletDamage;
            p_HealthBar.fillAmount -= bulletDamage / 100;
        }

        if (p_curHealth <= p_healthDeath)
        {
            Destroy(gameObject);
        }
    }

    void ShotCooldown()
    {
        if (shotTimer <= shotCooldown)
        {
            shotTimer += Time.deltaTime;
        }

        if (shotTimer >= shotCooldown)
        {
            hasShot = false;
            shotTimer = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //If player enters a room, close all doors
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

    }
    void LookAtMouse()
    {
        Vector3 dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Movement()
    {
        inputVector = new Vector2(Input.GetAxisRaw("Horizontal") * playerSpeed, Input.GetAxisRaw("Vertical") * playerSpeed);
        rb.velocity = inputVector;
    }
 }  
