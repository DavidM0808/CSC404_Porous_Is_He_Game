using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidSource : MonoBehaviour
{
    //this script is attached to a liquid source
    public string liquidType;
    public int maxLiquid;
    public bool regenerates;

    private int remainingLiquid;
    private LiquidInfo liquid;
    private const int INFINITE = -1;

    private Vector3 originalScale;
    private Vector3 newScale;

    private float? lastChanged = null;
    private float regenCooldown = 3.0f;


    void Start()
    {
        originalScale = transform.localScale;
        newScale = originalScale;
        remainingLiquid = maxLiquid;
    }

    void Update()
    {
        if (lastChanged != null && remainingLiquid < maxLiquid && Time.time - lastChanged > regenCooldown)
        {
            ++remainingLiquid;
            lastChanged = Time.time;
        }

        // rescale the liquid puddle if it is not infinite
        if (maxLiquid != INFINITE)
        {
            float liquidRatio = (float)remainingLiquid / (float)maxLiquid;
            newScale = new Vector3(originalScale.x * liquidRatio, originalScale.y * liquidRatio, originalScale.z * liquidRatio);
            transform.localScale = Vector3.Lerp(transform.localScale, newScale, 2 * Time.deltaTime);

            //destroy if liquid amount becomes 0
            if (transform.localScale == Vector3.zero)
            {
                Destroy(gameObject);
            }
        }
    }


    public LiquidInfo AbsorbLiquid(int amount)
    {
        liquid = new LiquidInfo(liquidType, amount);
        lastChanged = Time.time;

        if (maxLiquid == INFINITE) return liquid;

        int tempLiquid = remainingLiquid - amount;

        if (tempLiquid <= 0)
        {
            liquid.liquidAmount = remainingLiquid;
            remainingLiquid = 0;
        }
        else
        {
            liquid.liquidAmount = amount;
            remainingLiquid = tempLiquid;
        }
        return liquid;
    }

}
