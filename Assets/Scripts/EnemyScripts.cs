using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection))]
public class EnemyScripts : MonoBehaviour
{
    Animator animator;
    public float walkSpeed;
    public DetectionZone attackZone;

    Rigidbody2D rb;
    TouchingDirection touchingDirection;
    public float walkStopRate = 0.6f;
    public enum walkableDirection { right,left}

    private walkableDirection _walkDirection;
    private Vector2 walkableDirectionVector;
    public walkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set {
            if (_walkDirection != value)
            {
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);
                if (value == walkableDirection.right)
                {
                    walkableDirectionVector = Vector2.right;
                }
                else if (value == walkableDirection.left)
                {
                    walkableDirectionVector = Vector2.left;
                }

            }
            _walkDirection = value; }

    }

    public bool _hasTarget = false;
    public bool hasTarget { get { return _hasTarget; } private set { _hasTarget = value;
           animator.SetBool( AnimationStrings.hasTarget,value);
        } 
    
    }
    public bool canMove
    {
        get
        {
            return animator.GetBool( AnimationStrings.canMove );
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirection = GetComponent<TouchingDirection>();
        animator = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        if(touchingDirection.IsOnWall && touchingDirection.IsGrounded)
        {
            Flipdirection();
        }
        if(canMove)
        {
            rb.velocity = new Vector2(walkSpeed * walkableDirectionVector.x, rb.velocity.y);

        }
        else
        {
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x,0,walkStopRate), rb.velocity.y);
        }
    }
    
    private void Flipdirection()
    {
        if(WalkDirection == walkableDirection.right )
        {
            WalkDirection = walkableDirection.left;
        }
        else if(WalkDirection == walkableDirection.left)
        {
            WalkDirection = walkableDirection.right;
        }
        else
        {
            Debug.Log("illegal walkdirection");
            
        }
    }
    private void Update()
    {
        hasTarget = attackZone.detectedcolliders.Count > 0;
    }
}
