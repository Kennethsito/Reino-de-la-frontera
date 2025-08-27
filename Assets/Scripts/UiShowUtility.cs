using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class UiShowUtility : MonoBehaviour
{
    MonopolyNode nodeReference;
    Player playerReference;
    [Header("Buy Utility UI")]
    [SerializeField] GameObject utilityUiPanel;
    [SerializeField] TMP_Text utilityNameText;
    [SerializeField] Image colorField;
    [Space]
    [SerializeField] TMP_Text mortgagePriceText;
    [Space]
    [SerializeField] Button buyUtilityButton;
    [Space]
    [SerializeField] TMP_Text utilityPriceText;
    [SerializeField] TMP_Text playerMoneyText;

    void OnEnable()
    {
        MonopolyNode.OnShowUtilityBuyPanel += ShowBuyUtilityBuyPanel;
    }
    void OnDisable()
    {
        MonopolyNode.OnShowUtilityBuyPanel -= ShowBuyUtilityBuyPanel;
    }
    void Start()
    {
        utilityUiPanel.SetActive(false);
    }


     void ShowBuyUtilityBuyPanel(MonopolyNode node, Player currentPlayer)
    {
        nodeReference = node;
        playerReference = currentPlayer;
        //panel de arriba
        utilityNameText.text = node.name;
        //colorField.color = node.propertyColorField.color;

        //precios de casas y hoteles
        mortgagePriceText.text = "$ " + node.MortgageValue;

        //parte de abajo
        utilityPriceText.text = "Precio: $ " + node.price;
        playerMoneyText.text = "Dinero: $ " + currentPlayer.ReadMoney;

        //boton de comprar
        if (currentPlayer.CanAffordNode(node.price))
        {
            buyUtilityButton.interactable = true;
        }
        else
        {
            buyUtilityButton.interactable = false;
        }
        //ensenar el panel
        utilityUiPanel.SetActive(true);
    }

    public void BuyUtilityButton() // THIS IS CALLED FROM THE BUY BUTTON
    {
        // jugador que comprara la propiedad
        playerReference.BuyProperty(nodeReference);
        // cerrar el panel


        // MAKE THE BUTTON NOT INTERACTABLE ANYMORE
        buyUtilityButton.interactable = false;
    }

    public void CloseUtilityButton() // THIS IS CALLED FROM THE BUY BUTTON
    {
        // CLOSE THE PANEL
        utilityUiPanel.SetActive(false);

        // CLEAR NODEREFERENCE
        nodeReference = null;
        playerReference = null;
    }
}



