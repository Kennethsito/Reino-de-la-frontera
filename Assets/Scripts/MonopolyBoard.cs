using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Collections;
using System;
using System.Linq;

public class MonopolyBoard : MonoBehaviour
{

    public static MonopolyBoard instance;
    public List<MonopolyNode> route = new List<MonopolyNode>();

    [System.Serializable]
    public class NodeSet
    {
        public Color setColor = Color.white;
        public List<MonopolyNode> nodesInSetList = new List<MonopolyNode>();
    }

    public List<NodeSet> nodeSetList = new List<NodeSet>();

    void Awake()
    {
        instance = this;
    }

    void OnValidate()
    {
        route.Clear();
        foreach (Transform node in transform.GetComponentInChildren<Transform>())
        {
            route.Add(node.GetComponent<MonopolyNode>());
        }
    }

    

    void OnDrawGizmos()
    {
        if (route.Count > 1)
        {
            for (int i = 0; i < route.Count; i++)
            {
                Vector3 current = route[i].transform.position;
                Vector3 next = (i + 1 < route.Count) ? route[i + 1].transform.position : current;

                Gizmos.color = Color.green;
                Gizmos.DrawLine(current, next);
            }
        }
    }
    public void MovePlayerToken(int steps, Player player)
    {
        StartCoroutine(MovePlayerInSteps(steps, player));
    }

    public void MovePlayerToken(MonopolyNodeType type, Player player)
    {
        int indexOfNextNodeType = -1; // INDEX TO FIND
        int indexOnBoard = route.IndexOf(player.MyMonopolyNode); // WHERE IS THE PLAYER
        int startSearchIndex = (indexOnBoard + 1) % route.Count;
        int nodeSearches = 0; // AMOUNT OF FIELDS SEARCHED

        while (indexOfNextNodeType == -1 && nodeSearches < route.Count) // KEEP SEARCHING
        {
            if (route[startSearchIndex].monopolyNodeType == type) // FOUND THE DESIRED TYPE
            {
                indexOfNextNodeType = startSearchIndex;
            }
            startSearchIndex = (startSearchIndex + 1) % route.Count;
            nodeSearches++;
        }
        if (indexOfNextNodeType == -1) // SECURITY EXIT
        {
            //Debug.LogError("NO NODE FOUND");
            return;
        }
        StartCoroutine(MovePlayerInSteps(nodeSearches, player));
    }
    IEnumerator MovePlayerInSteps(int steps, Player player)
    {
        yield return new WaitForSeconds(0.5f);
        int stepsLeft = steps;
        GameObject tokenToMove = player.MyToken;
        int indexOnBoard = route.IndexOf(player.MyMonopolyNode);
        bool moveOverGo = false;
        bool isMovingFoward = steps > 0;
        if (isMovingFoward)
        {
            while (stepsLeft > 0)
            {
                indexOnBoard++;
                if (indexOnBoard > route.Count - 1)
                {
                    indexOnBoard = 0;
                    moveOverGo = true;
                }
                //Vector3 startPos = tokenToMove.transform.position;
                Vector3 endPos = route[indexOnBoard].transform.position;
                while (moveToNextNode(tokenToMove, endPos, 20))
                {
                    yield return null;
                }
                stepsLeft--;
            }
        }
        else
        {
            while (stepsLeft < 0)
            {
                indexOnBoard--;
                if (indexOnBoard < 0)
                {
                    indexOnBoard = route.Count - 1;
                }
                //Vector3 startPos = tokenToMove.transform.position;
                Vector3 endPos = route[indexOnBoard].transform.position;
                while (moveToNextNode(tokenToMove, endPos, 20))
                {
                    yield return null;
                }
                stepsLeft++;
            }
        }
        
        if (moveOverGo)
        {
            player.CollectMoney(GameManager.instance.GetGoMoney);
        }
        player.setMyCurrentNode(route[indexOnBoard]);


    }
    bool moveToNextNode(GameObject tokenToMove, Vector3 endPos, float speed)
    {
        return endPos != (tokenToMove.transform.position = Vector3.MoveTowards(tokenToMove.transform.position, endPos, speed * Time.deltaTime));
    }
    public (List<MonopolyNode> list, bool allSame) PlayerHasAllNodesOfSet(MonopolyNode node)
    {
        bool allSame = false;
        foreach (var nodeSet in nodeSetList)
        {
            if (nodeSet.nodesInSetList.Contains(node))
            {
                allSame = nodeSet.nodesInSetList.All(_node => _node.Owner == node.Owner);
                return (nodeSet.nodesInSetList, allSame); 
                
                
            }
        }
        return (null, allSame);
    }





}
