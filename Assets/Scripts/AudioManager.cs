using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;
    private string lastScene;
    private string currentScene;
    private Gamemode gm;
    [HideInInspector]
    public bool enteredMemRoom;
    [HideInInspector]
    public bool enteredBossRoom;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        if (currentScene != "Main")
        {
            Play("MenuMusic");
            lastScene = "Menu";
        }
        else
        {
            lastScene = "Main";
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Play();
    }
    public void StopPlaying(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Stop();
    }

    private void Update()
    {
        gm = FindObjectOfType<Gamemode>();

        if (gm.inMemoryRoom && !enteredMemRoom)
        {
            enteredMemRoom = true;
            //Debug.Log("playing memory music");
            StopPlaying("MainMusic");
            Play("TalkingMusic");
        }

        if (gm.inBossRoom && !enteredBossRoom)
        {
            enteredBossRoom = true;
            //Debug.Log("playing boss music");
            StopPlaying("MainMusic");
            Play("BattleMusic");
        }

        // If player is not in boss room, the if statement above can be fine
        if (!gm.inBossRoom)
        {
            enteredBossRoom = false;
        }

        // If player is not in boss room, the if statement above can be fine
        if (!gm.inMemoryRoom)
        {
            enteredMemRoom = false;
        }

        currentScene = SceneManager.GetActiveScene().name;
           
        if (currentScene != lastScene)
        {
            if (currentScene == "Main")
            {
                StopPlaying("TalkingMusic");
                StopPlaying("BattleMusic");
                StopPlaying("MenuMusic");
                Play("MainMusic");
            }

            else if (currentScene == "Menu")
            {
                StopPlaying("TalkingMusic");
                StopPlaying("BattleMusic");
                StopPlaying("MainMusic");
                Play("MenuMusic");
            }

            lastScene = currentScene;
        }
    }
}
