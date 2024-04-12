using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoCombust : MonoBehaviour
{
    [Header("Fire Indicators")]
    [SerializeField] private GameObject FireBorder;
    [SerializeField] private GameObject PoFire;
    [SerializeField] private GameObject RedFire;
    [SerializeField] private GameObject OrangeFire;
    [SerializeField] private GameObject YellowFire;

    [Header("Length of fire")]
    [SerializeField] private float oilAmountTaken = 0.01f;

    private LiquidTracker liquidTracker;
    private float amount;
    private float timeCounter;
    private int oilIndex = 1;
    public static bool isOnFire;

    // change size of fire
    private float scaleModifier;
    private float startValue;
    private float endValue;
    private Animator animator;

    void Start()
    {
        liquidTracker = GetComponent<LiquidTracker>();
        isOnFire = false;
        FireBorder.SetActive(false);
        PoFire.SetActive(false);
        animator = FireBorder.GetComponentInChildren<Animator>();
    }

    void Update()
    {   
        if (isOnFire)
        {
                liquidTracker.RemoveLiquidFromIndex(oilIndex, oilAmountTaken * Time.deltaTime * 60);
                amount = liquidTracker.GetLiquidAmountFromIndex(oilIndex);

                if (amount <= 0) EndFire();

        }
        
    }

    public void Combust()
    {
        if (isOnFire) return;
        if (liquidTracker.GetSelectedLiquid().liquidType != "Oil") return;
        if (liquidTracker.GetSelectedLiquid().liquidAmount <= 0) return;

        isOnFire = true;
        FireBorder.SetActive(true);
        PoFire.SetActive(true);
        RedFire.transform.localScale = new Vector3(3, 3, 3);
        OrangeFire.transform.localScale = new Vector3(3, 3, 3);
        YellowFire.transform.localScale = new Vector3(3, 3, 3);
        animator.SetBool("IsOnFire", true);
    }

    public void EndFire()
    {
        isOnFire = false;
        animator.SetBool("IsOnFire", false);
        StartCoroutine(LerpFireScale(1, 3, 0, RedFire, true));
        StartCoroutine(LerpFireScale(1, 3, 0, OrangeFire, true));
        StartCoroutine(LerpFireScale(1, 3, 0, YellowFire, true));
    }



    IEnumerator LerpFireScale(float duration, float startSize, float endSize, GameObject fire, bool endFire)
    {
        float time = 0;

        while (time < duration)
        {
            scaleModifier = Mathf.Lerp(startSize, endSize, time / duration);
            fire.transform.localScale = new Vector3(scaleModifier, scaleModifier, scaleModifier);
            time += Time.deltaTime;
            yield return null;
        }
        fire.transform.localScale = new Vector3(endSize, endSize, endSize);
        scaleModifier = endSize;

        if (endFire) PoFire.SetActive(false);
    }
}
