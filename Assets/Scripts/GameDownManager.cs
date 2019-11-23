using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDownManager : MonoBehaviour
{
    public Button youFaintedBut;
    public Button restartBut;
    public Button menuBut;
    private Gamemode gm;
    private DialogueManager dm;
    public bool playerDied;
    private Player playerScript;
    private GameObject player;

    private void Update()
    {
        if (playerDied)
        {
            youFaintedBut.gameObject.SetActive(true);
            restartBut.gameObject.SetActive(true);
            menuBut.gameObject.SetActive(true);
        }
        else
        {
            youFaintedBut.gameObject.SetActive(false);
            restartBut.gameObject.SetActive(false);
            menuBut.gameObject.SetActive(false);
        }
    }
    private void Start()
    {
        gm = FindObjectOfType<Gamemode>();
        dm = FindObjectOfType<DialogueManager>();
        gm.talkButton = GameObject.Find("Talk Button");

        playerDied = true;

        GameObject go = Instantiate(gm.playerPrefab, gm.playerSpawnPoint.position, Quaternion.identity);
        gm.player = go;
        player = go;

        playerScript = player.GetComponent<Player>();
        playerScript.p_HealthBar = GameObject.Find("Health").GetComponent<Image>();


        gm.joystickHolder = GameObject.Find("EGOPlayerController");
        gm.joystickMove = FindObjectOfType<FloatingJoystick>();
        gm.shootArea = GameObject.Find("Shoot Area Button");
        gm.waterButtonArea = GameObject.Find("Water Element Button");
        gm.fireButtonArea = GameObject.Find("Fire Element Button");
        gm.earthButtonArea = GameObject.Find("Earth Element Button");
        dm.animatorP = GameObject.Find("Graphics").GetComponent<Animator>();
        dm.choices = GameObject.Find("Choices");
        dm.choices.GetComponent<CanvasGroup>().alpha = 0;

        // Reset player's stats to default values
        gm.bulletSpeed = gm.bulletSpeedDef;
        gm.bulletDist = gm.bulletDistDef;
        gm.bulletDamage = gm.bulletDamageDef;
        gm.shotCooldown = gm.shotCooldownDef;

        gm.p_curHealth = gm.p_maxHealth;
        playerScript.p_HealthBar.fillAmount = 1f;
        gm.isFire = true;


        gm.talkButton = GameObject.Find("Talk Button");
        gm.talkButton.GetComponentInChildren<CanvasGroup>().alpha = 0;
        gm.talkButton.SetActive(true);
    }

    public void RestartButton()
    {
        playerDied = true;

        GameObject go = Instantiate(gm.playerPrefab, gm.playerSpawnPoint.position, Quaternion.identity);
        gm.player = go;

        player = go;
        playerScript = player.GetComponent<Player>();
        playerScript.p_HealthBar = GameObject.Find("Health").GetComponent<Image>();

        gm.joystickHolder = GameObject.Find("EGOPlayerController");
        gm.joystickMove = FindObjectOfType<FloatingJoystick>();
        gm.shootArea = GameObject.Find("Shoot Area Button");
        gm.waterButtonArea = GameObject.Find("Water Element Button");
        gm.fireButtonArea = GameObject.Find("Fire Element Button");
        gm.earthButtonArea = GameObject.Find("Earth Element Button");
        dm.animatorP = GameObject.Find("Graphics").GetComponent<Animator>();
        dm.choices = GameObject.Find("Choices");
        dm.choices.SetActive(false);

        // Reset player's stats to default values
        gm.bulletSpeed = gm.bulletSpeedDef;
        gm.bulletDist = gm.bulletDistDef;
        gm.bulletDamage = gm.bulletDamageDef;
        gm.shotCooldown = gm.shotCooldownDef;

        gm.p_curHealth = gm.p_maxHealth;
        playerScript.p_HealthBar.fillAmount = 1f;
        gm.isFire = true;


        gm.talkButton = GameObject.Find("Talk Button");
        gm.talkButton.GetComponentInChildren<CanvasGroup>().alpha = 0;
        gm.talkButton.SetActive(true);
    }

    public void MenuButton()
    {

    }
}
