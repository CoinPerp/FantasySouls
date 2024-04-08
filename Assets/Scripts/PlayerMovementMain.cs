using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementMain : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private float horizontal;
    public float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;
    public VariableJoystick variableJoystick; // Ensure you have assigned this in the inspector.
    private Animator animator;
    public bool move;

    private void Start()
    {
        animator = GetComponent<Animator>(); // Make sure your GameObject has an Animator component.
        move = true;
    }

    private void FixedUpdate()
    {
        float joystickInput = variableJoystick.Horizontal; // Make sure the joystick is setup correctly.
        float keyboardInput = Input.GetAxis("Horizontal");
        horizontal = joystickInput + keyboardInput;
        if (move)
        {
            horizontal = Mathf.Clamp(horizontal, -1f, 1f);
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

            // Correctly setting "Ismoving" based on player movement and grounded state
            if (Mathf.Abs(horizontal) > 0f)
            {
                animator.SetBool(AnimationStrings.Ismoving, true);
            }
            else
            {
                animator.SetBool(AnimationStrings.Ismoving, false);
            }
        }

        // Fixed logic for Jump animation. It was inverted in your original code.
        if (IsGrounded())
        {
            animator.SetBool(AnimationStrings.Grounded, false);
        }
        else
        {
            animator.SetBool(AnimationStrings.Grounded, true);
        }

        if (!isFacingRight && horizontal > 0f || isFacingRight && horizontal < 0f)
        {
            Flip();
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    // Removed the Move method as it was not utilized properly in this script. 
    // If you're using the new Input System's action callbacks, set up those calls directly within the Input Action's settings.

    public IEnumerator ReduceSpeed(float percent, float duration)
    {
        float originalSpeed = speed;
        speed -= originalSpeed * (percent / 100f);

        yield return new WaitForSeconds(duration);

        speed = originalSpeed;
    }

    public void NudgePlayer(float distance)
    {
        float nudgeDirection = isFacingRight ? 1f : -1f;
        Vector2 nudgeVector = new Vector2(nudgeDirection * distance, 0);
        rb.position += nudgeVector;
    }

    public void moving()
    {
        move = true;
    }

    public void stop()
    {
        move = false;
    }
}
