using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LiquidBarsUI : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    private LiquidTracker liquidTracker;

    [SerializeField] private Slider slider0;
    [SerializeField] private Slider slider1;
    [SerializeField] private Slider slider2;

    private Animator slider0Animator;
    private Animator slider1Animator;

    private float scaleModifier;
    private float valueIncrement = 0.01f;
    private float targetAmount;


    void Start()
    {
        // Enable input
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Swap.started += SwapLiquid;

        liquidTracker = GameObject.Find("Player").GetComponent<LiquidTracker>();
        slider0.value = 0;
        slider1.value = 0;
        slider2.value = 0;
        slider0.gameObject.SetActive(true);
        slider1.gameObject.SetActive(false); 
        slider2.gameObject.SetActive(false);

        if (liquidTracker.maxLiquidType >= 2)
        {
            slider1.gameObject.SetActive(true);
        }
        if (liquidTracker.maxLiquidType >= 3)
        {
            slider2.gameObject.SetActive(true);
        }

        slider0Animator = slider0.GetComponent<Animator>();
        slider1Animator = slider1.GetComponent<Animator>();
    }


    void Update()
    {
        if (liquidTracker.maxLiquidType >= 1)
        {
            targetAmount = liquidTracker.GetLiquidAmountFromIndex(0) / liquidTracker.maxLiquidAmount;
            if (Mathf.Abs(targetAmount - slider0.value) > valueIncrement)
            {
                slider0.value += (targetAmount > slider0.value) ? valueIncrement : -valueIncrement;
            } else
            {
                slider0.value = targetAmount;
            }
        }
        if (liquidTracker.maxLiquidType >= 2)
        {
            targetAmount = liquidTracker.GetLiquidAmountFromIndex(1) / liquidTracker.maxLiquidAmount;
            if (Mathf.Abs(targetAmount - slider1.value) > valueIncrement)
            {
                slider1.value += (targetAmount > slider1.value) ? valueIncrement : -valueIncrement;
            }
            else
            {
                slider1.value = targetAmount;
            }
        }
        if (liquidTracker.maxLiquidType >= 3)
        {
            targetAmount = liquidTracker.GetLiquidAmountFromIndex(2) / liquidTracker.maxLiquidAmount;
            if (Mathf.Abs(targetAmount - slider2.value) > valueIncrement)
            {
                slider2.value += (targetAmount > slider2.value) ? valueIncrement : -valueIncrement;
            }
            else
            {
                slider2.value = targetAmount;
            }
        }
    }

    // Swap currently selected liquid and change the scale of the UI
    private void SwapLiquid(InputAction.CallbackContext context)
    {
        SwapLiquidHelper();
    }

    public void SwapLiquidHelper()
    {
        if (liquidTracker.maxLiquidType == 1) return;

        int currentSelection = liquidTracker.GetSelectionIndex();
        if (currentSelection == 0)
            StartCoroutine(SliderLerpFunction(1f, 0.6f, slider0));
        else if (currentSelection == 1)
            StartCoroutine(SliderLerpFunction(1f, 0.6f, slider1));
        else if (currentSelection == 2)
            StartCoroutine(SliderLerpFunction(1f, 0.6f, slider2));

        int nextSelection = (currentSelection + 1) % liquidTracker.maxLiquidType;
        liquidTracker.SetSelectionIndex(nextSelection);

        if (nextSelection == 0)
            StartCoroutine(SliderLerpFunction(0.6f, 1f, slider0));
        else if (nextSelection == 1)
            StartCoroutine(SliderLerpFunction(0.6f, 1f, slider1));
        else if (nextSelection == 2)
            StartCoroutine(SliderLerpFunction(0.6f, 1f, slider2));
    }

    IEnumerator SliderLerpFunction(float startValue, float endValue, Slider slider)
    {
        float time = 0;
        float duration = 0.3f;

        while (time < duration)
        {
            scaleModifier = Mathf.Lerp(startValue, endValue, time / duration);
            slider.transform.localScale = new Vector3(scaleModifier, scaleModifier, 1);
            time += Time.deltaTime;
            yield return null;
        }
        slider.transform.localScale = new Vector3(endValue, endValue, 1);
        scaleModifier = endValue;
    }

    private void OnDestroy()
    {
        playerInputActions.Player.Disable();
    }

    public void ShakeLiquidBar(string liquidType)
    {

        if (liquidType == "Water")
        {
            if (!slider0Animator.GetBool("shake") && !slider0Animator.GetCurrentAnimatorStateInfo(0).IsName("waterBarShake"))
            {
                slider0Animator.SetTrigger("shake");
            }
        } 
        else if (liquidType == "Oil")
        {
            if (!slider1Animator.GetBool("shake") && !slider1Animator.GetCurrentAnimatorStateInfo(0).IsName("oilBarShake"))
            {
                slider1Animator.SetTrigger("shake");
            }
        }
    }
}

