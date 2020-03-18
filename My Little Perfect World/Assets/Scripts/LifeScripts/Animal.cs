using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : Creature
{
    public DNA.Sex sex;
    public DNA.Sex oppositeSex
    {
        get
        {
            if(sex == DNA.Sex.Male)
            {
                return DNA.Sex.Female;
            }
            else
            {
                return DNA.Sex.Male;
            }
        }
    }


    public bool hasAncestor;
    public float movementSpeed = 5;
    public float awarenessRadius = 10;
    public float maxLookDownDistance = 100;
    public float maxLookDownForItemDistance = 100;
    public float fallRiseSpeed = 20;
    public float changeDirectionDelay = 3;

    private int groundMask;
    private int animalPlantMask;

    public float thirst = 20;
    private float thirstThreshold = 10;
    public float drinkSpeed;

    private Coroutine changeDirectionCoroutine;

    private BoxCollider boxCollider;

    private Vector3 scanOrigin;
    private Vector3 rayCastDirection;

    private bool changeDirection = false;
    private Vector3 waterLocation;
    private Vector3 enemyPoint;
    private bool goingToWater = false;
    private bool drinking;

    private bool onChase;


    Vector3[] directions;
    private bool caughtOnDrinking;
    private bool onRun;
    private bool onHunt;
    private bool beingEaten;
    private bool eating;

    private Soil soil;
    public float eatingSpeed;
    private GameObject enemyGameObject;

    private float scaleMagnitude;

    private void Start()
    {
        if (!hasAncestor)
        {
            if(UnityEngine.Random.value <= 0.5)
            {
                sex = DNA.Sex.Male;
            }
            else
            {
                sex = DNA.Sex.Female;
            }
        }
        changeDirectionCoroutine = StartCoroutine(ChangeDirection(changeDirectionDelay));
        scaleMagnitude = transform.localScale.magnitude;
        soil = FindObjectOfType<Soil>();
        scanOrigin = transform.position;
        boxCollider = GetComponent<BoxCollider>();
        groundMask = 1 << 8;
        animalPlantMask = (1 << 9) | (1 << 10);
        directions = new Vector3[5];
        directions[0] = (new Vector3(-10, -1, 10)).normalized;
        directions[1] = (new Vector3(-5, -1, 10)).normalized;
        directions[2] = (new Vector3(0, -1, 10)).normalized;
        directions[3] = (new Vector3(5, -1, 10)).normalized;
        directions[4] = (new Vector3(10, -1, 10)).normalized;
    }

    private void Update()
    {
        MakeGrounded();
        ScanArea();
        RunAround();
        ReEvaluateProperties();
        if (eating)
        {
            Eat();
        }
        if (drinking)
        {
            DrinkWater();
        }
        
    }

    private void Die()
    {
        soil.IncreaseDead(scaleMagnitude);
        Destroy(gameObject);
    }

    private void Eat()
    {
        if(enemyGameObject == null) { eating = false; }
        enemyGameObject.transform.localScale -= Vector3.one * eatingSpeed * Time.deltaTime;
        if(enemyGameObject.transform.localScale.magnitude < 0.5f)
        {
            enemyGameObject.GetComponent<Animal>().Die();
            eating = false;
        }
    }

    private void ReEvaluateProperties()
    {
        
    }

    private void RunAround()
    {
        if (drinking || eating || beingEaten) { return; }
        if (!changeDirection)
        {
            changeDirectionCoroutine =  StartCoroutine(ChangeDirection(changeDirectionDelay));
        }
        transform.position += transform.forward * Time.deltaTime * movementSpeed;
    }


    private void DrinkWater()
    {
        thirst -= drinkSpeed * Time.deltaTime;
        if(thirst < 0)
        {
            thirst = 0;
            drinking = false;
            changeDirection = false;
        }
    }


    IEnumerator ChangeDirection(float seconds)
    {
        changeDirection = true;
        yield return new WaitForSeconds(seconds);
        Vector3 randomDirection = new Vector3(0, UnityEngine.Random.Range(-180, 180), 0);
        transform.rotation = Quaternion.Euler(randomDirection);
        changeDirection = false;
    }

    private void GoToWater(RaycastHit hit)
    {
        StopCoroutine(changeDirectionCoroutine);
        changeDirection = true;
        goingToWater = true;
        waterLocation = hit.point;
        transform.LookAt(hit.point);
    }

    private void Run(Vector3 enemyPoint)
    {
        transform.rotation = Quaternion.LookRotation(transform.position - enemyPoint);
    }

    private void Hunt(Vector3 enemyPoint)
    {
        transform.rotation = Quaternion.LookRotation(enemyPoint - transform.position);
    }

    private void Reproduce()
    {
        if(sex == DNA.Sex.Female)
        {
            GetPregnant();
        }
    }

    private void GetPregnant()
    {
        throw new NotImplementedException();
    }

    private void ScanArea()
    {
        scanOrigin = transform.position;
        if(beingEaten || eating) { return; }

            //Scan For Enemies or Friends
            Collider[] scannedColliders = Physics.OverlapSphere(scanOrigin, awarenessRadius, animalPlantMask);
            foreach (Collider collider in scannedColliders)
            {
                if(collider.transform != transform)
                {
                    
                    if (collider.gameObject.CompareTag(tag) && oppositeSex == collider.gameObject.GetComponent<Animal>().sex)
                    {
                        //Reproduce();
                    }
                    else
                    {
                        enemyGameObject = collider.gameObject;
                        byte enemyFoodChainIndex = collider.GetComponent<Creature>().foodChainIndex;
                        if (enemyFoodChainIndex != foodChainIndex)
                        {
                            onChase = true;
                            drinking = false; // todo drinking and eating priority setting
                            changeDirection = true;
                            goingToWater = false;
                            enemyPoint = collider.ClosestPoint(transform.position);
                            if (enemyFoodChainIndex > foodChainIndex)
                            {
                                onRun = true;
                                Run(enemyPoint);
                            }
                            else if (enemyFoodChainIndex < foodChainIndex)
                            {
                                onHunt = true;
                                Hunt(enemyPoint);
                            }
                        }
                    }
                    
                   

                }
                
            }
        

        if (onChase)
        {
            if(Vector3.Distance(transform.position, enemyPoint) <= 1f)
            {
                onChase = false;
                if (onRun)
                {
                    onRun = false;
                    beingEaten = true;
                }
                if (onHunt)
                {
                    onHunt = false;
                    eating = true;
                }
            }
            else if(Vector3.Distance(transform.position, enemyPoint) >= 25f)
            {
                onChase = false;
                onRun = false;
            }
        }

        int groundWaterMask = (1 << 4) | (1 << 8);
        if (!drinking && thirst >= thirstThreshold && !onChase)
        {
            //Search for Water
            for (int i = 0; i < directions.Length; i++)
            {
                RaycastHit hit;
                Vector3 direction = directions[i];
                direction = transform.TransformVector(direction);
                if (Physics.Raycast(scanOrigin, direction, out hit, maxLookDownForItemDistance, groundWaterMask))
                {
                    if (hit.transform.gameObject.layer == 4)
                    {
                        Debug.DrawLine(scanOrigin, hit.point, Color.green);
                        if (thirst >= thirstThreshold)
                        {
                            //print(hit.textureCoord);
                            GoToWater(hit);
                        }
                    }
                    else
                    {
                        Debug.DrawRay(scanOrigin, direction * hit.distance, Color.black);
                    }
                }
            }
        }

        if (goingToWater)
        {
            if (Vector3.Distance(transform.position,waterLocation) <= 2f)
            {
                //Debug.Log(gameObject.name + " is drinking water");
                goingToWater = false;
                drinking = true;
            }
        }

        RaycastHit hitItem;

        if (Physics.Raycast(scanOrigin + transform.forward * 2, rayCastDirection, out hitItem, maxLookDownForItemDistance, groundWaterMask))
        {
            if (hitItem.collider.gameObject.layer == 4)
            {
                //print("Hit water");
                if (thirst <= thirstThreshold || onChase)
                {
                    Debug.DrawLine(scanOrigin + transform.forward * 2, hitItem.point, Color.blue);
                    //transform.rotation = Quaternion.LookRotation(transform.position - hitItem.point);
                    Vector3 randomDirection = new Vector3(0, UnityEngine.Random.Range(-180, 180), 0);
                    transform.rotation = Quaternion.Euler(randomDirection);
                }
            }
            else
            {
                Debug.DrawLine(scanOrigin + transform.forward * 2, hitItem.point, Color.magenta);
                //print("Hit Ground");
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

        //Gizmos.DrawSphere((waterLocation + enemyPoint) / 2, 0.5f);
    }

}
