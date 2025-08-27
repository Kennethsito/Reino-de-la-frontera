using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System;
public class ManageUI : MonoBehaviour
{
    public static ManageUI instance;
    [SerializeField] GameObject managePanel; // TO SHOW AND HIDE
    [SerializeField] Transform propertyGrid; // TO PARENT PROPERTY SETS TO IT
    [SerializeField] GameObject propertySetPrefab; //
    Player playerReference;
    List<GameObject> propertyPrefabs = new List<GameObject>();
    [SerializeField] TMP_Text yourMoneyText;
    [SerializeField] TMP_Text systemMessageText;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        managePanel.SetActive(false);
    }

    public void OpenManager() // CALL FROM MANAGE BUTTON
    {
        playerReference = GameManager.instance.GetCurrentPlayer;
        CreatePropierties();

        // GET ALL NODES AS NODE SETS

        managePanel.SetActive(true);
        UpdateMoneyText();
    }

    public void CloseManager()
    {
        managePanel.SetActive(false);
        ClearPropierties();
        
    }
    void ClearPropierties()
    {
        for (int i = propertyPrefabs.Count - 1; i >= 0; i--)
        {
            Destroy(propertyPrefabs[i]);
        }
        propertyPrefabs.Clear();
    }
    void CreatePropierties()
    {
        List<MonopolyNode> processedSet = null;

        foreach (var node in playerReference.GetMonopolyNodes)
        {
            var (list, allSame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(node);
            List<MonopolyNode> nodeSet = new List<MonopolyNode>();
            nodeSet.AddRange(list);

            if (nodeSet != null && list != processedSet)
            {
                // UPDATE PROCESSED FIRST
                processedSet = list;

                nodeSet.RemoveAll(n => n.Owner != playerReference);

                // CREATE PREFAB WITH ALL NODES OWNED BY THE PLAYER
                GameObject newPropertySet = Instantiate(propertySetPrefab, propertyGrid, false);
                newPropertySet.GetComponent<ManagePropertyUi>().Setproperty(nodeSet, playerReference);

                propertyPrefabs.Add(newPropertySet);
            }
        }
    }

    public void UpdateMoneyText()
    {
        string showMoney = (playerReference.ReadMoney >= 0) ? "<color=green>$ " + playerReference.ReadMoney : "<color=red>$ " + playerReference.ReadMoney;
        yourMoneyText.text = "<color=black>Tu dinero: </color>" + showMoney;
    }

    public void UpdateSystemMessage(string message)
    {
        systemMessageText.text = message;
    }

    public void AutoHandleFounds()
    {
        if (playerReference.ReadMoney >= 0)
        {
            UpdateSystemMessage("No tienes deudas, no necesitas manejar fondos");
            return; // NO NEED TO HANDLE FUNDS
        }
        playerReference.HandleInsufficientFunds(Mathf.Abs(playerReference.ReadMoney));
        ClearPropierties();
        CreatePropierties();
        UpdateMoneyText();
    }
}
