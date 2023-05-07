using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserInterfaceController : MonoBehaviour
{
    public GameObject TheCitzPanel;

    public TMP_Text NameTB;
    public TMP_Text TirenessTB;
    public TMP_Text HungrynessTB;
    public TMP_Text HapinessTB;
    public TMP_Text MoneyTB;
    public TMP_Text FoodAtHomeTB;

    public TMP_Text NextActionTB;

    public TMP_Text DisplayedAverageRewardTB; 

    void Start()
    {
        TheCitzPanel.gameObject.SetActive(false);
    }
    // ================================================================================================
    // Update is called once per frame
    void Update()
    {
        

    }

    // =================================================================================================
    public void EnableDisableCitzPanelDisplay(bool ReqDisplay)
    {
        TheCitzPanel.gameObject.SetActive(ReqDisplay);

    } // EnableDisableCitzPanelDisplay
    // ==================================================================================================
    public void UpdateCitzPanelPosition(Vector3 TheTargetWorldPos, CitzAttributes TheCitzFeelings, BrainManager.NextActionTypes TheCitzAction)
    {
        TheCitzPanel.transform.position = Camera.main.WorldToScreenPoint(TheTargetWorldPos + new Vector3(0.0f, 3.0f, 0.0f));

        NameTB.text = TheCitzFeelings.Name;
        TirenessTB.text = TheCitzFeelings.Tiredness.ToString();
        HungrynessTB.text = TheCitzFeelings.Hungryness.ToString();
        HapinessTB.text = TheCitzFeelings.Hapiness.ToString();
        MoneyTB.text = TheCitzFeelings.Money.ToString(); 
        FoodAtHomeTB.text = TheCitzFeelings.FoodAtHome.ToString();

        if (TheCitzAction == BrainManager.NextActionTypes.GotoOfficeWork) NextActionTB.text = "Going To Office";
        if (TheCitzAction == BrainManager.NextActionTypes.GoHomeEat) NextActionTB.text = "Going Home To Eat";
        if (TheCitzAction == BrainManager.NextActionTypes.GoHomeToRest) NextActionTB.text = "Going Home To Rest";
        if (TheCitzAction == BrainManager.NextActionTypes.GotoPark) NextActionTB.text = "Going To Park";
        if (TheCitzAction == BrainManager.NextActionTypes.GotoPub) NextActionTB.text = "Going To Pub";
        if (TheCitzAction == BrainManager.NextActionTypes.GotoResturantToEat) NextActionTB.text = "Going To Restaurant";
        if (TheCitzAction == BrainManager.NextActionTypes.GoFoodShoping) NextActionTB.text = "Going To Food Shop";

    } // UpdateCitzPanelPosition
    // =================================================================================================
    public void UpdateDisplayedAverageReward(float TheRewardValue)
    {
        DisplayedAverageRewardTB.text = ((int)TheRewardValue).ToString();

    } // UpdateDisplayedAverageReward
    // =================================================================================================
}
