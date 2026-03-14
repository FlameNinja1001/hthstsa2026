using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomScript : MonoBehaviour
{
    public static bool canPlayerMove = true;

    public Behaviour target;
    public Transform player;

    public float moveUpUnits = 5f;
    public float moveRightUnits = 3f;

    public float moveSpeed = 2f;
    public bool hasWent = false;
    public AudioSource myAudioSource;
    public AudioClip newSong;

    void Start()
    {
        canPlayerMove = true;
        target.enabled = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerTag") && !hasWent)
        {
            StartCoroutine(MoveBossRoom());
            hasWent = true;
        }
    }

    public IEnumerator MoveBossRoom()
    {
        
        canPlayerMove = false;

        Vector3 startPos = transform.position;
        Vector3 upPos = startPos + Vector3.up * moveUpUnits;

        float t = 0;

        // Move room up
        while (t < 1)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPos, upPos, t);
            yield return null;
        }
        
        

        FollowTarget followTarget = FindObjectOfType<FollowTarget>();
        followTarget.SwitchBossPos();
        // Move player right
        Vector3 playerStart = player.position;
        Vector3 playerEnd = playerStart + Vector3.right * moveRightUnits;

        t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * moveSpeed;
            player.position = Vector3.Lerp(playerStart, playerEnd, t);
            yield return null;
        }

        // Move room back down
        t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(upPos, startPos, t);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        myAudioSource.clip = newSong;
        // Play the new clip
        myAudioSource.Play();
        target.enabled = true;
        canPlayerMove = true;
    }
}