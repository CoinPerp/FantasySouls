using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private float horizontal;
    public float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;
    public VariableJoystick variableJoystick;
    private Animator animator;
    public bool move;


    private void Start()
    {
        animator = GetComponent<Animator>();
        move = true;

    }
    void Update()
    {


        if (!isFacingRight && horizontal > 0f)
        {
            Flip();
        }
        else if (isFacingRight && horizontal < 0f)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        float joystickInput = variableJoystick.Horizontal;
        float keyboardInput = Input.GetAxis("Horizontal");
        horizontal = joystickInput + keyboardInput;
        if (move == true)
        {
            horizontal = Mathf.Clamp(horizontal, -1f, 1f);
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }


        if (IsGrounded())
        {
            animator.SetFloat("Speed", Mathf.Abs(horizontal));
            animator.SetBool("Jump", false);
        }
        else
        {
            animator.SetBool("Jump", true);
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
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
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