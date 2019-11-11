using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    public Text dialogueText;

    public Animator animator;

    private Queue<string> sentences;
    private Player playerScript;
    public GameObject choices;
    public bool dialogueTriggered;
    private Gamemode gamemode;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
        playerScript = GameObject.Find("Player").GetComponent<Player>();

        // Ensure the choices are disabled by default
        choices.SetActive(false);

        gamemode = FindObjectOfType<Gamemode>();
    }

    public void StartDialogue (Dialogue dialogue)
    {
        // Make it so the player cannot trigger this again
        dialogueTriggered = true;

        // Play Dialogue box open animation
        animator.SetBool("isOpen", true);

        // Set the name of the dialogue text
        nameText.text = dialogue.name;

        // Clear all previous sentences
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    private void Update()
    {
        if (Input.GetKeyDown("space") && dialogueTriggered)
        {
            DisplayNextSentence();
        }
    }
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    void EndDialogue()
    {
        // Display the choices 
        choices.SetActive(true);
    }

    // Button select upon on GOOD choice being selected
    public void ChooseGood()
    {
        // Make the buttons disappear
        choices.SetActive(false);

        // Increase player's bullet damage
        gamemode.bulletDamage += gamemode.chooseDamageIncrease;

        // Close Dialogue box
        CloseDialogue();
    }

    // Button select upon on NEUTRAL choice being selected
    public void ChooseNeutral()
    {
        // Make the buttons disappear
        choices.SetActive(false);

        // Increase player's health
        gamemode.p_maxHealth += gamemode.chooseHealthIncrease;

        // Close Dialogue box
        CloseDialogue();
    }

    // Button select upon on BAD choice being selected
    public void ChooseBad()
    {
        // Make the buttons disappear
        choices.SetActive(false);

        // Increase enemy's bullet damage
        gamemode.e_BulletDamage += gamemode.chooseE_DamageIncrease;

        // Close Dialogue box
        CloseDialogue();
    }

    void CloseDialogue()
    {
        // Close the Dialogue box
        animator.SetBool("isOpen", false);

        dialogueTriggered = false;
    }
}
