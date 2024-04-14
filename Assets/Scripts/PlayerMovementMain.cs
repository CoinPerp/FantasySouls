using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementMain : MonoBehaviour
{
    public Rigidbody2D rb;

    TouchingDirection touchingdirection;
    private float horizontal;
    public float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;
    public VariableJoystick variableJoystick; // Ensure you have assigned this in the inspector.
    private Animator animator;
    public bool move;

    private void Start()
    {

    }
    private void Awake()
    {
        animator = GetComponent<Animator>(); // Make sure your GameObject has an Animator component.
        move = true;
        touchingdirection = GetComponent<TouchingDirection>();
    }

    private void FixedUpdate()
    {
        float joystickInput = variableJoystick.Horizontal; // Make sure the joystick is setup correctly.
        float keyboardInput = Input.GetAxis("Horizontal");
        horizontal = joystickInput + keyboardInput;
        if (CanMove && isAlive)
        {
            horizontal = Mathf.Clamp(horizontal, -1f, 1f);
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
            animator.SetFloat(AnimationStrings.Yvelocity, rb.velocity.y);


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


        if (!isFacingRight && horizontal > 0f || isFacingRight && horizontal < 0f)
        {
            if(isAlive)
            {
                Flip();
            }
            else
            {
                Debug.Log("target not alive can't flip");
            }
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && touchingdirection.IsGrounded)
        {
            animator.SetTrigger(AnimationStrings.Jump);

            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }
    }

 
    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }
    public bool isAlive
    {
        get
        {
            return animator.GetBool(AnimationStrings.isAlive);
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }


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
