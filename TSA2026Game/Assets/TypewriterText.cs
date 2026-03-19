using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterText : MonoBehaviour
{
    [TextArea] public string inputText;     // Text to type out
    public float startDelay = 0.5f;         // Delay before typing starts
    public float typingSpeed = 0.05f;       // Time between each letter

    private TMP_Text textDisplay;
    private Coroutine typingCoroutine;

    void Start()
    {
        textDisplay = GetComponent<TMP_Text>();
        StartTyping();
    }

    public void StartTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(inputText));
    }

    IEnumerator TypeText(string fullText)
    {
        textDisplay.text = "";
        yield return new WaitForSeconds(startDelay);

        foreach (char c in fullText)
        {
            textDisplay.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}