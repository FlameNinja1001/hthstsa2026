using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GilgameshBossScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    public GameObject mesh;
    public Vector3 initialPos;
    public Vector3 leftPos;
    public Vector3 rightPos;
    public float moveDist;
    public float distance;
    public Vector3 target;

    public float speed;    
    public bool isJumping;
    public float jumpDistance;
    public bool isOppositeSide = false;
    public float jumpDuration;
    public float jumpHeight;
    public GameObject sandShotPrefab;
    public GameObject sandLionPrefab;
    
    public Transform shotSpawnUpLeft;
    public Transform shotSpawnDownLeft;
    public Transform lionSpawnLeft;
    public Transform shotSpawnUpRight;
    public Transform shotSpawnDownRight;
    public Transform lionSpawnRight;

    public float delayForShots1;  
    public float delayForShots2;  
    public float delayForShots3;  
    private float timer;  
    public bool hasShot1 = false;
    public bool hasShot2 = false;
    float originalScale;
    void Start()
    {         
        ResetPos();    
        animator = mesh.GetComponent<Animator>();
        originalScale = transform.localScale.x; 
        
    }

    // Update is called once per frame
    void Update()
    {        
        if (!isOppositeSide)
        {
            transform.localScale = new Vector3(-originalScale, transform.localScale.y, transform.localScale.z);            
        }
        else if (isOppositeSide)
        {
            transform.localScale = new Vector3(originalScale, transform.localScale.y, transform.localScale.z);            
        }
        animator.SetBool("IsJumping", isJumping);
        if (isJumping)
        {
            ResetTimer();
        }
        timer += Time.deltaTime;
        if (timer >= delayForShots1 && !hasShot1)
        {
            StartCoroutine(ShootAnim());
            if (isOppositeSide)
            {
                GameObject shot = Instantiate(sandShotPrefab, shotSpawnUpRight.position, sandShotPrefab.transform.rotation);            
            }
            else
            {
                GameObject shot = Instantiate(sandShotPrefab, shotSpawnUpLeft.position, sandShotPrefab.transform.rotation);    
                SandLionScript sandLionScript = shot.GetComponent<SandLionScript>();
                sandLionScript.isLeft = true;        
            }
            hasShot1 = true;
        }
        if (timer >= delayForShots2 && !hasShot2)
        {
            StartCoroutine(ShootAnim());
            if (isOppositeSide)
            {
                GameObject shot = Instantiate(sandLionPrefab, lionSpawnRight.position, sandLionPrefab.transform.rotation);            
            }
            else
            {
                GameObject shot = Instantiate(sandLionPrefab, lionSpawnLeft.position, sandLionPrefab.transform.rotation);    
                SandLionScript sandLionScript = shot.GetComponent<SandLionScript>();
                sandLionScript.isLeft = true;        
            }
            hasShot2 = true;
        }
        if (timer >= delayForShots3)
        {
            StartCoroutine(ShootAnim());
            if (isOppositeSide)
            {
                GameObject shot = Instantiate(sandShotPrefab, shotSpawnDownRight.position, sandShotPrefab.transform.rotation);            
            }
            else
            {
                GameObject shot = Instantiate(sandShotPrefab, shotSpawnDownLeft.position, sandShotPrefab.transform.rotation);    
                SandLionScript sandLionScript = shot.GetComponent<SandLionScript>();
                sandLionScript.isLeft = true;        
            }
            ResetTimer();
        }
        if (!isJumping)
        {
            if (Vector3.Distance(transform.position,target) < distance)
            {
                if (target == leftPos)
                {
                    target = rightPos;
                }
                else
                {
                    target = leftPos;
                }
            }
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }        
    }
    public void Jump()
    {
        if (!isJumping)
        {
            StartCoroutine(JumpCoroutine());
        }        
    }
    public void ResetPos()
    {
        initialPos = transform.position;
        leftPos = new Vector3(transform.position.x - moveDist, transform.position.y,transform.position.z);
        rightPos = new Vector3(transform.position.x + moveDist, transform.position.y,transform.position.z);
        target = leftPos;
    }

    public IEnumerator JumpCoroutine()
    {        
        animator.SetBool("IsShooting",false);
        isJumping = true;

        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(transform.position.x + (isOppositeSide ? jumpDistance : -jumpDistance),transform.position.y,transform.position.z);

        float time = 0f;

        // Center point used for Slerp arc
        Vector3 center = (startPos + endPos) / 2f;
        center.y -= jumpHeight; // Push center down to make arc upward

        // Offset positions relative to center
        Vector3 startRel = startPos - center;
        Vector3 endRel = endPos - center;

        while (time < jumpDuration)
        {
            float t = time / jumpDuration;

            // Slerp relative positions
            Vector3 relPos = Vector3.Slerp(startRel, endRel, t);

            transform.position = center + relPos;

            time += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        isOppositeSide = !isOppositeSide;
        isJumping = false;
        ResetPos();
    }    

    public void ResetTimer()
    {
        timer = 0f;
        hasShot1 = false;
        hasShot2 = false;
    }
    public IEnumerator ShootAnim()
    {
       animator.SetBool("IsShooting",true);
       yield return new WaitForSeconds(0.35f);
       animator.SetBool("IsShooting",false);
    }

}
