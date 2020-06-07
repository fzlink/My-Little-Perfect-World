using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rabbit : Animal
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
        animator.SetBool("isWalking", true);
        switch (state)
        {
            case AnimalState.Wandering:
                animator.SetBool("isWalking", true);
                animator.SetBool("isRunning", false);
                break;
            case AnimalState.GoingToSomething:
                if (moveController.isRunning)
                {
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isRunning", true);
                }
                break;
            case AnimalState.Drinking:
            case AnimalState.Sleeping:
            case AnimalState.Reproducing:
            case AnimalState.Eating:
                animator.SetBool("isWalking", false);
                animator.SetBool("isRunning", false);
                break;

        }
    }
}
