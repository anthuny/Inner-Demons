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
    private float hInput;
    private float vInput;
    public Camera cam;
    public float p_maxHealth = 100;
    public float p_curHealth;
    public float p_healthDeath = 0;
    private Image p_HealthBar;

    private Rigidbody rb;
    private Vector3 inputVector;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        p_HealthBar = GameObject.Find("PlayerHealth").GetComponent<Image>();
        Reset();
    }

    void Reset()
    {
        p_curHealth = p_maxHealth;
        p_HealthBar.fillAmount = 1f;
    }
    void Update()
    {
        hInput = Input.GetAxisRaw("Horizontal");
        vInput = Input.GetAxisRaw("Vertical");

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
    }

    void GodMode()
    {
        if (Input.GetKeyDown("g"))
        {
            p_healthDeath = -1000;
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

    void LookAtMouse()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition); ;
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {

            Vector3 target = hit.point;
            target.y = transform.position.y;
            //target.y = transform.localScale.y / 2f;

            transform.LookAt(target);
        }
    }

    void Movement()
    {
        inputVector = new Vector3(Input.GetAxisRaw("Horizontal") * playerSpeed, rb.velocity.y, Input.GetAxisRaw("Vertical") * playerSpeed);
        rb.velocity = inputVector;
    }
 }
