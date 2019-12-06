using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDownManager : MonoBehaviour
{
    [Header("Player Died Buttons")]
    public Button youFaintedBut;
    public Button restartBut;
    public Button menuBut;
    private Gamemode gm;
    private DialogueManager dm;
    public bool playerDied;
    private Player playerScript;
    public GameObject player;
    public GameObject[] memories;
    public bool gamePaused;
    private bool pausePressed;

    [Header("Player Menu Buttons")]
    public Button pausedBut;
    public Button continueBut;
    public Button mainMenuBut;

    [Header("Player Win")]
    public Button winBut;
    public Button restartBut2;
    public Button menuBut2;

    [Header("General")]
    public string currentScene;

    private void Start()
    {
        playerDied = false;

        // Have the pause menu buttons invisible
        pausedBut.gameObject.SetActive(false);
        continueBut.gameObject.SetActive(false);
        mainMenuBut.gameObject.SetActive(false);

        // Have the death menu buttons invisible
        youFaintedBut.gameObject.SetActive(false);
        restartBut.gameObject.SetActive(false);
        menuBut.gameObject.SetActive(false);

        // Have the win menu buttons invisible
        winBut.gameObject.SetActive(false);
        menuBut2.gameObject.SetActive(false);

        gm = FindObjectOfType<Gamemode>();
        dm = FindObjectOfType<DialogueManager>();
        //gm.talkButton = GameObject.Find("Talk Button");

        GameObject go = Instantiate(gm.playerPrefab, gm.playerSpawnPoint.position, Quaternion.identity);
        gm.player = go;
        player = go;;

        // Reset player's stats to default values
        gm.bulletSpeed = gm.bulletSpeedDef;
        gm.bulletDist = gm.bulletDistDef;
        gm.bulletDamage = gm.bulletDamageDef;
        gm.shotCooldown = gm.shotCooldownDef;

        gm.p_curHealth = gm.p_maxHealth;
        gm.e_IsFire = true;


        gm.talkButton = GameObject.Find("Talk Button");
        gm.talkButton.GetComponentInChildren<CanvasGroup>().alpha = 0;
        gm.talkButton.SetActive(true);
    }

    private void Update()
    {
        PauseGame();

        if (playerDied)
        {
            //Reset();

            youFaintedBut.gameObject.SetActive(true);
            restartBut.gameObject.SetActive(true);
            menuBut.gameObject.SetActive(true);

            gm.arrogance = 0;
            gm.ignorance = 0;
            gm.morality = 0;

            // Enable all memories

            foreach (GameObject memory in memories)
            {
                memory.SetActive(true);
            }
        }

        else
        {
            player = FindObjectOfType<Player>().gameObject;
        }

    }

    public void RestartButton()
    {
        pausePressed = false;

        // Play button click audio
        PlayButtonClickSFX();

        playerDied = false;

        youFaintedBut.gameObject.SetActive(false);
        restartBut.gameObject.SetActive(false);
        menuBut.gameObject.SetActive(false);

        if (!FindObjectOfType<Player>())
        {
            GameObject go = Instantiate(gm.playerPrefab, gm.playerSpawnPoint.position, Quaternion.identity);
            gm.player = go;
            player = go;
        }

        playerScript = player.GetComponent<Player>();

        gm.joystickHolder = GameObject.Find("EGOPlayerController");
        gm.joystickMove = FindObjectOfType<FloatingJoystick>();
        gm.shootArea = GameObject.Find("Shoot Area Button");
        gm.waterButtonArea = GameObject.Find("Water Element Button");
        gm.fireButtonArea = GameObject.Find("Fire Element Button");
        gm.earthButtonArea = GameObject.Find("Earth Element Button");
        dm.animatorP = GameObject.Find("Graphics").GetComponent<Animator>();

        // Reset player's stats to default values
        gm.bulletSpeed = gm.bulletSpeedDef;
        gm.bulletDist = gm.bulletDistDef;
        gm.bulletDamage = gm.bulletDamageDef;
        gm.shotCooldown = gm.shotCooldownDef;

        gm.p_curHealth = gm.p_maxHealth;
        gm.e_IsFire = true;

        gm.talkButton = GameObject.Find("Talk Button");
        gm.talkButton.GetComponentInChildren<CanvasGroup>().alpha = 0;
        gm.talkButton.SetActive(true);
    }

    public void PauseGame()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || gm.pausedPressed && !pausePressed)
        {
            pausePressed = true;
            // Play button click audio
            PlayButtonClickSFX();
            gamePaused = true;
        }

        if (gamePaused)
        {
            pausedBut.gameObject.SetActive(true);
            continueBut.gameObject.SetActive(true);
            mainMenuBut.gameObject.SetActive(true);
            Time.timeScale = 0;
        }

        else if (!gm.isFrozen)
        {
            pausedBut.gameObject.SetActive(false);
            continueBut.gameObject.SetActive(false);
            mainMenuBut.gameObject.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void ContinueButton()
    {
        pausePressed = false;

        // Play button click audio
        PlayButtonClickSFX();

        gm.pausedPressed = false;
        gamePaused = false;
    }

    public void PlayButtonClickSFX()
    {
        FindObjectOfType<AudioManager>().Play("ButtonClick");
    }
}