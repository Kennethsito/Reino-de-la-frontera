using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class ManageCardUi : MonoBehaviour
{
    [SerializeField] Image colorField;
    [SerializeField] TMP_Text propertyNameText;
    [SerializeField] GameObject[] buildings;
    [SerializeField] GameObject mortgageImage;
    [SerializeField] TMP_Text mortgageValueText;
    [SerializeField] Button mortgageButton, unMortgageButton;

    [SerializeField] Image iconImage;
    [SerializeField] Sprite houseSprite, railRoadSprite, UtilitySprite;
    Player playerReference;
    MonopolyNode nodeReference;
    ManagePropertyUi propertyReference;
    //Color setColor, int numberOfBuildings, bool isMortgaged, int mortgageValue
    public void SetCard(MonopolyNode node, Player owner, ManagePropertyUi propertySet)
    {
        nodeReference = node;
        playerReference = owner;
        propertyReference = propertySet;
        // SET COLOR
        if (node.propertyColorField != null)
        {
            colorField.color = node.propertyColorField.color;
        }
        else
        {
            colorField.color = Color.black; // Default color if none is set
        }

        //ensenar los edificios
        Showbuildings();

        mortgageImage.SetActive(node.IsMortgaged);
        mortgageValueText.text = "mortgageValue Value <br>$ " + node.MortgageValue;
        //botones
        mortgageButton.interactable = !node.IsMortgaged;
        unMortgageButton.interactable = node.IsMortgaged;
        //set icon
        switch (nodeReference.monopolyNodeType)
        {
            case MonopolyNodeType.Property:
                iconImage.sprite = houseSprite;
                break;
            case MonopolyNodeType.Railroad:
                iconImage.sprite = railRoadSprite;
                iconImage.color = Color.white; // Ensure the color is white for railroads
                break;
            case MonopolyNodeType.Utility:
                iconImage.sprite = UtilitySprite;
                iconImage.color = Color.black; // Ensure the color is black for utilities
                break;
        }
        //nombre de la propiedad
        propertyNameText.text = nodeReference.name;
    }

    public void MortgageButton()
    {
        if (!propertyReference.CheckIfMortgageAllowed())
        {
            string message = "Tienes casas u hoteles en una propiedad, no puedes hipotecarla";
            ManageUI.instance.UpdateSystemMessage(message);
            return;
        }
        if (nodeReference.IsMortgaged)
        {
            string message = "Ya esta hipotecada wey";
            ManageUI.instance.UpdateSystemMessage(message);
            return;
        }
        playerReference.CollectMoney(nodeReference.MortgageProperty());
        mortgageImage.SetActive(true);
        mortgageButton.interactable = false;
        unMortgageButton.interactable = true;
        ManageUI.instance.UpdateMoneyText();

    }

    public void UnMortgageButton()
    {
        if (!nodeReference.IsMortgaged)
        {
            string message = "No esta hipotecada wey";
            ManageUI.instance.UpdateSystemMessage(message);
            return;
        }
        if (playerReference.ReadMoney < nodeReference.MortgageValue)
        {
            string message = "Eres pobre, no tienes suficiente dinero";
            ManageUI.instance.UpdateSystemMessage(message);
            return;
        }
        playerReference.PayMoney(nodeReference.MortgageValue);
        nodeReference.UnMortgageProperty();
        mortgageImage.SetActive(false);
        mortgageButton.interactable = true;
        unMortgageButton.interactable = false;
        ManageUI.instance.UpdateMoneyText();
    }

    public void Showbuildings()
    {
        foreach (var icon in buildings)
        {
            icon.SetActive(false);
        }


        if (nodeReference.NumberOfHouses < 5)
        {
            for (int i = 0; i < nodeReference.NumberOfHouses; i++)
            {
                buildings[i].SetActive(true);
            }
        }
        else
        {
            buildings[4].SetActive(true);
        }
    }

}
