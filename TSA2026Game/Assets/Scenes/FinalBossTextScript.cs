using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class FinalBossTextScript : MonoBehaviour
{
    public static bool hasWent;
    public Material fadeMaterial;
    public MonoBehaviour playerScript1;
    public MonoBehaviour playerScript2;
    public MonoBehaviour playerScript3;
    public GameObject gameManager;

    [Header("Dialogue")]
    [TextArea] public string[] dialogue;

    [Header("References")]
    public RectTransform dialogueBox;
    public TypewriterText typewriter;

    [Header("Positions")]
    public Vector2 inPos;
    public Vector2 outPos;

    [Header("Movement")]
    public float moveSpeed = 800f; // UI usually needs higher speed

    [Header("Fade Settings")]
    [Range(0f, 1f)] public float startAlpha = 0f;
    [Range(0f, 1f)] public float endAlpha = 1f;
    public float fadeDuration = 1f;

    PlayerInputActions input;

    int currentLineIndex = 0;

    bool isActive = false;
    bool inputReleased = true;

    bool movingIn = false;
    bool movingOut = false;

    public bool hasDisabled;

    public GameObject timeKeeperModelNice;
    public GameObject timeKeeperModelBad;
    public Animator animator;
    public float flickerSpeed;

    void Awake()
    {
        input = new PlayerInputActions();
    }

    void OnEnable() => input.Player.Enable();
    void OnDisable() => input.Player.Disable();

    void Start()
    {
        dialogueBox.gameObject.SetActive(true);
        dialogueBox.anchoredPosition = outPos;

        animator.enabled = false;

        if (!hasWent)
        {
            // Normal dialogue sequence
            movingIn = true;
            movingOut = false;

            currentLineIndex = 0;
            isActive = true;

            StartCurrentLine();
            SetFadeAlpha(0f);

            hasWent = true; // Mark as already shown
        }
        else
        {
            // Skip dialogue, jump straight to fade + flicker
            isActive = false;
            movingOut = true;

            // Start the fade coroutine immediately
            StartCoroutine(Fade(startAlpha, endAlpha, fadeDuration));

            // Start the time keeper flicker sequence
            StartCoroutine(RevealTimeKeeper());
        }
    }

    void Update()
    {
        // Disable player once
        if (!hasDisabled)
        {
            StartCoroutine(PlayerCont(false));
            hasDisabled = true;
        }

        // 🔹 MOVE IN
        if (movingIn)
        {
            dialogueBox.anchoredPosition = Vector2.MoveTowards(
                dialogueBox.anchoredPosition,
                inPos,
                moveSpeed * Time.deltaTime
            );

            if (Vector2.Distance(dialogueBox.anchoredPosition, inPos) < 0.01f)
            {
                dialogueBox.anchoredPosition = inPos;
                movingIn = false;
            }
        }

        // 🔹 MOVE OUT
        if (movingOut)
        {
            dialogueBox.anchoredPosition = Vector2.MoveTowards(
                dialogueBox.anchoredPosition,
                outPos,
                moveSpeed * Time.deltaTime
            );

            if (Vector2.Distance(dialogueBox.anchoredPosition, outPos) < 0.01f)
            {
                dialogueBox.anchoredPosition = outPos;
                movingOut = false;

                // Only disable AFTER sliding out
                dialogueBox.gameObject.SetActive(false);
            }
        }

        if (!isActive) return;

        // 🔹 Input handling
        if (!inputReleased && !input.Player.Start.IsPressed())
            inputReleased = true;

        if (inputReleased && input.Player.Start.IsPressed() && !typewriter.isTyping)
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

        // 🔹 Start moving OUT instead of instantly hiding
        movingOut = true;

        // 🔹 Fade
        StartCoroutine(Fade(startAlpha, endAlpha, fadeDuration));

        // 🔹 Continue sequence
        StartCoroutine(RevealTimeKeeper());
    }

    public IEnumerator PlayerCont(bool status)
    {
        yield return new WaitForSeconds(0.01f);

        playerScript1.enabled = status;
        playerScript2.enabled = status;
        playerScript3.enabled = status;
    }

    IEnumerator Fade(float start, float end, float duration)
    {
        float timer = 0f;

        SetFadeAlpha(start);

        while (timer < duration)
        {
            float t = timer / duration;
            float a = Mathf.Lerp(start, end, t);

            SetFadeAlpha(a);

            timer += Time.deltaTime;
            yield return null;
        }

        SetFadeAlpha(end);
    }

    void SetFadeAlpha(float alpha)
    {
        if (fadeMaterial == null) return;

        Color c = fadeMaterial.GetColor("_BaseColor");
        c.a = alpha;

        fadeMaterial.SetColor("_BaseColor", c);
    }

    public IEnumerator RevealTimeKeeper()
    {
        for (int i = 0; i < 6; i++)
        {
            timeKeeperModelNice.SetActive(false);
            timeKeeperModelBad.SetActive(true);
            yield return new WaitForSeconds(flickerSpeed);

            timeKeeperModelNice.SetActive(true);
            timeKeeperModelBad.SetActive(false);
            yield return new WaitForSeconds(flickerSpeed);
        }

        timeKeeperModelNice.SetActive(false);
        timeKeeperModelBad.SetActive(true);

        yield return new WaitForSeconds(1f);

        StartCoroutine(PlayerCont(true));
        gameManager.SetActive(true);
        animator.enabled = true;
    }
}