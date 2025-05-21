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
    

    public bool IsInJail => isInJail;
    public GameObject MyToken => myToken;
    public MonopolyNode MyMonopolyNode => currentnode;














    //ia
    int aiMoneySavity = 200;


}
