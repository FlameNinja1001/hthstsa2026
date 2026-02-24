using UnityEngine;

public class PlayerAnims : MonoBehaviour
{
    public Animator animator;
    public ControlScript controlScript;
    public ProjectileScript projectileScript;
    public GameObject targetObject;

    private float ogScale;
    private float ogRot;

    private Transform targetTransform;

    void Start()
    {
        targetTransform = targetObject.transform;

        // Store original values
        ogScale = targetTransform.localScale.x;
        ogRot = targetTransform.localEulerAngles.y;
    }

    void Update()
    {
        // Get direction safely (prevents weird scaling)
        float direction = Mathf.Sign(controlScript.moveDirection);

        if (direction != 0)
        {
            // Flip scale
            targetTransform.localScale = new Vector3(
                ogScale * -direction,
                targetTransform.localScale.y,
                targetTransform.localScale.z
            );

            // Rotate on Y axis
            targetTransform.localEulerAngles = new Vector3(
                targetTransform.localEulerAngles.x,
                ogRot * -direction,
                targetTransform.localEulerAngles.z
            );
        }

        // Animator bools!
        anim.SetInteger("StatusOfWeapon", projectileScript.ammoCount - 1);
        animator.SetBool("IsGrounded", controlScript.isGrounded);
        animator.SetBool("IsDoubleJumping", controlScript.animBool);
        animator.SetBool("IsWallSliding", controlScript.isWallSliding);
        animator.SetBool("IsWallJumping", controlScript.isWallJumping);
        animator.SetBool("IsRunning", controlScript.move.x != 0);
        animator.SetBool("IsDashing", controlScript.isDashing);
        animator.SetBool("IsShooting", projectileScript.isShootAnimBoolActive);
    }
}
