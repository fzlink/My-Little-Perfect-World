using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface CreatureFactory
{
    GameObject CreateChild(GameObject gameObject, Vector3 position, Transform container);
}
