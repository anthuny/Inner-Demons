using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Bullet
    public float bulletSpeed;
    public float bulletLife;
    private bool hasShot;
    private float shotTimer = 0f;
    public float shotCooldown = 0.25f;
    public GameObject bullet;
    public Transform gunHolder;

    //Player
    public float playerSpeed;
    private float hInput;
    private float vInput;
    public Camera cam;

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
    }

    void Shoot()
    {
        if (!hasShot)
        {
            hasShot = true;
            Instantiate(bullet, gunHolder.position, Quaternion.identity);
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
        if (vInput > 0)
        {
            transform.position += Vector3.forward * Time.deltaTime * playerSpeed;
        }
        else if (vInput < 0)
        {
            transform.position += Vector3.back * Time.deltaTime * playerSpeed;
        }

        //Horizontal Input
        if (hInput > 0)
        {
            transform.position += Vector3.right * Time.deltaTime * playerSpeed;
        }
        else if (hInput < 0)
        {
            transform.position += Vector3.left * Time.deltaTime * playerSpeed;
        }
    }

 }
