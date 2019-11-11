using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    public Text dialogueText;

    private Queue<string> sentences;
    private Player playerScript;
    public GameObject choices;
    public bool dialogueTriggered;
    private Gamemode gamemode;
    private bool buttonTextSent;
    public float charTimeGapDef;
    private float charTimeGap;
    public bool timeFlag;
    private bool chooseGood;
    private bool chooseNeutral;
    private bool chooseBad;

    [Header("General")]
    public float alphaIncSpeed;
    [TextArea(3, 10)]
    public string[] choiceText;
    public Text statIncrease;
    public float waitTimeAlphaStart;

    [Header("Other Character's Dialogue Box")]
    public Animator animator;

    [Header("Player's Dialogue Box")]
    public Animator animatorP;
    public Text pDialogueText;

    [Header("Buttons")]
    public Text buttonGood;
    public Text buttonNeutral;
    public Text buttonBad;

    [Header("Responsibility Text")]
    public Text textGoodResp;
    public Text textNeutralResp;
    public Text textBadResp;

    [Header("Statistic Text")]
    public Text textPlusDamage;
    public Text textPlusHealth;
    public Text textPlusEDamage;

    // Start is called before the first frame update
    void Start()
    {
        charTimeGap = charTimeGapDef;

        sentences = new Queue<string>();
        playerScript = GameObject.Find("Player").GetComponent<Player>();

        // Ensure the choices are disabled by default
        choices.SetActive(false);

        gamemode = FindObjectOfType<Gamemode>();
    }

    public void StartDialogue(Dialogue dialogue)
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
        if (Input.GetKeyDown("space") && dialogueTriggered && sentences.Count != 0)
        {
            DisplayNextSentence();
        }

        if (!Input.GetKey("space"))
        {
            timeFlag = false;
        }

        if (Input.GetKey("space"))
        {
            timeFlag = true;
        }

        if (timeFlag)
        {
            charTimeGap = 0.005f;
        }

        if (!timeFlag)
        {
            charTimeGap = charTimeGapDef;
        }

        //If the player shoots, close player's dialogue box
        if (Input.GetMouseButton(0)) 
        {
            animatorP.SetBool("isOpenP", false);
        }
    }
    void DisplayNextSentence()
    {
        string sentence = sentences.Dequeue();

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    // Types each character in a sentence one by one
    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(charTimeGap);
        }
    }

    void EndDialogue()
    {
        // Display the choices 
        choices.SetActive(true);

        Memory memory = GameObject.Find("Memory").GetComponent<Memory>();

        // Set Good text
        if (!buttonTextSent)
        {
            buttonTextSent = true;

            buttonGood.text = memory.memoryResponses[0];
            StartCoroutine(ButtonGoodText(buttonGood.text));
        }
    }

    // Good text character by character
    IEnumerator ButtonGoodText(string sentence)
    {
        buttonGood.text = "";
        buttonNeutral.text = "";
        buttonBad.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            buttonGood.text += letter;
            yield return new WaitForSeconds(charTimeGap);
        }

        // Enable stat increase text to transition in
        textGoodResp.GetComponent<AlphaTransition>().canIncrease = true;
        textPlusDamage.GetComponent<AlphaTransition>().canIncrease = true;

        Memory memory = GameObject.Find("Memory").GetComponent<Memory>();

        // Set Neutral text
        buttonNeutral.text = memory.memoryResponses[1];
        StartCoroutine(ButtonNeutralText(buttonNeutral.text));
    }

    // Neutral text character by character
    IEnumerator ButtonNeutralText(string sentence)
    {
        buttonNeutral.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            buttonNeutral.text += letter;
            yield return new WaitForSeconds(charTimeGap);
        }

        // Enable stat increase text to transition in
        textNeutralResp.GetComponent<AlphaTransition>().canIncrease = true;
        textPlusHealth.GetComponent<AlphaTransition>().canIncrease = true;


        Memory memory = GameObject.Find("Memory").GetComponent<Memory>();

        // Set Bad text
        buttonBad.text = memory.memoryResponses[2];
        StartCoroutine(ButtonBadText(buttonBad.text));
    }

    // Bad text character by character
    IEnumerator ButtonBadText(string sentence)
    {
        buttonBad.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            buttonBad.text += letter;
            yield return new WaitForSeconds(charTimeGap);
        }

        // Enable stat increase text to transition in
        textBadResp.GetComponent<AlphaTransition>().canIncrease = true;
        textPlusEDamage.GetComponent<AlphaTransition>().canIncrease = true;
    }

    // Button select upon on GOOD choice being selected
    public void ChooseGood()
    {
        // Make the buttons disappear
        choices.SetActive(false);

        chooseNeutral = false;
        chooseBad = false;
        chooseGood = true;

        // Increase player's bullet damage
        gamemode.IncreaseDamage();

        // Close Dialogue box
        CloseDialogue();

        // Open player dialogue;
        StartCoroutine(OpenPlayerDialogue());
    }

    // Button select upon on NEUTRAL choice being selected
    public void ChooseNeutral()
    {
        // Make the buttons disappear
        choices.SetActive(false);

        chooseBad = false;
        chooseGood = false;
        chooseNeutral = true;

        // Increase player's health
        gamemode.IncreaseHealth();

        // Close Dialogue box
        CloseDialogue();

        // Open player dialogue;
        StartCoroutine(OpenPlayerDialogue());
    }

    // Button select upon on BAD choice being selected
    public void ChooseBad()
    {
        // Make the buttons disappear
        choices.SetActive(false);

        chooseGood = false;
        chooseNeutral = false;
        chooseBad = true;

        // Increase enemy's bullet damage
        gamemode.IncreaseEnemyDamage();

        // Close Dialogue box
        CloseDialogue();

        // Open player dialogue;
        StartCoroutine(OpenPlayerDialogue());
    }

    IEnumerator OpenPlayerDialogue()
    {
        animatorP.SetBool("isOpenP", true);

        if (chooseGood)
        {
            // If the player chooses the an option, display appropriate text in their dialogue
            pDialogueText.text = choiceText[0];
        }

        if (chooseNeutral)
        {
            // If the player chooses the an option, display appropriate text in their dialogue
            pDialogueText.text = choiceText[1];
        }

        if (chooseBad)
        {
            // If the player chooses the an option, display appropriate text in their dialogue
            pDialogueText.text = choiceText[2];
        }


        yield return new WaitForSeconds(waitTimeAlphaStart);

        // Enable stat increase text to transition in
        statIncrease.GetComponent<AlphaTransition>().canIncrease = true;

        // Increase player's statistics
        gamemode.IncreaseStatistics();

        yield return new WaitForSeconds(8f);

        animatorP.SetBool("isOpenP", false);
    }

    void CloseDialogue()
    {
        // Close the Dialogue box
        animator.SetBool("isOpen", false);

        dialogueTriggered = false;

        // Turn the memory off
        playerScript.memory.gameObject.SetActive(false);
    }
}
