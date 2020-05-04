using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meat : Food
{

    public string animalType { get; set; }
    private const float decaySpeed = 0.01f;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        Decay();
    }

    private void Decay()
    {
        if (!isBeingEaten)
        {
            transform.localScale -= Vector3.one * Time.deltaTime * decaySpeed * FourthDimension.tSM;
            if (transform.localScale.magnitude < destroyMagnitudeThreshold || isScaleNegative())
                Destroy(gameObject);
        }
    }
}
