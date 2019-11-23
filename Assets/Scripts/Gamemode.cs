using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gamemode : MonoBehaviour
{
    [Header("Current Platform")]
    public bool usingPC;

    [Header("General")]
    public GameObject playerPrefab;
    public int enemyCount = 0;
    public float chooseDamageInc;
    public float chooseHealthInc;
    public float chooseE_DamageInc;
    public float chooseProjectileSpeedInc;
    public float chooseReloadSpeedInc;
    public Transform playerSpawnPoint;
    public GameObject player;
    private Transform spawnPos;
    private GameDownManager gdm;
    private DialogueManager dm;

    [Header("Choices")]
    public int arrogance;
    public int ignorance;
    public int morality;
    public bool arroganceHighest;
    public bool ignoranceHighest;
    public bool moralityHighest;
    public int maxChoice;

    [Header("Element Handler")]

    [Header("Touch Inputs / Joysticks")]
    public GameObject joystickHolder;
    public Joystick joystickMove;
    public float minDragDistance;
    public Vector2 startPos;
    private Vector2 direction;
    public GameObject shootArea;
    public GameObject waterButtonArea;
    public GameObject fireButtonArea;
    public GameObject earthButtonArea;
    public Touch touch;

    [Header("Player Statistics")]
    public float playerSpeedCur;
    public float playerSpeedDef;
    public float playerSpeedDead = 0;
    public float p_maxHealth = 100;
    public float p_curHealth;
    public float p_healthDeath = 0;
    public bool isFire;
    public bool isWater;
    public bool isEarth;
    public int currentElement;
    public float fireDamage;
    public float waterDamage;
    public float earthDamage;
    public bool autoAimOn;
    public Transform nearestEnemy;
    public Vector2 shootDir;

    [Header("Player Attacking Statistics")]
    public float bulletSpeed;
    public float bulletSpeedDef;
    public float bulletDist;
    public float bulletDistDef;
    public float bulletDamage;
    public float bulletDamageDef;
    public float shotCooldown = 0.25f;
    public float shotCooldownDef;
    public float autoAimDis;

    [Header("Enemy Ranged Statistics")]
    public float e_MoveSpeed;
    public float e_ChaseSpeed;
    public float e_EvadeSpeed;
    public float e_MaxHealth = 100;

    public float e_HealthDeath = 0;
    public float e_ViewDis;
    public float evadetimerMax;
    public float bossDeathSpeed = 0;

    [Header("Enemy Ranged Attacking Statistics")]
    public float e_BulletDamage;
    public float e_ShotCooldown;
    public float e_BulletSpeed;
    public float e_BulletDist;

    [Header("Memory")]
    public GameObject talkButton;

    // Start is called before the first frame update
    void Start()
    {
        spawnPos = GameObject.Find("EGOSpawnPoint").transform;
        gdm = FindObjectOfType<GameDownManager>();
        dm = FindObjectOfType<DialogueManager>();

        playerSpeedCur = playerSpeedDef;
    }

    // Update is called once per frame
    void Update()
    {
        ElementManager();
        CalculateChoiceHigh();
        PlayerDeath();
    }

    void PlayerDeath()
    {
        if (player)
        {
            talkButton = GameObject.Find("Talk Button");
            talkButton.GetComponent<Button>().onClick.AddListener(dm.TriggerDialogue);
            talkButton.GetComponent<Button>().interactable = false;
        }

        if (!player)
        {
            player = FindObjectOfType<Player>().gameObject;
            gdm.youFaintedBut.gameObject.SetActive(true);
            gdm.restartBut.gameObject.SetActive(true);
            gdm.menuBut.gameObject.SetActive(true);
            arrogance = 0;
            ignorance = 0;
            morality = 0;
        }
    }

    public void CalculateChoiceHigh()
    {
        // Check what the highest choice is out of all choices
        maxChoice = Mathf.Max(arrogance, ignorance, morality);


        if (maxChoice == arrogance && maxChoice == ignorance && maxChoice == morality)
        {
            arroganceHighest = false;
            ignoranceHighest = false;
            moralityHighest = false;
            return;
        }

        // If maxchoice is arrogance and NOT balanced
        else if (maxChoice == arrogance)
        {
            arroganceHighest = true;
            ignoranceHighest = false;
            moralityHighest = false;
        }

        // If maxchoice is igorance and NOT balanced
        else if(maxChoice == ignorance)
        {
            ignoranceHighest = true;
            arroganceHighest = false;
            moralityHighest = false;
        }

        // If maxchoice is morality and NOT balanced
        else if(maxChoice == morality)
        {
            moralityHighest = true;
            arroganceHighest = false;
            ignoranceHighest = false;
        }
    }

    // Increase Statistics
    public void IncreaseStatistics()
    {
        // Increase projectile speed
        bulletSpeed += chooseProjectileSpeedInc;

        // Increase reload speed
        shotCooldown -= chooseReloadSpeedInc;
    }

    public void IncreaseDamage()
    {
        bulletDamage += chooseDamageInc;
    }

    public void IncreaseHealth()
    {
        p_maxHealth += chooseHealthInc;
    }

    public void IncreaseEnemyDamage()
    {
        e_BulletDamage += chooseE_DamageInc;
    }

    void ElementManager()
    {
        if (player)
        {
            // If player presses a, switch to left element
            if (Input.GetKeyDown("e"))
            {
                currentElement -= 1;
            }

            // If player pressed d, switch to right element
            if (Input.GetKeyDown("q"))
            {
                currentElement += 1;
            }

            // Ensure current element doesn't go out of bounds
            if (currentElement == -1)
            {
                currentElement = 2;
            }

            // Ensure current element doesn't go out of bounds
            if (currentElement == 3)
            {
                currentElement = 0;
            }

            if (Input.touchCount > 0)
            {
                // If there is just one touch on screen, set touch to that one
                if (Input.touchCount == 1)
                {
                    touch = Input.touches[0];
                }
                // If there is already a touch on the screen, set touch to the next one 
                else if (Input.touchCount == 2)
                {
                    touch = Input.touches[1];
                }
                //  If there is already TWO touches on the screen, set touch to the next one 
                else if (Input.touchCount == 3)
                {
                    touch = Input.touches[2];
                }

                // Check if the user touched the water button area
                if (RectTransformUtility.RectangleContainsScreenPoint(waterButtonArea.GetComponent<RectTransform>(), touch.position))
                {
                    currentElement = 1;
                }

                // Check if the user touched the water button area
                if (RectTransformUtility.RectangleContainsScreenPoint(fireButtonArea.GetComponent<RectTransform>(), touch.position))
                {
                    currentElement = 0;
                }

                // Check if the user touched the water button area
                if (RectTransformUtility.RectangleContainsScreenPoint(earthButtonArea.GetComponent<RectTransform>(), touch.position))
                {
                    currentElement = 2;
                }
            }
        }
    }
}
