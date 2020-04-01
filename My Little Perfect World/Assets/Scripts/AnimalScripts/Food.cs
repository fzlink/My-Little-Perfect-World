using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public bool isBeingEaten { get; set; }
    public float beingEatenSpeed { get; set; }
    private float magnitude;
    private const float destroyMagnitudeThreshold = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        magnitude = transform.localScale.magnitude;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        //Decay();
        if (isBeingEaten)
        {
            transform.localScale -= Vector3.one * Time.deltaTime * beingEatenSpeed;
            if(transform.localScale.magnitude < destroyMagnitudeThreshold)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Decay()
    {
        
    }
}
