using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class BossText : MonoBehaviour
{
    [Header("Dialogue")]
    [TextArea]
    public string[] dialogueLines;

    [Header("References")]
    public RectTransform dialogueBox;
    public TypewriterText typewriter;

    [Header("Positions")]
    public Vector2 inPos;
    public Vector2 outPos;

    [Header("Timing")]
    public float moveDuration = 1f;

    [Header("Post-Dialogue")]
    public GameObject niceTimeKeeper;
    public GameObject evilTimeKeeper;
    public GameObject dummyNed;
    public GameObject realNed;
    public Material fadeMaterial; // URP material
    public float flickerDuration = 5f;
    public float flickerSpeed = 0.2f; // seconds per toggle
    [Range(0f, 1f)] public float targetAlpha = 0.3f;
    public MonoBehaviour[] behaviorsToEnable = new MonoBehaviour[4];

    // Tracks if dialogue already played
    public static bool dialoguePlayed = false;

    PlayerInputActions input;
    int currentLineIndex = 0;
    bool isActive = false;
    bool inputReleased = true;

    void Awake()
    {
        input = new PlayerInputActions();
        dialogueBox.anchoredPosition = outPos;

        // Fade material starts transparent
        if (fadeMaterial != null)
        {
            Color c = fadeMaterial.GetColor("_BaseColor");
            c.a = 0f;
            fadeMaterial.SetColor("_BaseColor", c);
        }

        // Initial object states
        niceTimeKeeper.SetActive(true);
        evilTimeKeeper.SetActive(false);

        dummyNed.SetActive(true);
        realNed.SetActive(false);

        // Disable all MonoBehaviours at start
        foreach (var behavior in behaviorsToEnable)
        {
            if (behavior != null)
                behavior.enabled = false;
        }
    }

    void Start()
    {
        if (!dialoguePlayed)
        {
            StartDialogue(); // Play dialogue once
            dialoguePlayed = true;
        }
        else
        {
            // Skip straight to flicker/fade/enable
            StartCoroutine(PostDialogueEffect());
        }
    }

    void OnEnable() => input.Player.Enable();
    void OnDisable() => input.Player.Disable();

    void Update()
    {
        if (!isActive) return;

        if (!inputReleased && !input.Player.Start.IsPressed())
            inputReleased = true;

        if (inputReleased && input.Player.Start.IsPressed() && !typewriter.isTyping)
        {
            inputReleased = false;
            NextLine();
        }
    }

    [ContextMenu("Start Dialogue")]
    public void StartDialogue()
    {
        StopAllCoroutines();
        StartCoroutine(MoveIn());
    }

    IEnumerator MoveIn()
    {
        float timer = 0f;
        Vector2 startPos = dialogueBox.anchoredPosition;

        while (timer < moveDuration)
        {
            dialogueBox.anchoredPosition = Vector2.Lerp(startPos, inPos, timer / moveDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        dialogueBox.anchoredPosition = inPos;
        currentLineIndex = 0;
        isActive = true;

        ShowLine();
    }

    void ShowLine()
    {
        if (currentLineIndex < dialogueLines.Length)
        {
            typewriter.inputText = dialogueLines[currentLineIndex];
            typewriter.StartTyping();
        }
    }

    void NextLine()
    {
        currentLineIndex++;

        if (currentLineIndex < dialogueLines.Length)
        {
            ShowLine();
        }
        else
        {
            StartCoroutine(MoveOut());
        }
    }

    IEnumerator MoveOut()
    {
        isActive = false;
        float timer = 0f;
        Vector2 startPos = dialogueBox.anchoredPosition;

        while (timer < moveDuration)
        {
            dialogueBox.anchoredPosition = Vector2.Lerp(startPos, outPos, timer / moveDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        dialogueBox.anchoredPosition = outPos;
        typewriter.GetComponent<TMP_Text>().text = "";

        // Start post-dialogue effect
        StartCoroutine(PostDialogueEffect());
    }

    IEnumerator PostDialogueEffect()
    {
        float elapsed = 0f;
        bool toggle = false;

        while (elapsed < flickerDuration)
        {
            toggle = !toggle;
            niceTimeKeeper.SetActive(toggle);
            evilTimeKeeper.SetActive(!toggle);

            // Smooth fade of material alpha
            if (fadeMaterial != null)
            {
                Color c = fadeMaterial.GetColor("_BaseColor");
                c.a = Mathf.Lerp(0f, targetAlpha, elapsed / flickerDuration);
                fadeMaterial.SetColor("_BaseColor", c);
            }

            elapsed += flickerSpeed;
            yield return new WaitForSeconds(flickerSpeed);
        }

        // Final states after flicker
        niceTimeKeeper.SetActive(false);
        evilTimeKeeper.SetActive(true); // permanently active
        if (fadeMaterial != null)
        {
            Color c = fadeMaterial.GetColor("_BaseColor");
            c.a = targetAlpha;
            fadeMaterial.SetColor("_BaseColor", c);
        }

        // Enable MonoBehaviours
        foreach (var behavior in behaviorsToEnable)
        {
            if (behavior != null)
                behavior.enabled = true;
        }

        // Swap Ned objects
        dummyNed.SetActive(false);
        realNed.SetActive(true);
    }
}