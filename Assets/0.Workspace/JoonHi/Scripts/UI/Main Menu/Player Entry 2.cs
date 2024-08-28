using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public class PlayerEntry2 : MonoBehaviour
{
    [Header("Player Info")]
    [SerializeField] Image playerCharacterImage;
    [SerializeField] TMP_Text playerName;
    public Button playerReadyButton;

    private Player player;
    public Player Player { get { return player; } }

    public void SetPlayer(Player player)
    {
        this.player = player;
        playerName.text = player.NickName;
        playerName.color = player.GetReady() ? Color.green : Color.white;
        if (player.IsMasterClient)
            playerName.color = Color.yellow;
        if (!player.IsMasterClient && player.IsLocal)
        {
            playerReadyButton.gameObject.SetActive(true);
            playerReadyButton.onClick.AddListener(Ready);
        }
        if(player.IsMasterClient)
        {
            player.SetReady(true);
        }
    }

    public void Ready()
    {
        bool ready = player.GetReady();
        player.SetReady(!ready);
    }

    public void ChangeCustomProperty(PhotonHashTable property)
    {
        bool ready = player.GetReady();
        playerName.color = ready ? Color.green : Color.white;
        if (player.IsMasterClient)
            playerName.color = Color.yellow;
    }
}
