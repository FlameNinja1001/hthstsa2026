using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatePlayer : MonoBehaviour
{
    public Transform model;     
    public ControlScript controlScript;
    public MeleeScript meleeScript;
    public float originalScale;    
    
    public Animator animator;    
    public ProjectileScript projectileScript;

    void Start()
    {
        originalScale = model.localScale.z;
        controlScript = GetComponent<ControlScript>();
        projectileScript = GetComponent<ProjectileScript>();
        meleeScript = GetComponent<MeleeScript>();
    }
    void Update()
    {        
        if (controlScript.moveDirection > 0)
        {
            model.localRotation = Quaternion.Euler(0, -80, 0);
            model.localScale = new Vector3(model.localScale.x, model.localScale.y, originalScale);            
        }
        else
        {
            model.localRotation = Quaternion.Euler(0, -100, 0);
            model.localScale = new Vector3(model.localScale.x, model.localScale.y, -originalScale);            
        }
        animator.SetInteger("StatusOfWeapon", projectileScript.ammoCount - 1);
        animator.SetBool("IsGrounded", controlScript.isGrounded);
        animator.SetBool("IsSlashing", meleeScript.isSlashing);
        animator.SetBool("IsDoubleJumping", controlScript.animBool);
        animator.SetBool("IsWallSliding", controlScript.isWallSliding);
        animator.SetBool("IsWallJumping", controlScript.isWallJumping);
        animator.SetBool("IsRunning", controlScript.move.x != 0);
        animator.SetBool("IsDashing", controlScript.isDashing);
        animator.SetBool("IsShooting", projectileScript.isShootAnimBoolActive);

        TMScript tMScript = FindObjectOfType<TMScript>(); 
        if (tMScript != null && !tMScript.canPlayerMove)
        {
            Debug.LogWarning("bum");
            animator.SetBool("IsGrounded", true);
            animator.SetBool("IsSlashing", false);
            animator.SetBool("IsDoubleJumping", false);
            animator.SetBool("IsWallSliding", false);
            animator.SetBool("IsWallJumping", false);
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsDashing", false);
            animator.SetBool("IsShooting", false);
        }
    }
}
