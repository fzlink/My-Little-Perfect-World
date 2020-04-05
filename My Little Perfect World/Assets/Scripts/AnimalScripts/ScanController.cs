using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Masks
{
    public const int waterMask = (1 << 4);
    public const int groundMask = (1 << 8);
    public const int groundWaterMask = (1 << 4) | (1 << 8);

    public const int animalMask = (1 << 9);
    public const int plantMask = (1 << 10);
    public const int animalPlantMask = (1 << 9) | (1 << 10);

    public const int obstacleMask = (1 << 11);
    public const int waterObstacleMask = (1 << 4) | (1 << 11);

    public const int treeMask = (1 << 12);
    public const int groundWaterObstacleTreeMask = (1 << 8) | (1 << 4) | (1 << 11) | (1 << 12);
}


public class ScanController : MonoBehaviour
{
    private Animal animal;
    private AnimalProperties properties;
    private BoxCollider boxCollider;
    private Vector3 scanOrigin;
    private Vector3[] fovDirections;
    private Location target = new Location();
    private Collider[] scannedColliders;


    public event Action<Location> onTargetFound; 

    private void Awake()
    {
        animal = GetComponent<Animal>();
        boxCollider = GetComponent<BoxCollider>();
        properties = animal.GetProperties();
        fovDirections = properties.LookForWaterdirections;
    }

    private void FixedUpdate()
    {
        scanOrigin = transform.position;
        scannedColliders = Physics.OverlapSphere(scanOrigin, properties.AwarenessRadius, Masks.animalPlantMask);
        switch (animal.state)
        {
            case AnimalState.Wandering:
                if (SearchEnemies(scannedColliders.Where(c => c.GetComponent<Animal>() != null && c.GetComponent<Animal>().foodChainIndex > animal.foodChainIndex && !c.CompareTag(tag)).ToList()))
                    break;
                if (animal.isHungry)
                {
                    if (animal.isCarnivore)
                    {
                        if (SearchEnemies(scannedColliders.Where(c => c.GetComponent<Animal>() != null && !c.CompareTag(tag)).ToList()))
                            break;
                        if (SearchFood(scannedColliders.Where(c => c.GetComponent<Meat>() != null).ToList()))
                            break;
                    }
                    if (animal.isHerbivore)
                    {
                        if (SearchPlant(scannedColliders.Where(c => c.GetComponent<Plant>() != null).ToList()))
                            break;
                        if (SearchFood(scannedColliders.Where(c => c.GetComponent<Vegetation>() != null).ToList()))
                            break;
                    }
                }

                if (animal.isThirsty)
                    if (SearchWater())
                        break;
                if (SearchPartners(scannedColliders.Where(c => c.GetComponent<Animal>() != null && c.CompareTag(tag)).ToList()))
                    break;
                break;

            case AnimalState.Drinking:
            case AnimalState.Eating:
            case AnimalState.Reproducing:
            case AnimalState.GoingToSomething:
                if (SearchEnemies(scannedColliders.Where(c => c.gameObject.GetComponent<Animal>() != null && c.GetComponent<Animal>().foodChainIndex > animal.foodChainIndex && !c.CompareTag(tag)).ToList()))
                    break;
                break;
        }
    }

    private IOrderedEnumerable<Collider> SortCollidersForDistance(List<Collider> animalColliders)
    {
        return animalColliders.OrderBy(c => (c.transform.position - transform.position).sqrMagnitude);
    }

    private void AssignTarget(Transform objectToFollow, LocationType locationType, bool isGettingAwayFrom)
    {
        target.SetObjectToFollow(objectToFollow, locationType, isGettingAwayFrom);
        if(onTargetFound != null)
        {
            onTargetFound(target);
        }
    }
    private void AssignTarget(Vector3 location, LocationType locationType, bool isGettingAwayFrom)
    {

        target.SetLocation(location, locationType, isGettingAwayFrom);
        if(onTargetFound != null)
        {
            onTargetFound(target);
        }
    }


