using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : PopUpUI
{
    [SerializeField] Text resultText;
    [SerializeField] Text excaliburCount;
    [SerializeField] Text goldCount;
    [SerializeField] Button exitButton;

    private void Awake()
    {
        base.Awake();
        exitButton.onClick.AddListener(ExitGame);
    }

    public void Init(int exc, int gold, bool win)
    {
        if (win)
            resultText.text = "승리하셨습니다! 축하드립니다!";
        else
            resultText.text = "다음에는 높은 성적을 노려보죠!";

        excaliburCount.text = exc.ToString();
        goldCount.text = gold.ToString();
    }

    public void ExitGame()
    {
        Manager.UI.ClosePopUpUI();
        PhotonNetwork.LeaveRoom();
    }
}
