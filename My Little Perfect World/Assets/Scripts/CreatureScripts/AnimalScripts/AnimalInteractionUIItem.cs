using UnityEngine;
using UnityEngine.UI;

public class AnimalInteractionUIItem:MonoBehaviour
{
    public Transform interactionTransform;

    public Texture reproIcon;
    public Texture eatingIcon;
    public Texture runIcon;

    private void Start()
    {
        if(interactionTransform != null)
        {
            if (interactionTransform.GetComponent<ReproManager>() != null)
                GetComponentInChildren<RawImage>().texture = reproIcon;
            else if(interactionTransform.GetComponent<EatingManager>() != null)
                GetComponentInChildren<RawImage>().texture = eatingIcon;
            else if(interactionTransform.GetComponent<RunManager>() != null)
                GetComponentInChildren<RawImage>().texture = runIcon;
        }
    }

    public void OnClickedItem()
    {
        Camera.main.GetComponent<CameraTargetFollower>().SetTarget(interactionTransform);
    }

}

