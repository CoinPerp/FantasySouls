using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageAble : MonoBehaviour
{
    private Animator animator;
    private int _maxHealth = 100;
    [SerializeField]
    private int _health;
    private bool isAlive = true;
    private bool invincible = false;
    private float timeSinceHit = 0;
    public float invincibilityTime = 0.25f;

    public int maxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }

    public int health
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
                Debug.Log("Object has died!");
            }
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        _health = _maxHealth; // Initialize health to maxHealth
    }

    public void Hit(int damage)
    {
        if (isAlive && !invincible)
        {
            health -= damage;
            invincible = true; // Set invincible when hit
            timeSinceHit = 0; // Reset time since hit
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
        Hit(10);

    }
}
