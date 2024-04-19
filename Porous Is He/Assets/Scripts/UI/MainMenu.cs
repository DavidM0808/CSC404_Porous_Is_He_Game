using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private PauseMenu pauseMenu;

    private GameObject SelectBoard;

    public void StartGame()
    {
        SelectBoard = GameObject.Find("SelectBoard");
        if (SelectBoard != null)
        {
            SelectBoard.GetComponent<BubblesInLevelSelect>().sceneIsSwitched = false;
        }

        SceneManager.LoadScene("LevelSelect");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenSettings()
    {
        pauseMenu.PauseGame();
    }
}
