using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    public float movementSpeed = 5;
    public float awarenessRadius = 10;
    public float maxLookDownDistance = 100;
    public float fallRiseSpeed = 20;
    public float changeDirectionDelay = 3;

    private int groundMask;
    private int animalPlantMask;

    private BoxCollider boxCollider;

    private Vector3 scanOrigin;
    private Vector3 rayCastDirection;

    private bool changeDirection = false;

    private void Start()
    {
        scanOrigin = transform.position;
        boxCollider = GetComponent<BoxCollider>();
        groundMask = 1 << 8;
        animalPlantMask = (1 << 9) | (1 << 10);
    }

    private void Update()
    {
        MakeGrounded();
        ScanArea();
        RunAround();
    }

    private void RunAround()
    {
        if (!changeDirection)
        {
            StartCoroutine(ChangeDirection(changeDirectionDelay));
        }
        transform.position += transform.forward * Time.deltaTime * movementSpeed;
    }
    
    IEnumerator ChangeDirection(float seconds)
    {
        changeDirection = true;
        yield return new WaitForSeconds(seconds);
        Vector3 randomDirection = new Vector3(0, UnityEngine.Random.Range(-180, 180), 0);
        transform.rotation = Quaternion.Euler(randomDirection);
        changeDirection = false;
    }


    private void ScanArea()
    {
        scanOrigin = transform.position;
        Collider[] scannedColliders = Physics.OverlapSphere(scanOrigin, awarenessRadius, animalPlantMask);
        foreach (Collider collider in scannedColliders)
        {
            if(collider.transform != transform)
            {
                //print("Found another animal or plant");
            }
        }
    }

    public bool MakeGrounded()
    {
        RaycastHit hit;
        rayCastDirection = Vector3.down;
        if (Physics.Raycast(scanOrigin, rayCastDirection, out hit , maxLookDownDistance, groundMask))
        {
            if(hit.distance <= boxCollider.bounds.extents.y + 0.25f && hit.distance >= boxCollider.bounds.extents.y)
            {
                Debug.DrawLine(scanOrigin, hit.point, Color.green);
                return true;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, hit.point + Vector3.up/2, Time.deltaTime * 10);
                return false;
            }
        }
        else
        {
            transform.position += Vector3.up * Time.deltaTime * fallRiseSpeed;
            Debug.DrawRay(scanOrigin, rayCastDirection, Color.red);
            return false;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(scanOrigin, awarenessRadius); //ScanArea
    }

}
