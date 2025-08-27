using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine.UI;

public class CommunityChest : MonoBehaviour
{
    public static CommunityChest instance;
    [SerializeField] List<SRC_CommunityCard> cards = new List<SRC_CommunityCard>();
    [SerializeField] TMP_Text cardText;
    [SerializeField] GameObject cardHolderBackground;

    [SerializeField] float showTime = 3; // HIDE CARD AUTOMATIC AFTER 3 Seconds
    [SerializeField] Button closeCardButton;
    List<SRC_CommunityCard> cardPool = new List<SRC_CommunityCard>();
    List<SRC_CommunityCard> usedCardPool = new List<SRC_CommunityCard>();

    SRC_CommunityCard jailFreeCard;
    //carta actual y jugador que la agarro
    SRC_CommunityCard pickedCard;
    Player currentPlayer;
    //human panel
    public delegate void ShowHumanPanel(bool showPanel, bool activateRollDice, bool ActivateEndTurn, bool hasChanceJailCard, bool hasCommunityJailCard);
    public static ShowHumanPanel OnShowHumanPanel;

    void OnEnable()
    {
        MonopolyNode.OnDrawCommunityCard += DrawCard;
    }

    void OnDisable()
    {
        MonopolyNode.OnDrawCommunityCard -= DrawCard;
    }

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        cardHolderBackground.SetActive(false);
        cardPool.AddRange(cards);
        ShuffleCards();
    }

    void ShuffleCards()
    {
        for (int i = 0; i < cardPool.Count; i++)
        {
            int index = UnityEngine.Random.Range(0, cardPool.Count);
            SRC_CommunityCard tempCard = cardPool[index];
            cardPool[index] = cardPool[i];
            cardPool[i] = tempCard;
        }

    }

    void DrawCard(Player cardTaken)
    {
        //agarrar carta we
        pickedCard = cardPool[0];
        cardPool.RemoveAt(0);

        if (pickedCard.jailFreeCard)
        {
            jailFreeCard = pickedCard;

        }
        else
        {
            usedCardPool.Add(pickedCard);
        }

        if (cardPool.Count == 0)
        {
            cardPool.AddRange(usedCardPool);
            usedCardPool.Clear();
            ShuffleCards();
        }
        currentPlayer = cardTaken;

        //ver carta
        cardHolderBackground.SetActive(true);

        //texto de la carta
        cardText.text = pickedCard.textOnCard;

        //boton weyes
        if (currentPlayer.playerType == Player.PlayerType.AI)
        {
            closeCardButton.interactable = false;
            Invoke("ApplyCardEffect", showTime);
        }
        else
        {
            closeCardButton.interactable = true;
        }

    }

    public void ApplyCardEffect()
    {
        bool isMoving = false;
        if (pickedCard.rewardMoney != 0 && !pickedCard.collectFromPlayer)
        {
            currentPlayer.CollectMoney(pickedCard.rewardMoney);
        }
        else if (pickedCard.penaltyMoney != 0)
        {
            currentPlayer.PayMoney(pickedCard.penaltyMoney);
        }
        else if (pickedCard.moveToBoardIndex != -1)
        {
            isMoving = true;
            //casillas para la del goal

            int currentIndex = MonopolyBoard.instance.route.IndexOf(currentPlayer.MyMonopolyNode);
            int lengthOfBoard = MonopolyBoard.instance.route.Count;
            int stepsToMove = 0;
            if (currentIndex < pickedCard.moveToBoardIndex)
            {
                stepsToMove = pickedCard.moveToBoardIndex - currentIndex;
            }
            else if (currentIndex > pickedCard.moveToBoardIndex)
            {
                stepsToMove = lengthOfBoard - currentIndex + pickedCard.moveToBoardIndex;
            }


            ////empezar a mover al jugador   
            MonopolyBoard.instance.MovePlayerToken(stepsToMove, currentPlayer);
        }
        else if (pickedCard.collectFromPlayer)
        {
            int totalCollected = 0;
            List<Player> allPlayers = GameManager.instance.GetPlayers;

            foreach (var player in allPlayers)
            {
                if (player != currentPlayer)
                {
                    //no bancarrota
                    int amount = Mathf.Min(player.ReadMoney, pickedCard.rewardMoney);
                    player.PayMoney(amount);
                    totalCollected += amount;

                }
            }
            currentPlayer.CollectMoney(totalCollected);
        }
        else if (pickedCard.streetRepairs)
        {
            int[] allBuildings = currentPlayer.CountHousesAndHotels();
            int totalCosts = pickedCard.streetRepairsHousePrice * allBuildings[0] + pickedCard.streetRepairsHotelPrice * allBuildings[1];
            currentPlayer.PayMoney(totalCosts);
        }
        else if (pickedCard.goToJail)
        {

            //mandar al jugador a la carcel
            currentPlayer.GoToJail(MonopolyBoard.instance.route.IndexOf(currentPlayer.MyMonopolyNode));
            isMoving = true;
        }
        else if (pickedCard.jailFreeCard)
        {
            currentPlayer.AddCommunityJailFreeCard();
        }
        cardHolderBackground.SetActive(false);
        ContinueGame(isMoving);

    }

    void ContinueGame(bool isMoving)
    {
        Debug.Log(isMoving);
        if (currentPlayer.playerType == Player.PlayerType.AI)
        {
            if (!isMoving)
            {
                GameManager.instance.Continue();
            }
        }
        else //parte de jugador humano
        {
            if (isMoving)
            {
                bool jail1 = currentPlayer.HasChanceJailFreeCard;
                bool jail2 = currentPlayer.HasCommunityJailFreeCard;
                OnShowHumanPanel.Invoke(true, GameManager.instance.RolledADouble, !GameManager.instance.RolledADouble,jail1,jail2);
            }
        }




    }
    
    public void AddBackJailFreeCard()
    {
        usedCardPool.Add(jailFreeCard);
        jailFreeCard = null;
    }


}
