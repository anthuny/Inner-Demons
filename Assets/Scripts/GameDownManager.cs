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
    private DialogueManager dg;
    public bool playerDied;

    private void Start()
    {
        gm = FindObjectOfType<Gamemode>();
        dg = FindObjectOfType<DialogueManager>();

    }
    public void RestartButton()
    {
        GameObject go = Instantiate(gm.playerPrefab, gm.playerSpawnPoint.position, Quaternion.identity);
        gm.player = go;
        gm.joystickHolder = GameObject.Find("EGOPlayerController");
        gm.joystickMove = FindObjectOfType<FloatingJoystick>();
        gm.shootArea = GameObject.Find("Shoot Area Button");
        gm.waterButtonArea = GameObject.Find("Water Element Button");
        gm.fireButtonArea = GameObject.Find("Fire Element Button");
        gm.earthButtonArea = GameObject.Find("Earth Element Button");
        dg.animatorP = GameObject.Find("Graphics").GetComponent<Animator>();
        dg.choices = GameObject.Find("Choices");
        dg.choices.SetActive(false);

        youFaintedBut.gameObject.SetActive(false);
        restartBut.gameObject.SetActive(false);
        menuBut.gameObject.SetActive(false);

        playerDied = true;
    }

    public void MenuButton()
    {

    }
}
