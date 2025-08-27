using UnityEngine;
using System.Collections.Generic;
using System.Linq;


[System.Serializable]
public class Player
{
    public enum PlayerType
    {
        Human,
        AI
    }
    public PlayerType playerType;
    public string name;
    int money;
    MonopolyNode currentnode;
    bool isInJail;
    int numTurnsInJail = 0;

    [SerializeField] GameObject myToken;
    [SerializeField] List<MonopolyNode> myMonopolyNodes = new List<MonopolyNode>();
    public List<MonopolyNode> GetMonopolyNodes => myMonopolyNodes;

    bool hasChanceJailFreeCard, hasCommunityJailFreeCard;

    public bool HasChanceJailFreeCard => hasChanceJailFreeCard;
    public bool HasCommunityJailFreeCard => hasCommunityJailFreeCard;

    //informacion del jugador

    //ia
    int aiMoneySavity = 200;

    //estado de ia
    public enum AiStates
    {
        IDLE,
        TRADING
    }
    public AiStates aiState;

    PlayerInfo myInfo;
    // Retorno de informacion

    public bool IsInJail => isInJail;
    public GameObject MyToken => myToken;
    public MonopolyNode MyMonopolyNode => currentnode;
    public int ReadMoney => money;

    //sistema de mensajes
    public delegate void UpdateMessage(string message);
    public static UpdateMessage OnUpdateMessage;
    //human panel
    public delegate void ShowHumanPanel(bool showPanel, bool activateRollDice, bool ActivateEndTurn, bool hasChanceJailCard, bool hasCommunityJailCard);
    public static ShowHumanPanel OnShowHumanPanel;

    public void Initialize(MonopolyNode startNode, int startMoney, PlayerInfo info, GameObject token)
    {
        currentnode = startNode;
        money = startMoney;
        myInfo = info;
        myInfo.SetPlayerNameAndCash(name, money);
        myToken = token;
        myInfo.ActiveArrow(false);
    }
    public void setMyCurrentNode(MonopolyNode newNode)
    {
        currentnode = newNode;
        newNode.PlayerLandedOnNode(this);

        if (playerType == PlayerType.AI)
        {
            CheckIfPlayerHasASet();
            UnMortgageProperties();
            //TradingSystem.instance.FindMissingProperty(this);
        }
    }

    public void CollectMoney(int amount)
    {
        money += amount;
        myInfo.SetPlayerCash(money);
        if (playerType == PlayerType.Human && GameManager.instance.GetCurrentPlayer == this)
        {
            bool canEndTurn = !GameManager.instance.RolledADouble && ReadMoney >= 0 && GameManager.instance.HasRolledDice;
            bool canRollDice = (GameManager.instance.RolledADouble && ReadMoney >= 0) || (!GameManager.instance.HasRolledDice && ReadMoney >= 0);
            //show ui
            OnShowHumanPanel.Invoke(true, canRollDice, canEndTurn, hasChanceJailFreeCard, hasCommunityJailFreeCard);
        }
    }

    internal bool CanAffordNode(int price)
    {
        return price <= money;
    }

    public void BuyProperty(MonopolyNode node)
    {
        money -= node.price;
        node.SetOwner(this);
        //actualizar la informacion del jugador
        myInfo.SetPlayerCash(money);
        //agregar el nodo a la lista de nodos del jugador
        myMonopolyNodes.Add(node);
        //actualizar la informacion del nodo por precio
        SortPropertiesByPrice();
    }
    void SortPropertiesByPrice()
    {
        myMonopolyNodes = myMonopolyNodes.OrderBy(_node => _node.price).ToList();
    }

    internal void PayRent(int rentAmount, Player owner)
    {
        if (money < rentAmount)
        {
            if (playerType == PlayerType.AI)
            {
                HandleInsufficientFunds(rentAmount);
            }
            else
            {
                OnShowHumanPanel.Invoke(true, false, false, hasChanceJailFreeCard, hasCommunityJailFreeCard);
            }
        }
        money -= rentAmount;
        owner.CollectMoney(rentAmount);
        myInfo.SetPlayerCash(money);

    }

    internal void PayMoney(int amount)
    {
        if (money < amount)
        {
            if (playerType == PlayerType.AI)
            {
                HandleInsufficientFunds(amount);
            }
            // else
            //{
            //     OnShowHumanPanel.Invoke(true, false, false);
            // }

        }
        money -= amount;
        myInfo.SetPlayerCash(money);

        if (playerType == PlayerType.Human && GameManager.instance.GetCurrentPlayer == this)
        {
            bool canEndTurn = !GameManager.instance.RolledADouble && ReadMoney >= 0 && GameManager.instance.HasRolledDice;
            bool canRollDice = (GameManager.instance.RolledADouble && ReadMoney >= 0) || (!GameManager.instance.HasRolledDice && ReadMoney >= 0);
            //show ui
            OnShowHumanPanel.Invoke(true, canRollDice, canEndTurn, hasChanceJailFreeCard, hasCommunityJailFreeCard);
        }
    }

