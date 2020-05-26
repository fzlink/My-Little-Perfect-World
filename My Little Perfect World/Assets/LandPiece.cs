using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandPiece : MonoBehaviour
{

    private float floatingSpeed = 10;
    private Vector3 startPos;
    private Vector3 startRot;
    private Vector3 floatEnd;
    private bool onStartFloat;
    private bool canFloat = true;
    private bool onFront;
    private MainMenu mainMenu;
    public Collider collider;

    public string landPieceType;
    private bool rotateAround;

    private void Start()
    {
        mainMenu = FindObjectOfType<MainMenu>();
        mainMenu.OnCenterPlanetFinished += OnCanFloat;
        mainMenu.OnStateChanged += StateChange;
        startPos = transform.localPosition;
        startRot = transform.eulerAngles;
    }

    private void StateChange(MainMenu.State state)
    {
        if(state == MainMenu.State.START)
        {
            canFloat = false;
            onStartFloat = false;
            collider.enabled = false;
        }
        else if(state == MainMenu.State.SELECT_HABITAT)
        {
            transform.DOKill();
            transform.DOLocalMove(startPos, .25f).OnComplete(() => canFloat = true);
            transform.DOLocalRotate(startRot, .25f);
            rotateAround = false;
            onFront = false;
            canFloat = true;
            collider.enabled = true;
        }
        else if(state == MainMenu.State.INFO_SHOWN)
        {
            collider.enabled = false;
        }
    }

    private void Update()
    {
        if (rotateAround)
        {
            transform.Rotate(Vector3.down, 20 * Time.deltaTime);
        }
    }

    private void OnCanFloat()
    {
        onStartFloat = true;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && !onFront && onStartFloat)
        {
            onFront = true;
            BringFront();
        }

        if (!Input.GetMouseButton(0) && onStartFloat)
        {
            if (canFloat)
            {
                canFloat = false;
                floatEnd = transform.position + transform.up * floatingSpeed * Time.deltaTime;
                Tween tween = transform.DOMove(floatEnd, .25f);
            }
        }
    }

    private void BringFront()
    {
        mainMenu.OnHabitatBrought(landPieceType);
        Tween frontMoveTween = transform.DOMove(Camera.main.transform.position + Vector3.forward * 3 + Vector3.down, 1);
        Tween frontRotTween = transform.DORotate(Vector3.zero, 1);
        rotateAround = true;
    }

    private void OnMouseExit()
    {
        if (!onFront)
        {
            transform.DOLocalMove(startPos, .25f).OnComplete(() => canFloat = true);
        }
    }
}
