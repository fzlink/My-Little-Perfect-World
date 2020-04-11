using UnityEngine;

public class PlantFactory : CreatureFactory
{
    public GameObject CreateChild(GameObject gameObject, Vector3 position, Transform container)
    {
        return GameObject.Instantiate(gameObject, position, Quaternion.identity, container);
    }
}

