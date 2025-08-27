using UnityEngine;


[CreateAssetMenu(fileName = "New Chance Card", menuName = "Monopoly/Cards/Chance")]
public class SCR_ChanceCard : ScriptableObject
{
    public string textOnCard; // Description
    public int rewardMoney;   // GET MONEY
    public int penaltyMoney;  // PAY MONEY
    public int moveToBoardIndex = -1;
    public bool payToPlayer;
    [Header("Mover a lugares")]
    public bool nextRailroad;
    public bool nextUtility;
    public int moveStepsBackwards;


    [Header("Cosas de carcel")]
    public bool goToJail;
    public bool jailFreeCard;

    [Header("Pago de casas y hoteles")]
    public bool streetRepairs;
    public int streetRepairsHousePrice = 25;
    public int streetRepairsHotelPrice = 100;
}
