using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : Creature
{
    public enum AnimalState {
        Wandering = 0, //Wander, ScanForWater,ScanForFood,ScanForEnemy,ScanForPartner
        Sleeping = 1, //Sleep
        Running = 2, //ScanForOtherCreature,LookInFront,Run
        Drinking = 3, //ScanForOtherCreature, Drink
        Eating = 4, //ScanForOtherCreature, MakeNoise(maybe),Eat
        Reproducing = 5, //ScanForOtherCreature, Reproduce
        BeingEaten = 6, //BeEaten
    }
    public AnimalState state { get; set; }

    public DNA dNA { get; set; }
    [SerializeField] private AnimalProperties properties;

    private const int maxLookDownDistance = 100;
    private const int maxLookDownForItemDistance = 20;
    private const int fallRiseSpeed = 20;
    private const int groundMask = 1 << 8;
    private const int animalPlantMask = (1 << 9) | (1 << 10);
    private const int groundWaterMask = (1 << 4) | (1 << 8);
    private Vector3 defaultLook;

    //State (UI) Properties
    // Food   |xxxxx________| 1-FoodAmount, 2-FoodMaximum, 3-FoodDangerThreshold, 4-FoodEatingSpeed, 5-FoodDecreaseSpeed
    // Water  |xxxxxxxxxx___| 1-WaterAmount, 2-WaterMaximum, 3-WaterDangerThreshold, 4-WaterDrinkingSpeed, 5-WaterDecreaseSpeed
    // Sleep  |xxxxxxx______| 1-SleepAmount, 2-SleepMaximum, 3-SleepDangerThreshold, 4-SleepGettingSpeed, 5-SleepDecreaseSpeed
    // Stress |xx___________| 1-StressAmount, 2-StressMaximum, 3-StressDangerThreshold, 4-StressGettingSpeed, 5-StressDecreaseSpeed
    // Pregnancy |xxxxxxxxx___| 1-PregnancyAmount, 2-PregnancyMaximum, 3-PregnancySpeed

    private float foodAmount;
    private float waterAmount;
    private float sleepAmount;
    private float stressAmount;
    private float pregnancyAmount;
    private float reproduceAmount;

    private bool isHungry;
    private bool isThirsty;
    private bool isPregnant;
    Vector3[] fovDirections;


    public bool hasAncestor { get; set; }
    private Animal partner;


    public Animal mother { get; set; }
    public Animal father { get; set; }
    public List<Animal> childs { get; set; }

    private Coroutine changeDirectionCoroutine;
    private BoxCollider boxCollider;

    private Vector3 scanOrigin;


    private bool changeDirection;
    private GameObject enemyGameObject;
    private Vector3 waterLocation;
    private Vector3 enemyPoint;

    private Soil soil;
    private float scaleMagnitude;

    //Todo - Stop State, Continue State
    private bool onRun;
    private bool onHunt;
    private bool goingToWater;
    private bool goingToPartner;
    private Vector3 rotationEventDir;
    private bool rotationEvent;

    private void Awake()
    {
        if (!hasAncestor)
        {
            dNA = new DNA(properties.CommonSkinColor);
        }
        //else
        //{
        //    dNA = new DNA(properties.CommonSkinColor,
        //        (mother.dNA.skinIlluminance + father.dNA.skinIlluminance) / 2);
        //}

    }

    private void Start()
    {
        changeDirectionCoroutine = StartCoroutine(ChangeDirection(properties.WanderChangeDirectionDelay));
        scaleMagnitude = transform.localScale.magnitude;
        soil = FindObjectOfType<Soil>();
        scanOrigin = transform.position;
        boxCollider = GetComponent<BoxCollider>();
        fovDirections = properties.LookForWaterdirections;
    }

    private void Update()
    {
        scanOrigin = transform.position;

        switch(state){
            case AnimalState.Wandering:
                Wander();
                ScanForWater();
                ScanForOtherCreature();
                LookInFront();
                break;
            case AnimalState.Sleeping:
                Sleep();
                break;
            case AnimalState.Running:
                ScanForOtherCreature();
                Run();
                LookInFront();
                break;
            case AnimalState.Reproducing:
                ScanForOtherCreature();
                Reproduce();
                break;
            case AnimalState.Eating:
                ScanForOtherCreature();
                Eat();
                //MakeNoise();
                break;
            case AnimalState.Drinking:
                ScanForOtherCreature();
                DrinkWater();
                break;
            case AnimalState.BeingEaten:
                BeEaten();
                break;
            default:
                break;
        }
        MakeGrounded();
        //MakeProperRotation();
        ReEvaluateProperties();
        //ScanArea();
        //Wander();
        //ReEvaluateProperties();
        //if (eating)
        //{
        //    Eat();
        //}
        //if (drinking)
        //{
        //    DrinkWater();
        //}
        
    }

    public bool MakeGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(scanOrigin, Vector3.down, out hit, maxLookDownDistance, groundMask))
        {
            if (hit.distance <= boxCollider.bounds.extents.y + 0.25f && hit.distance >= boxCollider.bounds.extents.y)
            {
                Debug.DrawLine(scanOrigin, hit.point, Color.green);
                return true;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, hit.point + Vector3.up / 2, Time.deltaTime * 10);
                return false;
            }
        }
        else
        {
            transform.position += Vector3.up * Time.deltaTime * fallRiseSpeed;
            Debug.DrawRay(scanOrigin, Vector3.down, Color.red);
            return false;
        }
    }

    private void MakeProperRotation()
    {
        RaycastHit hit;
        if(rotationEvent)
        {
            transform.rotation = Quaternion.LookRotation(rotationEventDir, Vector3.up);
            rotationEvent = false;
        }
        if(Physics.Raycast(scanOrigin, Vector3.down, out hit, maxLookDownForItemDistance, groundMask))
        {
            //transform.up = hit.normal;
        }
    }

    private void RotationEvent(Vector3 rotation, int rotationType)
    {
        if(rotationType == 0)
        {
            transform.rotation = Quaternion.LookRotation(rotation);
        }
        else if(rotationType == 1)
        {
            transform.rotation = Quaternion.Euler(rotation);
        }
        //rotationEventDir = rotation;
        //rotationEvent = true;
    }

    private void ReEvaluateProperties()
    {
        if(state != AnimalState.Drinking)
        {
            waterAmount -= Time.deltaTime;
            if(waterAmount < 0) { waterAmount = 0; }
        }
        if(!isThirsty && waterAmount < properties.WaterDangerThreshold)
        {
            isThirsty = true;
        }

        if (state != AnimalState.Eating)
        {
            foodAmount -= Time.deltaTime;
            if (foodAmount < 0) { foodAmount = 0; }
        }
        if (!isHungry && foodAmount < properties.FoodDangerThreshold)
        {
            isHungry = true;
        }

        if (isPregnant)
        {
            pregnancyAmount += properties.PregnancySpeed * Time.deltaTime;
            if(state == AnimalState.Wandering && pregnancyAmount >= properties.PregnancyMaximum)
            {
                isPregnant = false;
                pregnancyAmount = 0;
                GiveBirth(partner);
            }
        }
        if(state != AnimalState.Sleeping)
        {
            sleepAmount -= Time.deltaTime;
            if(state == AnimalState.Wandering && sleepAmount < properties.SleepDangerThreshold)
            {
                if(sleepAmount <= 0) { sleepAmount = 0; }
                state = AnimalState.Sleeping;
            }
        }
    }

    private void Sleep()
    {
        sleepAmount += properties.SleepGettingSpeed * Time.deltaTime;
        if(sleepAmount >= properties.SleepMaximum)
        {
            sleepAmount = properties.SleepMaximum;
            state = AnimalState.Wandering;
        }
    }

    private void Wander()
    {
        if (!changeDirection)
        {
            changeDirectionCoroutine =  StartCoroutine(ChangeDirection(properties.WanderChangeDirectionDelay));
        }
        transform.position += transform.forward * Time.deltaTime * properties.WanderingSpeed;
    }

    private void Run()
    {
        transform.position += transform.forward * Time.deltaTime * properties.RunningSpeed;
    }

    IEnumerator ChangeDirection(float seconds)
    {
        changeDirection = true;
        yield return new WaitForSeconds(seconds);
        if(state == AnimalState.Wandering)
        {
            RotationEvent(new Vector3(0, UnityEngine.Random.Range(-180, 180), 0), 1);
        }
        changeDirection = false;
    }


    private void GiveBirth(Animal father)
    {
        for (int i = 0; i < properties.PregnancyChildAmount; i++)
        {
            GameObject child = Instantiate(properties.Animal, -transform.forward, Quaternion.identity);
            Animal childScript = child.GetComponent<Animal>();
            childScript.dNA = new DNA(properties.CommonSkinColor,
                    (dNA.skinIlluminance + father.dNA.skinIlluminance) / 2);

            childScript.mother = this;
            childScript.father = partner;
            childScript.hasAncestor = true;

            childs.Add(childScript);
            father.childs.Add(childScript);

        }
    }

    private void ScanForWater()
    {
        if(!isThirsty) { return; }
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
                    //print(hit.textureCoord);
                    waterLocation = hit.point;
                    //RotationEvent(waterLocation - transform.position);
                    goingToWater = true;
                }
                else
                {
                    Debug.DrawRay(scanOrigin, direction * hit.distance, Color.black);
                }
            }
        }

        if (goingToWater)
        {
            if (Vector3.Distance(transform.position, waterLocation) <= 2f)
            {
                goingToWater = false;
                state = AnimalState.Drinking;
            }
        }
    }

    private void DrinkWater()
    {
        waterAmount += properties.WaterDrinkingSpeed * Time.deltaTime;
        if (waterAmount > properties.WaterMaximum)
        {
            waterAmount = properties.WaterMaximum;
            state = AnimalState.Wandering;
        }
    }


    private void ScanForOtherCreature()
    {
        //Scan For Enemies or Friends
        Collider[] scannedColliders = Physics.OverlapSphere(scanOrigin, properties.AwarenessRadius, animalPlantMask);
        foreach (Collider collider in scannedColliders)
        {
            if (collider.transform != transform)
            {
                if (!collider.CompareTag(tag))
                {
                    enemyGameObject = collider.gameObject;
                    byte enemyFoodChainIndex = enemyGameObject.GetComponent<Creature>().foodChainIndex;
                    if (enemyFoodChainIndex != foodChainIndex)
                    {
                        enemyPoint = collider.ClosestPoint(transform.position);

                        if (enemyFoodChainIndex > foodChainIndex)
                        {
                            state = AnimalState.Running;
                            onRun = true;
                            //RotationEvent(transform.position - enemyPoint); //RunFrom
                            break;
                        }
                        else if (enemyFoodChainIndex < foodChainIndex && isHungry)
                        {
                            state = AnimalState.Running;
                            onHunt = true;
                            //RotationEvent(enemyPoint - transform.position); //RunTo
                            break;
                        }
                    }
                }
                else if(state == AnimalState.Wandering && collider.gameObject.CompareTag(tag)) 
                {
                    Animal potentialPartner = collider.gameObject.GetComponent<Animal>();
                    if(potentialPartner.state == AnimalState.Wandering && dNA.isOppositeSex(potentialPartner.dNA.sex))
                    {
                        partner = potentialPartner;
                        goingToPartner = true;
                        //RotationEvent(partner.transform.position - transform.position);
                    }
                }
            }
        }

        if (state == AnimalState.Running)
        {
            if (Vector3.Distance(transform.position, enemyPoint) <= 1f)
            {
                if (onRun)
                {
                    onRun = false;
                    state = AnimalState.BeingEaten;
                }
                else if (onHunt)
                {
                    onHunt = false;
                    state = AnimalState.Eating;
                }
            }
            else if (Vector3.Distance(transform.position, enemyPoint) >= 20f)
            {
                onRun = false;
                onHunt = false;
                state = AnimalState.Wandering;
            }
        }
        else if (goingToPartner)
        {
            if(Vector3.Distance(transform.position,partner.transform.position) <= 1f)
            {
                goingToPartner = false;
                state = AnimalState.Reproducing;
            }
        }
    }


    private void BeEaten()
    {
        if(transform.localScale.magnitude < 0.5f)
        {
            Die();
        }
    }

    private void Die()
    {
        soil.IncreaseDead(scaleMagnitude);
        Destroy(gameObject);
    }

    private void Eat()
    {
        if(enemyGameObject == null)
        {
            state = AnimalState.Wandering;
        }
        else
        {
            enemyGameObject.transform.localScale -= Vector3.one * properties.FoodEatingSpeed * Time.deltaTime;
            foodAmount += properties.FoodEatingSpeed * Time.deltaTime;
            if (foodAmount > properties.FoodMaximum) { foodAmount = properties.FoodMaximum; }
        }
    }

    private void Reproduce()
    {
        //Process Reproduce
        
        reproduceAmount += properties.ReproducingSpeed * Time.deltaTime;
        if(reproduceAmount >= properties.ReproducingMaximum)
        {
            reproduceAmount = 0;
            state = AnimalState.Wandering;
            if ( dNA.sex == DNA.Sex.Female) //Female
            {
                isPregnant = true;
            }
        }

    }

    private void LookInFront()
    {
        RaycastHit hitItem;

        if (Physics.Raycast(scanOrigin + transform.forward * 2, Vector3.down, out hitItem, maxLookDownForItemDistance, groundWaterMask))
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, hitItem.normal) * transform.rotation;
            if (hitItem.collider.gameObject.layer == 4)
            {
                //print("Hit water");
                if (!isThirsty || state == AnimalState.Running)
                {
                    Debug.DrawLine(scanOrigin + transform.forward * 2, hitItem.point, Color.blue);
                    //RotationEvent(new Vector3(transform.eulerAngles.x, UnityEngine.Random.Range(-180, 180), transform.eulerAngles.z));
                }
            }
            else
            {
                Debug.DrawLine(scanOrigin + transform.forward * 2, hitItem.point, Color.magenta);
                //print("Hit Ground");
            }
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
