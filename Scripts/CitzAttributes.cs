using System.Collections;
using System.Collections.Generic;


public class CitzAttributes 
{
    // ================================================================
    public string Name;
    public int Tiredness;
    public int Hungryness;
    public int FoodAtHome;
    public int Hapiness;
    public int Money;
    public bool HasFainted;
    // ================================================================
    public CitzAttributes()
    {
        Name = "empty"; 
        Tiredness = 0;
        Hungryness = 25;
        Hapiness = 25;
        Money = 25;
        FoodAtHome = 25;
    }  // Basic Constructor
    // ========================================================================
    /*public void ActivityCompleted(int TirednessImpact, int HungrynessImpact, int HapinessImpact, int CostImpact, int HomeFoodImpact)
    {
        Tiredness =   + TirednessImpact;
        if (Tiredness < 0) Tiredness = 0;
        if (Tiredness > 100) Tiredness = 100;

        Hungryness = Hungryness + HungrynessImpact;
        if (Hungryness < 0) Hungryness = 0;
        if (Hungryness > 100) Hungryness = 100;

        Hapiness = Hapiness + HapinessImpact;
        if (Hapiness < -100) Hapiness = -100;
        if (Hapiness > 100) Hapiness = 100;

        Money = Money - CostImpact;
        if (Money < 0) Money = 0;
        if (Money > 100) Money = 100;

        FoodAtHome = FoodAtHome + HomeFoodImpact;
        if (FoodAtHome < 0) FoodAtHome = 0;
        if (FoodAtHome > 100) FoodAtHome = 100;

    } // ActivityCompleted

    */
    // ================================================================
    public void CheckIfFainted()
    {
        HasFainted = false;
        if ((Tiredness > 95) || (Hungryness > 95)) HasFainted = true;
        
    } // CheckIfFainted
    // ================================================================

} // CitzState
