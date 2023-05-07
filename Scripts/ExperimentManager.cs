using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperimentManager : MonoBehaviour
{
    public enum SpeedFactors { Normal, Fast}
    // ======================================================================================
    
    public List<CitzController> TheListOfCitz;
    public BrainManager TheBrainManager;
    public SiteManager TheSiteManager;
    public GameObject CitzPrefab; 

    private UserInterfaceController TheUserInterface;

    public SpeedFactors TheSpeedFactor; 

    private int SimClock = 0; 
    // ======================================================================================
    private void Awake()
    {
        TheListOfCitz = new List<CitzController>();

        TheUserInterface = GetComponent<UserInterfaceController>();

        TheSpeedFactor = SpeedFactors.Normal; 

    } // Awake
    // ======================================================================================
    void Start()
    {
        SimClock = 0; 


    } // Start
    // ======================================================================================
    // Update is called once per frame
    void Update()
    {

        // UI Interventions - e.g. For Debug

        // Check if Quit Application
        if (Input.GetKey(KeyCode.Escape))
        {
            Debug.Log("[INFO]: Exit App by Escape Button");
            Application.Quit();
        }  // Escape App Check 

        // =================================================
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Create the Whole Set of Citzens

            //Debug.Log("[INFO]: Whole Set of Citzens");

            CreateACitzen(CitzController.CitzSex.Female, "GrandMa");
            CreateACitzen(CitzController.CitzSex.Male, "GranDad");
            CreateACitzen(CitzController.CitzSex.Female, "Juliet");
            CreateACitzen(CitzController.CitzSex.Male, "Thomas");
            CreateACitzen(CitzController.CitzSex.Male, "Leo");
            CreateACitzen(CitzController.CitzSex.Male, "Max");

            CreateACitzen(CitzController.CitzSex.Female, "GrandMa");
            CreateACitzen(CitzController.CitzSex.Male, "GranDad");
            CreateACitzen(CitzController.CitzSex.Female, "Juliet");
            CreateACitzen(CitzController.CitzSex.Male, "Thomas");
            CreateACitzen(CitzController.CitzSex.Male, "Leo");
            CreateACitzen(CitzController.CitzSex.Male, "Max");

        }  //KeyCode.C
        // ================================================
        // =================================================
        if (Input.GetKeyDown(KeyCode.N))
        {
            //Debug.Log("[INFO]: Setting Normal Simulation Speed ");
            TheSpeedFactor = SpeedFactors.Normal;
        }  //KeyCode.N
        if (Input.GetKeyDown(KeyCode.F))
        {
            //Debug.Log("[INFO]: Setting Fast Simulation Speed ");
            TheSpeedFactor = SpeedFactors.Fast;
        }  //KeyCode.F

        // ==================================================
        if (Input.GetKeyDown(KeyCode.S))
        {
            TheBrainManager.SaveBrainWeights();
        }  //KeyCode.S
        if (Input.GetKeyDown(KeyCode.R))
        {
            TheBrainManager.LoadBrainWeights();
        }  //KeyCode.R

        // ====================================================
        

    } // Update
    // ======================================================================================
    private void FixedUpdate()
    {

        // Maintain a Sim Time Click   function of SimSpeed Factor
        if(SimClock == 0)
        {
            // Update the CitZ Sim Time
            foreach(CitzController ACitz in TheListOfCitz)
            {
                ACitz.SimTickUpdate();
            }
            UpdateDisplayedReward();
            if (TheSpeedFactor == SpeedFactors.Fast) SimClock = 2;
            else SimClock = 10;
        }
        SimClock--;

        // =======================================

    } // FixedUpdate
    // ======================================================================================
    void CreateACitzen (CitzController.CitzSex WhichSex, string CitzName)
    {
        // Instantiate a Citz Prefab At the Town Hall 
        GameObject TheCitzGO = Instantiate(CitzPrefab, new Vector3(-38.0f,0.0f,-10.0f), Quaternion.Euler(0.0f, 0.0f, 0.0f));
        CitzController TheCitzController = TheCitzGO.GetComponent<CitzController>();

        // Initilaise the Citz
        //If Juliet => Light Brown Hair, with Red Top and Dark green Bottom
        if(CitzName =="Juliet") TheCitzController.InitialiseCitz(this, TheSiteManager, TheUserInterface, TheBrainManager, CitzName, WhichSex,CitzController.CitzHairColour.LightBrown,CitzController.CitzClothColour.Red,CitzController.CitzClothColour.DarkGreen);

        // If Thomas => Dark Brown Hair, Blue Top and Drak green Bottom
        if (CitzName == "Thomas") TheCitzController.InitialiseCitz(this, TheSiteManager, TheUserInterface, TheBrainManager, CitzName, WhichSex, CitzController.CitzHairColour.DarkBrown, CitzController.CitzClothColour.Blue, CitzController.CitzClothColour.DarkGreen);

        // If Leo => Light Brown Hair, Green Top and Yellow Bottom
        if (CitzName == "Leo") TheCitzController.InitialiseCitz(this, TheSiteManager, TheUserInterface, TheBrainManager, CitzName, WhichSex, CitzController.CitzHairColour.LightBrown, CitzController.CitzClothColour.Green, CitzController.CitzClothColour.Yellow);

        // If Max => Dark Brown Hair, Red Top and Dark Brown Bottom
        if (CitzName == "Max") TheCitzController.InitialiseCitz(this,TheSiteManager, TheUserInterface, TheBrainManager, CitzName, WhichSex, CitzController.CitzHairColour.DarkBrown, CitzController.CitzClothColour.Red, CitzController.CitzClothColour.DarkBrown);

        // If Grandad => Grey Hair, Dark Green Top and Dark Brown Bottom
        if (CitzName == "GranDad") TheCitzController.InitialiseCitz(this, TheSiteManager, TheUserInterface, TheBrainManager, CitzName, WhichSex, CitzController.CitzHairColour.Grey, CitzController.CitzClothColour.DarkGreen, CitzController.CitzClothColour.DarkBrown);

        //If GrandMa => Grey Hair, with Dark Brown Top and Dark green Bottom
        if (CitzName == "GrandMa") TheCitzController.InitialiseCitz(this, TheSiteManager, TheUserInterface, TheBrainManager, CitzName, WhichSex, CitzController.CitzHairColour.Grey, CitzController.CitzClothColour.DarkBrown, CitzController.CitzClothColour.DarkGreen);


        // Add into the List of Citz
        TheListOfCitz.Add(TheCitzController); 


    } // CreateACitzen
    // ======================================================================================
    private void UpdateDisplayedReward()
    {
        float DisplayedRewardValue = 0.0f;

        foreach (CitzController ACitz in TheListOfCitz)
        {
            DisplayedRewardValue = DisplayedRewardValue + 100.0f * ACitz.MovingAveragReward;
        }
        DisplayedRewardValue = DisplayedRewardValue / 8.0f;

        TheUserInterface.UpdateDisplayedAverageReward(DisplayedRewardValue);

    } // UpdateDisplayedReward

    // ======================================================================================

}
