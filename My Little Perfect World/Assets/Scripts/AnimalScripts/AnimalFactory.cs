using UnityEngine;

public class AnimalFactory
{
    public static void CreateChild(Animal mother, Animal father) // With Ancestor
    {
        GameObject newChild = GameObject.Instantiate(mother.GetProperties().Animal, mother.transform.position - mother.transform.forward.normalized * 2, Quaternion.identity, mother.gameObject.transform.parent);
        Animal newChildAnimal = newChild.GetComponent<Animal>();
        newChildAnimal.dNA = ConstructDNA(mother.dNA, father.dNA);
        CheckForFemaleAndAttributes(newChild, newChildAnimal);
        newChild.GetComponent<Renderer>().material.color = newChildAnimal.dNA.skinColor;

        newChildAnimal.mother = mother;
        newChildAnimal.father = father;
        newChildAnimal.hasAncestor = true;
        newChildAnimal.dayOfBirth = FourthDimension.currentDay;

        mother.childs.Add(newChildAnimal);
        father.childs.Add(newChildAnimal);

    }

    public static GameObject CreateChild(GameObject clone, Vector3 position, Transform container) //Without Ancestor
    {
        GameObject newChild = GameObject.Instantiate(clone, position, Quaternion.identity, container);
        Animal newChildAnimal = newChild.GetComponent<Animal>();
        newChildAnimal.dNA = ConstructDNA(newChildAnimal.GetProperties().CommonSkinColor);
        newChildAnimal.dayOfBirth = Random.Range(0, 5);
        CheckForFemaleAndAttributes(newChild, newChildAnimal);
        newChild.GetComponent<Renderer>().material.color = newChildAnimal.dNA.skinColor;
        return newChild;
    }

    private static void CheckForFemaleAndAttributes(GameObject newChild, Animal newChildAnimal)
    {
        if (newChildAnimal.dNA.sex == Sex.Female)
        {
            FemaleAttributes femaleAttr = newChild.AddComponent<FemaleAttributes>();
            femaleAttr.femaleProperties = newChildAnimal.GetFemaleProperties();
        }
    }

    private static DNA ConstructDNA(DNA motherDNA, DNA fatherDNA)
    {
        return new DNA(DetermineSkinColor(motherDNA.skinColor,(motherDNA.skinIlluminance+fatherDNA.skinIlluminance)/2), (motherDNA.skinIlluminance + fatherDNA.skinIlluminance) / 2,DetermineSex());
    }

    private static DNA ConstructDNA(Color commonSkinColor)
    {
        float skinIlluminance = Random.Range(0.50f, 1.50f);
        return new DNA(DetermineSkinColor(commonSkinColor, skinIlluminance),skinIlluminance, DetermineSex());
    }

    private static Sex DetermineSex()
    {
        if (Random.value <= 0.5f) { return Sex.Male; }
        else { return Sex.Female; }
    }

    private static Color DetermineSkinColor(Color commonSkinColor, float skinIlluminance)
    {
        Color newColor = commonSkinColor * skinIlluminance;
        newColor.a = 1f;
        return newColor;
    }

}

