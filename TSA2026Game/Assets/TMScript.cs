using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class TMScript : MonoBehaviour
{
    [Header("Fade")]
    public Material fadeMaterial;
    public float fadeDuration = 1f;
    public Color fadeColor = Color.white;
    public bool canPlayerMove = true;
    public Camera cam;
    [Header("Dialogue Arrays")]
    [TextArea] public string[] dialogueSet1;
    [TextArea] public string[] dialogueSet2;
    [TextArea] public string[] dialogueSet3;

    private List<string[]> allDialogues = new List<string[]>();

    [Header("References")]
    public RectTransform movingObject;
    public TypewriterText typewriter;

    [Header("Positions")]
    public Vector2 inPos;
    public Vector2 outPos;

    [Header("Timing")]
    public float moveDuration = 1f;

    PlayerInputActions input;

    int currentSetIndex = 0;
    int currentLineIndex = 0;

    bool isActive = false;
    bool inputReleased = true;
    public Transform camera;
    public Transform player;
    
    public GameObject animPlayer;
    public GameObject stillPlayer;
    public GameObject sling;
    public GameObject bat;
    public GameObject pixelPlayer;
    public GameObject dumPlayer;
    public AudioSource music;
    public float delay0;
    public float delay1;
    public float delay2;
    public float delay3;

    void Awake()
    {
        input = new PlayerInputActions();

        allDialogues.Add(dialogueSet1);
        allDialogues.Add(dialogueSet2);
        allDialogues.Add(dialogueSet3);

        // start hidden
        movingObject.anchoredPosition = outPos;
        if (fadeMaterial != null)
        {
            Color c = fadeColor;
            c.a = 0f;
            fadeMaterial.SetColor("_BaseColor", c);
        }
    }

    void OnEnable() => input.Player.Enable();
    void OnDisable() => input.Player.Disable();

    void Update()
    {
        if (!isActive) return;

        if (!inputReleased && !input.Player.Start.IsPressed())
            inputReleased = true;

        if (inputReleased && input.Player.Start.IsPressed())
        {
            inputReleased = false;
            NextLine();
        }
    }

    // 🔹 THIS shows up in inspector button
    [ContextMenu("Start Dialogue")]
    public void StartDialogueSequence()
    {
        if (currentSetIndex >= allDialogues.Count)
            return;

        StopAllCoroutines();
        StartCoroutine(MoveIn());
    }

    IEnumerator MoveIn()
    {
        if (currentSetIndex < 2)
        {
            camera.position = new Vector3(player.position.x,player.position.y + 1, camera.position.z);
            cam.orthographicSize = 6;

            animPlayer.SetActive(false);
            stillPlayer.SetActive(true);
            if (currentSetIndex == 0)
            {
                sling.SetActive(true);
            }
            else if (currentSetIndex == 1)
            {
                bat.SetActive(true);
            }
        }        
        canPlayerMove = false;  
        isActive = false;

        float timer = 0f;
        Vector2 startPos = movingObject.anchoredPosition;

        while (timer < moveDuration)
        {
            float t = timer / moveDuration;
            movingObject.anchoredPosition = Vector2.Lerp(startPos, inPos, t);

            timer += Time.deltaTime;
            yield return null;
        }

        movingObject.anchoredPosition = inPos;

        currentLineIndex = 0;
        isActive = true;

        StartCurrentLine();              
    }

    void StartCurrentLine()
    {
        string[] currentSet = allDialogues[currentSetIndex];

        if (currentLineIndex < currentSet.Length)
        {
            typewriter.inputText = currentSet[currentLineIndex];
            typewriter.StartTyping();
        }
    }

    void NextLine()
    {
        currentLineIndex++;

        string[] currentSet = allDialogues[currentSetIndex];

        if (currentLineIndex < currentSet.Length)
        {
            StartCurrentLine();
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
        Vector2 startPos = movingObject.anchoredPosition;

        while (timer < moveDuration)
        {
            float t = timer / moveDuration;
            movingObject.anchoredPosition = Vector2.Lerp(startPos, outPos, t);

            timer += Time.deltaTime;
            yield return null;
        }

        movingObject.anchoredPosition = outPos;

        // clear text
        typewriter.GetComponent<TMP_Text>().text = "";        
        cam.orthographicSize = 9;
        if (currentSetIndex < 2)
        {
            canPlayerMove = true;

            animPlayer.SetActive(true);
            stillPlayer.SetActive(false);
            bat.SetActive(false);
            sling.SetActive(false);            
        }            
        else
        {
            StartCoroutine(LevelTransition());
        }
        currentSetIndex++;
    }
    IEnumerator LevelTransition()
    {
        dumPlayer.transform.localPosition = new Vector3(dumPlayer.transform.localPosition.x, dumPlayer.transform.localPosition.y, 15f);
        animPlayer.transform.localPosition = new Vector3(animPlayer.transform.localPosition.x, animPlayer.transform.localPosition.y, 15f);
        pixelPlayer.transform.localPosition = new Vector3(pixelPlayer.transform.localPosition.x, pixelPlayer.transform.localPosition.y, 15f);
        music.Stop();

        StartCoroutine(Fade(0f, 1f, fadeDuration));        

        yield return new WaitForSeconds(delay0);
        animPlayer.SetActive(false);
        dumPlayer.SetActive(false);
        pixelPlayer.SetActive(true);        
        yield return new WaitForSeconds(delay3);        
        dumPlayer.SetActive(true);
        pixelPlayer.SetActive(false); 
        yield return new WaitForSeconds(delay0);
        dumPlayer.SetActive(false);
        pixelPlayer.SetActive(true);        
        yield return new WaitForSeconds(delay3);        
        dumPlayer.SetActive(true);
        pixelPlayer.SetActive(false); 
        yield return new WaitForSeconds(delay1);
        dumPlayer.SetActive(false);
        pixelPlayer.SetActive(true);        
        yield return new WaitForSeconds(delay3);        
        dumPlayer.SetActive(true);
        pixelPlayer.SetActive(false);  
        yield return new WaitForSeconds(delay1);
        dumPlayer.SetActive(false);
        pixelPlayer.SetActive(true);        
        yield return new WaitForSeconds(delay3);        
        dumPlayer.SetActive(true);
        pixelPlayer.SetActive(false); 
        yield return new WaitForSeconds(delay2);
        dumPlayer.SetActive(false);
        pixelPlayer.SetActive(true);        
        yield return new WaitForSeconds(delay3);        
        dumPlayer.SetActive(true);
        pixelPlayer.SetActive(false); 
        yield return new WaitForSeconds(delay2);
        dumPlayer.SetActive(false);
        pixelPlayer.SetActive(true);        
        yield return new WaitForSeconds(delay3);        
        dumPlayer.SetActive(true);
        pixelPlayer.SetActive(false); 
        yield return new WaitForSeconds(delay3);
        dumPlayer.SetActive(false);
        pixelPlayer.SetActive(true);        
        yield return new WaitForSeconds(delay3);        
        dumPlayer.SetActive(true);
        pixelPlayer.SetActive(false); 
        yield return new WaitForSeconds(delay3);
        dumPlayer.SetActive(false);
        pixelPlayer.SetActive(true);    
        yield return new WaitForSeconds(delay3);        
        dumPlayer.SetActive(true);
        pixelPlayer.SetActive(false); 
        yield return new WaitForSeconds(delay3);
        dumPlayer.SetActive(false);
        pixelPlayer.SetActive(true);        
        yield return new WaitForSeconds(delay3);        
        dumPlayer.SetActive(true);
        pixelPlayer.SetActive(false); 
        yield return new WaitForSeconds(delay3);
        dumPlayer.SetActive(false);
        pixelPlayer.SetActive(true);  
        yield return new WaitForSeconds(delay3);        
        dumPlayer.SetActive(true);
        pixelPlayer.SetActive(false); 
        yield return new WaitForSeconds(delay3);
        dumPlayer.SetActive(false);
        pixelPlayer.SetActive(true);        
        yield return new WaitForSeconds(delay3);        
        dumPlayer.SetActive(true);
        pixelPlayer.SetActive(false); 
        yield return new WaitForSeconds(delay3);
        dumPlayer.SetActive(false);
        pixelPlayer.SetActive(true);      

        yield return new WaitForSeconds(delay0 * 2);
        SceneLoadManager sceneLoadManager = FindObjectOfType<SceneLoadManager>();
        StartCoroutine(sceneLoadManager.OutroCoroutine(false));
        yield return new WaitForSeconds(delay0 * 2);
    }
    IEnumerator Fade(float start, float end, float duration)
    {
        float timer = 0f;

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

        Color c = fadeColor;
        c.a = alpha;

        fadeMaterial.SetColor("_BaseColor", c);
    }
}