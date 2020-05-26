using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capybara : Animal
{
    private Animator animator;
    private MoveController moveController;
    protected override void Awake()
    {
        base.Awake();

        animator = GetComponent<Animator>();
        moveController = GetComponent<MoveController>();
    }

    protected override void Update()
    {
        base.Update();

        switch (state)
        {
            case AnimalState.Wandering:
                animator.SetBool("isWalking", true);
                animator.SetBool("isRunning", false);
                animator.SetBool("isEating", false);
                break;
            case AnimalState.GoingToSomething:
                if (moveController.isRunning)
                {
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isRunning", true);
                    animator.SetBool("isEating", false);
                }
                break;
            case AnimalState.Drinking:
            case AnimalState.Eating:
                animator.SetBool("isEating", true);
                animator.SetBool("isRunning", false);
                animator.SetBool("isWalking", false);
                break;

        }
    }

}
