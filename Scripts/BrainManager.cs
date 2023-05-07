using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class BrainManager : MonoBehaviour
{
    public enum NextActionTypes { None, GotoOfficeWork, GoFoodShoping,GotoPub,GotoResturantToEat,GotoPark,GoHomeEat,GoHomeToRest}
    // ============================================================================================
    public SiteConfiguration OfficeParameters;
    public SiteConfiguration FoodShopParameters;
    public SiteConfiguration PubParameters;
    public SiteConfiguration ResturantParameters;
    public SiteConfiguration HomeParameters;
    public SiteConfiguration ParkParameters;


    // Need to maintain a 'Brain':  Function Citz: State => Next Action
    private float LearningRate = 0.1f; 

    // Brain Weights 7 Discrete Actions by 6 float State (Feeling)
    private float[,] BrainWeights;


    // File Write
    private StreamWriter BrainWeightsWriter;
    private StreamReader BrainWeightsReader;

    // ============================================================================================
    private void Awake()
    {

        // Brain Weights 7 Discrete Actions by 5 float State (Feeling)
        // 7x Actions: 0: GotoOfficeWork, 1: GoFoodShoping,2: GotoPub,3: GotoResturantToEat,4: GotoPark,5: GoHomeEat,6: GoHomeToRest
        // 5 x State feelings: 0:Tiredness [0:100], 1: Hungryness [0:100], 2: Hapiness [-100,100], 3: Money[0:100], 4: Food[0:100]
        
        // Initialise the BrainWeights
        BrainWeights = new float[7, 5];
        for (int i = 0; i < 7; i++)
            for (int j = 0; j < 5; j++)
                BrainWeights[i, j] = 1.0f;

    } // Awake
    // ============================================================================================
    void Start()
    {

    } // Start
    // ============================================================================
    void Update()
    {
        // Any UI controls Here 

    } // Update
    // ===========================================================================================

    ProbsSet ReturnCurrentProbabilities(CitzAttributes CurrentCitzFeelings)
    {
        ProbsSet RtnProbSet = new ProbsSet();

        // " Multiply " through the CurrentCitzFeelings with the common BrainWeights
        float OfficeActivation = BrainWeights[0, 0] * (float)CurrentCitzFeelings.Tiredness + BrainWeights[0, 1] *(float) CurrentCitzFeelings.Hungryness + BrainWeights[0, 2] * (float)CurrentCitzFeelings.Hapiness + BrainWeights[0, 3] * (float)CurrentCitzFeelings.Money + BrainWeights[0, 4] * (float)CurrentCitzFeelings.FoodAtHome;
        float ShopActivation = BrainWeights[1, 0] * (float)CurrentCitzFeelings.Tiredness + BrainWeights[1, 1] * (float)CurrentCitzFeelings.Hungryness + BrainWeights[1, 2] * (float)CurrentCitzFeelings.Hapiness + BrainWeights[1, 3] * (float)CurrentCitzFeelings.Money + BrainWeights[1, 4] * (float)CurrentCitzFeelings.FoodAtHome;
        float PubActivation = BrainWeights[2, 0] * (float)CurrentCitzFeelings.Tiredness + BrainWeights[2, 1] * (float)CurrentCitzFeelings.Hungryness + BrainWeights[2, 2] * (float)CurrentCitzFeelings.Hapiness + BrainWeights[2, 3] * (float)CurrentCitzFeelings.Money + BrainWeights[2, 4] * (float)CurrentCitzFeelings.FoodAtHome;
        float ResturantActivation =  BrainWeights[3, 0] * (float)CurrentCitzFeelings.Tiredness + BrainWeights[3, 1] * (float)CurrentCitzFeelings.Hungryness + BrainWeights[3, 2] * (float)CurrentCitzFeelings.Hapiness + BrainWeights[3, 3] * (float)CurrentCitzFeelings.Money + BrainWeights[3, 4] * (float)CurrentCitzFeelings.FoodAtHome;
        float ParkActivation = BrainWeights[4, 0] * (float)CurrentCitzFeelings.Tiredness + BrainWeights[4, 1] * (float)CurrentCitzFeelings.Hungryness + BrainWeights[4, 2] * (float)CurrentCitzFeelings.Hapiness + BrainWeights[4, 3] * (float)CurrentCitzFeelings.Money + BrainWeights[4, 4] * (float)CurrentCitzFeelings.FoodAtHome;
        float HomeEatActivation = BrainWeights[5, 0] * (float)CurrentCitzFeelings.Tiredness + BrainWeights[5, 1] * (float)CurrentCitzFeelings.Hungryness + BrainWeights[5, 2] * (float)CurrentCitzFeelings.Hapiness + BrainWeights[5, 3] * (float)CurrentCitzFeelings.Money + BrainWeights[5, 4] * (float)CurrentCitzFeelings.FoodAtHome;
        float HomeRestActivation = BrainWeights[6, 0] * (float)CurrentCitzFeelings.Tiredness + BrainWeights[6, 1] * (float)CurrentCitzFeelings.Hungryness + BrainWeights[6, 2] * (float)CurrentCitzFeelings.Hapiness + BrainWeights[6, 3] * (float)CurrentCitzFeelings.Money + BrainWeights[6, 4] * (float)CurrentCitzFeelings.FoodAtHome;

        // Send Through a [0..100] Soft Max, where 0 => 50

        RtnProbSet.GotoOffice = SoftMax100(OfficeActivation);
        RtnProbSet.GoFoodShop = SoftMax100(ShopActivation);
        RtnProbSet.GottoPub = SoftMax100(PubActivation);
        RtnProbSet.GotoResturant = SoftMax100(ResturantActivation);
        RtnProbSet.GotoPark = SoftMax100(ParkActivation);
        RtnProbSet.GoHomeEat = SoftMax100(HomeEatActivation);
        RtnProbSet.GoHomeRest = SoftMax100(HomeRestActivation);

        return RtnProbSet;
    } // ReturnProbabilities
    // ============================================================================================
    int SoftMax100(float Inputvalue)
    {
        // Based upon standard Softmax F(x) =1.0/(1+ exp(-x))
        return (int) (100.0f / (1.0f + Mathf.Exp(-0.01f * Inputvalue)));
    } // SoftMax100

    // ======================================================================================
    public void ReinforceActionWeights(NextActionTypes ActionApplied, CitzAttributes TheCurrentCitzAttributesState, float RewardFactor)
    {
        // Reinforce the Action Weights (Either positive or negative Reward Factor)
        // The Reward Factor adjusted for previous discounted values, by the Citz Caller


        // DeltaWeight = LearningRate*(CitzState-50.0)*RewardFactor                 /
        float DeltaTirednessWeight = LearningRate * ((float)TheCurrentCitzAttributesState.Tiredness - 50.0f) * RewardFactor;
        float DeltaHungrynessWeight = LearningRate * ((float)TheCurrentCitzAttributesState.Hungryness - 50.0f) * RewardFactor;
        float DeltaHapinessWeight = LearningRate * ((float)TheCurrentCitzAttributesState.Hapiness) * RewardFactor;
        float DeltaMoneyWeight = LearningRate * ((float)TheCurrentCitzAttributesState.Money - 50.0f) * RewardFactor;
        float DeltaFoodWeight = LearningRate * ((float)TheCurrentCitzAttributesState.FoodAtHome - 50.0f) * RewardFactor;
       
        if(ActionApplied == NextActionTypes.GotoOfficeWork)
        {
            BrainWeights[0, 0] = BrainWeights[0, 0] + DeltaTirednessWeight;
            BrainWeights[0, 1] = BrainWeights[0, 1] + DeltaHungrynessWeight;
            BrainWeights[0, 2] = BrainWeights[0, 2] + DeltaHapinessWeight;
            BrainWeights[0, 3] = BrainWeights[0, 3] + DeltaMoneyWeight;
            BrainWeights[0, 4] = BrainWeights[0, 4] + DeltaFoodWeight;
        } // GotoOfficeWork

        if (ActionApplied == NextActionTypes.GoFoodShoping)
        {
            BrainWeights[1, 0] = BrainWeights[1, 0] + DeltaTirednessWeight;
            BrainWeights[1, 1] = BrainWeights[1, 1] + DeltaHungrynessWeight;
            BrainWeights[1, 2] = BrainWeights[1, 2] + DeltaHapinessWeight;
            BrainWeights[1, 3] = BrainWeights[1, 3] + DeltaMoneyWeight;
            BrainWeights[1, 4] = BrainWeights[1, 4] + DeltaFoodWeight;
        } // GoFoodShoping

        if (ActionApplied == NextActionTypes.GotoPub)
        {
            BrainWeights[2, 0] = BrainWeights[2, 0] + DeltaTirednessWeight;
            BrainWeights[2, 1] = BrainWeights[2, 1] + DeltaHungrynessWeight;
            BrainWeights[2, 2] = BrainWeights[2, 2] + DeltaHapinessWeight;
            BrainWeights[2, 3] = BrainWeights[2, 3] + DeltaMoneyWeight;
            BrainWeights[2, 4] = BrainWeights[2, 4] + DeltaFoodWeight;
        } //  GotoPub

        if (ActionApplied == NextActionTypes.GotoResturantToEat)
        {
            BrainWeights[3, 0] = BrainWeights[3, 0] + DeltaTirednessWeight;
            BrainWeights[3, 1] = BrainWeights[3, 1] + DeltaHungrynessWeight;
            BrainWeights[3, 2] = BrainWeights[3, 2] + DeltaHapinessWeight;
            BrainWeights[3, 3] = BrainWeights[3, 3] + DeltaMoneyWeight;
            BrainWeights[3, 4] = BrainWeights[3, 4] + DeltaFoodWeight;
        } // GotoResturantToEat

        if (ActionApplied == NextActionTypes.GotoPark)
        {
            BrainWeights[4, 0] = BrainWeights[4, 0] + DeltaTirednessWeight;
            BrainWeights[4, 1] = BrainWeights[4, 1] + DeltaHungrynessWeight;
            BrainWeights[4, 2] = BrainWeights[4, 2] + DeltaHapinessWeight;
            BrainWeights[4, 3] = BrainWeights[4, 3] + DeltaMoneyWeight;
            BrainWeights[4, 4] = BrainWeights[4, 4] + DeltaFoodWeight;
        } //  GotoPark

        if (ActionApplied == NextActionTypes.GoHomeEat)
        {
            BrainWeights[5, 0] = BrainWeights[5, 0] + DeltaTirednessWeight;
            BrainWeights[5, 1] = BrainWeights[5, 1] + DeltaHungrynessWeight;
            BrainWeights[5, 2] = BrainWeights[5, 2] + DeltaHapinessWeight;
            BrainWeights[5, 3] = BrainWeights[5, 3] + DeltaMoneyWeight;
            BrainWeights[5, 4] = BrainWeights[5, 4] + DeltaFoodWeight;
        } // GoHomeEat

        if (ActionApplied == NextActionTypes.GoHomeToRest)
        {
            BrainWeights[6, 0] = BrainWeights[6, 0] + DeltaTirednessWeight;
            BrainWeights[6, 1] = BrainWeights[6, 1] + DeltaHungrynessWeight;
            BrainWeights[6, 2] = BrainWeights[6, 2] + DeltaHapinessWeight;
            BrainWeights[6, 3] = BrainWeights[6, 3] + DeltaMoneyWeight;
            BrainWeights[6, 4] = BrainWeights[6, 4] + DeltaFoodWeight;
        } // GoHomeToRest

    } // ReinforceActionWeights

    // ============================================================================================
    public NextActionTypes RequestNextAction(CitzAttributes TheCurrentCitzAttributes)
    {
        // Return the Next Action
        // Forward Pass the Current Citz Attributes through a Matrix Multiply and (0:100) Softmax Activation to Calculate Relative probabilties
        // Sample Action from porbability Distribution Set. 

        // Prob Distribution range [0..100] for each Action Element [GotoOfficeWork, GoFoodShoping,GotoPub,GotoResturantToEat,GotoPark,GoHomeEat,GoHomeToRest] 
        // Full Distribtion Length = Sum(EachAction Distribution)
        // Select a Random in from [0..Length] 

        // Forward Pass of Current Citz feelings => Probabilities 
        ProbsSet CalculatedProbabilities = ReturnCurrentProbabilities(TheCurrentCitzAttributes); 

        // Impose Some Hard Constraints - On Costs - If Cannot afford then Probs of Action => Zero Not Possible
        if (TheCurrentCitzAttributes.Money < -PubParameters.MoneyImpact) CalculatedProbabilities.GottoPub = 0;
        if (TheCurrentCitzAttributes.Money < -FoodShopParameters.MoneyImpact) CalculatedProbabilities.GoFoodShop = 0;
        if (TheCurrentCitzAttributes.Money < -ResturantParameters.MoneyImpact) CalculatedProbabilities.GotoResturant = 0;
        if (TheCurrentCitzAttributes.FoodAtHome < -HomeParameters.FoodImpact) CalculatedProbabilities.GoHomeEat = 0;


        int TotalDistributionLength = CalculatedProbabilities.TotalSum();

        // Note Can Saturate => ALL probs end up Being Zero, No Distribution
        // Fudge Reset Random 
        if(TotalDistributionLength<5)
        {
            Debug.Log("[WARNING]: Probablites Saturated all to Zero - return  Uniform Choice");
            CalculatedProbabilities.GotoOffice = 20;
            CalculatedProbabilities.GoFoodShop = 20;
            CalculatedProbabilities.GottoPub = 20;
            CalculatedProbabilities.GotoResturant = 20;
            CalculatedProbabilities.GotoPark = 20;
            CalculatedProbabilities.GoHomeEat = 20;
            CalculatedProbabilities.GoHomeRest = 20;

            TotalDistributionLength = CalculatedProbabilities.TotalSum();
        } // Saturated Probabilities 

        int RandomActionChoice = Random.Range(0, TotalDistributionLength);

        // Calculate Realtive Prob Thresholds
        int OfficeThreshold = CalculatedProbabilities.GotoOffice;
        int GoFoodShopingTheshold = OfficeThreshold + CalculatedProbabilities.GoFoodShop;
        int GotoPubThreshold = GoFoodShopingTheshold + CalculatedProbabilities.GottoPub;
        int GotoResturantToEatThreshold = GotoPubThreshold + CalculatedProbabilities.GotoResturant;
        int GotoParkThreshold = GotoResturantToEatThreshold + CalculatedProbabilities.GotoPark;
        int GoHomeEatThreshold = GotoParkThreshold + CalculatedProbabilities.GoHomeEat;
        
        // Select Action from the Random Choice within Thresholds
        if (RandomActionChoice <= OfficeThreshold) return NextActionTypes.GotoOfficeWork;
        if ((RandomActionChoice > OfficeThreshold) && (RandomActionChoice<= GoFoodShopingTheshold)) return NextActionTypes.GoFoodShoping;
        if ((RandomActionChoice > GoFoodShopingTheshold) && (RandomActionChoice <= GotoPubThreshold)) return NextActionTypes.GotoPub;
        if ((RandomActionChoice > GotoPubThreshold) && (RandomActionChoice <= GotoResturantToEatThreshold)) return NextActionTypes.GotoResturantToEat;
        if ((RandomActionChoice > GotoResturantToEatThreshold) && (RandomActionChoice <= GotoParkThreshold)) return NextActionTypes.GotoPark;
        if ((RandomActionChoice > GotoParkThreshold) && (RandomActionChoice <= GoHomeEatThreshold)) return NextActionTypes.GoHomeEat;
        if ((RandomActionChoice > GoHomeEatThreshold)) return NextActionTypes.GoHomeToRest;

        Debug.Log("[ERROR]: Have Not Allocated a viable Action : " + RandomActionChoice.ToString()); 

        return NextActionTypes.None;
    } // RequestNextAction
    // ============================================================================================
    public void SaveBrainWeights()
    {
        Debug.Log("Saving Brain Weights To File");

        string FilePath = "Assets/Brain/BrainFile.txt";
        BrainWeightsWriter = new StreamWriter(FilePath, false);

        BrainWeightsWriter.WriteLine(BrainWeights[0, 0].ToString() + ", " + BrainWeights[0, 1] + ", " + BrainWeights[0, 2] + ", " + BrainWeights[0, 3] + ", " + BrainWeights[0, 4]);
        BrainWeightsWriter.WriteLine(BrainWeights[1, 0].ToString() + ", " + BrainWeights[1, 1] + ", " + BrainWeights[1, 2] + ", " + BrainWeights[1, 3] + ", " + BrainWeights[1, 4]);
        BrainWeightsWriter.WriteLine(BrainWeights[2, 0].ToString() + ", " + BrainWeights[2, 1] + ", " + BrainWeights[2, 2] + ", " + BrainWeights[2, 3] + ", " + BrainWeights[2, 4]);
        BrainWeightsWriter.WriteLine(BrainWeights[3, 0].ToString() + ", " + BrainWeights[3, 1] + ", " + BrainWeights[3, 2] + ", " + BrainWeights[3, 3] + ", " + BrainWeights[3, 4]);
        BrainWeightsWriter.WriteLine(BrainWeights[4, 0].ToString() + ", " + BrainWeights[4, 1] + ", " + BrainWeights[4, 2] + ", " + BrainWeights[4, 3] + ", " + BrainWeights[4, 4]);
        BrainWeightsWriter.WriteLine(BrainWeights[5, 0].ToString() + ", " + BrainWeights[5, 1] + ", " + BrainWeights[5, 2] + ", " + BrainWeights[5, 3] + ", " + BrainWeights[5, 4]);
        BrainWeightsWriter.WriteLine(BrainWeights[6, 0].ToString() + ", " + BrainWeights[6, 1] + ", " + BrainWeights[6, 2] + ", " + BrainWeights[6, 3] + ", " + BrainWeights[6, 4]);

        BrainWeightsWriter.Close(); 
    } // SaveBrainWeights
    // ==============================================================================================
    public void LoadBrainWeights()
    {
        Debug.Log("Reading in Brain Weights From File");

        string FilePath = "Assets/Brain/BrainFile.txt";
        BrainWeightsReader = new StreamReader(FilePath);

        string[] Row0 = BrainWeightsReader.ReadLine().Split(char.Parse(","));
        string[] Row1 = BrainWeightsReader.ReadLine().Split(char.Parse(","));
        string[] Row2 = BrainWeightsReader.ReadLine().Split(char.Parse(","));
        string[] Row3 = BrainWeightsReader.ReadLine().Split(char.Parse(","));
        string[] Row4 = BrainWeightsReader.ReadLine().Split(char.Parse(","));
        string[] Row5 = BrainWeightsReader.ReadLine().Split(char.Parse(","));
        string[] Row6 = BrainWeightsReader.ReadLine().Split(char.Parse(","));

        BrainWeightsReader.Close();

        // Now Fill out the Brain Weights
        BrainWeights[0, 0] = float.Parse(Row0[0]);
        BrainWeights[0, 1] = float.Parse(Row0[1]);
        BrainWeights[0, 2] = float.Parse(Row0[2]);
        BrainWeights[0, 3] = float.Parse(Row0[3]);
        BrainWeights[0, 4] = float.Parse(Row0[4]);

        BrainWeights[1, 0] = float.Parse(Row1[0]);
        BrainWeights[1, 1] = float.Parse(Row1[1]);
        BrainWeights[1, 2] = float.Parse(Row1[2]);
        BrainWeights[1, 3] = float.Parse(Row1[3]);
        BrainWeights[1, 4] = float.Parse(Row1[4]);

        BrainWeights[2, 0] = float.Parse(Row2[0]);
        BrainWeights[2, 1] = float.Parse(Row2[1]);
        BrainWeights[2, 2] = float.Parse(Row2[2]);
        BrainWeights[2, 3] = float.Parse(Row2[3]);
        BrainWeights[2, 4] = float.Parse(Row2[4]);

        BrainWeights[3, 0] = float.Parse(Row3[0]);
        BrainWeights[3, 1] = float.Parse(Row3[1]);
        BrainWeights[3, 2] = float.Parse(Row3[2]);
        BrainWeights[3, 3] = float.Parse(Row3[3]);
        BrainWeights[3, 4] = float.Parse(Row3[4]);

        BrainWeights[4, 0] = float.Parse(Row4[0]);
        BrainWeights[4, 1] = float.Parse(Row4[1]);
        BrainWeights[4, 2] = float.Parse(Row4[2]);
        BrainWeights[4, 3] = float.Parse(Row4[3]);
        BrainWeights[4, 4] = float.Parse(Row4[4]);

        BrainWeights[5, 0] = float.Parse(Row5[0]);
        BrainWeights[5, 1] = float.Parse(Row5[1]);
        BrainWeights[5, 2] = float.Parse(Row5[2]);
        BrainWeights[5, 3] = float.Parse(Row5[3]);
        BrainWeights[5, 4] = float.Parse(Row5[4]);

        BrainWeights[6, 0] = float.Parse(Row6[0]);
        BrainWeights[6, 1] = float.Parse(Row6[1]);
        BrainWeights[6, 2] = float.Parse(Row6[2]);
        BrainWeights[6, 3] = float.Parse(Row6[3]);
        BrainWeights[6, 4] = float.Parse(Row6[4]);
    } // LoadBrainWeights


    // =================================================================================================
} // BrainManager
