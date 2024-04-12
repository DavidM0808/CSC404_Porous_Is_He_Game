using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PauseMenu : MonoBehaviour
{

    public GameObject pauseMenu;
    public static bool isPaused;

    private PlayerInputActions playerInputActions;
    [SerializeField] private GameObject pauseMenuFirst;

    [SerializeField] private Toggle hardmodeToggle;
    [SerializeField] private Toggle controllerToggle;
    [SerializeField] private Toggle showControlsToggle;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private Slider aimSensitivitySlider;

    [SerializeField] private TextMeshProUGUI pauseSymbol;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button resumeButton;

    [SerializeField] private Button closeDefaultButton;

    [SerializeField] private PlayerInfoManager pim;

    [SerializeField] private TMP_FontAsset std_font;
    [SerializeField] private TMP_FontAsset std_font_disabled;

    // Start is called before the first frame update
    void Start()
    {
        ResumeGame(false);

        playerInputActions = new PlayerInputActions();
        playerInputActions.GameManager.Enable();
        playerInputActions.GameManager.Pause.started += PauseGame;
        playerInputActions.GameManager.Cancel.started += ClosePause;

        Scene scene = SceneManager.GetActiveScene();
        if (scene.name != "MainMenu")
        {
            hardmodeToggle.GetComponent<Toggle>().interactable = false;
        } else
        {
            pauseSymbol.text = "";
            menuButton.interactable = false;
            menuButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().font = std_font_disabled;
            //resumeButton.GetComponent<RectTransform>().position += new Vector3(0, -20, 0); 
            //menuButton.gameObject.SetActive(false);
        }

        pim.loadedEvent.AddListener(ReflectSettings);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PauseGame(InputAction.CallbackContext context)
    {
        PauseGame();
    }

    public void ClosePause(InputAction.CallbackContext context)
    {
        if (isPaused)
        {
            ResumeGame(true);
        }
    }

    public void PauseGame()
    {
        if (LevelComplete.LevelEnd) return;
        if (isPaused)
        {
            ResumeGame(true);
        } else
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            isPaused = true;
            Cursor.lockState = CursorLockMode.None;
            EventSystem.current.SetSelectedGameObject(pauseMenuFirst);
        }
    }



    public void ResumeGame(bool doSave)
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Scene scene = SceneManager.GetActiveScene();
        if (scene.name != "MainMenu")
        {
            Cursor.lockState = CursorLockMode.Locked;
            EventSystem.current.SetSelectedGameObject(null);
        } else
        {
            EventSystem.current.SetSelectedGameObject(closeDefaultButton.gameObject);
        }


        if (doSave)
        {
            SaveSettings();
        }
    }

    public void ReflectSettings()
    {
        sensitivitySlider.value = pim.sensitivity;
        aimSensitivitySlider.value = pim.aimSensitivity;
        showControlsToggle.isOn = pim.showControls;
        controllerToggle.isOn = pim.useController;
        hardmodeToggle.isOn = pim.hardmode;
    }

    public void SaveSettings()
    {
        pim.sensitivity = sensitivitySlider.value;
        pim.aimSensitivity = aimSensitivitySlider.value;
        pim.showControls = showControlsToggle.isOn;
        pim.useController = controllerToggle.isOn;
        pim.hardmode = hardmodeToggle.isOn;
        pim.Save();
    }

    private void OnDestroy()
    {
        playerInputActions.GameManager.Disable();
    }

    
}
