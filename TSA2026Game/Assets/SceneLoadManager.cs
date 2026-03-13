using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoadManager : MonoBehaviour
{
    public Transform[]checkPointGoals;

    public Transform[]checkPointCameraGoals;
    public int lives = 3; 
    public int checkpointSpawn = 0;   

    public float delayTimer;  
    public float deathDelay;  

    public GameObject inScene;
    public GameObject outScene;  
    public Transform player;
    public Transform camera;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(IntroCoroutine());
        player.position = checkPointGoals[checkpointSpawn].position;
        camera.position = checkPointCameraGoals[checkpointSpawn].position;
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
        yield return new WaitForSeconds(deathDelay);
        StartCoroutine(OutroCoroutine(false));
        lives-=1;
        if (lives > 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }                
    }
    
}
