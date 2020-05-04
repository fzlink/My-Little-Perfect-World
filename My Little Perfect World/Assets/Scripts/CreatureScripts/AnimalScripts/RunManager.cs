using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RunManager: MonoBehaviour
{
    private Animal hunter;
    private Animal prey;
    private bool canFollow;

    public void SetHunterAndPrey(Animal hunter, Transform prey)
    {
        this.hunter = hunter;
        this.prey = prey.GetComponent<Animal>();
        canFollow = true;
    }

    private void LateUpdate()
    {
        if(canFollow )
        {
            if(hunter == null || prey == null)
            {
                AnimalInteractionManager.instance.FinishRunning(this);
            }
            else
            {
                //Follow Prey
                transform.position = prey.transform.position;
            }
        }


    }


}
