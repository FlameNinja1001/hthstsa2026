using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject text1;
    public GameObject flash1;
    public GameObject obj1; // Option 1
    public GameObject obj2; // Option 2

    [Header("Animation Timers")]
    public float flashDuration = 0.5f;
    public float bringUpDuration = 1f;
    public float textAppearDuration = 1.5f;

    [Header("Positions")]
    public float startY = -200f;
    public float endY = 0f;
    public float restartY = 50f;
    public float titleY = -50f;

    [Header("Restart Animation")]
    public GameObject emptyFrame;
    public Vector2 emptyFramePos1;
    public Vector2 emptyFramePos2;
    public float frameMoveDuration = 1f;

    private RectTransform textRect;
    private RectTransform flashRect;
    private RectTransform frameRect;

    private PlayerInputActions input;
    private Vector2 move;
    private bool canFlashArrow = false;
    private bool canMove = false;
    private bool inputReleased = true;

    private float flashTimer = 0f;
    private float textTimer = 0f;

    public static string loadString;

    void Awake()
    {
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.Player.Enable();
    }

    void OnDisable()
    {
        input.Player.Disable();
    }

    void Start()
    {    

        textRect = text1.GetComponent<RectTransform>();
        flashRect = flash1.GetComponent<RectTransform>();
        frameRect = emptyFrame.GetComponent<RectTransform>();

        // Initialize GAME OVER text position
        Vector2 pos = textRect.anchoredPosition;
        pos.y = startY;
        textRect.anchoredPosition = pos;

        // Initialize flash arrow to first option
        Vector2 arrowPos = flashRect.anchoredPosition;
        arrowPos.y = restartY;
        flashRect.anchoredPosition = arrowPos;

        // Hide menu options initially
        obj1.SetActive(false);
        obj2.SetActive(false);

        SceneLoadManager.lives = 3;
        SceneLoadManager.checkpointSpawn = 0;
        BossText.dialoguePlayed = false;
    }

    void Update()
    {
        float delta = Time.deltaTime;
        flashTimer += delta;
        textTimer += delta;

        // Move GAME OVER text upward
        if (textTimer <= bringUpDuration)
        {
            float t = textTimer / bringUpDuration;
            Vector2 pos = textRect.anchoredPosition;
            pos.y = Mathf.Lerp(startY, endY, t);
            textRect.anchoredPosition = pos;
        }

        // Show options after delay
        if (textTimer >= textAppearDuration)
        {
            obj1.SetActive(true);
            obj2.SetActive(true);
            canFlashArrow = true;
            canMove = true;
        }

        // Flash arrow
        if (canFlashArrow && flashTimer >= flashDuration)
        {
            flash1.SetActive(!flash1.activeSelf);
            flashTimer = 0f;
        }

        // Menu navigation
        if (canMove)
        {
            move = input.Player.Move.ReadValue<Vector2>();

            // Reset inputReleased if stick is near neutral
            if (Mathf.Abs(move.y) < 0.2f)
                inputReleased = true;

            // Move arrow up/down on fresh stick press
            if (inputReleased)
            {
                Vector2 arrowPos = flashRect.anchoredPosition;

                if (move.y > 0.5f)
                {
                    arrowPos.y = restartY;
                    flashRect.anchoredPosition = arrowPos;
                    inputReleased = false;
                }
                else if (move.y < -0.5f)
                {
                    arrowPos.y = titleY;
                    flashRect.anchoredPosition = arrowPos;
                    inputReleased = false;
                }
            }

            // Selection
            float arrowY = flashRect.anchoredPosition.y;

            if (input.Player.Start.WasPressedThisFrame())
            {
                if (Mathf.Abs(arrowY - restartY) < 0.01f)
                {
                    StartCoroutine(LoadSceneWithFrame(loadString));
                }
                else if (Mathf.Abs(arrowY - titleY) < 0.01f)
                {
                    StartCoroutine(LoadSceneWithFrame("TitleScreen"));
                }
            }
        }
    }

    // Generalized coroutine to animate frame then load any scene
    IEnumerator LoadSceneWithFrame(string sceneName)
    {
        float timer = 0f;

        while (timer < frameMoveDuration)
        {
            float t = timer / frameMoveDuration;
            frameRect.anchoredPosition = Vector2.Lerp(emptyFramePos1, emptyFramePos2, t);

            timer += Time.deltaTime;
            yield return null;
        }

        frameRect.anchoredPosition = emptyFramePos2;

        SceneManager.LoadScene(sceneName);
    }
}