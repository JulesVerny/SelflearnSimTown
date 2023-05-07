using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiteController : MonoBehaviour
{
    // ============================================================
    public enum SiteTypes {None, Home,Office,SuperMarket,Pub,Park,Resturant}

    // =============================================================
    public SiteTypes TheSiteType;
    public Transform TheGoToPoint;
    public SiteConfiguration TheSiteConfiguration;
    private List<CitzController> HostedCitizensList;
    public GameObject BuildingMesh; 
    public Material MainMaterial;
    public Material TransparentMainMaterial;
    public Material GreenTilesMaterial;
    public Material TransparentTransParentMaterial;


    // =============================================================
    private void Awake()
    {

        HostedCitizensList = new List<CitzController>();

    } // Awake
    // ===================================================================
    void Start()
    {

        SetOpaque(); 


    } // Start
    // ===================================================================



    // =======================================================================
    void FixedUpdate()
    {



    } // FixedUpdate
    // =======================================================================
    public void SimTickUpdate()
    {


    } // SimTimeUpdate
    // =======================================================================
    public int GetCountDownDelay()
    {
        return TheSiteConfiguration.DwellPeriod;
    } // GetCountDownDelay

    // =======================================================================
    public CitzAttributes ProcessSiteImpactUponFeelings(CitzAttributes CurrentFeelings)
    {
        CitzAttributes TheRevisedFeelings = CurrentFeelings;

        
        TheRevisedFeelings.Tiredness = CurrentFeelings.Tiredness + TheSiteConfiguration.TirednessImpact;
        if (TheRevisedFeelings.Tiredness < 0) TheRevisedFeelings.Tiredness = 0;
        if (TheRevisedFeelings.Tiredness >100) TheRevisedFeelings.Tiredness = 100;

        TheRevisedFeelings.Hapiness = CurrentFeelings.Hapiness + TheSiteConfiguration.HapinessImpact;
        if (TheRevisedFeelings.Hapiness < -100) TheRevisedFeelings.Hapiness = -100;
        if (TheRevisedFeelings.Hapiness > 100) TheRevisedFeelings.Hapiness = 100;

        TheRevisedFeelings.Hungryness = CurrentFeelings.Hungryness + TheSiteConfiguration.HungrenessImpact;
        if (TheRevisedFeelings.Hungryness < 0) TheRevisedFeelings.Hungryness = 0;
        if (TheRevisedFeelings.Hungryness > 100) TheRevisedFeelings.Hungryness = 100;

        TheRevisedFeelings.Money = CurrentFeelings.Money + TheSiteConfiguration.MoneyImpact;
        if (TheRevisedFeelings.Money < 0) TheRevisedFeelings.Money = 0;
        if (TheRevisedFeelings.Money > 100) TheRevisedFeelings.Money = 100;

        TheRevisedFeelings.FoodAtHome = CurrentFeelings.FoodAtHome + TheSiteConfiguration.FoodImpact;
        if (TheRevisedFeelings.FoodAtHome < 0) TheRevisedFeelings.FoodAtHome = 0;
        if (TheRevisedFeelings.FoodAtHome > 100) TheRevisedFeelings.FoodAtHome = 100;

        return TheRevisedFeelings; 
    } // ProcessSiteImpactUponFeelings
    // =======================================================================
    public void SetOpaque()
    {
        Material[] mats; 
        mats = BuildingMesh.GetComponent<Renderer>().materials;
        mats[0] = MainMaterial;
        BuildingMesh.GetComponent<Renderer>().materials = mats;

        if (TheSiteType == SiteTypes.Office)
        {
            mats = BuildingMesh.GetComponent<Renderer>().materials;
            mats[0] = MainMaterial;
            mats[1] = MainMaterial;
            BuildingMesh.GetComponent<Renderer>().materials = mats;
        }
        if (TheSiteType == SiteTypes.Resturant)
        {
            mats = BuildingMesh.GetComponent<Renderer>().materials;
            mats[0] = MainMaterial;
            mats[1] = GreenTilesMaterial;
            BuildingMesh.GetComponent<Renderer>().materials = mats;
        }
    } // SetOpaque
    // ===================================================================
    public void SetTransParent()
    {
        Material[] mats; 
        if (TheSiteType != SiteTypes.Park)
        {
            mats = BuildingMesh.GetComponent<Renderer>().materials;
            mats[0] = TransparentMainMaterial;
            BuildingMesh.GetComponent<Renderer>().materials = mats;
        } // Not Park
        if (TheSiteType == SiteTypes.Office)
        {
            mats = BuildingMesh.GetComponent<Renderer>().materials;
            mats[0] = TransparentMainMaterial;
            mats[1] = TransparentMainMaterial;
            BuildingMesh.GetComponent<Renderer>().materials = mats;
        } // Office 

        if (TheSiteType == SiteTypes.Resturant)
        {
            mats = BuildingMesh.GetComponent<Renderer>().materials;
            mats[0] = TransparentMainMaterial;
            mats[1] = TransparentTransParentMaterial;
            BuildingMesh.GetComponent<Renderer>().materials = mats;
        } // Resturant Materials

    } // SetTransParent

    // ======================================================================================================================
} // 
