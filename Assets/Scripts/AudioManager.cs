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
        gm = FindObjectOfType<Gamemode>();

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

    private void Awake()
    {
        if (currentScene != "Main")
        {
            lastScene = "Menu";
        }
        else
        {
            lastScene = "Main";
        }

    }
    private void Update()
    {
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

        if (gm.inMemoryRoom && !enteredMemRoom)
        {
            enteredMemRoom = true;
            StopPlaying("MainMusic");
            Play("TalkingMusic");
        }

        if (gm.inBossRoom && !enteredBossRoom)
        {
            enteredBossRoom = true;
            StopPlaying("MainMusic");
            Play("BattleMusic");
        }
    }
}
