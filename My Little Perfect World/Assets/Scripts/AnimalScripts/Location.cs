using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LocationType {
    Default = 0,
    Water = 1,
    Enemy = 2,
    Partner = 3,
    Food = 4,
    Plant = 5
};

[Serializable]
public class Location
{
    private LocationType locationType;
    private Vector3 location;

    private Transform objectToFollow = null;
    public bool isGettingAwayFrom;

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

    public Location()
    {
        location = Vector3.zero;
        locationType = LocationType.Default;
    }

    public Location(Transform objectToFollow, LocationType locationType, bool isGettingAwayFrom)
    {
        this.objectToFollow = objectToFollow;
        this.locationType = locationType;
        this.isGettingAwayFrom = isGettingAwayFrom;
    }

    public Location(Vector3 location, LocationType locationType, bool isGettingAwayFrom)
    {
        this.location = location;
        this.locationType = locationType;
        this.isGettingAwayFrom = isGettingAwayFrom;
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