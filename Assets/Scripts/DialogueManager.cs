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
    private bool chooseBad;
    private bool chooseNeutral;
    private bool chooseGood;

    [Header("General")]
    public float alphaIncSpeed;
    public float alphaDecSpeed;
    [TextArea(3, 10)]
    public string[] choiceText;
    public Text statIncrease;
    public string statIncreaseText;
    public float waitTimeAlphaStart;
    private GameObject memory;
    private Player player;
    public bool talkPressed;
    public bool buttonTriggered;
    public bool inRange;

    [Header("Other Character's Dialogue Box")]
    public Animator animator;

    [Header("Player's Dialogue Box")]
    public Animator animatorP;
    public Text pDialogueText;

    [Header("Buttons")]
    public Text buttonGood;
    public Text buttonNeutral;
    public Text buttonBad;
    public GameObject buttonTalk;

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
        buttonTalk.GetComponentInChildren<Button>().interactable = false;

        player = FindObjectOfType<Player>();
        charTimeGap = charTimeGapDef;

        sentences = new Queue<string>();
        playerScript = GameObject.Find("Player").GetComponent<Player>();

        // Ensure the choices are disabled by default
        choices.SetActive(false);

        gamemode = FindObjectOfType<Gamemode>();
    }

    private void FixedUpdate()
    {
        EnableTalkButton();
        memory = player.memory;
    }

    // Turns the talk button off
    public void DisableTalkButton()
    {
        if (buttonTriggered && !inRange)
        {
            buttonTriggered = false;
            player.memory.GetComponent<Memory>().interacted = false;
            buttonTalk.GetComponentInChildren<Button>().interactable = false;

            buttonTalk.GetComponentInChildren<AlphaTransition>().canIncrease = false;
            buttonTalk.GetComponentInChildren<CanvasGroup>().alpha = 1;
            buttonTalk.GetComponentInChildren<AlphaTransition>().canDecrease = true;
            talkPressed = false;
        }
    }

    // Turns the talk button on
    public void EnableTalkButton()
    {
        // Check if memory exists from player
        if (player.memory)
        {
            //buttonTalk.GetComponentInChildren<CanvasGroup>().alpha = 0;

            // Check if the particular memory has been interacted,
            // and if the player is standing still,
            // and if the player has hit the memory's hitbox
            if (player.memory.GetComponent<Memory>().interacted && player.playerStill && !talkPressed && inRange)
            {
                buttonTalk.GetComponentInChildren<Button>().interactable = true;
                buttonTalk.GetComponentInChildren<AlphaTransition>().canDecrease = false;
                buttonTalk.GetComponentInChildren<AlphaTransition>().canIncrease = true;

                // Allows the talk button to disable if player walks out of range
                buttonTriggered = true;
            }
        }
    }

    // Function the talk button does when pressed
    public void TriggerDialogue()
    {
        talkPressed = true;
        // Get reference to the memory's sentences, and send them to the dialogue manager
        StartDialogue(player.memory.GetComponent<Memory>().dialogue);

        buttonTalk.GetComponentInChildren<Button>().interactable = false;
        buttonTalk.GetComponentInChildren<AlphaTransition>().canIncrease = false;
        buttonTalk.GetComponentInChildren<CanvasGroup>().alpha = 1;
        buttonTalk.GetComponentInChildren<AlphaTransition>().canDecrease = true;

    }

    public void StartDialogue(Dialogue dialogue)
    {
        // Reset alpha of text to 0
        textGoodResp.GetComponent<CanvasGroup>().alpha = 0;
        textPlusDamage.GetComponent<CanvasGroup>().alpha = 0;
        textNeutralResp.GetComponent<CanvasGroup>().alpha = 0;
        textPlusHealth.GetComponent<CanvasGroup>().alpha = 0;
        textBadResp.GetComponent<CanvasGroup>().alpha = 0;
        textPlusEDamage.GetComponent<CanvasGroup>().alpha = 0;

        textGoodResp.GetComponent<AlphaTransition>().canIncrease = false;
        textPlusDamage.GetComponent<AlphaTransition>().canIncrease = false;
        textNeutralResp.GetComponent<AlphaTransition>().canIncrease = false;
        textPlusHealth.GetComponent<AlphaTransition>().canIncrease = false;
        textBadResp.GetComponent<AlphaTransition>().canIncrease = false;
        textPlusEDamage.GetComponent<AlphaTransition>().canIncrease = false;

        statIncreaseText = "+ Projectile Speed\n+ Reload Speed";

        // Reset stat increase text to default
        statIncrease.text = statIncreaseText;

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
        // If player touches
        if (Input.touchCount > 0 && dialogueTriggered && sentences.Count != 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                DisplayNextSentence();
            }
        }

        // If space is pressed
        if (Input.GetKeyDown("space") && dialogueTriggered && sentences.Count != 0)
        {
            DisplayNextSentence();
        }

        if (Input.touchCount != 0)
        {
            timeFlag = false;
        }

        if (Input.touchCount > 0)
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

        memory = player.memory;

        // Set Good text
        if (!buttonTextSent)
        {
            buttonTextSent = true;

            buttonGood.text = memory.GetComponent<Memory>().memoryResponses[0];
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

        // Reset alpha of text to 0
        textGoodResp.GetComponent<CanvasGroup>().alpha = 0;
        textPlusDamage.GetComponent<CanvasGroup>().alpha = 0;

        // Enable stat increase text to transition in
        textGoodResp.GetComponent<AlphaTransition>().canIncrease = true;
        textPlusDamage.GetComponent<AlphaTransition>().canIncrease = true;

        // Set Neutral text
        buttonNeutral.text = memory.GetComponent<Memory>().memoryResponses[1];
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

        // Reset alpha of text to 0
        textNeutralResp.GetComponent<CanvasGroup>().alpha = 0;
        textPlusHealth.GetComponent<CanvasGroup>().alpha = 0;

        // Enable stat increase text to transition in
        textNeutralResp.GetComponent<AlphaTransition>().canIncrease = true;
        textPlusHealth.GetComponent<AlphaTransition>().canIncrease = true;

        // Set Bad text
        buttonBad.text = memory.GetComponent<Memory>().memoryResponses[2];
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

        // Reset alpha of text to 0
        textBadResp.GetComponent<CanvasGroup>().alpha = 0;
        textPlusEDamage.GetComponent<CanvasGroup>().alpha = 0;

        // Enable stat increase text to transition in
        textBadResp.GetComponent<AlphaTransition>().canIncrease = true;
        textPlusEDamage.GetComponent<AlphaTransition>().canIncrease = true;
    }

    // Button select upon on GOOD choice being selected
    public void ChooseBad()
    {
        // DISABLE stat increase text from transition in
        textGoodResp.GetComponent<AlphaTransition>().canIncrease = false;
        textPlusDamage.GetComponent<AlphaTransition>().canIncrease = false;

        // Make the buttons disappear
        choices.SetActive(false);

        chooseNeutral = false;
        chooseGood = false;
        chooseBad = true;

        // Increase player's bullet damage
        gamemode.IncreaseDamage();

        // Increase the player's arrogance
        gamemode.arrogance++;

        // Close Dialogue box
        StartCoroutine("CloseDialogue");

        // Open player dialogue;
        StartCoroutine(OpenPlayerDialogue());
    }

    // Button select upon on NEUTRAL choice being selected
    public void ChooseNeutral()
    {
        // DISABLE stat increase text from transition in
        textNeutralResp.GetComponent<AlphaTransition>().canIncrease = false;
        textPlusHealth.GetComponent<AlphaTransition>().canIncrease = false;

        // Make the buttons disappear
        choices.SetActive(false);

        chooseGood = false;
        chooseBad = false;
        chooseNeutral = true;

        // Increase player's health
        gamemode.IncreaseHealth();

        // Increase the player's ignorance
        gamemode.ignorance++;

        // Close Dialogue box
        StartCoroutine("CloseDialogue");

        // Open player dialogue;
        StartCoroutine(OpenPlayerDialogue());
    }

    // Button select upon on BAD choice being selected
    public void ChooseGood()
    {
        // DISABLE stat increase text from transition in
        textBadResp.GetComponent<AlphaTransition>().canIncrease = false;
        textPlusEDamage.GetComponent<AlphaTransition>().canIncrease = false;

        // Make the buttons disappear
        choices.SetActive(false);

        chooseBad = false;
        chooseNeutral = false;
        chooseGood = true;

        // Increase enemy's bullet damage
        gamemode.IncreaseEnemyDamage();

        // Increase the player's morality
        gamemode.morality++;

        // Close Dialogue box
        StartCoroutine("CloseDialogue");

        // Open player dialogue;
        StartCoroutine(OpenPlayerDialogue());
    }

    IEnumerator OpenPlayerDialogue()
    {
        animatorP.SetBool("isOpenP", true);

        if (chooseBad)
        {
            // If the player chooses the an option, display appropriate text in their dialogue
            pDialogueText.text = choiceText[0];

            // Add specific stat to statIncrease based on choice
            statIncrease.text += "\n+ Damage";
        }

        if (chooseNeutral)
        {
            // If the player chooses the an option, display appropriate text in their dialogue
            pDialogueText.text = choiceText[1];

            // Add specific stat to statIncrease based on choice
            statIncrease.text += "\n+ Health";
        }

        if (chooseGood)
        {
            // If the player chooses the an option, display appropriate text in their dialogue
            pDialogueText.text = choiceText[2];

            // Add specific stat to statIncrease based on choice
            statIncrease.text += "\n+ Enemy Power";
        }

        yield return new WaitForSeconds(waitTimeAlphaStart);

        // Enable stat increase text to transition in
        statIncrease.GetComponent<AlphaTransition>().canIncrease = true;

        // Increase player's statistics
        gamemode.IncreaseStatistics();

        yield return new WaitForSeconds(8f);

        // Reset stat increase text to default
        statIncrease.text = "+ Projectile Speed\n+ Reload Speed";

        // Remove player dialogue box from screen
        animatorP.SetBool("isOpenP", false);
    }

    IEnumerator CloseDialogue()
    {
        // Reset alpha of text to 0
        textGoodResp.GetComponent<CanvasGroup>().alpha = 0;
        textPlusDamage.GetComponent<CanvasGroup>().alpha = 0;
        textNeutralResp.GetComponent<CanvasGroup>().alpha = 0;
        textPlusHealth.GetComponent<CanvasGroup>().alpha = 0;
        textBadResp.GetComponent<CanvasGroup>().alpha = 0;
        textPlusEDamage.GetComponent<CanvasGroup>().alpha = 0;

        textGoodResp.GetComponent<AlphaTransition>().canIncrease = false;
        textPlusDamage.GetComponent<AlphaTransition>().canIncrease = false;
        textNeutralResp.GetComponent<AlphaTransition>().canIncrease = false;
        textPlusHealth.GetComponent<AlphaTransition>().canIncrease = false;
        textBadResp.GetComponent<AlphaTransition>().canIncrease = false;
        textPlusEDamage.GetComponent<AlphaTransition>().canIncrease = false;
        

        // Close the Dialogue box
        animator.SetBool("isOpen", false);

        gamemode.playerSpeedCur = gamemode.playerSpeedDead;
        yield return new WaitForSeconds(1f);
        gamemode.playerSpeedCur = gamemode.playerSpeedDef;

        dialogueTriggered = false;
        buttonTextSent = false;

        // Turn the memory off
        memory.gameObject.SetActive(false);

        memory.GetComponent<Memory>().interacted = false;
    }
}

