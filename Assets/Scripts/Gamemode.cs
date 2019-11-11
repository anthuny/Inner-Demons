using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gamemode : MonoBehaviour
{
    [Header("General")]
    public GameObject playerPrefab;
    public int enemyCount = 0;
    public float chooseDamageIncrease;
    public float chooseHealthIncrease;
    public float chooseE_DamageIncrease;
    private GameObject player;
    private Transform spawnPos;

    [Header("Player Statistics")]
    public float playerSpeed;
    public float p_maxHealth = 100;
    public float p_curHealth;
    public float p_healthDeath = 0;
    public bool isFire;
    public bool isWater;
    public bool isEarth;
    public float fireDamage;
    public float waterDamage;
    public float earthDamage;

    [Header("Player Attacking Statistics")]
    public float bulletSpeed;
    public float bulletDist;
    public float bulletDamage;
    public float shotCooldown = 0.25f;

    [Header("Enemy Ranged Statistics")]
    public float e_MoveSpeed;
    public float e_ChaseSpeed;
    public float e_EvadeSpeed;
    public float e_MaxHealth = 100;
    public float e_CurHealth;
    public float e_HealthDeath = 0;
    public float e_ViewDis;
    public float evadetimerMax;

    [Header("Enemy Ranged Attacking Statistics")]
    public float e_BulletDamage;
    public float e_ShotCooldown;
    public float e_BulletSpeed;
    public float e_BulletDist;

    // Start is called before the first frame update
    void Start()
    {
        spawnPos = GameObject.Find("EGOSpawnPoint").transform;
    }

    // Update is called once per frame
    void Update()
    {
        player = GameObject.Find("Player");

        if (player == null)
        {
            if (Input.GetKeyDown("r"))
            {
                Instantiate(playerPrefab, spawnPos.position, Quaternion.identity);
                player = GameObject.Find("Player");
            }
        }
    }
}
