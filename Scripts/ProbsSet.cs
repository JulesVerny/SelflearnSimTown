using System.Collections;
using System.Collections.Generic;


public class ProbsSet 
{
    // ==============================================
    public int GotoOffice;
    public int GoFoodShop;
    public int GottoPub;
    public int GotoResturant;
    public int GotoPark;
    public int GoHomeEat;
    public int GoHomeRest;

    // ==============================================
    public ProbsSet()
    {
        GotoOffice = 50;
        GoFoodShop = 50;
        GottoPub = 50;
        GotoResturant = 50;
        GotoPark = 50;
        GoHomeEat = 50;
        GoHomeRest = 50;
    }  // Basic Constructor

    // ==============================================
    public int TotalSum()
    {
        return GotoOffice + GoFoodShop + GottoPub + GotoResturant + GotoPark + GoHomeEat + GoHomeRest;
    } // TotalSum
    // ==============================================
}  // ProbsSet
