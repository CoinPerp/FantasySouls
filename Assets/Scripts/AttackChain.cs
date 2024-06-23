using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackChain : MonoBehaviour
{
    public float holdThreshold = 1f; // Threshold to determine strong attack
    private float increaseRate = 1f;

    private Animator animator;
    private PlayerMovementMain player;
    private PlayerInput playerInput; // Reference to the PlayerInput component
    private InputAction normalAttack;
    private InputAction StrongAttack;
    TouchingDirecionPlayer touchingdirection;

    private GameObject strongattack;
    private attack attack;
    private float holdTime = 0f; // Track how long the button is held
    public float hold = 0f;
    public int JumpCount = 2;
    private void Awake()
    {
        strongattack = GameObject.Find("StrongAttack");
        attack = strongattack.GetComponent<attack>();
        player = GetComponent<PlayerMovementMain>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>(); // Ensure there's a PlayerInput component attached to the same GameObject
        normalAttack = playerInput.actions["Attack"]; // "Hit" should be replaced with the name of your actual input action
        StrongAttack = playerInput.actions["StrongAttack"]; // "Hit" should be replaced with the name of your actual input action
        StrongAttack.canceled += OnMoveActionCanceled;
        normalAttack.performed += OnNormalAttackPerformed;
        touchingdirection = GetComponent<TouchingDirecionPlayer>();



    }

    void Update()
    {
        if (StrongAttack.IsPressed())
        {
            if (holdTime <= 2f)
            {
                holdTime += Time.deltaTime; // Update holdTime continuously while StrongAttack is pressed
            }
            animator.SetBool(AnimationStrings.Hold,true);
            if (ishit)
            {

                holdTime = 0f;
                hold = holdTime;
            }
        }
        if(touchingdirection.IsGrounded)
        {
            JumpCount = 2;
        }


    }
    private void OnNormalAttackPerformed(InputAction.CallbackContext context)
    {
        if(!touchingdirection.IsGrounded)
        {
            animator.SetTrigger(AnimationStrings.AirAttack);
            if(JumpCount > 0)
            {
                player.AttackMoving(2f, 5f);
                Debug.Log("Jump");
                JumpCount--;
            }


        }


        if (touchingdirection.IsGrounded)
        {
            animator.SetTrigger(AnimationStrings.Attack);
            player.AttackMoving(2f, 0f);

        }
    }
    private void OnMoveActionCanceled(InputAction.CallbackContext context)
    {

        hold = holdTime;
        animator.SetBool(AnimationStrings.Hold, false);
        animator.SetTrigger(AnimationStrings.StrongAttack);
        float attackincrease = attack.attackdamage * holdTime * increaseRate;
        float maxAllowedIncrease = attack.attackdamage * 2f;
        // Limit the increase to the maximum allowed increase
        attackincrease = Mathf.Min(attackincrease, maxAllowedIncrease);
        float tempAttackDamage = attack.attackdamage;
        attack.attackdamage = attackincrease;
        attack.attackdamage = tempAttackDamage; // Reset the attack damage after use
        holdTime = 0f;
        player.AttackMoving(3f, 0f);

    }
    private bool ishit
    {
        get
        {
            return animator.GetBool(AnimationStrings.hit);
        }
    }
}