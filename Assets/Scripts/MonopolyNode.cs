using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;
public enum MonopolyNodeType
{
   Property,
      Utility,
   Railroad,
   Tax,
   Chance,
   CommunityChest,
   Go,
   Jail,
   FreeParking,
   GoToJail
}

public class MonopolyNode : MonoBehaviour
{
    public MonopolyNodeType monopolyNodeType;
    public Image propertyColorField;

    [Header("Nombre")]
    [SerializeField] internal new string name;
    [SerializeField] TMP_Text nameText;
    [Header("Property Price")]
    public int price;
    public int houseCost;
    [SerializeField] TMP_Text priceText;
    [Header("Property Rent")]
    [SerializeField] bool calculateRentAuto;
    [SerializeField] int currentRent;
    [SerializeField] internal int baseRent;
    [SerializeField] internal List<int> rentWithHouses = new List<int>();
    int numberOfHouses;
    public int NumberOfHouses => numberOfHouses;
    [SerializeField] GameObject[] houses;
    [SerializeField] GameObject hotel;
    [Header("Property Mortgage")]
    [SerializeField] GameObject mortgageImage;
    [SerializeField] GameObject propertyImage;

    [SerializeField] bool isMortgaged;
    [SerializeField] int mortgageValue;
    [Header("Property Owner")]
    [SerializeField] GameObject ownerBar;
    [SerializeField] TMP_Text ownerText;
    Player owner;
    //sistema de mensajes
    public delegate void UpdateMessage(string message);
    public static UpdateMessage OnUpdateMessage;
    //drag comminuty card
    public delegate void DrawCommunityCard(Player player);
    public static DrawCommunityCard OnDrawCommunityCard;
    //drag chance card
    public delegate void DrawChanceCard(Player player);
    public static DrawCommunityCard OnDrawChanceCard;

    //human panel
    public delegate void ShowHumanPanel(bool showPanel, bool activateRollDice, bool ActivateEndTurn, bool hasChanceJailCard, bool hasCommunityJailCard);
    public static ShowHumanPanel OnShowHumanPanel;
    //panel de propiedad de jugador
    public delegate void ShowPropertyBuyPanel(MonopolyNode node, Player player);
    public static ShowPropertyBuyPanel OnShowPropertyBuyPanel;

    //panel de propiedad de railroad de jugador
    public delegate void ShowRailRoadBuyPanel(MonopolyNode node, Player player);
    public static ShowRailRoadBuyPanel OnShowRailRoadBuyPanel;

    //panel de propiedad de utility de jugador
    public delegate void ShowUtilityBuyPanel(MonopolyNode node, Player player);
    public static ShowUtilityBuyPanel OnShowUtilityBuyPanel;
    public Player Owner => owner;
    public void SetOwner(Player newOwner)
    {
        owner = newOwner;
        OnOwnerUpdated();
    }





    void OnValidate()
    {
        if (nameText != null)
        {
            nameText.text = name;
        }


        //calculations
        if (calculateRentAuto)
        {
            if (monopolyNodeType == MonopolyNodeType.Property)
            {
                if (baseRent > 0)
                {
                    price = 3 * (baseRent * 10);
                    mortgageValue = price / 2;
                    rentWithHouses.Clear();
                    rentWithHouses.Add(baseRent * 5);
                    rentWithHouses.Add(baseRent * 5 * 3);
                    rentWithHouses.Add(baseRent * 5 * 9);
                    rentWithHouses.Add(baseRent * 5 * 16);
                    rentWithHouses.Add(baseRent * 5 * 25);

                }
                else if (baseRent <= 0)
                {
                    price = 0;
                    mortgageValue = 0;
                    rentWithHouses.Clear();
                    mortgageValue = 0;

                }

            }
            if (monopolyNodeType == MonopolyNodeType.Utility)
            {
                mortgageValue = price / 2;

            }
            if (monopolyNodeType == MonopolyNodeType.Railroad)
            {
                mortgageValue = price / 2;

            }

        }
        if (priceText != null)
        {
            priceText.text = "$" + price;
        }

        OnOwnerUpdated();
        UnMortgageProperty();
        //isMortgaged = false;
    }
    // MORTGAGE CONTENT

