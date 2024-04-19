using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.AI;


// This Switches the camera from 3rd person view to aiming mode
public class SwitchCamera : MonoBehaviour
{
    public CinemachineFreeLook thirdPersonCamera;
    public CinemachineVirtualCamera aimCamera;
    private Transform projectile;
    public GameObject projectionLine;
    private CinemachineBrain cameraBrain;

    //public Slider sensitivitySlider;
    //public Slider aimSensitivitySlider;
    public PlayerInfoManager pim;
    public Toggle usingController;
    private float Xsensitivity = 0.0264f;
    private float Ysensitivity = 0.0004f;

    private bool aiming = false;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();

    private float speedV = 0.011f;
    private float speedH = 0.011f;
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    private PlayerInputActions playerInputActions;


    private MoverScript moverScript;
    private ShootingScript shootingScript;
    private Transparency transparency;

    void Start()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Aim.started += Aim;
        playerInputActions.Player.Aim.canceled += StopAim;

        // initial setup
        thirdPersonCamera.Priority = 20;
        aimCamera.Priority = 10;
        thirdPersonCamera.m_RecenterToTargetHeading.m_enabled = false;
        projectionLine.SetActive(false);

        projectile = transform.Find("ProjectileSpawn");

        cameraBrain = Camera.main.GetComponent<CinemachineBrain>();

        moverScript = gameObject.GetComponent<MoverScript>();
        shootingScript = projectile.GetComponent<ShootingScript>();
        transparency = gameObject.GetComponent<Transparency>();

}

    void Update()
    {
        // there is a better way to handle this. but it's not a priority
        // should have a GameManager game object that handles the game control/pause
        if (PauseMenu.isPaused) 
        {
            cameraBrain.enabled = false;
            return;
        }
        cameraBrain.enabled = true;
        if (aiming)
        {
            AimCamera();
        }
        // update sensitivity
        if (usingController.isOn)
        {
            Xsensitivity = 0.0264f * 8f;
            Ysensitivity = 0.0004f * 8f;
        } else
        {
            Xsensitivity = 0.0264f;
            Ysensitivity = 0.0004f;
}
        //thirdPersonCamera.m_XAxis.m_MaxSpeed = Xsensitivity * sensitivitySlider.value;
        thirdPersonCamera.m_XAxis.m_MaxSpeed = Xsensitivity * pim.sensitivity;
        thirdPersonCamera.m_YAxis.m_MaxSpeed = Ysensitivity * pim.sensitivity;
    }

    private void Aim(InputAction.CallbackContext context)
    {
        if (PauseMenu.isPaused) return;
        if (PoCombust.isOnFire) return;
            // Get the center where the camera is pointing at
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        Vector3 mouseWorldPosition = ray.GetPoint(50);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            mouseWorldPosition = raycastHit.point;
        }

        // Rotate the player to face the camera's aim
        Vector3 worldAimTarget = mouseWorldPosition;
        worldAimTarget.y = transform.position.y;
        Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
        transform.forward = aimDirection;

        // Set player's curent rotation so that we can adjust this in AimCamera()
        rotationX = transform.eulerAngles.x;
        rotationY = transform.eulerAngles.y;

        // Enable third person camera recenter so that when we switch back to default camera,
        // the camera will be behind the player.
        thirdPersonCamera.m_RecenterToTargetHeading.m_enabled = true;

        // Switch to aiming mode
        aiming = true;
        thirdPersonCamera.Priority = 10;
        aimCamera.Priority = 20;
        projectionLine.SetActive(true);

        // Disable movement, enable shooting
        moverScript.aiming = true;
        shootingScript.aiming = true;
        transparency.aiming = true;
    }

    private void StopAim(InputAction.CallbackContext context)
    {
        //if (PauseMenu.isPaused) return;
        // Reset player's x rotation
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);

        // Switch to default camera mode
        aiming = false;
        thirdPersonCamera.Priority = 20;
        aimCamera.Priority = 10;
        projectionLine.SetActive(false);

        // Disable automatic recenter
        thirdPersonCamera.m_RecenterToTargetHeading.m_enabled = false;

        // Enable movement, disable shooting
        Invoke("Switch", 0.2f);
    }

    private void Switch()
    {
        if (aiming) return;
        moverScript.aiming = false;
        shootingScript.aiming = false;
        transparency.aiming = false;
    }

    private void AimCamera()
    {

        Vector2 inputVector = playerInputActions.Player.MouseLook.ReadValue<Vector2>();
        if (usingController.isOn)
        {
            //rotationX -= speedV * inputVector.y * 10 * aimSensitivitySlider.value;
            rotationX -= speedV * inputVector.y * 10 * pim.aimSensitivity;
            rotationY += speedH * inputVector.x * 10 * pim.aimSensitivity;
        }
        else
        {
            rotationX -= speedV * inputVector.y * pim.aimSensitivity;
            rotationY += speedH * inputVector.x * pim.aimSensitivity;
        }

        if (rotationX > 33.0f)
        {
            rotationX = 33.0f;
        }
        else if (rotationX < -30.0f)
        {
            rotationX = -30.0f;
        }

        transform.eulerAngles = new Vector3(rotationX, rotationY, 0);
    }
    private void OnDestroy()
    {
        playerInputActions.Player.Disable();
    }
}