    private bool SearchEnemies(List<Collider> animalColliders) 
    {
        Collider minDstHuntCollider = null;
        if (animalColliders.Count < 1) return false;
        IOrderedEnumerable<Collider> sortedAnimalColliders = SortCollidersForDistance(animalColliders);
        foreach(Collider collider in sortedAnimalColliders)
        {
            if(collider.transform != transform)
            {
                int situation = PotentialEnemyDetected(collider);
                if(situation == 1)
                {
                    if (animal.currentEnemy == collider.transform) return true;
                    AssignTarget(collider.transform, LocationType.Enemy, true);

                    return true;
                }
                else if(situation == 2)
                {
                    if(minDstHuntCollider == null && animal.isHungry)
                        minDstHuntCollider = collider;
                }
            }
        }
        if(minDstHuntCollider != null)
        {
            if (animal.currentEnemy == minDstHuntCollider.transform) return true;
            AssignTarget(minDstHuntCollider.transform, LocationType.Enemy, false);

            return true;
        }
        return false;
    }
    
    private int PotentialEnemyDetected(Collider collider)
    {
        int enemyFoodChainIndex = collider.GetComponent<Animal>().foodChainIndex;
        int foodChainIndex = animal.foodChainIndex;

        if (enemyFoodChainIndex != foodChainIndex)
        {
            if (enemyFoodChainIndex > foodChainIndex)
            {
                return 1; // Will Run
            }
            else if (enemyFoodChainIndex < foodChainIndex)
            {
                return 2; // Will Hunt
            }
        }
        return 0; // Is not enemy
    }


    private bool SearchPartners(List<Collider> animalColliders)
    {
        if (animalColliders.Count < 1) return false;
        IOrderedEnumerable<Collider> sortedAnimalColliders = SortCollidersForDistance(animalColliders);
        foreach (Collider collider in sortedAnimalColliders)
        {
            if(collider.transform != transform)
            {
                if (PotentialPartnerDetected(collider))
                {
                    AssignTarget(collider.transform, LocationType.Partner, false);
                    return true;
                }
            }
        }
        return false;
    }
    
    private bool PotentialPartnerDetected(Collider collider)
    {
        Animal potentialPartner = collider.GetComponent<Animal>();
        bool canInitiateReproducing = animal.isEligiblePartner(potentialPartner);
        bool isPartnerEligible = potentialPartner.isEligiblePartner(animal) && animal.dNA.isOppositeSex(potentialPartner.dNA.sex);
        if (canInitiateReproducing && isPartnerEligible)
        {
            return true;
        }
        return false;
    }
    
    private bool SearchFood(List<Collider> foodColliders)
    {
        if (foodColliders.Count < 1) return false;
        IOrderedEnumerable<Collider> sortedFoodColliders = SortCollidersForDistance(foodColliders);
        foreach(Collider collider in sortedFoodColliders)
        {
            if (PotentialFoodDetected(collider))
            {
                AssignTarget(collider.transform, LocationType.Food, false);
            }
        }
        return false;
    }

    private bool PotentialFoodDetected(Collider collider)
    {
        if (!collider.GetComponent<Food>().isDecayed)
        {
            return true;
        }
        return false;
    }

    private bool SearchPlant(List<Collider> plantColliders)
    {
        if (plantColliders.Count < 1) return false;
        IOrderedEnumerable<Collider> sortedPlantColliders = SortCollidersForDistance(plantColliders);
        foreach (Collider collider in sortedPlantColliders)
        {
            if (PotentialPlantDetected(collider))
            {
                AssignTarget(collider.transform, LocationType.Food, false);
            }
        }
        return false;
    }

    private bool PotentialPlantDetected(Collider collider)
    {
        if (!collider.GetComponent<Plant>().isNotEdible)
        {
            return true;
        }
        return false;
    }

    private bool SearchWater()
    {
        RaycastHit hit;
        Vector3 direction;
        float minDst = Mathf.Infinity;
        Vector3 minPoint = Vector3.zero;
        for (int i = 0; i < fovDirections.Length; i++)
        {
            direction = fovDirections[i].normalized;
            direction = transform.TransformVector(direction);
            if (Physics.Raycast(scanOrigin, direction, out hit, GroundingProp.maxLookDownForItemDistance, Masks.groundWaterMask))
            {
                if (hit.transform.gameObject.layer == 4) // Is Water
                {
                    if(hit.distance < minDst)
                    {
                        minDst = hit.distance;
                        minPoint = hit.point;
                    }
                    Debug.DrawLine(scanOrigin, hit.point, Color.green);
                }
                else
                {
                    Debug.DrawRay(scanOrigin, direction, Color.black);
                }
            }
        }
        if(minPoint != Vector3.zero)
        {
            AssignTarget(minPoint, LocationType.Water, false);
            return true;
        }
        return false;
    }
   

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(scanOrigin, properties.AwarenessRadius); //ScanArea
        }
    }

}

