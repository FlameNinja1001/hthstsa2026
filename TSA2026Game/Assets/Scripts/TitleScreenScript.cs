using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TitleScreenScript : MonoBehaviour
{
    [Header("Fade Panel")]
    public Image fadePanel;            // UI Panel Image
    public float fadeDuration = 1f;

    [Header("Moving Object")]
    public RectTransform movingObject;
    public Vector2 startPos;
    public Vector2 endPos;
    public float moveDuration = 1f;

    [Header("Flashing Object")]
    public GameObject flashObject;
    public float flashInterval = 0.5f;

    [Header("Input & Scene")]
    public PlayerInputActions input;
    public string loadSceneName;

    private bool waitingForInput = false;
    private bool inputReleased = true;
    private Coroutine flashCoroutine;

    void Start()
    {
        DialogueSceneManager.currentDialogueIndex = 0;
        SceneLoadManager.lives = 3;
        SceneLoadManager.checkpointSpawn = 0;
    }
    void Awake()
    {
        input = new PlayerInputActions();
        input.Player.Enable();

        // Start panel fully opaque
        SetFadeAlpha(1f);

        // Set moving object at start
        if (movingObject != null)
            movingObject.anchoredPosition = startPos;

        // Hide flashing object initially
        if (flashObject != null)
            flashObject.SetActive(false);

        // Start the title sequence
        StartCoroutine(TitleSequence());
    }

    void Update()
    {
        if (!waitingForInput) return;

        if (!inputReleased && !input.Player.Start.IsPressed())
            inputReleased = true;

        if (inputReleased && input.Player.Start.IsPressed())
        {
            inputReleased = false;
            waitingForInput = false;

            // Stop flashing
            if (flashCoroutine != null)
                StopCoroutine(flashCoroutine);
            if (flashObject != null)
                flashObject.SetActive(false);

            // Fade panel back in and then load scene
            StartCoroutine(FadeInAndLoadScene());
        }
    }

    IEnumerator TitleSequence()
    {
        // Wait 1 second before starting fade out
        yield return new WaitForSeconds(1f);

        // 1️⃣ Fade out panel
        yield return StartCoroutine(Fade(1f, 0f, fadeDuration));

        // 2️⃣ Move object
        if (movingObject != null)
        {
            float timer = 0f;
            Vector2 initialPos = movingObject.anchoredPosition;

            while (timer < moveDuration)
            {
                float t = timer / moveDuration;
                movingObject.anchoredPosition = Vector2.Lerp(initialPos, endPos, t);
                timer += Time.deltaTime;
                yield return null;
            }

            movingObject.anchoredPosition = endPos;
        }

        // 3️⃣ Start flashing object
        if (flashObject != null)
            flashCoroutine = StartCoroutine(FlashObject());

        // 4️⃣ Wait for input
        waitingForInput = true;
    }

    IEnumerator FlashObject()
    {
        while (true)
        {
            flashObject.SetActive(!flashObject.activeSelf);
            yield return new WaitForSeconds(flashInterval);
        }
    }

    IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            float t = timer / duration;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            SetFadeAlpha(alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        SetFadeAlpha(endAlpha);
    }

    IEnumerator FadeInAndLoadScene()
    {
        // Fade panel from 0 -> 1
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        // Load the scene
        if (!string.IsNullOrEmpty(loadSceneName))
            SceneManager.LoadScene(loadSceneName);
    }

    void SetFadeAlpha(float alpha)
    {
        if (fadePanel == null) return;
        Color c = fadePanel.color;
        c.a = alpha;
        fadePanel.color = c;
    }
}