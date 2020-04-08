using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public bool isBeingEaten { get; set; }
    public bool isDecayed { get; set; }

    public float beingEatenSpeed { get; set; }
    private float magnitude;
    public float nutritionValue;

    private const float destroyMagnitudeThreshold = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        magnitude = transform.localScale.magnitude;
        nutritionValue = 50f;
    }

    public void StartBeingEaten(float beingEatenSpeed)
    {
        this.beingEatenSpeed = beingEatenSpeed;
        isBeingEaten = true;
    }

    public virtual void Finish(Animal animal)
    {
        Destroy(gameObject);
    }

    public bool IsEdible()
    {
        return transform.localScale.magnitude > 0.5f;
    }

    // Update is called once per frame
    //protected virtual void Update()
    //{
    //    //Decay();
    //    //if (isBeingEaten)
    //    {
    //        //transform.localScale -= Vector3.one * Time.deltaTime * beingEatenSpeed;
    //        //if(transform.localScale.magnitude < destroyMagnitudeThreshold || isScaleNegative())
    //        //{
    //        //    Destroy(gameObject);
    //        //}
    //    }
    //}

    //private void Decay()
    //{
        
    //}
}
