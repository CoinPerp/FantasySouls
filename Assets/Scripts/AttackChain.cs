using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackChain : MonoBehaviour
{
    public float holdThreshold = 1f; // Threshold to determine strong attack
    private float increaseRate = 1f;

    private Animator animator;
    private PlayerMovement player;
    private PlayerInput playerInput; // Reference to the PlayerInput component
    private InputAction normalAttack;
    private InputAction StrongAttack;

    private GameObject strongattack;
    private attack attack;
    private float holdTime = 0f; // Track how long the button is held
    public float hold = 0f;
    private void Awake()
    {
        strongattack = GameObject.Find("StrongAttack");
        attack = strongattack.GetComponent<attack>();
        player = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>(); // Ensure there's a PlayerInput component attached to the same GameObject
        normalAttack = playerInput.actions["Attack"]; // "Hit" should be replaced with the name of your actual input action
        StrongAttack = playerInput.actions["StrongAttack"]; // "Hit" should be replaced with the name of your actual input action
        StrongAttack.canceled += OnMoveActionCanceled;
        normalAttack.performed += OnNormalAttackPerformed;
 


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


    }
    private void OnNormalAttackPerformed(InputAction.CallbackContext context)
    {
        animator.SetTrigger(AnimationStrings.Attack);
    }
    private void OnMoveActionCanceled(InputAction.CallbackContext context)
    {

        hold = holdTime;
        Debug.Log(hold);
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
        
    }
    private bool ishit
    {
        get
        {
            return animator.GetBool(AnimationStrings.hit);
        }
    }
}