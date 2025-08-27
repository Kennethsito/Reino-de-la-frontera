using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
public class UiShowRailroad : MonoBehaviour
{
    MonopolyNode nodeReference;
    Player playerReference;
    [Header("Buy Railroad UI")]
    [SerializeField] GameObject railRoadUiPanel;
    [SerializeField] TMP_Text railRoadNameText;
    [SerializeField] Image colorField;
    [Space]
    [SerializeField] TMP_Text oneRailRoadRentText;
    [SerializeField] TMP_Text twoRailRoadRentText;
    [SerializeField] TMP_Text threeRailRoadRentText;
    [SerializeField] TMP_Text fourRailRoadRentText;
    [Space]
    [SerializeField] TMP_Text mortgagePriceText;
    [Space]
    [SerializeField] Button buyRailRoadButton;
    [Space]
    [SerializeField] TMP_Text propertyPriceText;
    [SerializeField] TMP_Text playerMoneyText;

    void OnEnable()
    {
        MonopolyNode.OnShowRailRoadBuyPanel += ShowBuyRailRoadBuyPanelUI;
    }
    void OnDisable()
    {
        MonopolyNode.OnShowRailRoadBuyPanel -= ShowBuyRailRoadBuyPanelUI;
    }
    void Start()
    {
        railRoadUiPanel.SetActive(false);
    }

    void ShowBuyRailRoadBuyPanelUI(MonopolyNode node, Player currentPlayer)
    {
        nodeReference = node;
        playerReference = currentPlayer;
        //panel de arriba
        railRoadNameText.text = node.name;
        //colorField.color = node.propertyColorField.color;
        //centro de la carta
        //result = baseRent * (int)math.pow(2, amount - 1);
        oneRailRoadRentText.text = "$ " + node.baseRent * (int)Mathf.Pow(2, 1 - 1);
        twoRailRoadRentText.text = "$ " + node.baseRent * (int)Mathf.Pow(2, 2 - 1);
        threeRailRoadRentText.text = "$ " + node.baseRent * (int)Mathf.Pow(2, 3 - 1);
        fourRailRoadRentText.text = "$ " + node.baseRent * (int)Mathf.Pow(2, 4 - 1);

        //precios de casas y hoteles
        mortgagePriceText.text = "$ " + node.MortgageValue;

        //parte de abajo
        propertyPriceText.text = "Precio: $ " + node.price;
        playerMoneyText.text = "Dinero: $ " + currentPlayer.ReadMoney;

        //boton de comprar
        if (currentPlayer.CanAffordNode(node.price))
        {
            buyRailRoadButton.interactable = true;
        }
        else
        {
            buyRailRoadButton.interactable = false;
        }
        //ensenar el panel
        railRoadUiPanel.SetActive(true);
    }

    public void BuyRailRoadButton() // THIS IS CALLED FROM THE BUY BUTTON
    {
        // jugador que comprara la propiedad
        playerReference.BuyProperty(nodeReference);
        // cerrar el panel


        // MAKE THE BUTTON NOT INTERACTABLE ANYMORE
        buyRailRoadButton.interactable = false;
    }

    public void CloseRailroadButton() // THIS IS CALLED FROM THE BUY BUTTON
    {
        // CLOSE THE PANEL
        railRoadUiPanel.SetActive(false);

        // CLEAR NODEREFERENCE
        nodeReference = null;
        playerReference = null;
    }
}