    //jail---------------------------------------
    public void GoToJail(int indexOnBoard)
    {
        isInJail = true;
        //myToken.transform.position = MonopolyBoard.instance.route[10].transform.position;
        //currentnode = MonopolyBoard.instance.route[10];
        MonopolyBoard.instance.MovePlayerToken(CalculateDistanceFromJail(indexOnBoard), this);
        GameManager.instance.ResetRolledADouble();
    }

    public void SetOutOfJail()
    {
        isInJail = false;
        numTurnsInJail = 0;

    }
    int CalculateDistanceFromJail(int indexOnBoard)
    {
        int result = 0;
        int indexOfJail = 10;
        if (indexOnBoard > indexOfJail)
        {
            result = (indexOnBoard - indexOfJail) * -1;
        }
        else
        {
            result = (indexOfJail - indexOnBoard);
        }
        return result;
    }
    public int NumTurnsInJail => numTurnsInJail;
    public void IncreaseNumTurnsInJail()
    {
        numTurnsInJail++;
    }

    public int[] CountHousesAndHotels()
    {
        int houses = 0;
        int hotels = 0;

        foreach (var node in myMonopolyNodes)
        {
            if (node.NumberOfHouses != 5)
            {
                houses += node.NumberOfHouses;
            }
            else
            {
                hotels += 1;
            }
        }

        int[] allBuildings = new int[] { houses, hotels };
        return allBuildings;
    }

    public void HandleInsufficientFunds(int amountToPay)
    {
        int housesToSell = 0; // AVAILABLE HOUSES TO SELL
        int allHouses = 0;
        int propertiesToMortgage = 0;
        int allPropertiesToMortgage = 0;

        // COUNT ALL HOUSES
        foreach (var node in myMonopolyNodes)
        {
            allHouses += node.NumberOfHouses;
        }

        // LOOP THROUGH THE PROPERTIES AND TRY TO SELL AS MUCH AS NEEDED
        while (money < amountToPay && allHouses > 0)
        {
            foreach (var node in myMonopolyNodes)
            {
                housesToSell = node.NumberOfHouses;
                if (housesToSell > 0)
                {
                    // Aquí iría el código para vender casas
                    CollectMoney(node.SellHouseOrHotel());
                    allHouses--;
                    if (money >= amountToPay)
                    {
                        return;
                    }
                }
            }
        }//embargar propiedades
        foreach (var node in myMonopolyNodes)
        {
            allPropertiesToMortgage += (!node.IsMortgaged) ? 1 : 0;

        }
        while (money < amountToPay && allPropertiesToMortgage > 0)
        {
            foreach (var node in myMonopolyNodes)
            {
                propertiesToMortgage = (!node.IsMortgaged) ? 1 : 0;
                if (propertiesToMortgage > 0)
                {
                    CollectMoney(node.MortgageProperty());
                    allPropertiesToMortgage--;
                    if (money >= amountToPay)
                    {
                        return;
                    }
                }
            }
        }
        if (playerType == PlayerType.AI)
        {
            //bancarrota para la ia
            Bankrupt();
        }
    }

    internal void Bankrupt()
    {
        // TAKE OUT THE PLAYER OF THE GAME

        // SEND A MESSAGE TO MESSAGE SYSTEM
        OnUpdateMessage.Invoke(name + " is Bankrupt");

        // CLEAR ALL WHAT THE PLAYER HAS OWNED
        for (int i = myMonopolyNodes.Count - 1; i >= 0; i--)
        {
            myMonopolyNodes[i].ResetNode();
        }

        if (hasChanceJailFreeCard)
        {
            ChanceField.instance.AddBackJailFreeCard();
        }
        if (hasCommunityJailFreeCard)
        {
            CommunityChest.instance.AddBackJailFreeCard();
        }


        // REMOVE THE PLAYER
        GameManager.instance.RemovePlayer(this);

    }

    void UnMortgageProperties()
    {
        // FOR AI
        foreach (var node in myMonopolyNodes)
        {
            if (node.IsMortgaged)
            {
                int cost = node.MortgageValue + (int)(node.MortgageValue * 0.1f); // 10% Interest

                // CAN WE AFFORT TO UNMORTGAGE
                if (money >= aiMoneySavity + cost)
                {
                    PayMoney(cost);
                    node.UnMortgageProperty();
                }
            }
        }
    }

