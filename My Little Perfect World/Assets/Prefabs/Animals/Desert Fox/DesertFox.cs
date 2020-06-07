using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertFox : Animal
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
                animator.SetBool("isDrinking", false);
                animator.SetBool("isEating", false);
                animator.SetBool("isSleeping", false);
                break;
            case AnimalState.Reproducing:
            case AnimalState.Eating:
                animator.SetBool("isEating", true);
                break;
            case AnimalState.Drinking:
                animator.SetBool("isDrinking", true);
                break;
            case AnimalState.GoingToSomething:
                if (moveController.isRunning)
                {
                    animator.SetBool("isRunning", true);
                }
                break;
            case AnimalState.Sleeping:
                animator.SetBool("isSleeping", true);
                break;
        }

    }

}
