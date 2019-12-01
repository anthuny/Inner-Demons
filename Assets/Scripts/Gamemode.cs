using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Pathfinding;

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
    private GameObject cam;
    private Transform spawnPos;
    private GameDownManager gdm;
    private DialogueManager dm;
    private AIDestinationSetter aid;

    [Header("Camera Shake - Player Shoot")]
    public float shakeMagShoot;
    public float shakeRouShoot;
    public float shakeFadeIDurShoot;
    public float shakeFadeODurShoot;
    public Vector3 shakePosInfluenceShoot;
    public Vector3 shakeRotInfluenceShoot;

    [Header("Camera Shake - Hit")]
    public float shakeMagHit;
    public float shakeRouHit;
    public float shakeFadeIDurHit;
    public float shakeFadeODurHit;
    public Vector3 shakePosInfluenceHit;
    public Vector3 shakeRotInfluenceHit;

    [Header("Camera Shake - Door Close")]
    public float shakeMagDClose;
    public float shakeRouDClose;
    public float shakeFadeIDurDClose;
    public float shakeFadeODurDClose;
    public Vector3 shakePosInfluenceDClose;
    public Vector3 shakeRotInfluenceDClose;
    [HideInInspector]
    public bool depthShaking;
    public float depthCamDefAmount;
    public float depthDecAmount;
    public float depthDecSpeed;
    private bool retracting;

    [Header("Screen Freeze")]
    public float freezeDur;
    public float curFreezeDur = 0;
    public bool isFrozen;
    private bool doneOnce;

    [Header("Camera")]
    public float camSmoothTime = 0.2f;

    [Header("Bullets")]
    public float p_IncScaleRate;
    public float p_MaxScaleX;
    public float p_MaxScaleY;
    public float e_IncScaleRate;
    public float e_MaxScaleX;
    public float e_MaxScaleY;
    public GameObject bulletDeathParticle;

    [Header("Enemy Element Background")]
    public float incScaleRate;
    public float maxScaleX;
    public float maxScaleY;
    public float minScaleX;
    public float minScaleY;
    public bool gettingBigger;

    [Header("Choices")]
    public int arrogance;
    public int ignorance;
    public int morality;
    public bool arroganceHighest;
    public bool ignoranceHighest;
    public bool moralityHighest;
    public int maxChoice;

    [Header("Touch Inputs / Joysticks")]
    public GameObject joystickHolder;
    public Joystick joystickMove;
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
    public bool p_CanSeeTarget;
    [HideInInspector]
    public Vector2 shootDir;

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
    public float e_EvadeSpeed;
    public float e_EvadeSpeedDef;
    public float e_MaxHealth = 100;

    public float e_HealthDeath = 0;
    public float e_ViewDis;
    public float evadeTimerCur;
    public float evadeTimerDef;
    public float bossDeathSpeed = 0;
    public float e_BulletScaleInc;
    public Vector2 e_ShootDir;

    public float e_BulletDamage;
    public float e_ShotCooldown;
    public float e_BulletSpeed;
    public float e_BulletDist;
    public float e_rangeOffset;

    [Header("Enemy AI")]
    public float enemyTooCloseDis = 5f;
    public float enemyOverlapSpeed;
    public float enemyOverlapEvadeTimer = 1.25f;

    [Header("Memory")]
    public GameObject talkButton;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera");
        spawnPos = GameObject.Find("EGOSpawnPoint").transform;
        gdm = FindObjectOfType<GameDownManager>();
        dm = FindObjectOfType<DialogueManager>();
        aid = FindObjectOfType<AIDestinationSetter>();

        playerSpeedCur = playerSpeedDef;
        evadeTimerCur = evadeTimerDef;
    }

    // Update is called once per frame
    void Update()
    {
        ElementManager();
        CalculateChoiceHigh();
        SetEnemyTarget();
        CameraDepthShake();


        if (isFrozen && !doneOnce)
        {
            doneOnce = true;
            StartCoroutine(DoFreeze());
        }
    }

    public void Freeze()
    {
        isFrozen = true;
    }

    IEnumerator DoFreeze()
    {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(freezeDur);

        Time.timeScale = 1f;
        isFrozen = false;
        doneOnce = false;
    }

    void SetEnemyTarget()
    {
        if (aid)
        {
            aid.target = player.transform;
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

    public void CameraDepthShake()
    {
        if (depthShaking)
        {
            //decrease the size of the camera
            cam.GetComponent<Camera>().orthographicSize -= 0.1f * depthDecSpeed;

            if (cam.GetComponent<Camera>().orthographicSize <= depthDecAmount)
            {
                depthShaking = false;
                retracting = true;
            }
        }

        if (!depthShaking && retracting)
        {
            if (cam.GetComponent<Camera>().orthographicSize <= depthDecAmount)
            {
                //increase the size of the camera if it's under lowest point
                cam.GetComponent<Camera>().orthographicSize += 0.1f * depthDecSpeed;
            }

            if (cam.GetComponent<Camera>().orthographicSize > depthDecAmount && cam.GetComponent<Camera>().orthographicSize <= depthCamDefAmount)
            {
                //increase the size of the camera if it's above
                cam.GetComponent<Camera>().orthographicSize += 0.1f * depthDecSpeed;
            }

            else
            {
                retracting = false;
            }
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

    public void TextStatBad()
    {
        // Increase player damage
        bulletDamage += chooseDamageInc;
    }

    public void TextStatNeutral()
    {
        // Increase Enemy's health
        e_MaxHealth += chooseHealthInc;
    }

    public void TextStatGood()
    {
        // Increase Enemy's Damage
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
                    currentElement = 0;
                }

                // Check if the user touched the water button area
                if (RectTransformUtility.RectangleContainsScreenPoint(fireButtonArea.GetComponent<RectTransform>(), touch.position))
                {
                    currentElement = 1;
                }

                // Check if the user touched the water button area
                if (RectTransformUtility.RectangleContainsScreenPoint(earthButtonArea.GetComponent<RectTransform>(), touch.position))
                {
                    currentElement = 2;
                }
            }
        }

        if (currentElement == 0)
        {
            isEarth = false;
            isFire = false;
            isWater = true;
        }

        if (currentElement == 1)
        {
            isEarth = false;
            isWater = false;
            isFire = true;
        }

        if (currentElement == 2)
        {
            isWater = false;
            isFire = false;
            isEarth = true;
        }

    }

    public void LaunchGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LaunchMenu()
    {
        gdm.gamePaused = false;
        Time.timeScale = 1;

        // turn pause buttons off
        gdm.pausedBut.gameObject.SetActive(false);
        gdm.continueBut.gameObject.SetActive(false);
        gdm.mainMenuBut.gameObject.SetActive(false);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
