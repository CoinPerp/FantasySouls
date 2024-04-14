using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Import the UI namespace to work with UI elements
using UnityEngine.InputSystem;

public class AttackChain : MonoBehaviour
{
    public Slider slider;
    public float increaseRate = 1f;
    private bool sliderWasFilled = false;

    private Animator animator;
    private PlayerMovement player;
    private PlayerInput playerInput; // Reference to the PlayerInput component
    private InputAction moveAction;

    private void Start()
    {
        player = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>(); // Ensure there's a PlayerInput component attached to the same GameObject
        moveAction = playerInput.actions["Hit"]; // "Move" should be replaced with the name of your actual input action
        moveAction.canceled += OnMoveActionCanceled;
        moveAction.started += OnMoveActionStarted;
    }

    void Update()
    {
        if (moveAction.IsPressed())
        {
            animator.SetBool(AnimationStrings.Hold, true); // Set the Hold boolean to true

            if (slider.value < slider.maxValue)
            {
                slider.value += increaseRate * Time.deltaTime;
            }
            else if (slider.value >= slider.maxValue)
            {
                sliderWasFilled = true;
            }
        }
        else
        {
            animator.SetBool(AnimationStrings.Hold, false);
        }
    }

    private void OnMoveActionStarted(InputAction.CallbackContext context)
    {
        // Now handled in Update
    }

    private void OnMoveActionCanceled(InputAction.CallbackContext context)
    {
        if (sliderWasFilled)
        {
            animator.SetTrigger(AnimationStrings.StrongAttack);
        }
        else
        {
            animator.SetTrigger(AnimationStrings.Attack);
        }
        ResetSlider();
        animator.SetBool(AnimationStrings.Hold, false);
    }

    private void ResetSlider()
    {
        slider.value = slider.minValue;
        sliderWasFilled = false;
    }
}
