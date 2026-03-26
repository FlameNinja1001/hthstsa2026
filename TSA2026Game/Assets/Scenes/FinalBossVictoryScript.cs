using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalBossVictoryScript : MonoBehaviour
{
    public string loadString;
    public Transform player;
    public AudioSource audio;
    public AudioClip song;
    public float introVictDuration;
    public float victoryDuration;
    public float delayTimer;
    public float deathDelay;

    public GameObject enemy;
    public bool hasWent;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy == null)
        {
            if (!hasWent)
            {
                StartCoroutine(Victory());
                hasWent = true;
            }
        }        
    }

    public IEnumerator Victory()
    {
        BossRoomScript.canPlayerMove = false;
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();  
        playerHealth.canBeDamaged = false;

        audio.Stop();
        audio.loop = false;

        yield return new WaitForSeconds(introVictDuration);

        audio.clip = song;
        audio.Play();

        yield return new WaitForSeconds(victoryDuration);

        audio.Stop();

        yield return new WaitForSeconds(deathDelay);

        SceneLoadManager sceneLoadManager = FindObjectOfType<SceneLoadManager>(); 
        StartCoroutine(sceneLoadManager.OutroCoroutine(false));

        yield return new WaitForSeconds(delayTimer);
        yield return new WaitForSeconds(deathDelay);  

        SceneManager.LoadScene(loadString);             
    }
}