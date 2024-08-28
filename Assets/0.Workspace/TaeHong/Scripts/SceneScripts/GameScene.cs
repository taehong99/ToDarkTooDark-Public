using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;
using TMPro;
using MapGenerator;
using Firebase.Database;
using Firebase.Extensions;

public class GameScene : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] MapGenerator.MapGenerator mapGenerator;
    [SerializeField] GameObject loadingScreen;
    [SerializeField] TextMeshProUGUI loadCountText;
    [SerializeField] PlayerUI playerUI;
    [SerializeField] SkillsUI skillsUI;
    [SerializeField] GameOverUI gameOverUI;
    private bool loadComplete;
    private bool mapGenerateComplete;

    private const int SOLO_WIN_EXCALIBURS = 3;
    private const int TEAM_WIN_EXCALIBURS = 1;
    private const int SOLO_WIN_TOKENS = 900;
    private const int TEAM_WIN_TOKENS = 300;

    private void Awake()
    {
        Manager.Game.PhaseManager = FindObjectOfType<GamePhaseManager>();
    }

    private void Start()
    {
        StartCoroutine(LoadRoutine());
        // Subscribe to door open event to see which doors were Interacted with
        Manager.Event.DoorInteractionEvent += OnDoorInteraction;
        Manager.Event.ObjectBreakEvent += OnObjectBreak;
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDestroy()
    {
        Manager.Event.DoorInteractionEvent -= OnDoorInteraction;
        Manager.Event.ObjectBreakEvent -= OnObjectBreak;
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if(eventCode == EventManager.GameOverEventCode)
        {
            object[] data = (object[]) photonEvent.CustomData;
            int soloWinner = (int)data[0];
            Team winningTeam = (Team)data[1];

            if (PhotonNetwork.CurrentRoom.GetGameMode()) // 개인전
            {
                // 승리
                if (PhotonNetwork.LocalPlayer.ActorNumber == soloWinner)
                {
                    gameOverUI.Init(SOLO_WIN_EXCALIBURS, 0, true);
                    GetPrize(SOLO_WIN_EXCALIBURS, 0);
                }
                // 패배
                else
                {
                    gameOverUI.Init(0, SOLO_WIN_TOKENS, false);
                    GetPrize(0, SOLO_WIN_TOKENS);
                }
            }
            else // 팀전
            {
                // 승리
                if (PhotonNetwork.LocalPlayer.GetTeam() == winningTeam)
                {
                    gameOverUI.Init(TEAM_WIN_EXCALIBURS, 0, true);
                    GetPrize(TEAM_WIN_EXCALIBURS, 0);
                }
                // 패배
                else
                {
                    gameOverUI.Init(0, TEAM_WIN_TOKENS, false);
                    GetPrize(0, TEAM_WIN_TOKENS);
                }
            }

            Manager.UI.ShowPopUpUI(gameOverUI);
        }
    }

    public override void OnLeftRoom()
    {
        Manager.Pool.ClearPool();
        Manager.Game.playerDic.Clear();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
    {
        if (changedProps.ContainsKey(CustomProperty.LOAD))
        {
            loadCountText.text = $"{PlayerLoadCount()} / {PhotonNetwork.PlayerList.Length}";
            if (PlayerLoadCount() == PhotonNetwork.PlayerList.Length)
            {
                GameStart();
            }
        }
    }

    private int PlayerLoadCount()
    {
        int loadCount = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetLoad())
            {
                loadCount++;
            }
        }
        return loadCount;
    }


    private IEnumerator LoadRoutine()
    {
        // Show Loading Screen
        loadingScreen.SetActive(true);

        while (!loadComplete)
        {
            // Generate Map + Spawn Player
            mapGenerator.InitSeed();
            mapGenerator.GenerateMap();

            while (!mapGenerateComplete)
            {
                yield return null;
            }
        }

        StartGame();
    }

    private void StartGame()
    {
        Debug.Log($"My Actor Num : {PhotonNetwork.LocalPlayer.ActorNumber}");
        if (PhotonNetwork.InRoom)
        {
            Debug.Log($"Player Count : {PhotonNetwork.CurrentRoom.PlayerCount}");
            Debug.Log($"My Exit Num : {PhotonNetwork.LocalPlayer.ActorNumber + PhotonNetwork.CurrentRoom.PlayerCount - 1}");
        }

        Manager.Game.GameMode = PhotonNetwork.CurrentRoom.GetGameMode();
        playerUI.Init();
        skillsUI.Init();
        Manager.Event.SpawnExcalibur();
        PhotonNetwork.LocalPlayer.SetLoad(true);
        Manager.Game.PhaseManager.StartTimer();
        Manager.Sound.PlaySFX(Manager.Sound.SoundSO.GameStartSFX);
        Manager.Sound.PlayBGM(Manager.Sound.SoundSO.StartBGM);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        mapGenerator.FinishedGeneratingEvent += OnMapFinishedGenerating;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        mapGenerator.FinishedGeneratingEvent -= OnMapFinishedGenerating;
    }

    private void OnMapFinishedGenerating()
    {
        mapGenerateComplete = true;
        loadComplete = true;
    }

    private void GameStart()
    {
        loadingScreen.SetActive(false);
    }

    private void OnDoorInteraction(int doorID)
    {
        photonView.RPC("DoorInteract", RpcTarget.All, doorID);
    }

    [PunRPC]
    public void DoorInteract(int doorID)
    {
        MapManager.Instance.doorDic[doorID].InteractDoor();
    }

    public void OnObjectBreak(Vector3 pos)
    {
        photonView.RPC("SyncObjectBreak", RpcTarget.All, pos);
    }

    [PunRPC]
    public void SyncObjectBreak(Vector3 pos)
    {
        if (MapManager.Instance.ObjectDic.TryGetValue(pos, out GameObject obj))
        {
            if (obj.TryGetComponent(out BreakableObject breakableObject))
                breakableObject.SyncBreak();
        }
    }

    private void GetPrize(int excaliver, int token)
    {
        Userdata userData;
        string userID = FirebaseManager.Auth.CurrentUser.UserId;
        FirebaseManager.DB
            .GetReference("Userdata")
            .Child(userID)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("취소");
                    return;
                }
                else if (task.IsFaulted)
                {
                    Debug.Log("오류");
                    return;
                }
                DataSnapshot snapshot = task.Result;
                string json = snapshot.GetRawJsonValue();

                userData = JsonUtility.FromJson<Userdata>(json);

                Debug.Log("Player Data Load Complete");

                userData.excaliver += excaliver;
                userData.token += token;

                SavePrize(userID, userData);
            }
            );
    }

    private void SavePrize(string userId, Userdata userData)
    {
        string json = JsonUtility.ToJson(userData);
        FirebaseManager.DB
            .GetReference("Userdata")
            .Child(userId)
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    return;
                }
                else if (task.IsFaulted)
                {
                    return;
                }
                Debug.Log("Prize saved in firebase");
            }
            );
    }
}