    public void updateColorField(Color color)
    {
        if (propertyColorField != null)
        {
            propertyColorField.color = color;
        }
    }
    public int MortgageProperty()
    {
        isMortgaged = true;
        if (mortgageImage != null)
        {
            mortgageImage.SetActive(true);

        }

        if (propertyImage != null)
        {
            propertyImage.SetActive(false);
        }
        return mortgageValue;
    }

    public void UnMortgageProperty()
    {
        isMortgaged = false;
        if (mortgageImage != null)
        {
            mortgageImage.SetActive(false);

        }

        if (propertyImage != null)
        {
            propertyImage.SetActive(true);
        }
    }

    public bool IsMortgaged => isMortgaged;
    public int MortgageValue => mortgageValue;

    // UPDATE OWNER
    public void OnOwnerUpdated()
    {
        if (ownerBar != null)
        {
            if (owner != null)
            {
                ownerBar.SetActive(true);
                ownerText.text = owner.name;
            }
            else
            {
                ownerBar.SetActive(false);
                ownerText.text = "";
            }
        }
    }


    public void PlayerLandedOnNode(Player currentPlayer)
    {
        bool playerIsHuman = currentPlayer.playerType == Player.PlayerType.Human;
        bool continueTurn = true;

        //checar el nodo
        switch (monopolyNodeType)
        {
            case MonopolyNodeType.Property:
                if (!playerIsHuman) //ia
                {
                    if (owner != null && owner != currentPlayer && !isMortgaged)
                    {
                        //pagar la renta a alguien

                        //calcular la renta
                        int rentToPay = CalculatePropertyRent();

                        //pagar la renta al owner
                        currentPlayer.PayRent(rentToPay, owner);

                        //mensaje de que paso
                        OnUpdateMessage.Invoke(currentPlayer.name + " paid $ " + rentToPay + " to " + owner.name);
                    }
                    else if (owner == null && currentPlayer.CanAffordNode(price))
                    {
                        //Debug.Log("Player could buy");
                        OnUpdateMessage.Invoke(currentPlayer.name + " Compra " + this.name);
                        currentPlayer.BuyProperty(this);
                        // OnOwnerUpdated();
                    }
                    else
                    {

                    }
                }
                else //human
                {
                    if (owner != null && owner != currentPlayer && !isMortgaged)
                    {
                        //calcular la renta
                        int rentToPay = CalculatePropertyRent();

                        //pagar la renta al owner
                        currentPlayer.PayRent(rentToPay, owner);
                    }
                    else if (owner == null)
                    {
                        OnShowPropertyBuyPanel.Invoke(this, currentPlayer);
                    }
                    else
                    {

                    }
                }
                break;
            case MonopolyNodeType.Utility:
                if (!playerIsHuman) //ia
                {
                    if (owner != null && owner != currentPlayer && !isMortgaged)
                    {
                        //pagar la renta a alguien

                        //calcular la renta
                        int rentToPay = CalculateUtilityRent();
                        currentRent = rentToPay;

                        //pagar la renta al owner
                        currentPlayer.PayRent(rentToPay, owner);

                        //mensaje de que paso
                        OnUpdateMessage.Invoke(currentPlayer.name + " paid $ " + rentToPay + " to " + owner.name);
                    }
                    else if (owner == null && currentPlayer.CanAffordNode(price))
                    {
                        //Debug.Log("Player could buy");
                        OnUpdateMessage.Invoke(currentPlayer.name + " Compra " + this.name);
                        currentPlayer.BuyProperty(this);
                        OnOwnerUpdated();
                    }
                    else
                    {

                    }
                }
                else //human
                {
                    if (owner != null && owner != currentPlayer && !isMortgaged)
                    {
                        //calcular la renta
                        int rentToPay = CalculateUtilityRent();
                        currentRent = rentToPay;

                        //pagar la renta al owner
                        currentPlayer.PayRent(rentToPay, owner);
                    }
                    else if (owner == null)
                    {
                        OnShowUtilityBuyPanel.Invoke(this, currentPlayer);
                    }
                    else
                    {

                    }
                }

                break;
            case MonopolyNodeType.Railroad:
                if (!playerIsHuman) //ia
                {
                    if (owner != null && owner != currentPlayer && !isMortgaged)
                    {
                        //pagar la renta a alguien

                        //calcular la renta
                        int rentToPay = CalculateRailroadRent();
                        currentRent = rentToPay;

                        //pagar la renta al owner
                        currentPlayer.PayRent(rentToPay, owner);

                        //mensaje de que paso
                        OnUpdateMessage.Invoke(currentPlayer.name + " paid $ " + rentToPay + " to " + owner.name);
                    }
                    else if (owner == null && currentPlayer.CanAffordNode(price))
                    {
                        //Debug.Log("Player could buy");
                        OnUpdateMessage.Invoke(currentPlayer.name + " Compra " + this.name);
                        currentPlayer.BuyProperty(this);
                        OnOwnerUpdated();
                    }
                    else
                    {

                    }
                }
                else //human
                {
                    if (owner != null && owner != currentPlayer && !isMortgaged)
                    {
                        //calcular la renta
                        int rentToPay = CalculateRailroadRent();
                        currentRent = rentToPay;

                        //pagar la renta al owner
                        currentPlayer.PayRent(rentToPay, owner);
                    }
                    else if (owner == null)
                    {
                        OnShowRailRoadBuyPanel.Invoke(this, currentPlayer);
                    }
                    else
                    {

                    }
                }

                break;
            case MonopolyNodeType.Tax:
                GameManager.instance.AddTaxToPool(price);
                currentPlayer.PayMoney(price);
                OnUpdateMessage.Invoke(currentPlayer.name + " Paga el IVA de " + price);
                break;
            case MonopolyNodeType.FreeParking:
                int tax = GameManager.instance.GetTaxPool();
                currentPlayer.CollectMoney(tax);
                OnUpdateMessage.Invoke(currentPlayer.name + " Obtiene el IVA de " + tax);


                break;
            case MonopolyNodeType.GoToJail:
                int indexOnBoard = MonopolyBoard.instance.route.IndexOf(currentPlayer.MyMonopolyNode);
                currentPlayer.GoToJail(indexOnBoard);
                OnUpdateMessage.Invoke(currentPlayer.name + " <b>te vas al bote <color=red>cabron</color></b> ");
                continueTurn = false;
                break;
            case MonopolyNodeType.Chance:
                OnDrawChanceCard.Invoke(currentPlayer);
                continueTurn = false;
                break;
            case MonopolyNodeType.CommunityChest:
                OnDrawCommunityCard.Invoke(currentPlayer);
                continueTurn = false;
                break;
        }
        if (!continueTurn)
        {
            //si no se continua el turno, no se cambia de jugador
            return;
        }




        if (!playerIsHuman)
        {
            //Invoke("ContinueGame", GameManager.instance.SecondsBetweenTurns);
            currentPlayer.ChanceState(Player.AiStates.TRADING);
        }
        else
        {
            bool canEndTurn = !GameManager.instance.RolledADouble && currentPlayer.ReadMoney >= 0;
            bool canRollDice = GameManager.instance.RolledADouble && currentPlayer.ReadMoney >= 0;
            //show ui
            bool jail1 = currentPlayer.HasChanceJailFreeCard;
            bool jail2 = currentPlayer.HasCommunityJailFreeCard;
            OnShowHumanPanel.Invoke(true, canRollDice, canEndTurn,jail1,jail2);
        }
    }

