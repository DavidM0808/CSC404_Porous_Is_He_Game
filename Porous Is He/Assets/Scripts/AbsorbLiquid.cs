using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbsorbLiquid : MonoBehaviour
{
    // this script is attached to Player. It provides the absorb mechanic
    [SerializeField] private float amountAbsorbed = 0.025f;
    private bool touchingLiquid = false;
    private PlayerInputActions playerInputActions;
    private LiquidSource liquidSource;
    private LiquidTracker liquidTracker;

    private bool absorbing = false;
    private bool onFillableCup = false;
    private FillableCup fillableCup;

    public GameObject interactUI;

    [SerializeField] private LiquidBarsUI ui;

    void Start()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Absorb.started += StartAbsorb;
        playerInputActions.Player.Absorb.canceled += StopAbsorb;
        playerInputActions.Player.Release.started += ReleaseAllLiquid;
        liquidTracker = GetComponent<LiquidTracker>();
    }

    private void Update()
    {
        if (absorbing)
        {
            if (onFillableCup && fillableCup.GetLiquidAmount() > 0 && !liquidTracker.FullLiquid(fillableCup.GetSurfaceLiquidType()))
            {
                LiquidInfo liquid = fillableCup.RemoveLiquid(amountAbsorbed);
                liquidTracker.AddSelectedLiquid(liquid);
            }
            else if (touchingLiquid && !liquidTracker.FullLiquid(liquidSource.liquidType))
            {
                LiquidInfo liquid = liquidSource.AbsorbLiquid(amountAbsorbed);
                if (liquid.liquidAmount != 0)
                {
                    //gameObject.GetComponent<PoSoundManager>().PlaySound("Absorb");
                    liquidTracker.AddSelectedLiquid(liquid);
                }
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {

        interactUI.SetActive(false);
        touchingLiquid = false;
        onFillableCup = false;

        if (hit.collider.gameObject.tag == "Water" || hit.collider.gameObject.tag == "Oil")
        {
            interactUI.SetActive(true);
            //if (liquidTracker.GetSelectedLiquid().liquidType != "Water") return;
            touchingLiquid = true;
            liquidSource = hit.collider.gameObject.GetComponent<LiquidSource>();
        }
        if (hit.collider.gameObject.name == "LiquidLevelCollider")
        {
            interactUI.SetActive(true);
            onFillableCup = true;
            fillableCup = hit.collider.gameObject.GetComponentInParent<FillableCup>();
        }
    }


    private void StartAbsorb(InputAction.CallbackContext context)
    {
        if (PauseMenu.isPaused) return;
        absorbing = true;
        if (touchingLiquid && liquidTracker.FullLiquid(liquidSource.liquidType)){
            ui.ShakeLiquidBar(liquidSource.liquidType);
        }

        if ((touchingLiquid && !liquidTracker.FullLiquid(liquidSource.liquidType)) || 
            (onFillableCup && fillableCup.GetLiquidAmount() > 0 && !liquidTracker.FullLiquid(fillableCup.GetSurfaceLiquidType())))
        {
            gameObject.GetComponent<PoSoundManager>().PlaySound("Absorb");
            gameObject.GetComponent<AudioSource>().Play();
        }
        
    }

    private void StopAbsorb(InputAction.CallbackContext context)
    {
        absorbing = false;
        gameObject.GetComponent<AudioSource>().Stop();
    }

    private void ReleaseAllLiquid(InputAction.CallbackContext context)
    {
        //liquidTracker.RemoveSelectedLiquid(liquidTracker.GetSelectedLiquid().liquidAmount);
        if (liquidTracker.CalcWeight() > 0)
        {
            liquidTracker.RemoveAllLiquid();
            gameObject.GetComponent<PoSoundManager>().PlaySound("Release");
        }
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Disable();
    }
}
