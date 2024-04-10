using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidTracker : MonoBehaviour
{
    [SerializeField] private LiquidBarsUI ui;
    private LiquidInfo[] playerLiquids;

    // current liquid index
    private int liquidSelectionIndex = 0;

    // max number of liquid types player is allowed to have
    public int maxLiquidType = 2;

    public float maxLiquidAmount = 1;

    // Start is called before the first frame update
    void Start()
    {
        playerLiquids = new LiquidInfo[maxLiquidType];
        for (int i = 0; i < maxLiquidType; ++i)
        {
            if (i == 0)
                playerLiquids[i] = new LiquidInfo("Water", 0);
            else if (i == 1)
                playerLiquids[i] = new LiquidInfo("Oil", 0);
            else if (i == 2)
                playerLiquids[i] = new LiquidInfo("Coffee", 0);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public float GetLiquidAmountFromIndex(int index)
    {
        if (index < maxLiquidType && index >= 0)
        {
            return playerLiquids[index].liquidAmount;
        } else {
            return 0;
        }
    }

    public LiquidInfo GetSelectedLiquid()
    {
        return playerLiquids[liquidSelectionIndex];
        
    }

    public float AddSelectedLiquid(LiquidInfo liquid)
    {
        // if liquid is the same as player's current liquid

        int liquidIndex = GetLiquidIndex(liquid.liquidType);
        // switches to the liquid that is currently being absorbed
        if (liquidIndex != liquidSelectionIndex)
        {
            ui.SwapLiquidHelper();
        }

        float beforeAmount = playerLiquids[liquidIndex].liquidAmount;
        float tempAmount = playerLiquids[liquidIndex].liquidAmount + liquid.liquidAmount;
        playerLiquids[liquidIndex].liquidAmount = (tempAmount > maxLiquidAmount) ? maxLiquidAmount : tempAmount;

        return playerLiquids[liquidIndex].liquidAmount - beforeAmount;

        //if (liquid.liquidType.Equals(playerLiquids[liquidSelectionIndex].liquidType))
        //{
        //    int beforeAmount = playerLiquids[liquidSelectionIndex].liquidAmount;
        //    int tempAmount = playerLiquids[liquidSelectionIndex].liquidAmount + liquid.liquidAmount;
        //    playerLiquids[liquidSelectionIndex].liquidAmount = (tempAmount > maxLiquidAmount) ? maxLiquidAmount : tempAmount;

        //    return playerLiquids[liquidSelectionIndex].liquidAmount - beforeAmount;
        //}
        //else
        //{
        //    bool liquidAdded = false;
        //    int amountLiquidAdded = 0;
        //    for(int i = 0; i < maxLiquidType; i++)
        //    {

        //        if(playerLiquids[i] == null && !liquidAdded)
        //        {
        //            playerLiquids[i] = liquid;

        //            amountLiquidAdded = liquid.liquidAmount;

        //            liquidAdded = true;

        //            liquidSelectionIndex = i;
        //        }
        //        else if(playerLiquids[i] != null)
        //        {
        //            // clear non-current liquid
        //            playerLiquids[i].liquidAmount = 0;

        //        }
        //    }

        //    return amountLiquidAdded;
        //}

    }

    public void RemoveSelectedLiquid(float amount)
    {
        playerLiquids[liquidSelectionIndex].liquidAmount -= amount;
        if (playerLiquids[liquidSelectionIndex].liquidAmount < 0)
        {
            playerLiquids[liquidSelectionIndex].liquidAmount = 0;
        }
    }

    public void RemoveLiquidFromIndex(int index, float amount)
    {
        playerLiquids[index].liquidAmount -= amount;
        if (playerLiquids[index].liquidAmount < 0)
        {
            playerLiquids[index].liquidAmount = 0;
        }
    }

    public int GetSelectionIndex()
    {
        return liquidSelectionIndex;
    }

    public void SetSelectionIndex(int index)
    {
        liquidSelectionIndex = index;
    }

    public float CalcWeight()
    {
        float weight = 0;
        for(int i = 0; i < maxLiquidType; i++)
        {
            if(playerLiquids[i] != null)
            {
                weight += playerLiquids[i].liquidAmount * (1f/3f);
            }
        }

        

        return Mathf.Min(weight, 1);
    }
    public bool IsHeavy()
    {
        if (CalcWeight() > 0) {
            return true;
        }
        return false;
    }

    public int GetLiquidIndex(string liquidType)
    {
        for (int i = 0; i < maxLiquidType; i++)
        {
            if (liquidType == playerLiquids[i].liquidType)
            {
                return i;
            }
        }

        return -1;
    }

    public bool FullLiquid(string liquidType)
    {
        int liquidIndex = GetLiquidIndex(liquidType);
        if (playerLiquids[liquidIndex].liquidAmount == maxLiquidAmount) return true;
        return false;
    }

    public void RemoveAllLiquid()
    {
        for (int i = 0; i < maxLiquidType; i++)
        {
            playerLiquids[i].liquidAmount = 0;
        }
    }
}
