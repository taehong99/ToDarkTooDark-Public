using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public class PlayerEntry : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] Image jobPortrait;

    [Header("Sprites")]
    [SerializeField] Sprite swordsmanSprite;
    [SerializeField] Sprite archerSprite;
    [SerializeField] Sprite priestSprite;
    [SerializeField] Sprite wizardSprite;

    public Button playerReadyButton;

    private Player player = null;
    public Player Player { get { return player; } private set { player = value; } }

    public void SetPlayer(Player player, Team team)
    {
        if (player == null)
            return;

        Player = player;
        playerName.text = player.NickName;
        UpdateSprite();
        gameObject.SetActive(true);
    }

    public void RemovePlayer()
    {
        playerName.color = Color.white;
        gameObject.SetActive(false);
    }

    public void UpdateSprite()
    {
        switch (Player?.GetJob())
        {
            case PlayerJob.Swordsman:
                jobPortrait.sprite = swordsmanSprite;
                break;
            case PlayerJob.Archer:
                jobPortrait.sprite = archerSprite;
                break;
            case PlayerJob.Priest:
                jobPortrait.sprite = priestSprite;
                break;
            case PlayerJob.Wizard:
                jobPortrait.sprite = wizardSprite;
                break;
        }
    }

    public void UpdateNameColor()
    {
        playerName.color = Player.GetReady() ? Color.green : Color.white;
    }
}
