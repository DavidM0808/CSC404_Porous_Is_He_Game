using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Resizing : MonoBehaviour
{
    
    private LiquidTracker _liquidTracker;

    private Vector3 originalScale;
    private float increaseScaleBy = 0.1f;
    private float increaseScaleByHeavy = 0.3f;
    private Vector3 addScale;
    float percentage;



    void Start()
    {
        _liquidTracker = gameObject.GetComponent<LiquidTracker>();
        originalScale = transform.localScale;

    }

    // Update is called once per frame
    void Update()
    {
        //percentage = (_liquidTracker.GetSelectedLiquid().liquidAmount / _liquidTracker.maxLiquidAmount) * increaseScaleBy;
        percentage = _liquidTracker.CalcWeight() * increaseScaleBy;
        if (_liquidTracker.CalcWeight() > 0)
        {
            percentage += increaseScaleByHeavy;
        }
        addScale = new Vector3(percentage, percentage, percentage);
        transform.localScale = Vector3.Lerp(transform.localScale, originalScale + addScale, 2 * Time.deltaTime);
    }
}
