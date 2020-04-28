using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Vegetation : Food
{
    public string vegetationType;

    public override void Finish(Animal animal)
    {
        if(UnityEngine.Random.value < 0.2f)
        {
            GetComponentInChildren<Renderer>().enabled = false;
            LatchSeed(animal);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private void LatchSeed(Animal animal)
    {
        StartCoroutine(DropSeed(animal,UnityEngine.Random.Range(5, 10)));
    }
    private IEnumerator DropSeed(Animal animal,float secondsToDrop)
    {
        yield return new WaitForSeconds(secondsToDrop);
        if(animal != null)
        {
            transform.position = animal.transform.position - transform.forward;
            GetComponent<Plant>().enabled = true;
            GetComponentInChildren<Renderer>().enabled = true;
            Destroy(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}

