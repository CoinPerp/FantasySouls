using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection))]
public class EnemyScripts : MonoBehaviour
{
    private Transform playerTransform;

    Animator animator;
    public float walkSpeed;
    public float runSpeed;

    public DetectionZone attackZone;
    public CheckForPlayer checkZone;
    DamageAble damageAble;
    Rigidbody2D rb;
    TouchingDirection touchingDirection;
    public float lerpSpeed = 0.1f;
    public bool moveInBothAxes = false;
    public enum walkableDirection { right, left }

    private walkableDirection _walkDirection;
    private Vector2 walkableDirectionVector;
    public bool attackState;
    public bool walkState;
    public bool getPlayerpos;
    public walkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
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
            _walkDirection = value;
        }
    }

    public bool _hasTarget = false;
    public bool hasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }
    public bool _moveToTarget = false;
    public bool moveToTarget
    {
        get { return _moveToTarget; }
        private set
        {
            _moveToTarget = value;


        }
    }

    public bool canMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;

        touchingDirection = GetComponent<TouchingDirection>();
        animator = GetComponent<Animator>();
        damageAble = GetComponent<DamageAble>();
        walkState = true;

        // Get the DetectionZone and CheckForPlayer scripts from the child GameObjects
        Transform attackZoneTransform = transform.Find("AttackRange");
        Transform checkZoneTransform = transform.Find("CheckForPlayer");

        if (attackZoneTransform != null)
        {
            attackZone = attackZoneTransform.GetComponent<DetectionZone>();
            if (attackZone == null)
            {
                Debug.LogWarning("DetectionZone component not found on AttackRange");
            }
        }
        else
        {
            Debug.LogWarning("AttackRange object not found");
        }

        if (checkZoneTransform != null)
        {
            checkZone = checkZoneTransform.GetComponent<CheckForPlayer>();
            if (checkZone == null)
            {
                Debug.LogWarning("CheckForPlayer component not found on CheckForPlayer object");
            }
        }
        else
        {
            Debug.LogWarning("CheckForPlayer object not found");
        }
    }

    private void FixedUpdate()
    {
        if (canMove && touchingDirection.IsGrounded && !hasTarget && moveToTarget)
        {
            if ((playerTransform.position.x < transform.position.x && WalkDirection == walkableDirection.right) ||
                   (playerTransform.position.x > transform.position.x && WalkDirection == walkableDirection.left))
            {
                Flipdirection();
            }

            Vector2 direction = playerTransform.position - transform.position;

            if (!moveInBothAxes)
            {
                direction.y = 0; // Zero out the y direction if moveInBothAxes is false
            }

            direction.Normalize();
            rb.velocity = new Vector2(direction.x * walkSpeed, rb.velocity.y); // Preserve vertical velocity
            if (rb.velocity.x > 0)
            {
                WalkDirection = walkableDirection.right;
            }
            else if (rb.velocity.x < 0)
            {
                WalkDirection = walkableDirection.left;
            }
        }
        else if (touchingDirection.IsGrounded && canMove && attackState)
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // Preserve vertical velocity
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y); // Preserve vertical velocity
        }

    }

    private void Flipdirection()
    {
        if (WalkDirection == walkableDirection.right)
        {
            WalkDirection = walkableDirection.left;
        }
        else if (WalkDirection == walkableDirection.left)
        {
            WalkDirection = walkableDirection.right; 
        }
        else
        {
            Debug.Log("Illegal walk direction");
        }
    }

    private void Update()
    {
        if (attackZone.detectedcolliders.Count > 0)
        {
            hasTarget = true;
        }
        else
        {

            hasTarget = false;
        }
        if(checkZone.playerPos)
        {
            moveToTarget = true;
        }
        else
        {
            moveToTarget = false;

        }

    }

}