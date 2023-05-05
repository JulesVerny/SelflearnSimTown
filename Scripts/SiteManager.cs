using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SiteManager : MonoBehaviour
{
    public List<SiteController> TheListOfSites;

    // =========================================================================


    // =========================================================================
    private void Awake()
    {

    } // Awake

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // ============================================================================
    // Update is called once per frame
    void Update()
    {
      
    }


    // ===============================================================================
    public Vector3 FindSiteLocation(SiteController.SiteTypes TheReqSiteType)
    {
        List<SiteController> PossibleSites = new List<SiteController>();
        int FoundRef;

        foreach (SiteController ASiteObjectControl in TheListOfSites)
        {
            if(ASiteObjectControl.TheSiteType== TheReqSiteType)
            {
                PossibleSites.Add(ASiteObjectControl);
            }
        }
        int NumberPossibleSites = PossibleSites.Count; 
        if(NumberPossibleSites>0)
        {
            FoundRef = UnityEngine.Random.Range(0, NumberPossibleSites); 

        }
        else 
        {
            Debug.Log("[ERROR]:  Nop Offices Found");
            throw new KeyNotFoundException();
        }
        return PossibleSites[FoundRef].TheGoToPoint.position; 
    } // FindOfficeLocation



    // ================================================================================
}
