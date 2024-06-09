using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageAble : MonoBehaviour
{
    public UnityEvent<float, Vector2> damageableHit;
    private Animator animator;
    private float _maxHealth = 100f;
    [SerializeField]
    private float _health;
    private bool isAlive = true;
    private bool invincible = false;
    Rigidbody2D rb;
    private CapsuleCollider2D boxCollider;
    private float recentDamageAmount = 0; // Variable to store recent damage amount



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
       // _health = _maxHealth; // Initialize health to maxHealth
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<CapsuleCollider2D>();

    }

    public void Hit(float damage)
    {
        if (isAlive && !invincible)
        {
            ResetRecentDamage();
            health -= damage;
            invincible = true; 
            Hit(damage);
            lockVelocity = true;
            animator.SetTrigger(AnimationStrings.hit);
            timeSinceHit = 0; // Reset time since hit
            recentDamageAmount = damage; // Add the damage to the recent damage amount

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
    public void Onhit(int damage, Vector2 knockback)
    {
        if(rb != null)
        {
            rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
            Debug.Log(knockback.x +"and"+ knockback.y);
        }
        else
        {
            Debug.Log("cant find rb");
        }

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
