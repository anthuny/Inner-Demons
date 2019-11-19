using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private GameObject player;
    private Transform spawnPos;

    [Header("Choices")]
    public int arrogance;
    public int ignorance;
    public int morality;
    public bool arroganceHighest;
    public bool ignoranceHighest;
    public bool moralityHighest;
    public int maxChoice;

    [Header("Element Handler")]
    public GameObject currentEPos;
    public GameObject leftEPos;
    public GameObject rightEPos;
    public GameObject fireElement;
    public GameObject waterElement;
    public GameObject earthElement;
    public float lesserElementScale;
    public float currentElementScale;

    [Header("Touch Inputs / Joysticks")]
    public GameObject joystickHolder;
    public Joystick joystickMove;
    public Joystick joystickShoot;
    public float minDragDistance;
    private Vector2 startPos;
    private Vector2 direction;
    private bool swipeAccepted;
    private float xDis;
    private float yDis;
    private float lowestNumX;
    private float highestNumX;
    private float lowestNumY;
    private float highestNumY;
    public GameObject swipeArea;
    private Touch touch;

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

    public float e_HealthDeath = 0;
    public float e_ViewDis;
    public float evadetimerMax;
    public float bossDeathSpeed = 0;

    [Header("Enemy Ranged Attacking Statistics")]
    public float e_BulletDamage;
    public float e_ShotCooldown;
    public float e_BulletSpeed;
    public float e_BulletDist;

    // Start is called before the first frame update
    void Start()
    {
        spawnPos = GameObject.Find("EGOSpawnPoint").transform;

        playerSpeedCur = playerSpeedDef;
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

        ElementManager();
        CalculateChoiceHigh();
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
        if (maxChoice == arrogance)
        {
            arroganceHighest = true;
            ignoranceHighest = false;
            moralityHighest = false;
        }

        // If maxchoice is igorance and NOT balanced
        if (maxChoice == ignorance)
        {
            ignoranceHighest = true;
            arroganceHighest = false;
            moralityHighest = false;
        }

        // If maxchoice is morality and NOT balanced
        if (maxChoice == morality)
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

        // If player is Fire element, change visually
        if (currentElement == 0)
        {
            isEarth = false;
            isWater = false;
            isFire = true;

            // Move current element to current element position, and set it as a child
            fireElement.transform.position = currentEPos.transform.position;
            fireElement.transform.SetParent(currentEPos.transform);
            fireElement.transform.localScale = new Vector2(currentElementScale, currentElementScale);

            // Adjust the other elements accordingly
            earthElement.transform.position = rightEPos.transform.position;
            earthElement.transform.SetParent(rightEPos.transform);
            earthElement.transform.localScale = new Vector2(lesserElementScale, lesserElementScale);

            waterElement.transform.position = leftEPos.transform.position;
            waterElement.transform.SetParent(leftEPos.transform);
            waterElement.transform.localScale = new Vector2(lesserElementScale, lesserElementScale);
        }

        // If player is water element, change visually
        if (currentElement == 1)
        {
            isFire = false;
            isEarth = false;
            isWater = true;

            // Move current element to current element position, and set it as a child
            waterElement.transform.position = currentEPos.transform.position;
            waterElement.transform.SetParent(currentEPos.transform);
            waterElement.transform.localScale = new Vector2(currentElementScale, currentElementScale);

            // Adjust the other elements accordingly
            fireElement.transform.position = rightEPos.transform.position;
            fireElement.transform.SetParent(rightEPos.transform);
            fireElement.transform.localScale = new Vector2(lesserElementScale, lesserElementScale);

            earthElement.transform.position = leftEPos.transform.position;
            earthElement.transform.SetParent(leftEPos.transform);
            earthElement.transform.localScale = new Vector2(lesserElementScale, lesserElementScale);
        }

        // If player is earth element, change visually
        if (currentElement == 2)
        {
            isFire = false;
            isWater = false;
            isEarth = true;

            // Move current element to current element position, and set it as a child
            earthElement.transform.position = currentEPos.transform.position;
            earthElement.transform.SetParent(currentEPos.transform);
            earthElement.transform.localScale = new Vector2(currentElementScale, currentElementScale);

            // Adjust the other elements accordingly
            waterElement.transform.position = rightEPos.transform.position;
            waterElement.transform.SetParent(rightEPos.transform);
            waterElement.transform.localScale = new Vector2(lesserElementScale, lesserElementScale);

            fireElement.transform.position = leftEPos.transform.position;
            fireElement.transform.SetParent(leftEPos.transform);
            fireElement.transform.localScale = new Vector2(lesserElementScale, lesserElementScale);
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

            // Check if the user touched the swipe area
            if (RectTransformUtility.RectangleContainsScreenPoint(swipeArea.GetComponent<RectTransform>(), touch.position))
            {
                // Handle finger movements based on touch phsae
                switch (touch.phase)
                {
                    // record initial touch position
                    case TouchPhase.Began:
                        startPos = touch.position;
                        break;

                    // Determine direction by comparing the current touch position with the initial one.
                    case TouchPhase.Moved:
                        direction = touch.position - startPos;

                        // Determine the largest number of the x and y values and set it to a value
                        if (touch.position.x > startPos.x)
                        {
                            highestNumX = touch.position.x;
                            lowestNumX = startPos.x;
                        }
                        else
                        {
                            highestNumX = startPos.x;
                            lowestNumX = touch.position.x;
                        }

                        xDis = highestNumX - lowestNumX;

                        if (touch.position.y > startPos.y)
                        {
                            highestNumY = touch.position.y;
                            lowestNumY = startPos.y;
                        }
                        else
                        {
                            highestNumY = startPos.y;
                            lowestNumY = touch.position.y;
                        }

                        yDis = highestNumY - lowestNumY;

                        // Determine if the drag was long enough
                        if (xDis >= minDragDistance)
                        {
                            //alreadyattemped = true;

                            // Determine if the drag was horizontal or not
                            if (xDis > yDis * 2)
                            {
                                swipeAccepted = true;
                            }
                        }
                        else
                        {
                            swipeAccepted = false;
                        }
                        break;

                    // Report that a direction has been chosen when the finger is lifted.
                    case TouchPhase.Ended:
                        if (swipeAccepted)
                        {
                            if (startPos.x > touch.position.x)
                            {
                                currentElement -= 1;
                            }
                            else
                            {
                                currentElement += 1;
                            }

                        }
                        else
                        {
                        }
                        break;
                }
            }
        }
    }
}
