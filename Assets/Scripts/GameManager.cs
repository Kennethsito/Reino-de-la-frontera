using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] MonopolyBoard gameBoard;
    [SerializeField] List<Player> playerList = new List<Player>();
    [SerializeField] int currentPlayer;
    [Header("Global Game Settings")]

    [SerializeField] int maxTurnsInJail = 3;
    [SerializeField] int startMoney = 1500;
    [SerializeField] int goMoney = 500;
    [SerializeField] float secondsBetweenTurns = 3;
    [Header("Player Info")]
    [SerializeField] GameObject playerInfoPrefab;
    [SerializeField] Transform playerPanel;
    [SerializeField] List<GameObject> playerTokenList = new List<GameObject>();

    [Header("Game over/win")]
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TMP_Text winnerGameText;
    [Header("Dado")]
    [SerializeField] Dice _dice1;
    [SerializeField] Dice _dice2;
    List<int> rolledDice =new List<int>();
    bool rolledADouble;
    public bool RolledADouble => rolledADouble;
    public void ResetRolledADouble() => rolledADouble = false;
    int doubleRollCount;
    bool hasRolledDice;
    public bool HasRolledDice => hasRolledDice;
    int taxPool = 0;
    public int GetGoMoney => goMoney;
    public float SecondsBetweenTurns => secondsBetweenTurns;
    public List<Player> GetPlayers => playerList;
    public Player GetCurrentPlayer => playerList[currentPlayer];
    //sistema de mensajes
    public delegate void UpdateMessage(string message);
    public static UpdateMessage OnUpdateMessage;
    //human panel
    public delegate void ShowHumanPanel(bool showPanel, bool activateRollDice, bool ActivateEndTurn, bool hasChanceJailCard, bool hasCommunityJailCard);
    public static ShowHumanPanel OnShowHumanPanel;
    //pruebas
    // [SerializeField] bool alwaysDoubleRoll = false;
    // [SerializeField] bool forceDiceRolls;
    // [SerializeField] int dice1;
    // [SerializeField] int dice2;
    void Awake()
    {
        instance = this;

    }

    void Start()
    {
        currentPlayer = Random.Range(0, playerList.Count);
        gameOverPanel.SetActive(false);
        Inititialize();
        CameraSwitcher.instance.SwitchToTopDown();

        StartCoroutine(StartGame());
        OnUpdateMessage.Invoke("Bienvenido al reino de la frontera");
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(3f);
        if(playerList[currentPlayer].playerType == Player.PlayerType.AI)
        {
            //RollDice();
            RollPhysicalDice();
        }
        else
        {
            //SHOW UI FOR HUMAN INPUTS
            OnShowHumanPanel.Invoke(true, true, false, false, false);
        }
    }
    void Inititialize()
    {
        if (GameSettings.settingsList.Count == 0)
        {
            //Debug.LogError("Inicia el juego desde el menu principal");
            return;
        }
        foreach (var setting in GameSettings.settingsList)
        {
            Player p1 = new Player();
            p1.name = setting.playerName;
            p1.playerType = (Player.PlayerType)setting.selectedType;
            playerList.Add(p1);

            GameObject infoObject = Instantiate(playerInfoPrefab, playerPanel, false);
            PlayerInfo info = infoObject.GetComponent<PlayerInfo>();

            GameObject newToken = Instantiate(playerTokenList[setting.selectedColor], gameBoard.route[0].transform.position, Quaternion.identity);
            p1.Initialize(gameBoard.route[0], startMoney, info, newToken);
        }


        // for (int i = 0; i < playerList.Count; i++)
        // {
        //     GameObject infoObject = Instantiate(playerInfoPrefab, playerPanel, false);
        //     PlayerInfo info = infoObject.GetComponent<PlayerInfo>();

        //     //random token
        //     int randomIndex = Random.Range(0, playerTokenList.Count);
        //     GameObject newToken = Instantiate(playerTokenList[randomIndex], gameBoard.route[0].transform.position, Quaternion.identity);
        //     playerList[i].Initialize(gameBoard.route[0], startMoney, info, newToken);

        // }
        playerList[currentPlayer].ActivateSelector(true);
        if (playerList[currentPlayer].playerType == Player.PlayerType.Human)
        {
            bool jail1 = playerList[currentPlayer].HasChanceJailFreeCard;
            bool jail2 = playerList[currentPlayer].HasCommunityJailFreeCard;
            OnShowHumanPanel.Invoke(true, true, false, jail1, jail2);
        }
        else
        {
            bool jail1 = playerList[currentPlayer].HasChanceJailFreeCard;
            bool jail2 = playerList[currentPlayer].HasCommunityJailFreeCard;
            OnShowHumanPanel.Invoke(false, false, false, jail1, jail2);
        }
    }


    public void RollPhysicalDice()
    {
        CheckForJailFree();
        rolledDice.Clear();
        _dice1.RollDice();
        _dice2.RollDice();
        CameraSwitcher.instance.SwitchToDice();

        //humano 
        if (playerList[currentPlayer].playerType == Player.PlayerType.Human)
        {
            bool jail1 = playerList[currentPlayer].HasChanceJailFreeCard;
            bool jail2 = playerList[currentPlayer].HasCommunityJailFreeCard;
            OnShowHumanPanel.Invoke(true, false, false, jail1, jail2);
        }
        
    }

    void CheckForJailFree()
    {
        if (playerList[currentPlayer].IsInJail && playerList[currentPlayer].playerType == Player.PlayerType.AI)
        {
            if (playerList[currentPlayer].HasChanceJailFreeCard)
            {
                playerList[currentPlayer].UseChanceJailFreeCard();
            }
            else if (playerList[currentPlayer].HasCommunityJailFreeCard)
            {
                playerList[currentPlayer].UseCommunityJailFreeCard();
            }
        }
    }

    public void ReportDiceRolled(int diceValue)
    {
        rolledDice.Add(diceValue);
        if (rolledDice.Count == 2)
        {
            RollDice();
        }
    }


    void RollDice()
    {
        bool allowedToMove = true;
        hasRolledDice = true;
        // rolledDice = new int[2];
        // rolledDice[0] = Random.Range(1, 7);
        // rolledDice[1] = Random.Range(1, 7);
        //Debug.Log("Rolled Dice are: " + rolledDice[0] + " & " + rolledDice[1]);


        // //pruebas
        // if (alwaysDoubleRoll)
        // {
        //     rolledDice[0] = 1;
        //     rolledDice[1] = 1;
        // }
        // //rolledDice[0] = 3;
        // //rolledDice[1] = 4;
        // if (forceDiceRolls)
        // {
        //     rolledDice[0] = dice1;
        //     rolledDice[1] = dice2;
        // }


        rolledADouble = rolledDice[0] == rolledDice[1];

        if (playerList[currentPlayer].IsInJail)
        {
            playerList[currentPlayer].IncreaseNumTurnsInJail();


            if (rolledADouble)
            {
                playerList[currentPlayer].SetOutOfJail();
                OnUpdateMessage.Invoke(playerList[currentPlayer].name + "Sacaste dobles, puedes salir de la carcel");
                doubleRollCount++;
                //mover al jugador

            }
            else if (playerList[currentPlayer].NumTurnsInJail >= maxTurnsInJail)
            {
                playerList[currentPlayer].SetOutOfJail();
                OnUpdateMessage.Invoke(playerList[currentPlayer].name + "Eres libre");

            }
            else
            {
                allowedToMove = false;
            }
        }
        else
        {

            if (!rolledADouble)
            {
                doubleRollCount = 0;
            }
            else
            {
                doubleRollCount++;
                if (doubleRollCount >= 3)
                {
                    //mover a la carcel
                    int indexOnBoard = MonopolyBoard.instance.route.IndexOf(playerList[currentPlayer].MyMonopolyNode);
                    playerList[currentPlayer].GoToJail(indexOnBoard);
                    OnUpdateMessage.Invoke(playerList[currentPlayer].name + " <b>te vas a la carcel <color=red>cabron</color></b> ");
                    rolledADouble = false;
                    return;
                }
            }
        }




        if (allowedToMove)
        {
            OnUpdateMessage.Invoke(playerList[currentPlayer].name + " Sacaste " + rolledDice[0] + " & " + rolledDice[1]);
            StartCoroutine(DelayBeforeMove(rolledDice[0] + rolledDice[1]));

        }
        else
        {

            OnUpdateMessage.Invoke(playerList[currentPlayer].name + " Sigue en la carcel ");
            StartCoroutine(DelayBetweenSwitchPlayer());
        }
    }
    IEnumerator DelayBeforeMove(int rolledDice)
    {
        CameraSwitcher.instance.SwitchToPlayer(playerList[currentPlayer].MyToken.transform);
        yield return new WaitForSeconds(secondsBetweenTurns);
        gameBoard.MovePlayerToken(rolledDice, playerList[currentPlayer]);
    }
    IEnumerator DelayBetweenSwitchPlayer()
    {
        
        yield return new WaitForSeconds(secondsBetweenTurns);
        SwitchPlayer();
    }
    public void SwitchPlayer()
    {
        CameraSwitcher.instance.SwitchToTopDown();
        currentPlayer++;
        hasRolledDice = false;

        doubleRollCount = 0; // Reset double roll count after switching players
        if (currentPlayer >= playerList.Count)
        {
            currentPlayer = 0;
        }
        DeactivateArrows();
        playerList[currentPlayer].ActivateSelector(true);

        if (playerList[currentPlayer].playerType == Player.PlayerType.AI)
        {
           // RollDice();
            RollPhysicalDice();
            OnShowHumanPanel.Invoke(false, false, false, false, false);
        }
        else
        {
            bool jail1 = playerList[currentPlayer].HasChanceJailFreeCard;
            bool jail2 = playerList[currentPlayer].HasCommunityJailFreeCard;
            OnShowHumanPanel.Invoke(true, true, false, jail1, jail2);
        }

        //human

    }

    public List<int> LastRolledDice => rolledDice;

    public void AddTaxToPool(int amount)
    {
        taxPool += amount;
    }

    public int GetTaxPool()
    {
        int currentTaxCollected = taxPool;
        taxPool = 0; // Reset the pool after collecting
        return currentTaxCollected;
    }

    public void RemovePlayer(Player player)
    {
        playerList.Remove(player);
        CheckForGameOver();
    }

    void CheckForGameOver()
    {
        if (playerList.Count == 1)
        {
            //Debug.Log(playerList[0].name + " es el ganador ");
            OnUpdateMessage.Invoke(playerList[0].name + " es el ganador del juego!");


            gameOverPanel.SetActive(true);
            winnerGameText.text = playerList[0].name;

        }
    }
    void DeactivateArrows()
    {
        foreach (var player in playerList)
        {
            player.ActivateSelector(false);
        }
    }


    public void Continue()
    {
        if (playerList.Count > 1)
        {
            Invoke("ContinueGame", secondsBetweenTurns);
        }
    }
    void ContinueGame()
    {
        if (RolledADouble)
        {
            //RollDice();
            RollPhysicalDice();

        }
        else
        {
            if (playerList.Count > 1)
            {
                SwitchPlayer();
            }

        }
    }


    public void HumanBankrup()
    {
        playerList[currentPlayer].Bankrupt();

    }

    public void UseJail1Card()//chance card
    {
        playerList[currentPlayer].UseChanceJailFreeCard();
    }
    
    public void UseJail2Card()//community card
    {
        playerList[currentPlayer].UseCommunityJailFreeCard();
        
    }
}


