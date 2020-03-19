using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : Creature
{
    private enum AnimalState {
        Wandering = 0,
        Sleeping = 1,
        Running = 2,
        Drinking = 3,
        Eating = 4,
        Reproducing = 5,
        BeingEaten = 6,
    }
    private AnimalState state;

    [SerializeField] private DNA dNA;
    [SerializeField] private AnimalProperties properties;
    private const int maxLookDownDistance = 100;
    private const int maxLookDownForItemDistance = 20;
    private const int groundMask = 1 << 8;
    private const int animalPlantMask = (1 << 9) | (1 << 10);

    private float thirst = 20;
    public bool hasAncestor { get; set; }

    private Coroutine changeDirectionCoroutine;
    private BoxCollider boxCollider;

    private Vector3 scanOrigin;
    private Vector3 globalDownDir = Vector3.down;

    private bool changeDirection = false;
    private Vector3 waterLocation;
    private Vector3 enemyPoint;
    private bool goingToWater = false;
    private bool drinking;

    private bool onChase;


    Vector3[] fovDirections;

    //Todo - Stop State, Continue State
    private bool caughtOnDrinking;
    private bool onRun;
    private bool onHunt;
    private bool beingEaten;
    private bool eating;

    private Soil soil;

    private GameObject enemyGameObject;

    private float scaleMagnitude;

    private Animal mother;
    private Animal father;

    private void Awake()
    {
        if (!hasAncestor)
        {
            dNA = new DNA(properties.CommonSkinColor);
        }
        else
        {
            dNA = new DNA(properties.CommonSkinColor,
                (mother.dNA.skinIlluminance + father.dNA.skinIlluminance) / 2);
        }

    }

    private void Start()
    {
        changeDirectionCoroutine = StartCoroutine(ChangeDirection(properties.ChangeDirectionDelay));
        scaleMagnitude = transform.localScale.magnitude;
        soil = FindObjectOfType<Soil>();
        scanOrigin = transform.position;
        boxCollider = GetComponent<BoxCollider>();
        fovDirections = properties.Directions;

    }

    private void Update()
    {
        scanOrigin = transform.position;
        MakeGrounded();

        switch(state){
            case AnimalState.Wandering:

                break;
        }
        ScanArea();
        Wander();
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
        enemyGameObject.transform.localScale -= Vector3.one * properties.EatingSpeed * Time.deltaTime;
        if(enemyGameObject.transform.localScale.magnitude < 0.5f)
        {
            enemyGameObject.GetComponent<Animal>().Die();
            eating = false;
        }
    }

    private void ReEvaluateProperties()
    {
        
    }

    private void Wander()
    {
        if (drinking || eating || beingEaten) { return; }
        if (!changeDirection)
        {
            changeDirectionCoroutine =  StartCoroutine(ChangeDirection(properties.ChangeDirectionDelay));
        }
        transform.position += transform.forward * Time.deltaTime * properties.WanderingSpeed;
    }


    private void DrinkWater()
    {
        thirst -= properties.DrinkSpeed * Time.deltaTime;
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
        if( (int) dNA.sex == 1) //Female
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

        if(beingEaten || eating) { return; }

            //Scan For Enemies or Friends
            Collider[] scannedColliders = Physics.OverlapSphere(scanOrigin, properties.AwarenessRadius, animalPlantMask);
            foreach (Collider collider in scannedColliders)
            {
                if(collider.transform != transform)
                {
                    
                    if (collider.gameObject.CompareTag(tag) && dNA.isOppositeSex(collider.gameObject.GetComponent<Animal>().dNA.sex))
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
        if (!drinking && thirst >= properties.ThirstThreshold && !onChase)
        {
            //Search for Water
            for (int i = 0; i < fovDirections.Length; i++)
            {
                RaycastHit hit;
                Vector3 direction = fovDirections[i].normalized;
                direction = transform.TransformVector(direction);
                if (Physics.Raycast(scanOrigin, direction, out hit, maxLookDownForItemDistance, groundWaterMask))
                {
                    if (hit.transform.gameObject.layer == 4)
                    {
                        Debug.DrawLine(scanOrigin, hit.point, Color.green);
                        if (thirst >= properties.ThirstThreshold)
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

        if (Physics.Raycast(scanOrigin + transform.forward * 2, globalDownDir, out hitItem, maxLookDownForItemDistance, groundWaterMask))
        {
            if (hitItem.collider.gameObject.layer == 4)
            {
                //print("Hit water");
                if (thirst <= properties.ThirstThreshold || onChase)
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
        if (Physics.Raycast(scanOrigin, globalDownDir, out hit , maxLookDownDistance, groundMask))
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
            transform.position += Vector3.up * Time.deltaTime * properties.FallRiseSpeed;
            Debug.DrawRay(scanOrigin, globalDownDir, Color.red);
            return false;
        }
    }


    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(scanOrigin, properties.AwarenessRadius); //ScanArea
        }

        //Gizmos.DrawSphere((waterLocation + enemyPoint) / 2, 0.5f);
    }

}
