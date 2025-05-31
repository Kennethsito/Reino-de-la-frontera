using UnityEngine;
using System.Collections.Generic;


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
    int numTurnsInJail;

    [SerializeField] GameObject myToken;
    [SerializeField] List<MonopolyNode> myMonopolyNodes = new List<MonopolyNode>();

    //informacion del jugador

    //ia
    int aiMoneySavity = 200;

    PlayerInfo myInfo;
    // Retorno de informacion

    public bool IsInJail => isInJail;
    public GameObject MyToken => myToken;
    public MonopolyNode MyMonopolyNode => currentnode;

    public void Initialize(MonopolyNode startNode, int startMoney, PlayerInfo info)
    {
        currentnode = startNode;
        money = startMoney;
        myInfo = info;
        myInfo.SetPlayerNameAndCash(name, money);
    }


}
