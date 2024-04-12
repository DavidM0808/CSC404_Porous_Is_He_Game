using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Cancel : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    [SerializeField] private string screen;


    void Start()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.GameManager.Enable();
        playerInputActions.GameManager.Cancel.started += HandleCancel;
    }

    public void HandleCancel(InputAction.CallbackContext context)
    {
        SceneManager.LoadScene(0);
    }

    private void OnDestroy()
    {
        playerInputActions.GameManager.Disable();
    }
}
