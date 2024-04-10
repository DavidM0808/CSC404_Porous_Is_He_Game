
public class LiquidInfo
{
    public string liquidType;

    public float liquidAmount;

    public LiquidInfo(string liquidType, float liquidAmount)
    {
        this.liquidType = liquidType;
        this.liquidAmount = liquidAmount;
    }



    //how to use:
    //LiquidInfo liquid = new LiquidInfo("Water", 1);
    //liquid.liquidAmount = 3;
}
