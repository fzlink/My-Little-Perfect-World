using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class GroundingProp
{
    public const int fallRiseSpeed = 20;
    public const int maxLookDownDistance = 100;
    public const int maxLookDownForItemDistance = 20;
}

public class MoveController : MonoBehaviour
{
    private Animal animal;
    private AnimalProperties properties;
    private Collider collider;

    private Location target = null;
    private List<Location> targetHistory;

    private bool isRotated;
    public bool isRunning { get; set; }

    private Vector3 scanOrigin;
    private bool isWanderRotated;

    private float hitRotateDelay = 0.1f / FourthDimension.tSM;

    private bool followBias;
    int chunkCoordInc;

    private void Awake()
    {
        animal = GetComponent<Animal>();
        properties = animal.GetProperties();
        collider = GetComponent<Collider>();
    }

    private void Start()
    {
        chunkCoordInc = MapEventManager.chunkCoordInc;
        targetHistory = new List<Location>();
        GetComponent<ScanController>().onTargetFound += SetTarget;
    }

    private void FixedUpdate()
    {
        scanOrigin = transform.position;
        MakeGrounded();
        switch (animal.state)
        {
            case AnimalState.Wandering:
                if (LookInFarFront())
                    break;
                if (LookInFront())
                    break;
                break;
            case AnimalState.GoingToSomething:
                if (target.isGettingAwayFrom && LookInFarFront())
                    break;
                if (LookInFront())
                    break;
                break;
        }
    }

    private void Update()
    {
        switch (animal.state)
        {
            case AnimalState.Wandering:
                Wander();
                break;
            case AnimalState.GoingToSomething:
                if(target != null)
                {
                    GoToSomething();
                    JudgeDistance();
                }
                else
                {
                    animal.state = AnimalState.Wandering;
                }
                break;
        }
        ClampMovement();
    }

    private void ClampMovement()
    {
        Vector3 clampedPos = transform.position;
        float mapXClampVal = (MapEventManager.instance.xOffset * chunkCoordInc + (chunkCoordInc / 2)-3);
        float mapYClampVal = (MapEventManager.instance.yOffset * chunkCoordInc + (chunkCoordInc / 2)-3);
        clampedPos.x = Mathf.Clamp(clampedPos.x, -mapXClampVal, mapXClampVal);
        clampedPos.z = Mathf.Clamp(clampedPos.z, -mapYClampVal, mapYClampVal);
        transform.position = clampedPos;
    }

    private void SetTarget(Location target)
    {
        this.target = target;
        if (target.GetLocationType() == LocationType.Enemy)
        {
            isRunning = true;
            if (animal.runManager == null && animal.foodChainIndex > target.GetEnemyFoodChainIndex())
            {
                AnimalInteractionManager.instance.StartRunning(animal, target.GetObjectToFollow());
            }
        }

    }

    private void GoToSomething()
    {
        followBias = false;
        if (!isRotated)
        {
            Vector3 direction = target.GetPosition() - transform.position;
            if (target.isGettingAwayFrom) { direction = -direction; }
            transform.rotation = Quaternion.LookRotation(direction);
        }

        float speed = properties.WanderingSpeed;
        if (isRunning) { speed = properties.RunningSpeed; }
        transform.position += transform.forward * Time.deltaTime * speed;
    }

    private void JudgeDistance()
    {
        if (Vector3.Distance(transform.position, target.GetPosition()) <= 1.5f)
        {
            ArrivedToTarget();
        }
        else if (Vector3.Distance(transform.position, target.GetPosition()) >= 25f)
        {
            Lost();
        }
    }

    private void ArrivedToTarget()
    {
        switch (target.GetLocationType())
        {
            case LocationType.Water:
                animal.WaterConfrontation();
                break;
            case LocationType.Enemy:
                animal.EnemyConfrontation();
                isRunning = false;
                break;
            case LocationType.Partner:
                animal.PartnerConfrontation();
                break;
            case LocationType.Food:
                animal.FoodConfrontation();
                break;
            case LocationType.Plant:
                animal.PlantConfrontation();
                break;
            default:
                break;
        }
        targetHistory.Add(target);
        target = null;
    }



    private void Lost()
    {
        if (target.GetLocationType() == LocationType.Enemy)
        {
            animal.EnemyLost();
            isRunning = false;
        }
        target = null;
    }

    private bool MakeGrounded()
    {
        RaycastHit hit;

        if (Physics.Raycast(scanOrigin, Vector3.down, out hit, GroundingProp.maxLookDownDistance, Masks.groundMask))
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            if (hit.distance <= collider.bounds.extents.y + 0.25f && hit.distance >= collider.bounds.extents.y)
            {
                //Debug.DrawLine(scanOrigin, hit.point, Color.green);
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
            transform.position += Vector3.up * Time.deltaTime * GroundingProp.fallRiseSpeed;
            //Debug.DrawRay(scanOrigin, Vector3.down, Color.red);
            return false;
        }
    }

    public void RotationEvent(Vector3 rotation, int rotationType)
    {
        if (rotationType == 0)
        {
            transform.rotation = Quaternion.LookRotation(rotation);
        }
        else if (rotationType == 1)
        {
            transform.rotation = Quaternion.Euler(rotation);
        }
    }

    private void Wander()
    {
        if (ShouldBiasRotation() && !isWanderRotated &&  UnityEngine.Random.value < .05f)
        {
            Location biasLocation = GetBiasLocation(GetBiasType());
            if (biasLocation != null )
            {
                Vector3 direction = biasLocation.GetPosition() - transform.position;
                //direction.x = 0;
                if (biasLocation.isGettingAwayFrom) { direction = -direction; }
                transform.rotation = Quaternion.LookRotation(direction);
                followBias = true;
            }
        }
        else if (!isWanderRotated && !followBias)
        {
            StartCoroutine(WanderChangeDirection(properties.WanderChangeDirectionDelay));
        }
        transform.position += transform.forward * Time.deltaTime * properties.WanderingSpeed;
    }

