using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public enum State { START = 1, SELECT_HABITAT = 2, INFO_SHOWN = 3};

    private State state;

    public GameObject planet;
    public GameObject menuButtonContainer;
    public GameObject titleContainer;
    public GameObject navigationContainer;
    public GameObject infoContainer;

    private Vector3 planetStartPos;
    private Vector3 planetStartRot;

    public TMP_Text titleText;
    private bool rotatePlanet = true;
    [SerializeField] private float planetRotationSpeed;

    public Action OnCenterPlanetFinished;
    public Action<string> OnHabitatSelected;
    public Action<State> OnStateChanged;

    public Slider mapSizeXSlider;
    public Slider mapSizeYSlider;

    public void Exit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    public void NextButton()
    {
        if(state == State.INFO_SHOWN)
        {
            FindObjectOfType<MenuData>().SetMapSize((int)mapSizeXSlider.value, (int)mapSizeYSlider.value);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private void Start()
    {
        planetStartPos = planet.transform.position;
        planetStartRot = planet.transform.eulerAngles;
        state = State.START;
        StateChange();
    }

    private void Update()
    {
        if (rotatePlanet)
        {
            RotatePlanet();
        }
    }

    public void StateChange()
    {
        if(state == State.START)
        {
            menuButtonContainer.SetActive(true);
            navigationContainer.SetActive(false);
            infoContainer.SetActive(false);
            titleContainer.SetActive(true);
            titleText.SetText("MY LITTLE PERFECT WORLD");
            planet.GetComponent<DragRotater>().canDrag = false;
            StartStatePlanet();
            rotatePlanet = true;
        }
        else if(state == State.SELECT_HABITAT)
        {
            menuButtonContainer.SetActive(false);
            navigationContainer.SetActive(true);
            navigationContainer.transform.GetChild(1).gameObject.SetActive(false);
            infoContainer.SetActive(false);
            titleContainer.SetActive(true);
            titleText.SetText("PLEASE SELECT A HABITAT");
            rotatePlanet = false;
            CenterPlanet();
            planet.GetComponent<DragRotater>().canDrag = true;
        }
        else if(state == State.INFO_SHOWN)
        {
            menuButtonContainer.SetActive(false);
            navigationContainer.SetActive(true);
            navigationContainer.transform.GetChild(1).gameObject.SetActive(true);
            infoContainer.SetActive(true);
            titleContainer.SetActive(false);
            planet.GetComponent<DragRotater>().canDrag = false;
        }
        if(OnStateChanged != null)
        {
            OnStateChanged(state);
        }
    }

    public void StartButtonPressed()
    {
        state = State.SELECT_HABITAT;
        StateChange();
    }

    public void CenterPlanet()
    {
        planet.transform.DOMoveX(0, 1).OnComplete(() =>
        {
            if(OnCenterPlanetFinished != null)
            {
                OnCenterPlanetFinished();
            }
        });
    }

    public void StartStatePlanet()
    {
        planet.transform.DOMove(planetStartPos, 1);
        planet.transform.DORotate(planetStartRot, 1);
    }


    private void RotatePlanet()
    {
        planet.transform.Rotate(Vector3.down, planetRotationSpeed * Time.deltaTime);
    }

    public void OnHabitatBrought(string landPieceType)
    {
        FindObjectOfType<MenuData>().SetHabitatType(landPieceType);
        state = State.INFO_SHOWN;
        StateChange();
        if(OnHabitatSelected != null)
        {
            OnHabitatSelected(landPieceType);
        }

    }

    public void BackButton()
    {
        if((int) state > 1)
        {
            state--;
            StateChange();
        }
    }

}
