using UnityEngine;



[CreateAssetMenu(fileName = "New Community Card", menuName = "Monopoly/Cards/Community")]
public class SRC_CommunityCard : ScriptableObject
{
    public string textOnCard; // Description
    public int rewardMoney;   // GET MONEY
    public int penaltyMoney;  // PAY MONEY
    public int moveToBoardIndex = -1;
    public bool collectFromPlayer;
    
    [Header("Cosas de carcel")]
    public bool goToJail;
    public bool jailFreeCard;

    [Header("Pago de casas y hoteles")]
    public bool streetRepairs;
    public int streetRepairsHousePrice = 40;
    public int streetRepairsHotelPrice = 115;
}
