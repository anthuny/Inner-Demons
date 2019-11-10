using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Memory : MonoBehaviour
{
    public Dialogue dialogue;
    private bool dialogueTriggered;
    public GameObject player;
    public float viewDis;

    void Update()
    {
        float distance;
        distance = Vector2.Distance(transform.position, player.transform.position);

        if (Input.GetKeyDown("space") && dialogueTriggered == true && distance <= viewDis)
        {
            FindObjectOfType<DialogueManager>().DisplayNextSentence();

        }
        if (Input.GetKeyDown("space") && dialogueTriggered == false && distance <= viewDis)
        {
            TriggerDialogue();
            dialogueTriggered = true;
        }
    }
    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }
}
