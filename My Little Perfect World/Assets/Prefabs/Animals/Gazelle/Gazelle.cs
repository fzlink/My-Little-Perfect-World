using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gazelle : Animal
{
    private Animation animation;
    private MoveController moveController;

    protected override void Awake()
    {
        base.Awake();
        animation = GetComponent<Animation>();
        moveController = GetComponent<MoveController>();
    }


    protected override void Update()
    {
        base.Update();
        switch (state)
        {
            case AnimalState.Wandering:
                if (!animation.IsPlaying("Walk"))
                {
                    animation.Play("Walk");
                }
                break;
            case AnimalState.GoingToSomething:
                if (!animation.IsPlaying("Run Fast") && moveController.isRunning)
                {
                    animation.Play("Run Fast");
                }
                break;
            case AnimalState.Eating:
            case AnimalState.Drinking:
                if (!animation.IsPlaying("Eating"))
                {
                    animation.Play("Eating");
                }
                break;
            case AnimalState.Sleeping:
                if (!animation.IsPlaying("Rest"))
                {
                    animation.Play("Rest");
                }
                break;
        }

    }
}
