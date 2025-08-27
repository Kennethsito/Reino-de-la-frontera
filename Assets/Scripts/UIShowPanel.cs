using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class UIShowPanel : MonoBehaviour
{
    [SerializeField] GameObject humanPanel;
    [SerializeField] Button rollDiceButton;
    [SerializeField] Button endTurnButton;
    [SerializeField] Button jailFreeCard1;
    [SerializeField] Button jailFreeCard2;

    void OnEnable()
    {
        GameManager.OnShowHumanPanel += ShowPanel;
        MonopolyNode.OnShowHumanPanel += ShowPanel;
        CommunityChest.OnShowHumanPanel += ShowPanel;
        ChanceField.OnShowHumanPanel += ShowPanel;
        Player.OnShowHumanPanel += ShowPanel;
    }
    void OnDisable()
    {
        GameManager.OnShowHumanPanel -= ShowPanel;
        MonopolyNode.OnShowHumanPanel -= ShowPanel;
        CommunityChest.OnShowHumanPanel -= ShowPanel;
        ChanceField.OnShowHumanPanel -= ShowPanel;
        Player.OnShowHumanPanel -= ShowPanel;
    }
    void ShowPanel(bool ShowPanel, bool enableRollDice, bool enableEndTurn, bool hasChanceJailCard, bool hasCommunityJailCard)
    {
        humanPanel.SetActive(ShowPanel);
        rollDiceButton.interactable = enableRollDice;
        endTurnButton.interactable = enableEndTurn;
        jailFreeCard1.interactable = hasChanceJailCard;
        jailFreeCard2.interactable = hasCommunityJailCard;

    }
    
        
    
}
