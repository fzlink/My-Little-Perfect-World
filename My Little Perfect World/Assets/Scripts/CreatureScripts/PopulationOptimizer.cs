using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PopulationOptimizer: MonoBehaviour
{
    private ObjectPlacer objectPlacer;
    private AnimalInteractionManager animalInteractionManager;
    private Dictionary<GameObject, AnimalOptimizationProperties> animals;

    private void Awake()
    {
        animalInteractionManager = SimulationManger.instance.animalInteractionManager;
        objectPlacer = SimulationManger.instance.objectPlacer;
    }


    void Start()
    {
        animalInteractionManager.onAnimalDied += Evaluate;
        objectPlacer.onObjectsPlaced += InitCreatures;
    }

    private void Evaluate(GameObject animalObj)
    {
        AnimalOptimizationProperties aop;
        animals.TryGetValue(animalObj, out aop);
        float ratio = aop.startPopulation / (float)aop.container.childCount;

        aop.evaluatedPregnancySpeedFactor = ratio;
        if (aop.evaluatedPregnancySpeedFactor < 0.25f)
            aop.evaluatedPregnancySpeedFactor = 0.25f;

        if(aop.container.childCount > aop.topPopThres)
        {
            aop.bottomPopThres = aop.topPopThres;
            aop.topPopThres = aop.bottomPopThres * 1.5f;
            aop.evaluatedBirthCountFactor--;
        }
        else if(aop.container.childCount < aop.bottomPopThres)
        {
            aop.topPopThres = aop.bottomPopThres;
            aop.bottomPopThres = aop.topPopThres / 1.5f;
            aop.evaluatedBirthCountFactor++;
        }

        if (aop.evaluatedBirthCountFactor < 1)
            aop.evaluatedBirthCountFactor = 1;

        /*if (ratio >= aop.divideForBirthCountFactor)
        {
            aop.divideForBirthCountFactor*=2;
            aop.evaluatedBirthCountFactor++;
        }else if( ratio < aop.divideForBirthCountFactor)
        {
            aop.divideForBirthCountFactor /= 2;
            aop.evaluatedBirthCountFactor--;
            if (aop.evaluatedBirthCountFactor < 1)
                aop.evaluatedBirthCountFactor = 1;
        }*/
    }

    public int GetEvaluatedBirthCountFactor(GameObject animalObj)
    {
        AnimalOptimizationProperties aop;
        animals.TryGetValue(animalObj, out aop);
        if (!aop.willOptimized)
            return 1;
        return aop.evaluatedBirthCountFactor;
    }

    public float GetEvaluatedPregnancySpeedFactor(GameObject animalObj)
    {
        AnimalOptimizationProperties aop;
        animals.TryGetValue(animalObj, out aop);
        if (!aop.willOptimized)
            return 1;
        return aop.evaluatedPregnancySpeedFactor;
    }

    private void InitCreatures(List<ObjectsToPlace> objs)
    {
        animals = new Dictionary<GameObject, AnimalOptimizationProperties>();
        for (int i = 0; i < objs.Count; i++)
        {
            if(objs[i].obj.GetComponent<Creature>().GetType().BaseType == typeof(Animal))
            {
                AnimalOptimizationProperties ahc = new AnimalOptimizationProperties(
                    objs[i].container,
                    objs[i].population,
                    objs[i].willOptimized
                    );
                animals.Add(objs[i].obj, ahc);
            }
        }
    }

    private class AnimalOptimizationProperties
    {
        public Transform container;
        public int startPopulation;
        public bool willOptimized;

        public int evaluatedBirthCountFactor;
        public float evaluatedPregnancySpeedFactor;
        public float divideForBirthCountFactor;

        public float topPopThres;
        public float bottomPopThres;

        public AnimalOptimizationProperties(Transform container, int startPopulation, bool willOptimized)
        {
            this.container = container;
            this.startPopulation = startPopulation;
            this.willOptimized = willOptimized;
            evaluatedBirthCountFactor = 1;
            evaluatedPregnancySpeedFactor = 1;
            divideForBirthCountFactor = 2;

            topPopThres = startPopulation;
            bottomPopThres = topPopThres / 1.5f;
        }

        public void SetEvaluation(int evaluatedBirthCount, float evaluatedPregnancySpeed)
        {
            this.evaluatedBirthCountFactor = evaluatedBirthCount;
            this.evaluatedPregnancySpeedFactor = evaluatedPregnancySpeed;
        }
    }

}

