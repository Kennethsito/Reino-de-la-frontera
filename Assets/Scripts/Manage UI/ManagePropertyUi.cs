using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
public class ManagePropertyUi : MonoBehaviour
{
    [SerializeField] Transform cardHolder; //objeto horizontal
    [SerializeField] GameObject cardPrefab; //prefab do card
    [SerializeField] Button buyHouseButton, sellHouseButton; //boton para las casas y eso
    [SerializeField] TMP_Text buyHousePriceText, sellHousePriceText; //texto de dinero
    Player playerReference;
    List<MonopolyNode> nodesInSet = new List<MonopolyNode>();
    List<GameObject> cardsInSet = new List<GameObject>();
    [SerializeField] GameObject buttonBox;


    public void Setproperty(List<MonopolyNode> nodes, Player owner)
    {
        playerReference = owner;
        nodesInSet.AddRange(nodes);
        for (int i = 0; i < nodes.Count; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, cardHolder, false);
            ManageCardUi manageCardUi = newCard.GetComponent<ManageCardUi>();
            cardsInSet.Add(newCard);
            manageCardUi.SetCard(nodesInSet[i], owner, this);
        }
        var (list, allsame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(nodesInSet[0]);
        buyHouseButton.interactable = allsame && ChechIfbuyAllowed();
        sellHouseButton.interactable = CheckIfSellAllowed();

        buyHousePriceText.text = "- $" + nodesInSet[0].houseCost;
        sellHousePriceText.text = "+ $" + nodesInSet[0].houseCost / 2;
        if (nodes[0].monopolyNodeType != MonopolyNodeType.Property)
        {
            buttonBox.SetActive(false);
        }
    }
    public void BuyHouseButton()
    {
        if (!ChechIfbuyAllowed())
        {
            string message = "No puedes comprar casas si tienes propiedades hipotecadas";
            ManageUI.instance.UpdateSystemMessage(message);
            return;
        }
        if (playerReference.CanAffordHouse(nodesInSet[0].houseCost))
        {
            playerReference.BuildHouseOrHotelEvenly(nodesInSet);
            UpdateHouseVisualize();
            string message = "Construiste una casa guapo";
            ManageUI.instance.UpdateSystemMessage(message);

        }
        else
        {
            string message = "No tienes suficiente dinero para comprar una casa";
            ManageUI.instance.UpdateSystemMessage(message);
        }
        sellHouseButton.interactable = CheckIfSellAllowed();
        ManageUI.instance.UpdateMoneyText();
    }
    public void SellHouseButton()
    {
        playerReference.SellHouseEvenly(nodesInSet);
        UpdateHouseVisualize();
        sellHouseButton.interactable = CheckIfSellAllowed();
        ManageUI.instance.UpdateMoneyText();
    }

    bool CheckIfSellAllowed()
    {
        if (nodesInSet.Any(n => n.NumberOfHouses > 0))
        {
            return true;
        }
        return false;

    }

    bool ChechIfbuyAllowed()
    {
        if (nodesInSet.Any(n => n.IsMortgaged == true))
        {
            return false;
        }
        return true;
    }

    public bool CheckIfMortgageAllowed()
    {
        if (nodesInSet.Any(n => n.NumberOfHouses > 0))
        {
            return false;
        }
        return true;

    }


    void UpdateHouseVisualize()
    {
        foreach (var card in cardsInSet)
        {
            card.GetComponent<ManageCardUi>().Showbuildings();
        }
    }

}
