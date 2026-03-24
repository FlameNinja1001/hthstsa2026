using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class FinalBossTextScript : MonoBehaviour
{
    public MonoBehaviour playerScript1;
    public MonoBehaviour playerScript2;
    public MonoBehaviour playerScript3;
    [Header("Dialogue")]
    [TextArea] public string[] dialogue;

    [Header("References")]
    public RectTransform dialogueBox;
    public TypewriterText typewriter;

    [Header("Positions")]
    public Vector2 inPos;
    public Vector2 outPos;

    [Header("Movement")]
    public float moveSpeed = 8f;

    PlayerInputActions input;

    int currentLineIndex = 0;

    bool isActive = false;
    bool inputReleased = true;
    bool movingIn = false;
    public bool hasDisabled;

    void Awake()
    {
        input = new PlayerInputActions();
    }

    void OnEnable() => input.Player.Enable();
    void OnDisable() => input.Player.Disable();

    void Start()
    {
        // Start off-screen and activate
        dialogueBox.gameObject.SetActive(true);
        dialogueBox.anchoredPosition = outPos;

        movingIn = true;

        currentLineIndex = 0;
        isActive = true;

        StartCurrentLine();        
    }

    void Update()
    {
        if (!hasDisabled)
        {
            StartCoroutine(PlayerCont(false));
            hasDisabled = true;
        }        
        // 🔹 Handle movement (no coroutine)
        if (movingIn)
        {
            dialogueBox.anchoredPosition = Vector2.Lerp(
                dialogueBox.anchoredPosition,
                inPos,
                moveSpeed * Time.deltaTime
            );

            if (Vector2.Distance(dialogueBox.anchoredPosition, inPos) < 0.1f)
            {
                dialogueBox.anchoredPosition = inPos;
                movingIn = false;
            }
        }

        if (!isActive) return;

        // 🔹 Input handling
        if (!inputReleased && !input.Player.Start.IsPressed())
            inputReleased = true;

        if (inputReleased && input.Player.Start.IsPressed())
        {
            inputReleased = false;
            NextLine();
        }
    }

    void StartCurrentLine()
    {
        if (currentLineIndex < dialogue.Length)
        {
            typewriter.inputText = dialogue[currentLineIndex];
            typewriter.StartTyping();
        }
    }

    void NextLine()
    {
        currentLineIndex++;

        if (currentLineIndex < dialogue.Length)
        {
            StartCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        isActive = false;

        // Clear text
        typewriter.GetComponent<TMP_Text>().text = "";

        // Hide box (optional)
        dialogueBox.gameObject.SetActive(false);

        // Optional: trigger boss fight here
        // FindObjectOfType<YourBossScript>()?.StartFight();
    }

    public IEnumerator PlayerCont(bool status)
    {
        yield return new WaitForSeconds(0.01f);
        playerScript1.enabled = status;
        playerScript2.enabled = status;
        playerScript3.enabled = status;
    }
}