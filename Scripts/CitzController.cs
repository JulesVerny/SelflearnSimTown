using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CitzController : MonoBehaviour
{
    // ==============================================================================
    public enum CitzStatus { None, Walking, RestingAtHome,EatingAtHome, WorkingAtOffice, ShoppingForFood, DrinkingAtPub,EatingAtResturant,RelaxingInPark,Fainted}
    public enum CitzSex { None, Male, Female};

    public enum CitzHairColour {None,Grey,LightBrown,DarkBrown }
    public enum CitzClothColour { None, Blue,Red,Green,DarkBrown,Yellow,DarkGreen} 

    // ============================================================================
    public CitzStatus TheCitzStatus;
    public CitzAttributes TheCurrentCitzFeelings;

    private SiteManager TheSiteManager;
    private UserInterfaceController TheUserInterface;
    private BrainManager TheBrainManager;
    private ExperimentManager TheExperimentManager; 

    public GameObject TheMaleGO;
    public GameObject TheFemaleGO;

    public GameObject TheMaleHeadMeshGO;
    public GameObject TheMaleBodyMeshGO;
    public GameObject TheFemaleHeadMeshGO;
    public GameObject TheFemaleBodyMeshGO;

    private Animator TheMaleAnimator;
    private Animator TheFemaleAnimator;

    private int ActivityCountDown;
    private NavMeshAgent TheNavMeshAgent;

    private CitzSex TheCitzSex;
    private bool CitzSelected;

    private SiteController CurrentOccupiedSite;
    private BrainManager.NextActionTypes LastRequestedAction;
    private float TheCurrentReward;

    private float PrevReward, Prev2Reward;
    private CitzAttributes PrevFeelings, Prev2Feelings;
    private BrainManager.NextActionTypes PrevAction, Prev2Action; 


    private float DestinationReachedThreshold = 1.25f;
    private float CitzWalkSpeed = 1.75f;  
    private bool MovedOnFromDestination;


    // Various colours
    public Material GreyMaterial;
    public Material DarkBrownMaterial;
    public Material LightBrownMaterial;
    public Material GreenMaterial;
    public Material BlueMaterial;
    public Material RedMaterial;
    public Material YellowMaterial;
    public Material DarkGreenMaterial;


    // ============================================================================
    private void Awake()
    {
        TheCitzStatus = CitzStatus.None;
        ActivityCountDown = 10;                     // Need to be ready to do that first Action - Assume within 'Town Hall'
        TheNavMeshAgent = GetComponent<NavMeshAgent>();
        TheMaleAnimator = TheMaleGO.GetComponent<Animator>();
        TheFemaleAnimator = TheFemaleGO.GetComponent<Animator>();

        TheMaleGO.SetActive(false);
        TheFemaleGO.SetActive(false);
        CitzSelected = false;

        TheCurrentCitzFeelings = new CitzAttributes();
        LastRequestedAction = BrainManager.NextActionTypes.None;
        TheCurrentReward = 0.0f; 

        CurrentOccupiedSite = null; 
        MovedOnFromDestination = false;
        LastRequestedAction = BrainManager.NextActionTypes.None;

        // Set Up Previous History
        PrevReward = 0.0f;
        Prev2Reward = 0.0f;
        PrevFeelings = new CitzAttributes();
        Prev2Feelings = new CitzAttributes();
        PrevAction = BrainManager.NextActionTypes.None;
        Prev2Action = BrainManager.NextActionTypes.None;



} // Awake
  // ============================================================================
void Start()
    {
        // Note NOT Called if Instantiated by prefab !!

    } // Start
    // ============================================================================
    public void InitialiseCitz(ExperimentManager AExperimentManager, SiteManager ASiteManager, UserInterfaceController AUserInterfaceRef, BrainManager ABrainManager, string ACitzName, CitzSex TheReqSex, CitzHairColour TheHairColour, CitzClothColour TopColour, CitzClothColour BottomColour)
    {
        TheCurrentCitzFeelings.Name = ACitzName; 
        TheSiteManager = ASiteManager;
        TheUserInterface = AUserInterfaceRef;
        TheBrainManager = ABrainManager;
        TheExperimentManager = AExperimentManager;

        TheCitzSex = TheReqSex;

        Material[] mats;

        if (TheCitzSex == CitzSex.Male)
        {
            TheMaleGO.SetActive(true);
            TheMaleAnimator.SetTrigger("IsIdle");

            // Set Hair Colour
            mats = TheMaleHeadMeshGO.GetComponent<Renderer>().materials;
            if (TheHairColour == CitzHairColour.Grey) mats[1] = GreyMaterial;
            if (TheHairColour == CitzHairColour.DarkBrown) mats[1] = DarkBrownMaterial;
            if (TheHairColour == CitzHairColour.LightBrown) mats[1] = LightBrownMaterial;
            TheMaleHeadMeshGO.GetComponent<Renderer>().materials = mats;

            // Set Clothes colours
            mats = TheMaleBodyMeshGO.GetComponent<Renderer>().materials;
            if (TopColour == CitzClothColour.Blue) mats[0] = BlueMaterial;
            if (TopColour == CitzClothColour.Red) mats[0] = RedMaterial;
            if (TopColour == CitzClothColour.Green) mats[0] = GreenMaterial;
            if (TopColour == CitzClothColour.Yellow) mats[0] = YellowMaterial;
            if (TopColour == CitzClothColour.DarkBrown) mats[0] = DarkBrownMaterial;
            if (TopColour == CitzClothColour.DarkGreen) mats[0] = DarkGreenMaterial;

            if (BottomColour == CitzClothColour.Blue) mats[1] = BlueMaterial;
            if (BottomColour == CitzClothColour.Red) mats[1] = RedMaterial;
            if (BottomColour == CitzClothColour.Green) mats[1] = GreenMaterial;
            if (BottomColour == CitzClothColour.Yellow) mats[1] = YellowMaterial;
            if (BottomColour == CitzClothColour.DarkBrown) mats[1] = DarkBrownMaterial;
            if (BottomColour == CitzClothColour.DarkGreen) mats[1] = DarkGreenMaterial;

            TheMaleBodyMeshGO.GetComponent<Renderer>().materials = mats;

        } // Male
        if (TheCitzSex == CitzSex.Female)
        {
            TheFemaleGO.SetActive(true);
            TheFemaleAnimator.SetTrigger("IsIdle");

            // Set Hair Colour
            mats = TheFemaleHeadMeshGO.GetComponent<Renderer>().materials;
            if (TheHairColour == CitzHairColour.Grey) mats[2] = GreyMaterial;
            if (TheHairColour == CitzHairColour.DarkBrown) mats[2] = DarkBrownMaterial;
            if (TheHairColour == CitzHairColour.LightBrown) mats[2] = LightBrownMaterial;
            TheFemaleHeadMeshGO.GetComponent<Renderer>().materials = mats;

            // Set Clothes colours
            mats = TheFemaleBodyMeshGO.GetComponent<Renderer>().materials;
            if (TopColour == CitzClothColour.Blue) mats[0] = BlueMaterial;
            if (TopColour == CitzClothColour.Red) mats[0] = RedMaterial;
            if (TopColour == CitzClothColour.Green) mats[0] = GreenMaterial;
            if (TopColour == CitzClothColour.Yellow) mats[0] = YellowMaterial;
            if (TopColour == CitzClothColour.DarkBrown) mats[0] = DarkBrownMaterial;
            if (TopColour == CitzClothColour.DarkGreen) mats[0] = DarkGreenMaterial;

            if (BottomColour == CitzClothColour.Blue) mats[2] = BlueMaterial;
            if (BottomColour == CitzClothColour.Red) mats[2] = RedMaterial;
            if (BottomColour == CitzClothColour.Green) mats[2] = GreenMaterial;
            if (BottomColour == CitzClothColour.Yellow) mats[2] = YellowMaterial;
            if (BottomColour == CitzClothColour.DarkBrown) mats[2] = DarkBrownMaterial;
            if (BottomColour == CitzClothColour.DarkGreen) mats[2] = DarkGreenMaterial;

            TheFemaleBodyMeshGO.GetComponent<Renderer>().materials = mats;
        }  // Female
                
        TheCitzStatus = CitzStatus.None;


    } // InitialiseCitz
    // ============================================================================
    void Update()
    {
        // Any UI controls Here 

        if (CitzSelected) TheUserInterface.UpdateCitzPanelPosition(this.transform.position, TheCurrentCitzFeelings, LastRequestedAction);


    } // Update
    // ============================================================================
    private void FixedUpdate()
    {
        if (TheNavMeshAgent.remainingDistance > DestinationReachedThreshold) MovedOnFromDestination = true; 

            // Check Citz Nav Agent Reached Destination
            if (TheCitzStatus == CitzStatus.Walking)
            {
            if ((TheNavMeshAgent.remainingDistance < DestinationReachedThreshold) && (MovedOnFromDestination))
            {
                TheNavMeshAgent.speed = 0.0f;

                if (LastRequestedAction == BrainManager.NextActionTypes.GotoOfficeWork)
                {
                    SetSitting();
                }
                if (LastRequestedAction == BrainManager.NextActionTypes.GoHomeEat)
                {
                    SetSitting();
                }
                if (LastRequestedAction == BrainManager.NextActionTypes.GoHomeToRest)
                {
                    SetFainting();
                }
                if (LastRequestedAction == BrainManager.NextActionTypes.GotoPark)
                {
                    SetIdle();
                }
                if (LastRequestedAction == BrainManager.NextActionTypes.GoFoodShoping)
                {
                    SetIdle();
                }
                if (LastRequestedAction == BrainManager.NextActionTypes.GotoPub)
                {
                    SetClapping();
                }
                if (LastRequestedAction == BrainManager.NextActionTypes.GotoResturantToEat)
                {
                    SetClapping();
                }

                MovedOnFromDestination = false;

                // ================================================
                // Get the Count Down
                if (CurrentOccupiedSite != null)
                {
                    // Now Process the Consequences of the Actions
                    ActivityCountDown = CurrentOccupiedSite.GetCountDownDelay();

                } // Process Consequences
                // ================================================
            }  // Reached Destination
        }  // If Citz Was Walking



    }  // FixedUpdate
    // ============================================================================
    public void SimTickUpdate()
    {

        // Process any Activity Timeouts
        if(ActivityCountDown>0)
        {
            ActivityCountDown--;
            if(ActivityCountDown==0)
            {
                // The Citz Has Completed the Activity - So can process the Cosequences and Request the Next Action
                // ================================
                if(CurrentOccupiedSite!=null) ProcessTheImpactUponCitz();

                // ================================
                // Now Request the Next Action
                BrainManager.NextActionTypes NextRequestedAction = TheBrainManager.RequestNextAction(TheCurrentCitzFeelings);
                
                // Need to Avoid Last Requested Action (Hopefully go to a Different Place - Still Home Eat/Sleep Action possible conflict 
                while(NextRequestedAction == LastRequestedAction) NextRequestedAction = TheBrainManager.RequestNextAction(TheCurrentCitzFeelings);

                PerformNextAction(NextRequestedAction); 
            }
        } // ActivityCountDown

    } // SimTimeUpdate
    // ==============================================================================================================
    void ProcessTheImpactUponCitz()
    {
        // Update the Characters Feelings
        TheCurrentCitzFeelings = CurrentOccupiedSite.ProcessSiteImpactUponFeelings(TheCurrentCitzFeelings);
        TheCurrentReward = CurrentRewardAssessment();

        // Now Update the Weights in Brain Manager
        TheBrainManager.ReinforceActionWeights(LastRequestedAction, TheCurrentCitzFeelings, TheCurrentReward);

        //  And Update Weights of Previoes Two Actions
        if (PrevAction != BrainManager.NextActionTypes.None) TheBrainManager.ReinforceActionWeights(PrevAction, PrevFeelings, PrevReward);
        if (Prev2Action != BrainManager.NextActionTypes.None) TheBrainManager.ReinforceActionWeights(Prev2Action, Prev2Feelings, Prev2Reward);

        // Update the History Stack 

        Prev2Reward = PrevReward * 0.85f;         // Discount 0f 0.85 applied
        PrevReward = TheCurrentReward * 0.85f;    // Discount 0f 0.85 applied
        Prev2Action = PrevAction;
        PrevAction = LastRequestedAction;
        Prev2Feelings = PrevFeelings;
        PrevFeelings = TheCurrentCitzFeelings;

    } // ProcessTheImpactUponCitz
    // ===============================================================================================================
    public void PerformNextAction(BrainManager.NextActionTypes RequestedNextAction)
    {
        // public enum NextActionTypes { None, GotoOfficeWork, GoFoodShoping,GotoPub,GotoResturantToEat,GotoPark,GoHomeEat,GoHomeToRest}

        // Find the Location of the Requested Action
        Vector3 ProposedLocation = FindNextLocation(RequestedNextAction); 

        // Need to ensure Next Location is Some distance away
        while (Vector3.Distance(ProposedLocation,transform.position) < 5.0f)  ProposedLocation = FindNextLocation(RequestedNextAction);

        LastRequestedAction = RequestedNextAction;

        //Debug.Log("[INFO]: Citz: " + TheCurrentCitzFeelings.Name + " Has been Instructed to: " + RequestedNextAction.ToString());

        if(ProposedLocation.y<500.0f)
        {
            // Then Set Off Walking in Direction of the Site

            SetWalking();
            TheNavMeshAgent.SetDestination(ProposedLocation);
        }
        else
        {
            Debug.Log("[ERROR]:  Have Not Found a Site Location for Citz Action"); 
        }

    } // PerformNextAction
    // ===================================================================================================================================
    float CurrentRewardAssessment()
    {
        // Provide a view on Reward Fn(Current Feelings) 
        float RtnReward = 0.0f;

        // Good Stuff
        if (TheCurrentCitzFeelings.Hapiness >75) RtnReward = 1.0f;
        if ((TheCurrentCitzFeelings.Tiredness < 25) && (TheCurrentCitzFeelings.Hungryness < 25)) RtnReward = 1.0f;
        if (TheCurrentCitzFeelings.Money > 75) RtnReward = 1.0f;

        // Bad Stuff
        if (TheCurrentCitzFeelings.Money < 10) RtnReward = -1.0f;
        if (TheCurrentCitzFeelings.Hapiness < -50) RtnReward = -1.0f;
        if ((TheCurrentCitzFeelings.Tiredness > 80) && (TheCurrentCitzFeelings.Hungryness > 80)) RtnReward = -1.0f;

        return RtnReward; 
    } // RewardAssessment
    // ====================================================================================================================================
    private Vector3 FindNextLocation(BrainManager.NextActionTypes RequestedNextAction)
    {
        Vector3 NextLocation = new Vector3(0.0f, 1000.0f, 0.0f); ;
        if (RequestedNextAction == BrainManager.NextActionTypes.GotoOfficeWork) NextLocation = TheSiteManager.FindSiteLocation(SiteController.SiteTypes.Office);
        if (RequestedNextAction == BrainManager.NextActionTypes.GoFoodShoping) NextLocation = TheSiteManager.FindSiteLocation(SiteController.SiteTypes.SuperMarket);
        if (RequestedNextAction == BrainManager.NextActionTypes.GoHomeEat) NextLocation = TheSiteManager.FindSiteLocation(SiteController.SiteTypes.Home);
        if (RequestedNextAction == BrainManager.NextActionTypes.GoHomeToRest) NextLocation = TheSiteManager.FindSiteLocation(SiteController.SiteTypes.Home);
        if (RequestedNextAction == BrainManager.NextActionTypes.GotoPark) NextLocation = TheSiteManager.FindSiteLocation(SiteController.SiteTypes.Park);
        if (RequestedNextAction == BrainManager.NextActionTypes.GotoPub) NextLocation = TheSiteManager.FindSiteLocation(SiteController.SiteTypes.Pub);
        if (RequestedNextAction == BrainManager.NextActionTypes.GotoResturantToEat) NextLocation = TheSiteManager.FindSiteLocation(SiteController.SiteTypes.Resturant);
        return NextLocation; 
    } // FindNextLocation; 
    // ===================================================================================================================================
    // Citz Mouse Selections
    private void OnMouseDown()
    {
        // Mouse Down Selections
        if (CitzSelected)
        {
            CitzSelected = false;
            TheUserInterface.EnableDisableCitzPanelDisplay(false); 
        }
        else
        {
            CitzSelected = true;
            TheUserInterface.EnableDisableCitzPanelDisplay(true);
        }

       // Debug.Log("[INFO] Citz Selection: " + CitzSelected.ToString());
    } // OnMouseDown
    // ============================================================================
    // Trigger collsions
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            if (other.gameObject.GetComponent<SiteController>() != null)
            {
                //Debug.Log("[INFO] Citz Has Entered : " + other.gameObject.GetComponent<SiteController>().TheSiteType.ToString());
                other.gameObject.GetComponent<SiteController>().SetTransParent();

                CurrentOccupiedSite = other.gameObject.GetComponent<SiteController>();

            }
        }
    } // OnTriggerEnter
    // =======================================================================
    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player")
        {

            if (other.gameObject.GetComponent<SiteController>() != null)
            {
                //Debug.Log("[INFO] Citz Has Left : " + other.gameObject.GetComponent<SiteController>().TheSiteType.ToString());
                other.gameObject.GetComponent<SiteController>().SetOpaque();

                CurrentOccupiedSite = null;
            }
        }
    } // OnTriggerExit
    // ============================================================================
    void SetWalking()
    {
        if (TheCitzSex == CitzSex.Male) TheMaleAnimator.SetTrigger("IsWalking");
        if (TheCitzSex == CitzSex.Female) TheFemaleAnimator.SetTrigger("IsWalking");

        if(TheExperimentManager.TheSpeedFactor == ExperimentManager.SpeedFactors.Normal) TheNavMeshAgent.speed = CitzWalkSpeed;
        if (TheExperimentManager.TheSpeedFactor == ExperimentManager.SpeedFactors.Fast) TheNavMeshAgent.speed = 5.0f*CitzWalkSpeed;
     
        TheCitzStatus = CitzStatus.Walking;
    } // SetWalking
    // ============================
    void SetSitting()
    {
        if (TheCitzSex == CitzSex.Male) TheMaleAnimator.SetTrigger("IsSitting");
        if (TheCitzSex == CitzSex.Female) TheFemaleAnimator.SetTrigger("IsSitting");
    } // SetSitting
    // ============================
    void SetIdle()
    {
        if (TheCitzSex == CitzSex.Male) TheMaleAnimator.SetTrigger("IsIdle");
        if (TheCitzSex == CitzSex.Female) TheFemaleAnimator.SetTrigger("IsIdle");
    } // SetIdle
    // ============================
    void SetClapping()
    {
        if (TheCitzSex == CitzSex.Male) TheMaleAnimator.SetTrigger("IsClapping");
        if (TheCitzSex == CitzSex.Female) TheFemaleAnimator.SetTrigger("IsClapping");
    } // SetFainting

    void SetFainting()
    {
        if (TheCitzSex == CitzSex.Male) TheMaleAnimator.SetTrigger("IsFainting");
        if (TheCitzSex == CitzSex.Female) TheFemaleAnimator.SetTrigger("IsFainting");
    } // SetFainting
    // =================================================================================
}
