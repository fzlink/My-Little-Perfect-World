using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargetFollower : MonoBehaviour
{
    private Transform target;
    private Vector3 offset = Vector3.one * 30;
    private Camera cam;
    private PlayModeCamera playModeCamera;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        playModeCamera = GetComponent<PlayModeCamera>();
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        cam.fieldOfView = 30;
        playModeCamera.enabled = false;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            playModeCamera.enabled = true;
            if (this.target != null)
            {
                cam.fieldOfView = 60;
                target = null;
            }
        }
    }

    private void LateUpdate()
    {
        if(target != null)
        {
            transform.LookAt(target);
            transform.position = target.position + offset;
        }
    }


}