    void CheckIfPlayerHasASet()
    {
        List<MonopolyNode> processedSet = null;
        foreach (var node in myMonopolyNodes)
        {
            var (list, allSame) = MonopolyBoard.instance.PlayerHasAllNodesOfSet(node);
            if (!allSame)
            {
                continue;
            }
            List<MonopolyNode> nodeSet = list;

            if (nodeSet != null && nodeSet != processedSet)
            {
                bool hasMortgadedNode = nodeSet.Any(node => node.IsMortgaged) ? true : false;

                if (!hasMortgadedNode)
                {
                    if (nodeSet[0].monopolyNodeType == MonopolyNodeType.Property)
                    {
                        // WE COULD BUILD A HOUSE ON THE SET
                        BuildHouseOrHotelEvenly(nodeSet);
                        processedSet = nodeSet;
                    }
                }
            }
        }
    }

    internal void BuildHouseOrHotelEvenly(List<MonopolyNode> nodesToBuildOn)
    {
        int minHouses = int.MaxValue;
        int maxHouses = int.MinValue;

        // GET MIN AND MAX NUMBERS OF HOUSE CURRENTLY ON THE PROPERTIES
        foreach (var node in nodesToBuildOn)
        {
            int numOfHouses = node.NumberOfHouses;

            if (numOfHouses < minHouses)
            {
                minHouses = numOfHouses;
            }

            if (numOfHouses > maxHouses && numOfHouses < 5)
            {
                maxHouses = numOfHouses;
            }
        }

        // BUY HOUSES ON THE PROPERTIES FOR MAX ALLOWED ON THE PROPERTIES
        foreach (var node in nodesToBuildOn)
        {
            if (node.NumberOfHouses == minHouses && node.NumberOfHouses < 5 && CanAffordHouse(node.houseCost))
            {
                node.BuildHouseOrHotel();
                PayMoney(node.houseCost);
                break;
            }
        }
    }

    internal void SellHouseEvenly(List<MonopolyNode> nodesToSellFrom)
    {
        int minHouses = int.MaxValue;
        bool houseSold = false;
        foreach (var node in nodesToSellFrom)
        {
            minHouses = Mathf.Min(minHouses, node.NumberOfHouses);
        }
        for (int i = nodesToSellFrom.Count - 1; i >= 0; i--)
        {
            if (nodesToSellFrom[i].NumberOfHouses > minHouses)
            {
                CollectMoney(nodesToSellFrom[i].SellHouseOrHotel());
                houseSold = true;
                break;
            }
        }
        if (!houseSold)
        {
            CollectMoney(nodesToSellFrom[nodesToSellFrom.Count - 1].SellHouseOrHotel());
        }
    }
    internal bool CanAffordHouse(int price)
    {
        if (playerType == PlayerType.AI)
        {
            return (money - aiMoneySavity) >= price;
        }
        //human player
        return money >= price;
    }

    public void ActivateSelector(bool active)
    {
        myInfo.ActiveArrow(active);
    }

    public void AddProperty(MonopolyNode node)
    {
        myMonopolyNodes.Add(node);
        SortPropertiesByPrice();
    }
    public void RemoveProperty(MonopolyNode node)
    {
        myMonopolyNodes.Remove(node);
        SortPropertiesByPrice();
    }

    public void ChanceState(AiStates state)
    {
        if (playerType == PlayerType.Human)
        {
            return;
        }

        aiState = state;
        switch (aiState)
        {
            case AiStates.IDLE:
                {
                    GameManager.instance.Continue();
                }
                break;
            case AiStates.TRADING:
                {
                    TradingSystem.instance.FindMissingProperty(this);
                }
                break;

        }
    }


    public void AddChanceJailFreeCard()
    {
        hasChanceJailFreeCard = true;
    }

    public void AddCommunityJailFreeCard()
    {
        hasCommunityJailFreeCard = true;
    }

    public void UseCommunityJailFreeCard()//2
    {
        if (!IsInJail)
        {
            return;
        }
        hasCommunityJailFreeCard = false;
        SetOutOfJail();
        CommunityChest.instance.AddBackJailFreeCard();
        OnUpdateMessage.Invoke(name + " Uso Jail Free Card");
    }

    public void UseChanceJailFreeCard() //jail1
    {
        if (!IsInJail)
        {
            return;
        }
        SetOutOfJail();
        hasChanceJailFreeCard = false;
        
        ChanceField.instance.AddBackJailFreeCard();
    } 
}
