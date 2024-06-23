using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attack : MonoBehaviour
{
    public float attackdamage = 30;
    Collider2D attackcollider;
    public bool ischarge;
    private bool ishit;
    private UIController uicontroller;
    public float knockbackForceX = 50f; // Adjust the knockback force for the x-axis as needed
    public float knockbackForceY = 50f; // Adjust the knockback force for the y-axis as needed

    private void Start()
    {
        attackcollider = GetComponent<Collider2D>();

    }
    private void Awake()
    {
        GameObject playerObject = GameObject.Find("Player");
        uicontroller = playerObject.GetComponent<UIController>();
        ishit = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageAble damageAble = collision.GetComponent<DamageAble>();
        if(damageAble != null)
        {
            damageAble.Hit(attackdamage);
            if (collision.CompareTag("Enemy"))
            {
                if (damageAble != null)
                {
                    uicontroller.RecoverAfterAttack(ischarge, 1f);
                }

            }
        }
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Calculate the direction from the current object to the collided object
            Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;

            // Apply the knockback force to the collided object
            rb.velocity = Vector2.zero; // Reset the velocity to prevent accumulation of forces

            // Adjust the knockback force for each axis
            Vector2 knockbackForce = new Vector2(knockbackDirection.x * knockbackForceX, knockbackDirection.y * knockbackForceY);
            rb.AddForce(knockbackForce, ForceMode2D.Impulse);
        }

    }
    Vector2 CalculateKnockback(Vector2 knockback)
    {
        if (transform.parent.localScale.x > 0)
        {
            return knockback;

        }
        else
        {
            Vector2 invertedKnockback = new Vector2(-knockback.x, knockback.y);
            return invertedKnockback;
        }
    }
    public bool isHit()
    {
        return ishit;
    }
}
