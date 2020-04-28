using UnityEngine;
using UnityEngine.UI;

public class AnimalInteractionUIItem:MonoBehaviour
{
    public Transform interactionTransform;

    public Texture reproIcon;
    public Texture eatingIcon;

    private void Start()
    {
        if (interactionTransform.GetComponent<ReproManager>() != null)
            GetComponentInChildren<RawImage>().texture = reproIcon;
        else if(interactionTransform.GetComponent<EatingManager>() != null)
            GetComponentInChildren<RawImage>().texture = eatingIcon;
    }

    public void OnClickedItem()
    {
        Camera.main.GetComponent<CameraTargetFollower>().SetTarget(interactionTransform);
    }

}

