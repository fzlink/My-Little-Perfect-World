using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Iguana : Animal
{
    private Animator animator;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();

    }


    protected override void Update()
    {
        base.Update();
        switch (state)
        {
            case AnimalState.Wandering:
                animator.Play("WalkForward",0);
                break;
            case AnimalState.Eating:
            case AnimalState.Drinking:

                break;

        }

    }

}
