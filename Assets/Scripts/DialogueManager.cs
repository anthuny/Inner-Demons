using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public GameObject nameText;
    public GameObject dialogueText;

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
    private Gamemode gm;

    [Header("General")]
    public float alphaIncSpeed;
    public float alphaDecSpeed;
    [TextArea(3, 10)]
    public string[] choiceText;
    public GameObject statIncrease;
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
    public GameObject buttonBad;
    public GameObject buttonNeutral;
    public GameObject buttonGood;
    private Button talkButton;

    [Header("Responsibility Text")]
    public GameObject textGoodResp;
    public GameObject textNeutralResp;
    public GameObject textBadResp;

    [Header("Statistic Text")]
    public GameObject textPlusDamage;
    public GameObject textPlusHealth;
    public GameObject textPlusEDamage;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();

        charTimeGap = charTimeGapDef;

        sentences = new Queue<string>();

        gm = FindObjectOfType<Gamemode>();

        if (player)
        {
            talkButton = gm.talkButton.GetComponentInChildren<Button>();
            talkButton.interactable = false;
            playerScript = GameObject.Find("Player").GetComponent<Player>();
        }
    }

    private void LateUpdate()
    {
        if (!player)
        {
            player = FindObjectOfType<Player>();
        }
    }
    private void FixedUpdate()
    {
        if (player)
        {
            memory = player.memory;
            EnableTalkButton();

            animatorP = GameObject.Find("Player Dialogue Box").GetComponent<Animator>();
            animator = GameObject.Find("Other Dialogue Box").GetComponent<Animator>();

            buttonGood = GameObject.Find("Button Good");
            buttonNeutral = GameObject.Find("Button Neutral");
            buttonBad = GameObject.Find("Button Bad");

            textGoodResp = GameObject.Find("Responsibility Good");
            textNeutralResp = GameObject.Find("Responsibility Neutral");
            textBadResp = GameObject.Find("Responsibility Neutral");

            textPlusDamage = GameObject.Find("+ Damage");
            textPlusHealth = GameObject.Find("+ Health");
            textPlusEDamage = GameObject.Find("+ Enemy Damage");

            statIncrease = GameObject.Find("Statistic Increase");

            nameText = GameObject.Find("Other Name");
            dialogueText = GameObject.Find("Other Text");
        }

    }

    // Turns the talk button off
    public void DisableTalkButton()
    {
        if (buttonTriggered && !inRange)
        {
            buttonTriggered = false;
            player.memory.GetComponent<Memory>().interacted = false;
            gm.talkButton.GetComponentInChildren<Button>().interactable = false;

            gm.talkButton.GetComponentInChildren<AlphaTransition>().canIncrease = false;
            gm.talkButton.GetComponentInChildren<CanvasGroup>().alpha = 1;
            gm.talkButton.GetComponentInChildren<AlphaTransition>().canDecrease = true;
            talkPressed = false;
        }
    }

    // Turns the talk button on
    public void EnableTalkButton()
    {
        // Check if memory exists from player
        if (player.memory)
        {
            //gm.talkButton.GetComponentInChildren<CanvasGroup>().alpha = 0;

            // Check if the particular memory has been interacted,
            // and if the player is standing still,
            // and if the player has hit the memory's hitbox
            if (player.memory.GetComponent<Memory>().interacted && player.playerStill && !talkPressed && inRange)
            {
                gm.talkButton.GetComponentInChildren<Button>().interactable = true;
                gm.talkButton.GetComponentInChildren<AlphaTransition>().canDecrease = false;
                gm.talkButton.GetComponentInChildren<AlphaTransition>().canIncrease = true;

                // Allows the talk button to disable if player walks out of range
                buttonTriggered = true;
            }
        }
    }

    // Function the talk button does when pressed
    public void TriggerDialogue()
    {
        choices.GetComponent<CanvasGroup>().alpha = 1;
        talkPressed = true;
        // Get reference to the memory's sentences, and send them to the dialogue manager
        StartDialogue(player.memory.GetComponent<Memory>().dialogue);

        gm.talkButton.GetComponentInChildren<Button>().interactable = false;
        gm.talkButton.GetComponentInChildren<AlphaTransition>().canIncrease = false;
        gm.talkButton.GetComponentInChildren<CanvasGroup>().alpha = 1;
        gm.talkButton.GetComponentInChildren<AlphaTransition>().canDecrease = true;

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
        statIncrease.GetComponent<Text>().text = statIncreaseText;

        // Make it so the player cannot trigger this again
        dialogueTriggered = true;

        // Play Dialogue box open animation
        animator.SetBool("isOpen", true);

        // Set the name of the dialogue text
        nameText.GetComponent<Text>().text = dialogue.name;

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

        // If enter is pressed
        if (Input.GetKeyDown(KeyCode.Return) && dialogueTriggered && sentences.Count != 0)
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
        dialogueText.GetComponent<Text>().text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.GetComponent<Text>().text += letter;
            yield return new WaitForSeconds(charTimeGap);
        }
    }

    void EndDialogue()
    {
        // Enable each button to be ready to be visible
        buttonBad.GetComponent<CanvasGroup>().alpha = 1;
        buttonNeutral.GetComponent<CanvasGroup>().alpha = 1;
        buttonGood.GetComponent<CanvasGroup>().alpha = 1;

        memory = player.memory;

        // Set Good text
        if (!buttonTextSent)
        {
            buttonTextSent = true;

            // Assign what the text is going to say, then call it to happen
            buttonBad.GetComponentInChildren<Text>().text = memory.GetComponent<Memory>().memoryResponses[0];
            StartCoroutine(ButtonBadText(buttonBad.GetComponentInChildren<Text>().text));
        }
    }

    // Good text character by character
    IEnumerator ButtonBadText(string sentence)
    {
        buttonBad.GetComponentInChildren<Text>().text = "";
        buttonNeutral.GetComponentInChildren<Text>().text = "";
        buttonGood.GetComponentInChildren<Text>().text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            buttonBad.GetComponentInChildren<Text>().text += letter;
            yield return new WaitForSeconds(charTimeGap);
        }

        // Reset alpha of text to 0
        textBadResp.GetComponent<CanvasGroup>().alpha = 0;
        textPlusDamage.GetComponent<CanvasGroup>().alpha = 0;

        // Enable stat increase text to transition in
        textBadResp.GetComponent<AlphaTransition>().canIncrease = true;
        textPlusDamage.GetComponent<AlphaTransition>().canIncrease = true;

        // Set Neutral text
        buttonNeutral.GetComponentInChildren<Text>().text = memory.GetComponent<Memory>().memoryResponses[1];
        StartCoroutine(ButtonNeutralText(buttonNeutral.GetComponentInChildren<Text>().text));
    }

    // Neutral text character by character
    IEnumerator ButtonNeutralText(string sentence)
    {
        buttonNeutral.GetComponentInChildren<Text>().text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            buttonNeutral.GetComponentInChildren<Text>().text += letter;
            yield return new WaitForSeconds(charTimeGap);
        }

        // Reset alpha of text to 0
        textNeutralResp.GetComponent<CanvasGroup>().alpha = 0;
        textPlusHealth.GetComponent<CanvasGroup>().alpha = 0;

        // Enable stat increase text to transition in
        textNeutralResp.GetComponent<AlphaTransition>().canIncrease = true;
        textPlusHealth.GetComponent<AlphaTransition>().canIncrease = true;

        // Set Bad text
        buttonGood.GetComponentInChildren<Text>().text = memory.GetComponent<Memory>().memoryResponses[2];
        StartCoroutine(ButtonGoodText(buttonBad.GetComponentInChildren<Text>().text));
    }

    // Bad text character by character
    IEnumerator ButtonGoodText(string sentence)
    {
        buttonGood.GetComponentInChildren<Text>().text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            buttonGood.GetComponentInChildren<Text>().text += letter;
            yield return new WaitForSeconds(charTimeGap);
        }

        // Reset alpha of text to 0
        textGoodResp.GetComponent<CanvasGroup>().alpha = 0;
        textPlusEDamage.GetComponent<CanvasGroup>().alpha = 0;

        // Enable stat increase text to transition in
        textGoodResp.GetComponent<AlphaTransition>().canIncrease = true;
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
            statIncrease.GetComponent<Text>().text += "\n+ Damage";
        }

        if (chooseNeutral)
        {
            // If the player chooses the an option, display appropriate text in their dialogue
            pDialogueText.text = choiceText[1];

            // Add specific stat to statIncrease based on choice
            statIncrease.GetComponent<Text>().text += "\n+ Health";
        }

        if (chooseGood)
        {
            // If the player chooses the an option, display appropriate text in their dialogue
            pDialogueText.text = choiceText[2];

            // Add specific stat to statIncrease based on choice
            statIncrease.GetComponent<Text>().text += "\n+ Enemy Power";
        }

        yield return new WaitForSeconds(waitTimeAlphaStart);

        // Enable stat increase text to transition in
        statIncrease.GetComponent<AlphaTransition>().canIncrease = true;

        // Increase player's statistics
        gamemode.IncreaseStatistics();

        yield return new WaitForSeconds(8f);

        // Reset stat increase text to default
        statIncrease.GetComponent<Text>().text = "+ Projectile Speed\n+ Reload Speed";

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

