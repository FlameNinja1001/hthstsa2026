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

    // Saved initial scale of the dialogue box
    private Vector3 initialScale;

    PlayerInputActions input;
    int currentLineIndex = 0;
    bool isActive = false;
    bool inputReleased = true;

    void Awake()
    {
        input = new PlayerInputActions();

        // Save designer-set scale, then hide by scaling to zero
        initialScale = dialogueBox.localScale;
        dialogueBox.localScale = Vector3.zero;

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
            StartDialogue();
            dialoguePlayed = true;
        }
        else
        {
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
        StartCoroutine(ScaleIn());
    }

    IEnumerator ScaleIn()
    {
        // Grow from zero to initialScale
        float timer = 0f;
        while (timer < moveDuration)
        {
            float t = timer / moveDuration;
            dialogueBox.localScale = Vector3.Lerp(Vector3.zero, initialScale, t);
            timer += Time.deltaTime;
            yield return null;
        }
        dialogueBox.localScale = initialScale;

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
            ShowLine();
        else
            StartCoroutine(ScaleOut());
    }

    IEnumerator ScaleOut()
    {
        isActive = false;

        // Shrink from initialScale to zero
        float timer = 0f;
        while (timer < moveDuration)
        {
            float t = timer / moveDuration;
            dialogueBox.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
            timer += Time.deltaTime;
            yield return null;
        }
        dialogueBox.localScale = Vector3.zero;

        typewriter.GetComponent<TMP_Text>().text = "";

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
        evilTimeKeeper.SetActive(true);
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