    private Location GetBiasLocation(LocationType locationType)
    {
        if(locationType == LocationType.Water)
        {
            return FindLastWaterLocation();
        }
        return null;
    }

    private Location FindLastWaterLocation()
    {
        for (int i = targetHistory.Count - 1; i >= 0; i--)
        {
            if (targetHistory[i].GetLocationType() == LocationType.Water)
                return targetHistory[i];
        }
        return null;
    }

    private LocationType GetBiasType()
    {
        if (animal.isThirsty)
        {
            return LocationType.Water;
        }
        else
        {
            return LocationType.Default;
        }
    }

    private bool ShouldBiasRotation()
    {
        if (!followBias)
        {
            if (animal.isThirsty)
                return true;
            else
                return false;
        }
        else
        {
            return false;
        }

    }

    IEnumerator WanderChangeDirection(float seconds)
    {
        isWanderRotated = true;
        yield return new WaitForSeconds(seconds);
        if (animal.state == AnimalState.Wandering)
        {
            TurnToRandomRotation();
        }
        isWanderRotated = false;
    }

    private void TurnToRandomRotation()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, UnityEngine.Random.Range(-180, 180), 0));
    }

    private bool LookInFront()
    {
        RaycastHit hit;
        Vector3 scanPoint = scanOrigin + (transform.forward * collider.bounds.extents.z + transform.up*10);
        Vector3 direction = -transform.up;
        if (Physics.Raycast(scanPoint, direction, out hit, GroundingProp.maxLookDownForItemDistance, Masks.groundWaterObstacleTreeMask))
        {
            if (hit.collider.gameObject.layer == 4) // Is Water
            {
                if (target != null && target.GetLocationType() == LocationType.Water)
                {
                    Debug.DrawRay(scanPoint, direction * GroundingProp.maxLookDownForItemDistance, Color.blue);
                    animal.WaterConfrontation();
                    target = null;
                    //Vector3 angleBisectorVector = (hitItem.point - transform.position) + (enemyGameObject.transform.position - transform.position);
                    //StartCoroutine(RunChangeDirection(angleBisectorVector, 1f));
                }
                else
                {
                    RotationEvent(-transform.forward, 0);
                }
            }
            else if(hit.collider.gameObject.layer == 11 || hit.collider.gameObject.layer == 12)
            {
                Debug.DrawRay(scanPoint, direction * GroundingProp.maxLookDownForItemDistance, Color.magenta);
                RotationEvent(-transform.forward, 0);
            }
            Debug.DrawRay(scanPoint, direction * GroundingProp.maxLookDownForItemDistance, Color.black);
            return true;
        }
        return false;
    }

    private bool LookInFarFront()
    {
        if (isRotated) { return true; }
        RaycastHit hit;
        RaycastHit hit1;
        RaycastHit hit2;
        float maxD = 0;
        Vector3 direction = transform.TransformDirection(new Vector3(0, -1, 1));
        Vector3 scanPoint = scanOrigin + (transform.up * collider.bounds.extents.y);
        if (Physics.Raycast(scanPoint, direction, out hit, GroundingProp.maxLookDownForItemDistance, Masks.groundWaterObstacleTreeMask))
        {
            if ((hit.collider.gameObject.layer == 4 && target != null && target.GetLocationType() != LocationType.Water) || hit.collider.gameObject.layer == 11 || hit.collider.gameObject.layer == 12)
            {
                if(Physics.Raycast(scanPoint, direction, out hit1, GroundingProp.maxLookDownForItemDistance, Masks.groundWaterObstacleTreeMask))
                {
                    if (hit1.collider.gameObject.layer != 8)
                    {
                        maxD = -1;
                    }
                    else
                    {
                        maxD = hit1.distance;
                    }
                }

                if(Physics.Raycast(scanPoint, direction, out hit2, GroundingProp.maxLookDownForItemDistance, Masks.groundWaterObstacleTreeMask))
                {
                    if (hit2.collider.gameObject.layer != 8)
                    {
                        if (maxD == -1)
                        {
                            StartCoroutine(RunChangeDirection(-transform.forward, hitRotateDelay));
                        }
                        else
                        {
                            StartCoroutine(RunChangeDirection(transform.right, hitRotateDelay));
                        }
                    }
                    else
                    {
                        if (maxD == -1)
                        {
                            StartCoroutine(RunChangeDirection(-transform.right, hitRotateDelay));
                        }
                        else if (hit2.distance > maxD)
                        {
                            StartCoroutine(RunChangeDirection(-transform.right, hitRotateDelay));
                        }
                        else
                        {
                            StartCoroutine(RunChangeDirection(transform.right, hitRotateDelay));
                        }
                    }
                }
                //Debug.DrawLine(scanPoint , hit.point, Color.green);
                return true;
            }
            else
            {
                //Debug.DrawLine(scanPoint , hit.point, Color.black);
                return false;
            }
        }
        return false;
    }

    IEnumerator RunChangeDirection(Vector3 direction, float seconds)
    {
        isRotated = true;
        RotationEvent(direction, 0);
        yield return new WaitForSeconds(seconds);
        isRotated = false;
    }

    private void OnDestroy()
    {
        GetComponent<ScanController>().onTargetFound -= SetTarget;
    }

}
