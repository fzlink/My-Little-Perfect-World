
using UnityEngine;

public class EatingManager : MonoBehaviour
{
    private Animal animal;
    private bool canStartEating;
    public Transform food;
    private const float destroyMagnitudeThreshold = 0.5f;

    public void StartEating(Food food, Animal animal)
    {
        this.food = food.transform;
        this.animal = animal;
        canStartEating = true;
    }

    private void Update()
    {
        if (canStartEating && food != null)
        {
            food.localScale -= Vector3.one * Time.deltaTime * animal.GetProperties().FoodEatingSpeed;
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

