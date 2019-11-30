using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memory : MonoBehaviour
{
    public Dialogue dialogue;

    [TextArea(3, 10)]
    public string[] memoryResponses;
    
    public bool interacted;

    private DialogueManager dm;

    private GameObject enemy;
    private GameObject bossDialogueBad;
    private GameObject bossDialogueNeutral;
    private GameObject bossDialogueGood;
    private GameObject bossDialogueBalanced;
    private GameObject dialogueToSelect;

    private Gamemode gm;

    private void Start()
    {
        // Play idle animation
        GetComponent<Animator>().SetInteger("memBrain", 0);
    }
    private void OnEnable()
    {
        gm = FindObjectOfType<Gamemode>();

        // Referencing each boss dialogue texts
        bossDialogueBad = GameObject.Find("EGOBossDialogueBad");
        bossDialogueNeutral = GameObject.Find("EGOBossDialogueNeutral");
        bossDialogueGood = GameObject.Find("EGOBossDialogueGood");
        bossDialogueBalanced = GameObject.Find("EGOBossDialogueBalanced");

        // If the memory script is on the boss
        if (transform.tag == "Enemy")
        {
            enemy = transform.gameObject;

            // If the player's arrogrance is the highest, set the gameobject to select to be the arrogant dialogue one
            if (gm.arroganceHighest)
            {
                dialogueToSelect = bossDialogueBad;
            }

            // If the player's ignorance is the highest, set the gameobject to select to be the ignorance dialogue one
            if (gm.ignoranceHighest)
            {
                dialogueToSelect = bossDialogueNeutral;
            }

            // If the player's morality is the highest, set the gameobject to select to be the morality dialogue one
            if (gm.moralityHighest)
            {
                dialogueToSelect = bossDialogueGood;
            }

            if (!gm.moralityHighest && !gm.ignoranceHighest && !gm.arroganceHighest)
            {
                dialogueToSelect = bossDialogueBalanced;
            }

            // Assign the correct dialogue to the enemy to say
            // Dialogue for enemy array length must match the chosen dialogues array length
            for (int i = 0; i < dialogue.sentences.Length; i++)
            {
                dialogue.sentences[i] = dialogueToSelect.GetComponent<BossDialogueChoices>().sentences[i];
            }
        }
    }

    private void Update()
    {

    }
}
