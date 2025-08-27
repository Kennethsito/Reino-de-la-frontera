using UnityEngine;
using TMPro;

public class TradePlayerButton : MonoBehaviour
{
    Player playerReference;
    [SerializeField] TMP_Text PlayerNameText;
    public void SetPlayer(Player player)
    {
        playerReference = player;
        PlayerNameText.text = player.name;
    }

    public void SelectPlayer()
    {
        TradingSystem.instance.ShowRightPlayer(playerReference);
    }
}
