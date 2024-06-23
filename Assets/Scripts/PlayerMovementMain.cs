using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(DamageAble), typeof(TouchingDirecionPlayer))]
public class PlayerMovementMain : MonoBehaviour
{
    Rigidbody2D rb;
    DamageAble damageAble;
    TouchingDirecionPlayer touchingdirection;
    UIController UIcontrol;
    private float horizontal;
    public float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;
    public VariableJoystick variableJoystick; // Ensure you have assigned this in the inspector.
    private Animator animator;
    public int CurrentMoveSpeed;
    public float rollDistance = 5f;

    public LayerMask playerLayer;
    public LayerMask enemyLayer;
    public LayerMask enemyhitbox;
    public float rolljumpstamina = 20f;

    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;

    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private float rollBufferTime = 0.1f;
    private float rollBufferCounter;
    public float fixedRollDistance = 5f;

    private void Start()
    {
    }

    private void Awake()
    {
        animator = GetComponent<Animator>(); // Make sure your GameObject has an Animator component.
        touchingdirection = GetComponent<TouchingDirecionPlayer>();
        damageAble = GetComponent<DamageAble>();
        UIcontrol = GetComponent<UIController>();
        rb = GetComponent<Rigidbody2D>();
        // Set the layer masks to the appropriate layers
        playerLayer = LayerMask.NameToLayer("Player");
        enemyLayer = LayerMask.NameToLayer("Enemy");
        enemyhitbox = LayerMask.NameToLayer("EnemyHitbox");
    }

    private void Update()
    {
        if (touchingdirection.IsGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Update the jump buffer counter
        if (jumpBufferCounter > 0f)
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Update the roll buffer counter
        if (rollBufferCounter > 0f)
        {
            rollBufferCounter -= Time.deltaTime;
        }

        HandleJump(); // Handle jump logic in the update loop
        HandleRoll(); // Handle roll logic in the update loop
    }

    private void FixedUpdate()
    {
        float joystickInput = variableJoystick.Horizontal; // Make sure the joystick is setup correctly.
        float keyboardInput = Input.GetAxis("Horizontal");
        horizontal = joystickInput + keyboardInput;

        if (isAlive && CanMove)
        {
            horizontal = Mathf.Clamp(horizontal, -1f, 1f);

            if (!iswalled)
            {
                if (!isRolling)
                {
                    rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
                }
            }

            animator.SetFloat(AnimationStrings.Yvelocity, rb.velocity.y);

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
            if (isAlive && CanMove)
            {
                Flip();
            }

        }

        if (!isRolling)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
            Physics2D.IgnoreLayerCollision(playerLayer, enemyhitbox, false);

        }
    }
    
    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpBufferCounter = jumpBufferTime; // Set the buffer counter
        }
    }

    private void HandleJump()
    {
        
        
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f && isAlive && stamina > rolljumpstamina &&CanMove  )
        {
            animator.SetTrigger(AnimationStrings.Jump);
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            coyoteTimeCounter = 0f; // Reset coyote time after jump
            jumpBufferCounter = 0f; // Reset the buffer counter after jumping
            UIcontrol.ReduceStamina(rolljumpstamina);
        }
        
    }

    public void Roll(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rollBufferCounter = rollBufferTime; // Set the roll buffer counter
        }
    }

    private void HandleRoll()
    {
        if (rollBufferCounter > 0f && touchingdirection.IsGrounded && isAlive && CanMove &&!isRolling && stamina > rolljumpstamina &&CanMove)
        {
            float inputDirection = horizontal != 0 ? Mathf.Sign(horizontal) : (isFacingRight ? 1 : -1);

            // If there's no input, use the direction the player is facing
            float rollDirection = inputDirection != 0 ? inputDirection : (isFacingRight ? 1 : -1);
            float rollDistanceX = fixedRollDistance * rollDirection;

            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
            Physics2D.IgnoreLayerCollision(playerLayer, enemyhitbox, true);
            if (Mathf.Abs(rollDistanceX) > fixedRollDistance)
            {
                rollDistanceX = Mathf.Sign(rollDistanceX) * fixedRollDistance;
            }
            animator.SetTrigger(AnimationStrings.Roll);
            rb.velocity = new Vector2(rollDistanceX, rb.velocity.y);
            UIcontrol.ReduceStamina(rolljumpstamina);
            rollBufferCounter = 0f; // Reset the roll buffer counter after rolling
        }
    }

    public bool iswalled
    {
        get { return animator.GetBool(AnimationStrings.isOnWall); }
    }
    public bool isRolling
    {
        get { return animator.GetBool(AnimationStrings.Rolling); }
    }
    public bool isceiled
    {
        get { return animator.GetBool(AnimationStrings.isOnCelling); }
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
    public float stamina
    {
        get
        {
            return UIcontrol.GetStamina();
        }
    }

    private void Flip()
    {

            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;


    }
    public void AttackMoving(float x, float y)
    {
        float direction = isFacingRight ? 1 : -1;

        // Apply the velocity in the x direction according to the direction
        rb.velocity = new Vector2(direction * x, rb.velocity.y + y);
    }
}
