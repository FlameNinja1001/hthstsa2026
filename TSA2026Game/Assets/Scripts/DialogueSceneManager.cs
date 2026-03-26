using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DialogueSceneManager : MonoBehaviour
{
    [Header("Dialogue Arrays (Fixed 6)")]
    [TextArea] public string[] dialogueSet1;
    [TextArea] public string[] dialogueSet2;
    [TextArea] public string[] dialogueSet3;
    [TextArea] public string[] dialogueSet4;
    [TextArea] public string[] dialogueSet5;
    [TextArea] public string[] dialogueSet6;

    [Header("Scene Names (1 per set)")]
    public string[] loadStrings; // Should have 6 elements

    [Header("UI Objects")]
    public RectTransform moveObject1;
    public RectTransform moveObject2;
    public Vector2 startPos1;
    public Vector2 startPos2;
    public Vector2 moveUpPos1;
    public Vector2 moveUpPos2;
    public Vector2 exitPos1;
    public Vector2 exitPos2;

    [Header("Timing")]
    public float moveDuration = 1f;

    [Header("References")]
    public TypewriterText typewriter;

    private PlayerInputActions input;
    public static int currentDialogueIndex = 0;
    private int currentLineIndex = 0;
    private bool isActive = false;
    private bool inputReleased = true;

    private string[][] allDialogues;

    void Awake()
    {
        input = new PlayerInputActions();

        // Combine the 6 arrays into a 2D array for easy indexing
        allDialogues = new string[6][]
        {
            dialogueSet1,
            dialogueSet2,
            dialogueSet3,
            dialogueSet4,
            dialogueSet5,
            dialogueSet6
        };

        moveObject1.anchoredPosition = startPos1;
        moveObject2.anchoredPosition = startPos2;
    }

    void OnEnable() => input.Player.Enable();
    void OnDisable() => input.Player.Disable();

    void Start()
    {
        StartCoroutine(StartCutscene());
        SceneLoadManager.lives = 3;
        SceneLoadManager.checkpointSpawn = 0;
    }

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

    IEnumerator StartCutscene()
    {
        // Move both objects up at once
        yield return StartCoroutine(MoveBothObjects(startPos1, startPos2, moveUpPos1, moveUpPos2, moveDuration));

        currentLineIndex = 0;
        StartCurrentLine();
    }

    void StartCurrentLine()
    {
        string[] currentArray = allDialogues[currentDialogueIndex];
        if (currentLineIndex < currentArray.Length)
        {
            typewriter.inputText = currentArray[currentLineIndex];
            typewriter.StartTyping();
            isActive = true;
        }
    }

    void NextLine()
    {
        currentLineIndex++;
        string[] currentArray = allDialogues[currentDialogueIndex];

        if (currentLineIndex < currentArray.Length)
        {
            StartCurrentLine();
        }
        else
        {
            isActive = false;
            StartCoroutine(EndCutscene());
        }
    }

    IEnumerator EndCutscene()
    {
        // Move both objects offscreen at once
        yield return StartCoroutine(MoveBothObjects(moveUpPos1, moveUpPos2, exitPos1, exitPos2, moveDuration));

        // Load next scene
        if (currentDialogueIndex < loadStrings.Length)
        {
            string nextScene = loadStrings[currentDialogueIndex];
            currentDialogueIndex++;
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.LogWarning("No scene defined for this dialogue index!");
        }
    }

    IEnumerator MoveBothObjects(Vector2 from1, Vector2 from2, Vector2 to1, Vector2 to2, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            float t = timer / duration;
            moveObject1.anchoredPosition = Vector2.Lerp(from1, to1, t);
            moveObject2.anchoredPosition = Vector2.Lerp(from2, to2, t);
            timer += Time.deltaTime;
            yield return null;
        }
        moveObject1.anchoredPosition = to1;
        moveObject2.anchoredPosition = to2;
    }
}