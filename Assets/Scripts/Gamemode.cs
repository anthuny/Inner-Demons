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

    [Header("Player Health UI")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

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
    private GameDownManager gdm;
    private DialogueManager dm;
    private AIDestinationSetter aid;
    public bool inMemoryRoom;
    public bool inBossRoom;
    public GameObject pauseButton;
    public bool pausedPressed;
    public Text loadingText;
    public bool startPressed;

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

    [Header("Element UI")]
    public GameObject[] UIElements;

    [Header("Camera")]
    public float camSmoothTime = 0.2f;

    [Header("Regular Bullets")]
    public float p_IncScaleRate;
    public float p_MaxScaleX;
    public float p_MaxScaleY;
    public float e_IncScaleRate;
    public float e_MaxScaleX;
    public float e_MaxScaleY;
    public GameObject bulletDeathParticle;

    [Header("Boss Bullets")]
    public float e_BossIncScaleRate;

    [Header("Player Element Background")]
    private Animator playerEleBGAnimC;

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
    public GameObject waterButtonVis;
    public GameObject fireButtonVis;
    public GameObject earthButtonVis;

    public Touch touch;

    [Header("Player Statistics")]
    public float playerSpeedCur;
    public float playerSpeedDef;
    public float playerSpeedDead = 0;
    public float p_maxHealth = 5;
    public float p_curHealth;
    public float p_healthDeath = 0;
    public bool p_IsFire;
    public bool p_IsWater;
    public bool p_IsEarth;
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
    public float e_MoveSpeedDef;
    public float e_EvadeSpeed;
    public float e_EvadeSpeedDef;
    public float e_MaxHealth = 100;
    public float e_MaxHealthDef;

    public float e_HealthDeath = 0;
    public float e_ViewDis;
    public float evadeTimerCur;
    public float evadeTimerDef;

    public float bossDeathSpeed = 0;

    public float e_BulletScaleInc;
    public Vector2 e_ShootDir;

    public float e_BulletDamage;
    public float e_BulletDamageDef;
    public float e_ShotCooldown;
    public float e_BulletSpeed;
    public float e_BulletDist;
    public float e_rangeOffset;
    public float e_bulletSize;
    public int e_CurElement;
    float time;
    [HideInInspector]
    public float switchTime;

    public float switchTimeMin;
    public float switchTimeMax;
    public bool e_IsFire;
    public bool e_IsWater;
    public bool e_IsEarth;

    // All boss statistics are numbers as percentages. 1 = 100% of regular value
    [Header("Boss Statistics")]
    public float bossBulletIncScaleRateCur;
    public float bossBulletIncScaleRateDef;
    public float bossBulletDamageCur;
    public float bossBulletDamageDef;
    public float bossBulletSpeedCur;
    public float bossbulletSpeedDef;
    public float bossBulletDistCur;
    public float bossBulletDistDef;
    public float bossScaleCur;
    public float bossScaleDef;
    public float bossSpeedCur;
    public float bossSpeedDef;
    public float bossMaxHealthCur;
    public float bossMaxHealthDef;
    public float bossShotCooldownCur;
    public float bossShotCooldownDef;
    public float depSpeedCur;
    public float bossBulletSizeInfCur;
    public float bossBulletSizeInfDef;
    public float bossEnragedBulletSizeInfCur;
    public float bossEnragedBulletSizeInfDef;
    public float bossEnragedSizeCur;
    public float bossEnragedSizeDef;
    public float BossAimBot;
    [HideInInspector]
    public float r;

    [Header("Enemy AI")]
    public float enemyTooCloseDis = 5f;
    public float enemyOverlapSpeed;
    public float enemyOverlapEvadeTimer = 1.25f;

    [Header("Memory")]
    public GameObject talkButton;

    // Start is called before the first frame update
    void Start()
    {
        r = Random.Range(0, 2);
        cam = GameObject.Find("Main Camera");
        gdm = FindObjectOfType<GameDownManager>();
        dm = FindObjectOfType<DialogueManager>();
        aid = FindObjectOfType<AIDestinationSetter>();

        Reset();
    }
    
    // Pause button calls this function
    public void PauseGameButton()
    {
        pausedPressed = true;
        FindObjectOfType<AudioManager>().StopPlaying("Text");
    }

    // Update is called once per frame
    void Update()
    {
        ElementManager();
        CalculateChoiceHigh();
        SetEnemyTarget();
        CameraDepthShake();
        HealthManager();
        ElementUIAesthetic();
        ElementSwitching();
        LoadingText();

        if (isFrozen && !doneOnce)
        {
            doneOnce = true;
            StartCoroutine(DoFreeze());
        }
    }

    void LoadingText()
    {
        if (startPressed)
        {
            startPressed = false;

            // turn on loading text
            loadingText.gameObject.SetActive(true);
        }
    }
    public void GetSwitchTimer()
    {
        switchTime = Random.Range(switchTimeMin, switchTimeMax);
        r = Random.Range(0, 2);
    }

    void ElementSwitching()
    {
        // if there is a nearest enemy. Begin enemy element switching process
        if (nearestEnemy)
        {
            // Detect the nearest enemy's position
            foreach (Enemy e in FindObjectsOfType<Enemy>())
            {
                if (e.isDead)
                {
                    return;
                }
            }

            time += Time.deltaTime;

            // Change enemy's element after time has gone over max time
            if (time >= switchTime)
            {
                ResetEleSwitchTimer();

                // Randomly choose between cycling upwards or downwards for element choice
                if (r == 0)
                {
                    IncElementCount();
                }
                else if (r == 1)
                {
                    DecElementCount();
                }

                GetSwitchTimer();
            }
        }
    }

    void ResetEleSwitchTimer()
    {
        time = 0;
    }

    void IncElementCount()
    {
        e_CurElement++;

        // Play audio
        FindObjectOfType<AudioManager>().Play("BossElementSwitch");
    }

    void DecElementCount()
    {
        e_CurElement--;

        // Play audio
        FindObjectOfType<AudioManager>().Play("BossElementSwitch");
    }

    public void Reset()
    {
        playerSpeedCur = playerSpeedDef;
        evadeTimerCur = evadeTimerDef;

        e_MoveSpeed = e_MoveSpeedDef;
        e_BulletDamage = e_BulletDamageDef;
        e_MaxHealth = e_MaxHealthDef;
        e_EvadeSpeed = e_EvadeSpeedDef;

        bossBulletIncScaleRateCur = bossBulletIncScaleRateDef;
        bossBulletDamageCur = bossBulletDamageDef;
        bossBulletSpeedCur = bossbulletSpeedDef;
        bossBulletDistCur = bossBulletDistDef;
        bossScaleCur = bossScaleDef;
        bossSpeedCur = bossSpeedDef;
        bossEnragedSizeCur = bossEnragedSizeDef;

        GetSwitchTimer();
    }

    public void ChooseElement(int elementNum)
    {
        e_CurElement = elementNum;
    }


    void ElementUIAesthetic()
    {
        // If the player has shot, make the button darker
        if (player)
        {
            if (player.GetComponent<Player>().hasShot)
            {
                shootArea.GetComponent<Image>().color = new Color32(130, 130, 130, 120);
            }

            // If the player HAS NOT shot, make the button regular colour
            else
            {
                shootArea.GetComponent<Image>().color = new Color32(255, 255, 255, 120);
            }

            if (e_IsFire)
            {
                fireButtonVis.GetComponent<Image>().color = new Color32(255, 255, 255, 200);
                waterButtonVis.GetComponent<Image>().color = new Color32(130, 130, 130, 200);
                earthButtonVis.GetComponent<Image>().color = new Color32(130, 130, 130, 200);
            }

            if (e_IsWater)
            {
                waterButtonVis.GetComponent<Image>().color = new Color32(255, 255, 255, 200);
                fireButtonVis.GetComponent<Image>().color = new Color32(130, 130, 130, 200);
                earthButtonVis.GetComponent<Image>().color = new Color32(130, 130, 130, 200);
            }

            if (e_IsEarth)
            {
                earthButtonVis.GetComponent<Image>().color = new Color32(255, 255, 255, 200);
                waterButtonVis.GetComponent<Image>().color = new Color32(130, 130, 130, 200);
                fireButtonVis.GetComponent<Image>().color = new Color32(130, 130, 130, 200);
            }
        }     
    }

    void TurnOnElementUI(GameObject obj)
    {
        foreach (GameObject g in UIElements)
        {

            if (g == obj && !g.activeSelf)
            {
                g.SetActive(true);
            }
            else if (g != obj && g.activeSelf)
            {
                g.SetActive(false);
            }
        }
    }

    void HealthManager()
    {
        if (p_curHealth > p_maxHealth)
        {
            p_curHealth = p_maxHealth;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < p_curHealth)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < p_maxHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
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

                // Play audio
                FindObjectOfType<AudioManager>().Play("ElementSwitch");
            }

            // If player pressed d, switch to right element
            if (Input.GetKeyDown("q"))
            {
                currentElement += 1;

                // Play audio
                FindObjectOfType<AudioManager>().Play("ElementSwitch");
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

                    // Play audio
                    FindObjectOfType<AudioManager>().Play("ElementSwitch");
                }

                // Check if the user touched the water button area
                if (RectTransformUtility.RectangleContainsScreenPoint(fireButtonArea.GetComponent<RectTransform>(), touch.position))
                {
                    currentElement = 1;

                    // Play audio
                    FindObjectOfType<AudioManager>().Play("ElementSwitch");
                }

                // Check if the user touched the water button area
                if (RectTransformUtility.RectangleContainsScreenPoint(earthButtonArea.GetComponent<RectTransform>(), touch.position))
                {
                    currentElement = 2;

                    // Play audio
                    FindObjectOfType<AudioManager>().Play("ElementSwitch");
                }

            }

            // Change Element UI
            TurnOnElementUI(UIElements[currentElement]);
        }

        if (currentElement == 0)
        {
            e_IsEarth = false;
            e_IsFire = false;
            e_IsWater = true;
        }

        if (currentElement == 1)
        {
            e_IsEarth = false;
            e_IsWater = false;
            e_IsFire = true;
        }

        if (currentElement == 2)
        {
            e_IsWater = false;
            e_IsFire = false; 
            e_IsEarth = true;
        }

        if (player)
        {
            playerEleBGAnimC = GameObject.Find("Element BG").GetComponent<Animator>();

            playerEleBGAnimC.SetInteger("curElement", currentElement);
        }   
    }

    public void LaunchGame()
    {
        // Play button click audio
        FindObjectOfType<AudioManager>().Play("ButtonClick");

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        // turn on loading text
        loadingText.gameObject.SetActive(true);
    }

    public void LaunchMenu()
    {
        startPressed = true;

        // Play button click audio
        FindObjectOfType<AudioManager>().Play("ButtonClick");

        gdm.gamePaused = false;
        Time.timeScale = 1;

        // turn pause buttons off
        gdm.pausedBut.gameObject.SetActive(false);
        gdm.continueBut.gameObject.SetActive(false);
        gdm.mainMenuBut.gameObject.SetActive(false);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
