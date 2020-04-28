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
    private Collider boxCollider;

    private Location target = null;

    private bool isRotated;
    private bool isRunning;

    private Vector3 scanOrigin;
    private bool isWanderRotated;

    private float hitRotateDelay = 0.1f / FourthDimension.tSM;

    private void Awake()
    {
        animal = GetComponent<Animal>();
        properties = animal.GetProperties();
        boxCollider = GetComponent<Collider>();
    }

    private void Start()
    {
        GetComponent<ScanController>().onTargetFound += SetTarget;
    }

    private void FixedUpdate()
    {
        scanOrigin = transform.position;
        MakeGrounded();
        switch (animal.state)
        {
            case AnimalState.Wandering:
            case AnimalState.GoingToSomething:
                if (LookInFarFront())
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
    }

    private void SetTarget(Location target)
    {
        this.target = target;
        if (target.GetLocationType() == LocationType.Enemy)
            isRunning = true;
    }

    private void GoToSomething()
    {
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
            if (hit.distance <= boxCollider.bounds.extents.y + 0.25f && hit.distance >= boxCollider.bounds.extents.y)
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
        if (!isWanderRotated)
        {
            StartCoroutine(WanderChangeDirection(properties.WanderChangeDirectionDelay));
        }
        transform.position += transform.forward * Time.deltaTime * properties.WanderingSpeed;
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

        if (Physics.Raycast(scanOrigin + transform.forward * 2, Vector3.down, out hit, GroundingProp.maxLookDownForItemDistance, Masks.groundWaterObstacleTreeMask))
        {
            if (hit.collider.gameObject.layer == 4) // Is Water
            {
                if (target != null && target.GetLocationType() == LocationType.Water)
                {
                    //Debug.DrawLine(scanOrigin + transform.forward * 2, hit.point, Color.blue);
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
                Debug.DrawLine(scanOrigin + transform.forward * 2, hit.point, Color.magenta);
                RotationEvent(-transform.forward, 0);
            }
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
        if (Physics.Raycast(scanOrigin + transform.up , transform.TransformVector(new Vector3(0, -1, 1)), out hit, GroundingProp.maxLookDownForItemDistance, Masks.groundWaterObstacleTreeMask))
        {
            if ((hit.collider.gameObject.layer == 4 && target != null && target.GetLocationType() != LocationType.Water) || hit.collider.gameObject.layer == 11 || hit.collider.gameObject.layer == 12)
            {
                if(Physics.Raycast(scanOrigin + transform.up , transform.TransformVector(new Vector3(1, -1, 0)), out hit1, GroundingProp.maxLookDownForItemDistance, Masks.groundWaterObstacleTreeMask))
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

                if(Physics.Raycast(scanOrigin + transform.up, transform.TransformVector(new Vector3(-1, -1, 0)), out hit2, GroundingProp.maxLookDownForItemDistance, Masks.groundWaterObstacleTreeMask))
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
                //Debug.DrawLine(scanOrigin + transform.up , hit.point, Color.green);
                return true;
            }
            else
            {
                //Debug.DrawLine(scanOrigin + transform.up , hit.point, Color.black);
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
