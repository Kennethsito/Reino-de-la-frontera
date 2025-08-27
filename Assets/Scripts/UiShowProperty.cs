using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
public class UiShowProperty : MonoBehaviour
{
    MonopolyNode nodeReference;
    Player playerReference;
    [Header("Buy Property UI")]
    [SerializeField] GameObject propertyUiPanel;
    [SerializeField] TMP_Text propertyNameText;
    [SerializeField] Image colorField;
    [Space]
    [SerializeField] TMP_Text rentPriceText; // WITHOUT A HOUSE
    [SerializeField] TMP_Text oneHouseRentText;
    [SerializeField] TMP_Text twoHouseRentText;
    [SerializeField] TMP_Text threeHouseRentText;
    [SerializeField] TMP_Text fourHouseRentText;
    [SerializeField] TMP_Text hotelRentText;
    [Space]
    [SerializeField] TMP_Text housePriceText;
    [SerializeField] TMP_Text mortgagePriceText;
    [Space]
    [SerializeField] Button buyPropertyButton;
    [Space]
    [SerializeField] TMP_Text propertyPriceText;
    [SerializeField] TMP_Text playerMoneyText;

    void OnEnable()
    {
        MonopolyNode.OnShowPropertyBuyPanel += ShowBuyPropertyUi;
        propertyUiPanel.SetActive(false);
    }
    void OnDisable()
    {
        MonopolyNode.OnShowPropertyBuyPanel -= ShowBuyPropertyUi;
    }

    void Start()
    {
        propertyUiPanel.SetActive(false);
    }
    void ShowBuyPropertyUi(MonopolyNode node, Player currentPlayer)
    {
        nodeReference = node;
        playerReference = currentPlayer;
        //panel de arriba
        propertyNameText.text = node.name;
        colorField.color = node.propertyColorField.color;
        //centro de la carta
        rentPriceText.text = "$ " + node.baseRent;
        oneHouseRentText.text = "$ " + node.rentWithHouses[0];
        twoHouseRentText.text = "$ " + node.rentWithHouses[1];
        threeHouseRentText.text = "$ " + node.rentWithHouses[2];
        fourHouseRentText.text = "$ " + node.rentWithHouses[3];
        hotelRentText.text = "$ " + node.rentWithHouses[4];

        //precios de casas y hoteles
        housePriceText.text = "$ " + node.houseCost;
        mortgagePriceText.text = "$ " + node.MortgageValue;

        //parte de abajo
        propertyPriceText.text = "Precio: $ " + node.price;
        playerMoneyText.text = "Dinero: $ " + currentPlayer.ReadMoney;

        //boton de comprar
        if (currentPlayer.CanAffordNode(node.price))
        {
            buyPropertyButton.interactable = true;
        }
        else
        {
            buyPropertyButton.interactable = false;
        }
        //ensenar el panel
        propertyUiPanel.SetActive(true);
    }

    public void BuyPropertyButton() // THIS IS CALLED FROM THE BUY BUTTON
    {
        // jugador que comprara la propiedad
        playerReference.BuyProperty(nodeReference);
        // cerrar el panel


        // MAKE THE BUTTON NOT INTERACTABLE ANYMORE
        buyPropertyButton.interactable = false;
    }

    public void ClosePropertyButton() // THIS IS CALLED FROM THE BUY BUTTON
    {
        // CLOSE THE PANEL
        propertyUiPanel.SetActive(false);

        // CLEAR NODEREFERENCE
        nodeReference = null;
        playerReference = null;
    }



}
