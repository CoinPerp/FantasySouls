using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackChain : MonoBehaviour
{

    private Animator animator;
    private PlayerMovement player;

    private void Start()
    {
        player = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();    

    }

    private int currentAttackIndex = -1; // -1 indicates no attack has been started
    private float chainWindow = 0.75f; // Time in seconds to allow the next attack in the chain
    private bool isChaining = false;

    void Update()
    {
        // Check for the attack input
        if (Input.GetKeyDown(KeyCode.E))
        {
            ChainAttack();
        }
    }

    public void ChainAttack()
    {
        if (!isChaining || currentAttackIndex >= 2)
        {
            currentAttackIndex = 0;
        }
        else
        {
            currentAttackIndex++;

        }

        StopAllCoroutines(); // Ensure no other chain windows are running
        StartCoroutine(AttackChainWindow());

        switch (currentAttackIndex)
        {
            case 0:
                Attack1();
                break;
            case 1:
                Attack2();
                break;
            case 2:
                Attack3();
                break;
        }
    }

    IEnumerator AttackChainWindow()
    {
        isChaining = true;
        yield return new WaitForSeconds(chainWindow);
        isChaining = false;
    }

    void Attack1()
    {
        animator.SetTrigger("Attack1");

    }

    void Attack2()
    {
        animator.SetTrigger("Attack2");

    }

    void Attack3()
    {
        animator.SetTrigger("Attack3");

    }
}
