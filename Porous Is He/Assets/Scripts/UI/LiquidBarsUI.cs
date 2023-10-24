using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class LiquidBarsUI : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    private LiquidTracker liquidTracker;

    [SerializeField] private Slider slider0;
    [SerializeField] private Slider slider1;
    [SerializeField] private Slider slider2;

    float scaleModifier;



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
    }


    void Update()
    {
        // Adjust the liquid amount in the liquid meter bar
        LiquidInfo Liquid = liquidTracker.GetSelectedLiquid();
        float targetAmount = (float)Liquid.liquidAmount / (float)liquidTracker.maxLiquidAmount;

        int currentSelection = liquidTracker.GetSelectionIndex();
        if (currentSelection == 0)
            slider0.value = Mathf.Lerp(slider0.value, targetAmount, 2.5f * Time.deltaTime);
        else if (currentSelection == 1)
            slider1.value = Mathf.Lerp(slider1.value, targetAmount, 2.5f * Time.deltaTime);
        else if (currentSelection == 2)
            slider2.value = Mathf.Lerp(slider2.value, targetAmount, 2.5f * Time.deltaTime);


    }

    // Swap currently selected liquid and change the scale of the UI
    private void SwapLiquid(InputAction.CallbackContext context)
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

}

