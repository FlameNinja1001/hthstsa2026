using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameOverScript : MonoBehaviour
{
    public GameObject text1;
    public GameObject flash1;

    public float flashDuration;
    public float bringUpDuration;
    public float textAppearDuration;

    public float startY;
    public float endY;

    float timer;
    float timer2;

    public bool canFlashArrow;

    public GameObject obj1;
    public GameObject obj2;

    RectTransform textRect;
    RectTransform flashRect;

    PlayerInputActions input;
    public bool canMove;

    Vector2 move;

    public float restartY;
    public float titleY;

    bool inputReleased = true;

    [Header("Restart Animation")]
    public GameObject emptyFrame;
    public Vector2 emptyFramePos1;
    public Vector2 emptyFramePos2;
    public float frameMoveDuration = 1f;

    RectTransform frameRect;

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

        Vector2 pos = textRect.anchoredPosition;
        pos.y = startY;
        textRect.anchoredPosition = pos;
        SceneLoadManager.lives = 3;
        SceneLoadManager.checkpointSpawn = 0;   
    }

    void Update()
    {
        timer += Time.deltaTime;
        timer2 += Time.deltaTime;

        // Move GAME OVER text upward
        if (timer2 <= bringUpDuration)
        {
            float t = timer2 / bringUpDuration;

            Vector2 pos = textRect.anchoredPosition;
            pos.y = Mathf.Lerp(startY, endY, t);
            textRect.anchoredPosition = pos;
        }

        // Flash arrow
        if (timer >= flashDuration)
        {
            if (canFlashArrow)
                flash1.SetActive(!flash1.activeSelf);

            timer = 0f;
        }

        // Show options
        if (timer2 >= textAppearDuration)
        {
            obj1.SetActive(true);
            obj2.SetActive(true);

            canFlashArrow = true;
            canMove = true;
        }

        // Menu navigation
        if (canMove)
        {
            move = input.Player.Move.ReadValue<Vector2>();

            if (Mathf.Abs(move.y) < 0.2f)
                inputReleased = true;

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

            Vector2 arrowCheck = flashRect.anchoredPosition;

            if (Mathf.Abs(arrowCheck.y - restartY) < 0.01f)
            {
                if (input.Player.Start.IsPressed())
                {
                    StartCoroutine(RestartLevel());
                }
            }
        }
    }

    IEnumerator RestartLevel()
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
        SceneManager.LoadScene(loadString);
    }
}