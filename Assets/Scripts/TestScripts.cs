using UnityEngine;
using UnityEngine.InputSystem;

public class TestScripts : MonoBehaviour
{
    public float knockbackForceX = 50f; // Adjust the knockback force for the x-axis as needed
    public float knockbackForceY = 50f; // Adjust the knockback force for the y-axis as needed

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object has a Rigidbody2D component
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Calculate the direction from the current object to the collided object
            Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;

            // Apply the knockback force to the collided object
            rb.velocity = Vector2.zero; // Reset the velocity to prevent accumulation of forces

            // Adjust the knockback force for each axis
            Vector2 knockbackForce = new Vector2(knockbackDirection.x * knockbackForceX, knockbackDirection.y * knockbackForceY);
            rb.AddForce(knockbackForce, ForceMode2D.Impulse);
        }
    }
}
