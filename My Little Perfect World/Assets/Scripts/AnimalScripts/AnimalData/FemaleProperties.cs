using UnityEngine;


//[CreateAssetMenu(fileName = "Female Properties", menuName = "Animal/Female Properties", order = 1)]
public class FemaleProperties : Properties
{
    [Header("Pregnancy Values")]
    [SerializeField] private float pregnancyMaximum;
    [SerializeField] private float pregnancySpeed;
    [SerializeField] private int pregnancyChildAmount;

    public float PregnancyMaximum { get => pregnancyMaximum; set => pregnancyMaximum = value; }
    public float PregnancySpeed { get => pregnancySpeed; set => pregnancySpeed = value; }
    public int PregnancyChildAmount { get => pregnancyChildAmount; set => pregnancyChildAmount = value; }
}