    // void ContinueGame()
    // {
    //     if (GameManager.instance.RolledADouble)
    //     {
    //         GameManager.instance.RollDice();

    //     }
    //     else
    //     {
    //         GameManager.instance.SwitchPlayer();
    //     }
    // }

    int CalculatePropertyRent()
    {
        switch (numberOfHouses)
        {
            case 0:
                var (list, allSame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(this);



                if (allSame)
                {
                    currentRent = baseRent * 2;
                }
                else
                {
                    currentRent = baseRent;
                }

                break;
            case 1:
                currentRent = rentWithHouses[0];
                break;
            case 2:
                currentRent = rentWithHouses[1];
                break;
            case 3:
                currentRent = rentWithHouses[2];
                break;
            case 4:
                currentRent = rentWithHouses[3];
                break;
            case 5: //hotel
                currentRent = rentWithHouses[4];
                break;
        }
        return currentRent;
    }


    int CalculateUtilityRent()
    {
        List<int> lastRolledDIce = GameManager.instance.LastRolledDice;
        int result = 0;
        var (list, allSame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(this);
        if (allSame)
        {
            result = (lastRolledDIce[0] + lastRolledDIce[1]) * 10;
        }
        else
        {
            result = (lastRolledDIce[0] + lastRolledDIce[1]) * 4;
        }
        return result;
    }

    int CalculateRailroadRent()
    {
        int result = 0;
        var (list, allSame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(this);
        int amount = 0;
        foreach (var item in list)
        {
            amount += (item.owner == this.owner) ? 1 : 0;
        }


        result = baseRent * (int)Mathf.Pow(2, amount - 1);

        return result;
    }



    void VisualizeHouses()
    {
        switch (numberOfHouses)
        {
            case 0:
                houses[0].SetActive(false);
                houses[1].SetActive(false);
                houses[2].SetActive(false);
                houses[3].SetActive(false);
                hotel.SetActive(false);
                break;
            case 1:
                houses[0].SetActive(true);
                houses[1].SetActive(false);
                houses[2].SetActive(false);
                houses[3].SetActive(false);
                hotel.SetActive(false);
                break;
            case 2:
                houses[0].SetActive(true);
                houses[1].SetActive(true);
                houses[2].SetActive(false);
                houses[3].SetActive(false);
                hotel.SetActive(false);
                break;
            case 3:
                houses[0].SetActive(true);
                houses[1].SetActive(true);
                houses[2].SetActive(true);
                houses[3].SetActive(false);
                hotel.SetActive(false);
                break;
            case 4:
                houses[0].SetActive(true);
                houses[1].SetActive(true);
                houses[2].SetActive(true);
                houses[3].SetActive(true);
                hotel.SetActive(false);
                break;
            case 5:
                houses[0].SetActive(false);
                houses[1].SetActive(false);
                houses[2].SetActive(false);
                houses[3].SetActive(false);
                hotel.SetActive(true);
                break;

        }
    }


    public void BuildHouseOrHotel()
    {
        if (monopolyNodeType == MonopolyNodeType.Property)
        {
            numberOfHouses++;
            VisualizeHouses();
        }
    }
    public int SellHouseOrHotel()
    {
        if (monopolyNodeType == MonopolyNodeType.Property && numberOfHouses > 0)
        {
            numberOfHouses--;
            VisualizeHouses();
            return houseCost / 2; //half the cost of the house

        }
        return 0; //no house to sell
    }
    public void ResetNode()
    {
        if (isMortgaged)
        {
            propertyImage.SetActive(true);
            mortgageImage.SetActive(false);
            isMortgaged = false;
        }
        if (monopolyNodeType == MonopolyNodeType.Property)
        {
            numberOfHouses = 0;
            VisualizeHouses();
        }
        owner.RemoveProperty(this);
        owner.name = "";
        owner.ActivateSelector(false);
        owner = null;

        OnOwnerUpdated();
    }

    public void ChangeOwner(Player newOwner)
    {
        owner.RemoveProperty(this);
        newOwner.AddProperty(this);
        SetOwner(newOwner);
    }




}