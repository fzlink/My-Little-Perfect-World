
using UnityEngine;

public class EatingManager : MonoBehaviour
{
    public float eatingSpeed { get; set; }
    private bool canStartEating;
    public Transform food;
    private const float destroyMagnitudeThreshold = 0.5f;

    public void StartEating(Food food, float eatingSpeed)
    {
        this.food = food.transform;
        this.eatingSpeed = eatingSpeed;
        canStartEating = true;
    }

    private void Update()
    {
        if (canStartEating && food != null)
        {
            food.localScale -= Vector3.one * Time.deltaTime * eatingSpeed;
            if (food.localScale.magnitude < destroyMagnitudeThreshold || isScaleNegative())
            {
                AnimalInteractionManager.instance.FinishEating(this, true);
            }
        }
    }

    private bool isScaleNegative()
    {
        return food.localScale.x < 0 || food.localScale.y < 0 || food.localScale.z < 0;
    }
}

