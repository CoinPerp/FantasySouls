using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageAble : MonoBehaviour
{
    private Animator animator;
    private float _maxHealth = 100f;
    [SerializeField]
    private float _health;
    private bool isAlive = true;
    private bool invincible = false;
    Rigidbody2D rb;
    private CapsuleCollider2D boxCollider;
    private float recentDamageAmount = 0; // Variable to store recent damage amount
    public float poise = 100f;
    public float Maxpoise = 100f;
    public float counterDuration = 5.0f; // Duration for the counter in seconds
    private Coroutine poiseResetCoroutine;



    private float timeSinceHit = 0;
    public float invincibilityTime = 0.25f;

    public float maxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }

    public float health
    {
        get { return _health; }
        set
        {
            _health = value;
            if (_health <= 0)
            {
                _health = 0;
                isAlive = false;
                // Optionally, perform death logic here
                animator.SetBool("isAlive", false);
                boxCollider.enabled = false;

            }
        }
    }
    public bool lockVelocity
    {
        get
        {
            return animator.GetBool(AnimationStrings.lockVelocity);
        }
        set
        {
            animator.SetBool(AnimationStrings.lockVelocity, value);
        }

    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<CapsuleCollider2D>();
        poise = Maxpoise;

    }

    public void Hit(float damage, float poiseDamage)
    {
        if (isAlive && !invincible)
        {
            ResetRecentDamage();
            if(poise >0)
            {
                health -= damage;

            }
            else
            {
                health -= damage * 2;
            }

            invincible = true; 
            Hit(damage, poiseDamage);
            lockVelocity = true;
            poise -= poiseDamage;
            timeSinceHit = 0; // Reset time since hit
            recentDamageAmount = damage; // Add the damage to the recent damage amount
            if (poise <= 0)
            {
                animator.SetTrigger(AnimationStrings.hit);
                poiseResetCoroutine = StartCoroutine(ResetPoiseAfterDelay(0.5f));
            }
            if (poiseResetCoroutine != null)
            {
                StopCoroutine(poiseResetCoroutine);
            }
            poiseResetCoroutine = StartCoroutine(ResetPoiseAfterDelay(5.0f));
        }
    }

    private void Update()
    {
        if (invincible)
        {
            timeSinceHit += Time.deltaTime;
            if (timeSinceHit >= invincibilityTime)
            {
                invincible = false; // End invincibility after invincibilityTime
            }
        }
    }


    private IEnumerator ResetPoiseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        resetpoise();
    }
    public void resetpoise()
    {
        poise = Maxpoise;
    }
    public void RecoverHealth(float amount)
    {
        if (isAlive)
        {
            _health += Mathf.RoundToInt(amount); // Convert the float amount to an int
            _health = Mathf.Clamp(_health, 0, _maxHealth); // Ensure health does not exceed maxHealth
            
            if (!isAlive && _health > 0) // If recovering from a state of 0 health
            {
                isAlive = true;
                animator.SetBool("isAlive", true);
                boxCollider.enabled = true;
            }
        }
    }

    private IEnumerator PoiseHit()
    {

        yield return new WaitForSeconds(counterDuration); // Wait for the specified duration
        poise = Maxpoise;
    }
    public float GetRecentDamageAmount() 
    {
        return recentDamageAmount;
    }
    public void reduceRecentAmountDmg(float amount)
    {
        recentDamageAmount -= amount;
    }

    // Method to reset recent damage amount
    public void ResetRecentDamage()
    {
        recentDamageAmount = 0;
    }
 
}
