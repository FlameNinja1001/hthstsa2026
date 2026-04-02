using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoadManager : MonoBehaviour
{
    public string loadString;
    public GameObject sunObj;
    public Transform[]checkPointGoals;

    public Transform[]checkPointCameraGoals;
    public static int lives = 3; 
    public static int checkpointSpawn = 0;   

    public float delayTimer;  
    public float deathDelay;  

    public GameObject inScene;
    public GameObject outScene;  
    public Transform player;
    public Transform boss;
    public Transform camera;    
    bool hasCalled = false;
    public AudioSource audio;
    public AudioClip song;
    public AudioClip die;
    public float introVictDuration;
    public float victoryDuration;
    public bool isTutorial = false;
    // Start is called before the first frame update
    void Start()
    {        
        StartCoroutine(IntroCoroutine());
        player.position = checkPointGoals[checkpointSpawn].position;
        camera.position = checkPointCameraGoals[checkpointSpawn].position;
    }    

    void Update()
    {
        Debug.Log("Lives" + lives);
        FollowTarget followTarget = FindObjectOfType<FollowTarget>();  
        if (player == null && !hasCalled)
        {
            StartCoroutine(Death());

            hasCalled = true;
        } 
        if (boss == null && !hasCalled)
        {
            StartCoroutine(Victory());

            hasCalled = true;
            
        }        
        if (checkpointSpawn >= 2)
        {            
            followTarget.isBoss = true;      
        }
        else if (!isTutorial)
        {
            followTarget.isBoss = false;      
        }
        if (checkpointSpawn == 0)
        {
            sunObj.SetActive(true);
        }
        else
        {
            sunObj.SetActive(false);
        }
    }

    // Update is called once per frame
    public IEnumerator IntroCoroutine()
    {
        inScene.SetActive(false);
        inScene.SetActive(true);
        yield return new WaitForSeconds(delayTimer);
        inScene.SetActive(false);
    }
    public IEnumerator OutroCoroutine(bool willTurnOff)
    {
        outScene.SetActive(false);
        outScene.SetActive(true);
        yield return new WaitForSeconds(delayTimer);
        if (willTurnOff)
        {
            outScene.SetActive(false);
        }
    }

    public IEnumerator CheckpointSpawn()
    {
        StartCoroutine(OutroCoroutine(true));
        yield return new WaitForSeconds(delayTimer);        
        checkpointSpawn++;
        player.position = checkPointGoals[checkpointSpawn].position;
        camera.position = checkPointCameraGoals[checkpointSpawn].position;
        StartCoroutine(IntroCoroutine());

    }

    public IEnumerator Death()
    {
        audio.Stop();
        audio.clip = die;
        audio.loop = false;
        
        audio.Play();
        yield return new WaitForSeconds(deathDelay);
        StartCoroutine(OutroCoroutine(false));
        yield return new WaitForSeconds(delayTimer);
        yield return new WaitForSeconds(deathDelay); 
        lives-=1;
        if (lives > 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }                
        else
        {
            SceneManager.LoadScene("GameOver");
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
        StartCoroutine(OutroCoroutine(false));
        yield return new WaitForSeconds(delayTimer);
        yield return new WaitForSeconds(deathDelay);  
        SceneManager.LoadScene(loadString);             
    }
    
}
