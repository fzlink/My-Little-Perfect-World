using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    public static bool isPaused;

    private float cachedTimeScale;
    private float cachedTSM;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                Pause();
            else
                Resume();
        }
    }

    private void Pause()
    {
        isPaused = true;
        pauseMenuPanel.gameObject.SetActive(true);
        cachedTimeScale = Time.timeScale;
        cachedTSM = FourthDimension.tSM;
        Time.timeScale = 0f;
        FourthDimension.tSM = 0f;
    }

    public void Resume()
    {
        isPaused = false;
        pauseMenuPanel.gameObject.SetActive(false);
        Time.timeScale = cachedTimeScale;
        FourthDimension.tSM = cachedTSM;
    }

    public void LoadMainMenu()
    {
        isPaused = false;
        Time.timeScale = 1f;
        FourthDimension.tSM = 1f;
        SceneManager.LoadScene(0);
    }

}
