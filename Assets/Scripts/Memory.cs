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

    private void Start()
    {
        dm = FindObjectOfType<DialogueManager>();

        // Set each text in the options to equal the memories responses 
        dm.buttonGood.text = memoryResponses[0];

        //dm.buttonNeutral.text = memoryResponses[1] + "\n\nAvoid Responsbility \n\n+ Health";
        //dm.buttonBad.text = memoryResponses[2] + "\n\nDeny Responsbility \n\n+ Enemy Damage";
    }
}
