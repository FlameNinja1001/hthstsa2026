using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LancelotBossScript : MonoBehaviour
{
    private Rigidbody rb;
    public string stateString = "Walking";
    public float walkSpeed;
    public Transform player;
    public float ogScaleX;
    public Transform model;
    public float moveDir;
    public float dashSpeed;
    public Animator animator;
    public float customX = 1.447315f;
    public float ogModelX = 1.106341f;
    public bool jumpInitialVelocitySet;
    public float jumpVelocityY;
    public float jumpVelocityX;
    public float jumpVelocityXInitial;
    public float jumpVelocityXFall;
    public float initialGravity;
    public float fallGravity;
    public float gravity;
    public float playerDistThreshold;    
    public GameObject collider;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ogScaleX = transform.localScale.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (stateString == "Walking")
        {
            if (Mathf.Abs(transform.position.x - player.position.x) < playerDistThreshold)
            {
                collider.SetActive(true);
                animator.SetBool("isSlashing", true);
            }
            else
            {
                animator.SetBool("isSlashing", false);
                collider.SetActive(false);
            }
            jumpInitialVelocitySet = false;
            animator.SetBool("isDashing",false);
            animator.SetBool("isUppercutting",false);
            if (player.position.x > transform.position.x)
            {
                rb.velocity = new Vector3(walkSpeed,0f,0f);
                transform.localScale = new Vector3(-ogScaleX,transform.localScale.y,transform.localScale.z);
                model.localRotation = Quaternion.Euler(0,0,0);
                model.localScale = new Vector3(ogModelX,model.localScale.y,model.localScale.z);
                moveDir = 1;
            }
            else
            {    
                            
                rb.velocity = new Vector3(-walkSpeed,0f,0f);
                transform.localScale = new Vector3(ogScaleX,transform.localScale.y,transform.localScale.z);
                model.localRotation = Quaternion.Euler(0,0,0);
                model.localScale = new Vector3(ogModelX,model.localScale.y,model.localScale.z);
                moveDir = -1;
            }
        }
        if (stateString == "Dashing")
        {
            animator.SetBool("isSlashing", false);
            collider.SetActive(false);
            jumpInitialVelocitySet = false;
            animator.SetBool("isUppercutting",false);
            animator.SetBool("isDashing",true);
            rb.velocity = new Vector3(dashSpeed * moveDir,0f,0f);
            model.localRotation = Quaternion.Euler(0,45,0); 
            model.localScale = new Vector3(customX,model.localScale.y,model.localScale.z);
                       
        }
        if (stateString == "Jumping")
        {
            animator.SetBool("isSlashing", false);
            collider.SetActive(false);
            if (!jumpInitialVelocitySet)
            {
                jumpInitialVelocitySet = true;
                rb.velocity = new Vector3(jumpVelocityX * moveDir,jumpVelocityY,0f);
            }
            animator.SetBool("isUppercutting",true);
            animator.SetBool("isDashing",false);         

            rb.velocity -= new Vector3(0f,gravity,0f);            
            model.localScale = new Vector3(ogModelX,model.localScale.y,model.localScale.z);       
            if (rb.velocity.y < 0f)
            {
                gravity = fallGravity;
                jumpVelocityX = jumpVelocityXFall;
            }
            else
            {
                gravity = initialGravity;
                jumpVelocityX = jumpVelocityXInitial;
            }
                       
        }
    }
}
