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
    public float nutritionValue { get; set; }

    protected const float destroyMagnitudeThreshold = 0.5f;


    // Start is called before the first frame update
    void Start()
    {
        magnitude = transform.localScale.magnitude;
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
        return transform.localScale.magnitude > destroyMagnitudeThreshold;
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    protected bool isScaleNegative()
    {
        return transform.localScale.x < 0 || transform.localScale.y < 0 || transform.localScale.z < 0;
    }
}
