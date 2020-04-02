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
        GoingToSomething = 7,
        LookForDanger = 8
    }
    public AnimalState state { get; set; }

    public DNA dNA { get; set; }
    [SerializeField] private AnimalProperties properties;
    public AnimalProperties GetProperties() { return properties; }

    private const int maxLookDownDistance = 100;
    private const int maxLookDownForItemDistance = 20;
    private const int fallRiseSpeed = 20;
    private const int groundMask = 1 << 8;
    private const int animalPlantMask = (1 << 9) | (1 << 10);
    private const int animalMask = (1 << 9);
    private const int groundWaterMask = (1 << 4) | (1 << 8);

    //State (UI) Properties
    // Food   |xxxxx________| 1-FoodAmount, 2-FoodMaximum, 3-FoodDangerThreshold, 4-FoodEatingSpeed, 5-FoodDecreaseSpeed
    // Water  |xxxxxxxxxx___| 1-WaterAmount, 2-WaterMaximum, 3-WaterDangerThreshold, 4-WaterDrinkingSpeed, 5-WaterDecreaseSpeed
    // Sleep  |xxxxxxx______| 1-SleepAmount, 2-SleepMaximum, 3-SleepDangerThreshold, 4-SleepGettingSpeed, 5-SleepDecreaseSpeed
    // Stress |xx___________| 1-StressAmount, 2-StressMaximum, 3-StressDangerThreshold, 4-StressGettingSpeed, 5-StressDecreaseSpeed
    // Pregnancy |xxxxxxxxx___| 1-PregnancyAmount, 2-PregnancyMaximum, 3-PregnancySpeed

    public float foodAmount { get; set; }
    public float waterAmount { get; set; }
    public float sleepAmount { get; set; }
    public float stressAmount { get; set; }
    public float pregnancyAmount { get; set; }
    public float reproduceAmount { get; set; }

    public float age => FourthDimension.currentDay - dayOfBirth;

    private bool isHungry;
    private bool isThirsty;
    private bool isPregnant;
    Vector3[] fovDirections;

    public Food currentFood { get; set; }

    public bool hasAncestor { get; set; }
    private Animal partner;
    public int dayOfBirth { get; set; }
    public Animal mother { get; set; }
    public Animal father { get; set; }
    public List<Animal> childs { get; set; }
    public int reproId;

    private Coroutine changeDirectionCoroutine;
    private BoxCollider boxCollider;

    private Vector3 scanOrigin;

    public DNA.Sex sex;

    private bool wanderChangeDirection;
    public Location currentLocationInterest = new Location();
    private List<Location> locationHistory = new List<Location>();

    private Soil soil;
    private float scaleMagnitude;

    private GameObject enemyGameObject;

    public bool isRunning;
    private bool runChangeDirection;

    private void Awake()
    {
        if (!hasAncestor)
        {
            dNA = new DNA(properties.CommonSkinColor);
            GetComponent<Renderer>().material.color = dNA.skinColor;
            sex = dNA.sex;
        }
    }

    private void Start()
    {
        reproId = -1;
        dayOfBirth = FourthDimension.currentDay;
        foodAmount = 25;
        waterAmount = 25;
        sleepAmount = 50;
        changeDirectionCoroutine = StartCoroutine(WanderChangeDirection(properties.WanderChangeDirectionDelay));
        scaleMagnitude = transform.localScale.magnitude;
        soil = FindObjectOfType<Soil>();
        scanOrigin = transform.position;
        boxCollider = GetComponent<BoxCollider>();
        fovDirections = properties.LookForWaterdirections;
    }

    private void FixedUpdate()
    {
        MakeGrounded();
        if(state == AnimalState.Wandering)
        {
            ScanForOtherCreature();
        }
        else
        {
            ScanForEnemy();
        }

    }

    private void ScanForEnemy()
    {
        //Scan For Enemies or Friends
        Collider[] scannedColliders = Physics.OverlapSphere(scanOrigin, properties.AwarenessRadius, animalMask);
        foreach (Collider collider in scannedColliders)
        {
            if (collider.transform != transform)
            {
                if(collider.gameObject.GetComponent<Creature>() != null)
                {
                    if (!collider.CompareTag(tag) && collider.gameObject != enemyGameObject)
                    {
                        bool danger = PotentialEnemyDetected(collider);
                        if (danger) { break; }
                    }
                }
            }
        }
    }

    private void Update()
    {
        scanOrigin = transform.position;
        switch (state){
            case AnimalState.Wandering:
                Wander();
                ScanForWater();
                LookInFront();
                break;
            case AnimalState.Sleeping:
                Sleep();
                break;
            case AnimalState.Reproducing:
                break;
            case AnimalState.Eating:
                Eat();
                //MakeNoise();
                break;
            case AnimalState.Drinking:
                DrinkWater();
                break;
            case AnimalState.GoingToSomething:
                GoToSomething();
                LookInFarFront();
                JudgeDistance();
                break;
            case AnimalState.LookForDanger:
                //ScanForOtherCreature();
                StartCoroutine(GoWander());
                break;
            default:
                break;
        }
        //MakeGrounded();
        //MakeProperRotation();
        ReEvaluateProperties();
    }

    private IEnumerator GoWander()
    {
        yield return new WaitForSeconds(0.5f);
        if(state == AnimalState.LookForDanger)
        {
            state = AnimalState.Wandering;
        }
    }

    private void JudgeDistance()
    {

        if(Vector3.Distance(transform.position, currentLocationInterest.GetPosition()) <= 1.5f)
        {
            ArrivedToLocation();
        }
        else if( currentLocationInterest.GetLocationType() == Location.LocationType.Enemy && Vector3.Distance(transform.position, currentLocationInterest.GetPosition()) >= 25f){
            Lost();
        }
    }

    private void ArrivedToLocation()
    {
        switch (currentLocationInterest.GetLocationType())
        {
            case Location.LocationType.Water:
                state = AnimalState.Drinking;
                break;
            case Location.LocationType.Enemy:
                EnemyConfrontation();
                break;
            case Location.LocationType.Partner:
                PartnerConfrontation();
                break;
            case Location.LocationType.Food:
                FoodConfrontation();
                break;
            default:
                break;
        }
        currentLocationInterest = null;
        isRunning = false;
        //locationHistory.Add(currentLocationInterest);
    }

    private void FoodConfrontation()
    {
        currentFood = currentLocationInterest.GetObjectToFollow().gameObject.GetComponent<Food>();
        currentFood.GetComponent<Food>().beingEatenSpeed = properties.FoodEatingSpeed;
        currentFood.GetComponent<Food>().isBeingEaten = true;
        state = AnimalState.Eating;
    }

    private void PartnerConfrontation()
    {
        AnimalInteractionManager.instance.StartReproducing(this, partner, properties.ReproducingSpeed, properties.ReproducingMaximum);
        AnimalInteractionManager.instance.onReproducingFinished += OnReproductionFinished;
        state = AnimalState.Reproducing;
    }
    private void EnemyConfrontation()
    {
        isRunning = false;
        if (currentLocationInterest.isGettingAwayFrom)
        {
            Die();
        }
        else
        {
            enemyGameObject.GetComponent<Animal>().Die();
            FoodConfrontation();
            //state = AnimalState.Eating;
        }
    }

    private void OnReproductionFinished(int id, bool isSuccess)
    {
        if(id == reproId && isSuccess)
        {
            if(sex == DNA.Sex.Female)
            {
                isPregnant = true;
            }
        }
        reproId = -1;
        state = AnimalState.LookForDanger;
        AnimalInteractionManager.instance.onReproducingFinished -= OnReproductionFinished;
    }


    private void Lost()
    {
        isRunning = false;
        state = AnimalState.Wandering;
        //If enemySitutaion
        enemyGameObject = null;
    }

   

    private void GoToSomething()
    {
        if (!runChangeDirection)
        {
            Vector3 direction = currentLocationInterest.GetPosition() - transform.position;
            if (currentLocationInterest.isGettingAwayFrom) { direction = -direction; }
            transform.rotation = Quaternion.LookRotation(direction);
        }

        float speed = properties.WanderingSpeed;
        if (isRunning) { speed = properties.RunningSpeed; }
        transform.position += transform.forward * Time.deltaTime * speed;
    }


    private bool MakeGrounded()
    {
        RaycastHit hit;

        //if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1000,groundMask))
        //{
        //    var distanceToGround = hit.distance; 
        //    //use below code if your pivot point is in the middle
        //    transform.position = new Vector3(transform.position.x,hit.distance + boxCollider.bounds.extents.y*4,transform.position.z);
        //    //use below code if your pivot point is at the bottom
        //    //transform.position.y = hit.distance;
        //}

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
    }

    private void ReEvaluateProperties()
    {
        if(state != AnimalState.Drinking)
        {
            waterAmount -= Time.deltaTime;
            if(waterAmount < 0) { waterAmount = 0; }
        }
        if(waterAmount < properties.WaterDangerThreshold)
        {
            isThirsty = true;
        }
        else
        {
            isThirsty = false;
        }

        if (state != AnimalState.Eating)
        {
            foodAmount -= Time.deltaTime;
            if (foodAmount < 0) { foodAmount = 0; }
        }
        if (foodAmount < properties.FoodDangerThreshold)
        {
            isHungry = true;
        }
        else
        {
            isHungry = false;
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
        if (!wanderChangeDirection)
        {
            changeDirectionCoroutine =  StartCoroutine(WanderChangeDirection(properties.WanderChangeDirectionDelay));
        }
        transform.position += transform.forward * Time.deltaTime * properties.WanderingSpeed;
    }

    IEnumerator WanderChangeDirection(float seconds)
    {
        wanderChangeDirection = true;
        yield return new WaitForSeconds(seconds);
        if(state == AnimalState.Wandering)
        {
            TurnToRandomRotation();
        }
        wanderChangeDirection = false;
    }

    IEnumerator RunChangeDirection(Vector3 direction, float seconds)
    {
        runChangeDirection = true;
        RotationEvent(direction,0);
        yield return new WaitForSeconds(seconds);
        runChangeDirection = false;
    }

    private void TurnToRandomRotation()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(-180, 180), 0));
    }


    private void GiveBirth(Animal father)
    {
        for (int i = 0; i < properties.PregnancyChildAmount; i++)
        {
            GameObject child = Instantiate(properties.Animal, transform.position - transform.forward.normalized*2, Quaternion.identity);
            Animal childScript = child.GetComponent<Animal>();
            childScript.dNA = new DNA(properties.CommonSkinColor,
                    (dNA.skinIlluminance + father.dNA.skinIlluminance) / 2);
            child.GetComponent<Renderer>().material.color = childScript.dNA.skinColor;

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
                    //To-do -- Make nearest hit.point waterLocation

                    currentLocationInterest.SetLocation(hit.point, Location.LocationType.Water, false);
                    state = AnimalState.GoingToSomething;
                }
                else
                {
                    Debug.DrawRay(scanOrigin, direction * hit.distance, Color.black);
                }
            }
        }
    }

    private void DrinkWater()
    {
        waterAmount += properties.WaterDrinkingSpeed * Time.deltaTime;
        if (waterAmount > properties.WaterMaximum)
        {
            waterAmount = properties.WaterMaximum;
            RotationEvent(-transform.forward, 0);
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
                if(collider.gameObject.GetComponent<Creature>() != null)
                {
                    if (!collider.CompareTag(tag) && collider.gameObject != enemyGameObject)
                    {
                        bool danger = PotentialEnemyDetected(collider);
                        if(danger) { break; }
                    }
                    else if(enemyGameObject == null)
                    {
                        PotentialPartnerDetected(collider);
                    }
                }
                else if(isHungry && collider.gameObject.GetComponent<Food>() != null && state != AnimalState.Eating)
                {
                    PotentialFoodDetected(collider);
                }
            }
        }
    }

    private void PotentialFoodDetected(Collider collider)
    {
        state = AnimalState.GoingToSomething;
        isRunning = true;
        currentLocationInterest.SetObjectToFollow(collider.transform, Location.LocationType.Food, false);
    }

    private bool PotentialEnemyDetected(Collider collider)
    {

        int enemyFoodChainIndex = collider.gameObject.GetComponent<Creature>().foodChainIndex;
        if (enemyFoodChainIndex != foodChainIndex)
        {
            enemyGameObject = collider.gameObject;
            Interrupted();

            state = AnimalState.GoingToSomething;
            isRunning = true;
            if (enemyFoodChainIndex > foodChainIndex)
            {
                currentLocationInterest.SetObjectToFollow(collider.transform,Location.LocationType.Enemy,true);
            }
            else if (enemyFoodChainIndex < foodChainIndex)
            {
                currentLocationInterest.SetObjectToFollow(collider.transform, Location.LocationType.Enemy, false);
            }
            return true;
        }
        return false;
    }

    private void Interrupted()
    {
        AnimalInteractionManager.instance.Interrupted(this);
    }

    private void PotentialPartnerDetected(Collider collider)
    {
        Animal potentialPartner = collider.gameObject.GetComponent<Animal>();

        bool canInitiateReproducing = isEligiblePartner(potentialPartner);

        bool isPartnerEligible = potentialPartner.isEligiblePartner(this) && dNA.isOppositeSex(potentialPartner.dNA.sex);

        if (canInitiateReproducing && isPartnerEligible)
        {
            partner = potentialPartner;
            currentLocationInterest.SetObjectToFollow(collider.transform, Location.LocationType.Partner, false);
            state = AnimalState.GoingToSomething;
        }
    }

    public bool isEligiblePartner(Animal potentialPartner)
    {
        return !isPregnant && age >= properties.ReproducingAge && (state == AnimalState.Wandering || (state == AnimalState.GoingToSomething && potentialPartner == partner) );
    }

    private void Die()
    {
        Meat meat = gameObject.AddComponent<Meat>();
        meat.animalType = tag;
        Destroy(this);
    }

    //private void Die()
    //{
    //    soil.IncreaseDead(scaleMagnitude);
    //    Destroy(gameObject);
    //}

    private void Eat()
    {
        if(currentFood == null)
        {
            state = AnimalState.Wandering;
        }
        else
        {
            foodAmount += properties.FoodEatingSpeed * Time.deltaTime;
            if (foodAmount > properties.FoodMaximum)
            {
                foodAmount = properties.FoodMaximum;
                state = AnimalState.Wandering;
            }
        }
    }

    //private void Reproduce()
    //{
    //    //Process Reproduce
    //    if(partner.isPregnant || isPregnant)
    //    {
    //        state = AnimalState.Wandering;
    //        return;
    //    }
    //    reproduceAmount += properties.ReproducingSpeed * Time.deltaTime;
    //    print(reproduceAmount);
    //    if(reproduceAmount >= properties.ReproducingMaximum)
    //    {
    //        reproduceAmount = 0;
    //        state = AnimalState.Wandering;
    //        if ( dNA.sex == DNA.Sex.Female) //Female
    //        {
    //            isPregnant = true;
    //        }
    //    }
    //}

    

    private void LookInFarFront()
    {
        if (runChangeDirection) { return; }
        RaycastHit hit;
        RaycastHit hit1;
        RaycastHit hit2;
        float maxD = 0;
        if(Physics.Raycast(scanOrigin + transform.up * boxCollider.bounds.max.y, transform.TransformVector(new Vector3(0,-1,1)),out hit, maxLookDownForItemDistance, groundWaterMask))
        {
            if(hit.collider.gameObject.layer == 4 && currentLocationInterest.GetLocationType() != Location.LocationType.Water)
            {
                Physics.Raycast(scanOrigin + transform.up * boxCollider.bounds.max.y, transform.TransformVector(new Vector3(1, -1, 0)), out hit1, maxLookDownForItemDistance, groundWaterMask);
                if(hit1.collider.gameObject.layer == 4)
                {
                    maxD = -1;
                }
                else
                {
                    maxD = hit1.distance;
                }

                Physics.Raycast(scanOrigin + transform.up * boxCollider.bounds.max.y, transform.TransformVector(new Vector3(-1, -1, 0)), out hit2, maxLookDownForItemDistance, groundWaterMask);
                if(hit2.collider.gameObject.layer == 4)
                {
                    if(maxD == -1)
                    {
                        StartCoroutine(RunChangeDirection(-transform.forward, 0.5f));
                    }
                    else
                    {
                        StartCoroutine(RunChangeDirection(transform.right, 0.5f));
                    }
                }
                else
                {
                    if(maxD == -1)
                    {
                        StartCoroutine(RunChangeDirection(-transform.right, 0.5f));
                    }
                    else if(hit2.distance > maxD)
                    {
                        StartCoroutine(RunChangeDirection(-transform.right, 0.5f));
                    }
                    else
                    {
                        StartCoroutine(RunChangeDirection(transform.right, 0.5f));
                    }
                }
                Debug.DrawLine(scanOrigin + transform.up * boxCollider.bounds.max.y, hit.point, Color.green);
            }
            else
            {
                Debug.DrawLine(scanOrigin + transform.up * boxCollider.bounds.max.y, hit.point, Color.black);
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
                if (isRunning)
                {
                    Debug.DrawLine(scanOrigin + transform.forward * 2, hitItem.point, Color.blue);
                    if (!runChangeDirection)
                    {
                        Vector3 angleBisectorVector = (hitItem.point - transform.position) + (enemyGameObject.transform.position - transform.position);
                        StartCoroutine(RunChangeDirection(angleBisectorVector, 1f));
                    }

                    //TurnToRandomRotation();
                }
                else if (isThirsty)
                {
                    state = AnimalState.Drinking;
                }
                else
                {
                    TurnToRandomRotation();
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

    private void OnDestroy()
    {
        
    }

}

[Serializable]
public class Location
{
    public enum LocationType { Default = 0, Water = 1, Enemy = 2, Partner = 3, Food = 4};
    private LocationType locationType;
    private Vector3 location;

    private Transform objectToFollow;
    public bool isGettingAwayFrom;

    public Location()
    {
        objectToFollow = null;
        location = Vector3.zero;
        locationType = LocationType.Default;
    }

    public Vector3 GetPosition()
    {
        if (objectToFollow)
        {
            return objectToFollow.position;
        }
        else
        {
            return location;
        }
    }

    public void SetLocation(Vector3 location, LocationType locationType, bool isGettingAwayFrom)
    {
        this.location = location;
        this.locationType = locationType;
        this.isGettingAwayFrom = isGettingAwayFrom;
        objectToFollow = null;
    }

    public void SetObjectToFollow(Transform objectToFollow, LocationType locationType, bool isGettingAwayFrom)
    {
        this.objectToFollow = objectToFollow;
        this.locationType = locationType;
        this.isGettingAwayFrom = isGettingAwayFrom;
        location = Vector3.zero;
    }
    public Transform GetObjectToFollow()
    {
        return objectToFollow;
    }
    public Vector3 GetLocation()
    {
        return location;
    }
    public LocationType GetLocationType()
    {
        return locationType;
    }

}