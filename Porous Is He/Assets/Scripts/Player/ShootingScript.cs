using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class ShootingScript : MonoBehaviour
{

    public bool aiming = false;
    private GameObject Player;
    private LiquidTracker liquidTracker;

    // Shooting variables
    [SerializeField] private float shootDistance = 60;
    [SerializeField] private float shootAmount = 0.1f;
    private bool shooting = false;
    [SerializeField] private float timeBetweenShoot = 0.15f;
    private float time = 1f;

    private PlayerInputActions playerInputActions;

    // shooting force 
    private float maxShootForce = 32;
    private float minShootForce = 5;
    private float maxShootUpwardForce = 6;
    private float minShootUpwardForce = 5;
    private float shootForce;
    private float shootUpwardForce;
    private Vector2 inputVector;
    private float lastFailedShot = 0f;
    private bool dryFired = false;
    private float dryFireStart = 0f;
    private int dryFireTimesInRow = 0;

    [SerializeField] private Animator playerAnimator;
    [SerializeField] private LiquidBarsUI ui;

    void Start()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Shoot.started += StartShoot;
        playerInputActions.Player.Shoot.canceled += StopShoot;

        Player = GameObject.Find("Player");
        liquidTracker = Player.GetComponent<LiquidTracker>();
    }

    private void StartShoot(InputAction.CallbackContext context)
    {
        if (PauseMenu.isPaused) return;
        shooting = true;
        transform.GetComponent<AudioSource>().Play();
    }

    private void StopShoot(InputAction.CallbackContext context)
    {
        shooting = false;
        transform.GetComponent<AudioSource>().Stop();

        dryFired = false;
    }

    private void Update()
    {
        bool fired = false;

        if (Time.time - dryFireStart > 4.0f)
        {
            dryFireStart = Time.time;
            dryFireTimesInRow = 0;
        }

        if (aiming)
        {
            // this controls shoot force when aiming
            inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
            float tempShootForce = shootForce + inputVector.y * 0.2f;

            if (tempShootForce < minShootForce) 
                shootForce = minShootForce;
            else if (tempShootForce > maxShootForce) 
                shootForce = maxShootForce;
            else 
                shootForce = tempShootForce;
            shootUpwardForce = Utils.ConvertRatio(minShootForce, maxShootForce, shootForce, minShootUpwardForce, maxShootUpwardForce);

        }
        if (shooting && CanShoot())
        {
            fired = true;
            if (time >= timeBetweenShoot)
            {
                CreateBullet();
                DeductLiquid();
                time = 0;
            }
        } else if (shooting && !CanShoot())
        {
            if (liquidTracker.GetSelectedLiquid().liquidAmount <= 0 && !PoCombust.isOnFire && aiming)
            {
                ui.ShakeLiquidBar(liquidTracker.GetSelectedLiquid().liquidType);
            }
            transform.GetComponent<AudioSource>().Stop();
            if (Time.time - lastFailedShot >= 0.5f && !dryFired && aiming)
            {
                dryFired = true;
                dryFireTimesInRow++;

                if (dryFireTimesInRow >= 5 && GameObject.Find("Player").GetComponent<LiquidTracker>().CalcWeight() <= 0)
                {
                    PoMessenger poMessenger = GameObject.Find("Player").GetComponent<PoMessenger>();
                    PoMessage msg = new PoMessage("Please don't squeeze me when I'm dry :( It hurts", 4);
                    poMessenger.AddMessage(msg);
                }

                transform.parent.GetComponent<PoSoundManager>().PlaySound("NoLiquid");
                lastFailedShot = Time.time;
            }
        }
        if (time < timeBetweenShoot) time += Time.deltaTime;

        if (fired)
        {
            playerAnimator.SetBool("IsShooting", true);
        } else
        {
            playerAnimator.SetBool("IsShooting", false);
        }
    }

    private bool CanShoot()
    {
        LiquidInfo Liquid = liquidTracker.GetSelectedLiquid();
        if (PoCombust.isOnFire) return false;
        if (Liquid.liquidAmount <= 0)
            return false;
        if (!aiming) return false;

        return true;
    }
    private void DeductLiquid()
    {
        liquidTracker.RemoveSelectedLiquid(shootAmount);
    }
    private void CreateBullet()
    {
        LiquidInfo Liquid = liquidTracker.GetSelectedLiquid();
        Liquid = new LiquidInfo(Liquid.liquidType, shootAmount);

        Vector3 attackPointPos = transform.position;

        // Find hit position (at center of screen)
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        Vector3 targetPoint = ray.GetPoint(shootDistance);
        Vector3 direction = targetPoint - attackPointPos;

        // Spawn bullet
        //GameObject currentBullet = Instantiate(bullet, attackPointPos, Quaternion.identity);
        GameObject currentBullet = transform.GetComponent<ProjectileFactory>().NewProjectile(Liquid, attackPointPos, Quaternion.identity);
        currentBullet.transform.forward = direction.normalized;
        currentBullet.GetComponent<Rigidbody>().AddForce((direction.normalized * shootForce) + (Vector3.up * shootUpwardForce), ForceMode.Impulse);
        //PoSoundManager playerSound = Player.GetComponent<PoSoundManager>();
        //playerSound.PlaySound("Shoot");

    }

    public Vector3 GetVelocity()
    {
        Vector3 attackPointPos = transform.position;

        // Find hit position (at center of screen)
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        Vector3 targetPoint = ray.GetPoint(shootDistance);
        Vector3 direction = targetPoint - attackPointPos;
        Vector3 up = Vector3.up * shootUpwardForce;
        return (direction.normalized * shootForce) + up;
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Disable();
    }


    // THis is the old shooting script. Keeping it in case we need it.

    /*    public float shootForce;
        [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
        public bool aiming = false;

        private PlayerInputActions playerInputActions;

        void Start()
        {
            playerInputActions = new PlayerInputActions();
            playerInputActions.Player.Enable();
            playerInputActions.Player.Shoot.started += Shoot;
        }

        private void Shoot(InputAction.CallbackContext context)
        {
            if (PauseMenu.isPaused) return;
            if (!CanShoot())return;
            CreateBullet();
            DeductLiquid();
        }

        private bool CanShoot()
        {
            GameObject Player = GameObject.Find("Player");
            LiquidTracker LiqTrack = Player.GetComponent<LiquidTracker>();
            LiquidInfo Liquid = LiqTrack.GetSelectedLiquid();
            if (PoCombust.isOnFire) return false;
            if (Liquid.liquidAmount == 0)
                return false;
            if (!aiming) return false;

            return true;
        }
        private void DeductLiquid()
        {
            GameObject Player = GameObject.Find("Player");
            LiquidTracker LiqTrack = Player.GetComponent<LiquidTracker>();
            LiqTrack.RemoveSelectedLiquid(1);
        }
        private void CreateBullet()
        {
            GameObject Player = GameObject.Find("Player");
            LiquidTracker LiqTrack = Player.GetComponent<LiquidTracker>();
            LiquidInfo Liquid = LiqTrack.GetSelectedLiquid();

            Vector3 attackPointPos = transform.position;

            // Find hit position (at center of screen)
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

            // If ray hit something, aim there
            Vector3 targetPoint = ray.GetPoint(50);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
            {
                targetPoint = raycastHit.point;
            }

            // Calculate direction from attackPoint to targetPoint
            Vector3 direction = targetPoint - attackPointPos;

            // Spawn bullet
            //GameObject currentBullet = Instantiate(bullet, attackPointPos, Quaternion.identity);
            GameObject currentBullet = transform.GetComponent<ProjectileFactory>().NewProjectile(Liquid, attackPointPos, Quaternion.identity);
            currentBullet.transform.forward = direction.normalized;
            currentBullet.GetComponent<Rigidbody>().AddForce(direction.normalized * shootForce, ForceMode.Impulse);

            PoSoundManager playerSound = Player.GetComponent<PoSoundManager>();
            playerSound.PlaySound("Shoot");

        }

        private void OnDestroy()
        {
            playerInputActions.Player.Disable();
        }*/
}
